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
    public float BoostIntervalTime = 2f;
    public int miniCount = 0;


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
    }

    public override void OnShutDown() {
        Container.turntableUpgrade.Shutdown(true);
    }

    public override void FeaturePlay(object sender, EventArgs args) {
        Container.turntableUI.PlayWin2Animation(true);
        if (wheelOrder[1][Container.CurrentSpinTime - 1] > 0) {
            //无效Feature,该Feature位置不出现金币
            List<int> upgradeable = new List<int>();
            for (int i = 0; i < Container.Reels.Length; i++) {
                if (!UpgradeReelId.Contains(i) && Container.Reels[i].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.EMPTY) && !Container.collectGamePlay.ValidFeatureReelId.Contains(i)) {
                    upgradeable.Add(i);
                }
            }
            DelayCallback.Delay(this, 1.5f, () => {
                int upgradereelId = upgradeable[UnityEngine.Random.Range(0, upgradeable.Count)];
                Container.turntableUI.PlayFeatureFlyAnim(true, Container.upgradeBox[upgradereelId].transform.position, upgradereelId);
                DelayCallback.Delay(this, 27/30f, () => {
                    Container.upgradeBox[upgradereelId].SetActive(true);
                });
                UpgradeReelId.Add(upgradereelId);
                InvalidFeatureReelId.Add(upgradereelId);
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
                            Container.turntableUI.PlayFeatureFlyAnim(true, Container.upgradeBox[item].transform.position, item);
                            BonusCoinSymbol bsymbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
                            bsymbol.SetSortingOrder(bsymbol.DefaultSortingOrder + 500);
                            DelayCallback.Delay(this, 50 / 30f, () =>
                            {
                                bsymbol.SetSortingOrder(bsymbol.DefaultSortingOrder);
                            });
                            DelayCallback.Delay(this, 27/30f, () => {
                                Container.upgradeBox[item].SetActive(true);
                                (Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol).PlayBG();
                            });
                            UpgradeReelId.Add(item);
                        });
                    }
                }
            }
            //屏幕上还没有对应次数出现的金币
            else {
                List<int> upgradeable = new List<int>();
                for (int i = 0; i < Container.Reels.Length; i++) {
                    if (!UpgradeReelId.Contains(i) && !Container.collectGamePlay.InvalidFeatureReelId.Contains(i) && !Container.Reels[i].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN))
                    {
                        upgradeable.Add(i);
                    }
                }
                DelayCallback.Delay(this, 1.5f, () => {
                    int upgradereelId = upgradeable[UnityEngine.Random.Range(0, upgradeable.Count)];
                    Container.turntableUI.PlayFeatureFlyAnim(true, Container.upgradeBox[upgradereelId].transform.position, upgradereelId);
                    DelayCallback.Delay(this, 27/30f, () => {
                        Container.upgradeBox[upgradereelId].SetActive(true);
                    });
                    UpgradeReelId.Add(upgradereelId);
                    FeatureCoinReelId.Add(wheelOrder[0][Container.CurrentSpinTime - 1], upgradereelId);
                    ValidFeatureReelId.Add(upgradereelId);
                });
            }
        }
    }

    public override IEnumerator FeatureResult() {
        for (int i = 0; i < UpgradeReelId.Count - 1; i++)
        {
            for (int j = 0; j < UpgradeReelId.Count - 1 - i; j++)
            {
                if (UpgradeReelId[j] > UpgradeReelId[j + 1])
                {
                    int temp = UpgradeReelId[j];
                    UpgradeReelId[j] = UpgradeReelId[j + 1];
                    UpgradeReelId[j + 1] = temp;
                }
            }
        }

        BoostIntervalTime = 2f;
        foreach (var item in UpgradeReelId) {
            if (Container.Reels[item].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN)) {
                yield return new WaitForSeconds(1.5f);
                Container.CurrentActive = -1;
                UIController.BottomPanel.StopBtn.interactable = true;
                UIController.BottomPanel.OnStopBtnClickEvent -= BoostShutDown;
                UIController.BottomPanel.OnStopBtnClickEvent += BoostShutDown;
                break;
            }
        }

        foreach (var item in UpgradeReelId) {
            if (Container.Reels[item].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN)) {
                BonusCoinSymbol symbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;
                int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.UpgradeCoinCreditsProbability[GlobalObserver.CurrentBonusState]);
                float score = ConfigManager.bonusConfig.UpgradeCoinCredits[GlobalObserver.CurrentBonusState][index] * GlobalObserver.BetValue / 80;
                if((Container.collectGamePlay as CollectGamePlay).CollectReelId.Contains(item)&& !(Container.collectGamePlay as CollectGamePlay).CollectedReelId.Contains(item))
                {
                    symbol.PlayBoostAdd02();
                }
                else
                {
                    symbol.PlayBoostAdd01();
                }
                symbol.ScoreIncress(symbol.ScoreValue + (int)score);
                DelayCallback.Delay(this, 2f, symbol.PlayIdleAnim);
                AudioManager.PlayOneShot("FE_DBoostAdd");
                symbol.PlayBG();
                yield return new WaitForSeconds(BoostIntervalTime);
            }
        }
        if (BoostIntervalTime == 0)
        {
            //yield return new WaitForSeconds(2f); 
        }
        UIController.BottomPanel.OnStopBtnClickEvent -= BoostShutDown;
        yield return new WaitForSeconds(1f);
        //foreach (var item in UpgradeReelId)
        //{
        //    if (Container.Reels[item].GetVisibleSymbols()[0].ItemName.Equals(SymbolNames.BCOIN))
        //    {
        //        BonusCoinSymbol symbol = Container.Reels[item].GetVisibleSymbols()[0] as BonusCoinSymbol;

        //        symbol.PlayBoostAdd01();
        //    }
        //}
    }

    public void BoostShutDown()
    {
        BoostIntervalTime = 0;
    }

    public override void JackpotPlay(object sender, EventArgs args) {
        base.JackpotPlay(sender, args);
        if (JackPotData.MettersCount[jpType] == JackPotData.MettersZhong[jpType]) {
            Container.turntableUI.PlayWinAnimation(true);
        }
    }

    public override void ScoreRewardPlay(object sender, EventArgs args)
    {
        DelayCallback.BeginCoroutine(ScoreReward(sender, args));
    }
    IEnumerator ScoreReward(object sender, EventArgs args)
    {
        Container.turntableUI.PlayWinAnimation(true);
        yield return new WaitForSeconds(0.3f);
        Container.turntableUI.DouCreditFly.SetActive(true);
        AudioManager.PlayOneShot("FE_WheelstopCredit");
        yield return new WaitForSeconds(20 / 30f);
        Container.turntableUI.DouCreditFly.SetActive(false);
        base.ScoreRewardPlay(sender, args);
    }
}
