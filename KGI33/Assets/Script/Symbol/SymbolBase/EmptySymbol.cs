using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySymbol : CommonSymbol
{
    public Sprite[] B_Symbols;

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder);
    }

    public void InitSymbolMainTex()
    {
        MainTex.sprite = B_Symbols[Random.Range(0, B_Symbols.Length)];
    }
}
