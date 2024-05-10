using SlotGame.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SlotGame.UI {
    [Serializable]
    public class CheatUIPanel {
        public Button FeatureBtn;
        public Button AddCreditBtn;
        public Button AutoPlayBtn;
        public Button BonusGameBtn;
        public GameObject CheatBtnList;

        #region 事件监听
        public event Action OnAddCreditEvent;
        public event Action OnCheatSpinEvent;
        #endregion

        public void Initialize() {
            CheatBtnList.SetActive(false);
            FeatureBtn.onClick.AddListener(UpdateCheatBtnListState);

            AddCreditBtn.onClick.AddListener(OnAddCreditBtnClick);
            AutoPlayBtn.onClick.AddListener(OnAutoPlayBtnClick);
            BonusGameBtn.onClick.AddListener(OnBonusGameBtnClick);
        }
        /// <summary>
        /// 隐藏作弊按钮列表
        /// </summary>
        public void HideCheatBtnList() {
            CheatBtnList.SetActive(false);
        }
        /// <summary>
        /// 更新Feature按钮状态
        /// </summary>
        /// <param name="isActive"></param>
        public void UpdateFeatureBtnState(bool isActive) {
            FeatureBtn.interactable = isActive;
        }
        /// <summary>
        /// 更新作弊按钮列表状态
        /// </summary>
        private void UpdateCheatBtnListState() {
            CheatBtnList.SetActive(!CheatBtnList.activeSelf);
        }

        #region 行为事件
        /// <summary>
        /// 增加余额按钮点击事件
        /// </summary>
        private void OnAddCreditBtnClick() {
            OnAddCreditEvent();
            UpdateCheatBtnListState();
        }

        /// <summary>
        /// 自动游戏按钮点击事件
        /// </summary>
        private void OnAutoPlayBtnClick()
        {
            GlobalObserver.SetAutoPlay(true);
            Cheat.isAutoPlay = true;
            OnSpin();
        }

        /// <summary>
        /// 触发Bonus按钮点击事件
        /// </summary>
        private void OnBonusGameBtnClick()
        {
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnSpin() {
            UpdateCheatBtnListState();
            OnCheatSpinEvent?.Invoke();
        }
        #endregion

    }
}
