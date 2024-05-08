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
        public Button UpgradeCattleBtn;
        public Button ResetCattleBtn;
        public Button BigPayoutBtn;
        public Button GrandBtn;
        public Button MajorBtn;
        public Button MinorBtn;
        public Button MiniBtn;
        public Button GrandBonusBtn;
        public Button MajorBonusBtn;
        public Button MinorBonusBtn;
        public Button MiniBonusBtn;
        public Button MoreBonusCoinBtn;
        public Button AutoFreeBtn;
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
            UpgradeCattleBtn.onClick.AddListener(OnUpgradeCattleBtnClick);
            ResetCattleBtn.onClick.AddListener(OnResetCattleBtnClick);
            BigPayoutBtn.onClick.AddListener(OnBigPayoutBtnClick);
            GrandBtn.onClick.AddListener(OnGrandBtnClick);
            MajorBtn.onClick.AddListener(OnMajorBtnClick);
            MinorBtn.onClick.AddListener(OnMinorBtnClick);
            MiniBtn.onClick.AddListener(OnMiniBtnClick);
            GrandBonusBtn.onClick.AddListener(OnGrandBonusBtnClick);
            MajorBonusBtn.onClick.AddListener(OnMajorBonusBtnClick);
            MinorBonusBtn.onClick.AddListener(OnMinorBonusBtnClick);
            MiniBonusBtn.onClick.AddListener(OnMiniBonusBtnClick);
            MoreBonusCoinBtn.onClick.AddListener(OnMoreBonusCoinBtnClick);
            AutoFreeBtn.onClick.AddListener(OnAutoFreeBtnClick);
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

        private void OnAutoPlayBtnClick()
        {
            GlobalObserver.SetAutoPlay(true);
            Cheat.isAutoPlay = true;
            OnSpin();
        }

        private void OnBonusGameBtnClick()
        {
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnUpgradeCattleBtnClick()
        {
            Cheat.isUpgradeCattle = true;
            OnSpin();
        }

        private void OnResetCattleBtnClick()
        {
            GlobalObserver.ResetCattle();
            UpdateCheatBtnListState();
        }

        private void OnBigPayoutBtnClick()
        {
            Cheat.isBigPayout = true;
            OnSpin();
        }

        private void OnGrandBtnClick()
        {
            Cheat.isGrand = true;
            OnSpin();
        }

        private void OnMajorBtnClick()
        {
            Cheat.isMajor = true;
            OnSpin();
        }

        private void OnMinorBtnClick()
        {
            Cheat.isMinor= true;
            OnSpin();
        }

        private void OnMiniBtnClick()
        {
            Cheat.isMini = true;
            OnSpin();
        }

        private void OnGrandBonusBtnClick()
        {
            Cheat.isGrandBonus = true;
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnMajorBonusBtnClick()
        {
            Cheat.isMajorBonus = true;
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnMinorBonusBtnClick()
        {
            Cheat.isMinorBonus = true;
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnMiniBonusBtnClick()
        {
            Cheat.isMiniBonus = true;
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnMoreBonusCoinBtnClick()
        {
            Cheat.isMoreBonusCoin = true;
            Cheat.isTriggerBonusGame = true;
            OnSpin();
        }

        private void OnAutoFreeBtnClick()
        {
            Cheat.isAutoFree = !Cheat.isAutoFree;
            AutoFreeBtn.GetComponentInChildren<TextPro>().text = "AutoFree:" + Cheat.isAutoFree;
        }

        private void OnSpin() {
            UpdateCheatBtnListState();
            OnCheatSpinEvent?.Invoke();
        }
        #endregion

    }
}
