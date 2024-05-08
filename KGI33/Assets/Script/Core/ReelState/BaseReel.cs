using DG.Tweening;
using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Reel.Args;
using SlotGame.Symbol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UniversalModule.AudioSystem;
using UniversalModule.SpawnSystem;

public class BaseReel : ReelBase
{
    /*public override event Action<ReelBase> OnStepListener;
    public override event Action<ReelBase> OnStopListener;

    public void ReverseSpin(ReelSpinArgs args)
    {
        isReverse = true;
        currMoveStep = 0;
        isShutDown = false;
        reelSpinArgs = args;
        stripeID = args.stripeID;
        refreshPoint = new Vector3(0, setting.moveDistance * -3);
        Vector3 endPoint = Vector3.up * setting.moveDistance;
        maxMoveStep = setting.moveStep + (args.isHyper ? args.hyperStep : 0) + (args.isLong ? args.longStep : 0);
        //transform.DOLocalMove(endPoint, setting.moveSpeed * 5).SetRelative().SetEase(setting.beginCurve).OnComplete(ReverseRolling);
        ReverseRolling();
        DisplayTheInvisibleSymbol(true);
    }*/

    /*public override void Spin(ReelSpinArgs args)
    {
        isReverse = false;
        base.Spin(args);
    }

    protected void ReverseRolling()
    {
        if (!IsInfinity)
        {
            currMoveStep++;
        }
        ReverseRefreshSymbol();
        if (currMoveStep < maxMoveStep)
        {
            Vector3 endPoint = Vector3.up * setting.moveDistance;
            if (currMoveStep == maxMoveStep - 1)
            {
                transform.DOLocalMove(endPoint, setting.moveSpeed * 4).SetRelative().SetEase(setting.reverseCurve).OnComplete(ReverseRolling);
                //transform.DOLocalMove(endPoint, setting.moveSpeed * 5).SetRelative().SetEase(setting.finishCurve).OnComplete(ReverseRolling);
            }
            else
            {
            float speed = setting.moveSpeed;
            if (currMoveStep >= setting.moveStep + reelSpinArgs.WaitStep + (reelSpinArgs.isLong ? reelSpinArgs.longStep : 0) && !isShutDown)
            {
                speed = setting.moveSpeed * 0.5f;
                if (reelSpinArgs.isHyper)
                {
                    reelSpinArgs.isHyper = false;
                    AudioManager.Playback("FE_hyper");
                    SetHyperState(true);
                }
            }
            transform.DOLocalMove(endPoint, speed * 6).SetRelative().SetEase(setting.reverseCurve).OnComplete(ReverseRolling);
            }
        }
        else
        {
            ReverseStop();
        }
        OnStepListener?.Invoke(this);
    }*/
    /// <summary>
    /// 旋转停止
    /// </summary>
    /*protected void ReverseStop()
    {
        SetHyperState(false);
        AudioManager.Stop("Hyper");
        //停止之后将Reel和Symbol重新归位
        transform.localPosition = originPoint;
        int origin = setting.symbolCount - 1;
        for (int i = origin; i >= 0; i--)
        {
            Vector3 localPos = Vector3.down * (i * setting.moveDistance) - (Vector3.forward * (i * 0.2f + 1));
            CommonSymbol symbol = SymbolList[i];
            symbol.Install(transform, localPos, setting.reelID, setting.defaultLayer);
            symbol.UpdateIndexOnReel(i);
            if (i == 3 || i < origin - (setting.symbolCount - 3))
            {
                symbol.SetDisplay(false);
            }
        }
        if (isShutDown)
        {
            AudioManager.PlayOneShot("ReelStop");
        }
        else
        {
            if (installID < 5 || installID == 7)
                AudioManager.Playback("ReelStop");
        }
        OnStopListener?.Invoke(this);
    }
    protected override void Stop()
    {
        SetHyperState(false);
        AudioManager.Stop("Hyper");
        //停止之后将Reel和Symbol重新归位
        transform.localPosition = originPoint;
        int origin = setting.symbolCount - 1;
        for (int i = origin; i >= 0; i--)
        {
            Vector3 localPos = Vector3.down * (i * setting.moveDistance) - (Vector3.forward * (i * 0.2f + 1));
            CommonSymbol symbol = SymbolList[setting.symbolCount - 1 - i];
            symbol.Install(transform, localPos, setting.reelID, setting.defaultLayer);
            symbol.UpdateIndexOnReel(i);
            if (i == origin || i < origin - (setting.symbolCount - 3))
            {
                if (installID != 7)
                {
                    symbol.SetDisplay(false);
                }
                else
                {
                    if (symbol.ItemName == SymbolNames.SCATTER)
                    {
                        (symbol as ScatterSymbol).SetLongFireDisplay(false);
                    }
                }
            }
        }
        if (isShutDown)
        {
            AudioManager.PlayOneShot("ReelStop");
        }
        else
        {
            if (installID < 5 || installID == 7)
                AudioManager.Playback("ReelStop");
        }
        OnStopListener?.Invoke(this);
    }

    public override CommonSymbol[] GetAllSymbols()
    {
        CommonSymbol[] symbols = SymbolList.ToArray();
        if (isReverse)
        {
            symbols = symbols.ToArray();
        }
        else
        {
            symbols = symbols.Reverse().ToArray();
        }

        return symbols;
    }

    protected void ReverseRefreshSymbol()
    {
        SymbolList[SymbolList.Count - 1].Recycle();
        SymbolList.RemoveAt(SymbolList.Count - 1);
        for (int i = SymbolList.Count - 1; i >= 0; i--)
        {
            SymbolList[i].UpdateIndexOnReel(SymbolList.Count - i);
        }
        CommonSymbol symbol = ReverseGetNextSymbol();
        refreshPoint += setting.moveDistance * Vector3.down;
        symbol.Install(transform, refreshPoint, installID, setting.defaultLayer);
        SymbolList.Insert(0, symbol);
        //symbol.UpdateIndexOnReel(0);
    }*/

    /*protected CommonSymbol ReverseGetNextSymbol()
    {
        int id = stripeID++;
        stripeID %= setting.reelStripes.Length;
        string symbolName;
        CommonSymbol symbol;
        //symbolName = setting.reelStripes[id];
        //过程Symbol
        if (currMoveStep < maxMoveStep - (setting.symbolCount - 2) || currMoveStep > maxMoveStep - 2)
        {
            symbolName = setting.reelStripes[id];
            if (symbolName == SymbolNames.SCATTER)
            {
                symbolName = SymbolNames.WILD;
            }
            if (symbolName == SymbolNames.COIN)
            {
                int level = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinLevelProbability[installID % 5]) + 1;
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][level]);
                if (installID % 5 == 3 && level == 3)
                {
                    index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][4]);
                }
                if (level == 3 && index > ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4 - 1)
                {
                    JackpotType jackpotType = GlobalObserver.IDJackpotDic[index - (ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4)];
                    symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                    (symbol as CoinSymbol).SetScore(jackpotType);
                }
                else
                {
                    int score = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][level][index];
                    symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                    (symbol as CoinSymbol).Level = level;
                    (symbol as CoinSymbol).SetScore(score);
                }

            }
            else
            {
                symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            }
        }
        //结果Symbol
        else
        {
            id = maxMoveStep - currMoveStep - 1;
            symbolName = reelSpinArgs.resultSymbols[id];
            symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName == SymbolNames.COIN)
            {
                CoinSymbol coinSymbol = symbol as CoinSymbol;
                coinSymbol.Level = reelSpinArgs.coinLevel;
                coinSymbol.SetScore(reelSpinArgs.coinScore);
                coinSymbol.SetScore(reelSpinArgs.coinJPType);
                //coinSymbol.SetScore(GlobalObserver.IDJackpotDic[Random.Range(0,4)]);
            }
        }
        return symbol;
    }*/


    protected override CommonSymbol GetNextSymbol()
    {
        int id = stripeID++;
        stripeID %= setting.reelStripes.Length;
        string symbolName;
        CommonSymbol symbol;
        //symbolName = setting.reelStripes[id];
        //过程Symbol
        if (currMoveStep < maxMoveStep - (setting.symbolCount - 2) || currMoveStep > maxMoveStep - 2)
        {
            symbolName = setting.reelStripes[id];
            if(reelSpinArgs != null)
            {
                if (currMoveStep == maxMoveStep - (setting.symbolCount - 1) && reelSpinArgs.isReverse)
                {
                    symbolName = SymbolNames.SCATTER;
                }
            }
            if (symbolName == SymbolNames.COIN)
            {
                int level = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinLevelProbability[installID % 5]) + 1;
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][level]);
                if (installID % 5 == 3 && level == 3)
                {
                    index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][4]);
                }
                if (level == 3 && index > ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4 - 1)
                {
                    JackpotType jackpotType = GlobalObserver.IDJackpotDic[index - (ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4)];
                    symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                    (symbol as CoinSymbol).SetScore(jackpotType);
                }
                else
                {
                    int score = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][level][index];
                    symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                    (symbol as CoinSymbol).Level = level;
                    (symbol as CoinSymbol).SetScore(score);
                }
            }
            else
            {
                symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            }
        }
        //结果Symbol
        else
        {
            id = maxMoveStep - currMoveStep - 2;
            symbolName = reelSpinArgs.resultSymbols[id];
            symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName == SymbolNames.COIN)
            {
                CoinSymbol coinSymbol = symbol as CoinSymbol;
                coinSymbol.Level = reelSpinArgs.coinLevel;
                coinSymbol.SetScore(reelSpinArgs.coinScore);
                coinSymbol.SetScore(reelSpinArgs.coinJPType);
                //coinSymbol.SetScore(GlobalObserver.IDJackpotDic[Random.Range(0,4)]);
            }
        }
        //symbolName = SymbolNames.SCATTER;

        return symbol;
    }
}
