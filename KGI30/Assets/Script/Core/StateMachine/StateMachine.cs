using System;
using UnityEngine;
using SlotGame.Result;

namespace SlotGame.Core {
    /// <summary>
    /// 状态机主要用于处理画面表现相关的逻辑，比如控制Reel旋转
    /// </summary>
    public abstract class StateMachine : MonoBehaviour, IStateMachine {
        /// <summary>
        /// 游戏状态
        /// </summary>
        [SerializeField] private GameState state = 0;
        /// <summary>
        /// 赔付方式
        /// </summary>
        [SerializeField] private PayMode payMode = 0;

        #region 属性
        public GameState State { get { return state; } }
        public PayMode PayMode { get { return payMode; } }
        protected PayoutInfo Payout { get; set; }
        #endregion

        #region 事件监听
        protected Action OnCompleteListener;
        public virtual event Action OnCompleteEvent {
            add {
                OnCompleteListener -= value;
                OnCompleteListener += value;
            }
            remove {
                OnCompleteListener -= value;
            }
        }
        #endregion


        public abstract void Enter();
        public abstract bool ShutDown();
        public abstract PayoutInfo ReportTheResult();


        public virtual void Initialize() {
            Payout = new PayoutInfo(State);
        }
        public virtual void OnTrigger() {
            throw new NotImplementedException(State.ToString() + "状态机未实现接口OnTrigger");
        }
        public virtual bool MoveStep() {
            throw new NotImplementedException(State.ToString() + "状态机未实现接口MoveStep");
        }
        public virtual void OnReset() {
            Payout.Clear();
        }

        protected void OnComplate() {
            OnCompleteListener?.Invoke();
        }
    }
}
