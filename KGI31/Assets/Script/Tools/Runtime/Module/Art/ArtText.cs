using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class ArtText : MonoBehaviour {
    [SerializeField] private float maxWidth;
    [SerializeField] private bool isAdaption = false;
    [SerializeField] private Font font;

    public Font Font {
        get { return font; }
        set { SetFont(value); }
    }
    public string text {
        get {
            if (RendererComponent.sprite != null) {
                return RendererComponent.sprite.name;
            } else {
                return string.Empty;
            }
        }
        set { SetContent(value); }
    }

    private int HashCode { get; set; }
    private Texture2D TempTex {
        get {
            if (tempTex == null) {
                tempTex = new Texture2D(100, 100);
            }
            return tempTex;
        }
    }
    private Vector3 DefaultSize {
        get {
            if (defaultSize == Vector3.zero) {
                defaultSize = transform.localScale;
            }
            return defaultSize;
        }
    }
    private SpriteRenderer RendererComponent {
        get {
            if (rendererComponent == null) {
                rendererComponent = GetComponent<SpriteRenderer>();
            }
            return rendererComponent;
        }
    }

    private Texture2D tempTex;
    private Vector3 defaultSize;
    private bool isInitialize = false;
    private SpriteRenderer rendererComponent;
    private List<CharacterInfo> infos = new List<CharacterInfo>();
    private static Dictionary<int, Dictionary<int, Color[]>> fontCache;

    //private void Awake() {
    //    SetFont(font);
    //}

    /// <summary>
    /// Set a new font for text content
    /// </summary>
    /// <param name="newfont"></param>
    public void SetFont(Font newfont) {
        infos.Clear();
        this.font = newfont;
        HashCode = font.GetHashCode();
        Texture2D MainTex = font.material.mainTexture as Texture2D;
        if (fontCache == null) {
            fontCache = new Dictionary<int, Dictionary<int, Color[]>>();
        }
        if (!fontCache.ContainsKey(HashCode)) {
            fontCache.Add(HashCode, new Dictionary<int, Color[]>());
            foreach (var item in font.characterInfo) {
                int x = (int)(item.uvBottomLeft.x * MainTex.width);
                int y = (int)(item.uvBottomLeft.y * MainTex.height);
                int w = item.glyphWidth;
                int h = item.glyphHeight;
                Color[] colors = MainTex.GetPixels(x, y, w, h);
                fontCache[HashCode].Add(item.index, colors);
            }
        }
        if (isInitialize == true) {
            SetContent(text);
        }
        isInitialize = true;
    }
    /// <summary>
    /// Set text content based on string
    /// </summary>
    /// <param name="value"></param>
    public void SetContent(string value) {
        if (isInitialize == false) {
            SetFont(font);
        }
        Destroy(RendererComponent.sprite);
        Sprite cache = CreateNewSprite(value);
        if (cache != null) {
            //Debug.Log(value + ":" + cache.texture.width);
            SpriteAdaptive(cache.texture.width);
        }
        RendererComponent.sprite = cache;
    }
    /// <summary>
    /// Set the font maximum display range
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxWidth(float value) {
        if (value > 0) {
            maxWidth = value;
        } else {
            Debug.LogError("Font limits must be positive integers");
        }
    }
    /// <summary>
    /// Get adaptive zoom size based on string content
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Vector3 GetScale(string value) {
        int width = 0;
        for (int i = 0; i < value.Length; i++) {
            CharacterInfo info;
            if (font.GetCharacterInfo(value[i], out info)) {
                width += info.glyphWidth;
            }
        }
        float ratio = (maxWidth / width);
        if (ratio < DefaultSize.x) {
            return new Vector3(DefaultSize.x * ratio, DefaultSize.y * ratio, DefaultSize.z);
        } else {
            return DefaultSize;
        }
    }
    /// <summary>
    /// Adaptively scale the sprite according to the font maximum width limit
    /// </summary>
    /// <param name="width"></param>
    private void SpriteAdaptive(int width) {
        if (isAdaption == true) {
            float ratio = (maxWidth / width);
            if (ratio < DefaultSize.x) {
                transform.localScale = new Vector3(DefaultSize.x * ratio, DefaultSize.y * ratio, DefaultSize.z);
            } else {
                transform.localScale = DefaultSize;
            }
        }
    }
    /// <summary>
    /// Create a new sprite based on the string content
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private Sprite CreateNewSprite(string value) {
        if (value.Length > 0) {
            infos.Clear();
            int width = 0, height = 0;
            for (int i = 0; i < value.Length; i++) {
                CharacterInfo info;
                if (font.GetCharacterInfo(value[i], out info)) {
                    infos.Add(info);
                    width += info.glyphWidth;
                    height = height > Mathf.Abs(info.glyphHeight) ? height : Mathf.Abs(info.glyphHeight);
                }
            }

            TempTex.Resize(width, height);
            int desX = 0, desY = 0;
            for (int j = 0; j < infos.Count; j++) {
                int w = infos[j].glyphWidth;
                int h = infos[j].glyphHeight;
                TempTex.SetPixels(desX, desY, w, h, fontCache[HashCode][infos[j].index]);
                desX += infos[j].glyphWidth;
            }
            TempTex.Apply(true);

            Sprite newSprite = Sprite.Create(TempTex, Rect.MinMaxRect(0, 0, width, height), Vector2.one * 0.5f, 100, 0, SpriteMeshType.FullRect, Vector4.zero, false);
            newSprite.name = value;
            return newSprite;
        } else {
            return null;
        }
    }
}
