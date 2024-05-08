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
        public AnimationCurve reverseCurve;          //倒转曲线
        public string[] reelStripes { get; set; }    //转轮数据
    }

    public class ReelSpinArgs {
        public int stopID = 0;      //最终结果的停止ID
        public int WaitStep = 0;    //触发Hyper之前附加的等待次数
        public int hyperStep = 40;   //触发Hyper之后附加的移动次数
        public float hyperSpeed = 0.03f; //Hyper速度
        public bool isHyper = false;//是否触发了Hyper
        public bool isLong = false;//是否需要加长旋转过程
        public bool isFreeStart = false;
        public bool isReverse = false;//本轮是否有倒转，有的话需要让最下面一个Symbol为Scatter
        public int longStep = 10;
        public int freeStartStep = 20;
        public string[] resultSymbols;
        public int coinLevel = 1;
        public int coinScore = 0;
        public JackpotType coinJPType = JackpotType.Null;
        public int stripeID = 0;
    }
}
