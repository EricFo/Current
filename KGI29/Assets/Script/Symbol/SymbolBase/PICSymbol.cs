using UnityEngine;
using SlotGame.Symbol;
public class PICSymbol : CommonSymbol
{
    public SpriteRenderer PICBG;
    public override void SetSortingOrder(int order)
    {
        PICBG.sortingOrder = order;
        base.SetSortingOrder(order);
        PICBG.transform.localPosition = Vector3.forward * 0.1f;
    }
}
