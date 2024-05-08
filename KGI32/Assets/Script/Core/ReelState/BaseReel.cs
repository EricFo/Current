using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UniversalModule.SpawnSystem;

public class BaseReel : ReelBase
{

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
