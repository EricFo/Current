using UnityEngine;
using System.Linq;
using UniversalModule.Initialize;

namespace SlotGame.Config {
    public static class ConfigManager {
        public static BetConfig betConfig { get; private set; }
        public static CheatConfig cheatConfig { get; private set; }
        public static MeterConfig meterConfig { get; private set; }
        public static PayLineConfig payLineConfig { get; private set; }
        public static PayTableConfig payTableConfig { get; private set; }
        public static ReelStripeConfig reelStripConfig { get; private set; }
        public static FeatureGameConfig featureGameConfig { get; private set; }
        public static BaseConfig baseConfig { get; private set; }
        public static BonusConfig bonusConfig { get; private set; }


        [AutoLoad(0)]
        private static void Initialize() {
            UpdateConfig();
            LoadAllConfig();
        }

        /// <summary>
        /// 更新配置文件
        /// </summary>
        private static void UpdateConfig() {
            //JsonHelper.SaveJson(new BetConfig());
            //JsonHelper.SaveJson(new CheatConfig());
            //JsonHelper.SaveJson(new MeterConfig());
            //JsonHelper.SaveJson(new PayLineConfig());
            //JsonHelper.SaveJson(new JackPotConfig());
            //JsonHelper.SaveJson(new FreeGameConfig());
            //JsonHelper.SaveJson(new PayTableConfig());
            //JsonHelper.SaveJson(new ReelStripeConfig());
            //JsonHelper.SaveJson(new FeatureGameConfig());
        }

        /// <summary>
        /// 加载所有配置文件
        /// </summary>
        private static void LoadAllConfig() {
            betConfig = JsonHelper.ReadJson<BetConfig>();
            cheatConfig = JsonHelper.ReadJson<CheatConfig>();
            meterConfig = JsonHelper.ReadJson<MeterConfig>();
            payLineConfig = JsonHelper.ReadJson<PayLineConfig>();
            payTableConfig = JsonHelper.ReadJson<PayTableConfig>();
            reelStripConfig = JsonHelper.ReadJson<ReelStripeConfig>();
            featureGameConfig = JsonHelper.ReadJson<FeatureGameConfig>();
            baseConfig = JsonHelper.ReadJson<BaseConfig>();
            bonusConfig = JsonHelper.ReadJson<BonusConfig>();
            reelStripConfig.Reverse();
        }

        public static int GetIndexByProbability(int[] probability) {
            int value = Random.Range(probability.First(), probability.Last());
            for (int i = 1; i < probability.Length; i++) {
                if (value >= probability[i - 1] && value < probability[i]) {
                    return i - 1;
                }
            }
            return default;
        }
    }
}
