using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Result;

public class BonusGameStateMachine : StateMachine
{
    public ReelContainer container;

    public override void Initialize()
    {
        base.Initialize();
        container.Initialize();
        container.OnStopAllListener += OnStopAllComplete;
        GlobalObserver.UpdateResult(Payout);
    }
    public override void OnTrigger()
    {
        Payout.Clear();
        Payout.ResetTotalWin();
        (container as BonusReelContainer).ToBonusInit();
    }
    public override void Enter()
    {
        OnReset();
        container.Spin();
    }

    public override PayoutInfo ReportTheResult()
    {
        StatisticalResult.UpdateResult(PayMode, Payout, container.Reels);
        return Payout;
    }

    public override bool ShutDown()
    {
        return container.ShutDown();
    }

    /// <summary>
    /// ����ת�ֶ�ֹͣʱ����
    /// </summary>
    /// <param name="container"></param>
    private void OnStopAllComplete(ReelContainer container)
    {
        OnComplate();
    }

}
