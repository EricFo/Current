using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollUp : MonoBehaviour
{
    private Text content;

    private int totalValue;
    private int targetValue;

    public event Action OnComplateEvent;
    public bool isRunning { 
        get; 
        private set; 
    }
    public int TotalValue {
        get { return totalValue; }
    }

    public void Initialize() {
        isRunning = false;  
        content = GetComponent<Text>();
        UpdateContent("0");
    }
    /// <summary>
    /// 更新计数
    /// </summary>
    /// <param name="txt"></param>
    public void UpdateContent(string txt) {
        content.text = txt;
    }
    /// <summary>
    /// 重置各项属性
    /// </summary>
    public void OnReset()
    {
        targetValue = 0;
        totalValue = 0;
        UpdateContent("0");
    }
    /// <summary>
    /// 开始滚动计数
    /// </summary>
    /// <param name="target"></param>
    public void StartUp(int target) {
        if (isRunning == false)
        {
            isRunning = true;
            this.targetValue = totalValue + target;
            InvokeRepeating("Rolling", 0, 0.02f);
        }
    }
    /// <summary>
    /// 直接设置TotalValue
    /// </summary>
    /// <param name="totalValue"></param>
    public void SetTotalValue(int target) {
        isRunning = false;
        this.totalValue = totalValue + target;
        UpdateContent(totalValue.ToString());
        OnComplateEvent();
    }
    /// <summary>
    /// 直接设置奖励数值
    /// </summary>
    /// <param name="target"></param>
    public void SetAwardValue(int target) {
        isRunning = false;
        this.totalValue = target;
        UpdateContent(totalValue.ToString());
        OnComplateEvent();
    }
    /// <summary>
    /// 停止滚动计数
    /// </summary>
    public void Stop() {
        isRunning = false;
        CancelInvoke("Rolling");
        totalValue = targetValue;
        UpdateContent(totalValue.ToString());
        OnComplateEvent();
    }
    /// <summary>
    /// 滚动计数
    /// </summary>
    private void Rolling() {
        if (totalValue != targetValue)
        {
            totalValue++;
            UpdateContent(totalValue.ToString());
        }
        else
        {
            isRunning = false;
            OnComplateEvent();
            totalValue = targetValue;
            CancelInvoke("Rolling");
        }
    }
}
