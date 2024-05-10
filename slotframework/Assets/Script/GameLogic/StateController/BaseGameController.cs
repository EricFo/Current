using SlotGame.Core;
using SlotGame.Result;
using static UnityEngine.Networking.UnityWebRequest;
using UniversalModule.AudioSystem;

public class BaseGameController : StateProcessBase {

    PayoutInfo result = null;
    bool isWinloopAudio = false;

    public override GameState GameState {
        get {
            return GameState.Base;
        }
    }

    public override bool OnStateEnter() {
        var machine = GetStateMachine(GameState);
        if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue) {
            GlobalObserver.Consume();
            GlobalObserver.UpdateMeters();
            GlobalObserver.ResetTotalWin();
            GlobalObserver.OnAbortDeduce(machine.PayMode);
            UIController.BottomPanel.ResetWinPanel();
            UIController.BottomPanel.UpdatePressSpinBtnState();
            UIController.MeterPanel.UpdateMeters(GlobalObserver.Meters);
            UIController.BottomPanel.UpdateCredit(GlobalObserver.CurrCredit);
            UIController.CheatPanel.UpdateFeatureBtnState(false);
            UIController.CheatPanel.HideCheatBtnList();
            GlobalObserver.SetSpinStarted();
            return true;
        } else {
            UIController.BottomPanel.DisableAllButtons();
            return false;
        }
    }

    public override void OnStateExit(PayoutInfo result) {
        GlobalObserver.IsTriggerJackpot = CheckProcess.CheckJackPotIsTrigger(result);
        GlobalObserver.IsTriggerFreeGame = CheckProcess.CheckFreeGameIsTrigger(result);

        WinMoneyAndDeduce();
    }

    /// <summary>
    /// 结算
    /// </summary>
    public void WinMoneyAndDeduce()
    {
        result = OnDeduceState();
        if (result.WinMoney > 0)
        {
            AudioManager.LoopPlayback("AwardLoop");
            isWinloopAudio = true;
            UIController.BottomPanel.OnAwardComplateEvent += OnBaseGameAwardComplate;
            UIController.BottomPanel.StopBtn.interactable = true;
            UIController.BottomPanel.AwardRollUp(result.WinMoney);

        }
        else
        {
            OnBaseGameAwardComplate();
        }
    }

    /// <summary>
    /// 处理奖励结束时的逻辑
    /// </summary>
    private void OnBaseGameAwardComplate() {
        //取消事件注册，防止在不需要的情况下执行
        UIController.BottomPanel.OnAwardComplateEvent -= OnBaseGameAwardComplate;
        PayoutInfo result = GlobalObserver.GetResult(GameState.Base);
        if (!GlobalObserver.IsTriggerJackpot) {
            //没触发JackPot要检查是否触发Free，触发JackPot的情况下先进JackPot
            if (GlobalObserver.IsTriggerBonusGame) {
                //处理触发Free的情况

                //To Do 此处应该处理FreeGame弹框逻辑，弹框结束后应该是转场
                var popup = GetPopupBox("BonusGame");
                //此处事件已经做了防止重复注册的处理，不需要再手动注销
                popup.OnClickEvent += BaseGameToFreeGame;
                popup.Show(1f,Cheat.isAutoPlay);

                //触发任何特殊玩法都要禁用掉Auto Play
                GlobalObserver.SetAutoPlay(false);
            } else { //没触发Free或JP就恢复按钮状态
                GlobalObserver.UpdateCredit(result.TotalWin);
                UIController.BottomPanel.RestoreButtonDefultState();
                UIController.CheatPanel.UpdateFeatureBtnState(true);
                UIController.BottomPanel.UpdateCredit(GlobalObserver.CurrCredit);
                GlobalObserver.SetFinalizeState();
                //如果启用AutoSpin就直接继续
                if (GlobalObserver.AutoSpinEnabled == true) {
                    UIController.BottomPanel.OnSpinBtnClick();
                }
            }
        } else {
            //触发任何特殊玩法都要禁用掉Auto Play
            GlobalObserver.SetAutoPlay(false);
        }
    }

    private void BaseGameToFreeGame() {
        //每次转场之前最好先停止当前模式下的赔付流程!!!
        var machine = GetStateMachine(GameState);
        GlobalObserver.OnAbortDeduce(machine.PayMode);
        Cheat.isTriggerBonusGame = false;
        GlobalObserver.IsTriggerBonusGame = false;
        GlobalObserver.ToBonusState();
        //更新游戏状态到Free
        GlobalObserver.UpdateGameState(GameState.Bonus);
        ViewTransition.TransitionTo(GameState.Base, GameState.Bonus);
        UIController.BottomPanel.DisplaySpinBtn();
        GetStateMachine(GameState.Bonus).OnTrigger();
    }

    /// <summary>
    /// 赔付
    /// </summary>
    protected PayoutInfo OnDeduceState()
    {
        IStateMachine machine = GetStateMachine(GameState.Base);
        PayoutInfo result = machine.ReportTheResult();
        GlobalObserver.UpdateResult(result);
        GlobalObserver.OnEvaluate(machine.PayMode);
        GlobalObserver.OnDeduce(machine.PayMode);
        return result;
    }

}
