using DG.Tweening;
using Newtonsoft.Json.Linq;
using SlotGame.Core;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;

public class CattleUI : MonoBehaviour
{
    bool isBigCattle = false;
    bool isCattleLoop;
    public Animator cattleAnim;
    public Animator purseAnim;
    private int level;

    public ParticleSystem upperLoop;
    public ParticleSystem upperTrigger;

    public AnimationCurve purseShake;

    public Vector3[] cattlePos;
    public GameObject bigCattleAnim;
    public GameObject baseAddCattleAnim;
    public GameObject bonusAddCattleAnim;
    public SpriteRenderer[] cattleTex;
    public SpriteRenderer[] bigCattleTex;

    Coroutine loopCoroutine;


    public int Level
    {
        get { return level; }
        set
        {
            value = Mathf.Clamp(value, 1, 4);
            level = value;
        }
    }

    #region ¶¯»­Hash
    int IDLE = Animator.StringToHash("Idle");
    int CHANGE = Animator.StringToHash("Change");
    int HIT = Animator.StringToHash("Hit");
    int LOOP = Animator.StringToHash("Loop");
    int LOOP2 = Animator.StringToHash("Loop2");
    int TRIGGER = Animator.StringToHash("Trigger");
    int CHANGE01 = Animator.StringToHash("Change01");
    int CHANGE02 = Animator.StringToHash("Change02");
    int CHANGE03 = Animator.StringToHash("Change03");
    int CHANGE04 = Animator.StringToHash("Change04");
    int HIT01 = Animator.StringToHash("Hit01");
    int HIT02 = Animator.StringToHash("Hit02");
    int HIT03 = Animator.StringToHash("Hit03");
    int HIT04 = Animator.StringToHash("Hit04");
    int LOOP01 = Animator.StringToHash("Loop01");
    int LOOP02 = Animator.StringToHash("Loop02");
    int LOOP03 = Animator.StringToHash("Loop03");
    int LOOP04 = Animator.StringToHash("Loop04");
    int INTRO = Animator.StringToHash("Intro");
    #endregion


    public void Initialize()
    {
        level = 1;
        isCattleLoop = false;
        foreach(var item in bigCattleTex)
        {
            item.color = new Color(1, 1, 1, 0);
        }
        PlayCattleLoopAnim();
        PlayPurseLoopAnim();
        GlobalObserver.OnResetCattleEvent += ResetCattle;
    }

    public void PlayCattleLoopAnim()
    {
        //cattleAnim.Play(LOOP);
        if (!isCattleLoop)
        {
            isCattleLoop = true;
            if (loopCoroutine != null)
            {
                StopCoroutine(loopCoroutine);
            }
            loopCoroutine = StartCoroutine(_PlayCattleLoopAnim());
        }
    }

    IEnumerator _PlayCattleLoopAnim()
    {
        cattleAnim.Play(LOOP2);
        yield return new WaitForSeconds(Random.Range(0, 3) * (69 / 30f));
        cattleAnim.Play(LOOP);
        yield return new WaitForSeconds(69 / 30f);
        PlayCattleLoopAnim();
    }


    public void PlayPurseLoopAnim()
    {
        var emission = upperLoop.emission;
        emission.rateOverTime = new ParticleSystem.MinMaxCurve(10.0f * Mathf.Clamp(level, 1, 3));
        purseAnim.Play(Animator.StringToHash("Loop0" + level.ToString()));
    }
    public void PlayCattleHitAnim()
    {
        isCattleLoop = false;
        StopCoroutine(loopCoroutine);
        cattleAnim.Play(HIT);
    }
    public void PlayPurseHitAnim()
    {
        AudioManager.Playback("FE_BagHit");
        purseAnim.Play(Animator.StringToHash("Hit0" + level.ToString()));
    }
    public void PlayCattleChangeAnim()
    {
        isCattleLoop = false;
        StopCoroutine(loopCoroutine);
        cattleAnim.Play(CHANGE);
    }
    public void PlayPurseChangeAnim()
    {
        AudioManager.Playback("FE_BagChange0" + level.ToString());
        purseAnim.Play(Animator.StringToHash("Change0" + level.ToString()));
    }
    public float PlayTriggerAnim(bool isDisplayAnim = true)
    {
        if (isDisplayAnim)
        {
            StartCoroutine(_PlayTriggerAnim());
        }
        float time = (37 / 30) * (4 - level) + 2;
        return time;
    }
    IEnumerator _PlayTriggerAnim()
    {
        isCattleLoop = false;
        StopCoroutine(loopCoroutine);
        cattleAnim.Play(TRIGGER);
        for (int i = level; i <= 4; i++)
        {
            AudioManager.Playback("FE_BagChange0" + level.ToString());
            purseAnim.Play(Animator.StringToHash("Change0" + level.ToString()));
            yield return new WaitForSeconds(37 / 30f);
            level++;
        }
        level = 4;
        yield return new WaitForSeconds(23 / 30f);
        AudioManager.Playback("FeatureSelect");
        upperTrigger.Play();
        cattleAnim.transform.DOMove(cattlePos[1], 7 / 30f);
        AudioManager.LoopPlayback("FE_BagTrigger");
        purseAnim.Play(TRIGGER);
    }

    public void PlayCattleTriggerAnim()
    {
        isCattleLoop = false;
        StopCoroutine(loopCoroutine);
        cattleAnim.Play(TRIGGER);
    }

    public void PlayPurseShakeAnim()
    {
        purseAnim.transform.DOShakePosition(2f, Vector3.up * 0.3f, 7, 0, false, false).SetEase(purseShake);
    }

    public void PlayPurseIntroAnim()
    {
        cattleAnim.transform.DOMove(cattlePos[2], 10 / 30f);
        purseAnim.Play(INTRO);
    }

    public void DisplayBigCattle(bool isDisplay)
    {
        if (isDisplay)
        {
            if (!isBigCattle)
            {
                isBigCattle = true;
                StopCoroutine(loopCoroutine);
                foreach(var item in cattleTex)
                {
                    item.DOFade(0, 6 / 30f);
                }
                bigCattleAnim.SetActive(true);
                foreach (var item in bigCattleTex)
                {
                    item.DOFade(1, 6 / 30f);
                }
            }
        }
        else
        {
            if (isBigCattle)
            {
                isBigCattle = false;
                foreach (var item in cattleTex)
                {
                    item.DOFade(1, 6 / 30f);
                }
                PlayCattleLoopAnim();
                foreach (var item in bigCattleTex)
                {
                    item.DOFade(0, 6 / 30f).OnComplete(() =>
                    {
                        bigCattleAnim.SetActive(false);
                    });
                }
                /*cattleAnim.transform.DOMove(new Vector3(0, 13.65f), 0.5f).From();
                bigCattleAnim.transform.DOMove(new Vector3(0, -3.5f), 0.5f).OnComplete(() =>
                {
                    bigCattleAnim.gameObject.SetActive(false);
                });*/
            }
        }
    }

    public void PlayBaseAddCattleAnim()
    {
        StartCoroutine(_PlayBaseAddCattleAnim());
    }
    public IEnumerator _PlayBaseAddCattleAnim()
    {
        baseAddCattleAnim.SetActive(true);
        cattleAnim.gameObject.SetActive(false);
        yield return new WaitForSeconds(30 / 30f);
        AudioManager.Playback("FE_SpecialBack");
        yield return new WaitForSeconds(61 / 30f);
        baseAddCattleAnim.SetActive(false);
        cattleAnim.gameObject.SetActive(true);
        PlayCattleLoopAnim();
    }

    public void PlayBonusAddCattleAnim()
    {
        StartCoroutine(_PlayBonusAddCattleAnim());
    }
    public IEnumerator _PlayBonusAddCattleAnim()
    {
        bonusAddCattleAnim.SetActive(true);
        cattleAnim.gameObject.SetActive(false);
        yield return new WaitForSeconds(30 / 30f);
        AudioManager.Playback("FE_SpecialBack");
        yield return new WaitForSeconds(61 / 30f);
        bonusAddCattleAnim.SetActive(false);
        cattleAnim.gameObject.SetActive(true);
        PlayCattleLoopAnim();
    }
    

    public void ResetCattle()
    {
        level = 1;
        PlayCattleLoopAnim();
        PlayPurseLoopAnim();
    }

    public void BonusToBase()
    {
        ResetCattle();
        cattleAnim.transform.position = cattlePos[0];
    }

    public void BaseToBonus()
    {
        isBigCattle = false;
        PlayCattleLoopAnim();
    }

    public void BonusIntroOver()
    {
        isBigCattle = false;
        PlayCattleLoopAnim();
    }


}
