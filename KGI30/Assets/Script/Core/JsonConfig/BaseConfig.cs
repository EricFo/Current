using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConfig
{
    public Dictionary<int,int> CoinCountOfUpdate = new Dictionary<int,int>();
    /// <summary>
    /// 不同个数对应的Scatter组合
    /// </summary>
    public Dictionary<int, string[]> ScatterCombo = new Dictionary<int, string[]>();
    /// <summary>
    /// 不同个数对应的Scatter组合概率
    /// </summary>
    public Dictionary<int, float[]> ScatterComboProbability = new Dictionary<int, float[]>();
    ///// <summary>
    ///// 触发结果
    ///// </summary>
    //public List<string> ScatterResults = new List<string>();
    /// <summary>
    /// 不同Scatter组合对应的触发结果概率
    /// </summary>
    public Dictionary<string, float[]> ScatterResultProbability = new Dictionary<string, float[]>();
    /// <summary>
    /// VARSymbol可以变的Symbol种类
    /// </summary>
    public string[] VarSymbol = new string[0];
    /// <summary>
    /// 变各种Symbol的概率
    /// </summary>
    public float[] VarSymbolProbability = new float[0];
}
