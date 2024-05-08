using SlotGame.Core;
using SlotGame.Result;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using UniversalModule.SpawnSystem;

public class BaseGameController : StateProcessBase
{
    public JackPot JackPot;
    public BaseReelContainer baseReelContainer;
    PayoutInfo result = null;

    public GameObject bigCreditHitAnim;
    public GameObject CreditHitAnim;

    bool isWinloopAudio = false;

    public override GameState GameState
    {
        get
        {
            return GameState.Base;
        }
    }

    public override bool OnStateEnter()
    {
        var machine = GetStateMachine(GameState);
        if (GlobalObserver.CurrCredit >= GlobalObserver.BetValue)
        {
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
        }
        else
        {
            UIController.BottomPanel.DisableAllButtons();
            return false;
        }
    }

    public override void OnStateExit(PayoutInfo result)
    {
        GlobalObserver.IsTriggerJackpot = CheckProcess.CheckJackPotIsTrigger(result);
        GlobalObserver.IsTriggerFreeGame = CheckProcess.CheckFreeGameIsTrigger(result);



        StartCoroutine(PayCoinCredit());
    }

    IEnumerator PayCoinCredit()
    {
        if (baseReelContainer.isScatter)
        {
            yield return new WaitForSeconds(26 / 30f);
        }
        if (GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame)
        {
            /*float triggerAnimTime = 0;
            triggerAnimTime = baseReelContainer.cattleUI.PlayTriggerAnim();
            yield return new WaitForSeconds(triggerAnimTime);*/
            yield return new WaitForSeconds(baseReelContainer.triggerAnimTime);
        }

        if (baseReelContainer.isPayCoin)
        {
            if (baseReelContainer.isUpgrade)
            {
                yield return new WaitForSeconds(60 / 30f);
            }
            else
            {
                yield return new WaitForSeconds(15 / 30f);
            }
            List<CoinSymbol> coinSymbols = new List<CoinSymbol>();
            foreach (var item in baseReelContainer.Reels)
            {
                if (item.GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
                {
                    CoinSymbol coinSymbol = (CoinSymbol)item.GetVisibleSymbols()[0];
                    coinSymbols.Add(coinSymbol);
                }
            }
            PayoutInfo info = GlobalObserver.GetResult(GameState.Base);
            int totalCoinWin = 0;
            int totalCoinJpWin = 0;
            foreach (var item in coinSymbols)
            {
                if (item.JackpotType is not JackpotType.Grand and not JackpotType.Major)
                {
                    totalCoinWin += item.ScoreValue;
                }
                totalCoinJpWin += item.ScoreValue;
            }
            (baseReelContainer.Reels[7].GetVisibleSymbols()[0] as ScatterSymbol).PlayUpgradeAnim(totalCoinJpWin > GlobalObserver.BetValue * 3);
            yield return new WaitForSeconds(15 / 30f);
            foreach (var item in coinSymbols)
            {
                item.PlayWinAnim();
            }
            yield return new WaitForSeconds(1f);
            (baseReelContainer.Reels[7].GetVisibleSymbols()[0] as ScatterSymbol).PlayWinAnim();

            yield return JackPot.PlayJackpotCeleBase(coinSymbols);

            info.WinMoney = totalCoinWin;
            GlobalObserver.UpdateWin(info);


            float flyIntervalTime = 30 / 30f;
            bool isRollUp = true;
            bool isNormalSymbol = false;
            //float flyIntervalTime2 = 14 / 30f;
            //UIController.BottomPanel.StopBtn.interactable = true;
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
                /*if (item.JackpotType != JackpotType.Null)
                {
                    yield return JackPot.PlayJackpotCele(item);
                }*/
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
                    flySymbol.PlayCoinUpAnim(item.Level, item.transform.position, new Vector3(0, -8.3f));
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
                        if (item.Level == 3)
                        {
                            bigCreditHitAnim.SetActive(false);
                            bigCreditHitAnim.SetActive(true);
                        }
                        else
                        {
                            CreditHitAnim.SetActive(false);
                            CreditHitAnim.SetActive(true);
                        }
                    });
                }
                /*DelayCallback.Delay(this, 14 / 30f, () =>
                {
                    info.WinMoney = item.ScoreValue;
                    GlobalObserver.UpdateWin(info);
                    UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
                });*/
                //yield return new WaitForSeconds(flyIntervalTime2);

            }
            UIController.BottomPanel.OnStopBtnClickEvent -= FlyShutDown;
            UIController.BottomPanel.StopBtn.interactable = true;
            if (!isNormalSymbol)
            {
                WinMoneyAndDeduce();
            }

            //yield return new WaitForSeconds(15/30f);
            //CreditHitAnim.SetActive(false);
            //bigCreditHitAnim.SetActive(false);

            /*info.WinMoney += totalCoinWin;
            GlobalObserver.UpdateWin(info);
            //yield return new WaitForSeconds(25 / 30f);
            //WinMoneyAndDeduce();
            isWinloopAudio = true;
            UIController.BottomPanel.OnAwardComplateEvent += WinMoneyAndDeduce;
            UIController.BottomPanel.AwardRollUp(GlobalObserver.TotalWin);*/
            void FlyShutDown()
            {
                flyIntervalTime = 0;
            }

        }
        else
        {
            WinMoneyAndDeduce();
        }


        //WinMoneyAndDeduce();
    }

    /// <summary>
    /// 结算
    /// </summary>
    public void WinMoneyAndDeduce()
    {
        if (isWinloopAudio)
        {
            AudioManager.Stop("AwardLoop");
            isWinloopAudio = false;
            AudioManager.PlayOneShot("AwardEnd");
        }
        baseReelContainer.Reels[7].GetVisibleSymbols()[0].PlayIdleAnim();
        UIController.BottomPanel.OnAwardComplateEvent -= WinMoneyAndDeduce;

        foreach (var item in baseReelContainer.Reels)
        {
            if (item.GetVisibleSymbols()[0].ItemName == SymbolNames.COIN)
            {
                CoinSymbol coinSymbol = (CoinSymbol)item.GetVisibleSymbols()[0];
                coinSymbol.JPAnim.Play("Idle");
                coinSymbol.PlayIdleAnim();
                coinSymbol.UpSortingOrder();
            }
        }
        StartCoroutine(_WinMoneyAndDeduce());
    }
    IEnumerator _WinMoneyAndDeduce()
    {
        //yield return new WaitForSeconds(0.01f);
        yield return null;
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
    private void OnBaseGameAwardComplate()
    {
        if (isWinloopAudio)
        {
            AudioManager.Stop("AwardLoop");
            isWinloopAudio = false;
            AudioManager.PlayOneShot("AwardEnd");
        }
        //取消事件注册，防止在不需要的情况下执行
        UIController.BottomPanel.OnAwardComplateEvent -= OnBaseGameAwardComplate;
        PayoutInfo result = GlobalObserver.GetResult(GameState.Base);
        //没触发JackPot要检查是否触发Bonus，触发JackPot的情况下先进JackPot
        if (GlobalObserver.IsTriggerBonusGame || Cheat.isTriggerBonusGame)
        {
            //处理触发Free的情况
            //记录下初始COIN的位置和等级
            baseReelContainer.RecordCoinPosLevel();

            //To Do 此处应该处理FreeGame弹框逻辑，弹框结束后应该是转场
            /*var popup = GetPopupBox("BONUSGAME");
            //此处事件已经做了防止重复注册的处理，不需要再手动注销
            popup.OnClickEvent += BaseGameToBonusGame;
            popup.Show(result, 1f, Cheat.isAutoPlay);*/
            StartCoroutine(_BaseGameToBonusGame());

            //触发任何特殊玩法都要禁用掉Auto Play
            GlobalObserver.SetAutoPlay(false);
        }
        else
        { //没触发Free或JP就恢复按钮状态
            GlobalObserver.UpdateCredit(GlobalObserver.TotalWin);
            UIController.BottomPanel.RestoreButtonDefultState();
            UIController.CheatPanel.UpdateFeatureBtnState(true);
            //UIController.BottomPanel.UpdateCredit(GlobalObserver.CurrCredit);
            GlobalObserver.SetFinalizeState();
            //如果启用AutoSpin就直接继续
            if (GlobalObserver.AutoSpinEnabled == true)
            {
                UIController.BottomPanel.OnSpinBtnClick();
            }
        }
    }

    IEnumerator _BaseGameToBonusGame()
    {
        AudioManager.Playback("FeatureSelect");
        yield return new WaitForSeconds(2.5f);
        AudioManager.Stop("FE_BagTrigger");
        AudioManager.Playback("FE_BonusBagIntro");
        AudioManager.PlayOneShot("KGI_Game32_Bonus_Intro");
        baseReelContainer.cattleUI.PlayPurseIntroAnim();
        yield return new WaitForSeconds(7 / 30f);
        BaseGameToBonusGame();
    }

    private void BaseGameToBonusGame()
    {
        CreditHitAnim.SetActive(false);
        bigCreditHitAnim.SetActive(false);
        baseReelContainer.cowHint.SetActive(false);

        baseReelContainer.cattleUI.upperTrigger.Stop();
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
