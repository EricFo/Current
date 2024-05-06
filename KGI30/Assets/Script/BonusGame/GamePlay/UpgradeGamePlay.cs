using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Result;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class UpgradeGamePlay : BonusGamePlay {
    /// <summary>
    /// 触发加分效果的ID序列
    /// </summary>
    public List<int> UpgradeReelId = new List<int>();
    public int miniCount = 0;
    /// <summary>
    /// 本轮是否触发了Double玩法
    /// </summary>
    public bool isDouble = false;


    public UpgradeGamePlay(BonusReelContainer reelContainer, Wheel turntable) : base(reelContainer, turntable) {

    }

    public override void OnReelStop() {
        if (result == -1) {
            if (wheelOrder[1][Container.CurrentSpinTime - 1] > 0) {
                result = boostIndex[UnityEngine.Random.Range(0, boostIndex.Length)];
            } else {
                if (wheelOrder[0][Container.CurrentSpinTime - 1] > 0) {
                    result = boostIndex[UnityEngine.Random.Range(0, boostIndex.Length)];
                } else {
                    result = ConfigManager.bonusConfig.WheelScoreIndex[GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.WheelScoreProbability)];
                }
            }
        }
        Container.turntableUpgrade.SpeedUp(Container.turntableUpgrade.maxSpeed.z, Wheel.Status.Breaking, result);
    }

    public override void EnterBonus() {
        base.EnterBonus();
        UpgradeReelId.Clear();
    }

    public override void OnReelSpin() {
        if (Container.isFirst) {
            Container.turntableUpgrade.SpeedUp(Container.turntableUpgrade.bonusIdlingSpeed.z, Wheel.Status.Rolling);
        } else {
            Container.turntableUpgrade.StartRolling(Container.turntableUpgrade.bonusIdlingSpeed.z);
        }
        isDouble = false;
        isFeatureShutDown = false;
        isShutDownAudio = false;
    }

    public override void OnShutDown() {
        Container.turntableUpgrade.Shutdown(true);
    }

    public override void FeaturePlay(object sender, EventArgs args)
    {
        isDouble = true;
        Container.turntableUI.PlayWin2Animation(true);
    }

    public override IEnumerator FeatureResult()
    {
        if (!isDouble)
        {
            yield break;
        }
        FeatureIntervalTime = 1f;
        Container.CurrentActive = -1;
        Container.turntableUI.PlayFeatureFlyAnim(true);
        yield return new WaitForSeconds(7 / 30f);
        foreach (var item in Container.Reels)
        {
            if (item.GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN))
            {
                yield return new WaitForSeconds(0.2f);
                BonusCoinSymbol bsymbol = item.GetVisibleSymbols()[0] as BonusCoinSymbol;
                bsymbol.PlayDoubleFlyAnim(Container.turntableUI.BoostFly.transform.position);
                AudioManager.Playback("FE_Wheelstop02b");
            }
        }
        Container.turntableUI.PlayFlyIdleAnim(true);
        Container.turntableUI.PlayWinAnimation(true);
        yield return new WaitForSeconds(39 / 30f);
        UIController.BottomPanel.StopBtn.interactable = true;
        UIController.BottomPanel.OnStopBtnClickEvent -= FeatureShutDown;
        UIController.BottomPanel.OnStopBtnClickEvent += FeatureShutDown;
        foreach (var item in Container.Reels)
        {
            if (item.GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN))
            {
                BonusCoinSymbol bsymbol = item.GetVisibleSymbols()[0] as BonusCoinSymbol;
                bsymbol.PlayBoostAdd01();
                bsymbol.ScoreIncressTime(bsymbol.ScoreValue * 2, 1f);
                if (isFeatureShutDown)
                {
                    if (!isShutDownAudio)
                    {
                        AudioManager.Playback("FE_DBoostAdd");
                        isShutDownAudio = true;
                    }
                }
                else
                {
                    AudioManager.Playback("FE_DBoostAdd");
                }
                DelayCallback.Delay(this, 1f, () =>
                {
                    bsymbol.PlayIdleAnim();
                    bsymbol.PlayBGIdleAnim();
                });
                yield return new WaitForSeconds(FeatureIntervalTime);
            }
        }
        UIController.BottomPanel.OnStopBtnClickEvent -= FeatureShutDown;
    }

    public override void JackpotPlay(object sender, EventArgs args) {
        base.JackpotPlay(sender, args);
        //if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType]) {
        Container.turntableUI.PlayWin2Animation(true);
        //}
    }

    public override void ScoreRewardPlay(object sender, EventArgs args)
    {
        Container.isScore = true;
        DelayCallback.BeginCoroutine(ScoreReward(sender, args));
    }
    IEnumerator ScoreReward(object sender, EventArgs args)
    {
        Container.turntableUI.PlayWin2Animation(true);
        yield return new WaitForSeconds(0.3f);
        Container.turntableUI.DouCreditFly.SetActive(true);
        AudioManager.PlayOneShot("FE_WheelstopCredit");
        yield return new WaitForSeconds(20 / 30f);
        Container.turntableUI.DouCreditFly.SetActive(false);
        base.ScoreRewardPlay(sender, args);
    }
}
