using System;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Result;
using SlotGame.Core.StateProcess;

/// <summary>
/// 主要用于控制状态机进入之前和退出之后的相关逻辑，加钱、更新按钮状态等逻辑都在这里处理
/// </summary>
public abstract class StateProcessBase : MonoBehaviour, IStateProcess {
    public abstract GameState GameState { get; }

    public event Func<string, IPopup> GetPopupBoxEvent;
    public event Func<GameState, IStateProcess> GetStateProcessEvent;
    public event Func<GameState, IStateMachine> GetStateMachineEvent;

    public abstract bool OnStateEnter();
    public abstract void OnStateExit(PayoutInfo result);

    /// <summary>
    /// 根据弹出框名字获取弹框
    /// </summary>
    /// <param name="popUpName"></param>
    /// <returns></returns>
    protected IPopup GetPopupBox(string popUpName) {
        return GetPopupBoxEvent?.Invoke(popUpName);
    }
    /// <summary>
    /// 根据弹出框名字获取弹框
    /// </summary>
    /// <param name="popUpName"></param>
    /// <returns></returns>
    protected T GetPopupBox<T>(string popUpName) where T : Popup {
        IPopup popup =  GetPopupBoxEvent?.Invoke(popUpName);
        if (popup is T) {
            return popup as T;
        } else {
            throw new Exception(string.Format("无法将弹框转换为{0}类型", typeof(T).ToString()));
        }
    }
    /// <summary>
    /// 根据游戏状态获取流程控制器接口
    /// </summary>
    /// <param name="state">目标状态处理流程所属游戏状态</param>
    /// <returns>目标状态处理流程接口</returns>
    protected IStateProcess GetStateProcess(GameState state) {
        return GetStateProcessEvent?.Invoke(state);
    }
    /// <summary>
    /// 根据游戏状态获取流程控制器接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="state"></param>
    /// <returns></returns>
    protected T GetStateProcess<T>(GameState state) where T : StateProcessBase {
        IStateProcess process = GetStateProcessEvent?.Invoke(state);
        if (process is T) {
            return process as T;
        } else {
            throw new Exception(string.Format("无法将控制器转换为{0}类型", typeof(T).ToString()));
        }
    }
    /// <summary>
    /// 根据游戏状态获取状态机
    /// </summary>
    /// <param name="state">目标状态机的游戏状态</param>
    /// <returns>目标状态机接口</returns>
    protected IStateMachine GetStateMachine(GameState state) {
        return GetStateMachineEvent?.Invoke(state);
    }
    /// <summary>
    /// 根据游戏状态获取状态机
    /// </summary>
    /// <param name="state">目标状态机的游戏状态</param>
    /// <returns>目标状态机接口</returns>
    protected T GetStateMachine<T>(GameState state) where T : StateMachine {
        IStateMachine machine = GetStateMachineEvent?.Invoke(state);
        if (machine is T) {
            return machine as T;
        } else {
            throw new Exception(string.Format("无法将状态机转换为{0}类型", typeof(T).ToString()));
        }
    }
}
