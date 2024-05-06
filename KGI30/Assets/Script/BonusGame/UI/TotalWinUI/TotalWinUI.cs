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
        TotalWin.SetActive(true);
        TotalWinValue.SetContent("");
        //DelayCallback.Delay(this, 0.3f, () =>
        //{
            TotalWinValue.SetContent(value.ToString());
        //});
    }

    private void OnMouseUpAsButton()
    {
        if (OnClickAction != null)
        {
            OnClickAction.Invoke();
            TotalWin.SetActive(false);
        }
    }
}
