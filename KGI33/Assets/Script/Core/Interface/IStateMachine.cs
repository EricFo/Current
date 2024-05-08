using SlotGame.Result;
using System;

namespace SlotGame.Core {
    public interface IStateMachine {
        GameState State { get; }
        PayMode PayMode { get; }

        event Action OnCompleteEvent;

        /// <summary>
        /// 状态机初始化函数
        /// </summary>
        void Initialize();
        /// <summary>
        /// 状态机进入，开始执行逻辑之前
        /// </summary>
        void Enter();
        /// <summary>
        /// 状态机需要被终止
        /// </summary>
        /// <returns></returns>
        bool ShutDown();
        /// <summary>
        /// 通常在OnEnter之前调用，通常用于BaseGame触发Jackpot\FreeGame\BonusGame等新玩法时调用
        /// </summary>
        void OnTrigger();
        /// <summary>
        /// 状态机在结束之前的逻辑通常可以被分割成多个步骤一步步执行
        /// </summary>
        /// <returns></returns>
        bool MoveStep();
        /// <summary>
        /// 重置状态机，通常用于重置各种标记、信息等。
        /// </summary>
        void OnReset();

        /// <summary>
        /// 将接下来用于统计奖励信息的结果汇总并返回
        /// </summary>
        /// <returns></returns>
        PayoutInfo ReportTheResult();
    }
}
