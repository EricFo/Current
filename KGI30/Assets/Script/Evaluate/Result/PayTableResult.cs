using SlotGame.Symbol;
using System.Collections.Generic;

namespace SlotGame.Result {
    public class PayTableResult : ResultNode {
        /// <summary>
        /// 所有能看到的金币列表
        /// </summary>
        public List<CommonSymbol> CoinList { get; private set; }
        /// <summary>
        /// 所有能看到的Wild
        /// </summary>
        public List<CommonSymbol> WildList { get; private set; }
        /// <summary>
        /// 所有能看到的Scatter 
        /// </summary>
        public List<CommonSymbol> ScatterList { get; private set; }
        /// <summary>
        /// 可见区域的Symbol
        /// </summary>
        public CommonSymbol[][] VisibleSymbols { get; private set; }

        public Dictionary<string, List<CommonSymbol>> WinSymbols { get; private set; }

        public PayTableResult(PayMode mode, int reelCount): base(mode){
            CoinList = new List<CommonSymbol>();
            WildList = new List<CommonSymbol>();
            ScatterList = new List<CommonSymbol>();
            VisibleSymbols = new CommonSymbol[reelCount][];
            WinSymbols = new Dictionary<string, List<CommonSymbol>>();
        }

        public override void Clear() {
            CoinList.Clear();
            WildList.Clear();
            ScatterList.Clear();
            WinSymbols.Clear();
        }
    }
}
