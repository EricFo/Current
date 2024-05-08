using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;

public class ArtFontToPNGWindow : EditorWindow {
    public static Font font;
    public static string savePath;
    public static char separator = ';';
    public static float charPitch = 1f;
    public static Vector2Int limit = Vector2Int.zero;
    public static string contents = "100;200;300;400;";

    private string LastValue = string.Empty;
    
    private Font lastFont;
    private Texture2D texture;
    private Texture2D TempTex;
    private Texture2D Preview;
    private NativeArray<byte> tempData;
    private NativeArray<byte> mainTexData;
    private Dictionary<int, Color[]> fontCache;

    private Texture2D MainTex {
        get {
            if (texture == null) {
                texture = font.material.mainTexture as Texture2D;
            }
            return texture;
        }
    }
    private NativeArray<byte> MainTexData {
        get {
            if (mainTexData == null || mainTexData.Length == 0) {
                mainTexData = MainTex.GetRawTextureData<byte>();
            }
            return mainTexData;
        }
    }

    [MenuItem("Tools/ArtFontToPNG", priority = 1)]
    public static void OpenWindow() {
        EditorWindow window = GetWindow<ArtFontToPNGWindow>("ArtFontToPNG", true);
        window.maxSize = new Vector2(340, 280);
        window.minSize = new Vector2(340, 280);
        window.Show();
    }
    private void OnGUI() {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ArtFont:");
        GUILayout.Space(-180);
        font = EditorGUILayout.ObjectField("", font, typeof(Font), true) as Font;
        EditorGUILayout.EndHorizontal();
        if (lastFont != font) {
            lastFont = font;
            texture = null;
            if (mainTexData.Length > 0) {
                mainTexData = default;
            }
            LastValue = string.Empty;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("SavePath:");
        GUILayout.Space(-180);
        savePath = EditorGUILayout.TextField(savePath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Separator:");
        GUILayout.Space(-180);
        separator = Convert.ToChar(EditorGUILayout.TextField(separator.ToString()));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Character Pitch:");
        GUILayout.Space(-100);
        charPitch = EditorGUILayout.FloatField(charPitch);
        EditorGUILayout.EndHorizontal();
        limit = EditorGUILayout.Vector2IntField("Size Limit:", limit);
        EditorGUILayout.EndVertical();

        EditorGUILayout.LabelField("Contents:");
        contents = EditorGUILayout.TextArea(contents, new[] { GUILayout.Height(100)});
        if (font != null) {
            string assetPath = AssetDatabase.GetAssetPath(font);
            assetPath = assetPath.Substring(0, assetPath.LastIndexOf(font.name));
            savePath = assetPath;
        }
        GUI.skin.button.fontSize = 20;
        string[] splits = contents.Split(separator);
        if (GUILayout.Button("生成PNG",new[] { GUILayout.Height(50)})) {
            if (font != null) {
                if (MainTex != null) {
                    fontCache = new Dictionary<int, Color[]>();
                    foreach (var item in font.characterInfo) {
                        int x = (int)(item.uvBottomLeft.x * MainTex.width);
                        int y = (int)(item.uvBottomLeft.y * MainTex.height);
                        int w = item.glyphWidth;
                        int h = item.glyphHeight;
                        Color[] colors = MainTex.GetPixels(x, y, w, h);
                        fontCache.Add(item.index, colors);
                    }
                    
                    for (int i = 0; i < splits.Length; i++) {
                        CreateTexture(splits[i]);
                        EditorUtility.DisplayProgressBar("生成中...", string.Format("当前进度{0}:{1}", i + 1, splits.Length), (i + 1) / splits.Length);
                    }
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.Refresh();
                }
                else {
                    Debug.LogError("未找到相关字体纹理，请检查字体纹理设置");
                }
            }
            else {
                Debug.LogError("请设置艺术字体");
            }
        }
        //if (splits.Length > 0 && font != null) {
        //    if (splits[0].Length > 0) {
        //        Texture2D tex = GetPreview(splits[0]);
        //        int x = 0, y = 0;
        //        x = tex.width < 340 ? (340 - tex.width) / 2 : 0;
        //        y = tex.height < 200 ? (200 - tex.height) / 2 - tex.height: (tex.height - 200) / 2;
        //        if (tex.width < 340) {
        //            EditorGUI.DrawPreviewTexture(new Rect(x, 280, tex.width, 200), tex, new Material(Shader.Find("Sprites/Default")), ScaleMode.ScaleToFit);
        //        }
        //        else {
        //            EditorGUI.DrawPreviewTexture(new Rect(x, 240 - y, 340, 200), tex, new Material(Shader.Find("Sprites/Default")), ScaleMode.ScaleToFit);
        //        }
        //    }
        //}
        GUI.skin.button.fontSize = 11;
    }
    private void CreateTexture(string value) {
        int width = 0, height = 0;
        List<CharacterInfo> infos = new List<CharacterInfo>();
        for (int i = 0; i < value.Length; i++) {
            CharacterInfo info;
            if (font.GetCharacterInfo(value[i], out info)) {
                infos.Add(info);
                width += info.glyphWidth;
                height = height > Mathf.Abs(info.glyphHeight) ? height : Mathf.Abs(info.glyphHeight);
            }
        }

        TempTex = new Texture2D(width, height);
        int desX = 0, desY = 0;
        for (int j = 0; j < infos.Count; j++) {
            int w = infos[j].glyphWidth;
            int h = infos[j].glyphHeight;
            TempTex.SetPixels(desX, desY, w, h, fontCache[infos[j].index]);
            desX += infos[j].glyphWidth;
        }
        TempTex.Apply(true);
        
        if (limit.x > 0 || limit.y > 0) {
            float ratio = 1;
            int Width = TempTex.width, Height = TempTex.height;
            if (limit.x > TempTex.width) {
                if (limit.y < TempTex.height) {
                    ratio = limit.y * 1f / TempTex.height;
                }
            }
            else {
                ratio = limit.x * 1f / TempTex.width;
            }
            Width = Mathf.CeilToInt(TempTex.width * ratio);
            Height = Mathf.CeilToInt(TempTex.height * ratio);
            Width = (Width & 1) == 0 ? Width : Width - 1;
            Height = (Height & 1) == 0 ? Height : Height - 1;
            TempTex = ScaleTexture(TempTex, Width, Height);
        }
       
        byte[] png = TempTex.EncodeToPNG();
        if (Directory.Exists(savePath)) {
            string path = string.Format("{0}{1}.png", savePath, value);
            using (FileStream fs = File.Create(path)) {
                fs.Write(png, 0, png.Length);
            }
        }
        GameObject.DestroyImmediate(TempTex, true);
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight) {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int i = 0; i < result.height; i++) {
            for (int j = 0; j < result.width; j++) {
                Color newColor = source.GetPixelBilinear((float)j / result.width, (float)i / result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }
}
