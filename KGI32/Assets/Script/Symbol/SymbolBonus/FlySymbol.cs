using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Symbol;
using DG.Tweening;
using UniversalModule.DelaySystem;

public class FlySymbol : CommonSymbol
{

    #region ¶¯»­Hash
    protected int COINFLY = Animator.StringToHash("CoinFly");
    protected int GOLDFLYUP = Animator.StringToHash("GoldFlyUp");
    protected int GOLDFLYDOWN = Animator.StringToHash("GoldFlyDown");
    protected int MIXFLYUP = Animator.StringToHash("MixFlyUp");
    protected int MIXFLYDOWN = Animator.StringToHash("MixFlyDown");
    protected int SILVERFLYUP = Animator.StringToHash("SilverFlyUp");
    protected int SILVERFLYDOWN = Animator.StringToHash("SilverFlyDown");
    #endregion

    public void PlayCoinFlyAnim(Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        PlayAnimation(COINFLY);
        transform.DOMove(targetPos, 20 / 30f);
        DelayCallback.Delay(this, 20 / 30f, () =>
        {
            Recycle();
        });
    }
    public void PlayCoinUpAnim(int coinLevel, Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        startPos.z = 0;
        targetPos.z = 0;
        Vector3 dir = (targetPos - startPos).normalized;
        transform.up = dir;
        switch (coinLevel)
        {
            case 1:
                PlayAnimation(SILVERFLYUP);
                break;
            case 2:
                PlayAnimation(MIXFLYUP);
                break;
            case 3:
                PlayAnimation(GOLDFLYUP);
                break;
        }
        transform.DOMove(targetPos, 14 / 30f);
        DelayCallback.Delay(this, 15 / 30f, () =>
        {
            Recycle();
        });
    }

    public void PlayCoinDownAnim(int coinLevel, Vector3 targetPos)
    {
        switch (coinLevel)
        {
            case 1:
                PlayAnimation(SILVERFLYUP);
                break;
            case 2:
                PlayAnimation(MIXFLYUP);
                break;
            case 3:
                PlayAnimation(GOLDFLYUP);
                break;
        }
        transform.DOMove(targetPos, 20 / 30f);
        DelayCallback.Delay(this, 23 / 30f, () =>
        {
            Recycle();
        });
    }

    public override void Recycle()
    {
        SetSortingOrder(2100);
        base.Recycle();
    }

}
