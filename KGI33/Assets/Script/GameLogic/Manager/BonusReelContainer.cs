using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core.Reel;
using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Symbol;
using DG.Tweening;
using System;
using SlotGame.Reel.Args;
using Random = UnityEngine.Random;
using UniversalModule.SpawnSystem;
using System.Linq;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using Spine.Unity;

public class BonusReelContainer : ReelContainer
{
    public override event Action<IReelState> OnSpinReelListener;
    public override event Action<IReelState> OnStopReelListener;
    public override event Action<ReelContainer> OnSpinAllListener;
    public override event Action<ReelContainer> OnStopAllListener;

    public bool isReverse = false;
    public ReelSpinArgs reverseArg;
    /// <summary>
    /// Spin计数UI
    /// </summary>
    public BaseUI baseUI;
    /// <summary>
    /// BonusIntro奖池管理
    /// </summary>
    public PrizedrawController prizedrawController;
    /// <summary>
    /// 当前屏幕上所有Coin的reelId
    /// </summary>
    public List<int> exisCoinViews = new List<int>();
    /// <summary>
    /// 上一轮屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public int bonusCoinsViewCount = 0;
    /// <summary>
    /// 当前屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public int coinsViewCount = 0;
    /// <summary>
    /// 本轮Bonus总Coin数量
    /// </summary>
    public int totalCoinCount = 0;
    /// <summary>
    /// 本次SPIN参与旋转的所有Reel，键是ReelID
    /// </summary>
    public Dictionary<int, ReelSpinArgs> reelsPredict = new Dictionary<int, ReelSpinArgs>();
    /// <summary>
    /// Coin的等级组合，3/2/1
    /// </summary>
    public int[] coinLevelAssembly;
    /// <summary>
    /// BonusIntro中上飞COIN位置
    /// </summary>
    public BonusCoinSymbol[] IntroCoinPos;
    public Transform[] NormPos;
    /// <summary>
    /// symbol容器
    /// </summary>
    public List<BonusCoinSymbol> symbolContainer;
    /// <summary>
    /// COIN数字文本
    /// </summary>
    public ArtText[] coinCountTxts;
    /// <summary>
    /// COIN计数
    /// </summary>
    public int[] coinCounts;
    /// <summary>
    /// 奖池
    /// </summary>
    public GameObject prizeDraw;
    /// <summary>
    /// 本次SPIN新出现的COIN数量
    /// </summary>
    public int spinCoinCount = 0;
    public CattleUI cattleUI;
    public Dictionary<int, bool> canChangeLevelPos;
    /// <summary>
    /// 需要升级的Symbol
    /// </summary>
    List<BonusCoinSymbol> upgradeSymbols = new List<BonusCoinSymbol>();
    /// <summary>
    /// 需要保留不升级的Symbol
    /// </summary>
    List<int> retainSymbols = new List<int>();
    public List<int> resultSequence = new List<int>();
    public List<int> spinResultSequence = new List<int>();
    public bool willSelectJackpot = true;
    public JackPot JackPot;
    bool isCollectShutDown = false;


    public override void Initialize()
    {
        base.Initialize();
        prizedrawController.Initialize();
        prizedrawController.OnCoinClickEvent += OnCoinClick;
        prizedrawController.OnBonusIntroOverEvent += OnBonusIntroOverClick;
        for (int i = 0; i < IntroCoinPos.Length; i++)
        {
            IntroCoinPos[i].Level = i + 1;
        }
    }
    /// <summary>
    /// 每次进入Bonus的初始化
    /// </summary>
    public void ToBonusInit()
    {
        baseUI.Init();
        //cattleUI.BaseToBonus();
        InitParameter();
        InitReplaceSymbolForCoin();
    }
    /// <summary>
    /// 初始化参数
    /// </summary>
    private void InitParameter()
    {
        willSelectJackpot = true;
        exisCoinViews.Clear();
        upgradeSymbols.Clear();
        coinsViewCount = 0;
        bonusCoinsViewCount = 0;
        int initLevel3CoinCount = 0;
        totalCoinCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.EndCoinCountProbability) + 7;
        resultSequence = ConfigManager.bonusConfig.ResultSequence[Random.Range(0, ConfigManager.bonusConfig.ResultSequence.Length)].ToList();
        //resultSequence = new List<int>() { 1, 1, 1, 1, 1, 2, 2, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 3, 3, 3, 3, 3 };
        if (Cheat.isMoreBonusCoin)
        {
            totalCoinCount = 14;
        }
        int prizedrawCoinCount = Random.Range(3, 6);
        int introAddSpinCount = (resultSequence.Count - prizedrawCoinCount) / (resultSequence.Count > 15 ? 2 : 3);
        prizedrawController.InitBonus(prizedrawCoinCount, introAddSpinCount);
        resultSequence.RemoveRange(0, prizedrawCoinCount);
        foreach (var item in GlobalObserver.CoinPosLevel)
        {
            if (item.Value == 3)
            {
                initLevel3CoinCount++;
            }
        }
        canChangeLevelPos = new Dictionary<int, bool>() { { 1, true }, { 2, true }, { 3, true } };
        initLevel3CoinCount += prizedrawController.initLevel3CoinCount;
        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.IndexOfEndCoinCountProbability[totalCoinCount][initLevel3CoinCount]);
        int[][] levelAssemblys = ConfigManager.bonusConfig.AssemblyOfEndCoinCount[totalCoinCount][initLevel3CoinCount][index];
        coinLevelAssembly = levelAssemblys[Random.Range(0, levelAssemblys.Length)];
        if (Cheat.isMoreBonusCoin)
        {
            coinLevelAssembly = new int[] { 4, 4, 6 };
            Cheat.isMoreBonusCoin = false;
        }
        string info = "";
        foreach (var item in resultSequence)
        {
            info += item.ToString();
        }
        GlobalObserver.NewResult(info);

        //RecordRetainSymbols();

        /*GlobalObserver.IsGrandBonus = Random.Range(0, 1f) < ConfigManager.bonusConfig.GrandProbability;
        GlobalObserver.IsMajorBonus = Random.Range(0, 1f) < ConfigManager.bonusConfig.MajorProbability;
        if (Cheat.isGrandBonus)
        {
            GlobalObserver.IsGrandBonus = true;
            Cheat.isGrandBonus = false;
        }
        if (Cheat.isMajorBonus)
        {
            GlobalObserver.IsMajorBonus = true;
            Cheat.isMajorBonus = false;
        }*/
    }

    void RecordRetainSymbols()
    {
        retainSymbols.Clear();
        Dictionary<int, List<int>> initLevelPos = new Dictionary<int, List<int>>()
        {
            { 1, new List<int>() },
            { 2, new List<int>() },
            { 3, new List<int>() }
        };
        foreach (var item in GlobalObserver.CoinPosLevel)
        {
            initLevelPos[item.Value].Add(item.Key);
        }

        for (int i = 1; i < 3; i++)
        {
            int retainCount = 0;
            if (coinLevelAssembly[3 - i] < initLevelPos[i].Count)
            {
                retainCount = coinLevelAssembly[3 - i];
            }
            else
            {
                retainCount = initLevelPos[i].Count;
            }
            for (int j = 0; j < retainCount; j++)
            {
                int retainId = initLevelPos[i].GetRandomItem();
                retainSymbols.Add(retainId);
                initLevelPos[i].Remove(retainId);
            }
        }

        string info = "RetainSymbols:   ";
        foreach (var item in retainSymbols)
        {
            info += item.ToString() + "   ";
        }
        Debug.Log(info);
    }

    /// <summary>
    /// 初始化Symbol，继承Base的Coin
    /// </summary>
    void InitReplaceSymbolForCoin()
    {
        UIController.BottomPanel.DisableAllButtons();
        coinCounts = new int[] { 0, 0, 0 };
        UpdateCoinCounts();
        foreach (var item in reels)
        {
            item.ReplaceAllSymbol(SymbolNames.EMPTY);
            CommonSymbol[] emptySymbols = item.GetAllSymbols();
            foreach(var symbol in emptySymbols)
            {
                (symbol as EmptySymbol).InitSymbolMainTex();
            }
        }
        /*foreach (var item in GlobalObserver.CoinPosLevel)
        {
            BonusCoinSymbol symbol = reels[item.Key].ReplaceSymbol(SymbolNames.BCOIN, 1) as BonusCoinSymbol;
            symbol.Level = item.Value;
            symbol.SetScore(GlobalObserver.CoinPosScore[item.Key]);
            symbol.SetMaskMode(SpriteMaskInteraction.None);
            symbol.UpSortingOrder();
            exisCoinViews.Add(item.Key);
            coinsViewCount++;
        }*/
        bonusCoinsViewCount = coinsViewCount;
    }

    private void OnBonusIntroOverClick()
    {
        StartCoroutine(_IntroOverReplaceSymbolForCoin());
    }
    IEnumerator _IntroOverReplaceSymbolForCoin()
    {
        List<int> ableCollection = new List<int>();
        for (int i = 0; i < reels.Length; i++)
        {
            if (!exisCoinViews.Contains(i) && i != 7)
            {
                ableCollection.Add(i);
            }
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < coinCounts.Length; i++)
        {
            int coinCount = coinCounts[i];
            if (coinCount > 0)
            {
                for (int j = 0; j < coinCount; j++)
                {
                    AudioManager.Playback("FE_CoinFlydown");
                    FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                    int reelId = ableCollection.GetRandomItem();
                    ableCollection.Remove(reelId);
                    CommonSymbol symbol = reels[reelId].GetVisibleSymbols()[0];
                    int level = i + 1;
                    flySymbol.PlayCoinUpAnim(level, IntroCoinPos[i].transform.position, symbol.transform.position);
                    UpdateCoinCounts(level, -1);
                    yield return new WaitForSeconds(14 / 30f);
                    BonusCoinSymbol bSymbol = reels[reelId].ReplaceSymbol(SymbolNames.BCOIN, 1) as BonusCoinSymbol;
                    bSymbol.Level = level;
                    int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][bSymbol.Level]);
                    int score = ConfigManager.bonusConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][bSymbol.Level][index];
                    bSymbol.SetScore(score);
                    bSymbol.SetMaskMode(SpriteMaskInteraction.None);
                    bSymbol.UpSortingOrder();
                    bSymbol.PlayStopAnim();
                    exisCoinViews.Add(reelId);
                    coinsViewCount++;
                    bonusCoinsViewCount = coinsViewCount;
                    yield return new WaitForSeconds(16 / 30f);
                }
            }
        }
        prizeDraw.SetActive(false);


        if (Cheat.isAutoPlay)
        {
            UIController.BottomPanel.OnSpinBtnClick();
        }
        else
        {
            UIController.BottomPanel.DisplaySpinBtn();
        }
    }

    public override void Spin()
    {
        if (isRolling == false)
        {
            OnReset();
            isRolling = true;
            //OnSpinAllListener?.Invoke(this);
            List<int> ableIndex = new List<int>();
            for (int i = 0; i < reels.Length; i++)
            {
                if (!exisCoinViews.Contains(i))
                {
                    ableIndex.Add(i);
                    ReelSpinArgs reelSpinArg = Predict(i);
                    reelSpinArg.resultSymbols[0] = SymbolNames.EMPTY;
                    reelsPredict.Add(i, reelSpinArg);
                }
                //OnSpinReelListener?.Invoke(reels[i]);
            }
            ableIndex.Remove(7);
            resultSequence.RemoveRange(0, spinResultSequence.Count);
            spinResultSequence.Clear();
            if (!CheckResultComplete() && baseUI.CheckRemaining() == 0)
            {
                reelsPredict[7].resultSymbols[0] = SymbolNames.ADDSPIN2;
            }
            else
            {
                spinResultSequence = GetSpinResultSequence();
                if (spinResultSequence.Count == 0)
                {
                    float rand = Random.Range(0, 1f);
                    if (rand < ConfigManager.bonusConfig.AddSpin2Probability && !CheckResultComplete())
                    {
                        reelsPredict[7].resultSymbols[0] = SymbolNames.ADDSPIN2;
                    }
                }
                else
                {
                    foreach (var item in spinResultSequence)
                    {
                        if (item == 1)
                        {
                            int coinIndex = ableIndex[Random.Range(0, ableIndex.Count)];
                            reelsPredict[coinIndex].resultSymbols[0] = SymbolNames.BCOIN;
                            reelsPredict[coinIndex].coinLevel = 1;
                            ableIndex.Remove(coinIndex);
                        }
                        else
                        {
                            reelsPredict[7].resultSymbols[0] = SymbolNames.SCATTER;
                        }
                    }
                }

            }
            /*if (coinsViewCount < totalCoinCount && Random.Range(0, 1f) < ConfigManager.bonusConfig.CoinProbability)
            {
                int ableCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.SpinCoinCountProbability) + 1;
                if (ableCount > totalCoinCount - coinsViewCount)
                {
                    ableCount = totalCoinCount - coinsViewCount;
                }
                List<int> levelAble = GetCoinLevelAble();
                for (int i = 0; i < ableCount; i++)
                {
                    int coinIndex = ableIndex[Random.Range(0, ableIndex.Count)];
                    int coinLevel = levelAble[Random.Range(0, levelAble.Count)];
                    reelsPredict[coinIndex].resultSymbols[0] = SymbolNames.BCOIN;
                    reelsPredict[coinIndex].coinLevel = coinLevel;
                    ableIndex.Remove(coinIndex);
                    //levelAble.Remove(coinLevel);

                    Debug.Log("NewCoin: index  " + coinIndex + "     level  " + coinLevel);
                }
            }

            string result;
            if (CheckResultComplete())
            {
                result = SymbolNames.EMPTY;
            }
            else
            {
                if (baseUI.CheckRemaining() == 0)
                {
                    result = SymbolNames.ADDSPIN2;
                }
                else if (baseUI.CheckRemaining() <= 2)
                {
                    result = ConfigManager.bonusConfig.ReelCenterResult[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.ReelCenterResultProbability[1])];
                }
                else
                {
                    result = ConfigManager.bonusConfig.ReelCenterResult[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.ReelCenterResultProbability[0])];
                }
                if (result == SymbolNames.SCATTER && GetUpgradeCoin().Count <= 0)
                {
                    result = SymbolNames.EMPTY;
                }
            }
            reelsPredict[7].resultSymbols[0] = result;*/
            reverseArg = reelsPredict[7];
            if (reelsPredict[7].resultSymbols[0] == SymbolNames.SCATTER && Random.Range(0, 1f) < ConfigManager.bonusConfig.ReelReverseProbability)
            {
                isReverse = true;
                reelsPredict[7].resultSymbols[0] = SymbolNames.EMPTY;
            }

            foreach (var item in reelsPredict.Keys)
            {
                reels[item].GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
                if (reels[item].GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
                {
                    (reels[item].GetVisibleSymbols()[0] as ScatterSymbol).UpSortingOrder(false);
                }
                reels[item].Spin(reelsPredict[item]);
            }
        }
    }

    public override void OnReset()
    {
        isReverse = false;
        base.OnReset();
        reelsPredict.Clear();
        spinCoinCount = 0;
        upgradeSymbols.Clear();
    }

    protected override void OnReelStop(ReelBase reel)
    {
        reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
        stopCount++;
        if (reel.GetVisibleSymbols()[0].ItemName == SymbolNames.BCOIN)
        {
            (reel.GetVisibleSymbols()[0] as BonusCoinSymbol).PlayStopAnim();
            exisCoinViews.Add(reel.InstallID);
            coinsViewCount++;
            spinCoinCount++;
        }
        if (reel.GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
        {
            ScatterSymbol scatterSymbol = reel.GetVisibleSymbols()[0] as ScatterSymbol;
            scatterSymbol.UpSortingOrder();
            scatterSymbol.OnStop();
            scatterSymbol.PlayStopAnim();
            AudioManager.Playback("FE_SpecialStop Bonus");
        }
        OnStopReelListener?.Invoke(reel);
        if (stopCount >= reels.Length - bonusCoinsViewCount)
        {
            StartCoroutine(_OnAllReelStop());
        }
    }

    IEnumerator _OnAllReelStop()
    {
        if (isReverse)
        {
            //yield return new WaitForSeconds(3f);
            yield return new WaitForSeconds(1f);
            AudioManager.Playback("FE_SpecialBigPrompt");
            yield return cattleUI._PlayBonusAddCattleAnim();
            ScatterSymbol scatterSymbol = reels[7].ReplaceSymbol(SymbolNames.SCATTER, 1) as ScatterSymbol;
            scatterSymbol.SetMaskMode(SpriteMaskInteraction.None);
            scatterSymbol.UpSortingOrder();
            scatterSymbol.OnStop();
            scatterSymbol.PlayStopAnim();
            AudioManager.Playback("FE_SpecialStop Bonus");
            yield return new WaitForSeconds(0.5f);
        }
        switch (reels[7].GetVisibleSymbols()[0].ItemName)
        {
            case SymbolNames.SCATTER:
                cattleUI.PlayCattleTriggerAnim();
                yield return _UpgradeCoin();
                break;
            case SymbolNames.EMPTY:
                break;
            case SymbolNames.ADDSPIN1:
                AddSpinSymbol addSymbol = reels[7].GetVisibleSymbols()[0] as AddSpinSymbol;
                addSymbol.PlayFlyAnim();
                yield return new WaitForSeconds(56 / 30f);
                baseUI.AddSurplus(1);
                break;
            case SymbolNames.ADDSPIN2:
                AddSpinSymbol addSymbol2 = reels[7].GetVisibleSymbols()[0] as AddSpinSymbol;
                addSymbol2.PlayFlyAnim();
                yield return new WaitForSeconds(56 / 30f);
                baseUI.AddSurplus(2);
                break;
            default: break;
        }
        isRolling = false;
        OnStopAllListener?.Invoke(this);
        bonusCoinsViewCount = coinsViewCount;

        //调试信息
        string info = "";
        foreach (var item in exisCoinViews)
        {
            info += item.ToString() + "   ";
        }
        Debug.Log("ViewCoins: " + info);
    }

    IEnumerator _UpgradeCoin()
    {
        yield return new WaitForSeconds(30 / 30f);
        (reels[7].GetVisibleSymbols()[0] as ScatterSymbol).PlayBonusUpgradeAnim();
        yield return new WaitForSeconds(20 / 30f);
        /*List<int> upgradeAble = GetUpgradeCoin();
        int upgradeCount = Random.Range(1, 5);
        if (upgradeCount > upgradeAble.Count)
        {
            upgradeCount = upgradeAble.Count;
        }
        List<int> upgradeIndex = new List<int>();
        for (int i = 0; i < upgradeCount; i++)
        {
            int index = upgradeAble.GetRandomItem();
            upgradeIndex.Add(index);
            upgradeAble.Remove(index);
        }*/
        List<int> upgradeIndex = GetUpgradeCoin();
        upgradeIndex.Sort();
        foreach (var item in upgradeIndex)
        {
            BonusCoinSymbol bSymbol = (BonusCoinSymbol)reels[item].GetVisibleSymbols()[0];
            bSymbol.PlayUpgradeStopAnim();
            bSymbol.transform.DOScale(Vector3.one * 1.2f, 7 / 30f);
            bSymbol.DisplayGlow(true);
            upgradeSymbols.Add(bSymbol);
            //upgradeAble.Remove(item);
        }
        yield return new WaitForSeconds(30 / 30f);
        foreach (var upgradeSymbol in upgradeSymbols)
        {
            upgradeSymbol.PlayChangeAnim();
            yield return new WaitForSeconds(20 / 30f);
            if (willSelectJackpot && upgradeSymbol.Level == 3)
            {
                upgradeSymbol.SetScore(GlobalObserver.IDJackpotDic[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.JackpotProbability)]);
                willSelectJackpot = false;
                if (Cheat.isMiniBonus)
                {
                    upgradeSymbol.SetScore(JackpotType.Mini);
                    Cheat.isMiniBonus = false;
                }
                if (Cheat.isMinorBonus)
                {
                    upgradeSymbol.SetScore(JackpotType.Minor);
                    Cheat.isMinorBonus = false;
                }
                if (Cheat.isMajorBonus)
                {
                    upgradeSymbol.SetScore(JackpotType.Major);
                    Cheat.isMajorBonus = false;
                }
                if (Cheat.isGrandBonus)
                {
                    upgradeSymbol.SetScore(JackpotType.Grand);
                    Cheat.isGrandBonus = false;
                }
                if (upgradeSymbol.JackpotType != JackpotType.Null)
                {
                    upgradeSymbol.DisplayJPCoinNum();
                }
            }
            yield return new WaitForSeconds(10 / 30f);
        }
        yield return new WaitForSeconds(1);
        foreach (var item in upgradeSymbols)
        {
            if (item.JackpotType != JackpotType.Null)
            {
                yield return JackPot.PlayJackpotCele(item);
            }
        }
        yield return new WaitForSeconds(1);
        yield return CoinCollect(upgradeSymbols);
        cattleUI.PlayCattleLoopAnim();
    }

    IEnumerator CoinCollect(List<BonusCoinSymbol> needCollectSymbols)
    {
        float intervalTime = 10 / 30f;
        List<BonusCoinSymbol> bonusCoinSymbols = new List<BonusCoinSymbol>();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int index = i + j * 5;
                var symbol = reels[index].GetVisibleSymbols()[0];
                if (symbol.ItemName == SymbolNames.BCOIN)
                {
                    bonusCoinSymbols.Add(symbol as BonusCoinSymbol);
                }
            }
        }
        Dictionary<int, List<BonusCoinSymbol>> levelCoin = new Dictionary<int, List<BonusCoinSymbol>>()
        {    { 1, new List<BonusCoinSymbol>() },{ 2, new List<BonusCoinSymbol>() },{ 3, new List<BonusCoinSymbol>() } };
        foreach (var bsymbol in bonusCoinSymbols)
        {
            levelCoin[bsymbol.Level].Add(bsymbol);
        }
        foreach (var collectSymbol in needCollectSymbols)
        {
            isCollectShutDown = false;
            UIController.BottomPanel.StopBtn.interactable = true;
            UIController.BottomPanel.StopBtn.onClick.AddListener(CollectShutDown);
            if (collectSymbol.Level == 2)
            {
                AudioManager.Playback("FE_EndCoinCollectActive");
                collectSymbol.UpCollectOrder();
                collectSymbol.PlayCollectAnim(true);
                foreach (var beCollect in levelCoin[1])
                {
                    beCollect.DisplayGlow(true);
                }
                yield return new WaitForSeconds(1.3f);
                foreach (var beCollect in levelCoin[1])
                {
                    AudioManager.Playback("FE_EndCoinCollect_a");
                    FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                    flySymbol.SetSortingOrder(11000);
                    float flyTime = flySymbol.PlayCollectFlyAnim(beCollect.Level, beCollect.transform.position, collectSymbol.transform.position);
                    DelayCallback.Delay(this, flyTime, () =>
                    {
                        collectSymbol.SetScore(collectSymbol.ScoreValue += beCollect.ScoreValue);
                    });
                    yield return new WaitForSeconds(isCollectShutDown ? intervalTime : (20 / 30f));
                    beCollect.DisplayGlow(false);
                }
                yield return new WaitForSeconds(15 / 30f);
                collectSymbol.PlayCollectAnim(false);
                collectSymbol.UpSortingOrder();
                yield return new WaitForSeconds(20 / 30f);
            }
            UIController.BottomPanel.StopBtn.onClick.RemoveListener(CollectShutDown);
        }

        /*foreach (var collectSymbol in needCollectSymbols)
        {
            if (collectSymbol.Level == 3)
            {
                collectSymbol.PlayTobeCollectAnim();
            }
        }*/
        foreach (var collectSymbol in needCollectSymbols)
        {
            isCollectShutDown = false;
            UIController.BottomPanel.StopBtn.interactable = true;
            UIController.BottomPanel.StopBtn.onClick.AddListener(CollectShutDown);
            if (collectSymbol.Level == 3)
            {
                AudioManager.Playback("FE_EndCoinCollectActive");
                collectSymbol.UpCollectOrder();
                collectSymbol.PlayCollectAnim(true);
                foreach (var beCollect in bonusCoinSymbols)
                {
                    if (beCollect.Level != 3)
                    {
                        beCollect.DisplayGlow(true);
                    }
                }
                yield return new WaitForSeconds(1.5f);
                foreach (var beCollect in bonusCoinSymbols)
                {
                    if (beCollect.Level != 3)
                    {
                        yield return new WaitForSeconds(isCollectShutDown?0: (15 / 30f));
                        if (beCollect.Level == 1)
                        {
                            AudioManager.Playback("FE_EndCoinCollect_b");
                        }
                        else
                        {
                            AudioManager.Playback("FE_EndCoinCollect_c");
                        }
                        FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                        flySymbol.SetSortingOrder(11000);
                        float flyTime = flySymbol.PlayCollectFlyAnim(beCollect.Level, beCollect.transform.position, collectSymbol.transform.position);
                        DelayCallback.Delay(this, flyTime, () =>
                        {
                            collectSymbol.SetScore(collectSymbol.ScoreValue += beCollect.ScoreValue);
                        });
                        yield return new WaitForSeconds(5 / 30f);
                        beCollect.DisplayGlow(false);
                        yield return new WaitForSeconds(isCollectShutDown ? (5 / 30f) : (10 / 30f));
                    }
                }
                collectSymbol.PlayCollectAnim(false);
                collectSymbol.UpSortingOrder();
            }
            UIController.BottomPanel.StopBtn.onClick.RemoveListener(CollectShutDown);
        }
    }

    void CollectShutDown()
    {
        isCollectShutDown = true;
    }

    public void OnCoinClick(int coinLevel)
    {
        StartCoroutine(_OnCoinClick(coinLevel));
    }
    IEnumerator _OnCoinClick(int coinLevel)
    {
        if (canChangeLevelPos[coinLevel])
        {
            canChangeLevelPos[coinLevel] = false;
            int coinTypeCount = 0;
            foreach (var item in coinCounts)
            {
                if (item != 0)
                {
                    coinTypeCount++;
                }
            };
            IntroCoinPos[coinLevel - 1].transform.position = NormPos[coinTypeCount].position;
        }
        FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
        flySymbol.PlayCoinUpAnim(coinLevel, reels[7].GetVisibleSymbols()[0].transform.position, IntroCoinPos[coinLevel - 1].transform.position);
        yield return new WaitForSeconds(14 / 30f);
        UpdateCoinCounts(coinLevel, 1);
        IntroCoinPos[coinLevel - 1].PlayFlyHitAnim();
    }

    private void UpdateCoinCounts(int level = 1, int count = 0)
    {
        coinCounts[level - 1] += count;
        for (int i = 0; i < coinCountTxts.Length; i++)
        {
            if (coinCounts[i] == 0)
            {
                IntroCoinPos[i].gameObject.SetActive(false);
            }
            else
            {
                IntroCoinPos[i].gameObject.SetActive(true);
                coinCountTxts[i].SetContent(coinCounts[i].ToString());
            }
        }
    }

    protected override string[] GetReelStripe(int id)
    {
        return ConfigManager.reelStripConfig.GetStripe("Bonus")[id];
    }

    private List<int> GetSpinResultSequence()
    {
        List<int> result = new List<int>();
        if (resultSequence.Count >= 2)
        {
            result.Add(resultSequence[0]);
            for (int i = 1; i < resultSequence.Count; i++)
            {
                if (resultSequence[i] >= resultSequence[i - 1])
                {
                    result.Add(resultSequence[i]);
                }
                else
                {
                    break;
                }
            }
        }
        else if (resultSequence.Count == 1)
        {
            result.Add(resultSequence[0]);
        }

        int level1Count = 0;
        int level2Count = 0;
        int upgrade1Count = 0;
        int upgrade2Count = 0;
        foreach (var item in exisCoinViews)
        {
            if ((reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).Level == 1)
            {
                level1Count++;
            }
            if ((reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).Level == 2)
            {
                level2Count++;
            }
        }
        foreach (var item in result)
        {
            if (item == 2)
            {
                upgrade1Count++;
            }
            if (item == 3)
            {
                upgrade2Count++;
            }
        }
        if (upgrade1Count > level1Count || upgrade2Count > level2Count)
        {
            result = result.GetRange(0, 1);
        }

        if (result.Count > 0)
        {
            result = result.GetRange(0, Random.Range(0, result.Count + 1));
        }
        string info = "";
        foreach (var item in result)
        {
            info += item.ToString();
        }
        Debug.Log("result:   " + info);
        return result;
    }

    private List<int> GetUpgradeCoin()
    {
        List<int> coinAbleList = new List<int>();
        List<int> level1Able = new List<int>();
        List<int> level2Able = new List<int>();
        Dictionary<int, List<int>> coinLevelCount = new Dictionary<int, List<int>>();
        coinLevelCount.Add(1, new List<int>());
        coinLevelCount.Add(2, new List<int>());
        coinLevelCount.Add(3, new List<int>());
        foreach (var item in exisCoinViews)
        {
            BonusCoinSymbol symbol = reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
            if (symbol.Level == 1)
            {
                level1Able.Add(item);
            }
            if (symbol.Level == 2)
            {
                level2Able.Add(item);
            }
            coinLevelCount[symbol.Level].Add(item);
        }
        foreach (var item in spinResultSequence)
        {
            if (item == 2)
            {
                int reelID = level1Able[Random.Range(0, level1Able.Count)];
                coinAbleList.Add(reelID);
                level1Able.Remove(reelID);
            }
            if (item == 3)
            {
                int reelID = level2Able[Random.Range(0, level2Able.Count)];
                coinAbleList.Add(reelID);
                level2Able.Remove(reelID);
            }
        }
        /*if (coinLevelCount[1].Count > coinLevelAssembly[2])
        {
            for (int i = 0; i < coinLevelCount[1].Count - coinLevelAssembly[2]; i++)
            {
                int reelID = level1Able[Random.Range(0, level1Able.Count)];
                coinAbleList.Add(reelID);
                level1Able.Remove(reelID);
            }
        }
        if (coinLevelCount[2].Count > coinLevelAssembly[1])
        {
            for (int i = 0; i < coinLevelCount[2].Count - coinLevelAssembly[1]; i++)
            {
                int reelID = level2Able[Random.Range(0, level2Able.Count)];
                coinAbleList.Add(reelID);
                level2Able.Remove(reelID);
            }
        }*/

        //调试信息
        string info = "";
        foreach (var item in coinAbleList)
        {
            info += item.ToString() + "   ";
        }
        Debug.Log("UpgradeAble: " + info);

        return coinAbleList;
    }

    public List<int> GetCoinLevelAble()
    {
        List<int> levelPool = new List<int>();
        Dictionary<int, int> coinLevelCount = new Dictionary<int, int>()
        {
            { 1, 0},{ 2, 0},{ 3, 0}
        };
        foreach (var item in exisCoinViews)
        {
            BonusCoinSymbol symbol = reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
            coinLevelCount[symbol.Level]++;
        }
        for (int i = 0; i < coinLevelAssembly[0] - coinLevelCount[3]; i++)
        {
            //float rand = Random.Range(0, 1f);
            if (coinLevelAssembly[2] > coinLevelCount[1])
            {
                levelPool.Add(1);
            }
            else
            {
                if (Random.Range(0, 1f) < ConfigManager.bonusConfig.SilverCoinProbability)
                {
                    levelPool.Add(1);
                }
                else
                {
                    levelPool.Add(2);
                }
            }
        }
        for (int i = 0; i < coinLevelAssembly[1] - coinLevelCount[2]; i++)
        {
            if (Random.Range(0, 1f) < ConfigManager.bonusConfig.SilverCoinProbability)
            {
                levelPool.Add(1);
            }
            else
            {
                levelPool.Add(2);
            }
        }
        for (int i = 0; i < coinLevelAssembly[2] - coinLevelCount[1]; i++)
        {
            levelPool.Add(1);
        }

        //调试信息
        string info = "";
        foreach (var item in levelPool)
        {
            info += item.ToString() + "   ";
        }
        Debug.Log("LevelAble: " + info);

        return levelPool;
    }

    public bool CheckResultComplete()
    {
        if (resultSequence.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
        /*Dictionary<int, int> coinLevelCount = new Dictionary<int, int>()
        {
            { 1, 0},{ 2, 0},{ 3, 0}
        };
        foreach (var item in exisCoinViews)
        {
            BonusCoinSymbol symbol = reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
            coinLevelCount[symbol.Level]++;
        }
        if (coinLevelCount[1] == coinLevelAssembly[2] && coinLevelCount[2] == coinLevelAssembly[1] && coinLevelCount[3] == coinLevelAssembly[0])
        {
            return true;
        }
        else
        {
            return false;
        }*/
    }
}
