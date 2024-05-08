using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Config;
using SlotGame.Symbol;
using UniversalModule.SpawnSystem;
using UniversalModule.AudioSystem;
using System;
using Random = UnityEngine.Random;
using UniversalModule.DelaySystem;

public class ReelBonus : ReelBase
{
    public override event System.Action<ReelBase> OnStopListener;


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
                /*if (installID == 7)
                {
                    if (symbol.ItemName == SymbolNames.SCATTER)
                    {
                        (symbol as ScatterSymbol).SetLongFireDisplay(false);
                    }
                }
                else
                {*/
                symbol.SetDisplay(false);
                /*}*/
            }
        }
        if (isShutDown)
        {
            AudioManager.PlayOneShot("BG_reelstop_bonus");
        }
        else
        {
            AudioManager.Playback("BG_reelstop_bonus");
        }
        OnStopListener?.Invoke(this);
    }

    protected override void ReverseStop()
    {
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
            AudioManager.PlayOneShot("BG_reelstop_bonus");
        }
        else
        {
            if (installID < 5 || installID == 7)
                AudioManager.Playback("BG_reelstop_bonus");
        }
        DelayCallback.Delay(this, 45 / 30f, delegate ()
        {
            SetHyperState(false);
        });
        //isReverse = false;
        //Debug.Log(OnStopListener);
        OnStopListener?.Invoke(this);
    }

    /// <summary>
    /// 获取下一步要刷新的Symbol
    /// </summary>
    /// <returns></returns>
    protected override CommonSymbol GetNextSymbol()
    {
        int id = stripeID++;
        stripeID %= setting.reelStripes.Length;
        string symbolName;
        CommonSymbol symbol;
        if (currMoveStep < maxMoveStep - (setting.symbolCount - 2) || currMoveStep > maxMoveStep - 2)
        {
            symbolName = setting.reelStripes[id];
            if (reelSpinArgs != null)
            {
                if (currMoveStep == maxMoveStep - (setting.symbolCount - 1) && reelSpinArgs.isReverse && symbolName == SymbolNames.SCATTER)
                {
                    symbolName = SymbolNames.PIC01;
                }
            }
            symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName == SymbolNames.BCOIN)
            {
                BonusCoinSymbol bSymbol = symbol as BonusCoinSymbol;
                bSymbol.Level = 3;
                bSymbol.SetScore(GlobalObserver.IDJackpotDic[Random.Range(0, 4)]);
            }
        }
        else
        {
            id = maxMoveStep - currMoveStep - 2;
            symbolName = reelSpinArgs.resultSymbols[id];
            symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName.Equals(SymbolNames.BCOIN))
            {
                BonusCoinSymbol bSymbol = symbol as BonusCoinSymbol;
                bSymbol.Level = 3;
                bSymbol.SetScore(reelSpinArgs.coinJPType);
            }
        }
        return symbol;
    }

    public override void ReplaceAllSymbol(string symbolName)
    {
        for (int i = 0; i < SymbolList.Count; i++)
        {
            CommonSymbol oldSymbol = SymbolList[i];
            if (!oldSymbol.ItemName.Equals(symbolName))
            {
                ReplaceSymbol(symbolName, i);
            }
        }
    }

}
