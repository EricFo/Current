using SlotGame.Symbol;
using SlotGame.Core.Reel;

namespace SlotGame.Result
{
    public class StatisticalResult
    {
        /// <summary>
        /// 根据赔付类型更新结果，每种赔付有不同方式，默认提供PayLine和PayTable
        /// </summary>
        /// <param name="payMode"></param>
        /// <param name="payout"></param>
        /// <param name="reels"></param>
        public static void UpdateResult(PayMode payMode, PayoutInfo payout, IReelState[] reels)
        {
            payout.WinMoney = 0;
            switch (payMode)
            {
                case PayMode.Line:
                    UpdatePayLineResult(payMode, payout, reels);
                    break;
                case PayMode.Table:
                    UpdatePayTableResult(payMode, payout, reels);
                    break;
                default:
                    throw new System.Exception(string.Format("未找到{0}赔付模式下统计结果的实现方式", payMode.ToString()));
            }
        }
        /// <summary>
        /// 生成线赔结果
        /// </summary>
        private static void UpdatePayLineResult(PayMode payMode, PayoutInfo payout, IReelState[] reels)
        {
            PayLineResult result = new PayLineResult(payMode, reels.Length);
            for (int i = 0; i < reels.Length; i++)
            {
                result.VisibleSymbols[i] = reels[i].GetVisibleSymbols();
            }
            //提前统计好特殊Symbol，便于后续操作
            for (int i = 0; i < result.VisibleSymbols.Length; i++)
            {
                for (int j = 0; j < result.VisibleSymbols[i].Length; j++)
                {
                    switch (result.VisibleSymbols[i][j].ItemName)
                    {
                        case SymbolNames.WILD:
                            result.WildList.Add(result.VisibleSymbols[i][j]);
                            break;
                        case SymbolNames.SCATTER:
                            result.ScatterList.Add(result.VisibleSymbols[i][j]);
                            break;
                        case SymbolNames.COIN:
                            result.CoinList.Add(result.VisibleSymbols[i][j]);
                            break;
                    }
                }
            }
            payout.AddResultNode(result);
        }
        /// <summary>
        /// 生成全赔结果
        /// </summary>
        private static void UpdatePayTableResult(PayMode payMode, PayoutInfo payout, IReelState[] reels)
        {
            PayTableResult result;
            if (reels.Length > 5)
            {
                result = new PayTableResult(payMode, reels.Length / 3);
                for (int i = 0; i < reels.Length; i++)
                {
                    int col = i % 5;
                    int row = i / 5;
                    if (result.VisibleSymbols[col] == null)
                    {
                        result.VisibleSymbols[col] = new CommonSymbol[reels.Length / 5];
                    }
                    result.VisibleSymbols[col][row] = reels[i].GetVisibleSymbols()[0];
                }
            }
            else
            {
                result = new PayTableResult(payMode, reels.Length);
                for (int i = 0; i < reels.Length; i++)
                {
                    result.VisibleSymbols[i] = reels[i].GetVisibleSymbols();
                }
            }

            //提前统计好特殊Symbol，便于后续操作
            for (int i = 0; i < result.VisibleSymbols.Length; i++)
            {
                for (int j = 0; j < result.VisibleSymbols[i].Length; j++)
                {
                    switch (result.VisibleSymbols[i][j].ItemName)
                    {
                        case SymbolNames.WILD:
                            result.WildList.Add(result.VisibleSymbols[i][j]);
                            break;
                        case SymbolNames.SCATTER:
                            result.ScatterList.Add(result.VisibleSymbols[i][j]);
                            break;
                        case SymbolNames.COIN:
                            result.CoinList.Add(result.VisibleSymbols[i][j]);
                            break;
                    }
                }
            }
            payout.AddResultNode(result);
        }
    }
}
