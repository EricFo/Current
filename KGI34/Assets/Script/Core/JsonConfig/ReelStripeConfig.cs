using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace SlotGame.Config {
    public class ReelStripeConfig {
        /// <summary>
        /// Coin会出现的位置(下标对应ReelID，0代表不会出现，1代表会出现)
        /// </summary>
        public int[] CoinPositions = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        /// <summary>
        /// Scatter会出现的位置(下标对应ReelID，0代表不会出现，1代表会出现)
        /// </summary>
        public int[] ScatterPositions = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        /// <summary>
        /// 转轮数据
        /// </summary>
        public Dictionary<string, string[][]> Stripes = new Dictionary<string, string[][]>()
        {
            {
                "Base",
                new string[][]{
                    new string[]{ "9","PIC1" },
                    new string[]{ "10","PIC2" },
                    new string[]{ "J","PIC3" },
                    new string[]{ "Q","PIC4" },
                    new string[]{ "K","PIC5" },
                }
            }
        };
        /// <summary>
        /// 获取Stripe
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[][] GetStripe(string name) {
            if (Stripes.ContainsKey(name)) {
                return Stripes[name];
            }
            else {
                Debug.LogErrorFormat("未找到名为{0}的Stripe,请检查{1}文件设置", name, typeof(ReelStripeConfig).Name);
                return default;
            }
        }
        /// <summary>
        /// 反转转轮
        /// </summary>
        public void Reverse() {
            foreach (string[][] item in Stripes.Values) {
                for (int i = 0; i < item.Length; i++) {
                    item[i] = item[i].Reverse().ToArray();
                }
            }
        }
    }
}
