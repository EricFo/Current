using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;
using static UnityEngine.ParticleSystem;

public class AddSpinSymbol : CommonSymbol
{
    public SortingGroup loopAnimSorting;
    public SortingGroup flyAnimSorting;

    public ParticleSystemRenderer[] loopAnim;
    public GameObject flyAnim;
    public Animator spinLoopAnim;

    public SpriteRenderer spinRenderer;
    public Sprite[] spinSprites;


    #region 动画Hash
    protected int LOOP = Animator.StringToHash("Loop");
    #endregion

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder);
        if (ItemName == SymbolNames.ADDSPIN1)
        {
            spinRenderer.sprite = spinSprites[0];
        }
        else
        {
            spinRenderer.sprite = spinSprites[1];
        }
        loopAnimSorting.gameObject.SetActive(true);
        spinLoopAnim.Play(LOOP);
    }

    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        loopAnimSorting.sortingOrder = order + 1;
        flyAnimSorting.sortingOrder = order + 1000;
    }

    public override void SetMaskMode(SpriteMaskInteraction interaction)
    {
        base.SetMaskMode(interaction);
        foreach(var item in loopAnim)
        {
            item.maskInteraction = interaction;
        }
    }

    public override void SetDisplay(bool isDisplay)
    {
        base.SetDisplay(isDisplay);
        loopAnimSorting.gameObject.SetActive(isDisplay);
    }

    public void PlayFlyAnim()
    {
        AudioManager.Playback("FE_CoinAddSpin");
        flyAnim.SetActive(true);
        DelayCallback.Delay(this, 1.5f, () =>
        {
            spinLoopAnim.Play(HIDE);
        });
        DelayCallback.Delay(this, 2.5f, () =>
        {
            flyAnim.SetActive(false);
        });
    }
    

}
