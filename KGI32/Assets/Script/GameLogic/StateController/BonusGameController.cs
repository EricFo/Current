using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Result;
using SlotGame.Core.Reel;
using SlotGame.Symbol;
using UniversalModule.DelaySystem;
using UniversalModule.AudioSystem;
using DG.Tweening;

public class BonusGameController : StateProcessBase
{
    public CollectUI collectUI;
    public TotalWinUI totalWinUI;
    public BaseReelContainer baseReelContainer;
    public BonusReelContainer bonusReelContainer;
    public BaseUI baseUI;
    private PayoutInfo result = null;

    public override GameState GameState
    {
        get { return GameState.Bonus; }
    }

    public override bool OnStateEnter()
    {
        if (baseUI.CheckRemaining() > 0)
        {
            baseUI.Spin();
            UIController.BottomPanel.UpdatePressSpinBtnState();
            //UIController.BottomPanel.StopBtn.interactable = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStateExit(PayoutInfo result)
    {
        this.result = GetStateMachine(GameState).ReportTheResult();
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
        GlobalObserver.IsBonusGameIsOver = CheckProcess.CheckBonusGameIsOver(baseUI.CheckRemaining());
        if (GlobalObserver.IsBonusGameIsOver)
        {
            UIController.BottomPanel.DisableAllButtons();
            BonusGameIsOver();
            baseUI.OnBonusIsOver();
        }
        else
        {
            if (Cheat.isAutoPlay)
            {
                UIController.BottomPanel.OnSpinBtnClick();
            }
            else
            {
                UIController.BottomPanel.DisplaySpinBtn();
            }
        }
    }

    /// <summary>
    /// 处理Bonus游戏结束相关的状态
    /// </summary>
    private void BonusGameIsOver()
    {
        List<BonusCoinSymbol> bonusCoinSymbols = new List<BonusCoinSymbol>();
        bonusReelContainer.exisCoinViews.ListSorting();
        foreach (var item in bonusReelContainer.exisCoinViews)
        {
            bonusCoinSymbols.Add(bonusReelContainer.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol);
        }
        StartCoroutine(collectUI.DisPlayCoinCredit(bonusCoinSymbols, result));
        collectUI.collectStopEvent += TotalWinAction;
    }

    /// <summary>
    /// BonusGame切换到BaseGame
    /// </summary>
    private void BonusGameToBaseGame()
    {
        bonusReelContainer.cattleUI.BonusToBase();
        baseReelContainer.flyCount = 0;
        totalWinUI.OnClickEvent -= BonusGameToBaseGame;
        AudioManager.Stop("KGI_Game32_Bonus_Totalwin2");
        AudioManager.Stop("KGI_Game32_Bonus_Totalwin1");
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        GlobalObserver.UpdateCredit(GlobalObserver.TotalWin);
        //更新游戏状态
        GlobalObserver.UpdateGameState(GameState.Base);
        //控制UI转场
        ViewTransition.TransitionTo(GameState.Bonus, GameState.Base);
        baseReelContainer.DisplayTheInvisibleSymbols(false);
        //恢复按钮状态
        UIController.BottomPanel.RestoreButtonDefultState();
        UIController.CheatPanel.UpdateFeatureBtnState(true);

        if (Cheat.isAutoPlay)
        {
            GlobalObserver.SetAutoPlay(true);
            UIController.BottomPanel.OnSpinBtnClick();
        }
    }

    public void TotalWinAction()
    {
        collectUI.collectStopEvent -= TotalWinAction;
        AudioManager.Stop("KGI_Game32_Bonus_Music");
        float winAudioTime = 0;
        if (GlobalObserver.TotalWin > GlobalObserver.BetValue * 32.5f)
        {
            AudioManager.PlayOneShot("KGI_Game32_Bonus_Totalwin2");
            winAudioTime = AudioManager.GetAudioTime("KGI_Game32_Bonus_Totalwin2");
        }
        else
        {
            AudioManager.PlayOneShot("KGI_Game32_Bonus_Totalwin1");
            winAudioTime = AudioManager.GetAudioTime("KGI_Game32_Bonus_Totalwin1");
        }
        UIController.BottomPanel.DisableAllButtons();
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        //result.WinMoney += bonusReelContainer.PayCoinCredit();
        //GlobalObserver.UpdateWin(result);
        totalWinUI.PlayTotalWin(/*result.TotalWin*/GlobalObserver.TotalWin);
        if (result.TotalWin > GlobalObserver.BetValue * 50)
        {
            DelayCallback.Delay(this, winAudioTime, () =>
            {
                totalWinUI.OnClickEvent += BonusGameToBaseGame;
                totalWinUI.OnMouseUpAsButton();
            });
        }
        else
        {
            DelayCallback.Delay(this, 3f, () =>
            {
                totalWinUI.OnClickEvent += BonusGameToBaseGame;
            });
            DelayCallback.Delay(this, winAudioTime, () =>
            {
                if (!totalWinUI.isClick)
                {
                    totalWinUI.OnMouseUpAsButton();
                }
            });
        }
    }
}
