using UnityEngine;
using SlotGame.Symbol;
public class PICSymbol : CommonSymbol
{
    public SpriteRenderer PICBG;

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder);
    }
    public override void SetSortingOrder(int order)
    {
        PICBG.sortingOrder = order;
        base.SetSortingOrder(order);
        PICBG.transform.localPosition = Vector3.forward * 0.1f;
    }
}
