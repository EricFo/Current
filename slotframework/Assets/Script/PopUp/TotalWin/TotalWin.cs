using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.DelaySystem;

public class TotalWin : Popup
{
    [Tooltip("ToTalWin的值")] public ArtText TotalWinValue;

    /// <summary>
    /// 显示弹框，在指定时间内不可点击
    /// </summary>
    /// <param name="totalWin">总赔付值</param>
    /// <param name="freeTime">指定一个时间，在此时间内不可点击弹框</param>
    public void Show(int totalWin, float freeTime)
    {
        TotalWinValue.SetContent(totalWin.ToString());
        base.Show(freeTime);
    }

    /// <summary>
    /// 显示弹框，等待一段时间后可以自动隐藏，或者等待一段时间后可点击
    /// </summary>
    /// <param name="totalWin">总赔付值</param>
    /// <param name="duration">显示持续时间或不可点击时间，取决于自动隐藏参数的设置</param>
    /// <param name="isAutoHide">是否自动隐藏，若为False,则持续时间被当作不可点击时间处理</param>
    public void Show(int totalWin, float duration, bool isAutoHide = false)
    {
        TotalWinValue.SetContent(totalWin.ToString());
        base.Show(duration,isAutoHide);
    }

    /// <summary>
    /// 显示弹框，在指定时间内不可点击，持续一段时间后自动隐藏
    /// </summary>
    /// <param name="totalWin">总赔付值</param>
    /// <param name="freeTime">指定一个时间，在此时间内不可点击弹框</param>
    /// <param name="duration">显示持续时间，时间到后自动隐藏</param>
    public void Show(int totalWin, float freeTime, float duration)
    {
        TotalWinValue.SetContent(totalWin.ToString());
        base.Show(freeTime, duration);
    }
    
    /// <summary>
    /// 设置弹框显示的总赔付值
    /// </summary>
    /// <param name="totalWin">总赔付值</param>
    public void SetTotalWin(int totalWin)
    {
        TotalWinValue.SetContent(totalWin.ToString());
    }
    

}