using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.SpawnSystem;

public class BonusReel : ReelBase
{
    /*/// <summary>
    /// 获取下一步要刷新的Symbol
    /// </summary>
    /// <returns></returns>
    protected override CommonSymbol GetNextSymbol()
    {
        int id = stripeID++;
        stripeID %= setting.reelStripes.Length;
        string symbolName = null;
        if (currMoveStep < maxMoveStep - (setting.symbolCount - 2) || currMoveStep > maxMoveStep - 2)
        {
            symbolName = setting.reelStripes[id];
        }
        else
        {
            id = maxMoveStep - currMoveStep - 2;
            symbolName = reelSpinArgs.resultSymbols[id];
        }
        CommonSymbol symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
        if (symbol.ItemName.Equals(SymbolNames.BCOIN))
        {
            int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.InitialCoinCreditsProbability[GlobalObserver.CurrentBonusState]);
            float score = ConfigManager.bonusConfig.InitialCoinCredits[GlobalObserver.CurrentBonusState][index] * GlobalObserver.BetValue / 80;
            (symbol as BonusCoinSymbol).SetScore((int)score);
        }
        return symbol;
    }*/

    public override void ReplaceAllSymbol(string symbolName)
    {
        for (int i = 0; i < SymbolList.Count; i++)
        {
            CommonSymbol oldSymbol = SymbolList[i];
            if (!oldSymbol.ItemName.Equals(symbolName))
            {
                ReplaceSymbol(symbolName, oldSymbol.IndexOnReel);
            }
        }
    }
}
