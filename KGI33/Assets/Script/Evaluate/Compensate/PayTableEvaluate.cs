using SlotGame.Core;
using SlotGame.Result;
using SlotGame.Symbol;
using SlotGame.Config;
using System.Collections.Generic;
using UniversalModule.Initialize;
using static UnityEngine.Networking.UnityWebRequest;

namespace SlotGame.Evaluate {
    public class PayTableEvaluate {
        /// <summary>
        /// 缓存中奖结果
        /// </summary>
        private static List<WinResult> cacheResult;
        /// <summary>
        /// 缓存没中奖的结果
        /// </summary>
        private static List<WinResult> cacheNotWin;
        /// <summary>
        /// 用于统计中奖结果
        /// </summary>
        private struct WinResult {
            public string symbolName;             //symbol名字
            public int[,] eachColumnCounter;   //每一列中奖Symbol数量

            public WinResult(string name) {
                this.symbolName = name;
                eachColumnCounter = new int[5, 1];
                eachColumnCounter[0, 0] = 1;
            }
        }

        [AutoLoad]                                                                  
        private static void Initialize() {
            cacheResult = new List<WinResult>();
            cacheNotWin = new List<WinResult>();
            GlobalObserver.RegisterEvaluate(PayMode.Table, Compensate);
        }
        private static int Compensate(ResultNode node) {
            cacheResult.Clear();
            cacheNotWin.Clear();
            PayTableResult result = node as PayTableResult;
            //记录中奖条件
            for (int i = 0; i < result.VisibleSymbols[0].Length; i++) {
                var commonSymbol = result.VisibleSymbols[0][i];
                string symbolName = result.VisibleSymbols[0][i].ItemName;
                if (!symbolName.Equals(SymbolNames.SCATTER) && !symbolName.Equals(SymbolNames.WILD) && !symbolName.Equals(SymbolNames.COIN)) {
                    if (result.WinSymbols.ContainsKey(symbolName)) {
                        if (!result.WinSymbols[symbolName].Contains(commonSymbol)) {
                            result.WinSymbols[symbolName].Add(commonSymbol);
                        }
                    }
                    else {
                        result.WinSymbols.Add(symbolName, new List<CommonSymbol>());
                        result.WinSymbols[symbolName].Add(commonSymbol);
                    }
                    cacheResult.Add(new WinResult(symbolName));
                }
            }
            int sum = 0, money = 0;
            for (int i = 0; i < cacheResult.Count; i++) {
                //记录每列Symbol出现的次数
                for (int j = 1; j < result.VisibleSymbols.Length; j++) {
                    for (int k = 0; k < result.VisibleSymbols[j].Length; k++) {
                        if (result.VisibleSymbols[j][k].ItemName == cacheResult[i].symbolName || result.VisibleSymbols[j][k].ItemName == SymbolNames.WILD) {
                            cacheResult[i].eachColumnCounter[j, 0]++;
                            //判断前一列是否中奖了，中奖了就缓存这个Symbol
                            if (cacheResult[i].eachColumnCounter[j - 1, 0] > 0) {
                                var commonSymbol = result.VisibleSymbols[j][k];
                                if (!result.WinSymbols[cacheResult[i].symbolName].Contains(commonSymbol)) {
                                    result.WinSymbols[cacheResult[i].symbolName].Add(commonSymbol);
                                }
                            }
                        }
                    }
                }

                int counter = 0,multiple = 1;
                //如果连续出现次数不够三次，直接结束
                for (int j = 0; j < cacheResult[i].eachColumnCounter.Length; j++) {
                    if (cacheResult[i].eachColumnCounter[j, 0] > 0) {
                        multiple *= cacheResult[i].eachColumnCounter[j, 0];
                        counter++;
                        if (j == cacheResult[i].eachColumnCounter.Length - 1) {
                            money = ConfigManager.payTableConfig.PayTables[cacheResult[i].symbolName][counter];
                            sum += (money * multiple);
                        }
                    }
                    else {
                        money = ConfigManager.payTableConfig.PayTables[cacheResult[i].symbolName][counter];
                        if (money <= 0) {
                            cacheNotWin.Add(cacheResult[i]);
                        }
                        else {
                            sum += (money * multiple);
                        }
                        break;
                    }
                }
            }
            //清理不符合条件的结果值
            for (int i = 0; i < cacheNotWin.Count; i++) {
                result.WinSymbols.Remove(cacheNotWin[i].symbolName);
            }
            cacheNotWin.Clear();
            sum += ConfigManager.payTableConfig.PayTables[SymbolNames.SCATTER][result.ScatterList.Count];
            sum *= GlobalObserver.GetMultiplyer();
            result.WinMoney = sum;
            return sum;      
        }

        public static int Compensate(string[][] symbols,int scatterCount = 0) {
            List<WinResult> cacheResult = new List<WinResult>();
            List<WinResult> cacheNotWin = new List<WinResult>();
            Dictionary<string, List<string>> WinSymbols = new Dictionary<string, List<string>>();
            for (int i = 0; i < symbols[0].Length; i++)
            {
                string symbolName = symbols[0][i];
                if (!symbolName.Equals(SymbolNames.SCATTER) && !symbolName.Equals(SymbolNames.WILD) && !symbolName.Equals(SymbolNames.COIN))
                {
                    if (WinSymbols.ContainsKey(symbolName))
                    {
                        if (!WinSymbols[symbolName].Contains(symbolName))
                        {
                            WinSymbols[symbolName].Add(symbolName);
                        }
                    }
                    else
                    {
                        WinSymbols.Add(symbolName, new List<string>());
                        WinSymbols[symbolName].Add(symbolName);
                    }
                    cacheResult.Add(new WinResult(symbolName));
                }
            }
            int sum = 0, money = 0;
            for (int i = 0; i < cacheResult.Count; i++)
            {
                //记录每列Symbol出现的次数
                for (int j = 1; j < symbols.Length; j++)
                {
                    for (int k = 0; k < symbols[j].Length; k++)
                    {
                        if (symbols[j][k] == cacheResult[i].symbolName || symbols[j][k] == SymbolNames.WILD)
                        {
                            cacheResult[i].eachColumnCounter[j, 0]++;
                            //判断前一列是否中奖了，中奖了就缓存这个Symbol
                            if (cacheResult[i].eachColumnCounter[j - 1, 0] > 0)
                            {
                                var commonSymbol = symbols[j][k];
                                if (!WinSymbols[cacheResult[i].symbolName].Contains(commonSymbol))
                                {
                                    WinSymbols[cacheResult[i].symbolName].Add(commonSymbol);
                                }
                            }
                        }
                    }
                }

                int counter = 0, multiple = 1;
                //如果连续出现次数不够三次，直接结束
                for (int j = 0; j < cacheResult[i].eachColumnCounter.Length; j++)
                {
                    if (cacheResult[i].eachColumnCounter[j, 0] > 0)
                    {
                        multiple *= cacheResult[i].eachColumnCounter[j, 0];
                        counter++;
                        if (j == cacheResult[i].eachColumnCounter.Length - 1)
                        {
                            money = ConfigManager.payTableConfig.PayTables[cacheResult[i].symbolName][counter];
                            sum += (money * multiple);
                        }
                    }
                    else
                    {
                        money = ConfigManager.payTableConfig.PayTables[cacheResult[i].symbolName][counter];
                        if (money <= 0)
                        {
                            cacheNotWin.Add(cacheResult[i]);
                        }
                        else
                        {
                            sum += (money * multiple);
                        }
                        break;
                    }
                }
            }
            //清理不符合条件的结果值
            for (int i = 0; i < cacheNotWin.Count; i++)
            {
                WinSymbols.Remove(cacheNotWin[i].symbolName);
            }

            sum += ConfigManager.payTableConfig.PayTables[SymbolNames.SCATTER][scatterCount];
            sum *= GlobalObserver.GetMultiplyer();
            return sum;
        }
    }
}
