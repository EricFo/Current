using SlotGame.Result;

namespace SlotGame.Core {
    /// <summary>
    /// 检查各种主要流程
    /// </summary>
    public class CheckProcess {
        #region JackPot流程
        /// <summary>
        /// 检查是否触发JackPot逻辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public static bool CheckJackPotIsTrigger(PayoutInfo result) {
            return false;
        }
        /// <summary>
        /// 检查JackPot是否结束
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckJackPotIsGameOver(PayoutInfo result) {
            return true;
        }
        #endregion

        #region FreeGame流程
        /// <summary>
        /// 检查是否触发FreeGame
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        public static bool CheckFreeGameIsTrigger(PayoutInfo result) {
            return false;
        }
        /// <summary>
        /// 检查FreeGame是否结束
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool CheckFreeGameIsOver(PayoutInfo result) {
            return true;
        }
        #endregion

        #region BonusGame流程
        /// <summary>
        /// 检查BonusGame是否结束
        /// </summary>
        /// <param name="result">剩余次数计数</param>
        /// <returns></returns>
        public static bool CheckBonusGameIsOver(int surplus)
        {
            if (surplus <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
