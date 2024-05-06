using SlotGame.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniversalModule.AudioSystem;

public class JackPot : MonoBehaviour {

    public Text[] texts; 

    protected int INTRO = Animator.StringToHash("Intro");
    protected int IDLE = Animator.StringToHash("Idle");

    private const float GRANDTIME = 20f;
    private const float MAJORTIME = 11f;
    private const float MINITIME = 6f;

    public void Awake() {
        texts[0].text = "$2,000.00";
        texts[1].text = "$100.00";
        texts[2].text = "$10.00";
    }

    /// <summary>
    /// 播放JackPot弹窗动画
    /// </summary>
    /// <param name="i"></param>
    public float PlayJackPot(List<int> JPList) {

        JPList.Sort();

        StartCoroutine(PlayCeleIntro(JPList));

        float time = 0;
        for (int i = 0; i < JPList.Count; i++) {
            switch (JPList[i]) {
                case 0:
                    time += GRANDTIME;
                    break;
                case 1:
                    time += MAJORTIME;
                    break;
                case 2:
                    time += MINITIME;
                    break;
            }
        }

        return time;
    }

    private IEnumerator PlayCeleIntro(List<int> JPList) {
        for (int i = 0; i < JPList.Count; i++) {
            JackPotData.PlayJackPotById(JPList[i]);
            UpdateTexts(JPList[i]);
            float time = 0;
            switch (JPList[i]) {
                case 0:
                    time = GRANDTIME;
                    AudioManager.PlayOneShot("KGI29-Bonus-Cele3");
                    break;
                case 1:
                    time = MAJORTIME;
                    AudioManager.PlayOneShot("KGI29-Bonus-Cele2");
                    break;
                case 2:
                    time = MINITIME;
                    AudioManager.PlayOneShot("KGI29-Bonus-Cele1");
                    break;
            }
            AudioManager.Pause("KGI-Game29-Bonus-Music1");
            AudioManager.Pause("KGI-Game29-Bonus-Music2");
            AudioManager.Pause("KGI-Game29-Bonus-Music3");
            yield return new WaitForSeconds(time);
            AudioManager.Continue("KGI-Game29-Bonus-Music1");
            AudioManager.Continue("KGI-Game29-Bonus-Music2");
            AudioManager.Continue("KGI-Game29-Bonus-Music3");
            RestJackPot(JPList[i]);
            JackPotData.HideJackPotById(JPList[i]);
            if (JPList[i] == 2 || JPList[i] == 1)
            {
                for (int j = 0; j < JackPotData.MeterStar[JPList[i]].Length; j++)
                {
                    JackPotData.MeterStar[JPList[i]][j].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 更新弹窗的JP显示
    /// </summary>
    public void UpdateTexts(int id) {
        string value = GlobalObserver.Meters[id].ToString("N3");
        value = value.Remove(value.Length - 1);
        texts[id].text = string.Format("${0}", value);
    }

    /// <summary>
    /// 重新Mater上的奖池，以及将奖池的钱添加到ToTalWin中
    /// </summary>
    public void RestJackPot(int id) {
        var result = GlobalObserver.GetResult(GlobalObserver.CurrGameState);
        result.WinMoney = (int)(GlobalObserver.Meters[id] * 100);
        GlobalObserver.UpdateWin(result);
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        GlobalObserver.ResetMeter(id);
        UIController.MeterPanel.ResetMeter(id, GlobalObserver.Meters[id]);
    }
}
