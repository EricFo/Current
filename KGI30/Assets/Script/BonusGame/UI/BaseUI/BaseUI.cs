using SlotGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.DelaySystem;

public class BaseUI : MonoBehaviour
{
    [Tooltip("当前剩余次数，直接切图片")][SerializeField] private SpriteRenderer SurplusCountUI;
    [Tooltip("重置剩余次数动画")][SerializeField] private Animator SurplusCountAnim;
    public SpriteRenderer SurplusImage;
    public Sprite[] Images;
    [Tooltip("0-4")][SerializeField] public Sprite[] SurplusNumSprite;
    [Tooltip("0-4")][SerializeField] public Sprite[] SurplusNumSprite2;
    /// <summary>
    /// 初始Bonus模式SPIN次数
    /// </summary>
    public static int Surplus;


    public void UpdateCanvas()
    {
        if (GlobalObserver.CurrentBonusState == 3)
        {
            this.transform.position = new Vector3(0, 5.29f);
            SurplusImage.sprite = Images[1];
            SurplusCountUI.sprite = SurplusNumSprite[Surplus];
        }
        else
        {
            this.transform.position = new Vector3(6.62f, 6.08f);
            SurplusImage.sprite = Images[0];
            SurplusCountUI.sprite = SurplusNumSprite2[Surplus];
        }
    }
    /// <summary>
    /// 还原到初始化状态
    /// </summary>
    public void Init()
    {
        Surplus = 3;
        UpdateCanvas();
    }
    /// <summary>
    /// 点击Spin的时候次数减少一
    /// </summary>
    public void Spin()
    {
        Surplus--;
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
        if (GlobalObserver.CurrentBonusState == 3)
        {
            SurplusCountAnim.Play("Reset");
        }
        else
        {
            SurplusCountAnim.Play("Reset2");
        }
        DelayCallback.Delay(this, 0.82f, () =>
        {
            SurplusCountUI.enabled = true;
            SurplusCountAnim.Play("Idle");
        });
        //GlobalObserver.TotalSpinTime++;
    }

}
