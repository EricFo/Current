using System;
using UnityEngine.UI;
[Serializable]
public class MeterUIPanel {
    public Text[] BaseMeters;

    /// <summary>
    /// 更新Meter
    /// </summary>
    public void UpdateMeters(double[] meters) {
        for (int i = 0; i < meters.Length; i++) {
            string value = meters[i].ToString("N3");
            value = value.Remove(value.Length - 1);
            BaseMeters[i].text = string.Format("${0}", value);
        }
    }

    /// <summary>
    /// 重置Meter
    /// </summary>
    /// <param name="id">MeterID</param>
    /// <param name="value">Meter数值</param>
    public void ResetMeter(int id, double value) {
        BaseMeters[id].text = string.Format("${0:N2}", value);
    }
}
