using System;
using UnityEngine;

namespace SlotGame.Reel.Setting {
    [Serializable]
    public class ReelSetting : MonoBehaviour {
        [Header("-------------Reel设置-------------")]
        public SingleReelSetting reelSetting;

        [Header("-------------Symbol设置-------------")]
        public SymbolSetting symbolSetting;

        [Header("-------------Reel区域设置-------------")]
        public ReelAreaSetting reelAreaSetting;

        [Header("-------------Reel遮罩设置-------------")]
        public ReelMaskSetting reelMaskSetting;

        public const int TopHideSymbol = 2;
        public const int BottomHideSymbol = 1;

        [Serializable]
        public class SingleReelSetting {
            [Tooltip("Reel移动方式")]
            public MoveMode MoveMode;
            [Tooltip("Reel移动次数")]
            public int MoveStep;
            [Tooltip("每个Reel的移动次数")]
            public int[] MoveSteps;
            [Tooltip("Reel移动增量")]
            public int MoveIncrement;

            [Tooltip("Reel运行模式")]
            public ReelRunMode RunMode;
            [Tooltip("每个Reel的移动速度")]
            public float MoveSpeed;
            [Tooltip("每个Reel的移动速度")]
            public float[] MoveSpeeds;
            [Tooltip("Reel每次移动的距离")]
            public float MoveDistance;
            [Tooltip("Reel每次移动的距离")]
            public float[] MoveDistances;
            [Tooltip("每个Reel的可见Symbol数量")]
            public int VisibleSymbol;
            [Tooltip("每个Reel的Symbol数量")]
            public int[] VisibleSymbols;

            [Tooltip("移动曲线模式")]
            public MoveCurveMode MoveCurveMode;
            [Tooltip("Spin时的移动曲线")]
            public AnimationCurve BeginCurve;
            [Tooltip("每个Reel,Spin时的移动曲线")]
            public AnimationCurve[] BeginCurves;
            [Tooltip("Stop时的移动曲线")]
            public AnimationCurve FinishCurve;
            [Tooltip("每个Reel,Spin时的移动曲线")]
            public AnimationCurve[] FinishCurves;
        }
        [Serializable]
        public class SymbolSetting {
            [Tooltip("单个Symbol宽度")]
            public float SymbolWidth;
            [Tooltip("单个Symbol高度")]
            public float SymbolHeight;
            [Tooltip("Symbol水平间距")]
            public float LineSpacing;
            [Tooltip("Symbol垂直间距")]
            public float VerticalSpacing;
            [Tooltip("每个Symbol的默认层级")]
            public int DefaultLayer;
        }
        [Serializable]
        public class ReelAreaSetting {
            [Tooltip("Reel类型")]
            public ReelMode ReelMode;
            [Tooltip("Reel列数")]
            [Range(1, 100)] public int ColCount = 1;
            [Tooltip("Reel行数")]
            [Range(1, 100)] public int RowCount = 1;
            [Tooltip("Reel区域中心点的世界坐标位置")]
            public Vector3 CenterPoint;
        }
        [Serializable]
        public class ReelMaskSetting {
            public ReelMaskMode MaskMode;
            public int Layer;
            public MaskArgs MaskSetting;
            public MaskArgs[] MaskSettings;
        }
        [Serializable]
        public class MaskArgs {
            public int BackLayer;
            public int FrontLayer;
        }

        #region 辅助工具
        /// <summary>
        /// 获取移动次数
        /// </summary>
        /// <param name="reelID">转盘ID</param>
        public int GetMoveStep(int reelID) {
            switch (reelSetting.MoveMode) {
                case MoveMode.OneByOne:
                    return reelSetting.MoveStep + (reelSetting.MoveIncrement * reelID);
                case MoveMode.LineByLine:
                    return GetLinByLineMoveStep(reelID);
                case MoveMode.Independent:
                    return reelSetting.MoveSteps[reelID];
                case MoveMode.Unification:
                    return reelSetting.MoveStep;
                default:
                    Debug.LogError("未找到相关模式，请检查移动模式设置");
                    return 0;
            }
        }
        /// <summary>
        /// 获取SymbolCount
        /// </summary>
        /// <param name="reelID"></param>
        /// <returns></returns>
        public float GetMoveSpeed(int reelID) {
            switch (reelSetting.RunMode) {
                case ReelRunMode.Unification:
                case ReelRunMode.MoreSymbolCount:
                case ReelRunMode.MoreMoveDistance:
                case ReelRunMode.MoreSymbolCountAndDistance:
                    return reelSetting.MoveSpeed;

                case ReelRunMode.Independent:
                case ReelRunMode.MoreMoveSpeed:
                case ReelRunMode.MoreSpeedAndDistance:
                case ReelRunMode.MoreSpeedAndSymbolCount:
                    return reelSetting.MoveSpeeds[reelID];
                default:
                    Debug.LogError("未找到相关模式，请检查移动速度设置");
                    return 0;
            }
        }
        /// <summary>
        /// 获取SymbolCount
        /// </summary>
        /// <param name="reelID"></param>
        /// <returns></returns>
        public int GetSymbolCount(int reelID) {
            switch (reelSetting.RunMode) {
                case ReelRunMode.Unification:
                case ReelRunMode.MoreMoveSpeed:
                case ReelRunMode.MoreMoveDistance:
                case ReelRunMode.MoreSpeedAndDistance:
                    return reelSetting.VisibleSymbol + TopHideSymbol + BottomHideSymbol;

                case ReelRunMode.Independent:
                case ReelRunMode.MoreSymbolCount:
                case ReelRunMode.MoreSpeedAndSymbolCount:
                case ReelRunMode.MoreSymbolCountAndDistance:
                    return reelSetting.VisibleSymbols[reelID] + TopHideSymbol + BottomHideSymbol;
                default:
                    Debug.LogError("未找到相关模式，请检查Symbol数量设置");
                    return 0;
            }
        }
        /// <summary>
        /// 获取可见Symbol数量
        /// </summary>
        /// <param name="reelID"></param>
        /// <returns></returns>
        public int GetVisibleSymbolCount(int reelID) {
            switch (reelSetting.RunMode) {
                case ReelRunMode.Unification:
                case ReelRunMode.MoreMoveSpeed:
                case ReelRunMode.MoreMoveDistance:
                case ReelRunMode.MoreSpeedAndDistance:
                    return reelSetting.VisibleSymbol;

                case ReelRunMode.Independent:
                case ReelRunMode.MoreSymbolCount:
                case ReelRunMode.MoreSpeedAndSymbolCount:
                case ReelRunMode.MoreSymbolCountAndDistance:
                    return reelSetting.VisibleSymbols[reelID];
                default:
                    Debug.LogError("未找到相关模式，请检查Symbol数量设置");
                    return 0;
            }
        }
        /// <summary>
        /// 获取移动距离
        /// </summary>
        /// <param name="reelID"></param>
        /// <returns></returns>
        public float GetMoveDistance(int reelID) {
            switch (reelSetting.RunMode) {
                case ReelRunMode.Unification:
                case ReelRunMode.MoreMoveSpeed:
                case ReelRunMode.MoreSymbolCount:
                case ReelRunMode.MoreSpeedAndSymbolCount:
                    return reelSetting.MoveDistance;

                case ReelRunMode.Independent:
                case ReelRunMode.MoreMoveDistance:
                case ReelRunMode.MoreSpeedAndDistance:
                case ReelRunMode.MoreSymbolCountAndDistance:
                    return reelSetting.MoveDistances[reelID];
                default:
                    Debug.LogError("未找到相关模式，请检查移动距离设置");
                    return 0;
            }
        }
        /// <summary>
        /// 获取逐行模式下的移动次数
        /// </summary>
        /// <param name="reelID">转盘ID</param>
        /// <returns></returns>
        private int GetLinByLineMoveStep(int reelID) {
            switch (reelAreaSetting.ReelMode) {
                case ReelMode.Fill: {
                        return reelSetting.MoveStep;
                    }
                case ReelMode.HorizontalSplit: {
                        int id = Mathf.FloorToInt(reelID / reelAreaSetting.ColCount);
                        return reelSetting.MoveStep + id * reelSetting.MoveIncrement;
                    }
                case ReelMode.VerticalSplit: {
                        int id = reelID % reelAreaSetting.RowCount;
                        return reelSetting.MoveStep + id * reelSetting.MoveIncrement;
                    }
                default:
                    Debug.LogError("状态不存在！请检查Reel区域设置");
                    return 0;
            }
        }
        /// <summary>
        /// 获取开始曲线
        /// </summary>
        /// <param name="reelID">转盘ID</param>
        /// <returns></returns>
        public AnimationCurve GetBeginCurve(int reelID) {
            switch (reelSetting.MoveCurveMode) {
                case MoveCurveMode.Unification:
                    return reelSetting.BeginCurve;
                case MoveCurveMode.LineByLine:
                    return GetLineBylineMoveCurve(reelID, 0);
                case MoveCurveMode.Independent:
                    return reelSetting.BeginCurves[reelID];
                default:
                    Debug.LogError("未找到相关模式，请检查移动曲线设置");
                    return default;
            }
        }
        /// <summary>
        /// 获取结束曲线
        /// </summary>
        /// <param name="reelID">转盘ID</param>
        /// <returns></returns>
        public AnimationCurve GetFinishCurve(int reelID) {
            switch (reelSetting.MoveCurveMode) {
                case MoveCurveMode.Unification:
                    return reelSetting.FinishCurve;
                case MoveCurveMode.LineByLine:
                    return GetLineBylineMoveCurve(reelID, 1);
                case MoveCurveMode.Independent:
                    return reelSetting.FinishCurves[reelID];
                default:
                    Debug.LogError("未找到相关模式，请检查移动曲线设置");
                    return default;
            }
        }
        /// <summary>
        /// 获取逐行模式下的移动曲线
        /// </summary>
        /// <param name="reelID">转盘ID</param>
        /// <param name="curveID">0：Begin 1:Finish</param>
        /// <returns></returns>
        public AnimationCurve GetLineBylineMoveCurve(int reelID, int curveID) {
            switch (reelAreaSetting.ReelMode) {
                case ReelMode.Fill: {
                        return curveID == 0 ? reelSetting.BeginCurves[0] : reelSetting.FinishCurves[0];
                    }
                case ReelMode.HorizontalSplit: {
                        int id = Mathf.FloorToInt(reelID / reelAreaSetting.ColCount);
                        return curveID == 0 ? reelSetting.BeginCurves[id] : reelSetting.FinishCurves[id];
                    }
                case ReelMode.VerticalSplit: {
                        int id = reelID % reelAreaSetting.RowCount;
                        return curveID == 0 ? reelSetting.BeginCurves[id] : reelSetting.FinishCurves[id];
                    }
                default:
                    Debug.LogError("状态不存在！请检查Reel区域设置");
                    return default;
            }
        }
        #endregion
    }
}
