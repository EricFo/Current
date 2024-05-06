using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Symbol;
using DG.Tweening;
using UniversalModule.DelaySystem;
using Newtonsoft.Json.Linq;

public class BonusCoinSymbol : CommonSymbol
{
    public int ScoreValue;
    /// <summary>
    /// 层级
    /// </summary>
    public int tempSetSorting;
    public ArtText artText;
    public Animator BGAnim;
    public SpriteRenderer BGTex;
    /// <summary>
    /// 出现的序号，从1开始
    /// </summary>
    public int serialNum;
    /// <summary>
    /// Coin的分数text
    /// </summary>
    [SerializeField] public SpriteRenderer ScoreSpriteRender;
    public SpriteRenderer AnimSpriteRender;
    public SpriteRenderer Anim2SpriteRender;

    //public int ScoreValue { get => scoreValue; set => scoreValue = value; }
    public SpriteRenderer MainTexFly;
    public Font normalFont;
    public Font grayFont;

    protected int BG = Animator.StringToHash("BG");
    protected int STOP = Animator.StringToHash("Stop");
    protected int FLY = Animator.StringToHash("Fly");
    protected int CFLY = Animator.StringToHash("CFly");
    protected int BOOSTADD01 = Animator.StringToHash("BoostAdd01");
    protected int BOOSTADD02 = Animator.StringToHash("BoostAdd02");
    protected int GETADD01 = Animator.StringToHash("GetAdd01");
    protected int GETINTRO = Animator.StringToHash("GetIntro");
    private Tween incress;
    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder);
        artText.Font = normalFont;
        MainTex.transform.localScale = Vector3.one;
    }

    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        AnimSpriteRender.sortingOrder = order + 1;
        Anim2SpriteRender.sortingOrder = order + 2;
        ScoreSpriteRender.sortingOrder = order + 3;
        MainTexFly.sortingOrder = order + 1000;
        BGTex.sortingOrder = order - 1;
    }
    /// <summary>
    /// 刷新COIN上的分数
    /// </summary>
    /// <param name="artText"></param>
    public virtual void SetScore(int score)
    {
        ScoreValue = score;
        if (score > 0)
        {
            ScoreSpriteRender.enabled = true;
            if (score > 99999)
            {
                ScoreSpriteRender.transform.localScale = Vector3.one * 0.48f;
            }
            else if (score > 9999)
            {
                ScoreSpriteRender.transform.localScale = Vector3.one * 0.6f;
            }
            else if (score > 999)
            {
                ScoreSpriteRender.transform.localScale = Vector3.one * 0.7f;
            }
            else
            {
                //ScoreSpriteRender.gameObject.transform.localScale = Vector3.one * 0.5f;
                ScoreSpriteRender.transform.localScale = Vector3.one * 0.78f;
            }
            artText.SetContent(score.ToString());
        }
        else
        {
            ScoreSpriteRender.enabled = false;
        }
    }

    public void ScoreIncress(int value)
    {
        int currentValue = ScoreValue;
        incress = DOTween.To(() => currentValue, (tmp) => { currentValue = tmp; SetScore(tmp); }, 
            value, 2f).OnComplete(() => { SetScore(ScoreValue); });
        ScoreValue = value;
    }

    public void KillScoreIncress() {
        //SetScore(ScoreValue);
        if (incress != null && incress.IsActive())
        {
            incress.Kill(true);
        }
    }
    public override void SetMaskMode(SpriteMaskInteraction interaction)
    {
        base.SetMaskMode(interaction);
        ScoreSpriteRender.maskInteraction = interaction;
        MainTexFly.maskInteraction = interaction;
        BGTex.maskInteraction = interaction;
    }
    public void PlayFlyAnim(Vector3 target)
    {
        MainTexFly.enabled = true;
        Vector3 dir = (target - transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        //MainTexFly.transform.rotation = Quaternion.Euler(0, 0, angle);
        PlayAnimation(FLY);
        MainTexFly.transform.DOMove(target, 0.66f).SetEase(Ease.Linear).OnComplete(() =>
        {
            MainTexFly.enabled = false;
            DelayCallback.Delay(this, 0.5f, () =>
            {
                MainTexFly.transform.position = MainTex.transform.position;
            });
        });
    }
    public void PlayCFlyAnim(Vector3 target)
    {
        MainTexFly.enabled = true;
        Vector3 dir = (target - transform.position).normalized;
        float angle = Vector2.SignedAngle(Vector2.up, dir);
        MainTexFly.transform.rotation = Quaternion.Euler(0, 0, angle);
        PlayAnimation(CFLY);
        MainTexFly.transform.DOMove(target, 21/30f).SetEase(Ease.Linear).OnComplete(() =>
        {
            MainTexFly.enabled = false;
            DelayCallback.Delay(this, 0.5f, () =>
            {
                MainTexFly.transform.position = MainTex.transform.position;
                MainTexFly.transform.rotation = Quaternion.Euler(0, 0, 0);
            });
        });
    }

    public void PlayBoostAdd01()
    {
        SetSortingOrder(DefaultSortingOrder + 200);
        SetMaskMode(SpriteMaskInteraction.None);
        PlayAnimation(BOOSTADD01);
    }
    public void PlayBoostAdd02()
    {
        SetSortingOrder(DefaultSortingOrder + 200);
        SetMaskMode(SpriteMaskInteraction.None);
        PlayAnimation(BOOSTADD02);
    }
    public void PlayGetAdd01()
    {
        SetSortingOrder(DefaultSortingOrder + 200);
        SetMaskMode(SpriteMaskInteraction.None);
        PlayAnimation(GETADD01);
    }
    public void PlayGetIntro()
    {
        SetSortingOrder(DefaultSortingOrder + 200);
        SetMaskMode(SpriteMaskInteraction.None);
        PlayAnimation(GETINTRO);
    }
    public void PlayStop()
    {
        SetSortingOrder(DefaultSortingOrder + 200);
        SetMaskMode(SpriteMaskInteraction.None);
        PlayAnimation(STOP);
        DelayCallback.Delay(this, 20 / 30f, () =>
        {
            PlayIdleAnim();
        });
    }
    public void PlayBG()
    {
        BGAnim.Play(BG);
    }

    public override void PlayIdleAnim()
    {
        base.PlayIdleAnim();
        SetSortingOrder(DefaultSortingOrder);
        ScoreSpriteRender.sortingOrder = DefaultSortingOrder + 503;
    }

    public void CollectChange(bool isCollecting)
    {
        if (isCollecting)
        {
            MainTex.transform.DOScale(Vector3.one * 1.1f, 6 / 30f);
        }
        else
        {
            MainTex.transform.localScale = Vector3.one * 0.9f;
            artText.Font = grayFont;
        }
    }
}
