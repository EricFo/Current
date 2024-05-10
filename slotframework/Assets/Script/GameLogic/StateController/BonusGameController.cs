using SlotGame.Core;
using SlotGame.Result;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class BonusGameController : StateProcessBase
{
    public SurplusUI surplusUI;
    public BaseReelContainer baseReelContainer;
    private PayoutInfo result = null;

    public override GameState GameState
    {
        get { return GameState.Bonus; }
    }

    public override bool OnStateEnter()
    {
        var machine = GetStateMachine(GameState);
        if (surplusUI.CheckRemaining() > 0)
        {
            surplusUI.EditSurplus(1);
            UIController.BottomPanel.UpdatePressSpinBtnState();
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
    /// ����������ʱ���߼�
    /// </summary>
    public void AwardCoroutineComplete()
    {
        //ȡ���¼�ע�ᣬ��ֹ�ڲ���Ҫ�������ִ��
        UIController.BottomPanel.OnAwardComplateEvent -= AwardCoroutineComplete;
        //������һ��Bonusģʽ�Ƿ�������ж�
        GlobalObserver.IsBonusGameIsOver = CheckProcess.CheckBonusGameIsOver(surplusUI.CheckRemaining());
        if (GlobalObserver.IsBonusGameIsOver)
        {
            UIController.BottomPanel.DisableAllButtons();
            BonusGameIsOver();
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
    /// ����Bonus��Ϸ������ص�״̬
    /// </summary>
    private void BonusGameIsOver()
    {
        GlobalObserver.UpdateWin(result);
        TotalWinAction();
    }

    public void TotalWinAction()
    {
        //AudioManager.Stop("Bonus_Music");
        float winAudioTime = 10f;
        if (GlobalObserver.TotalWin > GlobalObserver.BetValue * 32.5f)
        {
            //AudioManager.PlayOneShot("");
            //winAudioTime = AudioManager.GetAudioTime("");
        }
        else
        {
            //AudioManager.PlayOneShot("");
            //winAudioTime = AudioManager.GetAudioTime("");
        }
        UIController.BottomPanel.DisableAllButtons();
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        TotalWin totalWin=GetPopupBox("TotalWin") as TotalWin;
        totalWin.SetTotalWin(GlobalObserver.TotalWin);
        totalWin.OnClickEvent += BonusGameToBaseGame;
        if (GlobalObserver.TotalWin > GlobalObserver.BetValue * 50)
        {
            totalWin.Show(winAudioTime,winAudioTime);
        }
        else
        {
            totalWin.Show(3f,winAudioTime);
        }
    }

    /// <summary>
    /// BonusGame�л���BaseGame
    /// </summary>
    private void BonusGameToBaseGame()
    {
        //AudioManager.Stop("KGI_Game32_Bonus_Totalwin2");
        //AudioManager.Stop("KGI_Game32_Bonus_Totalwin1");
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        GlobalObserver.UpdateCredit(GlobalObserver.TotalWin);
        //������Ϸ״̬
        GlobalObserver.UpdateGameState(GameState.Base);
        //����UIת��
        ViewTransition.TransitionTo(GameState.Bonus, GameState.Base);
        baseReelContainer.DisplayTheInvisibleSymbols(false);
        //�ָ���ť״̬
        UIController.BottomPanel.RestoreButtonDefultState();
        UIController.CheatPanel.UpdateFeatureBtnState(true);

        if (Cheat.isAutoPlay)
        {
            GlobalObserver.SetAutoPlay(true);
            UIController.BottomPanel.OnSpinBtnClick();
        }
    }

}
