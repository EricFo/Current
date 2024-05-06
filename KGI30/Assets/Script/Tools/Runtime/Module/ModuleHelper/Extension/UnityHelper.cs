using System;
using System.Linq;
using UnityEngine;

public static class BasicTypeConversionHelper {
    /// <summary>
    /// 将一个字符串转换成Color
    /// </summary>
    /// <param name="color">Color格式的字符串</param>
    /// <returns>转换后的color</returns>
    public static Color ParseToColor(this string obj) {
        string[] value = obj.Substring(obj.IndexOf("(")).Replace("(", "").Replace(")", "").Split(',');
        return new Color(value[0].ParseToSingle(), value[1].ParseToSingle(), value[2].ParseToSingle(), value[3].ParseToSingle());
    }
    /// <summary>
    /// 将一个字符串转换为Rect
    /// </summary>
    /// <param name="rect">Rect格式的字符串</param>
    /// <returns>转换后的Rect</returns>
    public static Rect ParseToRect(this string obj) {
        string[] value = obj.Replace("(", "").Replace(")", "").Split(',');
        string[] x = value[0].Split(':'); string[] y = value[1].Split(':');
        string[] w = value[2].Split(':'); string[] h = value[3].Split(':');
        return new Rect(x[1].ParseToSingle(), y[1].ParseToSingle(), w[1].ParseToSingle(), h[1].ParseToSingle());
    }
    /// <summary>
    /// 将一个字符串转换为Vector2向量
    /// </summary>
    /// <param name="v2">Vector2向量转换的字符串</param>
    /// <returns>转换后的Vector2向量</returns>
    public static Vector2 ParseToVector2(this string obj) {
        string[] value = obj.Replace("(", "").Replace(")", "").Split(',');
        return new Vector2(value[0].ParseToSingle(), value[1].ParseToSingle());
    }
    /// <summary>
    /// 将一个字符串转换为Vector2Int向量
    /// </summary>
    /// <param name="v2">Vector2向量转换的字符串</param>
    /// <returns>转换后的Vector2Int向量</returns>
    public static Vector2Int ParseToVector2Int(this string obj) {
        string[] value = obj.Replace("(", "").Replace(")", "").Split(',');
        return new Vector2Int(value[0].ParseToInt32(), value[1].ParseToInt32());
    }
    /// <summary>
    /// 将一个字符串转换为Vector3向量
    /// </summary>
    /// <param name="v3">Vector3向量转换的字符串</param>
    /// <returns>转换后的Vector3向量</returns>
    public static Vector3 ParseToVector3(this string obj) {
        string[] value = obj.Replace("(", "").Replace(")", "").Split(',');
        return new Vector3(value[0].ParseToSingle(), value[1].ParseToSingle(), value[2].ParseToSingle());
    }
    /// <summary>
    /// 将一个字符串转换为Vector3Int向量
    /// </summary>
    /// <param name="v3">Vector3向量转换的字符串</param>
    /// <returns>转换后的Vector3Int向量</returns>
    public static Vector3Int ParseToVector3Int(this string obj) {
        string[] value = obj.Replace("(", "").Replace(")", "").Split(',');
        return new Vector3Int(value[0].ParseToInt32(), value[1].ParseToInt32(), value[2].ParseToInt32());
    }
    /// <summary>
    /// 将一个字符串转换为四元数
    /// </summary>
    /// <param name="q4">由四元数转换的字符串</param>
    /// <returns>转换后的四元数</returns>
    public static Quaternion ParseToQuaternion(this string obj) {
        string[] value = obj.Replace("(", "").Replace(")", "").Split(',');
        return new Quaternion(value[0].ParseToSingle(), value[1].ParseToSingle(), value[2].ParseToSingle(), value[3].ParseToSingle());
    }

    /// <summary>
    /// 将一个字符串转换为Byte
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>byte值</returns>
    public static byte ParseToByte(this string obj) { return Convert.ToByte(obj); }
    /// <summary>
    /// 将一个字符串转换为SByte
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>sbyte值</returns>
    public static sbyte ParseToSByte(this string obj) { return Convert.ToSByte(obj); }

    /// <summary>
    /// 将一个字符串转换为Int
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>int值</returns>
    public static int ParseToInt32(this string obj) { return Convert.ToInt32(obj); }
    /// <summary>
    /// 将一个字符串转换为UInt
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>uint值</returns>
    public static uint ParseToUInt32(this string obj) { return Convert.ToUInt32(obj); }
    /// <summary>
    /// 将一个字符串转换为Long
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>long值</returns>
    public static long ParseToInt64(this string obj) { return Convert.ToInt64(obj); }
    /// <summary>
    /// 将一个字符串转换为ULong
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>ulong值</returns>
    public static ulong ParseToUInt64(this string obj) { return Convert.ToUInt64(obj); }
    /// <summary>
    /// 将一个字符串转换为Short
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>short值</returns>
    public static short ParseToInt16(this string obj) { return Convert.ToInt16(obj); }
    /// <summary>
    /// 将一个字符串转换为UShort
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>ushort值</returns>
    public static ushort ParseToUInt16(this string obj) { return Convert.ToUInt16(obj); }

    /// <summary>
    /// 将一个字符串转换为Float
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>float值</returns>
    public static float ParseToSingle(this string obj) { return Convert.ToSingle(obj); }
    /// <summary>
    /// 将一个字符串转换为Double
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>double值</returns>
    public static double ParseToDouble(this string obj) { return Convert.ToDouble(obj); }
    /// <summary>
    /// 将一个字符串转换为Decimal
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>decimal值</returns>
    public static decimal ParseToDecimal(this string obj) { return Convert.ToDecimal(obj); }

    /// <summary>
    /// 将一个字符串转换为Bool
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>bool值</returns>
    public static bool ParseToBoolean(this string obj) { return Convert.ToBoolean(obj); }
    /// <summary>
    /// 将一个字符串转换为Char
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>char值</returns>
    public static char ParseToChar(this string obj) { return Convert.ToChar(obj); }

    /// <summary>
    /// 将一个字符串转换为枚举
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type">枚举的类型</param>
    /// <returns>枚举值</returns>
    public static object ParseToEnum(this string obj, Type type) { return Enum.Parse(type, obj); }
}

public static class AnimatorHelper {
    /// <summary>
    /// Get the remaining duration of the currently playing animation
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static float GetRemainLength(this Animator anim, int layer = 0) {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(layer);
        float normalize = Mathf.Repeat(info.normalizedTime, 1);
        return info.length - (info.length * normalize);
    }
    /// <summary>
    /// Get animation duration based on animation clip ID
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="hashId"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static float GetAnimationLength(this Animator anim, int hashId, int layer = 0) {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        AnimationClip info = clips.Where((item) => { return Animator.StringToHash(item.name).Equals(hashId); }).FirstOrDefault();
        return info.length;
    }
    /// <summary>
    /// Get animation duration based on animation clip name
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="name"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static float GetAnimationLength(this Animator anim, string name, int layer = 0) {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        AnimationClip info = clips.Where((item) => { return item.name.Equals(name); }).FirstOrDefault();
        return info.length;
    }
    /// <summary>
    /// Check if the current animation is playing based on the animation ID
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="hashId"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool CheckAnimationIsPlaying(this Animator anim, int hashId, int layer = 0) {
        AnimatorStateInfo infos = anim.GetCurrentAnimatorStateInfo(layer);
        return infos.shortNameHash.Equals(hashId);
    }
    /// <summary>
    /// Check if the current animation is playing based on the animation name
    /// </summary>
    /// <param name="anim"></param>
    /// <param name="name"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool CheckAnimationIsPlaying(this Animator anim, string name, int layer = 0) {
        AnimatorStateInfo infos = anim.GetCurrentAnimatorStateInfo(layer);
        return infos.IsName(name);
    }
}

public static class TransformHelper {
    /// <summary>
    /// 重置所有Local属性，position\rotation\scale
    /// </summary>
    /// <param name="self"></param>
    public static void ResetLocalProperty(this Transform self) {
        self.localPosition = UnityEngine.Vector3.zero;
        self.localRotation = UnityEngine.Quaternion.identity;
        self.localScale = UnityEngine.Vector3.one;
    }
    /// <summary>
    /// 重置所有全局属性，position\rotation\scale
    /// </summary>
    /// <param name="self"></param>
    public static void ResetGlobalProperty(this Transform self) {
        self.position = UnityEngine.Vector3.zero;
        self.rotation = UnityEngine.Quaternion.identity;
        self.localScale = UnityEngine.Vector3.one;
    }
}
