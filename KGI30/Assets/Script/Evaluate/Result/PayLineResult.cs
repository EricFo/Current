using UnityEngine;
using SlotGame.Symbol;
using System.Collections.Generic;

namespace SlotGame.Result {
    public class PayLineResult : ResultNode {
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
        /// <summary>
        /// 中奖Symbol
        /// </summary>
        public List<CommonSymbol[]> WinSymbols { get; private set; }
        /// <summary>
        /// 线赔绘制的线段
        /// </summary>
        public List<Vector3[]> DrawLines { get; private set; }

        public PayLineResult(PayMode mode, int reelCount):base(mode){
            CoinList = new List<CommonSymbol>();
            WildList = new List<CommonSymbol>();
            ScatterList = new List<CommonSymbol>();
            VisibleSymbols = new CommonSymbol[reelCount][];
            DrawLines = new List<Vector3[]>();
            WinSymbols = new List<CommonSymbol[]>();
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        public override void Clear() {
            DrawLines.Clear();
            WinSymbols.Clear();
            CoinList.Clear();
            WildList.Clear();
            ScatterList.Clear();
        }
    }
}
