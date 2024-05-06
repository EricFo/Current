using System;
using System.Collections.Generic;

[Serializable]
public class WheelConfig {
    /// <summary>
    /// Grand收集的星星数量
    /// </summary>
    public int[][] GrandStarCount = new int[0][];
    /// <summary>
    /// Major收集的星星数量
    /// </summary>
    public int[][] MajorStarCount = new int[0][];
    /// <summary>
    /// Mini收集的星星数量
    /// </summary>
    public int[][] MiniStarCount = new int[0][];
}

public class BonusConfig {
    /// <summary>
    /// 转盘分数位置
    /// </summary>
    public int[] WheelScoreIndex = new int[0];
    /// <summary>
    /// 转盘分数位置概率
    /// </summary>
    public float[] WheelScoreProbability = new float[0];
    public WheelConfig SingleWheelConfig = new WheelConfig();
    public WheelConfig DoubleWheelConfig = new WheelConfig();
    /// <summary>
    /// Coin个数
    /// </summary>
    public Dictionary<int, int[]> CoinCount = new Dictionary<int, int[]>();
    /// <summary>
    /// Coin个数选取概率
    /// </summary>
    public Dictionary<int, float[]> CoinCountProbability = new Dictionary<int, float[]>();
    /// <summary>
    /// 不同Bonus对应的Coin初始Credit
    /// </summary>
    public Dictionary<int, int[]> InitialCoinCredits = new Dictionary<int, int[]>();
    /// <summary>
    /// 不同Bonus对应的Coin初始Credit概率
    /// </summary>
    public Dictionary<int, float[]> InitialCoinCreditsProbability = new Dictionary<int, float[]>();
    /// <summary>
    /// Bonus1和Bonus3玩法下Coin的Credit增加值
    /// </summary>
    public Dictionary<int, int[]> UpgradeCoinCredits = new Dictionary<int, int[]>();
    /// <summary>
    /// Bonus1和Bonus3玩法下Coin的Credit增加值概率
    /// </summary>
    public Dictionary<int, float[]> UpgradeCoinCreditsProbability = new Dictionary<int, float[]>();
    /// <summary>
    /// Coin序列选取概率，没有对应值123，选到的索引012即可直接应用
    /// </summary>
    public float[] CoinSequenceProbability = new float[0];
    /// <summary>
    /// 转盘序列选取概率，没有对应值123，选到的索引012即可直接应用
    /// </summary>
    public float[] WheelOrderProbability = new float[0];
    /// <summary>
    /// 不同Bonus下出Coin的序列，第一个键是当前Bonus模式，第二个键为序列Coin个数
    /// </summary>
    public Dictionary<int, Dictionary<int, int[][]>> CoinSequence = new Dictionary<int, Dictionary<int, int[][]>>();
    /// <summary>
    /// Bonus1取的转盘序列
    /// </summary>
    public Dictionary<int, Dictionary<int, int[][][]>> WheelOrderUp = new Dictionary<int, Dictionary<int, int[][][]>>();
    /// <summary>
    /// Bonus2取的转盘序列
    /// </summary>
    public Dictionary<int, Dictionary<int, int[][][]>> WheelOrderCol = new Dictionary<int, Dictionary<int, int[][][]>>();
    /// <summary>
    /// Bonus3取的加分转盘序列
    /// </summary>
    public Dictionary<int, Dictionary<int, int[][][]>> ComboOrderUp = new Dictionary<int, Dictionary<int, int[][][]>>();
    /// <summary>
    /// Bonus3取的收集转盘序列
    /// </summary>
    public Dictionary<int, Dictionary<int, int[][][]>> ComboOrderCol = new Dictionary<int, Dictionary<int, int[][][]>>();
}
