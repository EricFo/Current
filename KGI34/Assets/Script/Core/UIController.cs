using System;
using SlotGame.UI;
using UnityEngine;
using SlotGame.Core.Singleton;

namespace SlotGame.Core {
    public class UIController : GlobalSingleton<UIController> {
        [Serializable]
        public class UIPanel {
            public CheatUIPanel CheatPanel;     //作弊面板
            public MeterUIPanel MeterPanel;     //奖池面板
            public BottomUIPanel BottomPanel;   //底部UI条
            public StatisticsPanel StatisticsPanel;   //统计面板
        }

        [SerializeField] private UIPanel UIGroup = null;

        public static CheatUIPanel CheatPanel { 
            get {
                Instance.CheckIsInitialized();
                return Instance.UIGroup.CheatPanel;
            }
        }
        public static MeterUIPanel MeterPanel {
            get {
                Instance.CheckIsInitialized();
                return Instance.UIGroup.MeterPanel;
            }
        }
        public static BottomUIPanel BottomPanel {
            get {
                Instance.CheckIsInitialized();
                return Instance.UIGroup.BottomPanel;
            }
        }
        public static StatisticsPanel StatisticsPanel {
            get {
                Instance.CheckIsInitialized();
                return Instance.UIGroup.StatisticsPanel;
            }
        }

        /// <summary>
        /// UIController初始化，这里会将所有UI面板初始化
        /// </summary>
        public override void Initialize() {
            base.Initialize();
            if (IsInitialized == true) {
                UIGroup.CheatPanel.Initialize();
                UIGroup.StatisticsPanel.Initialize();
                UIGroup.MeterPanel.UpdateMeters(GlobalObserver.Meters);
                UIGroup.BottomPanel.Initialize(GlobalObserver.CurrCredit, GlobalObserver.BetValue);

                RegisterBtnDefaultEvent();
            }
        }

        /// <summary>
        /// 注册按钮默认事件，这些是按钮必须要具备的功能，其余功能可在外部任意添加
        /// </summary>
        private void RegisterBtnDefaultEvent() {
            //加减Bet需要更新Bet等级和奖池数值
            UIGroup.BottomPanel.OnAddBetBtnClickEvent += () => {
                int level = GlobalObserver.UpdateBetLevel2(1);
                UIGroup.MeterPanel.UpdateMeters(GlobalObserver.Meters);
                return level;
            };
            UIGroup.BottomPanel.OnSubBetBtnClickEvent += () => {
                int level = GlobalObserver.UpdateBetLevel2(-1);
                UIGroup.MeterPanel.UpdateMeters(GlobalObserver.Meters);
                return level;
            };

            //这个是默认的加钱作弊，防止余额不足无法进行游戏
            UIController.CheatPanel.OnAddCreditEvent += () => {
                GlobalObserver.UpdateCredit(10000);
                UIGroup.BottomPanel.RestoreButtonDefultState();
                UIGroup.BottomPanel.UpdateCredit(GlobalObserver.CurrCredit);
            };
        }
    }
}
