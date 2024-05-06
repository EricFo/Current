using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.DelaySystem;
using DG.Tweening;
using UniversalModule.AudioSystem;
using SlotGame.Core;
using Script.BonusGame.GamePlay.Wheel;

public class TurntableUI : MonoBehaviour {
    void Awake()
    {
        GlobalObserver.OnBetChangeEvent += OnBetChange;
    }

    private Vector3 BoostFlyStartPos;
    private Vector3 GetallFlyStartPos;

    //public Vector3 SinglePos;
    //public Vector3 ResetPos;

    private int BoostLevel = 1;
    private int GetallLevel = 1;

    public Wheel BoostWheel;
    public Wheel GetallWheel;

    public SpriteRenderer boostMask;
    public SpriteRenderer getallMask;


    public Transform BoostTrans;
    public Transform GetallTrans;

    public SpriteRenderer BoostAnimSR;
    public SpriteRenderer GetallAnimSR;
    public SpriteRenderer BoostFlySprite;
    public SpriteRenderer GetallFlySprite;

    public Animator BoostAnim;
    public Animator GetallAnim;
    public Animator BoostLoop;
    public Animator GetallLoop;
    public Animator BoostFly;
    public Animator GetallFly;

    public SpriteRenderer[] TurntableBoost;
    public SpriteRenderer[] TurntableGetall;
    public Sprite[] BaseBoostSprite;
    public Sprite[] BaseGetallSprite;
    public Sprite[] BaseDoubleWheelSprite;
    public Sprite[] BaseMultiplierWheelSprite;
    public Sprite[] BonusBoostSprite;
    public Sprite[] BonusGetallSprite;
    public Sprite[] BonusDoubleWheelSprite;
    public Sprite[] BonusMultiplierWheelSprite;

    public Material DefaultMaterial;
    public Material AddMaterial;

    public GameObject DouCreditFly;
    public GameObject MulCreditFly;

    public Dictionary<int, Vector3> baseScale = new Dictionary<int, Vector3>()
    {
        {1,Vector3.one*0.485f},
        {2,Vector3.one*0.66f},
        {3,Vector3.one*0.84f},
        {4,Vector3.one*0.94f}
    };
    public Dictionary<int, Vector3> bonusScale = new Dictionary<int, Vector3>()
    {
        {1,Vector3.one*0.235f},
        {2,Vector3.one*0.322f},
        {3,Vector3.one*0.41f},
        {4,Vector3.one*0.458f}
    };

    int IDLE = Animator.StringToHash("Idle");
    int CHANGE = Animator.StringToHash("Change");
    int TRIGGER = Animator.StringToHash("Trigger");
    int TURN = Animator.StringToHash("Turn");
    int WIN = Animator.StringToHash("Win");
    int WIN2 = Animator.StringToHash("Win2");
    int HIT = Animator.StringToHash("Hit");
    int GRAND = Animator.StringToHash("Grand");
    int MAJOR = Animator.StringToHash("Major");
    int MINI = Animator.StringToHash("Mini");
    int BOOSTFLY01 = Animator.StringToHash("BoostFly01");
    int BOOSTFLY02 = Animator.StringToHash("BoostFly02");
    int BOOSTFLY03 = Animator.StringToHash("BoostFly03");
    int GETALLFLY01 = Animator.StringToHash("GetallFly01");
    int GETALLFLY02 = Animator.StringToHash("GetallFly02");
    int GETALLFLY03 = Animator.StringToHash("GetallFly03");

    public int TurntableBoostLevel {
        get { return BoostLevel; }
        set {
            value = Mathf.Clamp(value, 1, 4);
            if (value > BoostLevel) {
                PlayChangeAnimation(true);
            }
            BoostLevel = value;
            if (BoostLoop != null && BoostLoop.isActiveAndEnabled) {
                BoostLoop.Play(Animator.StringToHash("Loop" + BoostLevel.ToString()), 0, 0);
            }
            BoostTrans.localScale = baseScale[BoostLevel];
        }
    }

    public int TurntableGetallLevel {
        get { return GetallLevel; }
        set {
            value = Mathf.Clamp(value, 1, 4);
            if (value > GetallLevel) {
                PlayChangeAnimation(false);
            }
            GetallLevel = value;
            if (GetallLoop != null && GetallLoop.isActiveAndEnabled) {
                GetallLoop.Play(Animator.StringToHash("Loop" + GetallLevel.ToString()), 0, 0);
            }
            GetallTrans.localScale = baseScale[GetallLevel];
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animHashCode">动画哈希值</param>
    protected void PlayBoostAnimation(int animHashCode) {
        if (BoostAnim != null && BoostAnim.isActiveAndEnabled) {
            BoostAnim.Play(animHashCode, 0, 0);
        }
    }
    protected void PlayGetallAnimation(int animHashCode) {
        if (GetallAnim != null && GetallAnim.isActiveAndEnabled) {
            GetallAnim.Play(animHashCode, 0, 0);
        }
    }

    public void PlayLoopIdleAnim() {
        BoostLoop.Play(IDLE);
        GetallLoop.Play(IDLE);
    }
    public void PlayFlyIdleAnim(bool isBoost)
    {
        if (isBoost)
        {
            BoostFly.Play(IDLE);
        }
        else
        {
            GetallFly.Play(IDLE);
        }
    }
    public void PlayIdleAnimation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(IDLE);
        } else {
            PlayGetallAnimation(IDLE);
        }
    }
    public void PlayIdleAnimation() {
        PlayBoostAnimation(IDLE);
        PlayGetallAnimation(IDLE);
    }
    public void PlayChangeAnimation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(CHANGE);
        } else {
            PlayGetallAnimation(CHANGE);
        }
        AudioManager.Stop("FE_JP_change");
        AudioManager.Playback("FE_JP_change");
    }
    public void PlayTriggerAnimation(bool isBoost) {
        if (isBoost) {
            BoostTrans.localScale = baseScale[4];
            PlayBoostAnimation(TRIGGER);
        } else {
            GetallTrans.localScale = baseScale[4];
            PlayGetallAnimation(TRIGGER);
        }
    }
    public void PlayTurnAnimation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(TURN);
        } else {
              PlayGetallAnimation(TURN);
        }
    }
    public void PlayWinAnimation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(WIN);
        } else {
            PlayGetallAnimation(WIN);
        }
    }
    public void PlayWinAnimation()
    {
        PlayBoostAnimation(WIN);
        PlayGetallAnimation(WIN);
    }
    public void PlayWin2Animation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(WIN2);
        } else {
            PlayGetallAnimation(WIN2);
        }
    }
    public void PlayHitAnimation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(HIT);
        } else {
            PlayGetallAnimation(HIT);
        }
        AudioManager.Playback("FE_JP_hit");
    }
    public void PlayGrandAnimation(bool isBoost, Vector3 target) {
        if (isBoost) {

        } else {

        }
    }

    public void PlayFeatureFlyAnim(bool isBoost)
    {
        if (isBoost)
        {
            BoostFlyStartPos = BoostFly.transform.position;
            BoostFlySprite.enabled = true;
            BoostFly.Play(BOOSTFLY01);
            BoostFly.transform.rotation = Quaternion.identity;
            //BoostFlySprite.material = DefaultMaterial;
            //DelayCallback.Delay(this, 0.43f, () =>
            //{
            //    BoostFly.Play(BOOSTFLY02);
            //    BoostFly.transform.DOMove(target, 0.46f).SetEase(Ease.Linear).OnComplete(() =>
            //    {
            //        BoostFlySprite.material = AddMaterial;
            //        BoostFly.Play(BOOSTFLY03);
            //        int layer = BoostFlySprite.sortingOrder;
            //        //BoostFlySprite.sortingOrder = 80 + id;
            //        DelayCallback.Delay(this, 0.66f, () =>
            //        {
            //            BoostFlySprite.enabled = false;
            //            BoostFlySprite.sortingOrder = layer;
            //            BoostFlySprite.transform.position = BoostFlyStartPos;
            //            PlayIdleAnimation(true);
            //        });
            //    });
            //});
        }
        else
        {
            GetallFlyStartPos = GetallFly.transform.position;
            GetallFlySprite.enabled = true;
            GetallFly.Play(GETALLFLY01);
            GetallFly.transform.rotation = Quaternion.identity;
            //DelayCallback.Delay(this, 0.43f, () =>
            //{
            //    GetallFly.Play(GETALLFLY02);
            //    GetallFly.transform.DOMove(target, 0.46f).SetEase(Ease.Linear).OnComplete(() =>
            //    {
            //        GetallFly.Play(GETALLFLY03);
            //        DelayCallback.Delay(this, 0.66f, () =>
            //        {
            //            GetallFlySprite.enabled = false;
            //            GetallFlySprite.transform.position = GetallFlyStartPos;
            //            PlayIdleAnimation();
            //        });
            //    });
            //});
        }
    }

    public void ToBonusLayout()
    {
        for (int i = 0; i < TurntableBoost.Length; i++)
        {
            TurntableBoost[i].sprite = BonusBoostSprite[i];
            TurntableGetall[i].sprite = BonusGetallSprite[i];
            if(i == 2)
            {
                TurntableBoost[i].sprite = BonusDoubleWheelSprite[GlobalObserver.BetLevel];
                TurntableGetall[i].sprite = BonusMultiplierWheelSprite[GlobalObserver.BetLevel];
            }
        }
        TurntableBoost[0].transform.localPosition = new Vector3(-0.03f, 6.57f);
        TurntableGetall[0].transform.localPosition = new Vector3(0.03f, 6.57f);
        BoostTrans.localScale = Vector3.one*0.458f;
        GetallTrans.localScale = Vector3.one*0.458f;
        AudioManager.PlayOneShot("FE_WheelIntro");
        BoostTrans.DOScale(1f, 0.5f).SetEase(Ease.Linear);
        GetallTrans.DOScale(1f, 0.5f).SetEase(Ease.Linear);
        BoostTrans.position = new Vector3(-3.99f, 3.69f);
        GetallTrans.position = new Vector3(3.99f, 3.69f);
        PlayIdleAnimation();
        //BoostAnimSR.DOFade(0, 0.2f).SetEase(Ease.Linear);
        //BoostAnimSR.DOFade(0, 0.2f).SetEase(Ease.Linear);
        //DelayCallback.Delay(this, 0.2f, () =>
        //{
        //    PlayIdleAnimation();
        //    BoostAnimSR.DOFade(1, 0);
        //    BoostAnimSR.DOFade(1, 0);
        //});
        switch (GlobalObserver.CurrentBonusState)
        {
            case 1:
                BoostTrans.DOLocalMove(new Vector3(0, -0.15f), 0.5f).SetEase(Ease.Linear);
                GetallWheel.Hide();
                GetallTrans.gameObject.SetActive(false);
                DouCreditFly.transform.position = new Vector3(0, -1.9f);
                DouCreditFly.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 2:
                GetallTrans.DOLocalMove(new Vector3(0, -0.15f), 0.5f).SetEase(Ease.Linear);
                BoostWheel.Hide();
                BoostTrans.gameObject.SetActive(false);
                MulCreditFly.transform.position = new Vector3(0, -1.9f);
                MulCreditFly.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case 3:
                BoostTrans.DOLocalMove(new Vector3(-7.93f, 0), 0.5f).SetEase(Ease.Linear);
                BoostTrans.rotation=Quaternion.Euler(new Vector3(0, 0, -45f));
                GetallTrans.DOLocalMove(new Vector3(7.93f, 0), 0.5f).SetEase(Ease.Linear);
                GetallTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 45f));
                DouCreditFly.transform.position = new Vector3(-2f, -2.3f);
                DouCreditFly.transform.eulerAngles = new Vector3(0,0,20f);
                MulCreditFly.transform.position = new Vector3(2f, -2.3f);
                MulCreditFly.transform.eulerAngles = new Vector3(0, 0, -20f);
                break;
        }
        TurntableBoost[0].enabled = true;
        TurntableGetall[0].enabled = true;
    }
    public void ToBaseLayout()
    {
        BoostTrans.gameObject.SetActive(true);
        GetallTrans.gameObject.SetActive(true);
        for (int i = 0; i < TurntableBoost.Length; i++)
        {
            TurntableBoost[i].sprite = BaseBoostSprite[i];
            TurntableGetall[i].sprite = BaseGetallSprite[i];
            if (i == 2)
            {
                TurntableBoost[i].sprite = BaseDoubleWheelSprite[GlobalObserver.BetLevel];
                TurntableGetall[i].sprite = BaseMultiplierWheelSprite[GlobalObserver.BetLevel];
            }
        }
        BoostTrans.position = new Vector3(-3.99f, 3.69f);
        GetallTrans.position = new Vector3(3.99f, 3.69f);
        BoostTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        GetallTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        TurntableBoost[0].enabled = false;
        TurntableGetall[0].enabled = false;
    }
    public void OnBetChange(int betLevel)
    {
        TurntableBoost[2].sprite = BaseDoubleWheelSprite[GlobalObserver.BetLevel];
        TurntableGetall[2].sprite = BaseMultiplierWheelSprite[GlobalObserver.BetLevel];
    }

    /*public void PlayCreditFlyAnim(bool isDouble)
    {
        if (isDouble)
        {
            DouCreditFly.SetActive(true);
            DelayCallback.Delay(this, 20 / 30f, () =>
            {
                DouCreditFly.SetActive(false);
            });
        }
        else
        {
            MulCreditFly.SetActive(true);
            DelayCallback.Delay(this, 20 / 30f, () =>
            {
                MulCreditFly.SetActive(false);
            });
        }
    }*/

}
