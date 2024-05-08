using SlotGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Config;
using System.Linq;
using System;
using SlotGame.Result;
using UniversalModule.DelaySystem;
using UniversalModule.AudioSystem;
using DG.Tweening;
using Random = UnityEngine.Random;

public enum PrizedrawResult
{
    AddSpin,
    AddSpin2,
    Credit,
    Coin1,
    Coin2,
    Coin3,
    Wild,
    StartGame,
    Default
}

public class PrizedrawController : MonoBehaviour
{
    public Action<int> OnCoinClickEvent;
    public Action OnBonusIntroOverEvent;

    public GameObject introText;
    public GameObject message;
    public GameObject mask;
    public GameObject touchmeMessage;
    public GameObject[] transitionAnim;
    public BaseUI baseUI;
    private TurnSymbol[] prizeContainer;
    public List<PrizedrawResult> prizedrawResults = new List<PrizedrawResult>();
    private int currentSpin = 0;
    public int initLevel3CoinCount = 0;
    public bool isNoNextClick = false;
    Coroutine touchmeMessageWaiting;
    public CattleUI cattleUI;


    private Dictionary<int, PrizedrawResult> levelCoinDic = new Dictionary<int, PrizedrawResult>()
    {
        {1, PrizedrawResult.Coin1},
        {2, PrizedrawResult.Coin2},
        {3, PrizedrawResult.Coin3},
    };

    /// <summary>
    /// 初始化
    /// </summary>
    public void Initialize()
    {
        if (prizeContainer == null)
        {
            prizeContainer = GetComponentsInChildren<TurnSymbol>();
        }
        foreach (var item in prizeContainer)
        {
            item.OnTurnOverComplete += OnPickTriggerDown;
        }
    }

    public void InitBonus(int introWildCount,int addSpinCount)
    {
        DelayCallback.Delay(this, 53 / 30f, () =>
        {
            AudioManager.LoopPlayback("KGI_Game32_Bonus_Waiting");
        });
        prizeContainer[0].gameObject.SetActive(true);
        mask.SetActive(true);
        message.SetActive(true);
        currentSpin = 0;
        initLevel3CoinCount = 0;
        gameObject.SetActive(true);
        InitPrizedrawResult(introWildCount, addSpinCount);
        prizeContainer[0].PlayIntroAnim();
        introText.SetActive(true);
        message.SetActive(false);
        prizeContainer[0].ResetCoin();
        foreach (var symbol in prizeContainer)
        {
            symbol._symbolCollider.enabled = false;
        }
        foreach (var trans in transitionAnim)
        {
            trans.SetActive(false);
        }

        DelayCallback.Delay(this, 3f, () =>
        {
            message.SetActive(true);
            touchmeMessage.SetActive(true);
            introText.SetActive(false);
            foreach (var symbol in prizeContainer)
            {
                symbol._symbolCollider.enabled = true;
            }
            if (Cheat.isAutoPlay)
            {
                OnPickTriggerDown(prizeContainer[0]);
            }
        });

    }

    private void InitPrizedrawResult(int introWildCount,int addSpinCount)
    {
        int creditCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.BonusIntroCreditCountProbability);
        int extraSpinCount = addSpinCount/*GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.BonusIntroExtraSpinCountProbability)*/;
        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.BonusIntroCoinCountProbability[GlobalObserver.CoinPosLevel.Count]);
        int wildCount = /*ConfigManager.bonusConfig.BonusIntroCoinCount[GlobalObserver.CoinPosLevel.Count][index]*/introWildCount;

        //计算差值，判断差值，修改coinCount
        /*int newCoinCount = totalCoinCount - GlobalObserver.CoinPosLevel.Count - coinCount;
        if (newCoinCount >= 9)
        {
            coinCount += 2;
        }
        else if (newCoinCount >= 3)
        {
            coinCount++;
        }*/

        //int totalCount = creditCount + extraSpinCount + coinCount;
        prizedrawResults.Clear();
        for (int i = 0; i < creditCount; i++)
        {
            prizedrawResults.Add(PrizedrawResult.Credit);
        }
        int addSpin2Count = Random.Range(0, (extraSpinCount / 2) + 1);
        int addSpin1Count = extraSpinCount - addSpin2Count * 2;
        for (int i = 0; i < addSpin1Count; i++)
        {

            prizedrawResults.Add(PrizedrawResult.AddSpin);
        }
        for(int i = 0; i < addSpin2Count; i++)
        {
            prizedrawResults.Add(PrizedrawResult.AddSpin2);
        }
        for (int i = 0; i < wildCount; i++)
        {
            prizedrawResults.Add(PrizedrawResult.Wild);
        }

        prizedrawResults.ListSortRandom();

        prizedrawResults.Add(PrizedrawResult.StartGame);
        foreach (var prizedrawResult in prizedrawResults)
        {
            Debug.Log(prizedrawResult);
        }
    }


    public void OnPickTriggerDown(TurnSymbol item)
    {
        AudioManager.Playback("FE_BagPick");
        isNoNextClick = false;
        touchmeMessage.SetActive(false);
        foreach (var symbol in prizeContainer)
        {
            symbol._symbolCollider.enabled = false;
        }
        StartCoroutine(OnPickTigger(item));
    }

    IEnumerator OnPickTigger(TurnSymbol item)
    {
        item.PlaySelectAnim();
        bool isIntroOver = false;
        switch (prizedrawResults[currentSpin])
        {
            case PrizedrawResult.Credit:
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.BonusIntroCreditsProbability);
                int credit = ConfigManager.bonusConfig.BonusIntroCredits[index] * GlobalObserver.GetMultiplyer();
                yield return item.PlayCreditAnim(credit);
                PayoutInfo result = GlobalObserver.GetResult(GameState.Bonus);
                result.WinMoney = credit;
                GlobalObserver.UpdateWin(result);
                UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
                //yield return new WaitForSeconds(1.5f);
                item.ResetCoin();
                break;
            case PrizedrawResult.AddSpin:
                item.PlayAddSpinAnim(1);
                yield return new WaitForSeconds(18 / 30f);
                AudioManager.Playback("FE_BagPickSPIN");
                yield return new WaitForSeconds(14 / 30f);
                item.addSpinAnim.SetActive(false);
                item.PlayAddSpinFlyAnim();
                yield return new WaitForSeconds(17 / 30f);
                baseUI.AddSurplus(1);
                item.ResetCoin();
                break;
            case PrizedrawResult.AddSpin2:
                item.PlayAddSpinAnim(2);
                yield return new WaitForSeconds(18 / 30f);
                AudioManager.Playback("FE_BagPickSPIN");
                yield return new WaitForSeconds(14 / 30f);
                item.addSpinAnim.SetActive(false);
                item.PlayAddSpinFlyAnim();
                yield return new WaitForSeconds(17 / 30f);
                baseUI.AddSurplus(2);
                item.ResetCoin();
                break;
            case PrizedrawResult.Wild:
                item.PlayCoinAnim(1);
                yield return new WaitForSeconds(18 / 30f);
                AudioManager.Playback("FE_BagPickCoin");
                yield return new WaitForSeconds(22 / 30f);
                item.ResetCoin();
                OnCoinClickEvent.Invoke(1);
                yield return new WaitForSeconds(0.5f);
                break;
            case PrizedrawResult.StartGame:
                item.PlayStartAnim();
                yield return new WaitForSeconds(18 / 30f);
                AudioManager.Playback("FE_BagPickStart");
                yield return new WaitForSeconds(14 / 30f);
                yield return new WaitForSeconds(45 / 30f);
                AudioManager.Playback("FE_BonusTurn");
                foreach (var trans in transitionAnim)
                {
                    trans.SetActive(true);
                }
                item.TransitionAnim();
                yield return new WaitForSeconds(25 / 30f);
                item.gameObject.SetActive(false);
                mask.SetActive(false);
                message.SetActive(false);
                cattleUI.BonusIntroOver();
                yield return new WaitForSeconds(15 / 30f);

                isIntroOver = true;
                AudioManager.Stop("KGI_Game32_Bonus_Waiting");
                AudioManager.LoopPlayback("KGI_Game32_Bonus_Music");
                OnBonusIntroOverEvent.Invoke();
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
        currentSpin++;
        foreach (var symbol in prizeContainer)
        {
            symbol._symbolCollider.enabled = true;
        }

        isNoNextClick = true;
        if (touchmeMessageWaiting != null)
        {
            StopCoroutine(touchmeMessageWaiting);
        }
        if (isActiveAndEnabled)
        {
            touchmeMessageWaiting = StartCoroutine(PlayTouchMeMessage());
        }
        if (Cheat.isAutoPlay && !isIntroOver)
        {
            OnPickTriggerDown(prizeContainer[0]);
        }
    }

    IEnumerator PlayTouchMeMessage()
    {
        yield return new WaitForSeconds(4);
        if (isNoNextClick)
        {
            touchmeMessage.SetActive(true);
        }
    }

    private void Update()
    {

    }



}
