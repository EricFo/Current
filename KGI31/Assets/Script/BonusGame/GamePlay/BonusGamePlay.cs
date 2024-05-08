using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Result;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using UniversalModule.SpawnSystem;

public abstract class BonusGamePlay
{
    /// <summary>
    /// Feature效果间隔时间
    /// </summary>
    public float FeatureIntervalTime;
    /// <summary>
    /// 是否中到了Feature玩法
    /// </summary>
    public bool isFeaturePlay = false;
    /// <summary>
    /// 是否中到了AddScore玩法
    /// </summary>
    public bool isAddScorePlay = false;
    /// <summary>
    /// 是否中到了转轮升级玩法
    /// </summary>
    public bool isUpdateGamePlay = false;
    /// <summary>
    /// 是否中到了加SPIN次数玩法
    /// </summary>
    public bool isAddSurplusPlay = false;
    /// <summary>
    /// 是否中到了Jackpot玩法
    /// </summary>
    public bool isJackpotPlay = false;
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
    /// <summary>
    /// 大单元格所在的位置索引
    /// </summary>
    public int[] bigIndex = new int[3] { 0, 3, 6 };

    public const int GRAND = 1;
    public const int MAJOR = 7;
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
    /// <summary>
    /// 触发加1SPIN的事件
    /// </summary>
    public event EventHandler OnAddSurplusPlay;
    /// <summary>
    /// 升级游戏玩法的事件
    /// </summary>
    public event EventHandler OnUpdateGamePlay;
    /// <summary>
    /// 转轮Wheel停止的事件
    /// </summary>
    public event Action<BonusGamePlay> OnStopListener;
    public Action OnWinAnimComplete;
    /// <summary>
    /// 转轮是否停止
    /// </summary>
    public bool IsStop;
    /// <summary>
    /// 触发事件时转轮位置索引所对应的参数
    /// </summary>
    public Dictionary<int, int> ResultArgs = new Dictionary<int, int>()
    {
        {0,80 },{1,1 },{3,2 },{4,50 },{5,1 },{6,80 },{7,1 },{9,2 },{10,50 },{11,1 }
    };
    /// <summary>
    /// 转轮停下时位置索引所对应的事件
    /// </summary>
    public Dictionary<int, EventHandler> ResultOptions = new Dictionary<int, EventHandler>();
    /// <summary>
    /// 当前玩法所绑定的实际转轮表现
    /// </summary>
    public Wheel turntableWheel = null;
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

    public bool isBigResult = false;

    public int jpType = -1;

    public int result = -1;
    public JackpotType jpResult = JackpotType.Empty;

    protected bool isFeatureShutDown = false;
    protected bool isShutDownAudio = false;

    bool isWinloopAudio = false;

    public BonusGamePlay() { }
    private BonusReelContainer _container;
    public BonusReelContainer Container { get { return _container; } set { _container = value; } }

    public BonusGamePlay(BonusReelContainer reelContainer,Wheel turntable) {
        Container = reelContainer;
        turntableWheel = turntable;
        turntableWheel.OnWheelStop += OnWheelStop;


        OnFeaturePlay -= FeaturePlay;
        OnFeaturePlay += FeaturePlay;
        OnJackpotPlay -= JackpotPlay;
        OnJackpotPlay += JackpotPlay;
        OnScoreRewardPlay -= ScoreRewardPlay;
        OnScoreRewardPlay += ScoreRewardPlay;
        OnAddSurplusPlay -= AddSurplusPlay;
        OnAddSurplusPlay += AddSurplusPlay;
        OnUpdateGamePlay -= UpdateGamePlay;
        OnUpdateGamePlay += UpdateGamePlay;
    }

    private void OnWheelStop()
    {
        isFly = false;
        jpFlyTime = 0;
        jpType = -1;
        UIController.BottomPanel.StopBtn.interactable = false;
        int stopPos = turntableWheel.CurrentCellIndex();
        //isBigResult = bigIndex.Contains(stopPos);
        Container.turntableUI.PlayWin2Animation(turntableWheel.isExtra);
        ResultOptions[stopPos].Invoke(this, EventArgs.Empty);
        OnStopInvoke();
    }

    /// <summary>
    /// 每次进入Bonus时所要做的初始化
    /// </summary>
    public virtual void EnterBonus()
    {
        InvalidFeatureReelId.Clear();
        ValidFeatureReelId.Clear();
        FeatureCoinReelId.Clear();
        ResultOptions.Clear();

        //初始化转轮单元格对应的事件
        if (GlobalObserver.CurrentBonusState == 3)
        {
            ResultOptions.Add(0, OnScoreRewardPlay);
        }
        else
        {
            if (GlobalObserver.CurrentBonusState == 1 && turntableWheel.isExtra)
            {
                ResultOptions.Add(0, OnUpdateGamePlay);
            }
            else if (GlobalObserver.CurrentBonusState == 2 && !turntableWheel.isExtra)
            {
                ResultOptions.Add(0, OnUpdateGamePlay);
            }
            else
            {
                ResultOptions.Add(0, OnScoreRewardPlay);
            }
        }

        ResultOptions.Add(1, OnFeaturePlay);
        ResultOptions.Add(2, OnAddSurplusPlay);
        ResultOptions.Add(3, OnFeaturePlay);
        ResultOptions.Add(4, OnScoreRewardPlay);
        ResultOptions.Add(5, OnFeaturePlay);
        ResultOptions.Add(6, OnScoreRewardPlay);
        ResultOptions.Add(7, OnFeaturePlay);
        ResultOptions.Add(8, OnAddSurplusPlay);
        ResultOptions.Add(9, OnFeaturePlay);
        ResultOptions.Add(10, OnScoreRewardPlay);
        ResultOptions.Add(11, OnFeaturePlay);
    }
    /// <summary>
    /// 按下SPIN后的事件
    /// </summary>
    public virtual void OnReelSpin()
    {
        if (Container.isFirst)
        {
            turntableWheel.StartRolling(turntableWheel.lowSpeed);
        }
        else
        {
            turntableWheel.StartRolling(0);
        }
        isFeaturePlay = false;
        isAddScorePlay = false;
        isUpdateGamePlay = false;
        isAddSurplusPlay = false;
        isJackpotPlay = false;
        isFeatureShutDown = false;
        isShutDownAudio = false;
        GlobalObserver.DoneWinAnim = false;
    }
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
    public abstract IEnumerator FeaturePlay();
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
        ResultOptions[result] -= JackpotPlay;
        isJackpotPlay = true;
        //Container.turntableUI.PlayWin2Animation(turntableWheel.isExtra);
        /*Container.isStarFly = true;
        //Debug.Log("JACKPOT相关");
        int stopPos = turntableWheel.CurrentCellIndex();
        //转盘停下的是哪种JP
        switch (jpResult) {
            case JackpotType.Grand:
                jpType = 0;
                break;
            case JackpotType.Major:
                jpType = 1;
                break;
            case JackpotType.Mini:
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
        }*/
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
    public IEnumerator JackpotPlay()
    {
        if (!isJackpotPlay)
        {
            yield break;
        }
        int stopPos = turntableWheel.CurrentCellIndex();
        //转盘停下的是哪种JP
        switch (jpResult)
        {
            case JackpotType.Grand:
                jpType = 0;
                break;
            case JackpotType.Major:
                jpType = 1;
                break;
            case JackpotType.Mini:
                jpType = 2;
                break;
            default:
                Debug.LogErrorFormat("结果错误，当结果为‘ {0} '时，不应当作为jp流程处理", stopPos);
                break;
        }
        JackPotData.MettersCount[jpType]++;

        Vector3 initPos = initFlyPos.position;
        Vector3 targetPos = JackPotData.meterPosStatic[jpType][JackPotData.MettersCount[jpType] - 1].position;
        JackPotData.meter.PlayMeterWin2(jpType, initPos, targetPos, JackPotData.MettersCount[jpType] - 1);
        yield return new WaitForSeconds(42 / 30f);

        if (jpType == 0 && JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType] - 1)
        {

            JackPotData.meter.PlayGrandRewin();
        }
        if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType])
        {

            //中了JP,让metter播放就行了
            JackPotData.meter.PlayMeterWin(jpType);
            JackPotData.JPList.Add(jpType);
        }
        if (JackPotData.JPList.Count != 0)
        {
            yield return new WaitForSeconds(0.2f);
            AudioManager.PlayOneShot("FeatureSelect");
            yield return new WaitForSeconds(AudioManager.GetAudioTime("FeatureSelect") + 0.5f);
            yield return new WaitForSeconds(JackPotData.PlayJackPot());
        }
    }
    /// <summary>
    /// 直接加分的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public virtual void ScoreRewardPlay(object sender, EventArgs args) {
        isAddScorePlay = true;
        Container.isScore = true;
        //AudioManager.Playback("FE_Wheelstop01a");
    }
    public virtual IEnumerator ScoreRewardPlay()
    {
        //Container.turntableUI.PlayWin2Animation(turntableWheel.isExtra);
        GameObject CreditFly = turntableWheel.isExtra ? Container.turntableUI.DouCreditFly : Container.turntableUI.MulCreditFly;
        yield return new WaitForSeconds(0.3f);
        CreditFly.SetActive(true);
        AudioManager.PlayOneShot("FE_WheelstopCredit");
        yield return new WaitForSeconds(20 / 30f);
        CreditFly.SetActive(false);

        int stopPos = turntableWheel.CurrentCellIndex();
        var result = GlobalObserver.GetResult(GlobalObserver.CurrGameState);
        result.WinMoney = ResultArgs[stopPos] * GlobalObserver.BetValue / 80;
        GlobalObserver.UpdateWin(result);
        AudioManager.LoopPlayback("KGI29-Winloop");
        isWinloopAudio = true;
        UIController.BottomPanel.OnAwardComplateEvent += OnAwardComplete;
        UIController.BottomPanel.AwardRollUp(result.WinMoney);
        Container.meterWin.SetActive(true);
    }

    /// <summary>
    /// 加一次SPIN的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public virtual void AddSurplusPlay(object sender, EventArgs args)
    {
        isAddSurplusPlay = true;
    }
    public virtual IEnumerator AddSurplusPlay()
    {
        if (!isAddSurplusPlay)
        {
            yield break;
        }
        yield return new WaitForSeconds(1f);
        Container.baseUI.PlayAddSurplusAnim(turntableWheel.isExtra);
        yield return new WaitForSeconds(23 / 30f);
        Container.baseUI.AddSurplus(2);
        Container.baseUI.PlayResetAnim();
    }
    
    public virtual void UpdateGamePlay(object sender, EventArgs args)
    {
        isUpdateGamePlay = true;
    }
    public virtual IEnumerator UpdateGamePlay()
    {
        if (!isUpdateGamePlay)
        {
            yield break;
        }
        AudioManager.Playback("FeatureSelect");
        ScatterSymbol scatter = SpawnFactory.GetObject<ScatterSymbol>(GlobalObserver.IsUpgradePlay? SymbolNames.SCATTERCOL:SymbolNames.SCATTERUP);
        Container.scatterWin = scatter;
        scatter.SetSortingOrder(1000);
        scatter.SetMaskMode(SpriteMaskInteraction.None);
        scatter.transform.localScale = Vector3.one * 0.67f;
        scatter.transform.position = new Vector3(0.027f, 5.695f);
        scatter.PlayWinAnim();

        ResultOptions[0] = OnScoreRewardPlay;
        Container.isFreeToComboCurrSpin = true;
        if (GlobalObserver.CurrentBonusState == 1)
        {
            Container.doneFree1ToCombo = true;
        }
        else
        {
            Container.doneFree2ToCombo = true;
        }
        yield return new WaitForSeconds(AudioManager.GetAudioTime("FeatureSelect"));
    }

    public void OnStopInvoke() {
        DelayCallback.BeginCoroutine(FeatureProcess());
    }
    IEnumerator FeatureProcess()
    {
        AudioManager.PlayOneShot("FE_Wheelstop03");
        AudioManager.Stop("FE_WheelHyper");
        AudioManager.Stop("FE_WheelHyperSmall");
        yield return JackpotPlay();
        yield return FeaturePlay();
        yield return UpdateGamePlay();
        yield return AddSurplusPlay();
        if (isAddScorePlay)
        {
            yield return ScoreRewardPlay();
        }
        else
        {
            OnAwardComplete();
        }
        if (isUpdateGamePlay)
        {
            yield return new WaitForSeconds(2f);
            GlobalObserver.DoneWinAnim = true;
            OnWinAnimComplete();
        }
    }
    public void OnAwardComplete() {
        Container.meterWin.SetActive(false);
        if (isWinloopAudio)
        {
            AudioManager.Stop("KGI29-Winloop");
            isWinloopAudio = false;
            AudioManager.PlayOneShot("KGI29-Winloopend");
        }
        UIController.BottomPanel.OnAwardComplateEvent -= OnAwardComplete;
        OnStopListener?.Invoke(this);
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