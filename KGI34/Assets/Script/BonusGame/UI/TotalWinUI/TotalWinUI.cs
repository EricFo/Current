using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class TotalWinUI : MonoBehaviour
{
    [Tooltip("ToTalWin")] public GameObject TotalWin;
    [Tooltip("ToTalWin的值")] public ArtText TotalWinValue;
    public BoxCollider2D _collider;
    public GameObject[] transitionAnim;
    public bool isClick = false;
    public CattleUI cattleUI;

    public Action OnClickAction;
    public event Action OnClickEvent
    {
        add
        {
            OnClickAction -= value;
            OnClickAction += value;
        }
        remove
        {
            OnClickAction -= value;
        }
    }
    /// <summary>
    /// 播放ToTalWin，不包括Base中的赢钱
    /// </summary>
    /// <param name="value"></param>
    public void PlayTotalWin(float value)
    {
        isClick = false;
        TotalWin.SetActive(true);
        TotalWinValue.SetContent("");
        TotalWinValue.SetContent(value.ToString());
        _collider.enabled = true;
    }

    public void OnMouseUpAsButton()
    {
        if (!isClick)
        {
            StartCoroutine(_OnClick());
        }
        /*if (OnClickAction != null)
        {
            OnClickAction.Invoke();
            TotalWin.SetActive(false);
            _collider.enabled = false;
        }*/
    }
    IEnumerator _OnClick()
    {
        if (OnClickAction != null)
        {
            isClick = true;
            AudioManager.Playback("FE_BonusTurn");
            foreach (var anim in transitionAnim)
            {
                anim.SetActive(false);
                anim.SetActive(true);
            }
            cattleUI.DisplayBigCattle(false);
            yield return new WaitForSeconds(25 / 30f);
            OnClickAction.Invoke();
            TotalWin.SetActive(false);
            _collider.enabled = false;
        }
    }

}
