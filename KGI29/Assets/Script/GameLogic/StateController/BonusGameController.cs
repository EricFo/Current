using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Result;
using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class BonusGameController : StateProcessBase
{
    [Tooltip("剩余次数")] public BaseUI baseUI;
    [Tooltip("收集动画")] public CollectUI collectUI;
    public TotalWinUI totalWinUI;
    public BaseReelContainer baseReelContainer;
    public BonusReelContainer bonusReelContainer;
    private PayoutInfo result = null;

    public override GameState GameState
    {
        get { return GameState.Bonus; }
    }

    public override bool OnStateEnter()
    {
        if (BaseUI.Surplus > 0)
        {
            baseUI.Spin();
            UIController.BottomPanel.UpdatePressSpinBtnState();
            UIController.BottomPanel.StopBtn.interactable = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStateExit(PayoutInfo result)
    {
        this.result=GetStateMachine(GameState).ReportTheResult();
        AwardCoroutineComplete();
    }

    /// <summary>
    /// 处理奖励结束时的逻辑
    /// </summary>
    public void AwardCoroutineComplete()
    {
        //取消事件注册，防止在不需要的情况下执行
        UIController.BottomPanel.OnAwardComplateEvent -= AwardCoroutineComplete;
        //这里有一个Bonus模式是否结束的判断
        GlobalObserver.IsBonusGameIsOver = CheckProcess.CheckBonusGameIsOver(BaseUI.Surplus);
        Debug.LogWarning(GlobalObserver.IsBonusGameIsOver);
        if (GlobalObserver.IsBonusGameIsOver)
        {
            GetComponent<BonusReelContainer>().CurrentActive = 3;
            UIController.BottomPanel.DisableAllButtons();
            result.WinMoney = bonusReelContainer.TotalScore();
            GlobalObserver.UpdateWin(result);
            BonusGameIsOver();
        }
        else
        {
            UIController.BottomPanel.DisplaySpinBtn();
        }
    }

    /// <summary>
    /// 处理Bonus游戏结束相关的状态
    /// </summary>
    private void BonusGameIsOver()
    {
        int allCoin = 0;
        //第一个参数是reelID，第二个参数是金币分数
        Dictionary<int, BonusCoinSymbol> reelIds = new Dictionary<int, BonusCoinSymbol>();
        foreach (IReelState reel in bonusReelContainer.Reels)
        {
            CommonSymbol symbol = reel.GetVisibleSymbols()[0];
            if (symbol.ItemName.Equals(SymbolNames.BCOIN))
            {
                allCoin += (symbol as BonusCoinSymbol).ScoreValue;
                reelIds.Add(reel.Setting.reelID, (symbol as BonusCoinSymbol)); //
            }
        }

        DelayCallback.Delay(this, 0.5f, () => {
            collectUI.PlayFlyAll(reelIds);
            collectUI.collectStopEvent += TotalWinAction;
        });
    }


    /// <summary>
    /// BonusGame切换到BaseGame
    /// </summary>
    private void BonusGameToBaseGame()
    {
        totalWinUI.OnClickAction -= BonusGameToBaseGame;
        AudioManager.Stop(GlobalObserver.BonusMusic[GlobalObserver.CurrentBonusState]);
        AudioManager.Stop("KGI-Game29-Bonus-Totalwin2");
        AudioManager.Stop("KGI-Game29-Bonus-Totalwin1");
        //更新余额
        //GlobalObserver.UpdateCredit(GlobalObserver.CurrCredit);
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        //UIController.BottomPanel.UpdateCredit(GlobalObserver.CurrCredit);
        //更新游戏状态
        GlobalObserver.UpdateGameState(GameState.Base);
        GlobalObserver.UpdateCredit(GlobalObserver.TotalWin);

        JackPotData.meter.OpenTxts(false);
        //控制UI转场
        ViewTransition.TransitionTo(GameState.Bonus, GameState.Base);
        baseReelContainer.DisplayTheInvisibleSymbols(false);
        //恢复按钮状态
        UIController.BottomPanel.RestoreButtonDefultState();
        UIController.CheatPanel.UpdateFeatureBtnState(true);
        //取消事件注册，占位待修改

        bonusReelContainer.ResetTurntableLayout();

        if (GlobalObserver.IsUpgradePlay)
        {
            bonusReelContainer.turntableUI.TurntableBoostLevel = 1;
        }
        if (GlobalObserver.IsCollectPlay)
        {
            bonusReelContainer.turntableUI.TurntableGetallLevel = 1;
        }
        bonusReelContainer.turntableUI.TurntableBoostLevel = bonusReelContainer.turntableUI.TurntableBoostLevel;
        bonusReelContainer.turntableUI.TurntableGetallLevel = bonusReelContainer.turntableUI.TurntableGetallLevel;

        bonusReelContainer.turntableUI.PlayIdleAnimation();

        var turntableUpgrade = GetComponent<BonusReelContainer>().turntableUpgrade;
        var turntableCollect = GetComponent<BonusReelContainer>().turntableCollect;
        DelayCallback.Delay(this, 0.01f, () =>
        {
            if (turntableUpgrade.WheelStatus == Wheel.Status.Stop)
            {
                turntableUpgrade.StartRolling(turntableUpgrade.idlingSpeed.z);
            }
            if (turntableCollect.WheelStatus == Wheel.Status.Stop)
            {
                turntableCollect.StartRolling(turntableCollect.idlingSpeed.z);
            }
        });
        Cheat.isGrand = false;
        Cheat.isMajor = false;
        Cheat.isMini = false;
    }

    public void TotalWinAction()
    {
        UIController.BottomPanel.DisableAllButtons();
        AudioManager.Stop(GlobalObserver.BonusMusic[GlobalObserver.CurrentBonusState]);
        if (GlobalObserver.TotalWin > GlobalObserver.BetValue * 32.5f)
        {
            AudioManager.PlayOneShot("KGI-Game29-Bonus-Totalwin2");
        }
        else
        {
            AudioManager.PlayOneShot("KGI-Game29-Bonus-Totalwin1");
        }
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        totalWinUI.PlayTotalWin(result.TotalWin);
        DelayCallback.Delay(this, 3f, () =>
        {
            totalWinUI.OnClickEvent += BonusGameToBaseGame;
        });
    }

}
