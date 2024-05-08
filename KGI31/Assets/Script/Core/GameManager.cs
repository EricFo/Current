using System;
using System.Linq;
using UnityEngine;
using SlotGame.Result;
using SlotGame.Core.Singleton;
using System.Collections.Generic;
using SlotGame.Core.StateProcess;

namespace SlotGame.Core {
    public class GameManager : MonoBehaviour {
        private Dictionary<string, IPopup> Popups;
        private Dictionary<GameState, IStateProcess> StateProcess;
        private Dictionary<GameState, IStateMachine> StateMachines;

        private Dictionary<GameState, Func<bool>> StateEnterManager;
        private Dictionary<GameState, Action<PayoutInfo>> StateExitManager;

        #region 初始化
        private void Start() {
            FindObjects();
            RegisterBtnEvent();
            RegisterStateProcess();
        }
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                UIController.BottomPanel.DetectBtnClick();
            }
        }
        /// <summary>
        /// 查找状态机和弹框等需要被初始化的对象
        /// </summary>
        private void FindObjects() {
            IGlobalSingleton[] singletons = FindAllOfType<IGlobalSingleton>();
            Array.ForEach(singletons, (item) => { item.Initialize(); });

            StateProcess = new Dictionary<GameState, IStateProcess>();
            IStateProcess[] stateProcesses = FindAllOfType<IStateProcess>();
            foreach (IStateProcess item in stateProcesses) {
                StateProcess.Add(item.GameState, item);
            }

            StateMachines = new Dictionary<GameState, IStateMachine>();
            IStateMachine[] machines = FindAllOfType<IStateMachine>();
            foreach (IStateMachine machine in machines) {
                if (machine != null) {
                    machine.Initialize();
                    machine.OnCompleteEvent += OnComplete;
                    if (!StateMachines.ContainsKey(machine.State)) {
                        StateMachines.Add(machine.State, machine);
                    } else {
                        Debug.LogErrorFormat("{0}状态重复，同类型状态机必须唯一", machine.State);
                    }
                }
            }

            Popups = new Dictionary<string, IPopup>();
            IPopup[] popups = FindAllOfType<IPopup>();
            foreach (IPopup item in popups) {
                if (item != null) {
                    item.Initialize();
                    if (!Popups.ContainsKey(item.WindowName)) {
                        Popups.Add(item.WindowName, item);
                    } else {
                        Debug.LogErrorFormat("名称重复，请检查{0}名称设置", item.WindowName);
                    }
                }
            }
        }
        /// <summary>
        /// 注册按钮事件，主要包括底部相关的UI按钮和作弊按钮
        /// </summary>
        private void RegisterBtnEvent() {
            UIController.BottomPanel.OnSpinBtnClickEvent += OnSpin;
            UIController.BottomPanel.OnStopBtnClickEvent += OnShutDown;

            //点击某个作弊按钮时将会立刻开始一轮游戏
            UIController.CheatPanel.OnCheatSpinEvent += OnSpin;
        }
        /// <summary>
        /// 分别注册状态开始和状态结束时需要处理的事情
        /// </summary>
        private void RegisterStateProcess() {
            StateEnterManager = new Dictionary<GameState, Func<bool>>();
            StateExitManager = new Dictionary<GameState, Action<PayoutInfo>>();
            foreach (IStateProcess stateProcess in StateProcess.Values) {
                stateProcess.GetPopupBoxEvent += GetPopupBox;
                stateProcess.GetStateProcessEvent += GetStateProcess;
                stateProcess.GetStateMachineEvent += GetStateMachine;
                StateEnterManager.Add(stateProcess.GameState, stateProcess.OnStateEnter);
                StateExitManager.Add(stateProcess.GameState, stateProcess.OnStateExit);
            }
        }
        #endregion

        #region 按钮行为事件
        /// <summary>
        /// Spin按钮响应事件，处理游戏开始前的逻辑，并通知状态机开始一轮游戏
        /// </summary>
        private void OnSpin() {
            IStateMachine machine = GetStateMachine(GlobalObserver.CurrGameState);
            if (StateEnterManager[GlobalObserver.CurrGameState]()) {
                machine.Enter();
            }
        }
        /// <summary>
        /// Stop按钮响应事件，用于通知状态机停止当前行为并立即呈现最终结果
        /// </summary>
        private void OnShutDown() {
            GetStateMachine(GlobalObserver.CurrGameState).ShutDown();
        }
        /// <summary>
        /// 当一轮游戏结束时，状态机会回调该事件，用于通知GameManager，便于后续逻辑执行
        /// </summary>
        private void OnComplete() {
            GameState state = GlobalObserver.CurrGameState;
            PayoutInfo result = null;
            if (GlobalObserver.CurrGameState.Equals(GameState.Base))
            {
            }
            StateExitManager[state]?.Invoke(result);
        }
        #endregion

        #region 辅助功能
        /// <summary>
        /// 查找所有相同类型接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T[] FindAllOfType<T>() {
            var interfaces = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<T>().ToArray();
            return interfaces;
        }
        /// <summary>
        /// 根据名称获取弹出框
        /// </summary>
        /// <param name="popUpName"></param>
        /// <returns></returns>
        private IPopup GetPopupBox(string popUpName) {
            if (Popups.ContainsKey(popUpName)) {
                return Popups[popUpName];
            } else {
                Debug.LogErrorFormat("未找到弹出框{0}", popUpName);
                return default;
            }
        }
        /// <summary>
        /// 根据游戏状态获取流程控制器
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private IStateProcess GetStateProcess(GameState state) {
            if (StateProcess.ContainsKey(state)) {
                return StateProcess[state];
            } else {
                Debug.LogErrorFormat("{0}控制器不存在", state);
                return default;
            }
        }
        /// <summary>
        /// 根据游戏状态获取状态机
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private IStateMachine GetStateMachine(GameState state) {
            if (StateMachines.ContainsKey(state)) {
                return StateMachines[state];
            } else {
                Debug.LogErrorFormat("状态机不存在{0}状态", state);
                return default;
            }
        }
        #endregion
    }
}
