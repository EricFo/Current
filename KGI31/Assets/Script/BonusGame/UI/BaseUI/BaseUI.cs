using SlotGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class BaseUI : MonoBehaviour
{
    [Tooltip("当前剩余次数，直接切图片")][SerializeField] private SpriteRenderer SurplusCountUI;
    public SpriteRenderer CurrSurplusCount;
    public ArtText SurplusText;
    public ArtText CurrSurplusText;
    public Font Spin1_Num;
    public Font Spin2_Num;
    public Animator ResetAnim;
    [Tooltip("重置剩余次数动画")][SerializeField] private Animator SurplusCountAnim;
    public SpriteRenderer SurplusImage;
    public SpriteRenderer SurplusImage2;
    public Sprite[] Images;
    [Tooltip("0-4")][SerializeField] public Sprite[] SurplusNumSprite;
    [Tooltip("0-4")][SerializeField] public Sprite[] SurplusNumSprite2;
    /// <summary>
    /// 初始Bonus模式SPIN次数
    /// </summary>
    public int Surplus;
    /// <summary>
    /// 当前SPIN次数
    /// </summary>
    public int CurrSurplus;

    int IDLE = Animator.StringToHash("Idle");
    int FLYSPIN1 = Animator.StringToHash("FlySpin1");
    int FLYSPIN2 = Animator.StringToHash("FlySpin2");
    int FLYSPIN3 = Animator.StringToHash("FlySpin3");
    int ADD3FREE = Animator.StringToHash("Add3Free");
    int RESET = Animator.StringToHash("Reset");


    public void UpdateCanvas()
    {
        //if (GlobalObserver.CurrentBonusState == 3)
        //{
        //    if (CurrSurplus > 9)
        //    {
        //        CurrSurplusText.transform.localPosition = new Vector3(-1.9f, -1.43f);
        //        SurplusText.transform.localPosition = new Vector3(1.8f, -1.43f);
        //    }
        //    else
        //    {
        //        CurrSurplusText.transform.localPosition = new Vector3(-1.5f, -1.43f);
        //        SurplusText.transform.localPosition = new Vector3(1.8f, -1.43f);
        //    }
        //}
        //else
        //{
        if (CurrSurplus > 9)
        {
            if (Surplus > 9)
            {
                CurrSurplusText.transform.localPosition = new Vector3(-1.47f, -1.2f);
            }   SurplusText.transform.localPosition = new Vector3(1.5f, -1.2f);
        }
        else
        {
            if (Surplus > 9)
            {
                CurrSurplusText.transform.localPosition = new Vector3(-1.2f, -1.2f);
                SurplusText.transform.localPosition = new Vector3(1.5f, -1.2f);
            }
            else
            {
                CurrSurplusText.transform.localPosition = new Vector3(-1.2f, -1.2f);
                SurplusText.transform.localPosition = new Vector3(1.3f, -1.2f);
            }
        }
        //}
        SurplusText.SetContent(Surplus.ToString());
        CurrSurplusText.SetContent(CurrSurplus.ToString());
    }
    /// <summary>
    /// 还原到初始化状态
    /// </summary>
    public void Init()
    {
        if (GlobalObserver.CurrentBonusState == 3)
        {
            transform.localPosition = new Vector3(0, 5.9f);
            transform.localScale = Vector3.one;
            //SurplusImage.sprite = Images[1];
            //SurplusImage2.sprite = Images[3];
            //SurplusText.Font = Spin1_Num;
            //CurrSurplusText.Font = Spin1_Num;
        }
        else
        {
            transform.localPosition = new Vector3(8.68f, 8f);
            transform.localScale = Vector3.one * 0.8f;
            //SurplusImage.sprite = Images[0];
            //SurplusImage2.sprite = Images[2];
            //SurplusText.Font = Spin2_Num;
            //CurrSurplusText.Font = Spin2_Num;
        }
        CurrSurplus = 0;
        Surplus = GlobalObserver.CurrentBonusState == 3 ? 12 : 9;
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

    public void PlayAddSurplusAnim(bool isExtra)
    {
        if(GlobalObserver.CurrentBonusState == 3)
        {
            SurplusCountAnim.transform.localPosition = new Vector3(0, 10f);
            SurplusCountAnim.Play(isExtra ? FLYSPIN2 : FLYSPIN3);
        }
        else
        {
            SurplusCountAnim.transform.localPosition = new Vector3(0, 4.4f);
            SurplusCountAnim.Play(FLYSPIN1);
        }
        AudioManager.Playback("FE_WheelSpinAdd");
    }
    public void PlayAdd3FreeAnim()
    {
        SurplusCountAnim.transform.localPosition = Vector3.zero;
        SurplusCountAnim.Play(ADD3FREE);
    }
    public void PlayResetAnim()
    {
        ResetAnim.Play(RESET);
    }

}
