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

public class CollectGamePlay : BonusGamePlay {
    /// <summary>
    /// 触发了收集效果的ID序列
    /// </summary>
    public List<int> CollectReelId = new List<int>();
    /// <summary>
    /// 本次Bonus已经收集过不能再触发收集的Reel
    /// </summary>
    public List<int> CollectedReelId = new List<int>();

    public CollectGamePlay(BonusReelContainer reelContainer, Wheel turntable) : base(reelContainer, turntable) {

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
        Container.turntableCollect.SpeedUp(Container.turntableCollect.maxSpeed.z, Wheel.Status.Breaking, result);
    }

    public override void EnterBonus() {
        base.EnterBonus();
        CollectReelId.Clear();
        CollectedReelId.Clear();
    }

    public override void OnReelSpin() {
        if (Container.isFirst) {
            Container.turntableCollect.SpeedUp(Container.turntableCollect.bonusIdlingSpeed.z, Wheel.Status.Rolling);
        } else {
            Container.turntableCollect.StartRolling(Container.turntableCollect.bonusIdlingSpeed.z);
        }
    }

    public override void OnShutDown() {
        Container.turntableCollect.Shutdown(true);
    }

    public override void FeaturePlay(object sender, EventArgs args) {
        Container.turntableUI.PlayWin2Animation(false);
        if (wheelOrder[1][Container.CurrentSpinTime - 1] > 0) {
            //无效Feature,该Feature位置不出现金币
            List<int> collectable = new List<int>();
            for (int i = 0; i < Container.Reels.Length; i++) {
                if (!CollectReelId.Contains(i) && Container.Reels[i].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.EMPTY) && !Container.upgradeGamePlay.ValidFeatureReelId.Contains(i)) {
                    collectable.Add(i);
                }
            }
            DelayCallback.Delay(this, 1.5f, () => {
                int collectreelId = collectable[UnityEngine.Random.Range(0, collectable.Count)];
                Container.turntableUI.PlayFeatureFlyAnim(false, Container.upgradeBox[collectreelId].transform.position, collectreelId);
                DelayCallback.Delay(this, 1.55f, () => {
                    Container.collectBox[collectreelId].SetActive(true);
                });
                CollectReelId.Add(collectreelId);
                InvalidFeatureReelId.Add(collectreelId);
            });
        } else {
            //有效Feature，位置为出第上数次出现的金币
            List<int> coinSerialNum = new List<int>();
            foreach (var item in Container.ExisCoinViews) {
                coinSerialNum.Add((Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).serialNum);
            }
            //屏幕上已有对应次数出现的金币
            if (coinSerialNum.Contains(wheelOrder[0][Container.CurrentSpinTime - 1])) {
                foreach (var item in Container.ExisCoinViews) {
                    if (wheelOrder[0][Container.CurrentSpinTime - 1].Equals((Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).serialNum)) {
                        DelayCallback.Delay(this, 1.5f, () => {
                            Container.turntableUI.PlayFeatureFlyAnim(false, Container.upgradeBox[item].transform.position, item);
                            DelayCallback.Delay(this, 1.55f, () => {
                                Container.collectBox[item].SetActive(true);
                            });
                            CollectReelId.Add(item);
                        });
                    }
                }
            }
            //屏幕上还没有对应次数出现的金币
            else {
                List<int> collectable = new List<int>();
                for (int i = 0; i < Container.Reels.Length; i++) {
                    if (!CollectReelId.Contains(i) && !Container.upgradeGamePlay.InvalidFeatureReelId.Contains(i) && !Container.Reels[i].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN)) {
                        collectable.Add(i);
                    }
                }
                DelayCallback.Delay(this, 1.5f, () => {
                    int collectreelId = collectable[UnityEngine.Random.Range(0, collectable.Count)];
                    Container.turntableUI.PlayFeatureFlyAnim(false, Container.upgradeBox[collectreelId].transform.position, collectreelId);
                    DelayCallback.Delay(this, 1.55f, () => {
                        Container.collectBox[collectreelId].SetActive(true);
                    });
                    CollectReelId.Add(collectreelId);
                    FeatureCoinReelId.Add(wheelOrder[0][Container.CurrentSpinTime - 1], collectreelId);
                    ValidFeatureReelId.Add(collectreelId);
                });
            }
        }
    }

    public override IEnumerator FeatureResult() {
        foreach (var item in CollectReelId) {
            if (!CollectedReelId.Contains(item)) {
                if (GlobalObserver.IsUpgradePlay)
                {

                }
                else
                {

                    yield return new WaitForSeconds(2f);
                }
                break;
            }
        }
        for (int i = 0; i < CollectReelId.Count - 1; i++)
        {
            for (int j = 0; j < CollectReelId.Count - 2; j++)
            {
                if (CollectReelId[j] > CollectReelId[j + 1])
                {
                    int temp = CollectReelId[j];
                    CollectReelId[j] = CollectReelId[j + 1];
                    CollectReelId[j + 1] = temp;
                }
            }
        }
        foreach (var item in CollectReelId) {
            if (!CollectedReelId.Contains(item)) {
                if (Container.Reels[item].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN)) {
                    BonusCoinSymbol symbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
                    symbol.PlayGetIntro();
                    AudioManager.PlayOneShot("FE_DGetAllUpgrade");
                    yield return new WaitForSeconds(50 / 30f);
                    Container.collectBox[item].SetActive(false);
                    symbol.PlayGetAdd01();
                    int totalScore = symbol.ScoreValue;
                    foreach (var reelid in Container.ExisCoinViews)
                    {
                        if (!symbol.Equals((Container.Reels[reelid].GetVisibleSymbols()[0] as BonusCoinSymbol)))
                        {
                            AudioManager.Playback("FE_DGetAllCollect");
                            (Container.Reels[reelid].GetVisibleSymbols()[0] as BonusCoinSymbol).PlayFlyAnim(symbol.transform.position);
                            yield return new WaitForSeconds(0.66f);
                            totalScore += (Container.Reels[reelid].GetVisibleSymbols()[0] as BonusCoinSymbol).ScoreValue;
                            symbol.SetScore(totalScore);

                        }
                    }
                    symbol.AnimSpriteRender.DOFade(0, 10 / 30f);
                    yield return new WaitForSeconds(10 / 30f);
                    symbol.PlayIdleAnim();
                    symbol.AnimSpriteRender.DOFade(1, 0);
                    CollectedReelId.Add(item);
                   
                }
            }
        }
        //foreach (var item in removeReelId)
        //{
        //    CollectedReelId.Remove(item);
        //}
    }

    public override void JackpotPlay(object sender, EventArgs args) {
        base.JackpotPlay(sender, args);
        if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType]) {
            Container.turntableUI.PlayWinAnimation(false);
        }
    }

    public override void ScoreRewardPlay(object sender, EventArgs args)
    {
        DelayCallback.BeginCoroutine(ScoreReward(sender, args));
    }

    IEnumerator ScoreReward(object sender, EventArgs args)
    {
        Container.turntableUI.PlayWinAnimation(false);
        yield return new WaitForSeconds(0.3f);
        Container.turntableUI.MulCreditFly.SetActive(true);
        AudioManager.PlayOneShot("FE_WheelstopCredit");
        yield return new WaitForSeconds(20 / 30f);
        Container.turntableUI.MulCreditFly.SetActive(false);
        base.ScoreRewardPlay(sender, args);
    }
}
