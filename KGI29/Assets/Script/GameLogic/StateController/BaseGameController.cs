using SlotGame.Core;
using SlotGame.Result;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using SlotGame.Symbol;

public class BaseGameController : StateProcessBase {
    PayoutInfo result = null;

    //[SerializeField] private BonusReelContainer bonusReelContainer;
    //[SerializeField] private BaseReelContainer baseReelContainer;

    IAudioPlayer Winloop;

    public override GameState GameState {
        get {
            return GameState.Base;
        }
    }

    public override bool OnStateEnter() {
        var machine = GetStateMachine(GameState);
        if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue) {
            UIController.BottomPanel.UpdateCredit(GlobalObserver.CurrCredit);
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
        this.result = result;
        if (GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame1 || Cheat.isTriggerBonusGame2 || Cheat.isTriggerBonusGame3) {
            //如果是作弊进的需要调整Bonus模式
            if (Cheat.isTriggerBonusGame1) {
                GlobalObserver.IsUpgradePlay = true;
                GlobalObserver.IsCollectPlay = false;
                GlobalObserver.CurrentBonusState = 1;
            }
            if (Cheat.isTriggerBonusGame2) {
                GlobalObserver.IsUpgradePlay = false;
                GlobalObserver.IsCollectPlay = true;
                GlobalObserver.CurrentBonusState = 2;
            }
            if (Cheat.isTriggerBonusGame3) {
                GlobalObserver.IsUpgradePlay = true;
                GlobalObserver.IsCollectPlay = true;
                GlobalObserver.CurrentBonusState = 3;
            }
        }
        if (BaseReelContainer.scatterCount > 0|| GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame1 || Cheat.isTriggerBonusGame2 || Cheat.isTriggerBonusGame3)
        {
            StartCoroutine(ScatterFly());
        }
        else
        {
            WinMoneyAndDeduce();
        }
    }

    IEnumerator ScatterFly()
    {
        yield return new WaitForSeconds(1.5f);
        //GetComponent<BaseReelContainer>().PlayScatterWin();

        if (GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame1 || Cheat.isTriggerBonusGame2 || Cheat.isTriggerBonusGame3)
        {
            if (GlobalObserver.IsUpgradePlay)
            {
                GetComponent<BaseReelContainer>().turntableUI.PlayTriggerAnimation(true);
            }
            if (GlobalObserver.IsCollectPlay)
            {
                GetComponent<BaseReelContainer>().turntableUI.PlayTriggerAnimation(false);
            }
        }
        if (GlobalObserver.IsTriggerBonusGame||Cheat.isTriggerBonusGame1 || Cheat.isTriggerBonusGame2 || Cheat.isTriggerBonusGame3) {
            float time = AudioManager.GetAudioTime("FeatureSelect");
            AudioManager.Playback("FeatureSelect");
            if (GlobalObserver.IsUpgradePlay && GlobalObserver.IsCollectPlay)
            {
                AudioManager.Playback("FE_JP_triggercombo");
            }
            for (int i = 0; i < GetComponent<BaseReelContainer>().Reels.Length; i++) {
                CommonSymbol[] symbols = GetComponent<BaseReelContainer>().Reels[i].GetVisibleSymbols();
                //ScatterCol\ScatterUp
                for (int j = 0; j < symbols.Length; j++) {
                    if (symbols[j].ItemName.Equals("ScatterUp") &&GlobalObserver.IsUpgradePlay ) {

                        (symbols[j] as ScatterSymbol).PlayWinAnim();
                    }
                    if (symbols[j].ItemName.Equals("ScatterCol") && GlobalObserver.IsCollectPlay)
                    {
                        (symbols[j] as ScatterSymbol).PlayWinAnim();
                    }
                }
            }
            yield return new WaitForSeconds(time);
        }
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
            Winloop = AudioManager.Playback("KGI29-Winloop");
            UIController.BottomPanel.OnAwardComplateEvent += OnBaseGameAwardComplate;
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

        if (Winloop != null) {
            Winloop.Stop();
            Winloop = null;
            AudioManager.PlayOneShot("KGI29-Winloopend");
        }
        //取消事件注册，防止在不需要的情况下执行
        UIController.BottomPanel.OnAwardComplateEvent -= OnBaseGameAwardComplate;
        PayoutInfo result = GlobalObserver.GetResult(GameState.Base);

        if (GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame1 || Cheat.isTriggerBonusGame2 || Cheat.isTriggerBonusGame3)
        {
            StartCoroutine(BaseGameToBonusGame2());
        }
        else
        { //没触发Free或JP就恢复按钮状态
            GlobalObserver.UpdateCredit(result.WinMoney);
            UIController.BottomPanel.RestoreButtonDefultState();
            UIController.CheatPanel.UpdateFeatureBtnState(true);
            GlobalObserver.SetFinalizeState();
            //如果启用AutoSpin就直接继续
            if (GlobalObserver.AutoSpinEnabled == true)
            {
                UIController.BottomPanel.OnSpinBtnClick();
            }
        }
    }

    private IEnumerator BaseGameToBonusGame2() {
        GlobalObserver.OnAbortDeduce(GetStateMachine(GameState).PayMode);
        float time = AudioManager.GetAudioTime("FeatureSelect");
        AudioManager.Playback("FeatureSelect");
        for (int i = 0; i < GetComponent<BaseReelContainer>().Reels.Length; i++) {
            CommonSymbol[] symbols = GetComponent<BaseReelContainer>().Reels[i].GetVisibleSymbols();
            //ScatterCol\ScatterUp
            for (int j = 0; j < symbols.Length; j++) {
                if (symbols[j].ItemName.Equals("ScatterUp") && GlobalObserver.IsUpgradePlay) {
                    (symbols[j] as ScatterSymbol).PlayWinAnim();
                }
                if (symbols[j].ItemName.Equals("ScatterCol") && GlobalObserver.IsCollectPlay)
                {
                    (symbols[j] as ScatterSymbol).PlayWinAnim();
                }
            }
        }
        yield return new WaitForSeconds(time);
        for (int i = 0; i < GetComponent<BaseReelContainer>().Reels.Length; i++) {
            CommonSymbol[] symbols = GetComponent<BaseReelContainer>().Reels[i].GetVisibleSymbols();
            //ScatterCol\ScatterUp
            for (int j = 0; j < symbols.Length; j++) {
                if (symbols[j].ItemName.Equals("ScatterUp") || symbols[j].ItemName.Equals("ScatterCol")) {
                    (symbols[j] as ScatterSymbol).PlayIdleAnim();
                }
            }
        }
        //处理触发Bonus的情况
        //To Do 此处应该处理FreeGame弹框逻辑，弹框结束后应该是转场
        var popup = GetPopupBox("BONUSGAME" + GlobalObserver.CurrentBonusState.ToString());
        //此处事件已经做了防止重复注册的处理，不需要再手动注销
        popup.OnClickEvent += BaseGameToBonusGame;
        popup.Show(result, 2f, false);
        AudioManager.PlayOneShot("KGI-Game29-Bonus-Intro");
        DelayCallback.Delay(this, 1f, () =>
        {
            AudioManager.LoopPlayback("KGI-Game29-Bonus-Waiting");
        });
        //触发任何特殊玩法都要禁用掉Auto Play
        GlobalObserver.SetAutoPlay(false);
    }

    private void BaseGameToBonusGame() {
        AudioManager.Stop("KGI-Game29-Bonus-Waiting");
        AudioManager.LoopPlayback(GlobalObserver.BonusMusic[GlobalObserver.CurrentBonusState]);
        GetPopupBox("BONUSGAME1").OnClickEvent -= BaseGameToBonusGame;
        //每次转场之前最好先停止当前模式下的赔付流程!!!
        var machine = GetStateMachine(GameState);
        GlobalObserver.OnAbortDeduce(machine.PayMode);

        GetComponent<BaseReelContainer>().turntableUI.PlayLoopIdleAnim();

        Cheat.isTriggerBonusGame1 = false;
        Cheat.isTriggerBonusGame2 = false;
        Cheat.isTriggerBonusGame3 = false;
        GlobalObserver.IsTriggerBonusGame = false;
        GlobalObserver.ToBonusState();
        //更新游戏状态到Bonus
        GlobalObserver.UpdateGameState(GameState.Bonus);
        //从Base状态过渡到Bonus状态
        ViewTransition.TransitionTo(GameState.Base, GameState.Bonus);
        //恢复按钮状态
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
