using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateArtFont : EditorWindow {
    private Texture fontTex;
    private string path = "";
    private string lastPath = "";
    private string content = "0123456789";
    public bool isWhetherToHandlePunctuation = true;

    private Vector2 Size = Vector2.one;
    private Vector2 Offset = Vector2.zero;

    [MenuItem("Tools/CreateArtFont", priority = 0)]
    private static void OpenWindow() {
        EditorWindow window = GetWindow<CreateArtFont>("CreateArtFont", true);
        window.maxSize = new Vector2(340, 220);
        window.minSize = new Vector2(340, 220);
        window.Show();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        GUILayout.Label("MainTex:", new[] { GUILayout.Height(28) });
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Size:");
        GUILayout.Space(10);
        Size = EditorGUILayout.Vector2Field("", Size);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Offset:");
        Offset = EditorGUILayout.Vector2Field("", Offset);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        GUILayout.Space(-140);
        fontTex = EditorGUILayout.ObjectField("", fontTex, typeof(Texture), true) as Texture;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("IsWhetherToHandlePunctuation:");
        GUILayout.Space(-20);
        isWhetherToHandlePunctuation = EditorGUILayout.Toggle(isWhetherToHandlePunctuation);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("SavePath:");
        if (fontTex != null) {
            string assetPath = AssetDatabase.GetAssetPath(fontTex);
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf(fontTex.name));
            if (!lastPath.Equals(assetPath)) {
                path = assetPath;
                lastPath = assetPath;
            }
        }
        path = EditorGUILayout.TextField(path);
        EditorGUILayout.LabelField("Content:");
        content = EditorGUILayout.TextArea(content, new[] { GUILayout.Height(50) });
        Size.x = content.Length;
        GUI.skin.button.fontSize = 20;
        if (GUILayout.Button("创建字体")) {
            if (fontTex != null) {
                float OX = ((fontTex.width - Offset.x) / fontTex.width);
                float OY = ((fontTex.height - Offset.y) / fontTex.height);
                float originX = 1 - OX;
                float originY = 1 - OY;
                float ratioX = OX / Size.x;
                float ratioY = OY / Size.y;
                float width = (fontTex.width - Offset.x) / Size.x;
                float height = (fontTex.height - Offset.y) / Size.y;
                CharacterInfo[] infos = new CharacterInfo[content.Length];
                Font artFont = new Font(fontTex.name);
                for (int i = 0; i < infos.Length; i++) {
                    bool isPunc = isWhetherToHandlePunctuation ? char.IsPunctuation(content[i]) : false;
                    infos[i].index = content[i];
                    infos[i].minX = (int)Offset.x;
                    infos[i].minY = (int)Offset.y;
                    infos[i].maxX = (int)(width * (i + 1) + Offset.x);
                    infos[i].maxY = (int)(height * ((i / (int)Size.x) + 1) + Offset.y);
                    infos[i].glyphWidth = (int)(width * (isPunc ? 0.5f : 1));
                    infos[i].glyphHeight = (int)height;
                    infos[i].uvTopLeft = new Vector2(originX + ratioX * i + (isPunc ? ratioX * 0.25f : 0), originY + ratioY * ((i / (int)Size.x) + 1));
                    infos[i].uvTopRight = new Vector2(originX + ratioX * (i + 1) - (isPunc ? ratioX * 0.25f : 0), originY + ratioY * ((i / (int)Size.x) + 1));
                    infos[i].uvBottomLeft = new Vector2(originX + ratioX * i + (isPunc ? ratioX * 0.25f : 0), originY + ratioY * (i / (int)Size.x));
                    infos[i].uvBottomRight = new Vector2(originX + ratioX * (i + 1) - (isPunc ? ratioX * 0.25f : 0), originY + ratioY * (i / (int)Size.x));
                    infos[i].advance = (int)(width * (isPunc ? 0.5f : 1));
                }
                artFont.characterInfo = infos;

                Material mat = new Material(Shader.Find("Transparent/Diffuse"));
                mat.SetTexture("_MainTex", fontTex);
                artFont.material = mat;
                if (Directory.Exists(path)) {
                    AssetDatabase.CreateAsset(mat, path + artFont.name + ".mat");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.CreateAsset(artFont, path + artFont.name + ".fontsettings");
                    AssetDatabase.SaveAssets();
                }
                else {
                    Debug.LogErrorFormat("路径{0}不存在", path);
                }
            }
            else {
                Debug.LogError("请设置字体纹理");
            }
        }
        GUI.skin.button.fontSize = 11;
    }
}
