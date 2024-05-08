using UnityEngine;
using SlotGame.Core;
using SlotGame.Symbol;
using SlotGame.Result;
using SlotGame.Config;
using System.Collections.Generic;
using UniversalModule.Initialize;

namespace SlotGame.Evaluate {
    public class PayLineEvaluate {
        [AutoLoad]
        private static void Initialize() {
            GlobalObserver.RegisterEvaluate(PayMode.Line, Compensate);
        }
        /// <summary>
        /// 计算赔付
        /// </summary>
        private static int Compensate(ResultNode node) {
            PayLineResult result = node as PayLineResult;
            int[][] points = ConfigManager.payLineConfig.GetPayLinePoint(GlobalObserver.CurrGameState.ToString());
            List<Vector3> drawLines = new List<Vector3>();
            List<CommonSymbol> symbols = new List<CommonSymbol>();
            for (int i = 0; i < points.Length; i++) {
                symbols.Clear();
                drawLines.Clear();
                int wildCount = 0;
                int symbolCount = 1;
                symbols.Add(result.VisibleSymbols[0][points[i][0]]);
                string symbolName = result.VisibleSymbols[0][points[i][0]].ItemName;
                drawLines.Add(result.VisibleSymbols[0][points[i][0]].transform.position);
                if (symbolName.Equals(SymbolNames.WILD)) {
                    wildCount++;
                }
                for (int j = 1; j < points[i].Length; j++) {
                    CommonSymbol symbol = result.VisibleSymbols[j][points[i][j]];
                    drawLines.Add(symbol.transform.position);
                    //第一种情况，完全相等并且能连起来，直接加，但是需要额外判断开头是Wild的情况
                    if (symbol.ItemName.Equals(symbolName) && symbolCount == j && !symbolName.Equals(SymbolNames.SCATTER)) {
                        symbols.Add(symbol);
                        symbolCount++;
                        if (symbolName.Equals(SymbolNames.WILD)) {
                            wildCount++;
                        }
                    } //第二种情况，两Symbol不相等但是开头是Wild,仍然参与赔付，但是要更新开头Symbol的名字,Scatter不参与赔付
                    else if (symbolName.Equals(SymbolNames.WILD) && !symbol.ItemName.Equals(SymbolNames.SCATTER)) {
                        symbolName = symbol.ItemName;
                        if (symbolCount == j) {
                            symbolCount++;
                            symbols.Add(symbol);
                        }
                    } //第三种情况，wild直接当作开头Symbol参与赔付，但是不能参与Scatter赔付
                    else if (symbol.ItemName.Equals(SymbolNames.WILD) && !symbolName.Equals(SymbolNames.SCATTER)) {
                        if (symbolCount == j) {
                            symbolCount++;
                            symbols.Add(symbol);
                        }
                    }
                }
                int winMoney = ConfigManager.payTableConfig.GetPayMoney(symbolName, symbolCount);
                int wildMoney = ConfigManager.payTableConfig.GetPayMoney(SymbolNames.WILD, wildCount);
                if (winMoney > 0 || wildMoney > 0) {
                    int multi = GlobalObserver.GetMultiplyer();
                    result.WinMoney += wildMoney > winMoney ? wildMoney * multi : winMoney * multi;
                    if (wildMoney > winMoney) {     //wild赔的钱多按照Wild进行赔付，因此需要剔除其他symbol
                        int remove = 0;
                        for (int j = 0; j < symbols.Count; j++) {
                            if (!symbols[j].ItemName.Equals(SymbolNames.WILD)) {
                                remove = j;
                                break;
                            }
                        }
                        int removeCount = symbols.Count - remove;
                        symbols.RemoveRange(remove, removeCount);
                    }
                    result.DrawLines.Add(drawLines.ToArray());
                    result.WinSymbols.Add(symbols.ToArray());
                }
            }
            //Scatter直接根据出现数量进行赔付，要在最后再加上
            int scatterMoney = ConfigManager.payTableConfig.GetPayMoney(SymbolNames.SCATTER, result.ScatterList.Count);
            result.WinMoney += scatterMoney * GlobalObserver.BetValue;
            return result.WinMoney;
        }
    }
}
