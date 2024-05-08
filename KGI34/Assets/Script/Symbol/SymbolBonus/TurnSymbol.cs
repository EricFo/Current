using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Core;
using SlotGame.Symbol;
using System;
using UniversalModule.DelaySystem;
using DG.Tweening;
using UnityEngine.Rendering;
using UniversalModule.AudioSystem;

public class TurnSymbol : CommonSymbol
{
    public ArtText Info;
    public SpriteRenderer InfoRender;
    public CircleCollider2D _symbolCollider;
    public SpriteRenderer[] addSpinTex;
    public Sprite[] addSpinSprites;

    public Animator turnSymbolAnim;
    public Animator optionAnim;
    public ParticleSystem selectParticle;

    public GameObject transformAnim;
    public GameObject addSpinAnim;
    public GameObject addSpinFlyAnim;
    public GameObject startGameAnim;
    public GameObject goldCoinAnim;
    public GameObject mixCoinAnim;
    public GameObject silverCoinAnim;
    public GameObject creditAnim;

    public GameObject CreditHitAnim;

    public SortingGroup selectSorting;

    public Action<TurnSymbol> OnTurnOver;
    public event Action<TurnSymbol> OnTurnOverComplete
    {
        add
        {
            OnTurnOver -= value;
            OnTurnOver += value;
        }
        remove
        {
            OnTurnOver -= value;
        }
    }

    #region 动画Hash
    int LOOP = Animator.StringToHash("Loop");
    int SELECT = Animator.StringToHash("Select");
    int ADDSPIN = Animator.StringToHash("AddSpin");
    int START = Animator.StringToHash("Start");
    int INTRO = Animator.StringToHash("Intro");
    #endregion



    void Awake()
    {
        _symbolCollider = GetComponent<CircleCollider2D>();
    }


    public void OnMouseDown()
    {
        OnTurnOver.Invoke(this);
    }

    public void SetInfo(string info)
    {
        InfoRender.enabled = true;
        Info.SetContent(info);
    }


    public void ResetCoin()
    {
        MainTex.color = Color.white;
        InfoRender.enabled = false;
        transformAnim.SetActive(false);
        addSpinAnim.SetActive(false);
        addSpinFlyAnim.SetActive(false);
        addSpinFlyAnim.transform.localPosition = Vector3.zero;
        startGameAnim.SetActive(false);
        goldCoinAnim.SetActive(false);
        mixCoinAnim.SetActive(false);
        silverCoinAnim.SetActive(false);
        creditAnim.SetActive(false);
    }

    public void PlaySelectAnim()
    {
        transformAnim.SetActive(true);
        turnSymbolAnim.Play(SELECT);
        selectParticle.Play();
    }

    public void TransitionAnim()
    {
        StartCoroutine(_TransitionAnim());
    }
    IEnumerator _TransitionAnim()
    {
        turnSymbolAnim.Play(SELECT);
        yield return new WaitForSeconds(18 / 30f);
        MainTex.DOFade(0, 7 / 30f);
    }

    public void PlayAddSpinAnim(int count)
    {
        foreach(var item in addSpinTex)
        {
            item.sprite = addSpinSprites[count - 1];
        }
        addSpinAnim.SetActive(true);
    }
    public void PlayAddSpinFlyAnim()
    {
        addSpinFlyAnim.SetActive(true);
        addSpinFlyAnim.transform.DOMove(new Vector3(6.4f, 0.5f), 16 / 30f);
    }
    public void PlayStartAnim()
    {
        startGameAnim.SetActive(true);
    }
    public void PlayCoinAnim(int coinLevel)
    {
        switch (coinLevel)
        {
            case 1:
                silverCoinAnim.SetActive(true);
                break;
            case 2:
                mixCoinAnim.SetActive(true);
                break;
            case 3:
                goldCoinAnim.SetActive(true);
                break;
        }
    }

    public IEnumerator PlayCreditAnim(int score)
    {
        creditAnim.GetComponent<ArtText>().SetContent(score.ToString());
        creditAnim.SetActive(true);
        yield return new WaitForSeconds(18 / 30f);
        AudioManager.Playback("FE_BagPickCREDIT");
        yield return new WaitForSeconds(14 / 30f);
        yield return new WaitForSeconds(15 / 30f);
        selectSorting.sortingOrder = 11000;
        creditAnim.transform.DOMove(new Vector3(0, -8.3f), 9 / 30f);
        creditAnim.transform.DOScale(Vector3.one * 0.3f, 9 / 30f);
        yield return new WaitForSeconds(9 / 30f);
        CreditHitAnim.SetActive(false);
        CreditHitAnim.SetActive(true);
        creditAnim.transform.localPosition = new Vector3(0, -0.15f);
        creditAnim.transform.localScale = Vector3.one;
        selectSorting.sortingOrder = 1800;
    }

    public void PlayIntroAnim()
    {
        turnSymbolAnim.Play(INTRO);
    }

}
