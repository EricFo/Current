using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core.Reel;
using SlotGame.Config;
using SlotGame.Reel.Args;
using TMPro;
using SlotGame.Core;
using Script.BonusGame.GamePlay.Wheel;
using System.ComponentModel;
using UnityEngine.UI;
using System.Linq;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using DG.Tweening;

public class BonusReelContainer : ReelContainer {
    public override event System.Action<IReelState> OnStopReelListener;
    public override event System.Action<ReelContainer> OnStopAllListener;
    public override event System.Action<ReelContainer> OnSpinAllListener;

    /// <summary>
    /// 当前屏幕上所有金币的reelId
    /// </summary>
    [HideInInspector] public List<int> ExisCoinViews = new List<int>();
    /// <summary>
    /// 上一轮屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public static int BonusCoinsViewCount = 0;
    /// <summary>
    /// 当前屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public static int CoinsViewCount = 0;
    public int CurrentActive = 0;
    /// <summary>
    /// 统计Coin总分
    /// </summary>
    [HideInInspector] public int ScoreTotal = 0;
    /// <summary>
    /// 本轮Bonus当前SPIN次数，从0开始
    /// </summary>
    [HideInInspector] public int CurrentSpinTime = 0;
    /// <summary>
    /// 本次SPIN会出现COIN的REEL的ID
    /// </summary>
    [HideInInspector] public List<int> SpinCoinReelId = new List<int>();
    /// <summary>
    /// 本次SPIN参与旋转的所有Reel，键是ReelID
    /// </summary>
    [HideInInspector] public Dictionary<int, ReelSpinArgs> reelsPredict = new Dictionary<int, ReelSpinArgs>();
    /// <summary>
    /// 本轮是否出现了新的COIN
    /// </summary>
    [HideInInspector] public bool IsNewCoin = false;
    /// <summary>
    /// 收集玩法的标记框
    /// </summary>
    public GameObject[] collectBox;
    /// <summary>
    /// 加分玩法的标记框
    /// </summary>
    public GameObject[] upgradeBox;
    /// <summary>
    /// 加分玩法的转轮
    /// </summary>
    [HideInInspector] public Wheel turntableUpgrade;
    /// <summary>
    /// 收集玩法的转轮
    /// </summary>
    [HideInInspector] public Wheel turntableCollect;
    /// <summary>
    /// 本次Bonus所包含的玩法表
    /// </summary>
    [HideInInspector] public List<BonusGamePlay> bonusGamePlays = new List<BonusGamePlay>();
    /// <summary>
    /// 转盘停止计数
    /// </summary>
    [HideInInspector] public int wheelStopCount = 0;
    /// <summary>
    /// 本次Bonus选取的序列包含的总Coin数量
    /// </summary>
    [HideInInspector] public int SequenceCoinCount = 0;
    /// <summary>
    /// 当前在生成第N个Coin
    /// </summary>
    [HideInInspector] public int CurrentCoinSerialNum = 1;
    /// <summary>
    /// Coin出现序列，包含每次SPIN可出现的Coin个数
    /// </summary>
    public int[] CoinSequence = new int[0];
    public BonusGamePlay upgradeGamePlay;
    public BonusGamePlay collectGamePlay;

    public int TotalSpinCount;

    public bool isFirst = true;
    public bool isReset = false;

    [Tooltip("剩余次数")] public BaseUI baseUI;

    public TurntableUI turntableUI;
    public GameObject[] messageUI;

    public GameObject TurntableBoost;
    public GameObject TurntableGetall;
    public GameObject ResetOjbect;
    Vector3 BoostStartPos;
    Vector3 GetallStartPos;
    Vector3 ResetStartPos;
    public Vector3 SinglePos;
    public Vector3 ResetPos;

    public int GrandStarCount;
    public int MajorStarCount;
    public int MiniStarCount;

    /// <summary>
    /// 本轮reel旋转期间是否快拍，控制钻石stop音效
    /// </summary>
    public bool isShutDowmByReel = false;
    /// <summary>
    /// 快拍后是否已经播放了一次stop音效，控制钻石stop音效
    /// </summary>
    public bool isShutDowmByReel_isStop = false;
    /// <summary>
    /// 用于同一列停止只播一遍stop音效
    /// </summary>
    public const float isAudioStop_time = 0.3f;
    /// <summary>
    /// 控制每一列只能播放只能同时存在一声Stop声音
    /// </summary>
    public bool isAudioStop_timeIsStop = false;

    public List<TurnTableResult> turnTableResults;

    public SpriteRenderer PressSpin;
    public SpriteRenderer BackGroundTop;
    public SpriteRenderer BackGroundBottom;
    public Sprite[] NormalBackGround;
    public Sprite[] ComboBackGround;

    public class TurnTableResult {
        public bool isUpgrad;
        public bool isGetAll;

        public int MiniResult;
        public int GrandResult;
        public int MajorResult;

        public bool DoubleMini;
        public bool DoubleGrand;
        public bool DoubleMajor;

        public TurnTableResult() {
            isUpgrad = false;
            isGetAll = false;

            MiniResult = -1;
            GrandResult = -1;
            MajorResult = -1;

            DoubleMini = false;
            DoubleGrand = false;
            DoubleMajor = false;
        }
    }

    public Text info1;

    public Transform[] StarTrans;

    public override void Initialize() {
        base.Initialize();
        upgradeGamePlay = new UpgradeGamePlay(this, turntableUpgrade);
        collectGamePlay = new CollectGamePlay(this, turntableCollect);
        upgradeGamePlay.OnStopListener += OnWheelStop;
        collectGamePlay.OnStopListener += OnWheelStop;
    }

    /// <summary>
    /// 每次进入Bonus的初始化
    /// </summary>
    public void ToBonusInit() {
        InitLayout();
        InitParameter();
        InitGamePlay();
        InitReplaceSymbolForCoin();
        upgradeGamePlay.EnterBonus();
        collectGamePlay.EnterBonus();
        JackPotData.MettersCount = new int[3] { 0, 0, 0 };
        JackPotData.HideAllStar();
        JackPotData.meter.OpenTxts(true);
    }
    /// <summary>
    /// 初始化参数
    /// </summary>
    public void InitParameter() {
        switch (GlobalObserver.CurrentBonusState) {
            case 1:
                upgradeGamePlay.initFlyPos = StarTrans[0];
                break;
            case 2:
                collectGamePlay.initFlyPos = StarTrans[0];
                break;
            case 3:
                upgradeGamePlay.initFlyPos = StarTrans[1];
                collectGamePlay.initFlyPos = StarTrans[2];
                break;
        }
        GetComponent<BonusGameController>().baseUI.Init();
        ExisCoinViews.Clear();
        isFirst = true;
        ScoreTotal = 0;
        CurrentSpinTime = 0;
        TotalSpinCount = 0;
        CoinsViewCount = 0;
        BonusCoinsViewCount = 0;
        CurrentCoinSerialNum = 1;
        //初始化金币生成序列
        SequenceCoinCount = ConfigManager.bonusConfig.CoinCount[GlobalObserver.CurrentBonusState][GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.CoinCountProbability[GlobalObserver.CurrentBonusState])];
        CoinSequence = ConfigManager.bonusConfig.CoinSequence[GlobalObserver.CurrentBonusState][SequenceCoinCount][GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.CoinSequenceProbability)];

        var betLevel = GlobalObserver.BetLevel;
        var wheelConfig = GlobalObserver.CurrentBonusState == 3 ? ConfigManager.bonusConfig.DoubleWheelConfig : ConfigManager.bonusConfig.SingleWheelConfig;
        GrandStarCount = ConfigManager.GetIndexByProbability(wheelConfig.GrandStarCount[betLevel]) + 1;
        MajorStarCount = ConfigManager.GetIndexByProbability(wheelConfig.MajorStarCount[betLevel]) + 1;
        MiniStarCount = ConfigManager.GetIndexByProbability(wheelConfig.MiniStarCount[betLevel]) + 1;
        if (Cheat.isGrand) {
            GrandStarCount = 3;
        }
        if (Cheat.isMajor) {
            MajorStarCount = 3;
        }
        if (Cheat.isMini) {
            MiniStarCount = 3;
        }
        Debug.LogError("Grand数量：" + GrandStarCount + "   Major数量：" + MajorStarCount + "   Mini数量：" + MiniStarCount);
    }
    /// <summary>
    /// 初始化玩法
    /// </summary>
    public void InitGamePlay() {
        //判断触发条件将需要的玩法加入到玩法表中
        bonusGamePlays.Clear();
        int comboRand = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.WheelOrderProbability);
        if (GlobalObserver.IsUpgradePlay) {
            if (GlobalObserver.IsCollectPlay) {
                upgradeGamePlay.wheelOrder = ConfigManager.bonusConfig.ComboOrderUp[SequenceCoinCount][CoinSequence.Length][comboRand];
            } else {
                //此行有DOTween报错，键不存在于字典当中
                upgradeGamePlay.wheelOrder = ConfigManager.bonusConfig.WheelOrderUp[SequenceCoinCount][CoinSequence.Length][comboRand];
            }
            Debug.LogFormat("当前取金币个数为{0}，SPIN次数为{1}的第{2}套加分转盘配置", SequenceCoinCount, CoinSequence.Length, comboRand);
            bonusGamePlays.Add(upgradeGamePlay);
        }
        if (GlobalObserver.IsCollectPlay) {
            if (GlobalObserver.IsUpgradePlay) {
                collectGamePlay.wheelOrder = ConfigManager.bonusConfig.ComboOrderCol[SequenceCoinCount][CoinSequence.Length][comboRand];
            } else {
                collectGamePlay.wheelOrder = ConfigManager.bonusConfig.WheelOrderCol[SequenceCoinCount][CoinSequence.Length][comboRand];
            }

            Debug.LogFormat("当前取金币个数为{0}，SPIN次数为{1}的第{2}套收集转盘配置", SequenceCoinCount, CoinSequence.Length, comboRand);
            bonusGamePlays.Add(collectGamePlay);
        }
        info1.text = SequenceCoinCount + "  " + CoinSequence.Length + "  " + comboRand;
        CaculateWheelResult();
    }
    /// <summary>
    /// 初始化所有Symbol都为空Symbol
    /// </summary>
    private void InitReplaceSymbolForCoin() {
        foreach (var item in reels) {
            item.ReplaceAllSymbol(SymbolNames.EMPTY);
        }
        foreach (var item in upgradeBox) {
            item.SetActive(false);
        }
        foreach (var item in collectBox) {
            item.SetActive(false);
        }
    }
    private void InitLayout() {
        PressSpin.enabled = true;
        if (GlobalObserver.CurrentBonusState == 3) {
            BackGroundTop.sprite = ComboBackGround[0];
            BackGroundBottom.sprite = ComboBackGround[1];
        } else {
            BackGroundTop.sprite = NormalBackGround[0];
            BackGroundBottom.sprite = NormalBackGround[1];
        }
        foreach (var item in messageUI) {
            item.SetActive(false);
        }
        messageUI[GlobalObserver.CurrentBonusState - 1].SetActive(true);

        BoostStartPos = TurntableBoost.transform.position;
        GetallStartPos = TurntableGetall.transform.position;
        switch (GlobalObserver.CurrentBonusState) {
            case 1:
                //TurntableBoost.transform.position = SinglePos;
                //turntableCollect.Hide();
                //TurntableGetall.SetActive(false);
                ResetOjbect.transform.position = ResetPos;
                break;
            case 2:
                //TurntableGetall.transform.position = SinglePos;
                //turntableUpgrade.Hide();
                //TurntableBoost.SetActive(false);
                ResetOjbect.transform.position = ResetPos;
                break;
            case 3:
                ResetOjbect.transform.position = new Vector3(0.021f, 1.009f);
                break;
            default:
                break;
        }
        turntableUI.ToBonusLayout();
    }
    public void ResetTurntableLayout() {
        //TurntableBoost.SetActive(true);
        //TurntableGetall.SetActive(true);
        ResetOjbect.transform.position = ResetStartPos;
        TurntableBoost.transform.position = BoostStartPos;
        TurntableGetall.transform.position = GetallStartPos;
        turntableUI.ToBaseLayout();
    }

    public override void Spin() {
        isShutDowmByReel = false;
        isShutDowmByReel_isStop = false;
        isAudioStop_timeIsStop = false;

        SetWheelResult();
        foreach (var reel in reels) {
            reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
        }
        foreach (var item in ExisCoinViews) {
            (reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).KillScoreIncress();
        }
        OnReset();
        for (int i = 0; i < reels.Length; i++) {
            reels[i].GetVisibleSymbols()[0].PlayIdleAnim();
            if (!ExisCoinViews.Contains(i)) {
                ReelSpinArgs reelSpinArgs = this.Predict(i);
                reelsPredict.Add(i, reelSpinArgs);
            }
        }
        NewSpinCoinReelId();

        foreach (var item in reelsPredict.Keys) {
            if (SpinCoinReelId.Contains(item)) {
                reelsPredict[item].resultSymbols[0] = SymbolNames.BCOIN;
            } else {
                reelsPredict[item].resultSymbols[0] = SymbolNames.EMPTY;
            }
            //reelsPredict[item].resultSymbols[0] = SymbolNames.BCOIN;
            reels[item].Spin(reelsPredict[item]);
        }
        foreach (var item in bonusGamePlays) {
            item.OnReelSpin();
        }
        turntableUI.PlayIdleAnimation();
        isFirst = false;
    }
    public override void OnReset() {
        base.OnReset();
        IsNewCoin = false;
        isReset = false;
        reelsPredict.Clear();
        CurrentSpinTime++;
        CurrentActive = 0;
        PressSpin.enabled = false;
    }
    public void NewSpinCoinReelId() {
        SpinCoinReelId.Clear();
        int coincount = CoinSequence[CurrentSpinTime - 1];
        List<int> ableId = new List<int>();
        foreach (var item in reelsPredict.Keys) {
            if (!upgradeGamePlay.InvalidFeatureReelId.Contains(item) && !collectGamePlay.InvalidFeatureReelId.Contains(item)) {
                if (!(upgradeGamePlay as UpgradeGamePlay).UpgradeReelId.Contains(item) && !(collectGamePlay as CollectGamePlay).CollectReelId.Contains(item)) {
                    ableId.Add(item);
                }
            }
        }
        for (int i = 0; i < coincount; i++) {
            int reelid = 0;
            if (upgradeGamePlay.FeatureCoinReelId.ContainsKey(CurrentCoinSerialNum)) {
                reelid = upgradeGamePlay.FeatureCoinReelId[CurrentCoinSerialNum];
            } else if (collectGamePlay.FeatureCoinReelId.ContainsKey(CurrentCoinSerialNum)) {
                reelid = collectGamePlay.FeatureCoinReelId[CurrentCoinSerialNum];
            } else {
                reelid = ableId[Random.Range(0, ableId.Count)];
            }
            ableId.Remove(reelid);
            SpinCoinReelId.Add(reelid);
            CurrentCoinSerialNum++;
        }
    }


    protected override void OnReelStop(ReelBase reel) {
        reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
        stopCount++;
        if (reel.GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN)) {
            BonusCoinSymbol bsymbol = reel.GetVisibleSymbols()[0] as BonusCoinSymbol;
            IsNewCoin = true;
            ExisCoinViews.Add(reel.Setting.reelID);
            CoinsViewCount++;
            bsymbol.serialNum = CoinsViewCount;
            //bsymbol.ScoreSpriteRender.sortingOrder += 1000;
            bsymbol.PlayStop();
            PlayAudioStop();

            if ((upgradeGamePlay as UpgradeGamePlay).UpgradeReelId.Contains(reel.installID)) {
                bsymbol.PlayBG();
            }
        }
        //判断本轮是否出现了新的COIN，用于重置SPIN次数
        if (IsNewCoin && !isReset) {
            isReset = true;
            baseUI.ResetSurplus();
        }
        OnStopReelListener?.Invoke(reel);
        if (stopCount >= reels.Length - BonusCoinsViewCount) {
            for (int i = 0; i < ExisCoinViews.Count - 1; i++) {
                for (int j = 0; j < ExisCoinViews.Count - 1 - i; j++) {
                    if (ExisCoinViews[j] > ExisCoinViews[j + 1]) {
                        int temp = ExisCoinViews[j];
                        ExisCoinViews[j] = ExisCoinViews[j + 1];
                        ExisCoinViews[j + 1] = temp;
                    }
                }
            }
            foreach (var item in ExisCoinViews) {
                Debug.Log(item);
            }
            if (bonusGamePlays.Contains(upgradeGamePlay)) {
                upgradeGamePlay.OnReelStop();
                turntableUI.PlayTurnAnimation(true);
                UIController.BottomPanel.StopBtn.interactable = true;
                CurrentActive = 1;
            } else if (bonusGamePlays.Contains(collectGamePlay)) {
                collectGamePlay.OnReelStop();
                turntableUI.PlayTurnAnimation(false);
                UIController.BottomPanel.StopBtn.interactable = true;
                CurrentActive = 2;
            }
            //foreach (var item in bonusGamePlays)
            //{
            //    item.OnShutDown();
            //}
            isRolling = false;
        }
    }
    //isShutDowmByReel
    //isShutDowmByReel_isStop
    //isAudioStop_time
    //isAudioStop_timeIsStop


    /// <summary>
    /// 播放钻石Stop声音
    /// </summary>
    private void PlayAudioStop() {
        ///没有快拍
        if (isShutDowmByReel == false) {
            if (isAudioStop_timeIsStop == false) {
                isAudioStop_timeIsStop = true;
                AudioManager.Playback("FE_BonusSymbolStop");
                StartCoroutine(AudioStopDelay());
            }
        }
        ///快拍，快拍后只播一个Stop声音就行了
        if (isShutDowmByReel && isShutDowmByReel_isStop == false) {
            isShutDowmByReel_isStop = true;
            AudioManager.Playback("FE_BonusSymbolStop");
        }
    }
    /// <summary>
    /// 控制每列只能同时播一个Stop声音
    /// </summary>
    /// <returns></returns>
    private IEnumerator AudioStopDelay() {
        yield return new WaitForSeconds(isAudioStop_time);
        isAudioStop_timeIsStop = false;
    }

    public void OnWheelStop(BonusGamePlay wheel) {
        StartCoroutine(OnWheelStop2(wheel));
    }
    private IEnumerator OnWheelStop2(BonusGamePlay wheel) {
        AudioManager.PlayOneShot("FE_Wheelstop03");
        yield return new WaitForSeconds(wheel.jpFlyTime);
        yield return new WaitForSeconds(wheel.JackpotPlay2());
        wheelStopCount++;
        float time = 0;
        if (JackPotData.JPList.Count != 0) {
            yield return new WaitForSeconds(0.2f);
            AudioManager.PlayOneShot("FeatureSelect");
            yield return new WaitForSeconds(AudioManager.GetAudioTime("FeatureSelect") + 0.5f);
            time = JackPotData.PlayJackPot();
        }

        //for (int i = 0; i < bonusGamePlays.Count; i++) {
        //if (bonusGamePlays[i].scoreIndex.Contains(bonusGamePlays[i].result)) {
        //    AudioManager.PlayOneShot("FE_Wheelstop01");
        //} else if (bonusGamePlays[i].boostIndex.Contains(bonusGamePlays[i].result)) {
        //    AudioManager.PlayOneShot("FE_Wheelstop02");
        //} else {
        //}
        //}

        //Debug.Log(time);
        if (wheelStopCount >= bonusGamePlays.Count) {
            StartCoroutine(StopWheelTwo(time));
        } else {
            StartCoroutine(StopWheelOne(time));
        }
    }

    private IEnumerator StopWheelOne(float time) {
        AudioManager.Stop("FE_WheelHyper");
        AudioManager.Stop("FE_WheelHyperSmall");
        yield return new WaitForSeconds(time);
        if (JackPotData.JPList.Count != 0) {
            turntableUI.PlayIdleAnimation(true);
            JackPotData.MettersCount[JackPotData.JPList[0]] = 0;
            JackPotData.JPList.Clear();
        } else {
            DelayCallback.Delay(this, 2f, () => { turntableUI.PlayIdleAnimation(true); });
        }
        yield return new WaitForSeconds(2f);
        collectGamePlay.OnReelStop();
        turntableUI.PlayTurnAnimation(false);
        UIController.BottomPanel.StopBtn.interactable = true;
        CurrentActive = 2;
    }

    /// <summary>
    /// 播放JackPot弹窗
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopWheelTwo(float time) {
        AudioManager.Stop("FE_WheelHyper");
        AudioManager.Stop("FE_WheelHyperSmall");
        yield return new WaitForSeconds(time);
        if (JackPotData.JPList.Count != 0) {
            turntableUI.PlayIdleAnimation();
            JackPotData.MettersCount[JackPotData.JPList[0]] = 0;
            JackPotData.JPList.Clear();
        } else {
            DelayCallback.Delay(this, 2f, turntableUI.PlayIdleAnimation);
        }
        yield return new WaitForSeconds(2f);
        foreach (var item in bonusGamePlays) {
            yield return item.FeatureResult();
        }
        //UIController.BottomPanel.DisplaySpinBtn();
        OnStopAllListener?.Invoke(this);
    }

    public override bool ShutDown() {
        isShutDowmByReel = true;
        switch (CurrentActive) {
            case 0:
                return base.ShutDown();
            case 1:
                upgradeGamePlay.OnShutDown();
                //Debug.Log("boost shut down");
                break;
            case 2:
                collectGamePlay.OnShutDown();
                //Debug.Log("getall shut down");
                break;
            default:
                break;
        }
        return true;
    }

    private void CaculateWheelResult() {
        int totalCount = CoinSequence.Length;
        Debug.LogWarning("总次数：" + totalCount);
        turnTableResults = new List<TurnTableResult>();
        //两个转轮可能同时中两个
        if (GlobalObserver.CurrentBonusState == 3) {
            for (int i = 0; i < totalCount; i++) {
                TurnTableResult result = new TurnTableResult();
                turnTableResults.Add(result);
            }
            List<int> grandCollection = Enumerable.Range(0, totalCount).ToList();
            List<int> majorCollection = Enumerable.Range(0, totalCount).ToList();
            List<int> miniCollection = Enumerable.Range(0, totalCount).ToList();
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < turnTableResults.Count; j++) {
                    //如果当前轮这个的Getal不为0，则代表会出Getal,就不能出JackPot
                    if (collectGamePlay.wheelOrder[i][j] != 0 && grandCollection.Contains(j)) {
                        turnTableResults[j].isGetAll = true;
                    }
                    //如果当前轮这个的boost不为0，则代表会出boost,就不能出JackPot
                    if (upgradeGamePlay.wheelOrder[i][j] != 0 && grandCollection.Contains(i)) {
                        turnTableResults[j].isUpgrad = true;
                    }
                    if (turnTableResults[j].isUpgrad == true && turnTableResults[j].isGetAll == true) {
                        grandCollection.Remove(j);
                        majorCollection.Remove(j);
                        miniCollection.Remove(j);
                    }
                }
            }
            for (int i = 0; i < GrandStarCount; i++) {
                int id = Random.Range(0, grandCollection.Count);
                if (turnTableResults[grandCollection[id]].GrandResult == BonusGamePlay.GRAND) {
                    turnTableResults[grandCollection[id]].DoubleGrand = true;
                }
                turnTableResults[grandCollection[id]].GrandResult = BonusGamePlay.GRAND;
                if (turnTableResults[grandCollection[id]].DoubleGrand || turnTableResults[grandCollection[id]].isUpgrad || turnTableResults[grandCollection[id]].isGetAll) {
                    grandCollection.RemoveAt(id);
                    majorCollection.RemoveAt(id);
                    miniCollection.RemoveAt(id);
                }
            }
            for (int i = 0; i < MajorStarCount; i++) {
                int id = Random.Range(0, majorCollection.Count);
                if (turnTableResults[majorCollection[id]].MajorResult == BonusGamePlay.MAJOR) {
                    turnTableResults[majorCollection[id]].DoubleMajor = true;
                }
                turnTableResults[majorCollection[id]].MajorResult = BonusGamePlay.MAJOR;
                if (turnTableResults[majorCollection[id]].DoubleMajor || turnTableResults[majorCollection[id]].isUpgrad || turnTableResults[majorCollection[id]].isGetAll) {
                    majorCollection.RemoveAt(id);
                    miniCollection.RemoveAt(id);
                }
            }
            for (int i = 0; i < MiniStarCount; i++) {
                int id = Random.Range(0, miniCollection.Count);
                if (turnTableResults[miniCollection[id]].MiniResult == BonusGamePlay.MINI) {
                    turnTableResults[miniCollection[id]].DoubleMini = true;
                }
                turnTableResults[miniCollection[id]].MiniResult = BonusGamePlay.MINI;
                if (turnTableResults[miniCollection[id]].DoubleMini || turnTableResults[miniCollection[id]].isUpgrad || turnTableResults[miniCollection[id]].isGetAll) {
                    miniCollection.RemoveAt(id);
                }
            }
        } else {
            int jackpotCount = GrandStarCount + MajorStarCount + MiniStarCount;
            List<int> enumCollection = Enumerable.Range(0, totalCount).ToList();
            if (GlobalObserver.CurrentBonusState == 1) {
                //删除Boost所在轮数
                for (int i = 0; i < 2; i++) {
                    for (int j = 0; j < upgradeGamePlay.wheelOrder[i].Length; j++) {
                        //如果当前轮这个的Boost不为0，则代表会出Boost,就不能出JackPot
                        if (upgradeGamePlay.wheelOrder[i][j] != 0 && enumCollection.Contains(j)) {
                            enumCollection.Remove(j);
                        }
                    }
                }
            }

            if (GlobalObserver.CurrentBonusState == 2) {
                //删除Getal所在轮数
                for (int i = 0; i < 2; i++) {
                    for (int j = 0; j < collectGamePlay.wheelOrder[i].Length; j++) {
                        //如果当前轮这个的Getal不为0，则代表会出Getal,就不能出JackPot
                        if (collectGamePlay.wheelOrder[i][j] != 0 && enumCollection.Contains(j)) {
                            enumCollection.Remove(j);
                        }
                    }
                }
            }

            List<int> totalList = new List<int>();
            for (int i = 0; i < jackpotCount; i++) {
                int id = Random.Range(0, enumCollection.Count);
                totalList.Add(enumCollection[id]);
                enumCollection.RemoveAt(id);
            }

            for (int i = 0; i < totalCount; i++) {
                bool isWin = totalList.Contains(i);
                TurnTableResult result = new TurnTableResult();
                result.isUpgrad = GlobalObserver.CurrentBonusState == 1;
                result.isGetAll = GlobalObserver.CurrentBonusState == 2;
                turnTableResults.Add(result);
                if (isWin == true) {
                    if (GrandStarCount > 0 && MajorStarCount > 0 && MiniStarCount > 0) {
                        int weight = Random.Range(0, 90);
                        if (weight < 30) {
                            turnTableResults[i].GrandResult = BonusGamePlay.GRAND;
                            GrandStarCount--;
                        } else if (weight < 60) {
                            turnTableResults[i].MajorResult = BonusGamePlay.MAJOR;
                            MajorStarCount--;
                        } else {
                            turnTableResults[i].MiniResult = BonusGamePlay.MINI;
                            MiniStarCount--;
                        }
                    } else if (GrandStarCount > 0 && MajorStarCount > 0) {
                        int weight = Random.Range(0, 100);
                        if (weight < 50) {
                            turnTableResults[i].GrandResult = BonusGamePlay.GRAND;
                            GrandStarCount--;
                        } else {
                            turnTableResults[i].MajorResult = BonusGamePlay.MAJOR;
                            MajorStarCount--;
                        }
                    } else if (GrandStarCount > 0 && MiniStarCount > 0) {
                        int weight = Random.Range(0, 100);
                        if (weight < 50) {
                            turnTableResults[i].GrandResult = BonusGamePlay.GRAND;
                            GrandStarCount--;
                        } else {
                            turnTableResults[i].MiniResult = BonusGamePlay.MINI;
                            MiniStarCount--;
                        }
                    } else if (MajorStarCount > 0 && MiniStarCount > 0) {
                        int weight = Random.Range(0, 100);
                        if (weight < 50) {
                            turnTableResults[i].MajorResult = BonusGamePlay.MAJOR;
                            MajorStarCount--;
                        } else {
                            turnTableResults[i].MiniResult = BonusGamePlay.MINI;
                            MiniStarCount--;
                        }
                    } else if (GrandStarCount > 0) {
                        turnTableResults[i].GrandResult = BonusGamePlay.GRAND;
                        GrandStarCount--;
                    } else if (MajorStarCount > 0) {
                        turnTableResults[i].MajorResult = BonusGamePlay.MAJOR;
                        MajorStarCount--;
                    } else if (MiniStarCount > 0) {
                        turnTableResults[i].MiniResult = BonusGamePlay.MINI;
                        MiniStarCount--;
                    }
                }
            }
        }
    }

    private void SetWheelResult() {
        Debug.LogWarningFormat("第{0}次", TotalSpinCount);
        var result = turnTableResults[TotalSpinCount];
        if (GlobalObserver.CurrentBonusState == 3) {
            if (result.isUpgrad && result.isGetAll) {
                upgradeGamePlay.result = -1;
                collectGamePlay.result = -1;
            } else if (!result.isUpgrad && !result.isGetAll) {
                int big = -1;
                int small = -1;
                if (!result.DoubleGrand && !result.DoubleMajor && !result.DoubleMini) {
                    big = result.GrandResult > -1 ? result.GrandResult : result.MajorResult;
                    small = result.MiniResult > -1 ? result.MiniResult : -1;
                } else {
                    if (result.DoubleGrand) {
                        big = result.GrandResult;
                        small = result.GrandResult;
                    } else if (result.DoubleMajor) {
                        big = result.MajorResult;
                        small = result.MajorResult;
                    } else if (result.DoubleMini) {
                        big = result.MiniResult;
                        small = result.MiniResult;
                    }
                }
                var weight = Random.Range(0, 100);
                if (weight < 50) {
                    upgradeGamePlay.result = big;
                    collectGamePlay.result = small;
                } else {
                    upgradeGamePlay.result = small;
                    collectGamePlay.result = big;
                }
            } else if (result.isUpgrad || result.isGetAll) {
                int value = FilterResult(result);
                if (result.isGetAll) {
                    collectGamePlay.result = -1;
                    upgradeGamePlay.result = value;
                } else if (result.isUpgrad) {
                    upgradeGamePlay.result = -1;
                    collectGamePlay.result = value;
                }
            }
        } else if (GlobalObserver.CurrentBonusState == 2) {
            collectGamePlay.result = FilterResult(result);
        } else if (GlobalObserver.CurrentBonusState == 1) {
            upgradeGamePlay.result = FilterResult(result);
        }
        TotalSpinCount++;
    }

    private int FilterResult(TurnTableResult result) {
        if (result.GrandResult > -1) {
            return result.GrandResult;
        } else if (result.MajorResult > -1) {
            return result.MajorResult;
        } else {
            return result.MiniResult;
        }
    }

    protected override string[] GetReelStripe(int id) {
        return ConfigManager.reelStripConfig.GetStripe("Bonus")[id];
    }
    protected override bool CheckScatter(int id, bool isScatter) {
        return false;
    }

    /// <summary>
    /// 统计所有COIN的分数，返回总分
    /// </summary>
    /// <returns></returns>
    public int TotalScore() {
        int scoreTotal = 0;
        foreach (var item in reels) {
            if (item.GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN)) {
                scoreTotal += (item.GetVisibleSymbols()[0] as BonusCoinSymbol).ScoreValue;
            }
        }
        return scoreTotal;
    }
}
