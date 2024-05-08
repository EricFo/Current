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
        public Button BonusGame1;
        public Button BonusGame2;
        public Button BonusGame3;
        public Button Grand;
        public Button Major;
        public Button Mini;
        public Button Free1ToFree3;

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
            BonusGame1.onClick.AddListener(OnBonusGame1BtnClick);
            BonusGame2.onClick.AddListener(OnBonusGame2BtnClick);
            BonusGame3.onClick.AddListener(OnBonusGame3BtnClick);
            Grand.onClick.AddListener(OnGrandBtnClick);
            Major.onClick.AddListener(OnMajorBtnClick);
            Mini.onClick.AddListener(OnMiniBtnClick);
            Free1ToFree3.onClick.AddListener(OnFree1ToFree3BtnClick);
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
        /// 进入BonusGame按钮点击事件
        /// </summary>
        private void OnBonusGame1BtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isTriggerBonusGame1 = true;
                GlobalObserver.IsUpgradePlay = true;
                OnSpin();
            }
        }
        private void OnBonusGame2BtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isTriggerBonusGame2 = true;
                GlobalObserver.IsCollectPlay = true;
                OnSpin();
            }
        }
        private void OnBonusGame3BtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isTriggerBonusGame3 = true;
                GlobalObserver.IsUpgradePlay = true;
                GlobalObserver.IsCollectPlay = true;
                OnSpin();
            }
        }
        private void OnGrandBtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isTriggerBonusGame1 = true;
                GlobalObserver.IsUpgradePlay = true;
                GlobalObserver.IsCollectPlay = false;
                Cheat.isGrand = true;
                OnSpin();
            }
        }
        private void OnMajorBtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isTriggerBonusGame1 = true;
                GlobalObserver.IsUpgradePlay = true;
                GlobalObserver.IsCollectPlay = false;
                Cheat.isMajor = true;
                OnSpin();
            }
        }
        private void OnMiniBtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isTriggerBonusGame1 = true;
                GlobalObserver.IsUpgradePlay = true;
                GlobalObserver.IsCollectPlay = false;
                Cheat.isMini = true;
                OnSpin();
            }
        }
        private void OnFree1ToFree3BtnClick()
        {
            if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
            {
                Cheat.isFree1ToFree3 = true;
                Cheat.isTriggerBonusGame1 = true;
                GlobalObserver.IsUpgradePlay = true;
                OnSpin();
            }
        }


        private void OnSpin() {
            UpdateCheatBtnListState();
            OnCheatSpinEvent?.Invoke();
        }
        #endregion

    }
}
