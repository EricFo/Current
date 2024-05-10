using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.DelaySystem;

public class SurplusUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SurplusImage;
    [SerializeField] private ArtText surplusText;
    [SerializeField] private ArtText totalText;
    [SerializeField] private SpriteRenderer surplusSpriteRenderer;
    [SerializeField] private SpriteRenderer totalSpriteRenderer;
    [SerializeField] private Animator ChangeAnim;
    /// <summary>
    /// 当前SPIN次数
    /// </summary>
    private int surplus;
    /// <summary>
    /// 初始Bonus模式SPIN次数
    /// </summary>
    private int total;

    public void UpdateCanvas()
    {
        surplusText.SetContent(surplus.ToString());
        totalText.SetContent(total.ToString());
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        surplus = 0;
        total = 10;
        UpdateCanvas();
    }

    /// <summary>
    /// 修改剩余次数
    /// </summary>
    /// <param name="variable">增量</param>
    public void EditSurplus(int variable)
    {
        if (variable > 0)
        {
            total += variable;
        }
        else
        {
            surplus -= variable;
        }
        UpdateCanvas();
    }

    /// <summary>
    /// 次数重置
    /// </summary>
    public void ResetSurplus()
    {
        surplus = 3;
        UpdateCanvas();
    }

    /// <summary>
    /// 检查剩余次数
    /// </summary>
    /// <returns></returns>
    public int CheckRemaining()
    {
        return total - surplus;
    }


}
