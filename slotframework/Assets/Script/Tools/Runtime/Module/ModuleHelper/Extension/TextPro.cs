using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UI/TextPro")]
public class TextPro : Text
{
    public float Spacing = 0f;
    public bool contentAdaptation = false;

    private Vector3 originSize;
    private Vector3 OriginSize {
        get {
            if (originSize.x == 0)
            {
                originSize = rectTransform.localScale;
            }
            return originSize;
        }
    }
    public override string text {
        get => base.text;
        set {
            if (contentAdaptation == false)
            {
                base.text = value;
            }
            else
            {
                base.text = value;
               
                if (preferredWidth <= rectTransform.rect.width)
                {
                    rectTransform.localScale = new Vector3(OriginSize.x, OriginSize.y, 1);
                }
                else
                {
                    float myScale = rectTransform.rect.width / preferredWidth * OriginSize.x;
                    rectTransform.localScale = new Vector3(myScale, myScale, 1);
                }
            }
        }
    }
#region 字符间距
    public enum AligmentType
    {
        Left,
        Center,
        Right
    }
    /// <summary>
    /// 由于可能存在换行符，因此要记录下每一行文本
    /// </summary>
    public class TextLine
    {
        // 起点索引
        public int StartVertexIndex { get; private set; }
        // 终点索引
        public int EndVertexIndex { get; private set; }

        public TextLine(int startVertexIndex, int length)
        {
            StartVertexIndex = startVertexIndex;
            //每个字符由两个三角形构成，共6个顶点
            EndVertexIndex = length * 6 - 1 + startVertexIndex;
        }
    }   
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
        if (!IsActive() || vh.currentVertCount == 0)
        {
            return;
        }
        
        // 水平对齐方式
        AligmentType horizontalAlignment;

        switch (alignment)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.LowerLeft:
                horizontalAlignment = AligmentType.Left;
                break;

            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
                horizontalAlignment = AligmentType.Center;
                break;

            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
                horizontalAlignment = AligmentType.Right;
                break;

            default:
                horizontalAlignment = AligmentType.Center;
                break;
        }

        List<UIVertex> vertexs = new List<UIVertex>();
        vh.GetUIVertexStream(vertexs);

        //根据换行符进行拆分
        string[] lineTexts = text.Split('\n');
        TextLine[] lines = new TextLine[lineTexts.Length];

        //记录每一行的顶点索引
        for (int i = 0; i < lines.Length; i++)
        {
            //最后长度+1是因为要加上换行符，最后一行没有换行符所以不需要+1
            if (i == 0)
                lines[i] = new TextLine(0, lineTexts[i].Length + 1);
            else if (i > 0 && i < lines.Length - 1)
                lines[i] = new TextLine(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length + 1);
            else
                lines[i] = new TextLine(lines[i - 1].EndVertexIndex + 1, lineTexts[i].Length);
        }

        UIVertex vt;
        //遍历每一行文本
        for (var i = 0; i < lines.Length; i++)
        {
            //根据每一行文本进行调整
            for (var j = lines[i].StartVertexIndex; j <= lines[i].EndVertexIndex; j++)
            {
                if (j < 0 || j >= vertexs.Count) continue;

                vt = vertexs[j];

                int charCount = lines[i].EndVertexIndex - lines[i].StartVertexIndex;
                //if (i == lines.Length - 1)
                //    charCount += 6;

                if (horizontalAlignment == AligmentType.Left)
                    vt.position += new Vector3(Spacing * ((j - lines[i].StartVertexIndex) / 6), 0, 0);
                else if (horizontalAlignment == AligmentType.Right)
                    vt.position += new Vector3(Spacing * (-(charCount - j + lines[i].StartVertexIndex) / 6 + 1), 0, 0);
                else if (horizontalAlignment == AligmentType.Center)
                {
                    float offset = (charCount / 6) % 2 == 0 ? 0.5f : 0f;
                    vt.position += new Vector3(Spacing * ((j - lines[i].StartVertexIndex) / 6 - charCount / 12 + offset), 0, 0);
                }

                vertexs[j] = vt;
                // 以下注意点与索引的对应关系
                if (j % 6 <= 2)
                    vh.SetUIVertex(vt, (j / 6) * 4 + j % 6);
                if (j % 6 == 4)
                    vh.SetUIVertex(vt, (j / 6) * 4 + j % 6 - 1);
            }
        }
    }
#endregion
}