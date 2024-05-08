using System;
using UnityEngine;

public abstract class ResultNode{
    public string ID { 
        get {
            return GUID.ToString();    
        } 
    }
    private Guid GUID { get; set; }
    /// <summary>
    /// 当前节点赢了多少钱
    /// </summary>
    public int WinMoney { get; set; }
    /// <summary>
    /// 赔付模式
    /// </summary>
    public PayMode PayMode { get; protected set; }

    protected ResultNode(PayMode mode) {
        WinMoney = 0;
        this.PayMode = mode;
        GUID = Guid.NewGuid(); 
    }

    public abstract void Clear();
}
