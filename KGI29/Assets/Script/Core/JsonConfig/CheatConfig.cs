using UnityEngine;
using System.Collections.Generic;

public class CheatConfig
{
    public Dictionary<string, string[][]> Cheats = new Dictionary<string, string[][]> {
        {
            "TriggerFreeGame" ,new string[][]{
                new string[]{ "N", "Scatter", "T" },
                new string[]{ "A", "PIC02", "PIC03"},
                new string[]{ "PIC05", "Scatter", "A" },
                new string[]{ "Q", "J", "PIC04" },
                new string[]{ "K", "Scatter", "N" },
            }
        },
    };

    public string[] GetCheats(string name,int id) {
        if (Cheats.ContainsKey(name)){
            return Cheats[name][id];
        }
        else{
            Debug.LogErrorFormat("不存在{0}作弊表");
            return default;
        }
    }
}
