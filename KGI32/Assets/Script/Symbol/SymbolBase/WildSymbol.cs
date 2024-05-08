using UnityEngine;
using SlotGame.Symbol;

public class WildSymbol : CommonSymbol {
    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101) {
        base.Install(parent, localPos, reelID, sortingOrder + 10);
    }

    public override void PlayIdleAnim()
    {
        SetSortingOrder(DefaultSortingOrder);
        base.PlayIdleAnim();
    }

    public override void PlayAwardAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 400);
        base.PlayAwardAnim();
    }

}
