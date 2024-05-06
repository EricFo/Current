using SlotGame.Core;
using SlotGame.Result;
using SlotGame.Core.Reel;

public class BaseGameStateMachine : StateMachine {
    public ReelContainer container;
    
    public override void Initialize() {
        base.Initialize();
        container.Initialize();
        container.OnStopAllListener += OnStopAllComplete;
    }

    public override void Enter() {
        OnReset();
        container.Spin();
    }

    public override bool ShutDown() {
        return container.ShutDown();
    }

    public override void OnReset() {
        base.OnReset();
        Payout.ResetTotalWin();
    }

    public override PayoutInfo ReportTheResult() {
        StatisticalResult.UpdateResult(PayMode, Payout, container.Reels);
        return Payout;
    }

    private void OnStopAllComplete(ReelContainer container) {
        OnComplate();
    }
}
