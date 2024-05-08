using UnityEngine;
using SlotGame.Symbol;
using UnityEngine.Rendering;
using System.Collections;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using UniversalModule.DelaySystem;

public class WildSymbol : CommonSymbol {
    private int multiplier = 1;
    //public SortingGroup multiplierTexSorting;
    public SpriteRenderer multiplierTex;
    public ArtText multiplierTxt;
    public SortingGroup mulChangeAnimSorting;
    public GameObject mulChangeAnim;
    public ArtText mulChangeTxt1;
    public ArtText mulChangeTxt2;
    public SortingGroup mulWinAnimSorting;
    public GameObject mulWinAnim;
    public ArtText mulWinTex;
    public SortingGroup mulHitAnimSorting;
    public GameObject mulHitAnim;

    #region 动画Hash
    protected int STOP = Animator.StringToHash("Stop");
    #endregion

    public int Multiplier
    {
        get { return multiplier; }
        set
        {
            multiplier = value;
            multiplierTxt.SetContent("*"+value.ToString());
            if (value > 1)
            {
                multiplierTex.enabled = true;
                if (value > 9)
                {
                    multiplierTex.transform.localPosition = Vector3.zero;
                    multiplierTex.transform.localScale = Vector3.one * 0.85f;
                }
                else
                {
                    multiplierTex.transform.localPosition = new Vector3(-0.03f, -0.07f);
                    multiplierTex.transform.localScale = Vector3.one;
                }
            }
            else
            {
                multiplierTex.enabled = false;
            }
        }
    }

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101) {
        base.Install(parent, localPos, reelID, sortingOrder + 10);
        Multiplier = 1;
    }

    public void UpSortingOrder(bool isUpgrade = true)
    {
        SetSortingOrder(DefaultSortingOrder + (isUpgrade ? 550 : 0));
    }

    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        multiplierTex.sortingOrder = order + 1;
        mulChangeAnimSorting.sortingOrder = order + 2;
        mulHitAnimSorting.sortingOrder = order + 2;
        mulWinAnimSorting.sortingOrder = order + 103;
    }

    public override void SetMaskMode(SpriteMaskInteraction interaction)
    {
        base.SetMaskMode(interaction);
        multiplierTex.maskInteraction = interaction;
    }

    public override void PlayIdleAnim()
    {
        if (multiplier > 1)
        {
            multiplierTex.enabled = true;
        }
        else
        {
            multiplierTex.enabled = false;
        }
        mulWinAnim.SetActive(false);
        //SetSortingOrder(DefaultSortingOrder);
        base.PlayIdleAnim();
    }

    public override void PlayAwardAnim()
    {
        multiplierTex.enabled = false;
        mulWinTex.SetContent("*" + multiplier.ToString());
        mulWinTex.GetComponent<SpriteRenderer>().enabled = multiplier != 1;
        mulWinAnim.SetActive(true);
        SetSortingOrder(DefaultSortingOrder + 550);
        base.PlayAwardAnim();
    }

    public void PlayStopAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 550);
        PlayAnimation(STOP);
    }

    public void PlayMultiplierChangeAnim(int addMultiplier)
    {
        StartCoroutine(_PlayMultiplierChangeAnim(addMultiplier));
    }

    IEnumerator _PlayMultiplierChangeAnim(int addMultiplier)
    {
        mulChangeTxt1.SetContent("*" + multiplier.ToString());
        if (multiplier > 9)
        {
            mulChangeTxt1.transform.localPosition = Vector3.zero;
            mulChangeTxt1.transform.localScale = Vector3.one * 0.85f;
        }
        else
        {
            mulChangeTxt1.transform.localPosition = new Vector3(-0.03f, -0.07f);
            mulChangeTxt1.transform.localScale = Vector3.one;
        }
        mulChangeTxt1.GetComponent<SpriteRenderer>().enabled = multiplier != 1;
        mulChangeTxt2.SetContent("*" + (multiplier+addMultiplier).ToString());
        if (multiplier + addMultiplier > 9)
        {
            mulChangeTxt2.transform.localPosition = Vector3.zero;
            mulChangeTxt2.transform.localScale = Vector3.one * 0.85f;
        }
        else
        {
            mulChangeTxt2.transform.localPosition = new Vector3(-0.03f, -0.07f);
            mulChangeTxt2.transform.localScale = Vector3.one;
        }
        mulChangeAnim.SetActive(true);
        yield return new WaitForSeconds(10 / 30f);
        Multiplier += addMultiplier;
        yield return new WaitForSeconds(1.2f);
        mulChangeAnim.SetActive(false);
    }

    public void PlayMultiplierHitAnim()
    {
        mulHitAnim.SetActive(false);
        mulHitAnim.SetActive(true);
        DelayCallback.Delay(this, 1f, delegate ()
        {
            mulHitAnim.SetActive(false);
        });
    }

    public override void Recycle()
    {
        base.Recycle();
    }

}
