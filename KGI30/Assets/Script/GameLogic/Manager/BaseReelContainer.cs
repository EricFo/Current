using Script.BonusGame.GamePlay.Wheel;
using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Reel.Args;
using System;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class BaseReelContainer : ReelContainer {
    public override event Action<IReelState> OnSpinReelListener;
    public override event Action<IReelState> OnStopReelListener;
    public override event Action<ReelContainer> OnSpinAllListener;
    public override event Action<ReelContainer> OnStopAllListener;

    /// <summary>
    /// Scatter计数
    /// </summary>
    public static int scatterCount = 0;
    public static Queue<string> scatterQueue = new Queue<string>();
    public List<ReelSpinArgs> ReelSpinArgs = new List<ReelSpinArgs>();

    public Transform target_Boost;
    public Transform target_GetAll;

    public Wheel Boost;
    public Wheel GetAll;

    public TurntableUI turntableUI;

    public int boostCount = 0;
    public int getallCount = 0;

    public bool isShutDownAudio = false;

    public override void Initialize() {
        base.Initialize();
        DelayCallback.Delay(this, 0.01f, () => {
            Boost.StartRolling(Boost.idlingSpeed.z);
            GetAll.StartRolling(GetAll.idlingSpeed.z);
        });
        turntableUI.TurntableBoostLevel = 1;
        turntableUI.TurntableGetallLevel = 1;
        turntableUI.ToBaseLayout();
    }

    public override void Spin() {
        isShutDownAudio = false;
        scatterCount = 0;
        ReelSpinArgs.Clear();
        scatterQueue.Clear();
        if (Cheat.isTriggerBonusGame1 || Cheat.isTriggerBonusGame2 || Cheat.isTriggerBonusGame3) {

        } else {
            GlobalObserver.IsUpgradePlay = false;
            GlobalObserver.IsCollectPlay = false;
        }
        GlobalObserver.VarSymbolName = ConfigManager.baseConfig.VarSymbol[GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.VarSymbolProbability)];
        for (int i = 0; i < reels.Length; i++) {
            ReelSpinArgs.Add(Predict(i));
        }
        foreach (var reel in reels) {
            foreach (var symbol in reel.GetVisibleSymbols()) {
                symbol.SetMaskMode(SpriteMaskInteraction.VisibleInsideMask);
            }
        }
        if (scatterCount > 0)//如果有Scatter就生成组合
        {
            string scatterCombo = ConfigManager.baseConfig.ScatterCombo[scatterCount][GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ScatterComboProbability[scatterCount])];
            int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.ScatterResultProbability[scatterCombo]);
            if (index < 3) {
                GlobalObserver.IsTriggerBonusGame = true;
                switch (index) {
                    case 0:
                        GlobalObserver.IsUpgradePlay = true;
                        break;
                    case 1:
                        GlobalObserver.IsCollectPlay = true;
                        break;
                    case 2:
                        GlobalObserver.IsUpgradePlay = true;
                        GlobalObserver.IsCollectPlay = true;
                        break;
                    default:
                        break;
                }
                GlobalObserver.CurrentBonusState = index + 1;
            }

            foreach (var item in scatterCombo) {
                scatterQueue.Enqueue(item.ToString());
            }
            Debug.Log(scatterCount);
        }
        if (isRolling == false) {
            OnReset();
            isRolling = true;
            OnSpinAllListener?.Invoke(this);
            for (int i = 0; i < reels.Length; i++) {
                reels[i].Spin(ReelSpinArgs[i]);
                OnSpinReelListener?.Invoke(reels[i]);
            }
        }
    }

    public override void OnReset() {
        base.OnReset();
        MaxScatterCount = 0;
        ScatterIsValid = false;
    }

    protected override void OnReelStop(ReelBase reel) {
        var symbols = reel.GetVisibleSymbols();
        foreach (var symbol in symbols) {
            symbol.SetMaskMode(SpriteMaskInteraction.None);
            if (symbol.ItemName.Equals(SymbolNames.SCATTERUP)) {
                if (isShutDown)
                {
                    if (!isShutDownAudio)
                    {
                        AudioManager.Playback("FE_BonusSymbolStop");
                        isShutDownAudio = true;
                    }
                }
                else
                {
                    AudioManager.Playback("FE_BonusSymbolStop");
                }
                (symbol as ScatterSymbol).PlayFlyAnim(target_Boost.position);
                boostCount++;
                if (turntableUI.TurntableBoostLevel < 4) {
                    if (boostCount >= ConfigManager.baseConfig.CoinCountOfUpdate[turntableUI.TurntableBoostLevel]) {
                        boostCount = 0;
                        DelayCallback.Delay(this, 0.9f, () => {
                            turntableUI.TurntableBoostLevel++;
                        });
                    } else {
                        DelayCallback.Delay(this, 0.9f, () => {
                            turntableUI.PlayHitAnimation(true);
                        });
                    }
                }
            }
            if (symbol.ItemName.Equals(SymbolNames.SCATTERCOL)) {
                if (isShutDown)
                {
                    if (!isShutDownAudio)
                    {
                        AudioManager.Playback("FE_BonusSymbolStop");
                        isShutDownAudio = true;
                    }
                }
                else
                {
                    AudioManager.Playback("FE_BonusSymbolStop");
                }
                (symbol as ScatterSymbol).PlayFlyAnim(target_GetAll.position);
                getallCount++;
                if (turntableUI.TurntableGetallLevel < 4) {
                    if (getallCount >= ConfigManager.baseConfig.CoinCountOfUpdate[turntableUI.TurntableGetallLevel]) {
                        getallCount = 0;
                        DelayCallback.Delay(this, 0.9f, () => {
                            turntableUI.TurntableGetallLevel++;
                        });
                    } else {
                        DelayCallback.Delay(this, 0.9f, () => {
                            turntableUI.PlayHitAnimation(false);
                        });
                    }
                }
            }
        }
        stopCount++;
        OnStopReelListener?.Invoke(reel);
        if (stopCount >= reels.Length) {
            isRolling = false;
            OnStopAllListener?.Invoke(this);
        }
    }

    public void BonusTrigger() {

    }

    protected override ReelSpinArgs Predict(int id) {
        ReelSpinArgs args = new ReelSpinArgs();
        args = base.Predict(id);
        //要作弊就直接替换下最终结果
        if (Cheat.isTriggerBonusGame1) {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerBonusGame1, id), 0, args.resultSymbols, 0, 3);
        }
        if (Cheat.isTriggerBonusGame2) {

            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerBonusGame2, id), 0, args.resultSymbols, 0, 3);
        }
        if (Cheat.isTriggerBonusGame3) {

            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerBonusGame3, id), 0, args.resultSymbols, 0, 3);
        }
        return args;
    }

    protected override string[] GetReelStripe(int id) {
        return ConfigManager.reelStripConfig.GetStripe("Base")[id];
    }

    public void PlayScatterWin() {
        foreach (var item in reels) {
            foreach (var itam in item.GetVisibleSymbols()) {
                if (GlobalObserver.IsUpgradePlay && itam.ItemName.Equals(SymbolNames.SCATTERUP)) {
                    (itam as ScatterSymbol).PlayWinAnim();
                }
                if (GlobalObserver.IsCollectPlay && itam.ItemName.Equals(SymbolNames.SCATTERCOL)) {
                    (itam as ScatterSymbol).PlayWinAnim();
                }
            }
        }
    }

}
