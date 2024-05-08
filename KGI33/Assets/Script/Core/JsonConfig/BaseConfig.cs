using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConfig
{
    /// <summary>
    /// 钱袋子不同等级升级所需要的上飞Coin数量
    /// </summary>
    public Dictionary<int, int> CoinCountOfUpgrade = new Dictionary<int, int>();
    /// <summary>
    /// 触发reel倒转的需要的Bet倍率阈值
    /// </summary>
    public float ReelReverseBetMultiplier;
    /// <summary>
    /// 触发reel倒转的概率
    /// </summary>
    public float ReelReverseProbability;
    /// <summary>
    /// 触发Bonus的概率
    /// </summary>
    public float BonusTriggerProbability;
    /// <summary>
    /// 中不同JP的概率
    /// </summary>
    public float[][] JackpotProbability = new float[0][];
    /// <summary>
    /// Reel最终结果Symbol的集合
    /// </summary>
    public string[] ResultSymbols = new string[0];
    /// <summary>
    /// Reel最终结果Symbol选取概率
    /// </summary>
    public float[][] ResultSymbolsProbability = new float[0][];
    /// <summary>
    /// Reel最终结果Symbol选取概率
    /// </summary>
    public float[][] ResultCoinSymbolsProbability = new float[0][];
    /// <summary>
    /// Coin的等级概率，《ReelID索引,概率[]》
    /// </summary>
    public Dictionary<int, float[]> CoinLevelProbability = new Dictionary<int, float[]>();
    /// <summary>
    /// Coin不同等级的分数值集合，《Level,Value[]》
    /// </summary>
    public Dictionary<int, Dictionary<int, int[]>> CoinValueByLevel = new Dictionary<int, Dictionary<int, int[]>>();
    /// <summary>
    /// Coin不同等级的分数取值概率，《Level,概率[]》
    /// </summary>
    public Dictionary<int, Dictionary<int, float[]>> CoinValueByLevelProbability = new Dictionary<int, Dictionary<int, float[]>>();
}
