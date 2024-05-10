using UnityEngine;
using SlotGame.Symbol;
using System.Collections.Generic;

namespace SlotGame.Result {
    public class PayoutInfo {
        /// <summary>
        /// 本轮中奖金额
        /// </summary>
        public int WinMoney { get; set; }
        /// <summary>
        /// 总中奖金额，BaseGame通常不需要计算TotalWin
        /// </summary>
        public int TotalWin { get; set; }
        /// <summary>
        /// 玩法是否结束，BaseGame通常不需要关心这里
        /// </summary>
        public bool IsGameOver { get; set; }
        /// <summary>
        /// 标记属于那个游戏状态的结果
        /// </summary>
        public GameState State { get; protected set; }
        /// <summary>
        /// 记录了所有赔付信息的节点列表
        /// </summary>
        public List<ResultNode> ResultNodes { get; protected set; }

        public PayoutInfo(GameState state) {
            WinMoney = 0;
            TotalWin = 0;
            State = state;
            ResultNodes = new List<ResultNode>();
        }

        public void AddResultNode(ResultNode node) {
            ResultNodes.Add(node);
        }

        /// <summary>
        /// 重置TotalWin
        /// </summary>
        public void ResetTotalWin() {
            TotalWin = 0;
        }
        /// <summary>
        /// 清理所有数据
        /// </summary>
        public void Clear() {
            WinMoney = 0;
            IsGameOver = false;
            foreach (var item in ResultNodes) {
                item.Clear();
            }
            ResultNodes.Clear();
        }
    }
}
