﻿namespace SlotGame.Core.Reel
{
    using System;
    using UnityEngine;
    using DG.Tweening;
    using System.Linq;
    using SlotGame.Symbol;
    using SlotGame.Reel.Args;
    using System.Collections.Generic;
    using UniversalModule.SpawnSystem;
    using UniversalModule.AudioSystem;
    using SlotGame.Config;

    public class ReelBase : MonoBehaviour, IReelState
    {
        #region 字段
        protected bool isReverse;
        /// <summary>
        /// 转盘SymbolID;
        /// </summary>
        protected int stripeID;
        /// <summary>
        /// 安装ID
        /// </summary>
        protected int installID;
        /// <summary>
        /// 本轮最大移动次数
        /// </summary>
        protected int maxMoveStep;
        /// <summary>
        /// 当前移动次数
        /// </summary>
        protected int currMoveStep;

        /// <summary>
        /// 是否暂停
        /// </summary>
        protected bool isPause;
        /// <summary>
        /// 是否手动停止转轮
        /// </summary>
        protected bool isShutDown;

        /// <summary>
        /// 记录起点坐标，每次Spin之后需要还原
        /// </summary>
        protected Vector3 originPoint;
        /// <summary>
        /// 新Symbol的刷新位置
        /// </summary>
        protected Vector3 refreshPoint;

        /// <summary>
        /// 点Spin传入的临时参数
        /// </summary>
        protected ReelSpinArgs reelSpinArgs;
        /// <summary>
        /// Symbol队列，保留转盘中的Symbol
        /// </summary>
        protected List<CommonSymbol> SymbolList;

        /// <summary>
        /// 当前Reel设置
        /// </summary>
        [SerializeField] protected GameObject hyperAnim = null;
        [SerializeField] protected ReelSettingArgs setting = null;
        #endregion

        #region 属性
        public int InstallID { get { return installID; } }
        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPause { get { return isPause; } }
        /// <summary>
        /// 是否手动停止
        /// </summary>
        public bool IsShutDown { get { return isShutDown; } }
        /// <summary>
        /// 是否可以无限滚动
        /// </summary>
        public bool IsInfinity { get; set; }
        /// <summary>
        /// 当前Reel设置
        /// </summary>
        public ReelSettingArgs Setting { get { return setting; } }
        #endregion

        #region 事件监听
        /// <summary>
        /// 每移动一格的回调事件
        /// </summary>
        public virtual event Action<ReelBase> OnStepListener;
        /// <summary>
        /// 转轮停止的回调事件
        /// </summary>
        public virtual event Action<ReelBase> OnStopListener;
        #endregion

        #region 虚函数接口
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="args">Reel设置相关参数</param>
        public virtual void Initialize(ReelSettingArgs args)
        {
            installID = args.reelID;
            refreshPoint = Vector3.zero;
            if (args != null) setting = args;
            originPoint = transform.localPosition;
            SymbolList = new List<CommonSymbol>();
            stripeID = setting.reelStripes.Length - setting.symbolCount;
            stripeID %= setting.reelStripes.Length;
            for (int i = 0; i < setting.symbolCount; i++)
            {
                CommonSymbol symbol = GetNextSymbol();
                Vector3 localPos = Vector3.down * (i * setting.moveDistance) - (Vector3.forward * (i * 0.2f + 1));
                symbol.Install(transform, localPos, installID, args.defaultLayer);
                SymbolList.Add(symbol);
            }
            SymbolList.Reverse();
        }
        /// <summary>
        /// 当前Reel开始Spin
        /// </summary>
        /// <param name="args">本轮Spin所需要的参数</param>
        public virtual void Spin(ReelSpinArgs args)
        {
            isReverse = false;
            currMoveStep = 0;
            isShutDown = false;
            reelSpinArgs = args;
            stripeID = args.stripeID;
            refreshPoint = Vector3.zero;
            Vector3 endPoint = Vector3.down * setting.moveDistance;
            maxMoveStep = setting.moveStep + (args.isHyper ? args.hyperStep : 0)+ (args.isLong ? args.longStep : 0);
            transform.DOLocalMove(endPoint, setting.moveSpeed * 5).SetRelative().SetEase(setting.beginCurve).OnComplete(Rolling);
            DisplayTheInvisibleSymbol(true);
        }
        /// <summary>
        /// 当前Reel开始倒转
        /// </summary>
        /// <param name="args"></param>
        public virtual void ReverseSpin(ReelSpinArgs args)
        {
            isReverse = true;
            currMoveStep = 0;
            isShutDown = false;
            reelSpinArgs = args;
            stripeID = args.stripeID;
            refreshPoint = new Vector3(0, setting.moveDistance * -3);
            Vector3 endPoint = Vector3.up * setting.moveDistance;
            maxMoveStep = /*setting.moveStep + (args.isHyper ? args.hyperStep : 0) + (args.isLong ? args.longStep : 0)*/2;
            //transform.DOLocalMove(endPoint, setting.moveSpeed * 5).SetRelative().SetEase(setting.beginCurve).OnComplete(ReverseRolling);
            ReverseRolling();
            DisplayTheInvisibleSymbol(true);
        }

        /// <summary>
        /// 始终保持滚动，直到结束
        /// </summary>
        protected virtual void Rolling()
        {
            if (!IsInfinity)
            {
                currMoveStep++;
            }
            RefreshSymbol();
            if (currMoveStep < maxMoveStep)
            {
                Vector3 endPoint = Vector3.down * setting.moveDistance;
                if (currMoveStep == maxMoveStep - 1)
                {
                    transform.DOLocalMove(endPoint, setting.moveSpeed * 5).SetRelative().SetEase(setting.finishCurve).OnComplete(Rolling);
                }
                else
                {
                    float speed = setting.moveSpeed;
                    if (currMoveStep >= setting.moveStep + reelSpinArgs.WaitStep+ (reelSpinArgs.isLong ? reelSpinArgs.longStep : 0) && !isShutDown)
                    {
                        speed = setting.moveSpeed * 0.5f;
                        if (reelSpinArgs.isHyper)
                        {
                            reelSpinArgs.isHyper = false;
                            AudioManager.Playback("FE_hyper");
                            SetHyperState(true);
                        }
                    }
                    transform.DOLocalMove(endPoint, speed).SetRelative().OnComplete(Rolling);
                }
            }
            else
            {
                Stop();
            }
            OnStepListener?.Invoke(this);
        }
        protected virtual void ReverseRolling()
        {
            if (!IsInfinity)
            {
                currMoveStep++;
            }
            ReverseRefreshSymbol();
            if (currMoveStep < maxMoveStep)
            {
                Vector3 endPoint = Vector3.up * setting.moveDistance;
                /*if (currMoveStep == maxMoveStep - 1)
                {
                    transform.DOLocalMove(endPoint, setting.moveSpeed * 4).SetRelative().SetEase(setting.reverseCurve).OnComplete(ReverseRolling);
                    //transform.DOLocalMove(endPoint, setting.moveSpeed * 5).SetRelative().SetEase(setting.finishCurve).OnComplete(ReverseRolling);
                }
                else
                {*/
                float speed = setting.moveSpeed;
                if (currMoveStep >= setting.moveStep + reelSpinArgs.WaitStep + (reelSpinArgs.isLong ? reelSpinArgs.longStep : 0) && !isShutDown)
                {
                    speed = setting.moveSpeed * 0.5f;
                    if (reelSpinArgs.isHyper)
                    {
                        reelSpinArgs.isHyper = false;
                        AudioManager.Playback("FE_hyper");
                        SetHyperState(true);
                    }
                }
                transform.DOLocalMove(endPoint, speed * 6).SetRelative().SetEase(setting.reverseCurve).OnComplete(ReverseRolling);
                /*}*/
            }
            else
            {
                ReverseStop();
            }
            OnStepListener?.Invoke(this);
        }

        /// <summary>
        /// 手动停止所有Reel
        /// </summary>
        /// <returns>当剩余移动次数小于一个阈值时，无法手动停止，此时将返回False</returns>
        public virtual bool ShutDown()
        {
            int step = maxMoveStep - (setting.symbolCount - 1);
            if (currMoveStep >= 0 && currMoveStep < step)
            {
                currMoveStep = step;
                isShutDown = true;
            }
            return isShutDown;
        }
        /// <summary>
        /// 旋转停止
        /// </summary>
        protected virtual void Stop()
        {
            SetHyperState(false);
            AudioManager.Stop("Hyper");
            //停止之后将Reel和Symbol重新归位
            transform.localPosition = originPoint;
            int origin = setting.symbolCount - 1;
            for (int i = origin; i >= 0; i--)
            {
                Vector3 localPos = Vector3.down * (i * setting.moveDistance) - (Vector3.forward * (i * 0.2f + 1));
                CommonSymbol symbol = SymbolList[setting.symbolCount - 1 - i];
                symbol.Install(transform, localPos, setting.reelID, setting.defaultLayer);
                symbol.UpdateIndexOnReel(i);
                if (i == origin || i < origin - (setting.symbolCount - 3))
                {
                    /*if (installID == 7)
                    {
                        if (symbol.ItemName == SymbolNames.SCATTER)
                        {
                            (symbol as ScatterSymbol).SetLongFireDisplay(false);
                        }
                    }
                    else
                    {*/
                        symbol.SetDisplay(false);
                    /*}*/
                }
            }
            if (isShutDown)
            {
                AudioManager.PlayOneShot("ReelStop");
            }
            else
            {
                if (installID < 5 || installID == 7)
                    AudioManager.Playback("ReelStop");
            }
            OnStopListener?.Invoke(this);
        }

        protected virtual void ReverseStop()
        {
            SetHyperState(false);
            AudioManager.Stop("Hyper");
            //停止之后将Reel和Symbol重新归位
            transform.localPosition = originPoint;
            int origin = setting.symbolCount - 1;
            for (int i = origin; i >= 0; i--)
            {
                Vector3 localPos = Vector3.down * (i * setting.moveDistance) - (Vector3.forward * (i * 0.2f + 1));
                CommonSymbol symbol = SymbolList[i];
                symbol.Install(transform, localPos, setting.reelID, setting.defaultLayer);
                symbol.UpdateIndexOnReel(i);
                if (i == 3 || i < origin - (setting.symbolCount - 3))
                {
                    symbol.SetDisplay(false);
                }
            }
            if (isShutDown)
            {
                AudioManager.PlayOneShot("ReelStop");
            }
            else
            {
                if (installID < 5 || installID == 7)
                    AudioManager.Playback("ReelStop");
            }
            OnStopListener?.Invoke(this);
        }

        /// <summary>
        /// 替换整列Symbol
        /// </summary>
        /// <param name="symbolName"></param>
        public virtual void ReplaceAllSymbol(string symbolName)
        {
            for (int i = 0; i < SymbolList.Count; i++)
            {
                CommonSymbol oldSymbol = SymbolList[i];
                if (oldSymbol.IndexOnReel >= 2 && oldSymbol.IndexOnReel <= SymbolList.Count - 2)
                {
                    if (!oldSymbol.ItemName.Equals(symbolName))
                    {
                        ReplaceSymbol(symbolName, oldSymbol.IndexOnReel);
                    }
                }
            }
        }
        /// <summary>
        /// 替换指定位置的Symbol
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="indexOnReel"></param>
        public virtual CommonSymbol ReplaceSymbol(string symbolName, int indexOnReel)
        {
            /*CommonSymbol oldSymbol = SymbolList.FirstOrDefault(item => item.IndexOnReel.Equals(indexOnReel));
            return ReplaceSymbol(symbolName, oldSymbol);*/
            CommonSymbol symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            symbol.Install(SymbolList[indexOnReel].transform.parent, SymbolList[indexOnReel].transform.localPosition, installID);
            SymbolList[indexOnReel].Recycle();
            SymbolList.Remove(SymbolList[indexOnReel]);
            SymbolList.Insert(indexOnReel, symbol);
            return symbol;
        }
        /// <summary>
        /// 替换指定位置的Symbol
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="id"></param>
        public virtual CommonSymbol ReplaceSymbol(string symbolName, CommonSymbol oldSymbol)
        {
            int idx = SymbolList.IndexOf(oldSymbol);
            if (idx != -1)
            {
                CommonSymbol newSymbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                newSymbol.Install(oldSymbol.transform.parent, oldSymbol.transform.localPosition, installID, setting.defaultLayer);
                newSymbol.UpdateIndexOnReel(oldSymbol.IndexOnReel);

                SymbolList.Remove(oldSymbol);
                SymbolList.Insert(idx, newSymbol);
                oldSymbol.Recycle();
                return newSymbol;
            }
            else
            {
                Debug.LogWarningFormat("未从Reel-{0}中找到名为<{1}>的Symbol", setting.reelID, oldSymbol.ItemName);
                return oldSymbol;
            }
        }
        /// <summary>
        /// 隐藏不可见Symbol
        /// </summary>
        public virtual void DisplayTheInvisibleSymbol(bool isDisplay)
        {
            CommonSymbol[] symbols = GetAllSymbols();
            for (int i = 0; i < symbols.Length; i++)
            {
                if (i < 2 || i >= setting.symbolCount - 1)
                {
                    symbols[i].SetDisplay(isDisplay);
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取全部Symbol,包括不可见
        /// </summary>
        /// <returns></returns>
        public virtual CommonSymbol[] GetAllSymbols()
        {
            CommonSymbol[] symbols = SymbolList.ToArray();
            if (isReverse)
            {
                symbols = symbols.ToArray();
            }
            else
            {
                symbols = symbols.Reverse().ToArray();
            }
            return symbols;
        }
        /// <summary>
        /// 获取全部可见Symbol
        /// </summary>
        /// <returns></returns>
        public CommonSymbol[] GetVisibleSymbols()
        {
            CommonSymbol[] symbols = GetAllSymbols();
            CommonSymbol[] visible = new CommonSymbol[symbols.Length - 3];
            Array.Copy(symbols, 2, visible, 0, visible.Length);
            return visible;
        }

        #region 辅助工具
        /// <summary>
        /// 获取Spin参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetSpinArgs<T>() where T : ReelSpinArgs
        {
            if (reelSpinArgs is T)
            {
                return reelSpinArgs as T;
            }
            else
            {
                Debug.LogErrorFormat("ReelSpinArgs无法转换成类型{0}", typeof(T).Name);
                return default;
            }
        }
        /// <summary>
        /// 刷新Symbol位置
        /// </summary>
        protected virtual void RefreshSymbol()
        {
            SymbolList[0].Recycle();
            SymbolList.RemoveAt(0);
            for (int i = 0; i < SymbolList.Count; i++)
            {
                SymbolList[i].UpdateIndexOnReel(SymbolList.Count - i);
            }
            CommonSymbol symbol = GetNextSymbol();
            refreshPoint += setting.moveDistance * Vector3.up;
            symbol.Install(transform, refreshPoint, installID, setting.defaultLayer);
            SymbolList.Add(symbol);
        }
        protected void ReverseRefreshSymbol()
        {
            SymbolList[SymbolList.Count - 1].Recycle();
            SymbolList.RemoveAt(SymbolList.Count - 1);
            for (int i = SymbolList.Count - 1; i >= 0; i--)
            {
                SymbolList[i].UpdateIndexOnReel(SymbolList.Count - i);
            }
            CommonSymbol symbol = ReverseGetNextSymbol();
            refreshPoint += setting.moveDistance * Vector3.down;
            symbol.Install(transform, refreshPoint, installID, setting.defaultLayer);
            SymbolList.Insert(0, symbol);
            //symbol.UpdateIndexOnReel(0);
        }
        /// <summary>
        /// 获取下一步要刷新的Symbol
        /// </summary>
        /// <returns></returns>
        protected virtual CommonSymbol GetNextSymbol()
        {
            int id = stripeID++;
            stripeID %= setting.reelStripes.Length;
            string symbolName;
            //symbolName = setting.reelStripes[id];
            //过程Symbol
            if (currMoveStep < maxMoveStep - (setting.symbolCount - 2) || currMoveStep > maxMoveStep - 2)
            {
                symbolName = setting.reelStripes[id];
            }
            //结果Symbol
            else
            {
                id = maxMoveStep - currMoveStep - 2;
                symbolName = reelSpinArgs.resultSymbols[id];
            }
            //symbolName = SymbolNames.SCATTER;
            var symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
            if (symbol.ItemName == SymbolNames.COIN)
            {
                CoinSymbol coinSymbol = symbol as CoinSymbol;
                coinSymbol.InitLevel(installID);
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][coinSymbol.Level]);
                if (installID % 5 == 3 && coinSymbol.Level == 3)
                {
                    index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][4]);
                }
                int score = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][coinSymbol.Level][index];
                coinSymbol.SetScore(score);
            }
            return symbol;
        }
        protected virtual CommonSymbol ReverseGetNextSymbol()
        {
            int id = stripeID++;
            stripeID %= setting.reelStripes.Length;
            string symbolName;
            CommonSymbol symbol;
            //symbolName = setting.reelStripes[id];
            //过程Symbol
            if (currMoveStep < maxMoveStep - (setting.symbolCount - 2) || currMoveStep > maxMoveStep - 2)
            {
                symbolName = setting.reelStripes[id];
                if (symbolName == SymbolNames.SCATTER)
                {
                    symbolName = SymbolNames.WILD;
                }
                if (symbolName == SymbolNames.COIN)
                {
                    int level = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinLevelProbability[installID % 5]) + 1;
                    int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][level]);
                    if (installID % 5 == 3 && level == 3)
                    {
                        index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][4]);
                    }
                    if (level == 3 && index > ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4 - 1)
                    {
                        JackpotType jackpotType = GlobalObserver.IDJackpotDic[index - (ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4)];
                        symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                        (symbol as CoinSymbol).SetScore(jackpotType);
                    }
                    else
                    {
                        int score = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][level][index];
                        symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                        (symbol as CoinSymbol).Level = level;
                        (symbol as CoinSymbol).SetScore(score);
                    }

                }
                else
                {
                    symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                }
            }
            //结果Symbol
            else
            {
                id = /*maxMoveStep - currMoveStep - 1*/0;
                symbolName = reelSpinArgs.resultSymbols[id];
                symbol = SpawnFactory.GetObject<CommonSymbol>(symbolName);
                if (symbol.ItemName == SymbolNames.COIN)
                {
                    CoinSymbol coinSymbol = symbol as CoinSymbol;
                    coinSymbol.Level = reelSpinArgs.coinLevel;
                    coinSymbol.SetScore(reelSpinArgs.coinScore);
                    coinSymbol.SetScore(reelSpinArgs.coinJPType);
                    //coinSymbol.SetScore(GlobalObserver.IDJackpotDic[Random.Range(0,4)]);
                }
            }
            return symbol;
        }
        /// <summary>
        /// 设置Hyper状态
        /// </summary>
        /// <param name="isEnable"></param>
        protected void SetHyperState(bool isEnable)
        {
            if (hyperAnim != null)
            {
                hyperAnim.SetActive(isEnable);
            }
        }

        public void StartHyper()
        {
            maxMoveStep = setting.moveStep + reelSpinArgs.hyperStep;
            SetHyperState(true);
        }
        #endregion
    }
}
