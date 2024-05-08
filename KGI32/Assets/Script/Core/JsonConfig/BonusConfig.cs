using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusConfig
{
    public float GrandProbability;
    public float MajorProbability;
    /// <summary>
    /// Coin概率
    /// </summary>
    public float CoinProbability;
    /// <summary>
    /// 新出银色(最低等级)金币的概率，相对金银金币
    /// </summary>
    public float SilverCoinProbability;
    /// <summary>
    /// Scatter概率
    /// </summary>
    public float ScatterProbability;
    /// <summary>
    /// 中间的结果
    /// </summary>
    public string[] ReelCenterResult = new string[0];
    /// <summary>
    /// 中间的结果概率
    /// </summary>
    public float[][] ReelCenterResultProbability = new float[0][];
    /// <summary>
    /// 新转出Coin个数概率
    /// </summary>
    public float[] SpinCoinCountProbability = new float[0];
    /// <summary>
    /// Coin不同等级的分数值集合，《Level,Value[]》
    /// </summary>
    public Dictionary<int, Dictionary<int, int[]>> CoinValueByLevel = new Dictionary<int, Dictionary<int, int[]>>();
    /// <summary>
    /// Coin不同等级的分数取值概率，《Level,概率[]》
    /// </summary>
    public Dictionary<int, Dictionary<int, float[]>> CoinValueByLevelProbability = new Dictionary<int, Dictionary<int, float[]>>();
    /// <summary>
    /// BonusIntro中COIN的数量，《初始Coin数量,Coin数量[]》
    /// </summary>
    public Dictionary<int, int[]> BonusIntroCoinCount = new Dictionary<int, int[]>();
    /// <summary>
    /// BonusIntro中COIN的数量概率，《初始Coin数量，概率[]》
    /// </summary>
    public Dictionary<int, float[]> BonusIntroCoinCountProbability = new Dictionary<int, float[]>();
    /// <summary>
    /// BonusIntro中到分数的值
    /// </summary>
    public int[] BonusIntroCredits = new int[0];
    /// <summary>
    /// BonusIntro中到分数值的概率
    /// </summary>
    public float[] BonusIntroCreditsProbability = new float[0];
    /// <summary>
    /// BonusIntro中到分数的次数
    /// </summary>
    public float[] BonusIntroCreditCountProbability = new float[0];
    /// <summary>
    /// BonusIntro中到额外SPIN次数的次数
    /// </summary>
    public float[] BonusIntroExtraSpinCountProbability = new float[0];
    /// <summary>
    /// BonusIntro中到Coin的等级概率
    /// </summary>
    public float[][] BonusIntroCoinLevelProbability = new float[0][];
    /// <summary>
    /// 最终COIN个数概率，个数8-14
    /// </summary>
    public float[] EndCoinCountProbability = new float[0];
    /// <summary>
    /// 根据最终COIN个数取到的等级组合索引，《EndCoinCount,《InitLevel3CoinCount, 概率[]》》
    /// </summary>
    public Dictionary<int, Dictionary<int, float[]>> IndexOfEndCoinCountProbability = new Dictionary<int, Dictionary<int, float[]>>();
    /// <summary>
    /// 不同COIN个数可用的组合，《EndCoinCount,《InitLevel3CoinCount, int[IndexOfEndCoinCountProbability][0-Array.Length][Level(3/2/1)]》》
    /// </summary>
    public Dictionary<int, Dictionary<int, int[][][]>> AssemblyOfEndCoinCount = new Dictionary<int, Dictionary<int, int[][][]>>();
}
