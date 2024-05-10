using System;
using SlotGame.Result;
public interface IPopup {
    string WindowName { get; }

    event Action OnClickEvent;
    void Initialize();
    /// <summary>
    /// 显示弹框，在指定时间内不可点击
    /// </summary>
    /// <param name="result">赔付信息</param>
    /// <param name="freeTime">指定一个时间，在此时间内不可点击弹框</param>
    void Show(/*PayoutInfo result, */float freeTime);
    /// <summary>
    /// 显示弹框，等待一段时间后可以自动隐藏，或者等待一段时间后可点击
    /// </summary>
    /// <param name="result">赔付信息</param>
    /// <param name="duration">显示持续时间或不可点击时间，取决于自动隐藏参数的设置</param>
    /// <param name="isAutoHide">是否自动隐藏，若为False,则持续时间被当作不可点击时间处理</param>
    void Show(/*PayoutInfo result, */float duration, bool isAutoHide);
    /// <summary>
    /// 显示弹框，在指定时间内不可点击，持续一段时间后自动隐藏
    /// </summary>
    /// <param name="result">赔付时间</param>
    /// <param name="freeTime">指定一个时间，在此时间内不可点击弹框</param>
    /// <param name="duration">显示持续时间，时间到后自动隐藏</param>
    /// <exception cref="NotImplementedException"></exception>
    void Show(/*PayoutInfo result, */float freeTime, float duration);
    void Hide();
}
