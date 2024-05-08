using TMPro;
using UnityEngine;
using System.Text;

public static class TextMeshProHelper {
    /// <summary>
    /// 设置遮罩交互方式
    /// </summary>
    /// <param name="tmp"></param>
    /// <param name="maskInteraction">交互方式</param>
    public static void SetMaskInteraction(this TextMeshPro tmp, SpriteMaskInteraction maskInteraction) {
        float stencilValue;

        switch (maskInteraction) {
            case SpriteMaskInteraction.None:
                stencilValue = 8f;
                break;
            case SpriteMaskInteraction.VisibleInsideMask:
                stencilValue = 2f;
                break;
            case SpriteMaskInteraction.VisibleOutsideMask:
                stencilValue = 3f;
                break;
            default:
                stencilValue = 8f;
                break;
        }
        tmp.fontSharedMaterial.SetFloat("_Stencil", 0);
        tmp.fontSharedMaterial.SetFloat("_StencilOp", 0);
        tmp.fontSharedMaterial.SetFloat("_StencilComp", stencilValue);
        tmp.fontSharedMaterial.SetFloat("_StencilWriteMask", 255);
        tmp.fontSharedMaterial.SetFloat("_StencilReadMask", 255);

        MeshRenderer meshRenderer = tmp.GetComponent<MeshRenderer>();
        if (meshRenderer != null) {
            meshRenderer.sharedMaterial.SetFloat("_Stencil", 0);
            meshRenderer.sharedMaterial.SetFloat("_StencilOp", 0);
            meshRenderer.sharedMaterial.SetFloat("_StencilComp", stencilValue);
            meshRenderer.sharedMaterial.SetFloat("_StencilWriteMask", 255);
            meshRenderer.sharedMaterial.SetFloat("_StencilReadMask", 255);
        }
    }

    /// <summary>
    /// 根据数值返回能够正确在TextMeshPro中显示的字符串
    /// (正常金币字体使用 "Coin" 图集， 灰色金币字体使用 "Coin_G" 图集)
    /// </summary>
    /// <param name="spriteName">要使用的图集名</param>
    /// <param name="value">源数值</param>
    /// <returns></returns>
    public static string GetArtText(string spriteName, int value) {
        StringBuilder str = new StringBuilder();
        char[] chars = value.ToString().ToCharArray();
        foreach (char val in chars) {
            str.Append(string.Format("<sprite=\"{0}\" index={1}>", spriteName, val));
        }
        return str.ToString();
    }
}
