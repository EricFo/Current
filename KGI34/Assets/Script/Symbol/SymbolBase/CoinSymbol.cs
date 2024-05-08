using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotGame.Symbol;
using SlotGame.Core;
using SlotGame.Config;
using DG.Tweening;
using UnityEngine.Rendering;
using System.Linq;

public class CoinSymbol : CommonSymbol
{
    public Animator JPAnim;
    public SpriteRenderer JPAnimTex;


    public Sprite[] coinSpriteByJackPot;
    public Sprite[] scoreSpriteByJackpot;

    private int scoreValue;
    public ArtText scoreText;
    public SpriteRenderer ScoreSpriteRender;
    public SpriteRenderer AnimSpriteRender;
    public SpriteRenderer AnimAddSpriteRender;
    protected int level = 1;
    protected JackpotType jackpotType = JackpotType.Null;

    public SpriteRenderer[] glowRenderer;
    public SortingGroup smallGlowSorting;


    public virtual int Level
    {
        get { return level; }
        set
        {
            level = value;
        }
    }

    public JackpotType JackpotType
    {
        get { return jackpotType; }
        set
        {
            jackpotType = value;
            MainTex.sprite = coinSpriteByJackPot[GlobalObserver.JackpotIDDic[value]];
        }
    }

    public int ScoreValue
    {
        get { return scoreValue; }
        set { scoreValue = value; }
    }

    #region 动画Hash
    protected int GOLDINTRO = Animator.StringToHash("GoldIntro");
    protected int GOLDWIN = Animator.StringToHash("GoldWin");
    protected int MIXINTRO = Animator.StringToHash("MixIntro");
    protected int MIXWIN = Animator.StringToHash("MixWin");
    protected int SILVERINTRO = Animator.StringToHash("SilverIntro");
    protected int SILVERWIN = Animator.StringToHash("SilverWin");
    protected int GRANDWIN = Animator.StringToHash("GrandWin");
    protected int MAJORWIN = Animator.StringToHash("MajorWin");
    protected int MINORWIN = Animator.StringToHash("MinorWin");
    protected int MINIWIN = Animator.StringToHash("MiniWin");
    protected int COINCHANGE = Animator.StringToHash("CoinChange");
    protected int COINADD = Animator.StringToHash("CoinAdd");
    protected int GRANDINTRO = Animator.StringToHash("GrandIntro");
    protected int MAJORINTRO = Animator.StringToHash("MajorIntro");
    protected int MINORINTRO = Animator.StringToHash("MinorIntro");
    protected int MINIINTRO = Animator.StringToHash("MiniIntro");
    #endregion


    public virtual void PlayJackpotCeleAnim()
    {
        PlayGlowAnim(false);
        SetSortingOrder(DefaultSortingOrder + 800);
        ScoreSpriteRender.enabled = false;
        PlayAnimation(GOLDWIN);
        switch (jackpotType)
        {
            case JackpotType.Grand:
                PlayAnimation(GRANDWIN);
                JPAnim.Play(GRANDWIN);
                break;
            case JackpotType.Major:
                PlayAnimation(MAJORWIN);
                JPAnim.Play(MAJORWIN);
                break;
            case JackpotType.Minor:
                PlayAnimation(MINORWIN);
                JPAnim.Play(MINORWIN);
                break;
            case JackpotType.Mini:
                PlayAnimation(MINIWIN);
                JPAnim.Play(MINIWIN);
                break;
            default:
                break;
        }
    }

    public virtual void PlayCoinChangeAnim()
    {
        PlayAnimation(COINCHANGE);
        JPAnim.Play(IDLE);
    }

    public void UpSortingOrder(bool isUp = true, int order = 200)
    {
        SetSortingOrder(DefaultSortingOrder + (isUp ? order : 0));
    }

    public virtual void PlayGlow()
    {

    }

    public virtual void PlayGlowAnim(bool Display = true)
    {
        glowRenderer[level - 1].DOFade(Display ? 1 : 0, 5 / 30f);
    }

    public virtual void PlayCoinAddAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 400);
        PlayAnimation(COINADD);
    }

    /// <summary>
    /// 播放奖励动画
    /// </summary>
    public virtual void PlayWinAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 400);
        switch (jackpotType)
        {
            case JackpotType.Grand:
                PlayAnimation(GRANDINTRO);
                break;
            case JackpotType.Major:
                PlayAnimation(MAJORINTRO);
                break;
            case JackpotType.Minor:
                PlayAnimation(MINORINTRO);
                break;
            case JackpotType.Mini:
                PlayAnimation(MINIINTRO);
                break;
        }
    }

    public override void PlayIdleAnim()
    {
        ScoreSpriteRender.enabled = true;
        SetSortingOrder(DefaultSortingOrder);
        base.PlayIdleAnim();
        MainTex.sprite = coinSpriteByJackPot[GlobalObserver.JackpotIDDic[jackpotType]];
    }

    public override void Install(Transform parent, Vector3 localPos, int reelID, int sortingOrder = 101)
    {
        base.Install(parent, localPos, reelID, sortingOrder + 10);
    }

    public void InitLevel(int reelID)
    {
        Level = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinLevelProbability[reelID % 5]) + 1;
    }
    /*public void InitScore()
    {
        int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[Level]);
        int score = ConfigManager.baseConfig.CoinValueByLevel[Level][index];
        SetScore(score);
    }*/

    public virtual void PlayStopAnim()
    {
        SetSortingOrder(DefaultSortingOrder + 400);
    }


    public override void SetSortingOrder(int order)
    {
        base.SetSortingOrder(order);
        AnimSpriteRender.sortingOrder = order + 1;
        AnimAddSpriteRender.sortingOrder = order + 1;
        JPAnimTex.sortingOrder = order + 2;
        smallGlowSorting.sortingOrder = order + 3;
        ScoreSpriteRender.sortingOrder = order + 5;
    }

    public override void SetMaskMode(SpriteMaskInteraction interaction)
    {
        base.SetMaskMode(interaction);
        AnimSpriteRender.maskInteraction = interaction;
        ScoreSpriteRender.maskInteraction = interaction;
        JPAnimTex.maskInteraction = interaction;
        foreach (var item in glowRenderer)
        {
            item.maskInteraction = interaction;
        }
    }

    public virtual void SetScore(int score)
    {
        jackpotType = JackpotType.Null;
        ScoreValue = score;
        if (scoreSpriteByJackpot.Contains(ScoreSpriteRender.sprite))
        {
            ScoreSpriteRender.sprite = null;
        }
        if (score > 0)
        {
            if (score > 9999)
            {
                ScoreSpriteRender.transform.localScale = Vector3.one * 0.7476f;
            }
            else if (score > 999)
            {
                ScoreSpriteRender.transform.localScale = Vector3.one * 0.9174f;
            }
            else
            {
                ScoreSpriteRender.transform.localScale = Vector3.one;
            }
            ScoreSpriteRender.enabled = true;
            scoreText.SetContent(score.ToString());
        }
        else
        {
            ScoreSpriteRender.enabled = false;
        }
    }
    public virtual void SetScore(JackpotType jackpot)
    {
        JackpotType = jackpot;
        if (jackpot == JackpotType.Null)
        {

        }
        else
        {
            ScoreSpriteRender.enabled = true;
            ScoreSpriteRender.sprite = scoreSpriteByJackpot[GlobalObserver.JackpotIDDic[jackpot]];
            /*switch (jackpot)
            {
                case JackpotType.Grand:
                    ScoreSpriteRender.sprite = scoreSpriteByJackpot[3];
                    break;
                case JackpotType.Major:
                    ScoreSpriteRender.sprite = scoreSpriteByJackpot[2];
                    break;
                case JackpotType.Minor:
                    ScoreSpriteRender.sprite = scoreSpriteByJackpot[1];
                    //scoreValue = 2500;
                    break;
                case JackpotType.Mini:
                    ScoreSpriteRender.sprite = scoreSpriteByJackpot[0];
                    //scoreValue = 1000;
                    break;
            }*/
        }
    }

    public override void Recycle()
    {
        foreach(var item in glowRenderer)
        {
            item.DOFade(0, 0);
        }
        base.Recycle();
    }

}
