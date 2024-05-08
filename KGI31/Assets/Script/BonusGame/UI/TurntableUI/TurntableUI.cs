using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.DelaySystem;
using DG.Tweening;
using UniversalModule.AudioSystem;
using SlotGame.Core;
using Script.BonusGame.GamePlay.Wheel;
using System.Linq;
using UniversalModule.SpawnSystem;

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
    public Animator InitCombo;

    public SpriteRenderer[] TurntableBoost;
    public SpriteRenderer[] TurntableGetall;
    public Sprite[] BaseBoostSprite;
    public Sprite[] BaseGetallSprite;
    public Sprite[] BaseDoubleWheelSprite;
    public Sprite[] BaseMultiplierWheelSprite;
    public Sprite[] FreeBoostSprite;
    public Sprite[] FreeGetallSprite;
    public Sprite[] FreeDoubleWheelSprite;
    public Sprite[] FreeMultiplierWheelSprite;
    public Sprite[] FreeDoubleWheelSprite_Up;
    public Sprite[] FreeMultiplierWheelSprite_Up;

    public Material DefaultMaterial;
    public Material AddMaterial;

    public GameObject DouCreditFly;
    public GameObject MulCreditFly;

    public GameObject[] leftJackpotStars;
    public GameObject[] rightJackpotStars;

    public Animator leftChangeStar;
    public Animator rightChangeStar;

    public Transform leftMessage;
    public Transform rightMessage;
    public Transform leftTrigger;
    public Transform rightTrigger;
    public Vector3[] leftMessagePos;
    public Vector3[] rightMessagePos;

    Tween triggerTween1 = null;
    Tween triggerTween2 = null;

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
    public Dictionary<JackpotType, int> jackpotIndex = new Dictionary<JackpotType, int>()
    {
        {JackpotType.Empty,-1 },
        {JackpotType.Grand,0 },
        {JackpotType.Major,1 },
        {JackpotType.Mini,2 }
    };

    #region 动画Hash
    int IDLE = Animator.StringToHash("Idle");
    int CHANGE = Animator.StringToHash("Change");
    int TRIGGER = Animator.StringToHash("Trigger");
    int TURN = Animator.StringToHash("Turn");
    int WIN = Animator.StringToHash("Win");
    int WIN2 = Animator.StringToHash("Win2");
    int WIN3 = Animator.StringToHash("Win3");
    int WIN4 = Animator.StringToHash("Win4");
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
    int ADD1FLY01 = Animator.StringToHash("Add1Fly01");
    int ADD1FLY02 = Animator.StringToHash("Add1Fly02");
    int ADD2FLY01 = Animator.StringToHash("Add2Fly01");
    int ADD2FLY02 = Animator.StringToHash("Add2Fly02");
    int ADDFLYHIT = Animator.StringToHash("AddFlyHit");
    int WILDFLY01 = Animator.StringToHash("WildFly01");
    int CHANGESTAR = Animator.StringToHash("ChangeStar");
    int INITCOMBO = Animator.StringToHash("InitCombo");
    int INITFREE = Animator.StringToHash("InitFree");
    #endregion

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
            leftMessage.position = leftMessagePos[BoostLevel];
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
            rightMessage.position = rightMessagePos[GetallLevel];
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
            TriggerLeftShake();
            leftMessage.position = leftMessagePos.Last();
        } else {
            GetallTrans.localScale = baseScale[4];
            PlayGetallAnimation(TRIGGER);
            TriggerRightShake();
            rightMessage.position = rightMessagePos.Last();
        }
    }
    public void PlayTriggerInitComboAnim()
    {
        InitCombo.Play(INITCOMBO);
        AudioManager.Playback("FE_WheelUpgrade");
        leftTrigger.DOShakePosition(1f, 0.3f, 20, 90f, false, false);
        rightTrigger.DOShakePosition(1f, 0.3f, 20, 90f, false, false);
    }
    public void PlayTriggerInitFreeAnim()
    {
        InitCombo.Play(INITFREE);
    }

    public void TriggerLeftShake()
    {
        triggerTween1 = BoostTrans.DOShakePosition(10f, 0.25f, 20, 90f, false, false).OnComplete(() =>
        {
            TriggerLeftShake();
        });
    }
    public void TriggerRightShake()
    {
        triggerTween2 = GetallTrans.DOShakePosition(10f, 0.25f, 20, 90f, false, false).OnComplete(() =>
        {
            TriggerRightShake();
        });
    }
    public void DoKillTriggerShake()
    {
        if (triggerTween1 != null)
        {
            triggerTween1.Kill();
        }
        if (triggerTween2 != null)
        {
            triggerTween2.Kill();
        }
    }
    public void PlayTurnAnimation(bool isBoost) {
        if (isBoost) {
            PlayBoostAnimation(TURN);
        } else {
            PlayGetallAnimation(TURN);
        }
    }
    public void PlayWinAnimation(bool isBoost)
    {
        if (isBoost)
        {
            PlayBoostAnimation(WIN);
        }
        else
        {
            PlayGetallAnimation(WIN);
        }
    }
    public void PlayWinAnimation()
    {
        PlayBoostAnimation(WIN);
        PlayGetallAnimation(WIN);
    }
    public void PlayWin2Animation(bool isBoost)
    {
        if (isBoost)
        {
            PlayBoostAnimation(WIN2);
        }
        else
        {
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
    public void PlayChangeStarAnimation(bool isBoost) {
        if (isBoost) {
            leftChangeStar.Play(CHANGESTAR);
        } else {
            rightChangeStar.Play(CHANGESTAR);
        }
    }

    public void PlayWildFlyAnim()
    {
        BoostFly.transform.rotation = Quaternion.identity;
        BoostFly.Play(WILDFLY01);
    }
    public void PlayMultiplierFlyAnim(Vector3 target,bool isAdd1)
    {
        GetallFlyStartPos = GetallFly.transform.position;
        GetallFlySprite.enabled = true;
        GetallFly.Play(isAdd1?ADD1FLY01:ADD2FLY01);
        GetallFly.transform.rotation = Quaternion.identity;
        DelayCallback.Delay(this, 13/30f, () => {
            GetallFly.Play(isAdd1?ADD1FLY02:ADD2FLY02);
            GetallFly.transform.DOMove(target, 14/30f).OnComplete(() => {
                GetallFlySprite.material = AddMaterial;
                GetallFly.Play(ADDFLYHIT);
                //GetallFlySprite.sortingOrder = 400;
                DelayCallback.Delay(this, 20/30f, () => {
                    GetallFlySprite.enabled = false;
                    GetallFlySprite.transform.position = GetallFlyStartPos;
                    GetallFlySprite.material = DefaultMaterial;
                });
            });
        });
    }

    public void PlayFeatureFlyAnim(bool isBoost, Vector3 target,int id) {
        if (isBoost) {
            BoostFlyStartPos = BoostFly.transform.position;
            BoostFlySprite.enabled = true;
            BoostFly.Play(BOOSTFLY01);
            BoostFly.transform.rotation = Quaternion.identity;
            BoostFlySprite.material = DefaultMaterial;
            DelayCallback.Delay(this, 0.43f, () => {
                BoostFly.Play(BOOSTFLY02);
                BoostFly.transform.DOMove(target, 0.46f).SetEase(Ease.Linear).OnComplete(() => {
                    BoostFlySprite.material = AddMaterial;
                    BoostFly.Play(BOOSTFLY03);
                    int layer = BoostFlySprite.sortingOrder;
                    //BoostFlySprite.sortingOrder = 80 + id;
                    DelayCallback.Delay(this, 0.66f, () => {
                        BoostFlySprite.enabled = false;
                        BoostFlySprite.sortingOrder = layer;
                        BoostFlySprite.transform.position = BoostFlyStartPos;
                        PlayIdleAnimation(true);
                    });
                });
            });
        } else {
            GetallFlyStartPos = GetallFly.transform.position;
            GetallFlySprite.enabled = true;
            GetallFly.Play(GETALLFLY01);
            GetallFly.transform.rotation = Quaternion.identity;
            DelayCallback.Delay(this, 0.43f, () => {
                GetallFly.Play(GETALLFLY02);
                GetallFly.transform.DOMove(target, 0.46f).SetEase(Ease.Linear).OnComplete(() => {
                    GetallFly.Play(GETALLFLY03);
                    GetallFlySprite.sortingOrder = 400;
                    DelayCallback.Delay(this, 0.66f, () => {
                        GetallFlySprite.enabled = false;
                        GetallFlySprite.sortingOrder = 1000;
                        GetallFlySprite.transform.position = GetallFlyStartPos;
                        PlayIdleAnimation();
                    });
                });
            });
        }
        AudioManager.Playback("FE_Wheelstop02b");
    }

    public void UpdateGamePlayTex()
    {
        TurntableBoost[2].sprite = FreeDoubleWheelSprite[GlobalObserver.BetLevel];
        TurntableGetall[2].sprite = FreeMultiplierWheelSprite[GlobalObserver.BetLevel];
    }

    /// <summary>
    /// 进入Bonus模式时更新转轮布局
    /// </summary>
    public void ToBonusLayout()
    {
        for (int i = 0; i < TurntableBoost.Length; i++)
        {
            TurntableBoost[i].sprite = FreeBoostSprite[i];
            TurntableGetall[i].sprite = FreeGetallSprite[i];
            if (i == 2)
            {
                if (GlobalObserver.CurrentBonusState == 3)
                {
                    TurntableBoost[i].sprite = FreeDoubleWheelSprite[GlobalObserver.BetLevel];
                    TurntableGetall[i].sprite = FreeMultiplierWheelSprite[GlobalObserver.BetLevel];
                }
                else
                {
                    TurntableBoost[i].sprite = FreeDoubleWheelSprite_Up[GlobalObserver.BetLevel];
                    TurntableGetall[i].sprite = FreeMultiplierWheelSprite_Up[GlobalObserver.BetLevel];
                }
            }
        }
        TurntableBoost[0].transform.localPosition = new Vector3(0f, 9.05f);
        TurntableGetall[0].transform.localPosition = new Vector3(0f, 9.05f);
        BoostTrans.localScale = Vector3.one*0.458f;
        GetallTrans.localScale = Vector3.one*0.458f;
        AudioManager.PlayOneShot("FE_WheelIntro");
        BoostTrans.DOScale(GlobalObserver.CurrentBonusState==3? 0.95f:1f, 0.5f);
        GetallTrans.DOScale(GlobalObserver.CurrentBonusState == 3 ? 0.95f : 1f, 0.5f);
        //BoostTrans.position = new Vector3(-10.75f, 0.04f);
        //GetallTrans.position = new Vector3(-10.75f, 0.04f);
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
                BoostTrans.DOLocalMove(new Vector3(0, -1.04f), 0.5f);
                //GetallWheel.Hide();
                GetallTrans.gameObject.SetActive(false);
                DouCreditFly.transform.position = new Vector3(0, -6.6f);
                DouCreditFly.transform.eulerAngles = Vector3.zero;
                break;
            case 2:
                GetallTrans.DOLocalMove(new Vector3(0, -1.04f), 0.5f);
                //BoostWheel.Hide();
                BoostTrans.gameObject.SetActive(false);
                MulCreditFly.transform.position = new Vector3(0, -6.6f);
                MulCreditFly.transform.eulerAngles = Vector3.zero;
                break;
            case 3:
                BoostTrans.gameObject.SetActive(true);
                GetallTrans.gameObject.SetActive(true);
                BoostTrans.DOLocalMove(new Vector3(-10.75f, 0.04f), 0.5f);
                BoostTrans.rotation=Quaternion.Euler(new Vector3(0, 0, -45f));
                GetallTrans.DOLocalMove(new Vector3(10.75f, 0.04f), 0.5f);
                GetallTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 45f));
                DouCreditFly.transform.position = new Vector3(-3f, -6.5f);
                DouCreditFly.transform.eulerAngles = new Vector3(0, 0, 15f);
                MulCreditFly.transform.position = new Vector3(3f, -6.5f);
                MulCreditFly.transform.eulerAngles = new Vector3(0, 0, -15f);
                break;
        }
        TurntableBoost[0].enabled = true;
        TurntableGetall[0].enabled = true;
    }
    /// <summary>
    /// 进入Base模式时更新转轮布局
    /// </summary>
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
        BoostTrans.position = new Vector3(-5.51f, 1.16f);
        GetallTrans.position = new Vector3(5.51f, 1.16f);
        BoostTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        GetallTrans.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        TurntableBoost[0].enabled = false;
        TurntableGetall[0].enabled = false;
        for (int i = 0; i < leftJackpotStars.Length; i++)
        {
            leftJackpotStars[i].SetActive(false);
        }
        for(int i = 0; i < rightJackpotStars.Length; i++)
        {
            rightJackpotStars[i].SetActive(false);
        }
    }
    /// <summary>
    /// 当BET变化时转轮的图片要相应变化
    /// </summary>
    /// <param name="betLevel"></param>
    public void OnBetChange(int betLevel)
    {
        TurntableBoost[2].sprite = BaseDoubleWheelSprite[GlobalObserver.BetLevel];
        TurntableGetall[2].sprite = BaseMultiplierWheelSprite[GlobalObserver.BetLevel];
    }

    public void UpdateStars(int resultIndex,JackpotType jackpotType,bool isExtra)
    {
        StartCoroutine(UpdateStarsIen(resultIndex, jackpotType, isExtra));
    }
    private IEnumerator UpdateStarsIen(int resultIndex, JackpotType jackpotType,bool isExtra)
    {
        GameObject[] jackpotStars = isExtra ? leftJackpotStars : rightJackpotStars;
        for(int i = 0; i < jackpotStars.Length; i++)
        {
            jackpotStars[i].SetActive(false);
        }
        List<int> ableCollection = Enumerable.Range(0, 12).ToList();
        ableCollection.Remove(resultIndex);
        for (int i = 0; i < jackpotStars.Length; i++)
        {
            DropStar dropStar = SpawnFactory.GetObject<SpawnItem>("DropStar") as DropStar;
            dropStar.transform.parent = isExtra ? BoostTrans : GetallTrans;
            dropStar.transform.ResetLocalProperty();
            //dropStar.transform.localPosition = new Vector3(0, 4, 0);
            int[] indexAble;
            if (GlobalObserver.CurrentBonusState == 3)
            {
                indexAble = isExtra ? new int[3] { 0, 2, 3 } : new int[3] { 0, 9, 10 };
            }
            else
            {
                indexAble = new int[5] { 0, 2, 3, 9, 10 };
            }
            dropStar.transform.localEulerAngles = new Vector3(0, 0, -30f * indexAble[Random.Range(0,indexAble.Length)]);
            dropStar.PlayDropStarAnim(i);
            AudioManager.Playback("FE_WheelStarAdd");
            yield return new WaitForSeconds(15 / 30f);
            int index;
            if (i == jackpotIndex[jackpotType])
            {
                index = resultIndex;
            }
            else
            {
                index = ableCollection[Random.Range(0, ableCollection.Count)];
            }
            jackpotStars[i].transform.localEulerAngles = new Vector3(0, 0, index * -30f);
            jackpotStars[i].SetActive(true);
            ableCollection.Remove(index);
            yield return new WaitForSeconds(10 / 30f);
        }
    }


}
