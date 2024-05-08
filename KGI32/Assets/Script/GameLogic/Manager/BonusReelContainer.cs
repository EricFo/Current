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
using System.Net.Http.Headers;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using System.Reflection;

public class BonusReelContainer : ReelContainer
{
    public override event Action<IReelState> OnSpinReelListener;
    public override event Action<IReelState> OnStopReelListener;
    public override event Action<ReelContainer> OnSpinAllListener;
    public override event Action<ReelContainer> OnStopAllListener;

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

    List<BonusCoinSymbol> upgradeSymbols = new List<BonusCoinSymbol>();


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
        StartCoroutine(InitReplaceSymbolForCoin());
    }
    /// <summary>
    /// 初始化参数
    /// </summary>
    private void InitParameter()
    {
        exisCoinViews.Clear();
        upgradeSymbols.Clear();
        coinsViewCount = 0;
        bonusCoinsViewCount = 0;
        int initLevel3CoinCount = 0;
        foreach (var item in GlobalObserver.CoinPosLevel)
        {
            if (item.Value == 3)
            {
                initLevel3CoinCount++;
            }
        }
        canChangeLevelPos = new Dictionary<int, bool>() { { 1, true }, { 2, true }, { 3, true } };
        initLevel3CoinCount += prizedrawController.initLevel3CoinCount;
        totalCoinCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.EndCoinCountProbability) + 8;
        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.IndexOfEndCoinCountProbability[totalCoinCount][initLevel3CoinCount]);
        int[][] levelAssemblys = ConfigManager.bonusConfig.AssemblyOfEndCoinCount[totalCoinCount][initLevel3CoinCount][index];
        coinLevelAssembly = levelAssemblys[Random.Range(0, levelAssemblys.Length)];
        GlobalObserver.NewResult(string.Format("{0}--{1}--{2}", coinLevelAssembly[0], coinLevelAssembly[1], coinLevelAssembly[2]));
        GlobalObserver.IsGrandBonus = Random.Range(0, 1f) < ConfigManager.bonusConfig.GrandProbability;
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
        }
    }
    /// <summary>
    /// 初始化Symbol，继承Base的Coin
    /// </summary>
    IEnumerator InitReplaceSymbolForCoin()
    {
        UIController.BottomPanel.DisableAllButtons();
        coinCounts = new int[] { 0, 0, 0 };
        UpdateCoinCounts();
        foreach (var item in reels)
        {
            item.ReplaceAllSymbol(SymbolNames.EMPTY);
        }
        foreach (var item in GlobalObserver.CoinPosLevel)
        {
            BonusCoinSymbol symbol = reels[item.Key].ReplaceSymbol(SymbolNames.BCOIN, 1) as BonusCoinSymbol;
            symbol.Level = item.Value;
            symbol.SetScore(0);
            symbol.SetMaskMode(SpriteMaskInteraction.None);
            symbol.UpSortingOrder();
            exisCoinViews.Add(item.Key);
            coinsViewCount++;
        }
        bonusCoinsViewCount = coinsViewCount;

        /*yield return new WaitForSeconds(0.5f);
        foreach (var item in exisCoinViews)
        {
            FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
            BonusCoinSymbol bSymbol = reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
            int level = bSymbol.Level;
            flySymbol.PlayCoinUpAnim(bSymbol.Level, bSymbol.transform.position, IntroCoinPos[bSymbol.Level - 1].position);
            reels[item].ReplaceSymbol(SymbolNames.EMPTY, 1);
            yield return new WaitForSeconds(20 / 30f);
            IntroCoinPos[level - 1].GetComponent<BonusCoinSymbol>().PlayStopAnim();
            UpdateCoinCounts(level, 1);
            //yield return new WaitForSeconds(10 / 30f);
        }*/
        yield return null;
        prizedrawController.InitBonus(totalCoinCount);
    }

    private void OnBonusIntroOverClick()
    {
        StartCoroutine(_IntroOverReplaceSymbolForCoin());
    }
    IEnumerator _IntroOverReplaceSymbolForCoin()
    {
        //cattleUI.PlayPurseLoopAnim();

        //exisCoinViews.Clear();
        //coinsViewCount = 0;
        /*List<int> ableCollection = Enumerable.Range(0, 15).ToList();
        ableCollection.Remove(7);*/
        List<int> ableCollection = new List<int>();
        for (int i = 0; i < reels.Length; i++)
        {
            if (!exisCoinViews.Contains(i) && i != 7)
            {
                ableCollection.Add(i);
            }
        }

        yield return new WaitForSeconds(0.5f);
        /*int coinCount = symbolContainer.Count;
        for (int i = 0; i < coinCount; i++)
        {
            FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
            int reelId = ableCollection.GetRandomItem();
            ableCollection.Remove(reelId);
            CommonSymbol symbol = reels[reelId].GetVisibleSymbols()[0];
            int level = symbolContainer[i].Level;
            flySymbol.PlayCoinUpAnim(level, IntroCoinPos[i].transform.position, symbol.transform.position);
            symbolContainer[i].Recycle();
            yield return new WaitForSeconds(20 / 30f);
            BonusCoinSymbol bSymbol = reels[reelId].ReplaceSymbol(SymbolNames.BCOIN, 1) as BonusCoinSymbol;
            bSymbol.Level = level;
            bSymbol.SetScore(0);
            bSymbol.SetMaskMode(SpriteMaskInteraction.None);
            bSymbol.PlayStopAnim();
            exisCoinViews.Add(reelId);
            coinsViewCount++;
            bonusCoinsViewCount = coinsViewCount;
            yield return new WaitForSeconds(10 / 30f);
        }
        symbolContainer.Clear();*/
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
                    bSymbol.SetScore(0);
                    bSymbol.SetMaskMode(SpriteMaskInteraction.None);
                    bSymbol.UpSortingOrder();
                    bSymbol.PlayStopAnim();
                    //bSymbol.PlayStopAnim();
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
            if (coinsViewCount < totalCoinCount && Random.Range(0, 1f) < ConfigManager.bonusConfig.CoinProbability)
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
                    levelAble.Remove(coinLevel);

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
            reelsPredict[7].resultSymbols[0] = result;
            //reelsPredict[7].resultSymbols[0] = SymbolNames.ADDSPIN1;

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
        base.OnReset();
        reelsPredict.Clear();
        spinCoinCount = 0;
        /*if(upgradeSymbols != null)
        {
            foreach(var upgradeSymbol in upgradeSymbols)
            {
                upgradeSymbol.transform.localScale = Vector3.one;
            }
        }*/
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
            AudioManager.Playback("FE_SpecialStop（bonus）");
        }
        OnStopReelListener?.Invoke(reel);
        if (stopCount >= reels.Length - bonusCoinsViewCount)
        {
            StartCoroutine(_OnAllReelStop());
        }
    }
    IEnumerator _OnAllReelStop()
    {
        switch (reels[7].GetVisibleSymbols()[0].ItemName)
        {
            case SymbolNames.SCATTER:
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
        //if (spinCoinCount > 0)
        //{
        yield return new WaitForSeconds(30 / 30f);
        //}
        (reels[7].GetVisibleSymbols()[0] as ScatterSymbol).PlayBonusUpgradeAnim();
        yield return new WaitForSeconds(20 / 30f);
        List<int> upgradeAble = GetUpgradeCoin();
        int upgradeCount = Random.Range(1, 5);
        if (upgradeCount > upgradeAble.Count)
        {
            upgradeCount = upgradeAble.Count;
        }
        //string info1 = "";
        List<int> upgradeIndex = new List<int>();
        for (int i = 0; i < upgradeCount; i++)
        {
            int index = upgradeAble.GetRandomItem();
            upgradeIndex.Add(index);
            upgradeAble.Remove(index);
        }
        upgradeIndex.Sort();
        foreach(var item in upgradeIndex)
        {
            BonusCoinSymbol bSymbol = (BonusCoinSymbol)reels[item].GetVisibleSymbols()[0];
            bSymbol.PlayUpgradeStopAnim();
            bSymbol.transform.DOScale(Vector3.one * 1.2f, 7 / 30f);
            bSymbol.DisplayGlow(true);
            upgradeSymbols.Add(bSymbol);
            //upgradeAble.Remove(item);
        }
        yield return new WaitForSeconds(30 / 30f);
        foreach(var upgradeSymbol in upgradeSymbols)
        {
            upgradeSymbol.PlayChangeAnim();
            yield return new WaitForSeconds(30 / 30f);
        }
        /*for (int i = 0; i < upgradeCount; i++)
        {
            int index = upgradeAble[Random.Range(0, upgradeAble.Count)];
            (reels[index].GetVisibleSymbols()[0] as BonusCoinSymbol).PlayChangeAnim();
            DelayCallback.Delay(this, 30 / 30f, () =>
            {
                (reels[index].GetVisibleSymbols()[0] as BonusCoinSymbol).Level++;
            });

            upgradeAble.Remove(index);
            yield return new WaitForSeconds(15 / 30f);

            info1 += index + "  ";
        }
        Debug.Log("UpgradeCoins:  " + info1);*/
        yield return new WaitForSeconds(1);
    }

    public void OnCoinClick(int coinLevel)
    {
        /*List<int> ableCollection = new List<int>();
        for (int i = 0; i < reels.Length; i++)
        {
            if (!exisCoinViews.Contains(i) && i != 7)
            {
                ableCollection.Add(i);
            }
        }

        int debugInt = Random.Range(0, ableCollection.Count);
        Debug.Log("ableCollection.Count:   " + ableCollection.Count);
        Debug.Log("Index:   " + debugInt);
        int index = ableCollection[debugInt];
        BonusCoinSymbol symbol = reels[index].ReplaceSymbol(SymbolNames.BCOIN, 1) as BonusCoinSymbol;
        symbol.Level = coinLevel;
        symbol.SetScore(0);
        symbol.SetMaskMode(SpriteMaskInteraction.None);
        exisCoinViews.Add(index);
        coinsViewCount++;
        bonusCoinsViewCount = coinsViewCount;*/
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
        //IntroCoinPos[coinLevel - 1].PlayStopAnim();
        /*BonusCoinSymbol bSymbol = SpawnFactory.GetObject<BonusCoinSymbol>(SymbolNames.BCOIN);
        bSymbol.Install(prizeDraw.transform, IntroCoinPos[coinLevel - 1].position, 0, 1200);
        bSymbol.SetMaskMode(SpriteMaskInteraction.None);
        bSymbol.Level = coinLevel;
        bSymbol.SetScore(0);
        bSymbol.PlayStopAnim();
        symbolContainer.Add(bSymbol);*/
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
        if (coinLevelCount[1].Count > coinLevelAssembly[2])
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
        }

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
        Dictionary<int, int> coinLevelCount = new Dictionary<int, int>()
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
        }
    }

    public int PayCoinCredit()
    {
        int totalCoinWin = 0;
        foreach (var item in reels)
        {
            if (item.GetVisibleSymbols()[0].ItemName == SymbolNames.BCOIN)
            {
                BonusCoinSymbol symbol = item.GetVisibleSymbols()[0] as BonusCoinSymbol;
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][symbol.Level]);
                int score = ConfigManager.bonusConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][symbol.Level][index];
                symbol.SetScore(score);
                totalCoinWin += symbol.ScoreValue;
            }
        }
        return totalCoinWin;
    }
}
