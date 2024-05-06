using SlotGame.Result;
using UnityEngine.UI;

public class TotalWin : Popup
{
    public Text WinMoneyText;
    public override void Show(PayoutInfo result, float freeTime, float duration) {
        base.Show(result, freeTime, duration);
        WinMoneyText.text = result.TotalWin.ToString();
    }
}
