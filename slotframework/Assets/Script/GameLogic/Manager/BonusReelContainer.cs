using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SlotGame.Core.Reel;
using System.Buffers.Text;
using System;
using SlotGame.Config;
using SlotGame.Reel.Args;
using SlotGame.Core;
using SlotGame.Symbol;

public class BonusReelContainer : ReelContainer
{
    public override event Action<IReelState> OnSpinReelListener;
    public override event Action<IReelState> OnStopReelListener;
    public override event Action<ReelContainer> OnSpinAllListener;
    public override event Action<ReelContainer> OnStopAllListener;

    /// <summary>
    /// Spin计数UI
    /// </summary>
    public SurplusUI surplusUI;


    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// 每次进入Bonus的初始化
    /// </summary>
    public void ToBonusInit()
    {
        surplusUI.Init();
        InitParameter();
        InitReplaceSymbolForCoin();
    }

    /// <summary>
    /// 初始化参数
    /// </summary>
    private void InitParameter()
    {

    }

    /// <summary>
    /// 初始化Symbol，继承Base的Coin
    /// </summary>
    void InitReplaceSymbolForCoin()
    {

        if (Cheat.isAutoPlay)
        {
            UIController.BottomPanel.OnSpinBtnClick();
        }
    }

    public override void Spin()
    {
        if (isRolling == false)
        {
            OnReset();
            isRolling = true;
            OnSpinAllListener?.Invoke(this);
            for (int i = 0; i < reels.Length; i++)
            {
                reels[i].Spin(Predict(i));
                OnSpinReelListener?.Invoke(reels[i]);
            }
        }
    }

    public override void OnReset()
    {
        base.OnReset();
    }

    protected override void OnReelStop(ReelBase reel)
    {
        stopCount++;
        OnStopReelListener?.Invoke(reel);
        if (stopCount >= reels.Length)
        {
            isRolling = false;
            OnStopAllListener?.Invoke(this);
        }
    }

    protected override string[] GetReelStripe(int id)
    {
        return ConfigManager.reelStripConfig.GetStripe("Bonus")[id];
    }
    protected override bool CheckScatter(int id, bool isScatter)
    {
        return false;
    }

}
