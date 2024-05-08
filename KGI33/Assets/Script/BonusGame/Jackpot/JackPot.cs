using SlotGame.Config;
using SlotGame.Core;
using SlotGame.Symbol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UniversalModule.AudioSystem;
using UniversalModule.SpawnSystem;

public enum JackpotType
{
    Grand,
    Major,
    Minor,
    Mini,
    Null
}

public class JackPot : MonoBehaviour {

    public Text[] texts;
    public GameObject[] celeAnims;
    public GameObject[] meterCeleAnims;

    public float[] CeleTime;
    private const float GRANDTIME = 25f;
    private const float MAJORTIME = 19f;
    private const float MINORTIME = 14f;
    private const float MINITIME = 9.5f;

    public void Awake() {
        /*texts[0].text = "$10.00";
        texts[1].text = "$25.00";*/
        texts[2].text = "$400.00";
        texts[3].text = "$5,000.00";
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
                    time += MINITIME;
                    break;
                case 1:
                    time += MINORTIME;
                    break;
                case 2:
                    time += MAJORTIME;
                    break;
                case 3:
                    time += GRANDTIME;
                    break;
            }
        }
        return time;
    }

    private IEnumerator PlayCeleIntro(List<int> JPList) {
        for (int i = 0; i < JPList.Count; i++) {
            celeAnims[JPList[i]].SetActive(true);
            meterCeleAnims[JPList[i]].SetActive(true);
            UpdateTexts(JPList[i]);
            float time = 0;
            switch (JPList[i]) {
                case 0:
                    time = MINITIME;
                    AudioManager.PlayOneShot("KGI_Game32_Cele1");
                    break;
                case 1:
                    time = MINORTIME;
                    AudioManager.PlayOneShot("KGI_Game32_Cele2");
                    break;
                case 2:
                    time = MAJORTIME;
                    AudioManager.PlayOneShot("KGI_Game32_Cele3");
                    break;
                case 3:
                    time = GRANDTIME;
                    AudioManager.PlayOneShot("KGI_Game32_Cele4");
                    break;
            }
            AudioManager.Pause("KGI_Game32_Bonus_Music");
            yield return new WaitForSeconds(time);
            AudioManager.Continue("KGI_Game32_Bonus_Music");
            RestJackPot(JPList[i]);
            celeAnims[JPList[i]].SetActive(false);
            meterCeleAnims[JPList[i]].SetActive(false);
        }
    }

    public IEnumerator PlayJackpotCele(BonusCoinSymbol coinSymbol)
    {
        AudioManager.PlayOneShot("FeatureSelect");
        JackpotType jackpotType = coinSymbol.JackpotType;
        int jackpotIndex = GlobalObserver.JackpotIDDic[jackpotType];
        coinSymbol.PlayJackpotCeleAnim();
        meterCeleAnims[jackpotIndex].SetActive(true);
        yield return new WaitForSeconds(3f);
        if (jackpotType is JackpotType.Grand or JackpotType.Major)
        {
            AudioManager.Pause("KGI_Game32_Bonus_Music");
            AudioManager.PlayOneShot("KGI_Game32_Cele"+(jackpotIndex+1).ToString());
            UpdateTexts(jackpotIndex);
            celeAnims[jackpotIndex].SetActive(true);
            yield return new WaitForSeconds(CeleTime[jackpotIndex]);
            RestJackPot(jackpotIndex);
            celeAnims[jackpotIndex].SetActive(false);
            AudioManager.Continue("KGI_Game32_Bonus_Music");
        }
        else
        {
            coinSymbol.PlayGlow();
        }
        coinSymbol.PlayIdleAnim();
        coinSymbol.JPAnim.Play("Idle");
        coinSymbol.UpSortingOrder();
        meterCeleAnims[jackpotIndex].SetActive(false);

        coinSymbol.PlayJPCoinNumAnim();
        yield return new WaitForSeconds(18 / 30f);
        if (jackpotType == JackpotType.Mini)
        {
            coinSymbol.SetScore(1000 + coinSymbol.ScoreValue);
        }
        else if (jackpotType == JackpotType.Minor)
        {
            coinSymbol.SetScore(2500 + coinSymbol.ScoreValue);
        }
        else
        {
            coinSymbol.SetScore(coinSymbol.ScoreValue);
        }
        yield return new WaitForSeconds(22 / 30f);
        coinSymbol.JPCoinNumAnim.SetActive(false);
        /*coinSymbol.PlayCoinChangeAnim();
        yield return new WaitForSeconds(21 / 30f);
        if (jackpotType == JackpotType.Mini)
        {
            coinSymbol.SetScore(1000 + coinSymbol.ScoreValue);
        }
        else if (jackpotType == JackpotType.Minor)
        {
            coinSymbol.SetScore(2500 + coinSymbol.ScoreValue);
        }
        else
        {
            coinSymbol.SetScore(coinSymbol.ScoreValue);
        }
        yield return new WaitForSeconds(5 / 30f);*/
    }

    public IEnumerator PlayJackpotCeleBase(List<CoinSymbol> coinSymbols)
    {
        List<CoinSymbol> bigCoinSymbols = new List<CoinSymbol>();
        List<CoinSymbol> smallCoinSymbols = new List<CoinSymbol>();
        foreach (var item in coinSymbols)
        {
            if (item.JackpotType == JackpotType.Grand)
            {
                bigCoinSymbols.Add(item);
            }
        }
        foreach(var item in coinSymbols)
        {
            if (item.JackpotType == JackpotType.Major)
            {
                bigCoinSymbols.Add(item);
            }
        }
        foreach (var item in coinSymbols)
        {
            if (item.JackpotType is JackpotType.Mini or JackpotType.Minor)
            {
                smallCoinSymbols.Add(item);
            }
        }
        if (bigCoinSymbols.Count > 0)
        {
            foreach(var item in bigCoinSymbols)
            {
                yield return PlayJackpotCelebase(item);
            }
        }
        if(smallCoinSymbols.Count > 0)
        {
            foreach (var item in smallCoinSymbols)
            {
                StartCoroutine(PlayJackpotCelebase(item));
            }
            yield return new WaitForSeconds(4.5f);
        }

    }

    public IEnumerator PlayJackpotCelebase(CoinSymbol coinSymbol)
    {
        AudioManager.PlayOneShot("FeatureSelect");
        JackpotType jackpotType = coinSymbol.JackpotType;
        int jackpotIndex = GlobalObserver.JackpotIDDic[jackpotType];
        coinSymbol.PlayJackpotCeleAnim();
        meterCeleAnims[jackpotIndex].SetActive(true);
        yield return new WaitForSeconds(3f);
        if (jackpotType is JackpotType.Grand or JackpotType.Major)
        {
            AudioManager.Pause("KGI_Game32_Bonus_Music");
            AudioManager.PlayOneShot("KGI_Game32_Cele" + (jackpotIndex + 1).ToString());
            UpdateTexts(jackpotIndex);
            celeAnims[jackpotIndex].SetActive(true);
            yield return new WaitForSeconds(CeleTime[jackpotIndex]);
            RestJackPot(jackpotIndex);
            celeAnims[jackpotIndex].SetActive(false);
            AudioManager.Continue("KGI_Game32_Bonus_Music");
        }
        else
        {
            coinSymbol.PlayGlow();
        }
        //coinSymbol.PlayIdleAnim();
        coinSymbol.UpSortingOrder();
        meterCeleAnims[jackpotIndex].SetActive(false);

        if (jackpotType is JackpotType.Grand or JackpotType.Major)
        {
            coinSymbol.PlayIdleAnim();
            coinSymbol.JPAnim.Play("Idle");
        }
        yield return new WaitForSeconds(0.5f);
        /*if(jackpotType is JackpotType.Grand or JackpotType.Major)
        {
            coinSymbol.PlayCoinChangeAnim();
            yield return new WaitForSeconds(21 / 30f);
            int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][3]);
            int score = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][3][index];
            coinSymbol.SetScore(score);
            yield return new WaitForSeconds(5 / 30f);
        }*/
        /*if (jackpotType == JackpotType.Mini)
        {
            coinSymbol.SetScore(1000);
        }
        else if(jackpotType == JackpotType.Minor)
        {
            coinSymbol.SetScore(2500);
        }
        else
        {
            int index = GlobalObserver.GetRandomWeightedIndex(ConfigManager.baseConfig.CoinValueByLevelProbability[GlobalObserver.GetMultiplyer()][3]);
            if (index > ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4 - 1)
            {
                JackpotType coinJackpotType = GlobalObserver.IDJackpotDic[index - (ConfigManager.baseConfig.CoinValueByLevel[1][3].Length - 4)];
                coinSymbol.SetScore(coinJackpotType);
            }
            else
            {
                int score = ConfigManager.baseConfig.CoinValueByLevel[GlobalObserver.GetMultiplyer()][3][index];
                coinSymbol.SetScore(score);
            }
        }*/
    }

    /// <summary>
    /// 更新弹窗的JP显示
    /// </summary>
    public void UpdateTexts(int id) {
        id = 3 - id;
        string value = GlobalObserver.Meters[id].ToString("N3");
        value = value.Remove(value.Length - 1);
        texts[id].text = string.Format("${0}", value);
    }

    /// <summary>
    /// 重新Mater上的奖池，以及将奖池的钱添加到ToTalWin中
    /// </summary>
    public void RestJackPot(int id) {
        id = 3 - id;
        var result = GlobalObserver.GetResult(GlobalObserver.CurrGameState);
        result.WinMoney = (int)(GlobalObserver.Meters[id] * 100);
        GlobalObserver.UpdateWin(result);
        UIController.BottomPanel.SetAward(GlobalObserver.TotalWin);
        GlobalObserver.ResetMeter(id);
        UIController.MeterPanel.ResetMeter(id, GlobalObserver.Meters[id]);
    }
}
