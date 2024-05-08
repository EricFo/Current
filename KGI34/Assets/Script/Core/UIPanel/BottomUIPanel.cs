using SlotGame.Core;
using System;
using UnityEngine;
using UnityEngine.UI;
using UniversalModule.AudioSystem;

namespace SlotGame.UI {
    [Serializable]
    public class BottomUIPanel {
        public Text BetTxt;
        public Text CreditTxt;
        public Text SpeedUpTxt;

        public Button StopBtn;
        public Button SpinBtn;
        public Button AddBetBtn;
        public Button SubBetBtn;
        public Button SpeedUpBtn;

        public ScrollUp WinTxt;

        #region 事件监听
        private Action OnSpinBtnClickListener;
        private Action OnStopBtnClickListener;
        private Func<int> OnAddBetBtnClickListener;
        private Func<int> OnSubBetBtnClickListener;

        public Action OnAwardComplateListener;

        public event Action OnSpinBtnClickEvent {
            add {
                OnSpinBtnClickListener -= value;
                OnSpinBtnClickListener += value;
            }
            remove {
                OnSpinBtnClickListener -= value;
            }
        }
        public event Action OnStopBtnClickEvent {
            add {
                OnStopBtnClickListener -= value;
                OnStopBtnClickListener += value;
            }
            remove {
                OnStopBtnClickListener -= value;
            }
        }
        public event Func<int> OnAddBetBtnClickEvent {
            add {
                OnAddBetBtnClickListener -= value;
                OnAddBetBtnClickListener += value;
            }
            remove {
                OnAddBetBtnClickListener -= value;
            }
        }
        public event Func<int> OnSubBetBtnClickEvent {
            add {
                OnSubBetBtnClickListener -= value;
                OnSubBetBtnClickListener += value;
            }
            remove {
                OnSubBetBtnClickListener -= value;
            }
        }

        public event Action OnAwardComplateEvent {
            add {
                OnAwardComplateListener -= value;
                OnAwardComplateListener += value;
            }
            remove {
                OnAwardComplateListener -= value;
            }
        }
        #endregion

        public void Initialize(int credit, int betValue) {
            BetTxt.text = betValue.ToString();
            CreditTxt.text = credit.ToString();

            WinTxt.Initialize();
            WinTxt.OnComplateEvent += OnAwardComplate;

            SpinBtn.onClick.AddListener(OnSpinBtnClick);
            StopBtn.onClick.AddListener(OnStopAutoPlay); //Stop按钮的两个事件顺序不要调换，确保停止AutoSpin事件先执行
            StopBtn.onClick.AddListener(OnStopBtnClick);
            AddBetBtn.onClick.AddListener(OnAddBetBtnClick);
            SubBetBtn.onClick.AddListener(OnSubBetBtnClick);
            SpeedUpBtn.onClick.AddListener(OnSpeedUpBtnClick);
        }

        #region 行为事件
        /// <summary>
        /// Spin按钮点击事件
        /// </summary>
        public void OnSpinBtnClick() {
            OnSpinBtnClickListener?.Invoke();
            AudioManager.PlayOneShot("SpinBtn");
        }
        /// <summary>
        /// Stop按钮点击事件
        /// </summary>
        public void OnStopBtnClick() {
            Cheat.isAutoPlay = false;
            StopBtn.interactable = false;
            if (WinTxt.isRunning == true) {
                WinTxt.Stop();
            } else {
                OnStopBtnClickListener?.Invoke();
            }
        }
        /// <summary>
        /// 空格按钮按下后，根据按钮状态触发点击事件
        /// </summary>
        public void DetectBtnClick() {
            if (SpinBtn.isActiveAndEnabled == true && SpinBtn.interactable == true) {
                OnSpinBtnClick();
                return;
            } 
            if (StopBtn.isActiveAndEnabled == true && StopBtn.interactable == true) {
                OnStopBtnClick();
                return;
            }
        }
        /// <summary>
        /// 加倍按钮点击事件
        /// </summary>
        private void OnAddBetBtnClick() {
            int value = OnAddBetBtnClickListener();
            BetTxt.text = value.ToString();
        }
        /// <summary>
        /// 减倍按钮点击事件
        /// </summary>
        private void OnSubBetBtnClick() {
            int value = OnSubBetBtnClickListener();
            BetTxt.text = value.ToString();
        }
        /// <summary>
        /// Win窗口文字滚动完成事件
        /// </summary>
        private void OnAwardComplate() {
            OnAwardComplateListener?.Invoke();
        }
        /// <summary>
        /// 当点击Stop按钮的时候，无论在什么情况下，都需要停止AutoPlay
        /// </summary>
        private void OnStopAutoPlay() { 
            GlobalObserver.SetAutoPlay(false);
        }
        #endregion

        #region 辅助工具
        /// <summary>
        /// 中奖后Win窗口滚动奖金
        /// </summary>
        /// <param name="value"></param>
        public void AwardRollUp(int value, bool isPlay = true) {
            if (isPlay) {
                WinTxt.StartUp(value);
                //StopBtn.interactable = true;
            } else {
                WinTxt.SetTotalValue(value);
            }
        }
        /// <summary>
        /// 直接设置要显示的奖励值,累加
        /// </summary>
        /// <param name="value"></param>
        public void SetAward(int value) {
            WinTxt.SetAwardValue(value);
        }
        /// <summary>
        /// 更新余额
        /// </summary>
        public void UpdateCredit(int credit) {
            CreditTxt.text = credit.ToString();
        }
        /// <summary>
        /// 禁用全部按钮
        /// </summary>
        public void DisableAllButtons() {
            StopBtn.interactable = false;
            SpinBtn.interactable = false;
            AddBetBtn.interactable = false;
            SubBetBtn.interactable = false;
        }
        /// <summary>
        /// 所有按钮恢复默认状态
        /// </summary>
        public void RestoreButtonDefultState() {
            StopBtn.interactable = false;
            StopBtn.gameObject.SetActive(false);
            SpinBtn.interactable = true;
            SpinBtn.gameObject.SetActive(true);
            AddBetBtn.interactable = true;
            SubBetBtn.interactable = true;
        }
        /// <summary>
        /// 更新按Spin时的按钮状态
        /// </summary>
        public void UpdatePressSpinBtnState() {
            SpinBtn.interactable = false;
            SpinBtn.gameObject.SetActive(false);
            StopBtn.interactable = true;
            StopBtn.gameObject.SetActive(true);
            AddBetBtn.interactable = false;
            SubBetBtn.interactable = false;
        }
        /// <summary>
        /// 显示Spin按钮
        /// </summary>
        public void DisplaySpinBtn() {
            StopBtn.interactable = false;
            StopBtn.gameObject.SetActive(false);
            SpinBtn.interactable = true;
            SpinBtn.gameObject.SetActive(true);
        }
        /// <summary>
        /// 重置Win面板
        /// </summary>
        public void ResetWinPanel() {
            WinTxt.OnReset();
        }

        float m_Time = 1;
        private void OnSpeedUpBtnClick()
        {
            m_Time++;
            if (m_Time > 5)
            {
                m_Time = 1;
            }
            Time.timeScale = m_Time;
            SpeedUpTxt.text = m_Time.ToString();
        }
        #endregion
    }
}
