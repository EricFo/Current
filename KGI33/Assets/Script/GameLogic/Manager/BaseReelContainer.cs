using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Evaluate;
using SlotGame.Reel.Args;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using UniversalModule.SpawnSystem;
using Random = UnityEngine.Random;

public class BaseReelContainer : ReelContainer
{
    public override event Action<IReelState> OnSpinReelListener;
    public override event Action<IReelState> OnStopReelListener;
    public override event Action<ReelContainer> OnSpinAllListener;
    public override event Action<ReelContainer> OnStopAllListener;

    public bool isPayCoin = false;
    public bool isViewCoin = false;
    public bool isScatter = false;
    public bool isHyper = false;
    public bool isUpgrade = false;
    public bool isReverse = false;
    public ReelSpinArgs reverseArg;
    int totalCoinCredit = 0;
    public CattleUI cattleUI;
    public ParticleSystem reelStar;
    public GameObject cowHint;

    public int flyCount = 0;
    public float triggerAnimTime = 0;

    public override void Initialize()
    {
        base.Initialize();
        cattleUI.Initialize();
        /*DelayCallback.Delay(this, 1f, () =>
        {
            cattleUI.DisplayBigCattle(true);
        });
        DelayCallback.Delay(this, 7f, () =>
        {
            cattleUI.DisplayBigCattle(false);
        });*/
    }

    public override void Spin()
    {
        isReverse = false;
        isHyper = false;
        isUpgrade = false;
        totalCoinCredit = 0;
        MaxScatterCount = 0;
        ScatterIsValid = false;
        isPayCoin = false;
        isViewCoin = false;
        isScatter = false;
        foreach (var reel in reels)
        {
            reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
            if (reel.GetVisibleSymbols()[0].ItemName is SymbolNames.SCATTER)
            {
                (reel.GetVisibleSymbols()[0] as ScatterSymbol).UpSortingOrder(false);
            }
            if (reel.GetVisibleSymbols()[0].ItemName is SymbolNames.COIN)
            {
                (reel.GetVisibleSymbols()[0] as CoinSymbol).UpSortingOrder(false);
            }
        }
        int[] stripeId = new int[5];
        for (int i = 0; i < 5; i++)
        {
            stripeId[i] = Random.Range(0, ConfigManager.reelStripConfig.Stripes["Base"][0].Length);
        }
        Dictionary<int, ReelSpinArgs> reelsPredict = new Dictionary<int, ReelSpinArgs>();
        for (int i = 0; i < reels.Length; i++)
        {
            ReelSpinArgs reelSpinArgs = Predict(i);
            reelSpinArgs.stripeID = stripeId[i % 5];
            reelsPredict.Add(i, reelSpinArgs);
        }
        reelsPredict[new List<int> { 1, 6, 11 }.GetRandomItem()].resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultCoinSymbolsProbability[0])];
        reelsPredict[new List<int> { 2, 12 }.GetRandomItem()].resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultCoinSymbolsProbability[1])];
        List<int> reelID = new List<int>() { 3, 8, 13 };
        int index = reelID.GetRandomItem();
        reelsPredict[index].resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultCoinSymbolsProbability[2])];
        reelID.Remove(index);
        reelsPredict[reelID.GetRandomItem()].resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultCoinSymbolsProbability[3])];
        reelsPredict[new List<int> { 4, 9, 14 }.GetRandomItem()].resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultCoinSymbolsProbability[4])];
        reelsPredict[7].resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultCoinSymbolsProbability[5])];
        reverseArg = reelsPredict[7];
        //reelsPredict[7].resultSymbols[0] = SymbolNames.SCATTER;

        //Cheat
        foreach (var item in reelsPredict)
        {
            if (Cheat.isTriggerBonusGame)
            {
                System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerBonusGame + Random.Range(0, 5).ToString(), item.Key), 0, item.Value.resultSymbols, 0, 1);
            }
            if (Cheat.isUpgradeCattle)
            {
                System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.UpgradeCattle, item.Key), 0, item.Value.resultSymbols, 0, 1);
            }
            if (Cheat.isBigPayout)
            {
                System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.BigPayout, item.Key), 0, item.Value.resultSymbols, 0, 1);
            }
            if (Cheat.isMini || Cheat.isMinor || Cheat.isMajor || Cheat.isGrand)
            {
                System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, item.Key), 0, item.Value.resultSymbols, 0, 1);
            }
        }

        bool isReelStar = false;
        if (reelsPredict[7].resultSymbols[0] == SymbolNames.SCATTER && Random.Range(0, 1f) < ConfigManager.baseConfig.BonusTriggerProbability)
        {
            GlobalObserver.IsTriggerBonusGame = true;
            if (Random.Range(0, 1f) < 0.5f)
            {
                isReelStar = true;
            }
        }

        int totalSymbolWin = 0;
        bool willSelectJackpot = true;
        foreach (var item in reelsPredict)
        {
            if (item.Value.resultSymbols[0] == SymbolNames.COIN)
            {
                item.Value.coinLevel = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinLevelProbability[item.Key % 5]) + 1;
                int index2 = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][item.Value.coinLevel]);
                if (item.Key % 5 == 3 && item.Value.coinLevel == 3)
                {
                    index2 = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][4]);
                }
                item.Value.coinScore = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][item.Value.coinLevel][index2];
                /*if (item.Value.coinLevel == 3 && index2 > (ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 2 - 1))
                {
                    //item.Value.coinScore = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][item.Value.coinLevel][index2];
                    item.Value.coinJPType = GlobalObserver.IDJackpotDic[index2 - (ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 2)];
                }*/
                if (item.Key % 5 == 1 && item.Value.coinLevel == 3)
                {
                    item.Value.coinJPType = GlobalObserver.IDJackpotDic[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.JackpotProbability[0])];
                }
                if (item.Key % 5 == 3 && item.Value.coinLevel == 3&&willSelectJackpot)
                {
                    item.Value.coinJPType = GlobalObserver.IDJackpotDic[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.JackpotProbability[1])];
                    willSelectJackpot = false;
                }
                if (item.Value.coinJPType != JackpotType.Null)
                {
                    item.Value.coinScore = (int)(GlobalObserver.Meters[3 - GlobalObserver.JackpotIDDic[item.Value.coinJPType]] * 100);
                }

                if (Cheat.isBigPayout)
                {
                    item.Value.coinLevel = 3;
                    item.Value.coinScore = 2500;
                    item.Value.coinJPType = JackpotType.Null;
                }
            }
        }
        Cheat.isBigPayout = false;

        if (Cheat.isGrand)
        {
            reelsPredict[8].coinLevel = 3;
            reelsPredict[8].coinScore = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][3][ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 1];
            reelsPredict[8].coinJPType = JackpotType.Grand;
            Cheat.isGrand = false;
        }
        if (Cheat.isMajor)
        {
            reelsPredict[8].coinLevel = 3;
            reelsPredict[8].coinScore = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][3][ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 2];
            reelsPredict[8].coinJPType = JackpotType.Major;
            Cheat.isMajor = false;
        }
        if (Cheat.isMinor)
        {
            reelsPredict[8].coinLevel = 3;
            reelsPredict[8].coinScore = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][3][ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 3];
            reelsPredict[8].coinJPType = JackpotType.Minor;
            Cheat.isMinor = false;
        }
        if (Cheat.isMini)
        {
            reelsPredict[8].coinLevel = 3;
            reelsPredict[8].coinScore = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][3][ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4];
            reelsPredict[8].coinJPType = JackpotType.Mini;
            Cheat.isMini = false;
        }

        foreach (var item in reelsPredict)
        {
            totalCoinCredit += item.Value.coinScore;
        }
        //reelsPredict[6].coinJPType = JackpotType.Major;
        //reelsPredict[8].coinJPType = JackpotType.Grand;
        //reelsPredict[9].coinJPType = JackpotType.Minor;

        int totalCoinCount = 0;
        string[][] symbols = new string[5][];
        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i] = new string[3];
        }
        for (int i = 0; i < reelsPredict.Count; i++)
        {
            symbols[i % 5][i / 5] = reelsPredict[i].resultSymbols[0];
            if (reelsPredict[i].resultSymbols[0] == SymbolNames.COIN)
            {
                totalCoinCount++;
            }
        }
        totalSymbolWin = PayTableEvaluate.Compensate(symbols) + (reelsPredict[7].resultSymbols[0] == SymbolNames.SCATTER ? totalCoinCredit : 0);
        Debug.Log(totalSymbolWin);

        if (totalSymbolWin > GlobalObserver.BetValue * 15 || isReelStar)
        {
            //reelStar.Play();
            DelayCallback.Delay(this, 1f, () =>
            {
                cowHint.SetActive(false);
                cowHint.SetActive(true);
                AudioManager.Playback("FE_BigWinAward");
            });
            UIController.BottomPanel.DisableAllButtons();
            foreach (var item in reelsPredict)
            {
                item.Value.isLong = true;
            }
        }
        if ((reelsPredict[7].resultSymbols[0] == SymbolNames.SCATTER ? totalCoinCredit : 0) > GlobalObserver.BetValue * ConfigManager.baseConfig.ReelReverseBetMultiplier && Random.Range(0, 1f) < ConfigManager.baseConfig.ReelReverseProbability)
        {
            isReverse = true;
            //reelsPredict[7].isReverse = true;
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
        }
        /*if (totalCoinCredit > GlobalObserver.BetValue * 5 || totalCoinCount > 3)
        {
            isHyper = true;
            reelsPredict[7].isHyper = true;
        }*/


        if (isRolling == false)
        {
            OnReset();
            isRolling = true;
            OnSpinAllListener?.Invoke(this);
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].Spin(reelsPredict[i]);
                OnSpinReelListener?.Invoke(reels[i]);
            }
        }
    }

    protected override void OnReelStop(ReelBase reel)
    {

        if (reel.GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
        {
            ScatterSymbol scatterSymbol = reel.GetVisibleSymbols()[0] as ScatterSymbol;
            scatterSymbol.UpSortingOrder();
            scatterSymbol.OnStop();
            AudioManager.Playback("FE_SpecialStop");
            isScatter = true;
            StartCoroutine(CoinFly(reel.GetVisibleSymbols()[0] as ScatterSymbol));
        }
        else if (reel.GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
        {
            isViewCoin = true;
            (reel.GetVisibleSymbols()[0] as CoinSymbol).UpSortingOrder();
        }
        reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
        stopCount++;
        OnStopReelListener?.Invoke(reel);
        if (stopCount == reels.Length - 1 && isHyper)
        {
            if (!isShutDown)
            {
                AudioManager.Playback("FE_CoinLight");
                for (int i = 0; i < reels.Length; i++)
                {
                    if (i != 7)
                    {
                        if (reels[i].GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
                        {
                            (reels[i].GetVisibleSymbols()[0] as CoinSymbol).PlayGlowAnim();
                        }
                    }
                }
            }
        }
        if (stopCount >= reels.Length)
        {
            StartCoroutine(_OnAllReelStop());
            /*if (isReverse)
            {
                foreach(var item in reels[7].GetAllSymbols())
                {
                    item.SetDisplay(true);
                }
                UIController.BottomPanel.DisableAllButtons();
                reels[7].GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
                reels[7].OnStopListener -= OnReelStop;
                reels[7].OnStopListener -= AllReelStop;
                reels[7].OnStopListener += AllReelStop;
                reverseArg.resultSymbols[0] = SymbolNames.SCATTER;
                (reels[7] as BaseReel).ReverseSpin(reverseArg);
            }
            else
            {
                //检查是否触发coin赔付
                if (reels[7].GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
                {
                    if (isViewCoin)
                    {
                        isPayCoin = true;
                    }
                }
                else
                {
                    for (int i = 0; i < reels.Length; i++)
                    {
                        if (reels[i].GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
                        {
                            (reels[i].GetVisibleSymbols()[0] as CoinSymbol).PlayGlowAnim(false);
                        }
                    }
                }
                isRolling = false;
                OnStopAllListener?.Invoke(this);
            }*/
        }
    }

    IEnumerator _OnAllReelStop()
    {
        if (isReverse)
        {
            yield return cattleUI._PlayBaseAddCattleAnim();
            ScatterSymbol scatterSymbol = reels[7].ReplaceSymbol(SymbolNames.SCATTER, 1) as ScatterSymbol;
            scatterSymbol.SetMaskMode(SpriteMaskInteraction.None);
            scatterSymbol.UpSortingOrder();
            scatterSymbol.OnStop();
            AudioManager.Playback("FE_SpecialStop");
            isScatter = true;
            StartCoroutine(CoinFly(scatterSymbol));
            yield return new WaitForSeconds(0.5f);
        }
        //检查是否触发coin赔付
        if (reels[7].GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
        {
            if (isViewCoin)
            {
                isPayCoin = true;
            }
        }
        else
        {
            for (int i = 0; i < reels.Length; i++)
            {
                if (reels[i].GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
                {
                    (reels[i].GetVisibleSymbols()[0] as CoinSymbol).PlayGlowAnim(false);
                }
            }
        }
        isRolling = false;
        OnStopAllListener?.Invoke(this);
    }

    private void AllReelStop(ReelBase reel)
    {
        reels[7].OnStopListener -= AllReelStop;
        reels[7].OnStopListener -= OnReelStop;
        reels[7].OnStopListener += OnReelStop;
        reel.GetVisibleSymbols()[0].SetMaskMode(SpriteMaskInteraction.None);
        if (reel.GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
        {
            ScatterSymbol scatterSymbol = reel.GetVisibleSymbols()[0] as ScatterSymbol;
            scatterSymbol.UpSortingOrder();
            scatterSymbol.OnStop();
            AudioManager.Playback("FE_SpecialStop");
            isScatter = true;
            StartCoroutine(CoinFly(reel.GetVisibleSymbols()[0] as ScatterSymbol));
        }
        //检查是否触发coin赔付
        if (reels[7].GetVisibleSymbols()[0].ItemName == SymbolNames.SCATTER)
        {
            if (isViewCoin)
            {
                isPayCoin = true;
            }
        }
        else
        {
            for (int i = 0; i < reels.Length; i++)
            {
                if (reels[i].GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
                {
                    (reels[i].GetVisibleSymbols()[0] as CoinSymbol).PlayGlowAnim(false);
                }
            }
        }
        isRolling = false;
        OnStopAllListener?.Invoke(this);
    }


    IEnumerator CoinFly(ScatterSymbol scatterSymbol)
    {
        triggerAnimTime = cattleUI.PlayTriggerAnim(false);
        AudioManager.Playback("FE_SpecialFly");
        scatterSymbol.PlayHitAnim();
        yield return new WaitForSeconds(3 / 30f);
        FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
        flySymbol.PlayCoinFlyAnim(scatterSymbol.transform.position + new Vector3(1.45f, 0.8f), new Vector3(0, 2.5f));
        flyCount++;
        if (GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame)
        {
            yield return new WaitForSeconds(23 / 30f);
            cattleUI.PlayTriggerAnim();
            yield break;
        }
        isUpgrade = (flyCount >= ConfigManager.baseConfig.CoinCountOfUpgrade[cattleUI.Level] || Cheat.isUpgradeCattle) && cattleUI.Level < 4;
        yield return new WaitForSeconds(23 / 30f);
        if (isUpgrade)
        {
            flyCount = 0;
            cattleUI.PlayCattleChangeAnim();
            cattleUI.PlayPurseChangeAnim();
            cattleUI.Level++;
            yield return new WaitForSeconds(40 / 30f);
            cattleUI.PlayCattleLoopAnim();
            yield return new WaitForSeconds(20 / 30f);
            cattleUI.PlayPurseLoopAnim();
        }
        else
        {
            cattleUI.PlayPurseShakeAnim();
            cattleUI.PlayCattleHitAnim();
            cattleUI.PlayPurseHitAnim();
            yield return new WaitForSeconds(30 / 30f);
            cattleUI.PlayPurseLoopAnim();
            yield return new WaitForSeconds(10 / 30f);
            cattleUI.PlayCattleLoopAnim();
        }
        Cheat.isUpgradeCattle = false;
    }

    protected override ReelSpinArgs Predict(int id)
    {
        var reelSpinArgs = base.Predict(id);
        reelSpinArgs.resultSymbols[0] = ConfigManager.baseConfig.ResultSymbols[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ResultSymbolsProbability[id % 5])];
        //要作弊就直接替换下最终结果
        if (Cheat.isTriggerJackPot)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isTriggerFreeGame)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isFreeAndJackPot)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isFreeInJackPot)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        /*if (Cheat.isTriggerBonusGame)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerBonusGame, id), 0, reelSpinArgs.resultSymbols, 0, 1);
        }
        if (Cheat.isUpgradeCattle)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.UpgradeCattle, id), 0, reelSpinArgs.resultSymbols, 0, 1);
        }*/
        return reelSpinArgs;
    }

    protected override string[] GetReelStripe(int id)
    {
        return ConfigManager.reelStripConfig.GetStripe("Base")[id];
    }

    /*public int PayCoinCredit()
    {
        int totalCoinWin = 0;
        if (isPayCoin)
        {
            (reels[7].GetVisibleSymbols()[0] as ScatterSymbol).PlayUpgradeAnim();
            foreach (var item in reels)
            {
                if (item.GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
                {
                    totalCoinWin += (item.GetVisibleSymbols()[0] as CoinSymbol).scoreValue;
                }
            }
        }
        return totalCoinWin;
    }*/
    //IEnumerator _PayCoinCredit()
    //{

    //}

    public void RecordCoinPosLevel()
    {
        GlobalObserver.CoinPosLevel.Clear();
        GlobalObserver.CoinPosScore.Clear();
        for (int i = 0; i < reels.Length; i++)
        {
            if (reels[i].GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
            {
                GlobalObserver.CoinPosLevel.Add(i, (reels[i].GetVisibleSymbols()[0] as CoinSymbol).Level);
                GlobalObserver.CoinPosScore.Add(i, (reels[i].GetVisibleSymbols()[0] as CoinSymbol).ScoreValue);
            }
        }
    }
}
