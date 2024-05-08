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
using UniversalModule.SpawnSystem;

public class BonusGameController : StateProcessBase
{
    public CollectUI collectUI;
    public TotalWinUI totalWinUI;
    public BaseReelContainer baseReelContainer;
    public BonusReelContainer bonusReelContainer;
    public BaseUI baseUI;
    //public GameObject[] transitionAnim;
    private PayoutInfo result = null;
    public JackPot JackPot;
    public GameObject bigCreditHitAnim;

    bool isWinloopAudio = false;

    public override GameState GameState
    {
        get { return GameState.Bonus; }
    }

    public override bool OnStateEnter()
    {
        var machine = GetStateMachine(GameState);
        if (baseUI.CheckRemaining() > 0)
        {
            baseUI.Spin();
            UIController.BottomPanel.UpdatePressSpinBtnState();
            //UIController.BottomPanel.StopBtn.interactable = false;
            GlobalObserver.OnAbortDeduce(machine.PayMode);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnStateExit(PayoutInfo result)
    {
        //this.result = GetStateMachine(GameState).ReportTheResult();
        StartCoroutine(_PayJackpot());
    }

    IEnumerator _PayJackpot()
    {
        List<CoinSymbol> coinSymbols = bonusReelContainer.coinSymbols;
        if (coinSymbols.Count > 0)
        {
            PayoutInfo info = GlobalObserver.GetResult(GameState.Bonus);
            int totalCoinWin = 0;
            foreach (var item in coinSymbols)
            {
                if (item.JackpotType is not JackpotType.Grand and not JackpotType.Major)
                {
                    totalCoinWin += item.ScoreValue;
                }
            }
            yield return new WaitForSeconds(1f);
            (bonusReelContainer.Reels[7].GetVisibleSymbols()[0] as ScatterSymbol).PlayWinAnim();

            yield return JackPot.PlayJackpotCeleBase(coinSymbols);

            info.WinMoney = totalCoinWin;
            GlobalObserver.UpdateWin(info);

            float flyIntervalTime = 30 / 30f;
            bool isRollUp = true;
            bool isNormalSymbol = false;
            UIController.BottomPanel.OnStopBtnClickEvent += FlyShutDown;
            foreach (var item in coinSymbols)
            {
                if (item.JackpotType is not JackpotType.Grand and not JackpotType.Major)
                {
                    isNormalSymbol = true;
                }
            }
            foreach (var item in coinSymbols)
            {
                if (item.JackpotType is not JackpotType.Grand and not JackpotType.Major)
                {
                    yield return new WaitForSeconds(flyIntervalTime);
                    if (item.Level == 3)
                    {
                        AudioManager.Playback("FE_CoinCreditCollect_b");
                    }
                    else
                    {
                        AudioManager.Playback("FE_CoinCreditCollect_a（Base）");
                    }
                    //item.PlayIdleAnim();
                    item.PlayGlowAnim(false);
                    item.UpSortingOrder();
                    FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                    flySymbol.SetSortingOrder(11000);
                    flySymbol.PlayJPCoinFlyAnim(item.JackpotType, item.transform.position, new Vector3(0, -8.3f));
                    DelayCallback.Delay(this, 14 / 30f, () =>
                    {
                        if (isRollUp)
                        {
                            isRollUp = false;
                            isWinloopAudio = true;
                            AudioManager.LoopPlayback("AwardLoop");
                            UIController.BottomPanel.OnAwardComplateEvent += WinMoneyAndDeduce;
                            UIController.BottomPanel.AwardRollUp(/*GlobalObserver.TotalWin*/info.WinMoney);
                        }
                        bigCreditHitAnim.SetActive(false);
                        bigCreditHitAnim.SetActive(true);
                    });
                }
            }
            UIController.BottomPanel.OnStopBtnClickEvent -= FlyShutDown;
            UIController.BottomPanel.StopBtn.interactable = true;
            if (!isNormalSymbol)
            {
                WinMoneyAndDeduce();
            }

            void FlyShutDown()
            {
                flyIntervalTime = 0;
            }
        }
        else
        {
            WinMoneyAndDeduce();
        }
    }

    public void WinMoneyAndDeduce()
    {
        if (isWinloopAudio)
        {
            AudioManager.Stop("AwardLoop");
            isWinloopAudio = false;
            AudioManager.PlayOneShot("AwardEnd");
        }
        JackPot.DisableAllMeterAnim();
        bonusReelContainer.Reels[7].GetVisibleSymbols()[0].PlayIdleAnim();
        UIController.BottomPanel.OnAwardComplateEvent -= WinMoneyAndDeduce;

        foreach(var coinSymbol in bonusReelContainer.coinSymbols)
        {
            coinSymbol.JPAnim.Play("Idle");
            coinSymbol.PlayIdleAnim();
            coinSymbol.UpSortingOrder();
        }
        StartCoroutine(_WinMoneyAndDeduce());
    }

    IEnumerator _WinMoneyAndDeduce()
    {
        yield return null;

        result = OnDeduceState();
        if (result.WinMoney > 0)
        {
            AudioManager.LoopPlayback("AwardLoop");
            isWinloopAudio = true;
            UIController.BottomPanel.OnAwardComplateEvent += AwardCoroutineComplete;
            UIController.BottomPanel.AwardRollUp(result.WinMoney);
            UIController.BottomPanel.StopBtn.interactable = true;
        }
        else
        {
            AwardCoroutineComplete();
        }
    }

    /// <summary>
    /// 处理奖励结束时的逻辑
    /// </summary>
    public void AwardCoroutineComplete()
    {
        if (isWinloopAudio)
        {
            AudioManager.Stop("AwardLoop");
            isWinloopAudio = false;
            AudioManager.PlayOneShot("AwardEnd");
        }
        //取消事件注册，防止在不需要的情况下执行
        UIController.BottomPanel.OnAwardComplateEvent -= AwardCoroutineComplete;
        //这里有一个Bonus模式是否结束的判断
        GlobalObserver.IsBonusGameIsOver = CheckProcess.CheckBonusGameIsOver(baseUI.CheckRemaining());
        if (GlobalObserver.IsBonusGameIsOver)
        {
            UIController.BottomPanel.DisableAllButtons();
            BonusGameIsOver();
        }
        else
        {
            if (Cheat.isAutoFree)
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
        //baseUI.OnBonusIsOver();
        /*List<BonusCoinSymbol> bonusCoinSymbols = new List<BonusCoinSymbol>();
        bonusReelContainer.exisCoinViews.ListSorting();
        foreach (var item in bonusReelContainer.exisCoinViews)
        {
            bonusCoinSymbols.Add(bonusReelContainer.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol);
        }
        List<BonusCoinSymbol> sortBonusCoinSymbols = new List<BonusCoinSymbol>();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int index = i + j * 5;
                var symbol = bonusReelContainer.Reels[index].GetVisibleSymbols()[0];
                if (symbol.ItemName == SymbolNames.BCOIN)
                {
                    sortBonusCoinSymbols.Add(symbol as BonusCoinSymbol);
                }
            }
        }*/

        //StartCoroutine(collectUI.CoinSymbolCollect(sortBonusCoinSymbols, result));
        //collectUI.collectStopEvent += TotalWinAction;
        TotalWinAction();
    }

    /// <summary>
    /// BonusGame切换到BaseGame
    /// </summary>
    private void BonusGameToBaseGame()
    {
        //清除最后一次的赔付列表
        GlobalObserver.OnAbortDeduce(GetStateMachine(GameState).PayMode);

        bigCreditHitAnim.SetActive(false);
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
    /*IEnumerator _BonusGameToBaseGame()
    {

    }*/

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
        //bonusReelContainer.cattleUI.DisplayBigCattle(false);
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
