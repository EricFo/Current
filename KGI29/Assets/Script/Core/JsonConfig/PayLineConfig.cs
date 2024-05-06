using System.Collections.Generic;
using UnityEngine;

namespace SlotGame.Config
{
    public class PayLineConfig
    {
        public Dictionary<string,int[][]> Points = new Dictionary<string, int[][]> {
            { 
                "Base",new int[][]{
                    new int[]{ 0, 0, 0, 0, 0 },
                }             
            }                 
        };

        public int[][] GetPayLinePoint(string name) {
            if (Points.ContainsKey(name))
            {
                return Points[name];
            }
            else
            {
                Debug.LogErrorFormat("未找到名为{0}的LinePoint,请检查{1}文件设置", name, typeof(PayLineConfig).Name);
                return default;
            }
        }
    }                         
}                             
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              
                              