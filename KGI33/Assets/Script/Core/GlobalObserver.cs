using System;
using UnityEngine;
using SlotGame.Result;
using SlotGame.Config;
using System.Collections.Generic;
using UniversalModule.Initialize;
using Random = UnityEngine.Random;

namespace SlotGame.Core
{
    public static class GlobalObserver
    {
        #region 属性字段
        /// <summary>
        /// Bet等级
        /// </summary>
        public static int BetLevel { get; private set; }
        /// <summary>
        /// Bet数值
        /// </summary>
        public static int BetValue { get; private set; }
        /// <summary>
        /// TotalWin计数
        /// </summary>
        public static int TotalWin { get; private set; }
        /// <summary>
        /// 当前余额
        /// </summary>
        public static int CurrCredit { get; private set; }
        /// <summary>
        /// 是否触发JackPot
        /// </summary>
        public static bool IsTriggerJackpot { get; set; }
        /// <summary>
        /// 是否触发FreeGame
        /// </summary>
        public static bool IsTriggerFreeGame { get; set; }
        /// <summary>
        /// 是否触发BonusGame
        /// </summary>
        public static bool IsTriggerBonusGame { get; set; }
        public static bool IsGrandBonus { get; set; }
        public static bool IsMajorBonus { get; set; }
        /// <summary>
        /// BonusGame是否结束
        /// </summary>
        public static bool IsBonusGameIsOver { get; set; }
        /// <summary>
        /// Meter计数，对应MAXI MEGA MAJOR MINI
        /// </summary>
        public static double[] Meters { get; private set; }
        /// <summary>
        /// AutoPlay是否启用
        /// </summary>
        public static bool AutoSpinEnabled { get; private set; }
        /// <summary>
        /// 一轮游戏是否已经开始,他和IsFinalized属性总是相反
        /// </summary>
        public static bool IsSpinStarted { get; private set; }
        /// <summary>
        /// 游戏是否已经结束完整一轮(一次Spin触发的所有Feature都已经完成，没触发的情况下在BaseGame算完钱就算一轮)
        /// </summary>
        public static bool IsFinalized { get; private set; }

        /// <summary>
        /// 上一次的游戏状态
        /// </summary>
        public static GameState LastGameState { get; private set; }
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public static GameState CurrGameState { get; private set; }

        /// <summary>
        /// 所有玩法的结果集合
        /// </summary>
        private static Dictionary<GameState, PayoutInfo> Results;

        /// <summary>
        /// 记录Base中Bonus时Coin的位置和等级
        /// </summary>
        public static Dictionary<int, int> CoinPosLevel = new Dictionary<int, int>();
        /// <summary>
        /// 记录Base中Bonus时Coin的位置和分数
        /// </summary>
        public static Dictionary<int, int> CoinPosScore = new Dictionary<int, int>();

        public static Dictionary<int, JackpotType> IDJackpotDic = new Dictionary<int, JackpotType>()
        {
        {0,JackpotType.Mini },
        {1,JackpotType.Minor},
        {2,JackpotType.Major},
        {3,JackpotType.Grand},
        {4,JackpotType.Null},
        };
        public static Dictionary<JackpotType, int> JackpotIDDic = new Dictionary<JackpotType, int>()
        {
        {JackpotType.Mini ,0},
        {JackpotType.Minor,1},
        {JackpotType.Major,2},
        {JackpotType.Grand,3}
        };
        #endregion

        #region 事件监听
        private static Dictionary<PayMode, Action<ResultNode>> OnDeduceEvents;
        private static Dictionary<PayMode, Func<ResultNode, int>> OnEvaluateEvents;
        private static Dictionary<PayMode, Action<ResultNode>> OnAbortDeduceEvents;

        /// <summary>
        /// 一轮完整游戏结束时的监听事件
        /// </summary>
        public static event Action OnFinalizedEvent;
        /// <summary>
        /// 一轮完整游戏开始时的监听事件
        /// </summary>
        public static event Action OnSpinStartedEvent;
        /// <summary>
        /// 进入Bonus模式时的监听事件
        /// </summary>
        public static event Action OnToBonusEvent;
        /// <summary>
        /// 获取到新结果时的监听事件
        /// </summary>
        public static event Action<string> OnNewResultEvent;
        /// <summary>
        /// 切换Bet时的监听事件
        /// </summary>
        public static event Action<int> OnBetChangeEvent;
        /// <summary>
        /// AutoPlay状态切换时的监听事件
        /// </summary>
        public static event Action<bool> OnAutoPlayChangeEvent;
        public static event Action OnResetCattleEvent;
        #endregion

        /// <summary>
        /// 初始化函数
        /// </summary>
        [AutoLoad(3)]
        private static void Initialize()
        {
            BetLevel = 0;
            BetValue = ConfigManager.betConfig.BaseValue;
            CurrCredit = ConfigManager.betConfig.BaseCredit;
            Results = new Dictionary<GameState, PayoutInfo>();
            UnityEngine.Random.InitState(DateTime.Now.Millisecond);
            Meters = new double[ConfigManager.meterConfig.MeterBase.Length];
            Array.Copy(ConfigManager.meterConfig.MeterBase, Meters, Meters.Length);
            OnDeduceEvents = new Dictionary<PayMode, Action<ResultNode>>();
            OnEvaluateEvents = new Dictionary<PayMode, Func<ResultNode, int>>();
            OnAbortDeduceEvents = new Dictionary<PayMode, Action<ResultNode>>();
        }

        #region 游戏信息相关接口

        /// <summary>
        /// Base里每次点Spin都是一次消费行为
        /// </summary>
        public static void Consume()
        {
            UpdateCredit(-BetValue);
        }
        /// <summary>
        /// 检查余额是否充足
        /// </summary>
        /// <returns></returns>
        public static bool CheckCredit()
        {
            return CurrCredit >= BetValue;
        }
        /// <summary>
        /// 获取相应倍率
        /// </summary>
        /// <returns></returns>
        public static int GetMultiplyer()
        {
            return ConfigManager.betConfig.BetMultiplyer[BetLevel];
        }
        /// <summary>
        /// 获取游戏结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public static T GetResult<T>(GameState state) where T : PayoutInfo
        {
            if (Results.ContainsKey(state))
            {
                if (Results[state] is T)
                {
                    return Results[state] as T;
                }
                else
                {
                    Debug.LogErrorFormat("数据类型{0}无法转换，请检查类型", typeof(T).Name);
                    return default;
                }
            }
            else
            {
                Debug.LogErrorFormat("游戏状态{0}的结果尚不明确", state.ToString());
                return default;
            }
        }
        /// <summary>
        /// 获取游戏结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public static PayoutInfo GetResult(GameState state)
        {
            if (Results.ContainsKey(state))
            {
                return Results[state];
            }
            else
            {
                Debug.LogErrorFormat("游戏状态{0}的结果尚不明确", state.ToString());
                return default;
            }
        }
        /// <summary>
        /// 根据当前游戏状态获取结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetResultByCurrentState<T>() where T : PayoutInfo
        {
            return GetResult<T>(CurrGameState);
        }
        /// <summary>
        /// 根据当前游戏状态获取结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PayoutInfo GetResultByCurrentState()
        {
            return GetResult(CurrGameState);
        }
        #endregion

        #region 数据更新接口
        public static void SetSpinStarted()
        {
            IsFinalized = false;
            IsSpinStarted = true;
            OnSpinStartedEvent?.Invoke();
        }
        /// <summary>
        /// 设置本轮游戏是否结束的状态
        /// </summary>
        /// <param name="isOver">本轮游戏是否已经结束</param>
        public static void SetFinalizeState()
        {
            IsFinalized = true;
            IsSpinStarted = false;
            OnFinalizedEvent?.Invoke();
        }
        public static void ToBonusState()
        {
            OnToBonusEvent?.Invoke();
        }
        public static void NewResult(string info)
        {
            OnNewResultEvent?.Invoke(info);
        }
        /// <summary>
        /// 设置游戏AutoPlay状态
        /// </summary>
        /// <param name="isEnable">是否启用AutoPlay</param>
        /// <returns>返回设置后的AutoPlay状态，某些情况下不允许启用</returns>
        public static bool SetAutoPlay(bool isEnable)
        {
            if (isEnable == true)
            {
                if (CurrGameState == GameState.Base && CheckCredit())
                {
                    AutoSpinEnabled = true;
                }
            }
            else
            {
                AutoSpinEnabled = false;
            }
            OnAutoPlayChangeEvent?.Invoke(AutoSpinEnabled);
            return AutoSpinEnabled;
        }
        public static void ResetCattle()
        {
            OnResetCattleEvent?.Invoke();
        }
        /// <summary>
        /// Base里每次点Spin刷新一次Meter
        /// </summary>
        public static void UpdateMeters()
        {
            int[] increment = ConfigManager.meterConfig.Increment;
            int[] exchangeRate = ConfigManager.meterConfig.ExchangeRate;
            for (int i = 0; i < Meters.Length; i++)
            {
                /*ConfigManager.baseConfig.CoinValueByLevel[3][i + 3] = */
                Meters[i] = Math.Round(Meters[i] + (increment[i] * 1f / exchangeRate[i] * (BetLevel + 1)), 3);
            }
        }
        /// <summary>
        /// 重置Meter,因为最大的两个奖项数值会随着Spin次数增长，因此Jackpot中这两个大奖之后需要重置
        /// </summary>
        public static double ResetMeter(int id)
        {
            Meters[id] = ConfigManager.meterConfig.MeterBase[id];
            if (id >= 2)
            {
                Meters[id] *= 1/*GetMultiplyer()*/;
            }
            return Meters[id];
        }
        /// <summary>
        /// 重置TotalWin
        /// </summary>
        public static void ResetTotalWin()
        {
            TotalWin = 0;
        }
        /// <summary>
        /// 更新TotalWin
        /// </summary>
        /// <param name="win"></param>
        public static void UpdateWin(PayoutInfo result)
        {
            //GlobalObserver的TotalWin是游戏全部赢钱数量，
            //如果连续触发了多个玩法，每个玩法赢的钱都会包含在这里
            TotalWin += result.WinMoney;
            //这里代表每个玩法自身累积的TotalWin，
            //例如：FreeGame中有8轮游戏，这里将会累积8轮总钱数
            result.TotalWin += result.WinMoney;
        }
        public static void UpdateWin(int credit)
        {
            TotalWin += credit;
        }
        /// <summary>
        /// 更新余额
        /// </summary>
        /// <param name="count"></param>
        public static void UpdateCredit(int count)
        {
            CurrCredit += count;
        }
        /// <summary>
        /// 更新押金等级
        /// </summary>
        public static int UpdateBetLevel(int level)
        {
            BetLevel += level;
            BetLevel = Mathf.Clamp(BetLevel, 0, ConfigManager.betConfig.BetMultiplyer.Length - 1);
            BetValue = ConfigManager.betConfig.BaseValue * ConfigManager.betConfig.BetMultiplyer[BetLevel];
            Meters[2] = ConfigManager.meterConfig.MeterBase[2] * GetMultiplyer();
            Meters[3] = ConfigManager.meterConfig.MeterBase[3] * GetMultiplyer();

            OnBetChangeEvent?.Invoke(BetLevel);
            return BetValue;
        }
        /// <summary>
        /// 更新bet等级
        /// </summary>
        public static int UpdateBetLevel2(int level)
        {
            BetLevel += level;
            //BetLevel = Mathf.Clamp(BetLevel, 0, ConfigManager.betConfig.BetMultiplyer.Length - 1);
            BetLevel %= 5;
            BetValue = ConfigManager.betConfig.BaseValue * ConfigManager.betConfig.BetMultiplyer[BetLevel];

            OnBetChangeEvent?.Invoke(BetLevel);
            return BetValue;
        }
        /// <summary>
        /// 更新游戏结果
        /// </summary>
        /// <param name="result"></param>
        public static void UpdateResult(PayoutInfo result)
        {
            if (Results.ContainsKey(result.State))
            {
                Results[result.State] = result;
            }
            else
            {
                Results.Add(result.State, result);
            }
        }
        /// <summary>
        /// 更新游戏状态
        /// </summary>
        /// <param name="state"></param>
        public static void UpdateGameState(GameState state)
        {
            if (CurrGameState != state)
            {
                LastGameState = CurrGameState;
                CurrGameState = state;
            }
        }
        #endregion

        #region 事件行为
        /// <summary>
        /// 过程推演(播放中奖动画)
        /// </summary>
        /// <param name="mode"></param>
        public static void OnDeduce(PayMode mode)
        {
            if (OnDeduceEvents.ContainsKey(mode))
            {
                PayoutInfo info = GetResultByCurrentState();
                foreach (var item in info.ResultNodes)
                {
                    OnDeduceEvents[mode]?.Invoke(item);
                }
            }
        }
        /// <summary>
        /// 评估结果(评估行为会彻底完善当前游戏状态的结果)
        /// </summary>
        public static void OnEvaluate(PayMode mode)
        {
            if (OnEvaluateEvents.ContainsKey(mode))
            {
                PayoutInfo info = GetResultByCurrentState();
                foreach (var item in info.ResultNodes)
                {
                    if (OnEvaluateEvents[mode] != null)
                    {
                        info.WinMoney += OnEvaluateEvents[mode].Invoke(item);
                    }
                }
                UpdateWin(info);
            }
        }
        /// <summary>
        /// 终止推演(停止播放中奖动画)
        /// </summary>
        /// <param name="mode"></param>
        public static void OnAbortDeduce(PayMode mode)
        {
            if (OnAbortDeduceEvents.ContainsKey(mode) && Results.ContainsKey(CurrGameState))
            {
                PayoutInfo info = GetResultByCurrentState();
                foreach (var item in info.ResultNodes)
                {
                    OnAbortDeduceEvents[mode]?.Invoke(item);
                }
            }
        }
        #endregion

        #region 事件注册
        /// <summary>
        /// 注册推演模块
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="action"></param>
        public static void RegisterDeduce(PayMode mode, Action<ResultNode> action)
        {
            if (OnDeduceEvents.ContainsKey(mode))
            {
                OnDeduceEvents[mode] -= action;
                OnDeduceEvents[mode] += action;
            }
            else
            {
                OnDeduceEvents.Add(mode, action);
            }
        }
        /// <summary>
        /// 注册评估模块
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="action"></param>
        public static void RegisterEvaluate(PayMode mode, Func<ResultNode, int> action)
        {
            if (OnEvaluateEvents.ContainsKey(mode))
            {
                OnEvaluateEvents[mode] -= action;
                OnEvaluateEvents[mode] += action;
            }
            else
            {
                OnEvaluateEvents.Add(mode, action);
            }
        }
        /// <summary>
        /// 注册终止推演模块
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="action"></param>
        public static void RegisterAbortDeduce(PayMode mode, Action<ResultNode> action)
        {
            if (OnAbortDeduceEvents.ContainsKey(mode))
            {
                OnAbortDeduceEvents[mode] -= action;
                OnAbortDeduceEvents[mode] += action;
            }
            else
            {
                OnAbortDeduceEvents.Add(mode, action);
            }
        }

        /// <summary>
        /// 根据概率随机一个索引，概率总和为1
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int GetRandomWeightedIndex(float[] weights)
        {
            float weightSum = 0;
            float Random = UnityEngine.Random.Range(0f, 1f);
            //Debug.Log("随机数：" + Random);
            for (int i = 0; i < weights.Length; i++)
            {
                weightSum += weights[i];
                if (Random <= weightSum)
                {
                    return i;
                }
            }
            Debug.LogError("Random Error");
            return 1;
        }
        /// <summary>
        /// 根据概率随机一个索引，概率递增至1
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int GetRandomWeightedIndex_V(float[] weights)
        {
            float weight = 0;
            float Random = UnityEngine.Random.Range(0f, 1f);
            //Debug.Log("随机数：" + Random);
            for (int i = 0; i < weights.Length; i++)
            {
                weight = weights[i];
                if (Random <= weight)
                {
                    return i;
                }
            }
            Debug.LogError("Random Error");
            return 1;
        }

        public static void ListSortRandom<T>(this List<T> list)
        {
            int randomIndex;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                randomIndex = Random.Range(0, i);
                (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
            }
        }

        public static T GetRandomItem<T>(this List<T> list)
        {
            T item = list[Random.Range(0, list.Count)];
            return item;
        }

        public static void ListSorting(this List<int> list, bool sortType = true)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count - 1 - i; j++)
                {
                    if (sortType)
                    {
                        if (list[j] > list[j + 1])
                        {
                            (list[j], list[j + 1]) = (list[j + 1], list[j]);
                        }
                    }
                    else
                    {
                        if (list[j] < list[j + 1])
                        {
                            (list[j], list[j + 1]) = (list[j + 1], list[j]);
                        }
                    }
                }
            }
        }


        #endregion
    }
}
