using UnityEditor;
using UnityEngine;
using SlotGame.Core.Reel;
using SlotGame.Reel.Setting;
using System.Collections.Generic;

[CustomEditor(typeof(ReelSetting), true)]
public class ReelSettingEditor : Editor {
    private ReelSetting setting;
    private List<string> display;
    private SortingLayer[] layers;
    private MoveMode CurrMoveMode;
    private ReelRunMode CurrRunMode;
    private MoveCurveMode CurrCurveMode;
    private ReelMode CurrReelMode;
    private ReelMaskMode CurrMaskMode;


    #region 序列化属性
    private SerializedProperty reelSetting;
    private SerializedProperty moveMode;
    private SerializedProperty moveStep;
    private SerializedProperty moveSteps;
    private SerializedProperty moveIncrement;

    private SerializedProperty runMode;
    private SerializedProperty moveSpeed;
    private SerializedProperty moveSpeeds;
    private SerializedProperty moveDistance;
    private SerializedProperty moveDistances;
    private SerializedProperty visibleSymbol;
    private SerializedProperty visibleSymbols;

    private SerializedProperty moveCurveMode;
    private SerializedProperty beginCurve;
    private SerializedProperty finishCurve;
    private SerializedProperty beginCurves;
    private SerializedProperty finishCurves;

    private SerializedProperty symbolSetting;
    private SerializedProperty symbolWidth;
    private SerializedProperty symbolHeight;
    private SerializedProperty lineSpace;
    private SerializedProperty verticalSpace;
    private SerializedProperty defaultLayer;

    private SerializedProperty reelAreaSetting;
    private SerializedProperty reelMode;
    private SerializedProperty colCount;
    private SerializedProperty rowCount;
    private SerializedProperty centerPoint;

    private SerializedProperty reelMaskSetting;
    private SerializedProperty maskMode;
    private SerializedProperty maskSetting;
    private SerializedProperty maskSettings;
    private SerializedProperty frontLayer;
    private SerializedProperty backLayer;
    #endregion

    private void OnEnable() {
        setting = target as ReelSetting;
        layers = SortingLayer.layers;
        display = new List<string>();
        for (int i = 0; i < layers.Length; i++) {
            display.Add(layers[i].name);
        }

        reelSetting = serializedObject.FindProperty("reelSetting");
        moveMode = reelSetting.FindPropertyRelative("MoveMode");
        moveStep = reelSetting.FindPropertyRelative("MoveStep");
        moveSteps = reelSetting.FindPropertyRelative("MoveSteps");
        moveIncrement = reelSetting.FindPropertyRelative("MoveIncrement");

        runMode = reelSetting.FindPropertyRelative("RunMode");
        moveSpeed = reelSetting.FindPropertyRelative("MoveSpeed");
        moveSpeeds = reelSetting.FindPropertyRelative("MoveSpeeds");
        moveDistance = reelSetting.FindPropertyRelative("MoveDistance");
        moveDistances = reelSetting.FindPropertyRelative("MoveDistances");
        visibleSymbol = reelSetting.FindPropertyRelative("VisibleSymbol");
        visibleSymbols = reelSetting.FindPropertyRelative("VisibleSymbols");

        moveCurveMode = reelSetting.FindPropertyRelative("MoveCurveMode");
        beginCurve = reelSetting.FindPropertyRelative("BeginCurve");
        finishCurve = reelSetting.FindPropertyRelative("FinishCurve");
        beginCurves = reelSetting.FindPropertyRelative("BeginCurves");
        finishCurves = reelSetting.FindPropertyRelative("FinishCurves");

        symbolSetting = serializedObject.FindProperty("symbolSetting");
        lineSpace = symbolSetting.FindPropertyRelative("LineSpacing");
        symbolWidth = symbolSetting.FindPropertyRelative("SymbolWidth");
        symbolHeight = symbolSetting.FindPropertyRelative("SymbolHeight");
        verticalSpace = symbolSetting.FindPropertyRelative("VerticalSpacing");
        defaultLayer = symbolSetting.FindPropertyRelative("DefaultLayer");

        reelAreaSetting = serializedObject.FindProperty("reelAreaSetting");
        reelMode = reelAreaSetting.FindPropertyRelative("ReelMode");
        colCount = reelAreaSetting.FindPropertyRelative("ColCount");
        rowCount = reelAreaSetting.FindPropertyRelative("RowCount");
        centerPoint = reelAreaSetting.FindPropertyRelative("CenterPoint");

        reelMaskSetting = serializedObject.FindProperty("reelMaskSetting");
        maskMode = reelMaskSetting.FindPropertyRelative("MaskMode");
        maskSetting = reelMaskSetting.FindPropertyRelative("MaskSetting");
        maskSettings = reelMaskSetting.FindPropertyRelative("MaskSettings");
        frontLayer = maskSetting.FindPropertyRelative("FrontLayer");
        backLayer = maskSetting.FindPropertyRelative("BackLayer");
    }
    public override void OnInspectorGUI() {
        serializedObject.Update();

        ReelSetting();
        SymbolSetting();
        ReelAreaSetting();
        ReelMaskSetting();

        UpdateCurrMode();
        GUIStyle style = GUI.skin.GetStyle("Button");
        style.fontSize = 20;
        if (GUILayout.Button("创建Reel区域", style, new[] { GUILayout.Height(50) })) {
            if (!Application.isPlaying) {
                CreateReelArea();
                CreateMaskArea();
            }
        }
        style.fontSize = 12;
        if (GUILayout.Button("清空Reel区域")) {
            if (!Application.isPlaying)
                ClearReelArea();
        }
        serializedObject.ApplyModifiedProperties();
    }

    #region Inspector面板显示
    /// <summary>
    /// Reel设置
    /// </summary>
    private void ReelSetting() {
        EditorGUI.indentLevel = 0;
        if (EditorGUILayout.PropertyField(reelSetting)) {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(moveMode);
            SwitchToMoveMode();
            EditorGUILayout.PropertyField(runMode);
            UpdateReelRunMode();
            EditorGUILayout.PropertyField(moveCurveMode);
            SwitchToMoveCurveMode();
        }
    }
    /// <summary>
    /// Symbol设置
    /// </summary>
    private void SymbolSetting() {
        EditorGUI.indentLevel = 0;
        if (EditorGUILayout.PropertyField(symbolSetting)) {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(symbolWidth);
            EditorGUILayout.PropertyField(symbolHeight);
            EditorGUILayout.PropertyField(lineSpace);
            EditorGUILayout.PropertyField(verticalSpace);
            EditorGUILayout.PropertyField(defaultLayer);
        }
    }
    /// <summary>
    /// Reel区域设置
    /// </summary>
    private void ReelAreaSetting() {
        EditorGUI.indentLevel = 0;
        if (EditorGUILayout.PropertyField(reelAreaSetting)) {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(reelMode);
            switch (setting.reelAreaSetting.ReelMode) {
                case ReelMode.Fill:
                    EditorGUILayout.PropertyField(colCount);
                    EditorGUILayout.PropertyField(rowCount);
                    EditorGUILayout.PropertyField(centerPoint);
                    if (setting.reelSetting.MoveCurveMode == MoveCurveMode.LineByLine) {
                        setting.reelSetting.MoveCurveMode = MoveCurveMode.Unification;
                        SwitchToMoveCurveMode();
                    }
                    break;
                case ReelMode.HorizontalSplit:
                case ReelMode.VerticalSplit:
                    ShowReelAreaSetting();
                    break;
            }
        }
    }
    /// <summary>
    /// 显示Reel区域设置
    /// </summary>
    private void ShowReelAreaSetting() {
        EditorGUILayout.PropertyField(colCount);
        EditorGUILayout.PropertyField(rowCount);
        EditorGUILayout.PropertyField(centerPoint);

        switch (setting.reelSetting.MoveCurveMode) {
            case MoveCurveMode.LineByLine:
                SetLineByLineMoveCurve();
                break;
            case MoveCurveMode.Independent:
                SetIndependentMoveCurve();
                break;
        }
    }

    /// <summary>
    /// Reel遮罩设置
    /// </summary>
    private void ReelMaskSetting() {
        EditorGUI.indentLevel = 0;

        if (EditorGUILayout.PropertyField(reelMaskSetting)) {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(maskMode);
            int id = EditorGUILayout.Popup("SortingLayer", setting.reelMaskSetting.Layer, display.ToArray());
            setting.reelMaskSetting.Layer = id;
            switch (setting.reelMaskSetting.MaskMode) {
                case ReelMaskMode.Fill:
                    if (EditorGUILayout.PropertyField(maskSetting)) {
                        EditorGUILayout.PropertyField(backLayer);
                        EditorGUILayout.PropertyField(frontLayer);
                    }
                    break;
                case ReelMaskMode.OneByOne:
                    InitMaskSetting(colCount.intValue);
                    EditorGUILayout.PropertyField(maskSettings, true);
                    break;
                case ReelMaskMode.LineByLine:
                    InitMaskSetting(rowCount.intValue);
                    EditorGUILayout.PropertyField(maskSettings, true);
                    break;
                case ReelMaskMode.Independent:
                    InitMaskSetting(rowCount.intValue * colCount.intValue);
                    EditorGUILayout.PropertyField(maskSettings, true);
                    break;
            }
        }
    }
    /// <summary>
    /// 切换移动方式
    /// </summary>
    private void SwitchToMoveMode() {
        switch (setting.reelSetting.MoveMode) {
            case MoveMode.OneByOne:
                EditorGUILayout.PropertyField(moveStep);
                EditorGUILayout.PropertyField(moveIncrement);
                break;
            case MoveMode.LineByLine:
                EditorGUILayout.PropertyField(moveStep);
                EditorGUILayout.PropertyField(moveIncrement);
                break;
            case MoveMode.Independent:
                EditorGUILayout.PropertyField(moveSteps, true);
                SetIndependentMoveSteps();
                break;
            case MoveMode.Unification:
                EditorGUILayout.PropertyField(moveStep);
                break;
        }
    }
    /// <summary>
    /// 更新运行模式
    /// </summary>
    private void UpdateReelRunMode() {
        switch (setting.reelSetting.RunMode) {
            case ReelRunMode.Unification:
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(visibleSymbol);
                EditorGUILayout.PropertyField(moveDistance);
                break;
            case ReelRunMode.Independent:
                EditorGUILayout.PropertyField(moveSpeeds, true);
                EditorGUILayout.PropertyField(visibleSymbols, true);
                EditorGUILayout.PropertyField(moveDistances, true);
                SetMoveSpeed();
                SetSymbolCount();
                SetMoveDistance();
                break;
            case ReelRunMode.MoreMoveSpeed:
                EditorGUILayout.PropertyField(moveSpeeds, true);
                EditorGUILayout.PropertyField(visibleSymbol);
                EditorGUILayout.PropertyField(moveDistance);
                SetMoveSpeed();
                break;
            case ReelRunMode.MoreSymbolCount:
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(visibleSymbols, true);
                EditorGUILayout.PropertyField(moveDistance);
                SetSymbolCount();
                break;
            case ReelRunMode.MoreMoveDistance:
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(visibleSymbol);
                EditorGUILayout.PropertyField(moveDistances, true);
                SetMoveDistance();
                break;
            case ReelRunMode.MoreSpeedAndSymbolCount:
                EditorGUILayout.PropertyField(moveSpeeds, true);
                EditorGUILayout.PropertyField(visibleSymbols, true);
                EditorGUILayout.PropertyField(moveDistance);
                SetMoveSpeed();
                SetSymbolCount();
                break;
            case ReelRunMode.MoreSpeedAndDistance:
                EditorGUILayout.PropertyField(moveSpeeds, true);
                EditorGUILayout.PropertyField(visibleSymbol);
                EditorGUILayout.PropertyField(moveDistances, true);
                SetMoveSpeed();
                SetMoveDistance();
                break;
            case ReelRunMode.MoreSymbolCountAndDistance:
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(visibleSymbols, true);
                EditorGUILayout.PropertyField(moveDistances, true);
                SetSymbolCount();
                SetMoveDistance();
                break;
        }
    }
    /// <summary>
    /// 切换移动曲线模式
    /// </summary>
    private void SwitchToMoveCurveMode() {
        switch (setting.reelSetting.MoveCurveMode) {
            case MoveCurveMode.Unification:
                EditorGUILayout.PropertyField(beginCurve);
                EditorGUILayout.PropertyField(finishCurve);
                break;
            case MoveCurveMode.Independent:
                EditorGUILayout.PropertyField(beginCurves, true);
                EditorGUILayout.PropertyField(finishCurves, true);
                SetIndependentMoveCurve();
                break;
            case MoveCurveMode.LineByLine:
                EditorGUILayout.PropertyField(beginCurves, true);
                EditorGUILayout.PropertyField(finishCurves, true);
                SetLineByLineMoveCurve();
                break;
        }
    }
    /// <summary>
    /// 更新当前模式
    /// </summary>
    private void UpdateCurrMode() {
        CurrMoveMode = setting.reelSetting.MoveMode;
        CurrRunMode = setting.reelSetting.RunMode;
        CurrCurveMode = setting.reelSetting.MoveCurveMode;
        CurrReelMode = setting.reelAreaSetting.ReelMode;
        CurrMaskMode = setting.reelMaskSetting.MaskMode;
    }
    #endregion

    #region 自动设置
    /// <summary>
    /// 根据ReelMode初始化Independent移动次数
    /// </summary>
    private void SetIndependentMoveSteps() {
        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                InitMoveSteps(colCount.intValue);
                break;
            case ReelMode.HorizontalSplit:
                InitMoveSteps(colCount.intValue * rowCount.intValue);
                break;
            case ReelMode.VerticalSplit:
                InitMoveSteps(colCount.intValue * rowCount.intValue);
                break;
        }
    }
    /// <summary>
    ///  根据ReelMode初始化Independen移动曲线
    /// </summary>
    private void SetIndependentMoveCurve() {
        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                InitMoveCurve(colCount.intValue);
                break;
            case ReelMode.HorizontalSplit:
                InitMoveCurve(colCount.intValue * rowCount.intValue);
                break;
            case ReelMode.VerticalSplit:
                InitMoveCurve(colCount.intValue * rowCount.intValue);
                break;
        }
    }
    /// <summary>
    /// 根据ReelMode初始化LineByLine移动曲线
    /// </summary>
    private void SetLineByLineMoveCurve() {
        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                InitMoveCurve(1);
                break;
            case ReelMode.HorizontalSplit:
                InitMoveCurve(rowCount.intValue);
                break;
            case ReelMode.VerticalSplit:
                InitMoveCurve(rowCount.intValue);
                break;
        }
    }
    /// <summary>
    /// 设置移动速度
    /// </summary>
    private void SetMoveSpeed() {
        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                InitMoveSpeed(colCount.intValue);
                break;
            case ReelMode.HorizontalSplit:
                InitMoveSpeed(colCount.intValue * rowCount.intValue);
                break;
            case ReelMode.VerticalSplit:
                InitMoveSpeed(colCount.intValue * rowCount.intValue);
                break;
        }
    }
    /// <summary>
    /// 设置Symbol数量
    /// </summary>
    private void SetSymbolCount() {
        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                InitSymbolCount(colCount.intValue);
                break;
            case ReelMode.HorizontalSplit:
                InitSymbolCount(colCount.intValue * rowCount.intValue);
                break;
            case ReelMode.VerticalSplit:
                InitSymbolCount(colCount.intValue * rowCount.intValue);
                break;
        }
    }
    /// <summary>
    /// 设置移动距离
    /// </summary>
    private void SetMoveDistance() {
        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                InitMoveDistance(colCount.intValue);
                break;
            case ReelMode.HorizontalSplit:
                InitMoveDistance(colCount.intValue * rowCount.intValue);
                break;
            case ReelMode.VerticalSplit:
                InitMoveDistance(colCount.intValue * rowCount.intValue);
                break;
        }
    }
    #endregion

    #region 自动计算容量大小
    /// <summary>
    /// 初始化移动次数
    /// </summary>
    /// <param name="count"></param>
    private void InitMoveSteps(int count) {
        if (CurrReelMode != setting.reelAreaSetting.ReelMode || CurrMoveMode != setting.reelSetting.MoveMode) {
            if (setting.reelSetting.MoveSteps == null || setting.reelSetting.MoveSteps.Length != count)
                setting.reelSetting.MoveSteps = new int[count];
        }
    }
    /// <summary>
    /// 初始化移动曲线
    /// </summary>
    /// <param name="count"></param>
    private void InitMoveCurve(int count) {
        if (CurrReelMode != setting.reelAreaSetting.ReelMode || CurrCurveMode != setting.reelSetting.MoveCurveMode) {
            if (setting.reelSetting.BeginCurves == null || setting.reelSetting.BeginCurves.Length != count)
                setting.reelSetting.BeginCurves = new AnimationCurve[count];
            if (setting.reelSetting.FinishCurves == null || setting.reelSetting.FinishCurves.Length != count)
                setting.reelSetting.FinishCurves = new AnimationCurve[count];
        }
    }
    /// <summary>
    /// 初始化移动速度
    /// </summary>
    /// <param name="count"></param>
    private void InitMoveSpeed(int count) {
        if (CurrReelMode != setting.reelAreaSetting.ReelMode || CurrRunMode != setting.reelSetting.RunMode) {
            if (setting.reelSetting.MoveSpeeds == null || setting.reelSetting.MoveSpeeds.Length != count)
                setting.reelSetting.MoveSpeeds = new float[count];
        }
    }
    /// <summary>
    /// 初始化移动距离
    /// </summary>
    /// <param name="count"></param>
    private void InitMoveDistance(int count) {
        if (CurrReelMode != setting.reelAreaSetting.ReelMode || CurrRunMode != setting.reelSetting.RunMode) {
            if (setting.reelSetting.MoveDistances == null || setting.reelSetting.MoveDistances.Length != count)
                setting.reelSetting.MoveDistances = new float[count];
        }
    }
    /// <summary>
    /// 初始化Symbol数量
    /// </summary>
    /// <param name="count"></param>
    private void InitSymbolCount(int count) {
        if (CurrReelMode != setting.reelAreaSetting.ReelMode || CurrRunMode != setting.reelSetting.RunMode) {
            if (setting.reelSetting.VisibleSymbols == null || setting.reelSetting.VisibleSymbols.Length != count)
                setting.reelSetting.VisibleSymbols = new int[count];
        }
    }
    /// <summary>
    /// 初始化遮罩设置
    /// </summary>
    /// <param name="count"></param>
    private void InitMaskSetting(int count) {
        if (CurrMaskMode != setting.reelMaskSetting.MaskMode) {
            if (setting.reelMaskSetting.MaskSettings == null || setting.reelMaskSetting.MaskSettings.Length != count)
                setting.reelMaskSetting.MaskSettings = new ReelSetting.MaskArgs[count];
        }
    }
    #endregion

    #region 填充Reel区域  
    /// <summary>
    /// 创建转盘区域
    /// </summary>
    private void CreateReelArea() {
        ClearReelArea();
        float offsetX, offsetY, originX, originY;
        offsetX = setting.reelAreaSetting.ColCount % 2 == 0 ? -setting.symbolSetting.SymbolWidth * 0.5f : 0;
        offsetY = setting.reelAreaSetting.RowCount % 2 == 0 ? -setting.symbolSetting.SymbolHeight * 0.5f : 0;
        originX = setting.reelAreaSetting.CenterPoint.x;
        int col = Mathf.FloorToInt(setting.reelAreaSetting.ColCount / 2f);
        if (setting.reelAreaSetting.ColCount == 1)
            originX = setting.reelAreaSetting.CenterPoint.x;
        else if (setting.reelAreaSetting.ColCount % 2 == 0)
            originX -= (setting.symbolSetting.SymbolWidth * col) + (setting.symbolSetting.LineSpacing * (col - 1)) + offsetX;
        else
            originX -= (setting.symbolSetting.SymbolWidth * col) + (setting.symbolSetting.LineSpacing * col) + offsetX;

        originY = setting.reelAreaSetting.CenterPoint.y;
        int row = Mathf.FloorToInt(setting.reelAreaSetting.RowCount / 2f) + 2;

        if (setting.reelAreaSetting.RowCount % 2 == 0)
            originY += (setting.symbolSetting.SymbolHeight * row) + (setting.symbolSetting.VerticalSpacing * (row - 1)) + offsetY;
        else
            originY += (setting.symbolSetting.SymbolHeight * row) + (setting.symbolSetting.VerticalSpacing * row) + offsetY;

        switch (setting.reelAreaSetting.ReelMode) {
            case ReelMode.Fill:
                FillReelArea(originX, originY);
                break;
            case ReelMode.HorizontalSplit:
                HorizontalReelArea(originX, originY);
                break;
            case ReelMode.VerticalSplit:
                VerticalReelArea(originX, originY);
                break;
        }
    }
    /// <summary>
    /// 清空转盘区域
    /// </summary>
    private void ClearReelArea() {
        for (int i = 0; i < setting.transform.childCount;) {
            GameObject obj = setting.transform.GetChild(i).gameObject;
            Undo.RegisterFullObjectHierarchyUndo(obj, obj.name);
            DestroyImmediate(obj);
        }
    }
    /// <summary>
    /// Fill模式填充Reel区域
    /// <param name="originX"></param>
    /// <param name="originY"></param>
    /// </summary>
    private void FillReelArea(float originX, float originY) {
        for (int i = 0; i < setting.reelAreaSetting.ColCount; i++) {
            string reelName = string.Format("Reel-{0}", i);
            GameObject Reel = new GameObject(reelName);
            Reel.AddComponent<ReelBase>();
            Reel.transform.SetParent(setting.transform);
            float X = originX + (i * setting.symbolSetting.SymbolWidth) + (setting.symbolSetting.LineSpacing * i);
            Reel.transform.position = new Vector3(X, originY, setting.reelAreaSetting.CenterPoint.z);
            Undo.RegisterCreatedObjectUndo(Reel, reelName);
        }
    }
    /// <summary>
    /// Horizontal模式填充Reel区域
    /// </summary>
    /// <param name="originX"></param>
    /// <param name="originY"></param>
    private void HorizontalReelArea(float originX, float originY) {
        int id = 0;
        for (int i = 0; i < setting.reelAreaSetting.RowCount; i++) {
            float Y = originY - (i * setting.symbolSetting.SymbolHeight) - (setting.symbolSetting.VerticalSpacing * i);
            for (int j = 0; j < setting.reelAreaSetting.ColCount; j++) {
                string reelName = string.Format("Reel-{0}", id++);
                GameObject Reel = new GameObject(reelName);
                Reel.AddComponent<ReelBonus>();
                Reel.transform.SetParent(setting.transform);
                float X = originX + (j * setting.symbolSetting.SymbolWidth) + (setting.symbolSetting.LineSpacing * j);
                Reel.transform.position = new Vector3(X, Y, setting.reelAreaSetting.CenterPoint.z);
                Undo.RegisterCreatedObjectUndo(Reel, Reel.name);
            }
        }
    }
    /// <summary>
    /// 垂直模式填充Reel区域
    /// </summary>
    /// <param name="originX"></param>
    /// <param name="originY"></param>
    private void VerticalReelArea(float originX, float originY) {
        int id = 0;
        for (int i = 0; i < setting.reelAreaSetting.ColCount; i++) {
            for (int j = 0; j < setting.reelAreaSetting.RowCount; j++) {
                string reelName = string.Format("Reel-{0}", id++);
                GameObject Reel = new GameObject(reelName);
                Reel.AddComponent<ReelBase>();
                Reel.transform.SetParent(setting.transform);
                float X = originX + (i * setting.symbolSetting.SymbolWidth) + (setting.symbolSetting.LineSpacing * i);
                float Y = originY - (j * setting.symbolSetting.SymbolHeight) - (setting.symbolSetting.VerticalSpacing * j);
                Reel.transform.position = new Vector3(X, Y, setting.reelAreaSetting.CenterPoint.z);
                Undo.RegisterCreatedObjectUndo(Reel, Reel.name);
            }
        }
    }
    /// <summary>
    /// 创建遮罩区域
    /// </summary>
    private void CreateMaskArea() {
        Vector3 Size = Vector3.zero;
        int col = Mathf.FloorToInt(setting.reelAreaSetting.ColCount / 2f);
        float originX = setting.reelAreaSetting.CenterPoint.x, offsetX = symbolWidth.floatValue * 0.5f;
        if (setting.reelAreaSetting.ColCount == 1)
            originX = setting.reelAreaSetting.CenterPoint.x;
        else if (setting.reelAreaSetting.ColCount % 2 == 0)
            originX -= (setting.symbolSetting.SymbolWidth * col) + (setting.symbolSetting.LineSpacing * (col - 1)) + offsetX;
        else
            originX -= (setting.symbolSetting.SymbolWidth * col) + (setting.symbolSetting.LineSpacing * col);
        float originY = centerPoint.vector3Value.y, offsetY = symbolHeight.floatValue + verticalSpace.floatValue;
        int row = Mathf.FloorToInt(rowCount.intValue / 2);
        if (rowCount.intValue % 2 == 0)
            originY += (symbolHeight.floatValue * row) + (verticalSpace.floatValue * (row - 1)) - (symbolHeight.floatValue * 0.5f);
        else
            originY += (symbolHeight.floatValue * row) + (verticalSpace.floatValue * row);
        switch (setting.reelMaskSetting.MaskMode) {
            case ReelMaskMode.Fill: {
                    Size += Vector3.right * ((symbolWidth.floatValue * (colCount.intValue + 2)) + (lineSpace.floatValue * (colCount.intValue + 1)));
                    Size += Vector3.up * ((symbolHeight.floatValue * rowCount.intValue) + (verticalSpace.floatValue * (rowCount.intValue - 1)));
                    GenerateMask(centerPoint.vector3Value, Size, frontLayer.intValue, backLayer.intValue);
                }
                break;
            case ReelMaskMode.OneByOne: {
                    Vector3 point = Vector3.right * originX + centerPoint.vector3Value;
                    for (int i = 0; i < colCount.intValue; i++) {
                        Size = Vector3.zero;
                        Size += Vector3.right * symbolWidth.floatValue;
                        Size += Vector3.up * ((symbolHeight.floatValue * rowCount.intValue) + (verticalSpace.floatValue * (rowCount.intValue - 1)));
                        int flayer = setting.reelMaskSetting.MaskSettings[i].FrontLayer;
                        int blayer = setting.reelMaskSetting.MaskSettings[i].BackLayer;
                        GenerateMask(point + (Vector3.right * ((symbolWidth.floatValue + lineSpace.floatValue) * i)), Size, flayer, blayer, i);
                    }
                }
                break;
            case ReelMaskMode.LineByLine: {
                    Vector3 point = Vector3.up * originY;
                    for (int i = 0; i < rowCount.intValue; i++) {
                        Size = Vector3.zero;
                        Size += Vector3.right * ((symbolWidth.floatValue * (colCount.intValue + 2)) + (lineSpace.floatValue * (colCount.intValue + 1)));
                        Size += Vector3.up * symbolHeight.floatValue;
                        int flayer = setting.reelMaskSetting.MaskSettings[i].FrontLayer;
                        int blayer = setting.reelMaskSetting.MaskSettings[i].BackLayer;
                        GenerateMask(point - (Vector3.up * (offsetY * i)), Size, flayer, blayer, i);
                    }
                }
                break;
            case ReelMaskMode.Independent: {
                    for (int i = 0; i < setting.reelMaskSetting.MaskSettings.Length; i++) {
                        Size = Vector3.zero;
                        Size += Vector3.right * symbolWidth.floatValue;
                        Size += Vector3.up * symbolHeight.floatValue;
                        Vector3 pos = Vector3.zero;
                        int c = i / rowCount.intValue;
                        int r = i % rowCount.intValue;
                        pos += Vector3.right * (originX + (c * symbolWidth.floatValue) + (lineSpace.floatValue * c));
                        pos += Vector3.up * (originY - (r * symbolHeight.floatValue) - (verticalSpace.floatValue * r));
                        int flayer = setting.reelMaskSetting.MaskSettings[i].FrontLayer;
                        int blayer = setting.reelMaskSetting.MaskSettings[i].BackLayer;
                        GenerateMask(pos, Size, flayer, blayer, i);
                    }
                }
                break;
        }
    }
    /// <summary>
    /// 生成遮罩
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="front"></param>
    /// <param name="back"></param>
    /// <param name="id"></param>
    private void GenerateMask(Vector3 pos, Vector3 size, int front, int back, int id = -1) {
        GameObject mask = new GameObject(id == -1 ? "ReelMask" : "ReelMask-" + id);
        mask.transform.position = pos;
        mask.transform.localScale = size;
        mask.transform.SetParent(setting.transform);
        SpriteMask component = mask.AddComponent<SpriteMask>();
        component.isCustomRangeActive = true;
        component.frontSortingOrder = front;
        component.backSortingOrder = back;
        component.frontSortingLayerID = layers[setting.reelMaskSetting.Layer].id;
        component.backSortingLayerID = layers[setting.reelMaskSetting.Layer].id;
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Texture/Mask/Mask-100x100.png");
        if (sprite == null)
            Debug.LogError("遮罩纹理不存在，请检查文件Assets/Texture/Mask/Mask-100x100.png");
        component.sprite = sprite;
        Undo.RegisterCreatedObjectUndo(mask, mask.name);
    }
    #endregion
}
