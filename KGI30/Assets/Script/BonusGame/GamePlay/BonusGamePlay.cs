using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Result;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public abstract class BonusGamePlay
{
    /// <summary>
    /// 钻石分数加的间隔时间
    /// </summary>
    public float FeatureIntervalTime;
    /// <summary>
    /// 转轮结果序列
    /// </summary>
    public int[][] wheelOrder = new int[0][];
    /// <summary>
    /// 所有boost所在的转盘位置索引
    /// </summary>
    public int[] boostIndex = new int[6] { 1, 3, 5, 7, 9, 11 };
    /// <summary>
    /// 所有赔付分数所在的转盘位置索引
    /// </summary>
    public int[] scoreIndex = new int[4] { 2, 4, 6, 10 };

    public const int GRAND = 0;
    public const int MAJOR = 8;
    public const int MINI = 4;

    /// <summary>
    /// 无效Feature所在的ReelID,不能出现Coin
    /// </summary>
    public List<int> InvalidFeatureReelId = new List<int>();
    /// <summary>
    /// 有效Feature所在的ReelID,不能出现无效Feature
    /// </summary>
    public List<int> ValidFeatureReelId = new List<int>();

    /// <summary>
    /// 需要在之后出的Coin,键为出现次序，值为出现ID
    /// </summary>
    public Dictionary<int, int> FeatureCoinReelId = new Dictionary<int, int>();

    /// <summary>
    /// 触发特殊玩法的事件
    /// </summary>
    public event EventHandler OnFeaturePlay;
    /// <summary>
    /// 触发Jackpot玩法的事件
    /// </summary>
    public event EventHandler OnJackpotPlay;
    /// <summary>
    /// 触发直接赔付分值的事件
    /// </summary>
    public event EventHandler OnScoreRewardPlay;
    public event Action<BonusGamePlay> OnStopListener;
    /// <summary>
    /// 转轮是否停止
    /// </summary>
    public bool IsStop;
    /// <summary>
    /// 触发事件时转轮位置索引所对应的参数
    /// </summary>
    public Dictionary<int, int> ResultArgs = new Dictionary<int, int>()
    {
        {1,2 },{ 2, 50 },{3,3 },{ 4, 0 },{5,4 },{6,80 },{7,5 },{9,6 },{10,50 },{11,7 }
    };
    /// <summary>
    /// 转轮停下时位置索引所对应的事件
    /// </summary>
    public Dictionary<int, EventHandler> ResultOptions = new Dictionary<int, EventHandler>();
    /// <summary>
    /// 当前玩法所绑定的实际转轮表现
    /// </summary>
    protected Wheel turntableWheel = null;
    /// <summary>
    /// 星星上飞的初始位置
    /// </summary>
    public Transform initFlyPos = null;
    /// <summary>
    /// 如果本轮有JP停下需要延迟的时间
    /// </summary>
    public float jpFlyTime = 0;
    /// <summary>
    /// 本轮已经上飞过了
    /// </summary>
    public bool isFly = false;

    public int jpType = -1;

    public int result = -1;

    protected bool isFeatureShutDown = false;
    protected bool isShutDownAudio = false;

    public BonusGamePlay() { }
    private BonusReelContainer _container;
    public BonusReelContainer Container { get { return _container; } set { _container = value; } }

    public BonusGamePlay(BonusReelContainer reelContainer,Wheel turntable/*, int[][] wheelorder*/) {
        //this.wheelOrder = wheelorder;
        Container = reelContainer;
        turntableWheel = turntable;
        turntableWheel.OnStatusChange -= OnWheelStatusChange;
        turntableWheel.OnStatusChange += OnWheelStatusChange;


        OnFeaturePlay -= FeaturePlay;
        OnFeaturePlay += FeaturePlay;
        OnJackpotPlay -= JackpotPlay;
        OnJackpotPlay += JackpotPlay;
        OnScoreRewardPlay -= ScoreRewardPlay;
        OnScoreRewardPlay += ScoreRewardPlay;
        ResultOptions.Add(0, OnJackpotPlay);
        ResultOptions.Add(1, OnFeaturePlay);
        ResultOptions.Add(2, OnScoreRewardPlay);
        ResultOptions.Add(3, OnFeaturePlay);
        ResultOptions.Add(4, OnJackpotPlay);
        ResultOptions.Add(5, OnFeaturePlay);
        ResultOptions.Add(6, OnScoreRewardPlay);
        ResultOptions.Add(7, OnFeaturePlay);
        ResultOptions.Add(8, OnJackpotPlay);
        ResultOptions.Add(9, OnFeaturePlay);
        ResultOptions.Add(10, OnScoreRewardPlay);
        ResultOptions.Add(11, OnFeaturePlay);
    }


    protected void OnWheelStatusChange(object sender, StatusChangeEventArgs args)
    {  
        if (args.To == Wheel.Status.Start)
        {
            Container.wheelStopCount = 0;
        }
        if (args.To == Wheel.Status.Rolling)
        {
            UIController.BottomPanel.StopBtn.interactable = true;
        }
        if (args.To == Wheel.Status.Stop)
        {
            isFly = false;
            jpFlyTime = 0;
            jpType = -1;
            UIController.BottomPanel.StopBtn.interactable = false;
            int stopPos = turntableWheel.CurrentCellIndex();
            ResultOptions[stopPos].Invoke(this, EventArgs.Empty);
            if (stopPos.Equals(GRAND) || stopPos.Equals(MAJOR) || stopPos.Equals(MINI)) {
                jpFlyTime = 1f;
                OnStopInvoke();
            } else {
                OnStopInvoke();
            }
        }
    }
   
    /// <summary>
    /// 每次进入Bonus时所要做的初始化
    /// </summary>
    public virtual void EnterBonus()
    {
        InvalidFeatureReelId.Clear();
        ValidFeatureReelId.Clear();
        FeatureCoinReelId.Clear();
    }
    /// <summary>
    /// 按下SPIN后的事件
    /// </summary>
    public abstract void OnReelSpin();
    /// <summary>
    /// 当上一个转轮流程停止时的事件
    /// </summary>
    public abstract void OnReelStop();
    /// <summary>
    /// 快拍时的事件
    /// </summary>
    public abstract void OnShutDown();
    /// <summary>
    /// 触发特殊玩法时的事件，负责结果相关数值的计算
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public abstract void FeaturePlay(object sender, EventArgs args);
    /// <summary>
    /// 特殊玩法的表现，负责表现赔付流程
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator FeatureResult();
    /// <summary>
    /// 转轮转到Jackpot相关槽位的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public virtual void JackpotPlay(object sender, EventArgs args)
    {
        Container.isStarFly = true;
        Debug.Log("JACKPOT相关");
        int stopPos = turntableWheel.CurrentCellIndex();
        //转盘停下的是哪种JP
        switch (stopPos) {
            case GRAND:
                jpType = 0;
                break;
            case MAJOR:
                jpType = 1;
                break;
            case MINI:
                jpType = 2;
                break;
            default:
                Debug.LogErrorFormat("结果错误，当结果为‘ {0} '时，不应当作为jp流程处理", stopPos);
                break;
        }
        JackPotData.MettersCount[jpType]++;

        if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType]) {
            //中了JP，先播转盘win动画
            jpFlyTime = 2.5f;
        } else {
            //没中，直接上飞星星
            JackpotPlay2();
        }
    }
    public float JackpotPlay2() {
        if (jpType == -1) {
            return 0;
        }
        //时间不为
        if (isFly) {
            return 0;
        }
        isFly = true;
        Vector3 initPos = initFlyPos.position;
        Vector3 targetPos = JackPotData.meterPosStatic[jpType][JackPotData.MettersCount[jpType] - 1].position;
        JackPotData.meter.PlayMeterWin2(jpType, initPos, targetPos, JackPotData.MettersCount[jpType] - 1);
        if (jpType == 0 && JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType] - 1) {

            JackPotData.meter.PlayGrandRewin();
        }
        if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType]) {

            //中了JP,让metter播放就行了
            JackPotData.meter.PlayMeterWin(jpType);
            JackPotData.JPList.Add(jpType);
            return 2.5f;
        }
        //AudioManager.Playback("FE_Wheelstop03b");
        return 1;
    }
    /// <summary>
    /// 直接加分的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public virtual void ScoreRewardPlay(object sender, EventArgs args) {
        Container.CurrentActive = -1;
        int stopPos = turntableWheel.CurrentCellIndex();
        var result = GlobalObserver.GetResult(GlobalObserver.CurrGameState);
        result.WinMoney = ResultArgs[stopPos] * GlobalObserver.BetValue / 80;
        GlobalObserver.UpdateWin(result);

        UIController.BottomPanel.IncressAward(GlobalObserver.TotalWin, 2f);

          //AudioManager.Playback("FE_Wheelstop01a");
    }

    IEnumerator FeatureProcess()
    {
        AudioManager.PlayOneShot("FE_Wheelstop03");
        AudioManager.Stop("FE_WheelHyper");
        AudioManager.Stop("FE_WheelHyperSmall");
        yield return FeatureResult();
        OnStopListener?.Invoke(this);
    }

    public void OnStopInvoke() {
        DelayCallback.BeginCoroutine(FeatureProcess());
    }

    public virtual void FeatureShutDown()
    {
        FeatureIntervalTime = 0;
        isFeatureShutDown = true;
    }


}

public class ScoreArg:EventArgs
{
    public ScoreArg(int a) { this.score = a; }
    public int score = 0;
}