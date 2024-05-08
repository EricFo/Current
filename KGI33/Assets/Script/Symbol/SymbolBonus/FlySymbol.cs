using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Symbol;
using DG.Tweening;
using UniversalModule.DelaySystem;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

public class FlySymbol : CommonSymbol
{
    public GameObject collectFlyAnim;
    public GameObject collectFlyAnim2;
    public GameObject collectGlowAnim;
    public GameObject collectGoldenFlyAnim;
    public GameObject collectGoldenHitAnim;


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

    public float PlayCollectFlyAnim(int coinLevel, Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(_PlayCollectFlyAnim(coinLevel, startPos, targetPos));
        if (Vector3.Distance(startPos, targetPos) > 3.53f)
        {
            return 18 / 30f;
        }
        else
        {
            return 10 / 30f;
        }
    }

    IEnumerator _PlayCollectFlyAnim(int coinLevel, Vector3 startPos, Vector3 targetPos)
    {
        transform.position = startPos;
        MainTex.enabled = false;
        //collectFlyAnim.SetActive(true);
        if (coinLevel == 3)
        {
            collectGlowAnim.SetActive(true);
            yield return new WaitForSeconds(31 / 30f);
            collectGoldenFlyAnim.SetActive(true);
            transform.DOMove(targetPos, 17 / 30f);
            yield return new WaitForSeconds(17 / 30f);
            collectGoldenHitAnim.SetActive(true);
            yield return new WaitForSeconds(30 / 30f);
        }
        else
        {
            if (Vector3.Distance(startPos, targetPos) > 3.53f)
            {
                collectFlyAnim.SetActive(true);
                transform.DOMove(targetPos, 18 / 30f);
                yield return new WaitForSeconds(2f);
            }
            else
            {
                collectFlyAnim2.SetActive(true);
                transform.DOMove(targetPos, 10 / 30f);
                yield return new WaitForSeconds(2f);
            }
        }
        MainTex.enabled = true;
        Recycle();
    }

    public override void Recycle()
    {
        SetSortingOrder(2100);
        collectFlyAnim.SetActive(false);
        collectFlyAnim2.SetActive(false);
        collectGlowAnim.SetActive(false);
        collectGoldenFlyAnim.SetActive(false);
        collectGoldenHitAnim.SetActive(false);
        base.Recycle();
    }

}
