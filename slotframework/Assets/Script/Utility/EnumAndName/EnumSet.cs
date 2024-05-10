/// <summary>
/// 转盘类型
/// </summary>
public enum ReelMode { 
    Fill,
    HorizontalSplit,
    VerticalSplit
}
/// <summary>
/// 转盘移动方式
/// </summary>
public enum MoveMode { 
    OneByOne,           //一个接一个，逐个递增
    LineByLine,         //一行接一行，逐行递增
    Independent,        //每个转轮独立设置
    Unification,        //所有转轮同时停
}
/// <summary>
/// 转盘运行模式
/// </summary>
public enum ReelRunMode {
    Unification,                //所有转轮使用相同设置
    Independent,                //每个转轮独立设置
    MoreMoveSpeed,              //每个转盘有不同移动速度
    MoreSymbolCount,            //每个转盘有不同Symbol数量
    MoreMoveDistance,           //每个转盘有不同移动距离
    MoreSpeedAndDistance,       //每个转盘有不同移动速度和距离
    MoreSpeedAndSymbolCount,    //每个转盘有不同移动速度和Symbol数量
    MoreSymbolCountAndDistance, //每个转盘有不同Symbol数量和移动距离
}
/// <summary>
/// 转盘移动曲线模式
/// </summary>
public enum MoveCurveMode {
    Unification,   //所有转轮都一样
    LineByLine,    //一行一行设置
    Independent,   //每个转轮独立设置
}
/// <summary>
/// 转盘遮罩模式
/// </summary>
public enum ReelMaskMode { 
   Fill,          //直接填充全部
   OneByOne,      //一列一列填充
   LineByLine,    //一行一行填充
   Independent,   //每个转轮独立设置
}
/// <summary>
/// 游戏状态
/// </summary>
public enum GameState { 
    Base,
    Free,
    Bonus,
    JackPot, 
}
/// <summary>
/// 赔付方式
/// </summary>
public enum PayMode { 
    Line,       //线赔
    Table,      //表赔
    JackPot,    //JackPot
}


