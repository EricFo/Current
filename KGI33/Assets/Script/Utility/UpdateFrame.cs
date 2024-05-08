using UnityEngine;

public class UpdateFrame : MonoBehaviour {
    public bool isShowFPS;
    public int designWidth;
    public int designHeight;
    public Color fpsColor = Color.white;

    private float FPS = 0;
    private int frames = 0;
    private float TimeNow = 0;
    private double lastInterval;
    private float updateInterval = 0.5f;
    private int FontSize = 0;
    //private Rect pos;

    void Awake() {
        // 关闭垂直同步
        QualitySettings.vSyncCount = 0;
        //int cutWidth = (int)(Screen.width * 0.12037f);
        //int cutHeight = (int)(Screen.height * 0.015625f);
        FontSize = (int)(50 * (Screen.height / (designHeight * 1f)));
        //pos = new Rect(Screen.width - cutWidth, Screen.height - cutHeight, cutWidth, cutHeight);
        if ((float)(Screen.width / (float)Screen.height) >= ((float)designWidth / (float)designHeight)) {
            transform.GetComponent<Camera>().orthographicSize = (float)designHeight / 200;
        }
        else {
            float size = designWidth / 2 / ((float)(Screen.width / (float)Screen.height));
            transform.GetComponent<Camera>().orthographicSize = (float)size / 100;
        }
    }

    private void Update() {
        if (isShowFPS) {
            ++frames;
            TimeNow = Time.realtimeSinceStartup;
            if (TimeNow > lastInterval + updateInterval) {
                FPS = (float)(frames / (TimeNow - lastInterval));
                frames = 0;
                lastInterval = TimeNow;
            }
        }
    }

    private void OnGUI() {
        if (isShowFPS == true) {
            GUI.contentColor = fpsColor;
            GUI.skin.label.fontSize = FontSize;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUILayout.Label(string.Format("FPS:{0}", FPS.ToString("f0")));
        }
    }
}
