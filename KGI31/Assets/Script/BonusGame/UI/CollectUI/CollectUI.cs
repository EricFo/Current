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

public class CollectUI : MonoBehaviour
{
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

    public void PlayFlyAll(Dictionary<int,BonusCoinSymbol> reelIdAnValue)
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
                FlyIdsValue.Enqueue(reel.Value.ScoreValue);
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
                item[key].CollectChange(true);
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
            symbol.PlayCFlyAnim(new Vector3(0, 1.04f));
            symbol.CollectChange(false);
            int value = FlyIdsValue.Dequeue();
            collectValue += value;
            //AudioManager.Playback("G5_RS_PFly");
            //Flys[id].gameObject.SetActive(true);
            DelayCallback.Delay(this,0.66f, () => {
                if (!isHit)
                {
                    PlayTotalWinHit();
                }
                Text.SetContent(collectValue.ToString());
            });
            DelayCallback.Delay(this,1f, () => {
                //Flys[id].gameObject.SetActive(false);
            });
            //2、金币的粒子效果消失
            //symbol.PlayCoinEndAnim();
            //symbol.PlayStopAnim();

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
            symbol.PlayCFlyAnim(new Vector3(0, 1.04f));
            symbol.CollectChange(false);
            int value = FlyIdsValue.Dequeue();
            collectValue += value;
            //AudioManager.PlayOneShot("G5_RS_PFly");
            //Flys[id].gameObject.SetActive(true);

            //2、金币的粒子效果消失
            //symbol.PlayCoinEndAnim();
            //symbol.PlayStopAnim();
        }
        DelayCallback.Delay(this,0.8f, () => {
            PlayTotalWinHit();
            string textTemp = string.Format("${0}", (((double)collectValue) / 100).ToString("N2"));
            Text.SetContent(textTemp);
        });
        //DelayCallback.Delay(this,1f, () => {
        //    foreach (var item in Flys)
        //    {
        //        item.gameObject.SetActive(false);
        //    }
        //});
        yield return new WaitForSeconds(1f);
        if (FlyIds.Count == 0)
        {
            DelayCallback.Delay(this,1f, () => {
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
