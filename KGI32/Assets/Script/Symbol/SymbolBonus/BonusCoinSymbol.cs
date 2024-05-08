using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class BonusCoinSymbol : CoinSymbol
{
    public SortingGroup[] glowSorting;
    public SpriteRenderer[] glowSpriteRenderers;
    public ParticleSystemRenderer[] glowParticleRenderer;
    public GameObject[] glowParticles;
    public GameObject[] addCreditAnim;
    public SortingGroup scoreAnimSorting;
    public ArtText[] scoreAnimText;
    public bool isGrand = false;
    public bool isMajor = false;
    public SpriteRenderer[] scaleGlow;
    public SortingGroup scaleGlowSorting;


    public override int Level
    {
        get { return level; }
        set
        {
            level = value;
            MainTex.sprite = coinSpriteByLevel[value - 1];
            for (int i = 0; i < glowParticles.Length; i++)
            {
                if (i == value - 1)
                {
                    glowParticles[i].SetActive(true);
                }
                else
                {
                    glowParticles[i].SetActive(false);
                }
            }
        }
    }

    #region 动画Hash
    protected int CHANGE01 = Animator.StringToHash("Change01");
    protected int CHANGE02 = Animator.StringToHash("Change02");
    protected int GOLDSTOP = Animator.StringToHash("GoldStop");
    protected int MIXSTOP = Animator.StringToHash("MixStop");
    protected int SILVERSTOP = Animator.StringToHash("SilverStop");
    protected int GOLDGLOW = Animator.StringToHash("GoldGlow");
    protected int MIXGLOW = Animator.StringToHash("MixGlow");
    protected int SILVERGLOW = Animator.StringToHash("SilverGlow");
    protected int GOLDFLYHIT = Animator.StringToHash("GoldFlyHit");
    protected int MIXFLYHIT = Animator.StringToHash("MixFlyHit");
    protected int SILVERFLYHIT = Animator.StringToHash("SilverFlyHit");
    #endregion

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder);
        MainTex.enabled = true;
        MainTex.color = Color.white;
        transform.localScale = Vector3.one;
        foreach(var item in scaleGlow)
        {
            item.color = new Color(1, 1, 1, 0);
        }
    }

    public void UpSortingOrder()
    {
        SetSortingOrder(DefaultSortingOrder + 500);
    }

    public override void PlayJackpotCeleAnim()
    {
        glowParticles[level-1].SetActive(false);
        base.PlayJackpotCeleAnim();
    }


    public override void PlayGlow()
    {
        base.PlayGlow();
        glowParticles[level - 1].SetActive(true);
    }

    /// <summary>
    /// 播放奖励动画
    /// </summary>
    public override void PlayWinAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 500);
        switch (Level)
        {
            case 1:
                PlayAnimation(SILVERWIN);
                break;
            case 2:
                PlayAnimation(MIXWIN);
                break;
            case 3:
                PlayAnimation(GOLDWIN);
                break;
        }
    }

    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        foreach (var item in glowSpriteRenderers)
        {
            item.sortingOrder = order + 2;
        }
        foreach (var item in glowSorting)
        {
            item.sortingOrder = order + 4;
        }
        scaleGlowSorting.sortingOrder = order + 3;
        scoreAnimSorting.sortingOrder = order + 5;
    }

    public override void SetMaskMode(SpriteMaskInteraction interaction)
    {
        base.SetMaskMode(interaction);
        foreach (var item in glowSpriteRenderers)
        {
            item.maskInteraction = interaction;
        }
        foreach (var item in glowParticleRenderer)
        {
            item.maskInteraction = interaction;
        }
        foreach(var item in scaleGlow)
        {
            item.maskInteraction = interaction;
        }
    }

    public override void SetDisplay(bool isDisplay)
    {
        base.SetDisplay(isDisplay);
        glowParticles[level - 1].SetActive(isDisplay);
    }

    public void PlayChangeAnim()
    {
        StartCoroutine(_PlayChangeAnim());
    }
    IEnumerator _PlayChangeAnim()
    {
        AudioManager.Playback("FE_CoinUpgrade");
        SetSortingOrder(DefaultSortingOrder + 900);
        PlayAnimation(Animator.StringToHash("Change0" + Level.ToString()));
        AnimSpriteRender.sortingOrder += 5;
        yield return new WaitForSeconds(10 / 30f);
        MainTex.enabled = false;
        glowParticles[level - 1].SetActive(false);
        yield return new WaitForSeconds(10 / 30f);
        transform.DOScale(Vector3.one, 7 / 30f);
        DisplayGlow(false);
        yield return new WaitForSeconds(5 / 30f);
        AnimSpriteRender.sortingOrder -= 5;
        yield return new WaitForSeconds(5 / 30f);
        Level++;
        MainTex.enabled = true;
        yield return new WaitForSeconds(12 / 30f);
        SetSortingOrder(DefaultSortingOrder + 500);
    }

    public void DisplayGlow(bool display)
    {
        if (display)
        {
            scaleGlow[level - 1].DOFade(1, 7 / 30f);
        }
        else
        {
            foreach(var item in scaleGlow)
            {
                item.DOFade(0, 7 / 30f);
            }
        }
    }

    public override void PlayStopAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 500);
        switch (Level)
        {
            case 1:
                AudioManager.Playback("FE_CoinStop");
                PlayAnimation(SILVERSTOP);
                break;
            case 2:
                AudioManager.Playback("FE_CoinStop");
                PlayAnimation(MIXSTOP);
                break;
            case 3:
                AudioManager.Playback("FE_CoinStop special");
                PlayAnimation(GOLDSTOP);
                break;
        }
    }

    public void PlayUpgradeStopAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 500);
        switch (Level)
        {
            case 1:
                AudioManager.PlayOneShot("FE_CoinStop");
                PlayAnimation(SILVERSTOP);
                break;
            case 2:
                AudioManager.PlayOneShot("FE_CoinStop");
                PlayAnimation(MIXSTOP);
                break;
            case 3:
                AudioManager.PlayOneShot("FE_CoinStop special");
                PlayAnimation(GOLDSTOP);
                break;
        }
    }

    public void PlayGlowAnim()
    {
        //addCreditAnim[level - 1].SetActive(true);
        switch (Level)
        {
            case 1:
                PlayAnimation(SILVERGLOW);
                break;
            case 2:
                PlayAnimation(MIXGLOW);
                break;
            case 3:
                PlayAnimation(GOLDGLOW);
                break;
        }
    }

    public void PlayFlyHitAnim()
    {
        switch (Level)
        {
            case 1:
                PlayAnimation(SILVERFLYHIT);
                break;
            case 2:
                PlayAnimation(MIXFLYHIT);
                break;
            case 3:
                PlayAnimation(GOLDFLYHIT);
                break;
        }
    }

    public override void SetScore(int score)
    {
        base.SetScore(score);
        /*foreach(var item in scoreAnimText)
        {
            item.SetContent(score.ToString());
        }
        ScoreValue = score;*/
    }

    public override void Recycle()
    {
        isGrand = false;
        isMajor = false;
        foreach (var item in addCreditAnim)
        {
            item.SetActive(false);
        }
        base.Recycle();
    }

    public void FlyChange()
    {
        for (int i = 0; i < glowParticles.Length; i++)
        {
            glowParticles[i].SetActive(false);
        }
    }

}
