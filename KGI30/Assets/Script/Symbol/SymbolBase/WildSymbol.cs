using UnityEngine;
using SlotGame.Symbol;

public class WildSymbol : CommonSymbol {
    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101) {
        base.Install(parent, localPos, reelID, sortingOrder + 20);
    }

    public override void PlayAwardAnim() {
        if (MainTex.sortingOrder < 200) {
            MainTex.sortingOrder = MainTex.sortingOrder + 200;
        }
        base.PlayAwardAnim();
    }

    public override void PlayIdleAnim() {
        if (MainTex.sortingOrder > 200) {
            MainTex.sortingOrder = MainTex.sortingOrder - 200;
        }
        base.PlayIdleAnim();
    }
}
