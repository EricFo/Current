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
    /// ��ǰSPIN����
    /// </summary>
    private int surplus;
    /// <summary>
    /// ��ʼBonusģʽSPIN����
    /// </summary>
    private int total;

    public void UpdateCanvas()
    {
        surplusText.SetContent(surplus.ToString());
        totalText.SetContent(total.ToString());
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init()
    {
        surplus = 0;
        total = 10;
        UpdateCanvas();
    }

    /// <summary>
    /// �޸�ʣ�����
    /// </summary>
    /// <param name="variable">����</param>
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
    /// ��������
    /// </summary>
    public void ResetSurplus()
    {
        surplus = 3;
        UpdateCanvas();
    }

    /// <summary>
    /// ���ʣ�����
    /// </summary>
    /// <returns></returns>
    public int CheckRemaining()
    {
        return total - surplus;
    }


}
