using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniversalModule.DelaySystem;
using SlotGame.Core;
using DG.Tweening;
using UniversalModule.AudioSystem;
using System.ComponentModel;
using Spine.Unity;
using SlotGame.Config;
using SlotGame.Result;
using UniversalModule.SpawnSystem;
using Random = UnityEngine.Random;
using System.Linq;
using static UnityEngine.Networking.UnityWebRequest;

public class CollectUI : MonoBehaviour
{
    public JackPot JackPot;
    public Camera mainCamera;
    //[Tooltip("收集动画")] public GameObject[] Flys;
    [Tooltip("收集ToTalWin窗口")] public GameObject TotalWin;
    [Tooltip("收集ToTalWin窗口Hit动画")] public GameObject TotalWin_Hit;
    [Tooltip("收集窗口金币")] public ArtText Text;

    /// <summary>
    /// 收集完全部coin后调用的事件
    /// </summary>
    public Action collectStopEvent;

    /// <summary>
    /// 当前收集的所有coin上的分数
    /// </summary>
    private int collectValue;

    /// <summary>
    /// 本轮所有参与上飞的FlyId
    /// </summary>
    private Queue<Dictionary<int, BonusCoinSymbol>> FlyIds = new Queue<Dictionary<int, BonusCoinSymbol>>();
    /// <summary>
    /// 本轮所有参与上飞的分数
    /// </summary>
    private Queue<int> FlyIdsValue = new Queue<int>();

    /// <summary>
    /// 防止Hit还没播完就播下次
    /// </summary>
    private bool isHit = false;

    public GameObject[] CreditIntroAnim;
    public GameObject CreditHitAnim;
    public GameObject bigCreditHitAnim;

    public CattleUI cattleUI;

    public IEnumerator CoinSymbolCollect(List<BonusCoinSymbol> bonusCoinSymbols, PayoutInfo result)
    {
        yield return new WaitForSeconds(0.5f);
        AudioManager.Playback("FE_SpecialBigAppear");
        cattleUI.DisplayBigCattle(true);
        yield return new WaitForSeconds(1f);
        /*Dictionary<int, List<BonusCoinSymbol>> levelCoin = new Dictionary<int, List<BonusCoinSymbol>>()
        {
            { 1, new List<BonusCoinSymbol>() },{ 2, new List<BonusCoinSymbol>() },{ 3, new List<BonusCoinSymbol>() }
        };
        foreach (var bsymbol in bonusCoinSymbols)
        {
            levelCoin[bsymbol.Level].Add(bsymbol);
        }

        if (levelCoin[2].Count > 0 && levelCoin[1].Count > 0)
        {
            foreach (var collectSymbol in levelCoin[2])
            {
                AudioManager.Playback("FE_EndCoinCollectActive");
                collectSymbol.UpCollectOrder();
                collectSymbol.PlayCollectAnim(true);
                foreach (var beCollect in levelCoin[1])
                {
                    beCollect.DisplayGlow(true);
                }
                yield return new WaitForSeconds(1.3f);
                foreach (var beCollect in levelCoin[1])
                {
                    AudioManager.Playback("FE_EndCoinCollect_a");
                    FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                    flySymbol.SetSortingOrder(11000);
                    float flyTime= flySymbol.PlayCollectFlyAnim(beCollect.Level, beCollect.transform.position, collectSymbol.transform.position);
                    DelayCallback.Delay(this, flyTime, () =>
                    {
                        collectSymbol.SetScore(collectSymbol.ScoreValue += beCollect.ScoreValue);
                    });
                    yield return new WaitForSeconds(10 / 30f);
                    beCollect.DisplayGlow(false);
                }
                yield return new WaitForSeconds(15 / 30f);
                collectSymbol.PlayCollectAnim(false);
                collectSymbol.UpSortingOrder();
                yield return new WaitForSeconds(20 / 30f);
            }
        }

        if (levelCoin[3].Count > 0)
        {
            foreach (var collectSymbol in levelCoin[3])
            {
                AudioManager.Playback("FE_EndCoinCollectActive");
                collectSymbol.UpCollectOrder();
                collectSymbol.PlayCollectAnim(true);
                foreach (var beCollect in bonusCoinSymbols)
                {
                    if (beCollect != collectSymbol)
                    {
                        beCollect.DisplayGlow(true);
                    }
                }
                yield return new WaitForSeconds(1.5f);
                foreach (var beCollect in bonusCoinSymbols)
                {
                    if (beCollect.Level != 3)
                    {
                        yield return new WaitForSeconds(15 / 30f);
                        if (beCollect.Level == 1)
                        {
                            AudioManager.Playback("FE_EndCoinCollect_b");
                        }
                        else
                        {
                            AudioManager.Playback("FE_EndCoinCollect_c");
                        }
                        FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                        flySymbol.SetSortingOrder(11000);
                        float flyTime= flySymbol.PlayCollectFlyAnim(beCollect.Level, beCollect.transform.position, collectSymbol.transform.position);
                        DelayCallback.Delay(this, flyTime, () =>
                        {
                            collectSymbol.SetScore(collectSymbol.ScoreValue += beCollect.ScoreValue);
                        });
                        yield return new WaitForSeconds(5 / 30f);
                        beCollect.DisplayGlow(false);
                        yield return new WaitForSeconds(10 / 30f);
                    }
                }
                foreach (var beCollect in bonusCoinSymbols)
                {
                    if (beCollect.Level == 3 && beCollect != collectSymbol)
                    {
                        AudioManager.Playback("FE_EndCoinCollect_c");
                        FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                        flySymbol.SetSortingOrder(11000);
                        flySymbol.PlayCollectFlyAnim(beCollect.Level, beCollect.transform.position, collectSymbol.transform.position);
                        yield return new WaitForSeconds(5 / 30f);
                        beCollect.DisplayGlow(false);
                        yield return new WaitForSeconds(43 / 30f);
                        collectSymbol.SetScore(collectSymbol.ScoreValue += beCollect.ScoreValue);
                        yield return new WaitForSeconds(30 / 30f);
                    }
                }
                collectSymbol.PlayCollectAnim(false);
                collectSymbol.UpSortingOrder();
            }
        }

        yield return new WaitForSeconds(0.5f);*/

        AudioManager.Playback("FE_EndCoinCollectActive");
        foreach (var item in bonusCoinSymbols)
        {
            item.PlayBecollectAnim(true);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < bonusCoinSymbols.Count; i++)
        {
            if (bonusCoinSymbols[i].Level == 3)
            {
                yield return new WaitForSeconds(6 / 30f);
                AudioManager.Playback("FE_CoinCreditCollect_b");
            }
            else
            {
                yield return new WaitForSeconds(6 / 30f);
                AudioManager.Playback("FE_CoinCreditCollect_a（Base）");
            }
            bonusCoinSymbols[i].FlyChange();
            FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
            flySymbol.SetSortingOrder(11000);
            flySymbol.PlayCoinUpAnim(bonusCoinSymbols[i].Level, bonusCoinSymbols[i].transform.position, new Vector3(0, -8.3f));
            yield return new WaitForSeconds(14 / 30f);
            bigCreditHitAnim.SetActive(false);
            CreditHitAnim.SetActive(false);
            if (bonusCoinSymbols[i].Level == 3)
            {
                bigCreditHitAnim.SetActive(true);
            }
            else
            {
                CreditHitAnim.SetActive(true);
            }
            result.WinMoney = bonusCoinSymbols[i].ScoreValue;
            GlobalObserver.UpdateWin(result);
            UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
            if (bonusCoinSymbols[i].Level == 3)
            {
                yield return new WaitForSeconds(20 / 30f);
            }
            else
            {
                yield return new WaitForSeconds(10 / 30f);
            }
        }
        yield return new WaitForSeconds(20 / 30f);
        CreditHitAnim.SetActive(false);
        bigCreditHitAnim.SetActive(false);
        /*result.WinMoney += totalCoinWin;
        GlobalObserver.UpdateWin(result);*/
        collectStopEvent.Invoke();
    }

    public IEnumerator DisPlayCoinCredit(List<BonusCoinSymbol> bonusCoinSymbols, PayoutInfo result)
    {
        //mainCamera.transform.DOShakePosition(0.8f, new Vector3(0.2f, 0.2f, 0f), 70, 90, false, true);
        yield return new WaitForSeconds(1f);

        Dictionary<int, List<BonusCoinSymbol>> levelCoin = new Dictionary<int, List<BonusCoinSymbol>>()
        {
            { 1, new List<BonusCoinSymbol>() },{ 2, new List<BonusCoinSymbol>() },{ 3, new List<BonusCoinSymbol>() }
        };
        foreach (var bsymbol in bonusCoinSymbols)
        {
            levelCoin[bsymbol.Level].Add(bsymbol);
        }
        List<int> ableCollection = Enumerable.Range(0, levelCoin[3].Count).ToList();
        if (GlobalObserver.IsGrandBonus)
        {
            int index = ableCollection.GetRandomItem();
            levelCoin[3][index].isGrand = true;
            ableCollection.Remove(index);
        }
        if (GlobalObserver.IsMajorBonus)
        {
            levelCoin[3][ableCollection.GetRandomItem()].isMajor = true;
        }

        int totalCoinWin = 0;
        for (int i = 1; i <= levelCoin.Count; i++)
        {
            if (levelCoin[i].Count > 0)
            {
                yield return new WaitForSeconds(10 / 30f);
                /*if (i > 1)
                {
                    if (i > 2)
                    {
                        foreach (var item in levelCoin[i])
                        {
                            item.PlayWinAnim();
                        }
                        yield return new WaitForSeconds(30 / 30f);
                        foreach (var item in levelCoin[i])
                        {
                            item.PlayIdleAnim();
                            item.UpSortingOrder();
                        }
                    }
                    CreditIntroAnim[i - 1].SetActive(true);
                    yield return new WaitForSeconds(30 / 30f);
                    CreditIntroAnim[i - 1].SetActive(false);
                }*/
                if (i == 3)
                {
                    foreach (var item in levelCoin[i])
                    {
                        AudioManager.Playback("FE_CoinCreditAppearSpecial");
                        CreditIntroAnim[i - 1].SetActive(true);
                        yield return new WaitForSeconds(20 / 30f);
                        CreditIntroAnim[i - 1].transform.DOMove(item.transform.position, 8 / 30f);
                        yield return new WaitForSeconds(8 / 30f);
                        CreditIntroAnim[i - 1].SetActive(false);
                        CreditIntroAnim[i - 1].transform.localPosition = new Vector3(0, -4.2f);
                        mainCamera.transform.DOShakePosition(18 / 30f, new Vector3(0.2f, 0.2f, 0f), 70, 90, false, true);
                        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][item.Level]);
                        int score = ConfigManager.bonusConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][item.Level][index];
                        item.PlayGlowAnim();
                        //yield return new WaitForSeconds(5 / 30f);
                        item.SetScore(score);
                        if (Cheat.isMinorBonus)
                        {
                            index = 8;
                            Cheat.isMinorBonus = false;
                        }
                        if (Cheat.isMiniBonus)
                        {
                            index = 7;
                            Cheat.isMiniBonus = false;
                        }
                        //index = 8;
                        if (index > 6)
                        {
                            score = ConfigManager.bonusConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][item.Level][index];
                            item.SetScore(score);
                            item.SetScore(GlobalObserver.IDJackpotDic[index - 7]);
                        }
                        if (item.isGrand || item.isMajor)
                        {
                            item.SetScore(item.isGrand ? JackpotType.Grand : JackpotType.Major);
                        }
                        //item.SetScore(GlobalObserver.IDJackpotDic[Random.Range(0, 3)]);
                        totalCoinWin += item.ScoreValue;
                        yield return new WaitForSeconds(30 / 30f);
                        if (item.JackpotType != JackpotType.Null)
                        {
                            yield return JackPot.PlayJackpotCele(item);
                        }
                    }
                }
                else
                {
                    AudioManager.Playback("FE_CoinCreditAppear");
                    CreditIntroAnim[i - 1].SetActive(true);
                    yield return new WaitForSeconds(30 / 30f);
                    CreditIntroAnim[i - 1].SetActive(false);
                    foreach (var item in levelCoin[i])
                    {
                        item.PlayGlowAnim();
                    }
                    //yield return new WaitForSeconds(5 / 30f);
                    foreach (var item in levelCoin[i])
                    {
                        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][item.Level]);
                        int score = ConfigManager.bonusConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][item.Level][index];
                        item.SetScore(score);
                        totalCoinWin += item.ScoreValue;
                    }
                    yield return new WaitForSeconds(30 / 30f);
                }
            }
        }
        for (int i = 0; i < bonusCoinSymbols.Count; i++)
        {
            if (bonusCoinSymbols[i].JackpotType is not JackpotType.Grand and not JackpotType.Major)
            {
                yield return new WaitForSeconds(6 / 30f);
                if (bonusCoinSymbols[i].Level == 3)
                {
                    AudioManager.Playback("FE_CoinCreditCollect_b");
                }
                else
                {
                    AudioManager.Playback("FE_CoinCreditCollect_a（Base）");
                }
                bonusCoinSymbols[i].FlyChange();
                FlySymbol flySymbol = SpawnFactory.GetObject<FlySymbol>("Fly");
                flySymbol.SetSortingOrder(11000);
                flySymbol.PlayCoinUpAnim(bonusCoinSymbols[i].Level, bonusCoinSymbols[i].transform.position, new Vector3(0, -8.3f));
                yield return new WaitForSeconds(14 / 30f);
                bigCreditHitAnim.SetActive(false);
                CreditHitAnim.SetActive(false);
                if (bonusCoinSymbols[i].Level == 3)
                {
                    bigCreditHitAnim.SetActive(true);
                }
                else
                {
                    CreditHitAnim.SetActive(true);
                }
                result.WinMoney = bonusCoinSymbols[i].ScoreValue;
                GlobalObserver.UpdateWin(result);
                UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
                yield return new WaitForSeconds(10 / 30f);
            }
        }
        yield return new WaitForSeconds(20 / 30f);
        CreditHitAnim.SetActive(false);
        bigCreditHitAnim.SetActive(false);
        /*result.WinMoney += totalCoinWin;
        GlobalObserver.UpdateWin(result);*/
        collectStopEvent.Invoke();
    }

    public void PlayFlyAll(Dictionary<int, BonusCoinSymbol> reelIdAnValue)
    {
        Text.SetContent("");
        collectValue = 0;
        ShowTotalWin();
        DelayCallback.Delay(this, 1f, () =>
        {
            foreach (var reel in reelIdAnValue)
            {
                Dictionary<int, BonusCoinSymbol> dic = new Dictionary<int, BonusCoinSymbol>();
                dic.Add(reel.Key, reel.Value);
                FlyIds.Enqueue(dic);
                //FlyIdsValue.Enqueue(reel.Value.ScoreValue);
            }
            StartCoroutine("FlyAnim");
            DelayCallback.Delay(this, 1f, () =>
            {
                UIController.BottomPanel.OnStopBtnClickEvent += StopTotalWin;
                UIController.BottomPanel.StopBtn.interactable = true;
            });
        });
    }

    /// <summary>
    /// 收集动画
    /// </summary>
    /// <returns></returns>
    IEnumerator FlyAnim()
    {
        foreach (var item in FlyIds)
        {
            foreach (var key in item.Keys)
            {
                //item[key].CollectChange(true);
            }
        }
        yield return new WaitForSeconds(6 / 30f);
        while (FlyIds.Count > 0)
        {
            //1、粒子上飞
            int id = 0;
            BonusCoinSymbol symbol = null;
            Dictionary<int, BonusCoinSymbol> dic = FlyIds.Dequeue();
            foreach (var item in dic)
            {
                id = item.Key;
                symbol = item.Value;
            }
            AudioManager.Playback("FE_TotalFly");
            //symbol.PlayCFlyAnim(new Vector3(0, 1.04f));
            //symbol.CollectChange(false);
            int value = FlyIdsValue.Dequeue();
            collectValue += value;
            DelayCallback.Delay(this, 0.66f, () =>
            {
                if (!isHit)
                {
                    PlayTotalWinHit();
                }
                Text.SetContent(collectValue.ToString());
            });

            yield return new WaitForSeconds(1f);
            if (FlyIds.Count == 0)
            {
                UIController.BottomPanel.OnStopBtnClickEvent -= StopTotalWin;
                DelayCallback.Delay(this, 1f, () =>
                {
                    collectStopEvent.Invoke();
                    HideTotalWin();
                });
            }
        }
    }

    /// <summary>
    /// 全部收集
    /// </summary>
    /// <returns></returns>
    IEnumerator FlyAnimAll()
    {
        UIController.BottomPanel.OnStopBtnClickEvent -= StopTotalWin;

        while (FlyIds.Count > 0)
        {
            //1、粒子上飞
            int id = 0;
            BonusCoinSymbol symbol = null;
            Dictionary<int, BonusCoinSymbol> dic = FlyIds.Dequeue();
            foreach (var item in dic)
            {
                id = item.Key;
                symbol = item.Value;
            }
            //symbol.PlayCFlyAnim(new Vector3(0, 1.04f));
            //symbol.CollectChange(false);
            int value = FlyIdsValue.Dequeue();
            collectValue += value;
        }
        DelayCallback.Delay(this, 0.8f, () =>
        {
            PlayTotalWinHit();
            string textTemp = string.Format("${0}", (((double)collectValue) / 100).ToString("N2"));
            Text.SetContent(textTemp);
        });
        yield return new WaitForSeconds(1f);
        if (FlyIds.Count == 0)
        {
            DelayCallback.Delay(this, 1f, () =>
            {
                collectStopEvent.Invoke();
                HideTotalWin();
            });
        }
    }

    /// <summary>
    /// 收集动画快拍
    /// </summary>
    public void StopTotalWin()
    {
        if (FlyIds != null)
        {
            StopCoroutine("FlyAnim");
            StartCoroutine("FlyAnimAll");
        }
    }

    /// <summary>
    /// 显示收集窗口
    /// </summary>
    public void ShowTotalWin()
    {
        TotalWin.gameObject.SetActive(true);
        //播放音乐
        AudioManager.PlayOneShot("FE_TotalMeterIntro");
    }

    /// <summary>
    /// ToTalHit动画
    /// </summary>
    public void PlayTotalWinHit()
    {
        //播放音乐
        isHit = true;
        TotalWin_Hit.gameObject.SetActive(true);
        DelayCallback.Delay(this, 0.9f, () =>
        {
            isHit = false;
            TotalWin_Hit.gameObject.SetActive(false);
        });
    }

    /// <summary>
    /// 隐藏收集窗口
    /// </summary>
    public void HideTotalWin()
    {
        TotalWin.gameObject.SetActive(false);
        Text.SetContent("");
    }


}
