using System;
using UnityEngine;

namespace SlotGame.Reel.Args {
    [Serializable]
    public class ReelSettingArgs {
        public int reelID;                           //转轮ID
        public int moveStep;                         //移动步数
        public int rowCount;                         //有多少行
        public int colCount;                         //有多少列
        public int symbolCount;                      //Symbol数量
        public int visibleSymbolCount;               //可见区域Symbol数量
        public float moveSpeed;                      //移动速度
        public float moveDistance;                   //移动距离
        public int defaultLayer;                     //默认层级
        public AnimationCurve beginCurve;            //起始曲线
        public AnimationCurve finishCurve;           //完成曲线
        public string[] reelStripes { get; set; }    //转轮数据
    }

    public class ReelSpinArgs {
        public int stopID = 0;      //最终结果的停止ID
        public int WaitStep = 0;    //触发Hyper之前附加的等待次数
        public int hyperStep = 0;   //触发Hyper之后附加的移动次数
        public float hyperSpeed = 0.03f; //Hyper速度
        public bool isHyper = false;//是否触发了Hyper
        public string[] resultSymbols;
    }
}
