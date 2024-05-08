using SlotGame.Result;
using System;

namespace SlotGame.Core.StateProcess {
    public interface IStateProcess {
        GameState GameState { get; }

        event Func<string, IPopup> GetPopupBoxEvent;
        event Func<GameState, IStateProcess> GetStateProcessEvent;
        event Func<GameState, IStateMachine> GetStateMachineEvent;
        /// <summary>
        /// 处理进入状态机时的逻辑
        /// </summary>
        /// <returns></returns>
        bool OnStateEnter();
        /// <summary>
        /// 处理退出状态机时的逻辑
        /// </summary>
        /// <param name="result"></param>
        void OnStateExit(PayoutInfo result);
    }
}
