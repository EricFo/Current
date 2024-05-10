using System;
using UnityEngine;
using SlotGame.Result;
using UniversalModule.DelaySystem;

[RequireComponent(typeof(BoxCollider2D))]
public class Popup : MonoBehaviour, IPopup
{
    [SerializeField] private string windowName = null;

    protected float timer;
    protected float freeTime;
    protected float duration;
    protected bool isClick = false;
    protected bool isAutoHide = false;
    protected BoxCollider2D boxCollider;
    protected DelayNodeBase delayNode;

    private Action OnClickAction;

    public string WindowName {
        get { return windowName; }
    }

    public event Action OnClickEvent {
        add {
            OnClickAction -= value;
            OnClickAction += value;
        }
        remove {
            OnClickAction -= value;
        }
    }

    public virtual void Initialize(){
        Hide();
        isClick = false;
        boxCollider = GetComponent<BoxCollider2D>(); 
        boxCollider.size = Vector3.right * 20 + Vector3.up * 15;
    }

    /// <summary>
    /// 显示弹框，在指定时间内不可点击
    /// </summary>
    /// <param name="result">赔付信息</param>
    /// <param name="freeTime">指定一个时间，在此时间内不可点击弹框</param>
    public virtual void Show(/*PayoutInfo result, */float freeTime) {
        this.isClick = false;
        this.timer = Time.time;
        this.freeTime = freeTime;
        this.gameObject.SetActive(true);
    }
    /// <summary>
    /// 显示弹框，等待一段时间后可以自动隐藏，或者等待一段时间后可点击
    /// </summary>
    /// <param name="result">赔付信息</param>
    /// <param name="duration">显示持续时间或不可点击时间，取决于自动隐藏参数的设置</param>
    /// <param name="isAutoHide">是否自动隐藏，若为False,则持续时间被当作不可点击时间处理</param>
    public virtual void Show(/*PayoutInfo result, */float duration, bool isAutoHide = false) {
        this.isClick = false;
        this.timer = Time.time;
        this.duration = duration;
        this.isAutoHide = isAutoHide;
        this.gameObject.SetActive(true);
        if (isAutoHide == true) {
            delayNode = this.Delay(duration, OnClick);
        } else {
            this.freeTime = duration;
        }
    }
    /// <summary>
    /// 显示弹框，在指定时间内不可点击，持续一段时间后自动隐藏
    /// </summary>
    /// <param name="result">赔付时间</param>
    /// <param name="freeTime">指定一个时间，在此时间内不可点击弹框</param>
    /// <param name="duration">显示持续时间，时间到后自动隐藏</param>
    /// <exception cref="NotImplementedException"></exception>
    public virtual void Show(/*PayoutInfo result, */float freeTime, float duration) {
        this.isClick = false;
        this.timer = Time.time;
        this.isAutoHide = true;
        this.freeTime = freeTime;
        this.duration = duration;
        this.gameObject.SetActive(true);
        delayNode = this.Delay(duration, OnClick);
    }
    protected virtual void OnClick() {
        Hide();
        OnClickAction?.Invoke();
        if (delayNode != null) {
            DelayCallback.StopDelay(delayNode);
            delayNode = null;
        }
    }
    public virtual void Hide() {
        this.gameObject.SetActive(false);
    }

    private void OnMouseUpAsButton(){
        if (Time.time - timer > freeTime && (!isClick || isAutoHide)){
            isClick = true;
            OnClick();
        }
    }
}
