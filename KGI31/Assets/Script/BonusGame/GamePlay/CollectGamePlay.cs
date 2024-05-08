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
    /// 不同乘数对应转轮的位置索引
    /// </summary>
    private Dictionary<int, int> MultiplierIndex = new Dictionary<int, int>()
    {
        {2,1},{3,3},{4,5},{5,7},{7,9},{10,11}
    };


    public CollectGamePlay(BonusReelContainer reelContainer, Wheel turntable) : base(reelContainer, turntable) {

    }

    public override void OnReelStop() {
        //Debug.Log(result);
        turntableWheel.SpeedUp(result);
    }

    public override void EnterBonus() {
        base.EnterBonus();
        CollectReelId.Clear();
        CollectedReelId.Clear();
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
            int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.bonusConfig.FeatureScoreProbability_R[dicIndex]);
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
    }
    public override IEnumerator FeaturePlay()
    {
        if (!isFeaturePlay)
        {
            yield break;
        }
        //if (jpResult != JackpotType.Empty)
        //{
        //    yield return new WaitForSeconds(42 / 30f);
        //}

        int stopPos = turntableWheel.CurrentCellIndex();
        Container.turntableUI.PlayMultiplierFlyAnim(Container.multiplierUI.transform.position, ResultArgs[stopPos] == 1);
        AudioManager.Playback("FE_WheelRAdd");
        yield return new WaitForSeconds(27 / 30f);
        GlobalObserver.PayMultiplier += ResultArgs[stopPos];
        Container.UpdateMultiplier();
        //Container.turntableUI.PlayWinAnimation(false);
    }

    public override IEnumerator FeatureResult() {
        yield break;
    }

}
