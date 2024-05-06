using UnityEngine;
using System.Collections.Generic;
namespace SlotGame.Config
{
    public class PayTableConfig
    {
        public Dictionary<string, int[]> PayTables = new Dictionary<string, int[]>() {
        { "9", new int[]{ 0,0,50,100,150 } },
    };
        /// <summary>
        /// 获取赔付金额
        /// </summary>
        /// <param name="symbolName"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int GetPayMoney(string symbolName, int count)
        {
            if (PayTables.ContainsKey(symbolName))
            {
                if (count < PayTables[symbolName].Length)
                {
                    return PayTables[symbolName][count];
                }
                else
                {
                    Debug.LogErrorFormat("超出{0}赔付表所对应的佩服数量上限，请检查Symbol数量设置或PayTableConfig配置文件", symbolName);
                    return default;
                }
            }
            else
            {
                Debug.LogErrorFormat("未找到和{0}对应的赔付表，请检查Symbol名称设置或PayTableConfig配置文件", symbolName);
                return default;
            }
        }
    }
}
