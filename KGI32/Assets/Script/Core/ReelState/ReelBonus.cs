using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Config;
using SlotGame.Symbol;
using UniversalModule.SpawnSystem;
using UniversalModule.AudioSystem;

public class ReelBonus : ReelBase
{
    public override event System.Action<ReelBase> OnStopListener;

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
            symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName == SymbolNames.BCOIN)
            {
                (symbol as BonusCoinSymbol).Level = Random.Range(1, 3);
                (symbol as BonusCoinSymbol).SetScore(0);
            }
        }
        else
        {
            id = maxMoveStep - currMoveStep - 2;
            symbolName = reelSpinArgs.resultSymbols[id];
            symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName.Equals(SymbolNames.BCOIN))
            {
                (symbol as BonusCoinSymbol).Level = reelSpinArgs.coinLevel;
                (symbol as BonusCoinSymbol).SetScore(0);
            }
        }
        if (symbol.ItemName == SymbolNames.EMPTY)
        {
            (symbol as EmptySymbol).InitSymbolMainTex();
        }
        return symbol;
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
                symbol.SetDisplay(false);
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
