using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackPotData : MonoBehaviour
{
    /// <summary>
    /// 代表Grand和Major的metter的星星的累计数量
    /// </summary>
    public static int[] MettersCount = new int[3] { 0, 0 , 0};
    /// <summary>
    /// 代表Grand和Major的metter累计多少星星的时候会中
    /// </summary>
    public static int[] MettersZhong = new int[3] { 3, 3 ,3};
    /// <summary>
    /// meter奖池
    /// </summary>
    public static Meter meter;
    /// <summary>
    /// JackPot弹窗
    /// </summary>
    public JackPot jackPotHierarchy;
    /// <summary>
    /// JackPot弹窗
    /// </summary>
    public static JackPot jackPot;
    /// <summary>
    /// meter奖池
    /// </summary>
    public static Meter meterStatic;
    /// <summary>
    /// Grand的Metter的上飞位置
    /// </summary>
    public Transform[] grandMeterPos;
    /// <summary>
    /// Major的Metter的上飞位置
    /// </summary>
    public Transform[] majorMeterPos;
    /// <summary>
    /// Mini的Metter的上飞位置
    /// </summary>
    public Transform[] miniMeterPos;
    /// <summary>
    /// Metter的上飞位置
    /// </summary>
    public static Transform[][] meterPosStatic = new Transform[3][];
    /// <summary>
    /// 本轮中了什么
    /// </summary>
    public static List<int> JPList = new List<int>();
    /// <summary>
    /// grand星星上飞位置
    /// </summary>
    public GameObject[] grandsStar;
    /// <summary>
    /// major星星上飞位置
    /// </summary>
    public GameObject[] MajorsStar;
    /// <summary>
    /// mini星星上飞位置
    /// </summary>
    public GameObject[] MinisStar;
    /// <summary>
    /// 所有星星上飞位置
    /// </summary>
    public static GameObject[][] MeterStar = new GameObject[3][];
    /// <summary>
    /// JackPot弹窗
    /// </summary>
    public GameObject[] JackPotWindowsHierarchy = new GameObject[3];
    /// <summary>
    /// JackPot弹窗
    /// </summary>
    public static GameObject[] JackPotWindows = new GameObject[3];
    /// <summary>
    /// Metter激活动画
    /// </summary>
    public Animator[] MetersAnimator= new Animator[3];
    /// <summary>
    /// Metter激活动画
    /// </summary>
    public static Animator[] MeterWindows = new Animator[3];


    public void Awake() {
        meter = GetComponent<Meter>();
        meterPosStatic[0] = new Transform[3];
        meterPosStatic[1] = new Transform[3];
        meterPosStatic[2] = new Transform[3];
        meterPosStatic[0] = grandMeterPos;
        meterPosStatic[1] = majorMeterPos;
        meterPosStatic[2] = miniMeterPos;

        MeterStar[0] = new GameObject[3];
        MeterStar[1] = new GameObject[3];
        MeterStar[2] = new GameObject[3];
        MeterStar[0] = grandsStar;
        MeterStar[1] = MajorsStar;
        MeterStar[2] = MinisStar;

        MeterWindows = MetersAnimator;

        jackPot = jackPotHierarchy;

        JackPotWindows = JackPotWindowsHierarchy;
    }

    /// <summary>
    /// 播放所有JackPot
    /// </summary>
    /// <returns></returns>
    public static float PlayJackPot() {
        return jackPot.PlayJackPot(JPList);
    }
    /// <summary>
    /// 开启jackPot弹窗
    /// </summary>
    /// <param name="id"></param>
    public static void PlayJackPotById(int id) {
        JackPotWindows[id].SetActive(true);
        JackPotWindows[id].transform.localScale = Vector2.zero;
        JackPotWindows[id].transform.DOScale(Vector3.one, 8f / 30f);
    }
    /// <summary>
    /// 关闭JackPot弹窗
    /// </summary>
    /// <param name="id"></param>
    public static void HideJackPotById(int id) {
        JackPotWindows[id].SetActive(false);
        meter.StopMeterWin(id);
    }
    public static void HideAllStar() {
        for (int i = 0; i < MeterStar.Length; i++) {
            for (int j = 0; j < MeterStar[i].Length; j++) {
                if (MeterStar[i][j] != null) {
                    MeterStar[i][j].SetActive(false);
                }
            }
        }
    }
}
