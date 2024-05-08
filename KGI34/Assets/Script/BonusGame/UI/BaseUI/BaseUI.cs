using SlotGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.DelaySystem;

public class BaseUI : MonoBehaviour
{
    [Tooltip("当前剩余次数，直接切图片")][SerializeField] private SpriteRenderer SurplusCountUI;
    public ArtText CurrSurplusText;
    public ArtText SurplusText;
    public SpriteRenderer CurrSurplusSpriteRenderer;
    public SpriteRenderer SurplusSpriteRenderer;
    [Tooltip("重置剩余次数动画")][SerializeField] private Animator SurplusCountAnim;
    public SpriteRenderer SurplusImage;
    public Sprite[] surplusImages;
    /// <summary>
    /// 初始Bonus模式SPIN次数
    /// </summary>
    public int Surplus;
    /// <summary>
    /// 当前SPIN次数
    /// </summary>
    public int CurrSurplus;


    public void UpdateCanvas()
    {
        if (CurrSurplus > 9)
        {
            CurrSurplusText.transform.localPosition = new Vector3(-0.79f, -0.42f);
        }
        else
        {
            CurrSurplusText.transform.localPosition = new Vector3(-0.7f, -0.42f);
        }
        if (Surplus > 9)
        {
            SurplusText.transform.localPosition = new Vector3(0.78f, -0.42f);
        }
        else
        {
            SurplusText.transform.localPosition = new Vector3(0.65f, -0.42f);
        }
        CurrSurplusText.SetContent(CurrSurplus.ToString());
        SurplusText.SetContent(Surplus.ToString());
    }
    /// <summary>
    /// 还原到初始化状态
    /// </summary>
    public void Init()
    {
        CurrSurplus = 0;
        Surplus = 8;
        SurplusImage.sprite = surplusImages[0];
        SurplusSpriteRenderer.enabled = true;
        CurrSurplusSpriteRenderer.enabled = true;
        UpdateCanvas();
    }
    /// <summary>
    /// 点击Spin的时候次数减少一
    /// </summary>
    public void Spin()
    {
        CurrSurplus++;
        UpdateCanvas();
    }
    /// <summary>
    /// 加SPIN次数
    /// </summary>
    public void AddSurplus(int surplus)
    {
        Surplus += surplus;
        UpdateCanvas();
    }
    /// <summary>
    /// 出现新的Coin时次数重置
    /// </summary>
    public void ResetSurplus()
    {
        Surplus = 3;
        UpdateCanvas();
        SurplusCountUI.enabled = false;
            SurplusCountAnim.Play("Reset");
        DelayCallback.Delay(this, 0.82f, () =>
        {
            SurplusCountUI.enabled = true;
            SurplusCountAnim.Play("Idle");
        });
        //GlobalObserver.TotalSpinTime++;
    }

    public int CheckRemaining()
    {
        return Surplus - CurrSurplus;
    }

    public void OnBonusIsOver()
    {
        SurplusImage.sprite = surplusImages[1];
        SurplusSpriteRenderer.enabled = false;
        CurrSurplusSpriteRenderer.enabled = false;
    }

}
