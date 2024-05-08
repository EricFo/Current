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
    /// 当前屏幕上所有Wild的reelId
    /// </summary>
    public List<int> exisWildViews = new List<int>();
    /// <summary>
    /// 上一轮屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public int bonusCoinsViewCount = 0;
    /// <summary>
    /// 上一轮屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public int bonusWildsViewCount = 0;
    /// <summary>
    /// 当前屏幕上BonusCoin的数量
    /// </summary>
    [HideInInspector] public int coinsViewCount = 0;
    /// <summary>
    /// 当前屏幕上Wild的数量
    /// </summary>
    [HideInInspector] public int wildsViewCount = 0;
    /// <summary>
    /// 本轮Bonus总Coin数量
    /// </summary>
    public int totalCoinCount = 0;
    /// <summary>
    /// 本轮Bonus总Wild数量
    /// </summary>
    public int totalWildCount = 0;
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
    public int freeSpinCount;
    public int bigUpgradeMultiplierCount = 0;
    public int scatterCount = 0;
    private int prizedrawWildCount;
    private int introAddSpinCount;
    private bool isFirstSpin;
    List<int> midFlyList = new List<int>();
    List<FlySymbol> flySymbols = new List<FlySymbol>();
    public List<CoinSymbol> coinSymbols = new List<CoinSymbol>();


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
        InitParameter();
        InitReplaceSymbolForCoin();
    }
    /// <summary>
    /// 初始化参数
    /// </summary>
    private void InitParameter()
    {
        isFirstSpin = true;
        willSelectJackpot = true;
        flySymbols.Clear();
        midFlyList.Clear();
        exisCoinViews.Clear();
        exisWildViews.Clear();
        upgradeSymbols.Clear();
        scatterCount = 0;
        coinsViewCount = 0;
        wildsViewCount = 0;
        bonusCoinsViewCount = 0;
        bonusWildsViewCount = 0;
        bigUpgradeMultiplierCount = 0;
        int initLevel3CoinCount = 0;
        totalCoinCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.EndCoinCountProbability) + 7;
        resultSequence = ConfigManager.bonusConfig.ResultSequence[Random.Range(0, ConfigManager.bonusConfig.ResultSequence.Length)].ToList();
        //resultSequence = new List<int>() { 1, 1, 1, 1, 1, 2, 2, 1, 1, 2, 1, 2, 1, 1, 1, 2, 1, 3, 3, 3, 3, 3 };
        if (Cheat.isMoreBonusCoin)
        {
            totalCoinCount = 14;
        }
        prizedrawWildCount = Random.Range(2, 4);
        freeSpinCount = ConfigManager.bonusConfig.FreeSpinCount[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.FreeSpinCountProbability)];
        introAddSpinCount = freeSpinCount - baseUI.CheckRemaining();
        //prizedrawController.InitBonus(prizedrawWildCount, introAddSpinCount);
        resultSequence.RemoveRange(0, prizedrawWildCount);
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
    }

    /// <summary>
    /// 初始化Symbol，继承Base的Coin
    /// </summary>
    void InitReplaceSymbolForCoin()
    {
        StartCoroutine(_InitReplaceSymbolForCoin());
    }

    IEnumerator _InitReplaceSymbolForCoin()
    {
        List<int> ableCollection = new List<int>() { 1, 2, 11, 12 };
        Dictionary<int, Vector3> symbolPositions = new Dictionary<int, Vector3>();
        foreach (var item in ableCollection)
        {
            symbolPositions[item] = reels[item].GetVisibleSymbols()[0].transform.position;
        }
        for (int i = 0; i < prizedrawWildCount; i++)
        {
            int reelId = ableCollection.GetRandomItem();
            midFlyList.Add(reelId);
            ableCollection.Remove(reelId);
        }
        midFlyList.ListSorting();
        if (Cheat.isAutoFree)
        {
            UIController.BottomPanel.OnSpinBtnClick();
            UIController.BottomPanel.StopBtn.interactable = false;
        }
        else
        {
            UIController.BottomPanel.DisplaySpinBtn();
        }
        yield return new WaitForSeconds(30 / 30f);
        cattleUI.BaseToBonus();
        AudioManager.Playback("FE_BagPickOpen");
        cattleUI.PlayPurseIntroAnim();
        DelayCallback.Delay(this, 1.3f, delegate ()
        {
            cattleUI.upperTrigger.Stop();
        });
        yield return new WaitForSeconds(13 / 30f);
        //cattleUI.PlayPurseIntro2Anim();

        //yield return new WaitForSeconds(0.5f);

        foreach (var reelId in midFlyList)
        {
            AudioManager.Playback("FE_CoinFlydown");
            FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
            flySymbols.Add(flySymbol);
            flySymbol.SetSortingOrder(2100 + reelId);
            flySymbol.PlayWildFlyAnim(cattleUI.purseAnim.transform.position + new Vector3(-0.35f, -0.35f), symbolPositions[reelId]);
            yield return new WaitForSeconds(7 / 30f);
        }
    }

    private void OnBonusIntroOverClick()
    {
        StartCoroutine(_IntroOverReplaceSymbolForWild());
    }
    IEnumerator _IntroOverReplaceSymbolForWild()
    {
        List<int> ableCollection = new List<int>() { 1, 2, 11, 12/*, 3, 4, 9, 13, 14*/ };
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
                    WildSymbol wSymbol = reels[reelId].ReplaceSymbol(SymbolNames.WILD, 1) as WildSymbol;
                    wSymbol.SetMaskMode(SpriteMaskInteraction.None);
                    wSymbol.UpSortingOrder();
                    exisWildViews.Add(reelId);
                    wildsViewCount++;
                    bonusWildsViewCount = wildsViewCount;
                    yield return new WaitForSeconds(16 / 30f);
                }
            }
        }
        prizeDraw.SetActive(false);


        UIController.BottomPanel.OnSpinBtnClick();
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
                if (!exisWildViews.Contains(i))
                {
                    if (i != 7)
                    {
                        ableIndex.Add(i);
                    }
                    ReelSpinArgs reelSpinArg = Predict(i);
                    if (isFirstSpin)
                    {
                        reelSpinArg.isFreeStart = true;
                    }
                    reelsPredict.Add(i, reelSpinArg);
                }
                //OnSpinReelListener?.Invoke(reels[i]);
            }

            reelsPredict[7].resultSymbols[0] = ConfigManager.bonusConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.ResultSymbolsProbability[5])];
            if (Random.Range(0, 1f) < ConfigManager.bonusConfig.ScatterCountProbability[scatterCount] || Cheat.isMiniBonus || Cheat.isMinorBonus || Cheat.isMajorBonus || Cheat.isGrandBonus)
            {
                reelsPredict[7].resultSymbols[0] = SymbolNames.SCATTER;
            }
            else if (introAddSpinCount > 0 && Random.Range(0, 1f) > ConfigManager.bonusConfig.AddSpinProbability)
            {
                reelsPredict[7].resultSymbols[0] = SymbolNames.ADDSPIN1;
            }
            if (baseUI.CheckRemaining() == 0 && introAddSpinCount > 0)
            {
                reelsPredict[7].resultSymbols[0] = SymbolNames.ADDSPIN1;
            }
            for (int i = 0; i < 2; i++)
            {
                List<int> ableIndex2 = new List<int>();
                foreach (var item in reelsPredict)
                {
                    if (item.Key % 5 == 1 + i * 2 && !midFlyList.Contains(item.Key))
                    {
                        ableIndex2.Add(item.Key);
                    }
                }
                int index = ableIndex2.GetRandomItem();
                reelsPredict[index].resultSymbols[0] = ConfigManager.bonusConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.ResultSymbolsProbability[6 + i])];
                if (reelsPredict[index].resultSymbols[0] == SymbolNames.BCOIN)
                {
                    reelsPredict[index].coinJPType = GlobalObserver.IDJackpotDic[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.JackpotProbability[i])];
                }

                if (!isFirstSpin)
                {
                    if (Cheat.isMiniBonus)
                    {
                        reelsPredict[index].resultSymbols[0] = SymbolNames.BCOIN;
                        reelsPredict[index].coinJPType = JackpotType.Mini;
                        Cheat.isMiniBonus = false;
                    }
                    if (Cheat.isMinorBonus)
                    {
                        reelsPredict[index].resultSymbols[0] = SymbolNames.BCOIN;
                        reelsPredict[index].coinJPType = JackpotType.Minor;
                        Cheat.isMinorBonus = false;
                    }
                    if (Cheat.isMajorBonus)
                    {
                        reelsPredict[index].resultSymbols[0] = SymbolNames.BCOIN;
                        reelsPredict[index].coinJPType = JackpotType.Major;
                        Cheat.isMajorBonus = false;
                    }
                    if (Cheat.isGrandBonus)
                    {
                        reelsPredict[index].resultSymbols[0] = SymbolNames.BCOIN;
                        reelsPredict[index].coinJPType = JackpotType.Grand;
                        Cheat.isGrandBonus = false;
                    }
                }
            }
            if (isFirstSpin)
            {
                foreach (var item in midFlyList)
                {
                    reelsPredict[item].resultSymbols[0] = SymbolNames.WILD;
                }
            }

            reverseArg = reelsPredict[7];
            if (reelsPredict[7].resultSymbols[0] == SymbolNames.SCATTER && Random.Range(0, 1f) < ConfigManager.bonusConfig.ReelReverseProbability)
            {
                isReverse = true;
                List<string> ableSymbols = new List<string>();
                string[] allSymbols = new string[] { "PIC01", "PIC02", "PIC03", "PIC04", "PIC05", "A", "K", "Q", "J", "T", "N" };
                foreach (var symbol in allSymbols)
                {
                    if (symbol != reelsPredict[0].resultSymbols[0] && symbol != reelsPredict[5].resultSymbols[0] && symbol != reelsPredict[10].resultSymbols[0])
                    {
                        ableSymbols.Add(symbol);
                    }
                }
                reelsPredict[7].resultSymbols[0] = ableSymbols.GetRandomItem();
                reelsPredict[7].isReverse = true;
            }

            foreach (var item in reelsPredict.Keys)
            {
                reels[item].GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
                if (reels[item].GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
                {
                    (reels[item].GetVisibleSymbols()[0] as ScatterSymbol).UpSortingOrder(false);
                }
                if (reels[item].GetVisibleSymbols()[0].ItemName == SymbolNames.BCOIN)
                {
                    (reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).UpSortingOrder(false);
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
        exisCoinViews.Clear();
        coinsViewCount = 0;
        coinSymbols.Clear();
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
        if (reel.GetVisibleSymbols()[0].ItemName == SymbolNames.WILD)
        {
            if (!midFlyList.Contains(reel.InstallID))
            {
                AudioManager.Playback("FE_wildstop");
                (reel.GetVisibleSymbols()[0] as WildSymbol).PlayStopAnim();
            }
            exisWildViews.Add(reel.InstallID);
            wildsViewCount++;
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
        if (stopCount >= reels.Length - bonusWildsViewCount)
        {
            if (isFirstSpin)
            {
                foreach (var item in flySymbols)
                {
                    item.Recycle();
                }
                isFirstSpin = false;
            }
            //StartCoroutine(_OnAllReelStop());
            if (isReverse)
            {
                StartCoroutine(_ReverseSpin());
            }
            else
            {
                isRolling = false;
                StartCoroutine(_OnAllReelStop());
            }
        }
    }

    IEnumerator _ReverseSpin()
    {
        AudioManager.Playback("FE_SpecialBigPrompt");
        StartCoroutine( cattleUI._PlayBonusAddCattleAnim(reels));
        yield return new WaitForSeconds(71 / 30f);
        foreach (var item in reels[7].GetAllSymbols())
        {
            item.SetDisplay(true);
        }
        UIController.BottomPanel.DisableAllButtons();
        reels[7].GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
        reels[7].OnStopListener -= OnReelStop;
        reels[7].OnStopListener -= AllReelStop;
        reels[7].OnStopListener += AllReelStop;
        reverseArg.resultSymbols[0] = SymbolNames.SCATTER;
        (reels[7] as ReelBonus).ReverseSpin(reverseArg);
    }

    void AllReelStop(ReelBase reel)
    {
        foreach (var item in reels)
        {
            if (item.GetVisibleSymbols()[0].ItemName == SymbolNames.WILD)
            {
                (item.GetVisibleSymbols()[0] as WildSymbol).UpSortingOrder();
            }
        }
        reels[7].OnStopListener -= OnReelStop;
        reels[7].OnStopListener += OnReelStop;
        reels[7].OnStopListener -= AllReelStop;
        ScatterSymbol scatterSymbol = reel.GetVisibleSymbols()[0] as ScatterSymbol;
        scatterSymbol.UpSortingOrder();
        scatterSymbol.OnStop();
        scatterSymbol.SetMaskMode(SpriteMaskInteraction.None);
        AudioManager.Playback("FE_SpecialStop Bonus");
        
        isRolling = false;
        StartCoroutine(_OnAllReelStop());
    }

    IEnumerator _OnAllReelStop()
    {
        /*if (isReverse)
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
        }*/
        switch (reels[7].GetVisibleSymbols()[0].ItemName)
        {
            case SymbolNames.SCATTER:
                scatterCount++;
                if (scatterCount > 8)
                {
                    Debug.LogError("scatter out of range");
                }
                cattleUI.PlayCattleTriggerAnim();
                yield return new WaitForSeconds(0.5f);
                //yield return _UpgradeCoin();
                //yield return _PayJackpot();
                yield return _UpgradeWild();
                yield return new WaitForSeconds(0.5f);
                break;
            case SymbolNames.EMPTY:
                break;
            case SymbolNames.ADDSPIN1:
                introAddSpinCount--;
                AddSpinSymbol addSymbol = reels[7].GetVisibleSymbols()[0] as AddSpinSymbol;
                addSymbol.PlayFlyAnim();
                yield return new WaitForSeconds(56 / 30f);
                baseUI.AddSurplus(1);
                reels[7].ReplaceSymbol(ConfigManager.bonusConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.ResultSymbolsProbability[5])], addSymbol);
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
        bonusWildsViewCount = wildsViewCount;

        //调试信息
        string info = "";
        foreach (var item in exisCoinViews)
        {
            info += item.ToString() + "   ";
        }
        Debug.Log("ViewCoins: " + info);
    }

    IEnumerator _PayJackpot()
    {
        if (coinsViewCount > 0)
        {
            yield return new WaitForSeconds(0.5f);
            foreach (var item in exisCoinViews)
            {
                yield return JackPot.PlayJackpotCele(reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator _UpgradeWild()
    {
        List<int> upgradeAble = new(exisWildViews);
        List<int> upgradeIndex = new List<int>();
        int upgradeCount = ConfigManager.bonusConfig.UpgradeCount[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.UpgradeCountProbability[scatterCount > 2 ? 1 : 0])];
        if (upgradeCount > wildsViewCount)
        {
            upgradeCount = wildsViewCount;
        }
        for (int i = 0; i < upgradeCount; i++)
        {
            int index = upgradeAble.GetRandomItem();
            upgradeIndex.Add(index);
            upgradeAble.Remove(index);
        }
        upgradeIndex.ListSorting();
        ScatterSymbol scatter = (ScatterSymbol)reels[7].GetVisibleSymbols()[0];
        scatter.PlayBonusUpgradeAnim(true);
        yield return new WaitForSeconds(20 / 30f);
        for (int i = 0; i < reels.Length; i++)
        {
            if (upgradeIndex.Contains(i) || exisCoinViews.Contains(i))
            {
                if (reels[i].GetVisibleSymbols()[0].ItemName == SymbolNames.WILD)
                {
                    WildSymbol wildSymbol = (WildSymbol)reels[i].GetVisibleSymbols()[0];
                    FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                    flySymbol.PlayMultiplierFlyAnim(scatter.transform.position + Vector3.up * 0.5f, wildSymbol.transform.position);
                    yield return new WaitForSeconds(13 / 30f);
                    int multiplier = ConfigManager.bonusConfig.UpgradeMultiplier[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.UpgradeMultiplierProbability[bigUpgradeMultiplierCount < 3 ? 0 : 1])];
                    bigUpgradeMultiplierCount += multiplier == 3 ? 1 : 0;
                    wildSymbol.PlayMultiplierChangeAnim(multiplier);
                    yield return new WaitForSeconds(2 / 30f);
                }
                else
                {
                    CoinSymbol coinSymbol = (CoinSymbol)reels[i].GetVisibleSymbols()[0];
                    coinSymbols.Add(coinSymbol);
                    FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                    flySymbol.PlayMultiplierFlyAnim(scatter.transform.position + Vector3.up * 0.5f, coinSymbol.transform.position);
                    yield return new WaitForSeconds(13 / 30f);
                    coinSymbol.PlayWinAnim();
                    yield return new WaitForSeconds(2 / 30f);
                }
            }
        }
        scatter.PlayBonusUpgradeAnim(false);
        yield return new WaitForSeconds(15 / 30f);
        scatter.UpSortingOrder(false);
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

    protected override ReelSpinArgs Predict(int id)
    {
        var reelSpinArgs = base.Predict(id);
        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.ResultSymbolsProbability[id % 5]);
        reelSpinArgs.resultSymbols[0] = ConfigManager.bonusConfig.ResultSymbols[index];
        return reelSpinArgs;
    }
}
