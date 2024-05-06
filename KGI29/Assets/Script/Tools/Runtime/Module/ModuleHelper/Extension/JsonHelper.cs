using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class JsonHelper {
    private static string PATH = Application.streamingAssetsPath + "/Config/";

    /// <summary>
    /// 保存Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    public static void SaveJson<T>(T value) where T : class {
        string filename = string.Format("{0}{1}.json", PATH, typeof(T).Name);
        if (!File.Exists(filename))
            Directory.CreateDirectory(PATH);
        else
            File.Delete(filename);

        using (StreamWriter sw = File.CreateText(filename)) {
            sw.Write(JsonConvert.SerializeObject(value));
        }
    }
    /// <summary>
    /// 读取Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ReadJson<T>() where T : class {
        string filename = string.Format("{0}{1}.json", PATH, typeof(T).Name);
        if (!File.Exists(filename)) {
            Debug.Log(filename);
            Debug.LogError("Json文件丢失");
            return null;
        }
        string text = File.ReadAllText(filename);
        T obj = JsonConvert.DeserializeObject<T>(text);
        return obj;
    }
}
