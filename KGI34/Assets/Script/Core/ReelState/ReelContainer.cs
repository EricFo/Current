namespace SlotGame.Core.Reel {
    using System;
    using UnityEngine;        
    using SlotGame.Config;
    using SlotGame.Reel.Args;
    using SlotGame.Reel.Setting;

    [RequireComponent(typeof(ReelSetting))]
    public class ReelContainer : MonoBehaviour {
        /// <summary>
        /// 移动前的等待次数
        /// </summary>
        [Range(1, 1000)]
        [SerializeField] private int waitStep = 1;
        /// <summary>
        /// Hyper的移动次数
        /// </summary>
        [Range(1, 1000)]
        [SerializeField] private int hyperStep = 1;
        /// <summary>
        /// Hyper移动速度
        /// </summary>
        [SerializeField] private float hyperSpeed = 0.04f;
        /// <summary>
        /// 是否激活HyperSpin
        /// </summary>
        [SerializeField] private bool isEnableHyperSpin = true;
        /// <summary>
        /// 是否应用Reel设置
        /// </summary>
        [SerializeField] private bool isApplyReelSetting = true;

        protected int[] steps;
        protected int stopCount;
        protected IReelState[] reels;
        protected ReelSetting setting;

        #region 属性
        /// <summary>
        /// Reel是否在旋转中
        /// </summary>
        public bool isRolling { get; protected set; }
        /// <summary>
        /// 是否需要终止状态机
        /// </summary>
        public bool isShutDown { get; protected set; }
        /// <summary>
        /// 提供外部访问所有Reel的属性
        /// </summary>
        public IReelState[] Reels { get { return reels; } }
        /// <summary>
        /// 在移动之前需要等待多少步，有的时候不是每个转轮都同时转
        /// </summary>
        protected int WaitStep { get { return waitStep; } }
        /// <summary>
        /// HyperSpin旋转多少步（加速转）
        /// </summary>
        protected int HyperStep { get { return hyperStep; } }
        /// <summary>
        /// HyperSpin的旋转速度
        /// </summary>
        protected float HyperSpeed { get { return hyperSpeed; } }
        /// <summary>
        /// 是否可以激活HyperSpin
        /// </summary>
        protected bool IsEnableHyperSpin { get { return isEnableHyperSpin; } }
        /// <summary>
        /// 触发HyperSpin时，必须等到他前一列Reel停止才能开始，因此需要增加一定的旋转时间
        /// </summary>
        protected int HyperWaitID { get; set; }
        /// <summary>
        /// 这一轮停在屏幕上的最大Scatter数量，可用于判断是否触发Feature(Free\Bonus等)
        /// </summary>
        protected int MaxScatterCount { get; set; } 
        /// <summary>
        /// Scatter是否有效，假设游戏1，3，5列可以出现Scatter,有几种情况不能播放Scatter动画和音效：
        /// 1、第一列没有，3、5列都不播放
        /// 2、第一列有，第三列没有，第一列要播放，第五列不播放
        /// 大原则就是如果Scatter停下后已经确定无法触发Feature了，那就不需要播放动画和音效
        /// </summary>
        protected bool ScatterIsValid { get; set; }
        #endregion

        public virtual event Action<IReelState> OnSpinReelListener;
        public virtual event Action<IReelState> OnStopReelListener;
        public virtual event Action<ReelContainer> OnSpinAllListener;
        public virtual event Action<ReelContainer> OnStopAllListener;

        #region 公共访问接口
        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Initialize() {
            isRolling = false;
            isShutDown = false;
            setting = GetComponent<SlotGame.Reel.Setting.ReelSetting>();
            reels = GetComponentsInChildren<IReelState>();
            steps = new int[reels.Length];
            for (int i = 0; i < reels.Length; i++) {
                reels[i].OnStopListener += OnReelStop;
                reels[i].Initialize(isApplyReelSetting ? ApplyReelSetting(i) : null);
            }
            foreach (var reel in reels)
            {
                reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
            }
            DisplayTheInvisibleSymbols(false);
        }
        /// <summary>
        /// 所有Reel开始Spin
        /// </summary>
        public virtual void Spin() {
            foreach (var reel in reels)
            {
                reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
            }
            if (isRolling == false) {
                OnReset();
                isRolling = true;
                OnSpinAllListener?.Invoke(this);
                for (int i = 0; i < reels.Length; i++) {
                    reels[i].Spin(Predict(i));
                    OnSpinReelListener?.Invoke(reels[i]);
                }
            }
        }
        /// <summary>
        /// 手动停止Spin
        /// </summary>
        /// <returns>如果有一个Reel成功Shutdown，则说明本轮是手动结束，返回True,否则是自动结束，返回False</returns>
        public virtual bool ShutDown() {
            if (isShutDown == false) {
                for (int i = 0; i < reels.Length; i++) {
                    bool success = reels[i].ShutDown();
                    if (success) {
                        isShutDown = true;
                    }
                }
            }
            return isShutDown;
        }
        /// <summary>
        /// 重置相关设置
        /// </summary>
        public virtual void OnReset() {
            stopCount = 0;
            HyperWaitID = 0;
            MaxScatterCount = 0;

            isShutDown = false;
            ScatterIsValid = false;
        }
        /// <summary>
        /// 隐藏不可见Symbol
        /// </summary>
        public void DisplayTheInvisibleSymbols(bool isDisplay) {
            foreach (var item in reels) {
                item.DisplayTheInvisibleSymbol(isDisplay);
            }
        }
        #endregion

        #region 辅助工具
        /// <summary>
        /// 应用Reel设置
        /// </summary>
        protected ReelSettingArgs ApplyReelSetting(int id) {
            ReelSettingArgs args = new ReelSettingArgs() {
                reelID = id,
                moveStep = setting.GetMoveStep(id),
                moveSpeed = setting.GetMoveSpeed(id),
                beginCurve = setting.GetBeginCurve(id),
                finishCurve = setting.GetFinishCurve(id),
                reverseCurve = setting.GetReverseCurve(id),
                symbolCount = setting.GetSymbolCount(id),
                visibleSymbolCount = setting.GetVisibleSymbolCount(id),
                rowCount = setting.reelAreaSetting.RowCount,
                colCount = setting.reelAreaSetting.ColCount,
                moveDistance = setting.reelSetting.MoveDistance,
                defaultLayer = setting.symbolSetting.DefaultLayer,
                reelStripes = GetReelStripe(id),
            };
            return args;
        }
        /// <summary>
        /// 提前预测结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual ReelSpinArgs Predict(int id) {
            //预算结果
            bool isScatter = false;
            ReelSpinArgs args = new ReelSpinArgs();

            args.resultSymbols = GetReelResult(id);
            //查看是否存在Scatter
            for (int i = 1; i < args.resultSymbols.Length - 2; i++) {
                if (args.resultSymbols[i].Equals(SymbolNames.SCATTER)) {
                    isScatter = true;
                }
            }

            //检查Scatter是否有效
            ScatterIsValid = CheckScatter(id, isScatter);

            if (IsEnableHyperSpin) {
                //判断是否要启用Hyper
                if (MaxScatterCount >= ConfigManager.featureGameConfig.Trigger - 1) {
                    int wait = HyperWaitID++ * waitStep;
                    args.hyperStep = HyperStep + wait;
                    args.hyperSpeed = HyperSpeed;
                    args.WaitStep = wait;
                    args.isHyper = true;
                }
            }

            if (isScatter) MaxScatterCount++;

            return args;
        }
        /// <summary>
        /// 获取Reel最终结果，包含隐藏部分
        /// </summary>
        /// <param name="reelID"></param>
        /// <param name="stopID"></param>
        /// <returns></returns>
        protected virtual string[] GetReelResult(int reelID) {
            int count = reels[reelID].Setting.symbolCount - 3;
            steps[reelID] = UnityEngine.Random.Range(0, reels[reelID].Setting.reelStripes.Length);

            string[] result = new string[count];
            for (int i = 0; i < reels[reelID].Setting.symbolCount; i++) {
                steps[reelID] %= reels[reelID].Setting.reelStripes.Length;
                if (i > 0 && i < reels[reelID].Setting.symbolCount - 2) {
                    count--;
                    result[count] = reels[reelID].Setting.reelStripes[steps[reelID]];
                }
                
                steps[reelID]++;
            }
            return result;
        }
        /// <summary>
        /// 获取转盘Stripe信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual string[] GetReelStripe(int id) {
            return default;
        }
        /// <summary>
        /// 根据转轮ID和转轮是否命中Scatter确定本轮Scatter是否有效
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isScatter"></param>
        /// <returns></returns>
        protected virtual bool CheckScatter(int id, bool isScatter) {
            bool isValid = false;
            //检查Scatter是否有效,满足触发条件的情况下都是有效的
            if (ConfigManager.reelStripConfig.ScatterPositions[id] > 0 && isScatter) {
                isValid = true;
            }
            //最后一列可以完全确定有几个有效Scatter
            if (id == ConfigManager.reelStripConfig.ScatterPositions.Length - 1) {
                if (MaxScatterCount < ConfigManager.featureGameConfig.Trigger - 1) {
                    isValid = false;
                }
            }
            return isValid;
        }
        #endregion

        #region 行为事件
        /// <summary>
        /// 每个转盘停止的事件
        /// </summary>
        /// <param name="reel"></param>
        protected virtual void OnReelStop(ReelBase reel) {
            reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
            stopCount++;
            OnStopReelListener?.Invoke(reel);
            if (stopCount >= reels.Length) {
                isRolling = false;
                OnStopAllListener?.Invoke(this);
            }
        }
        #endregion
    }
}
