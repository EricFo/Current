using UnityEngine;
using SlotGame.Symbol;

public class ScatterSymbol : CommonSymbol {
    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101) {
        base.Install(parent, localPos, reelID, sortingOrder + 10);
    }
}
