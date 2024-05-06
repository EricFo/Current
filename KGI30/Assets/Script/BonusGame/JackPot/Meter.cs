using SlotGame.Symbol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniversalModule.AudioSystem;
using UniversalModule.SpawnSystem;

public class Meter : MonoBehaviour
{
    public GameObject[] txts;

    protected int WIN = Animator.StringToHash("Win");
    protected int IDLE = Animator.StringToHash("Idle");
    protected int REWIN = Animator.StringToHash("Rewin");

    /// <summary>
    /// 播放Metter激活动画
    /// </summary>
    /// <param name="i"></param>
    public void PlayMeterWin(int MeterId) {
        if (MeterId < 0 || MeterId > 2) {
            Debug.LogErrorFormat("不存在当前序号的Metter：{0}", MeterId);
        }
        StartCoroutine(meterActive(MeterId));
    }
    /// <summary>
    /// 激活meter
    /// </summary>
    /// <returns></returns>
    private IEnumerator meterActive(int MeterId) {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("win");
        PlayAnimation(MeterId, WIN);
    }
    /// <summary>
    /// 星星上飞
    /// </summary>
    public void PlayMeterWin2(int jpType, Vector3 InitPos, Vector3 targetPot, int StarId)
    {
        if (jpType < 0 || jpType > 2)
        {
            Debug.LogErrorFormat("不存在当前序号的Metter：{0}", jpType);
        }
        StartCoroutine(StarFly(jpType, InitPos, targetPot, StarId));
    }
    private IEnumerator StarFly(int jpType, Vector3 InitPos, Vector3 targetPot, int StarId) {
        string starName = null;
        switch (jpType) {
            case 0:
                starName = "GrandStar";
                break;
            case 1:
                starName = "MajorStar";
                break;
            case 2:
                starName = "MiniStar";
                break;
        }
        if (starName != null) {
            StarFlySymbol starFlySymbol = SpawnFactory.GetObject<SpawnItem>(starName) as StarFlySymbol;
            starFlySymbol.FlyStar(InitPos, targetPot);
        }
        yield return new WaitForSeconds(1.4f);
        JackPotData.MeterStar[jpType][StarId].SetActive(true);
    }

    /// <summary>
    /// 关闭Metter激活动画
    /// </summary>
    public void StopMeterWin(int MeterId) {
        Debug.Log("idle");
        PlayAnimation(MeterId, IDLE);
    }

    public void PlayAnimation(int MeterId, int HashCode) {

        JackPotData.MeterWindows[MeterId].Play(HashCode);
    }
    public void PlayGrandRewin() {
        StartCoroutine(PlayGrandRewinDelay());
    }
    private IEnumerator PlayGrandRewinDelay() {
        yield return new WaitForSeconds(1.5f);
        JackPotData.MeterWindows[0].Play(REWIN);
    }

    #region txts
    public void OpenTxts(bool isBool) {
        for (int i = 0; i < txts.Length; i++) {
            txts[i].SetActive(isBool);
        }
    }
    #endregion
}
