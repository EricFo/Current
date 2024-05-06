using System;
using System.Text;
using System.Collections.Generic;

public static class UrlHelper
{
    /// <summary>
    /// 根据参数字典，创建Url参数
    /// </summary>
    /// <param name="param">字典参数</param>
    /// <returns>组合完成的参数字符串</returns>
    public static string BuildParam(Dictionary<string, string> param) {
        StringBuilder builder = new StringBuilder();

        int idx = 0;
        foreach (var item in param) {
            builder.AppendFormat("{0}={1}", item.Key, item.Value);
            if (idx < param.Count - 1) {
                builder.Append("&");
                idx++;
            }
        }

        return builder.ToString();
    }
}

