using DG.Tweening;
using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Config;
using SlotGame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using Random = UnityEngine.Random;

public class CollectGamePlay : BonusGamePlay {
    /// <summary>
    /// 触发了收集效果的ID序列
    /// </summary>
    public List<int> CollectReelId = new List<int>();
    /// <summary>
    /// 本次Bonus已经收集过不能再触发收集的Reel
    /// </summary>
    public List<int> CollectedReelId = new List<int>();
    /// <summary>
    /// 乘数需要下飞到钻石的数量
    /// </summary>
    private int multiplierCount = 0;
    /// <summary>
    /// 本轮是否触发了Multiplier玩法
    /// </summary>
    private bool isMultiplier = false;
    /// <summary>
    /// 不同乘数对应转轮的位置索引
    /// </summary>
    private Dictionary<int, int> MultiplierIndex = new Dictionary<int, int>()
    {
        {2,1},{3,3},{4,5},{5,7},{6,9},{7,11}
    };
    public bool isMultiplier7;
    public bool isMultiplier6;

    public CollectGamePlay(BonusReelContainer reelContainer, Wheel turntable) : base(reelContainer, turntable) {

    }

    public override void OnReelStop()
    {
        if (result == -1)
        {
            int multiplier = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.MultiplierValueProbability) + 2;
            /*if (isMultiplier6)
            {
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.MultiplierValueProbability_6);
                multiplier = ConfigManager.bonusConfig.MultiplierValue_6[index];
            }
            if (isMultiplier7)
            {
                multiplier = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.MultiplierValueProbability_7) + 2;
            }
            if (isMultiplier6 && isMultiplier7)
            {
                multiplier = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.MultiplierValueProbability_67) + 2;
            }*/
            if (wheelOrder[1][Container.CurrentSpinTime - 1] > 0)
            {
                result = MultiplierIndex[multiplier];
            }
            else
            {
                if (wheelOrder[0][Container.CurrentSpinTime - 1] > 0)
                {
                    result = MultiplierIndex[multiplier];
                }
                else
                {
                    result = ConfigManager.bonusConfig.WheelScoreIndex[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.WheelScoreProbability)];
                }
            }
        }
        if (result == MultiplierIndex[7])
        {
            isMultiplier7 = true;
        }
        if (result == MultiplierIndex[6])
        {
            isMultiplier6 = true;
        }
        Container.turntableCollect.SpeedUp(Container.turntableCollect.maxSpeed.z, Wheel.Status.Breaking, result);
    }

    public override void EnterBonus() {
        base.EnterBonus();
        CollectReelId.Clear();
        CollectedReelId.Clear();
        isMultiplier7 = false;
        isMultiplier6 = false;
    }

    public override void OnReelSpin()
    {
        if (Container.isFirst)
        {
            Container.turntableCollect.SpeedUp(Container.turntableCollect.bonusIdlingSpeed.z, Wheel.Status.Rolling);
        }
        else
        {
            Container.turntableCollect.StartRolling(Container.turntableCollect.bonusIdlingSpeed.z);
        }
        isMultiplier = false;
        isFeatureShutDown = false;
        isShutDownAudio = false;
    }

    public override void OnShutDown() {
        Container.turntableCollect.Shutdown(true);
    }

    public override void FeaturePlay(object sender, EventArgs args)
    {
        isMultiplier = true;
        Container.turntableUI.PlayWin2Animation(false);
    }

    public override IEnumerator FeatureResult() {
        if (!isMultiplier)
        {
            yield break;
        }
        FeatureIntervalTime = 2f;
        Container.CurrentActive = -1;
        int multiplier = ResultArgs[turntableWheel.CurrentCellIndex()];
        multiplierCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.MultiplierCountProbability) + 1;
        List<int> mulAbleReel = new List<int>();
        for (int i = 0; i < Container.Reels.Length; i++)
        {
            if (Container.Reels[i].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN))
            {
                mulAbleReel.Add(i);
            }
        }
        //restrict multiplier count cant more than diamond count before have math data
        if (multiplierCount > mulAbleReel.Count)
        {
            multiplierCount = mulAbleReel.Count;
        }
        List<int> mulReelId = new List<int>();
        for(int i = 0; i < multiplierCount; i++)
        {
            int reelId = mulAbleReel[Random.Range(0, mulAbleReel.Count)];
            mulReelId.Add(reelId);
            mulAbleReel.Remove(reelId);
        }
        //bubble sort,from small to big
        for (int i = 0; i < mulReelId.Count - 1; i++)
        {
            for (int j = 0; j < mulReelId.Count - 1 - i; j++)
            {
                if (mulReelId[j] > mulReelId[j + 1])
                {
                    int temp = mulReelId[j];
                    mulReelId[j] = mulReelId[j + 1];
                    mulReelId[j + 1] = temp;
                }
            }
        }
        Container.turntableUI.PlayFeatureFlyAnim(false);
        yield return new WaitForSeconds(7 / 30f);
        foreach (var item in mulReelId)
        {
            yield return new WaitForSeconds(0.2f);
            BonusCoinSymbol bsymbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
            bsymbol.PlayMulFlyAnim(Container.turntableUI.GetallFly.transform.position);
            AudioManager.Playback("FE_Wheelstop02b");
        }
        Container.turntableUI.PlayFlyIdleAnim(false);
        yield return new WaitForSeconds(34/30f);
        UIController.BottomPanel.StopBtn.interactable = true;
        UIController.BottomPanel.OnStopBtnClickEvent -= FeatureShutDown;
        UIController.BottomPanel.OnStopBtnClickEvent += FeatureShutDown;
        //foreach (var item in mulReelId)
        //{
        //    BonusCoinSymbol bsymbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
        //    bsymbol.PlayGetIntro();
        //    AudioManager.PlayOneShot("FE_DGetAllUpgrade");
        //}
        //yield return new WaitForSeconds(50 / 30f);
        //foreach (var item in mulReelId)
        //{
        //    BonusCoinSymbol bsymbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
        //    bsymbol.ResetSortingOrder();
        //}
        FeatureIntervalTime = 20 * multiplier / 30f;
        bool isWait = true;
        for (int i = 0; i < mulReelId.Count; i++)
        {
            int item = mulReelId[i];
            BonusCoinSymbol bsymbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
            DelayCallback.BeginCoroutine(MulSymbol(multiplier));
            bsymbol.PlayGetAdd01();
            //AudioManager.Playback("FE_DGetAllCollect");
            bsymbol.ScoreIncressMultiplier(multiplier);
            DelayCallback.Delay(this, 20 * (multiplier - 1) / 30f, () =>
            {
                bsymbol.AnimSpriteRender.DOFade(0, 10 / 30f);
                DelayCallback.Delay(this, 10 / 30f, () =>
                {
                    bsymbol.PlayIdleAnim();
                    bsymbol.AnimSpriteRender.DOFade(1, 0);
                });
            });
            if (i == mulReelId.Count - 1)
            {
                if (FeatureIntervalTime>0)
                {
                    isWait = false;
                }
                else
                {
                    isWait = true;
                }
            }
            yield return new WaitForSeconds(FeatureIntervalTime);
        }
        UIController.BottomPanel.OnStopBtnClickEvent -= FeatureShutDown;
        if (isFeatureShutDown&&isWait)
        {
            yield return new WaitForSeconds(20 * (multiplier - 1) / 30f);
        }
        Container.turntableUI.PlayWinAnimation(false);
    }
    private IEnumerator MulSymbol(int multiplier)
    {
        for(int i = 1; i < multiplier; i++)
        {
            AudioManager.Playback("FE_DGetAllCollect");
            yield return new WaitForSeconds(20/30f);
        }
    }

    public override void JackpotPlay(object sender, EventArgs args) {
        base.JackpotPlay(sender, args);
        //if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType]) {
        Container.turntableUI.PlayWin2Animation(false);
        //}
    }

    public override void ScoreRewardPlay(object sender, EventArgs args) {
        Container.isScore = true;
        DelayCallback.BeginCoroutine(ScoreReward(sender, args));
    }

    IEnumerator ScoreReward(object sender, EventArgs args)
    {
        Container.turntableUI.PlayWin2Animation(false);
        yield return new WaitForSeconds(0.3f);
        Container.turntableUI.MulCreditFly.SetActive(true);
        AudioManager.PlayOneShot("FE_WheelstopCredit");
        yield return new WaitForSeconds(20 / 30f);
        Container.turntableUI.MulCreditFly.SetActive(false);
        base.ScoreRewardPlay(sender, args);
    }
}
