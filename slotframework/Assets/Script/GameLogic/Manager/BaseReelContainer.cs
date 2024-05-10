using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Core.Reel;
using SlotGame.Reel.Args;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseReelContainer : ReelContainer {
    public override event Action<IReelState> OnSpinReelListener;
    public override event Action<IReelState> OnStopReelListener;
    public override event Action<ReelContainer> OnSpinAllListener;
    public override event Action<ReelContainer> OnStopAllListener;

    public override void Spin() {
        MaxScatterCount = 0;
        ScatterIsValid = false;
        base.Spin();
    }

    protected override void OnReelStop(ReelBase reel)
    {
        stopCount++;
        OnStopReelListener?.Invoke(reel);
        if (stopCount >= reels.Length)
        {
            if (Cheat.isTriggerBonusGame)
            {
                GlobalObserver.IsTriggerBonusGame = true;
            }
            isRolling = false;
            OnStopAllListener?.Invoke(this);
        }
    }

    protected override ReelSpinArgs Predict(int id) {
        var reelSpinArgs = base.Predict(id);
        //要作弊就直接替换下最终结果
        if (Cheat.isTriggerJackPot) {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isTriggerFreeGame) {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isFreeAndJackPot) {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isFreeInJackPot) {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerJackPot, id), 0, reelSpinArgs.resultSymbols, 1, 3);
        }
        if (Cheat.isTriggerBonusGame)
        {
            System.Array.Copy(ConfigManager.cheatConfig.GetCheats(Cheat.TriggerBonusGame, id), 0, reelSpinArgs.resultSymbols, 0, 3);
        }
        return reelSpinArgs;
    }

    protected override string[] GetReelStripe(int id) {
        return ConfigManager.reelStripConfig.GetStripe("Base")[id];
    }
}
