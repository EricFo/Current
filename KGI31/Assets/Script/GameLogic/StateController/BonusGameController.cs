using DG.Tweening;
using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Result;
using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using UniversalModule.SpawnSystem;

public class BonusGameController : StateProcessBase
{

    [Tooltip("剩余次数")] public BaseUI baseUI;
    [Tooltip("收集动画")] public CollectUI collectUI;
    public TotalWinUI totalWinUI;
    public BaseReelContainer baseReelContainer;
    public BonusReelContainer bonusReelContainer;
    private PayoutInfo result = null;

    public Animator multiplierAnim;
    public Animator ToComboAnim;
    public SpriteRenderer ToComboMessage;
    public Sprite[] FreeMessage;

    public GameObject Message;

    public GameObject Add3SpinAnim;

    //IAudioPlayer Winloop;
    bool isWinloopAudio = false;

    public override GameState GameState
    {
        get { return GameState.Bonus; }
    }

    void Awake()
    {
        bonusReelContainer.upgradeGamePlay.OnWinAnimComplete += AwardCoroutineComplete;
        bonusReelContainer.collectGamePlay.OnWinAnimComplete += AwardCoroutineComplete;
    }

    public override bool OnStateEnter()
    {
        var machine = GetStateMachine(GameState);
        if (baseUI.Surplus > 0)
        {
            baseUI.Spin();
            UIController.BottomPanel.UpdatePressSpinBtnState();
            UIController.BottomPanel.StopBtn.interactable = false;
            GlobalObserver.OnAbortDeduce(machine.PayMode);
            GlobalObserver.DonePayout = false;
            if (multiplierAnim.isActiveAndEnabled)
            {
                multiplierAnim.Play("Idle");
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStateExit(PayoutInfo result)
    {
        //baseUI.AddSurplus();
        //this.result=GetStateMachine(GameState).ReportTheResult();
        WinMoneyAndDeduce();
    }

    public void WinMoneyAndDeduce()
    {
        result = OnDeduceState();
        if (result.WinMoney > 0)
        {
            AudioManager.LoopPlayback("KGI29-Winloop");
            isWinloopAudio = true;
            UIController.BottomPanel.OnAwardComplateEvent += AwardCoroutineComplete;
            UIController.BottomPanel.AwardRollUp(result.WinMoney);
            if (multiplierAnim.isActiveAndEnabled)
            {
                if (GlobalObserver.PayMultiplier > 9)
                {
                    multiplierAnim.Play(/*GlobalObserver.CurrentBonusState == 3 ? "Payout3" : */"Payout2");
                }
                else
                {
                    multiplierAnim.Play(/*GlobalObserver.CurrentBonusState == 3 ? "Payout5" : */"Payout2");
                }
            }
        }
        else
        {
            GlobalObserver.DonePayout = true;
            AwardCoroutineComplete();
        }
    }

    /// <summary>
    /// 处理奖励结束时的逻辑
    /// </summary>
    public void AwardCoroutineComplete()
    {
        if (isWinloopAudio && GlobalObserver.DonePayout)
        {
            AudioManager.Stop("KGI29-Winloop");
            isWinloopAudio = false;
            AudioManager.PlayOneShot("KGI29-Winloopend");
        }

        if ((!GlobalObserver.DonePayout || !GlobalObserver.DoneWinAnim) && bonusReelContainer.isFreeToComboCurrSpin)
        {
            return;
        }
        //取消事件注册，防止在不需要的情况下执行
        UIController.BottomPanel.OnAwardComplateEvent -= AwardCoroutineComplete;

        StartCoroutine(UpdateGamePlay());
    }

    public IEnumerator UpdateGamePlay()
    {
        bool isUpdate = false;
        if (bonusReelContainer.isFreeToComboCurrSpin)
        {
            isUpdate = true;
            AudioManager.Playback("FeatureSelect");
            Message.SetActive(false);
            ScatterSymbol scatter = SpawnFactory.GetObject<ScatterSymbol>(GlobalObserver.IsUpgradePlay ? SymbolNames.SCATTERCOL : SymbolNames.SCATTERUP);
            scatter.SetMaskMode(SpriteMaskInteraction.None);
            scatter.SetSortingOrder(2500);
            scatter.transform.position = new Vector3(-0.06f, -8.28f);
            scatter.transform.localScale = Vector3.zero;
            scatter.transform.DOScale(Vector3.one * 1.555f, 8 / 30f);
            scatter.PlayWinAnim();
            ToComboMessage.sprite = FreeMessage[GlobalObserver.IsUpgradePlay ? 1 : 0];
            ToComboAnim.Play("Display");

            //bonusReelContainer.turntableUI.PlayChangeStarAnimation(GlobalObserver.CurrentBonusState==1);
            yield return new WaitForSeconds(2f);
            Add3SpinAnim.SetActive(true);
            AudioManager.Playback("FE_+3SpinsFlyup");
            yield return new WaitForSeconds(23 / 30f);
            baseUI.PlayResetAnim();
            baseUI.AddSurplus(3);
            yield return new WaitForSeconds(10 / 30f);

            yield return new WaitForSeconds(1f);
            ToComboAnim.Play("Hide");

            bonusReelContainer.turntableUI.UpdateGamePlayTex();
            bonusReelContainer.UpdateGamePlay();
            bonusReelContainer.isFreeToComboCurrSpin = false;
            Message.SetActive(true);
            Add3SpinAnim.SetActive(false);
            scatter.Recycle();
            bonusReelContainer.scatterWin.Recycle();

            yield return new WaitForSeconds(0.3f);
            baseUI.transform.DOLocalMove(new Vector3(0, 5.9f), 17 / 30f);
            baseUI.transform.DOScale(Vector3.one, 17 / 30f);
        }

        //这里有一个Bonus模式是否结束的判断
        GlobalObserver.IsBonusGameIsOver = CheckProcess.CheckBonusGameIsOver(baseUI.CurrSurplus, baseUI.Surplus);
        //Debug.LogWarning(GlobalObserver.IsBonusGameIsOver);
        if (GlobalObserver.IsBonusGameIsOver)
        {
            GetComponent<BonusReelContainer>().CurrentActive = -1;
            UIController.BottomPanel.DisableAllButtons();
            //result.WinMoney = bonusReelContainer.TotalScore();
            GlobalObserver.UpdateWin(result);
            BonusGameIsOver();
        }
        else
        {
            if (isUpdate)
            {
                yield return new WaitForSeconds(0.6f);
            }
            UIController.BottomPanel.OnSpinBtnClick();
            //UIController.BottomPanel.DisplaySpinBtn();
        }
    }

    /// <summary>
    /// 处理Bonus游戏结束相关的状态
    /// </summary>
    private void BonusGameIsOver()
    {
        TotalWinAction();
    }


    /// <summary>
    /// BonusGame切换到BaseGame
    /// </summary>
    private void BonusGameToBaseGame()
    {
        //清除最后一次的赔付列表
        GlobalObserver.OnAbortDeduce(GetStateMachine(GameState).PayMode);

        totalWinUI.TotalWin.SetActive(false);
        totalWinUI.OnClickAction -= BonusGameToBaseGame;
        for (int i = 1; i <= 3; i++)
        {
            AudioManager.Stop(GlobalObserver.BonusMusic[i]);
        }
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
        turntableUpgrade.SlowUniForm();
        turntableCollect.SlowUniForm();
        Cheat.isGrand = false;
        Cheat.isMajor = false;
        Cheat.isMini = false;
        Cheat.isFree1ToFree3 = false;

        //自动测试部分
        if (Cheat.isAutoPlay)
        {
            GlobalObserver.SetAutoPlay(true);
            UIController.BottomPanel.OnSpinBtnClick();
        }
    }

    public void TotalWinAction()
    {
        UIController.BottomPanel.DisableAllButtons();
        for(int i = 1; i <= 3; i++)
        {
            AudioManager.Stop(GlobalObserver.BonusMusic[i]);
        }
        float winAudioTime = 0;
        if (GlobalObserver.TotalWin > GlobalObserver.BetValue * 32.5f)
        {
            AudioManager.PlayOneShot("KGI-Game29-Bonus-Totalwin2");
            winAudioTime = AudioManager.GetAudioTime("KGI-Game29-Bonus-Totalwin2");
        }
        else
        {
            AudioManager.PlayOneShot("KGI-Game29-Bonus-Totalwin1");
            winAudioTime = AudioManager.GetAudioTime("KGI-Game29-Bonus-Totalwin1");
        }
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        totalWinUI.PlayTotalWin(/*result.TotalWin*/GlobalObserver.TotalWin);
        DelayCallback.Delay(this, 1f, () =>
        {
            totalWinUI.OnClickEvent += BonusGameToBaseGame;
        });
        DelayCallback.Delay(this, winAudioTime, () =>
        {
            if (!totalWinUI.isClick)
            {
                BonusGameToBaseGame();
            }
        });
    }

    /// <summary>
    /// 赔付
    /// </summary>
    protected PayoutInfo OnDeduceState()
    {
        IStateMachine machine = GetStateMachine(GameState.Bonus);
        PayoutInfo result = machine.ReportTheResult();
        GlobalObserver.UpdateResult(result);
        GlobalObserver.OnEvaluate(machine.PayMode);
        GlobalObserver.OnDeduce(machine.PayMode);
        return result;
    }

}
