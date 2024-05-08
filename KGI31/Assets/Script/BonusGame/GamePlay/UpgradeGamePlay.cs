using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Result;
using SlotGame.Symbol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using UniversalModule.SpawnSystem;
using Random = UnityEngine.Random;

public class UpgradeGamePlay : BonusGamePlay {
    /// <summary>
    /// 触发加分效果的ID序列
    /// </summary>
    public List<int> UpgradeReelId = new List<int>();
    /// <summary>
    /// 效果间隔时间
    /// </summary>
    public float BoostIntervalTime = 2f;
    public int miniCount = 0;

    /// <summary>
    /// 可以被替换为Wild的Symbol位置
    /// </summary>
    public List<int[]> wildReplaceAble = new List<int[]>()
    {
        {new int[] {1,2 } },
        {new int[] {2,2 } },
        {new int[] {3,2 } },
        {new int[] {1,3 } },
        {new int[] {2,3 } },
        {new int[] {3,3 } },
        {new int[] {1,4 } },
        {new int[] {2,4 } },
        {new int[] {3,4 } }
    };
    /// <summary>
    /// 需要被替换为Wild的Symbol位置
    /// </summary>
    //public List<int[]> wildReplaceFeature = new List<int[]>();


    public UpgradeGamePlay(BonusReelContainer reelContainer, Wheel turntable) : base(reelContainer, turntable) {
    }

    public override void OnReelStop() {
        //Debug.Log(result);
        turntableWheel.SpeedUp(result);
    }

    public override void EnterBonus() {
        base.EnterBonus();
        UpgradeReelId.Clear();
    }

    public override void OnReelSpin() {
        base.OnReelSpin();
        if (result == -1)
        {
            int dicIndex;
            if (Container.doneFree1ToCombo)
            {
                dicIndex = 3;
            }
            else if (Container.doneFree2ToCombo)
            {
                dicIndex = 4;
            }
            else
            {
                dicIndex = GlobalObserver.CurrentBonusState == 3 ? 2 : 1;
            }
            int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.FeatureScoreProbability_L[dicIndex]);
            result = ConfigManager.bonusConfig.FeatureScoreIndex[index];
        }
        //result = 2;
        if (jpResult != JackpotType.Empty)
        {
            ResultOptions[result] -= JackpotPlay;
            ResultOptions[result] += JackpotPlay;
        }
        Container.turntableUI.UpdateStars(result, jpResult, turntableWheel.isExtra);
    }

    public override void OnShutDown() {
        turntableWheel.ShutDown();
    }

    public override void FeaturePlay(object sender, EventArgs args) {
        isFeaturePlay = true;
        //Container.turntableUI.PlayWin2Animation(true);
        //Container.turntableUI.PlayWin2Animation(true);
    }
    public override IEnumerator FeaturePlay()
    {
        if(!isFeaturePlay)
        {
            yield break;
        }
        //if(jpResult != JackpotType.Empty)
        //{
        //    yield return new WaitForSeconds(42 / 30f);
        //}

        FeatureIntervalTime = 1f;
        Container.CurrentActive = -1;
        Container.turntableUI.PlayWildFlyAnim();
        AudioManager.Playback("FE_WheelLExtra01");
        yield return new WaitForSeconds(20 / 30f);

        int wildCount = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.WildCountProbability[GlobalObserver.CurrentBonusState]) + 2;
        List<int> indexWild = new List<int>();
        //List<int> indexAble = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        List<int> indexAble = Enumerable.Range(0, 9).ToList();
        for (int i = 0; i < wildCount; i++)
        {
            int index = indexAble[Random.Range(0,indexAble.Count)];
            indexWild.Add(index);
            indexAble.Remove(index);
        }
        for(int i = 0; i < indexWild.Count-1; i++)
        {
            for(int j= 0; j < indexWild.Count-i-1; j++)
            {
                if (indexWild[j] > indexWild[j + 1])
                {
                    (indexWild[j + 1], indexWild[j]) = (indexWild[j], indexWild[j + 1]);
                }
            }
        }
        //UIController.BottomPanel.StopBtn.interactable = true;
        UIController.BottomPanel.OnStopBtnClickEvent -= FeatureShutDown;
        UIController.BottomPanel.OnStopBtnClickEvent += FeatureShutDown;
        for (int i = 0; i < indexWild.Count; i++)
        {
            int item = indexWild[i];
            yield return new WaitForSeconds(0.2f);
            int reelIndex = wildReplaceAble[item][0];
            int symbolIndex = wildReplaceAble[item][1];
            FlySymbol wildFly = SpawnFactory.GetObject<SpawnItem>(SymbolNames.WILDFLY) as FlySymbol;
            Vector3 startPos = Container.turntableUI.BoostFly.transform.position;
            Vector3 targetPos = Container.Reels[reelIndex].GetAllSymbols()[symbolIndex].transform.position;
            wildFly.PlayFlyAnim(startPos, targetPos, i);
            AudioManager.Playback("FE_WheelLExtra02");
            DelayCallback.Delay(this, 14 / 30f, () =>
            {
                CommonSymbol symbol = Container.Reels[reelIndex].ReplaceSymbol(SymbolNames.WILD, symbolIndex);
                symbol.SetMaskMode(SpriteMaskInteraction.None);
            });
            //yield return new WaitForSeconds(FeatureIntervalTime);
        }
        Container.turntableUI.PlayFlyIdleAnim(true);
        yield return new WaitForSeconds(38 / 30f);
        //Container.turntableUI.PlayWinAnimation(false);
        UIController.BottomPanel.OnStopBtnClickEvent -= FeatureShutDown;
    }

    public override IEnumerator FeatureResult() {
        yield break;
    }

}
