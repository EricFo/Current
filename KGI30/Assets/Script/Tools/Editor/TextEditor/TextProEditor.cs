using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(TextPro))]
public class TextProEditor : UnityEditor.UI.TextEditor
{
    SerializedProperty property;
    SerializedProperty contentAdaptation;
    protected override void OnEnable()
    {
        base.OnEnable();
        property = serializedObject.FindProperty("Spacing");
        contentAdaptation = serializedObject.FindProperty("contentAdaptation");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.Space();
        GUIStyle style = GUI.skin.customStyles[0];
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 11;
        EditorGUILayout.PrefixLabel("Extend", style, style);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(property);
        contentAdaptation.boolValue = EditorGUILayout.Toggle("Content Adaptation", !contentAdaptation.boolValue);

        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("GameObject/UI/TextPro")]
    public static void CreateTextPro()
    {
        GameObject obj = new GameObject("TextPro");
        GameObjectUtility.SetParentAndAlign(obj, Selection.activeGameObject);
        obj.AddComponent<RectTransform>();
        obj.AddComponent<TextPro>();
        if (obj.GetComponentsInParent<Canvas>().Length <= 0)
        {
            GameObject canvas = new GameObject("Canvas");
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            GameObjectUtility.SetParentAndAlign(obj, canvas);
            GameObjectUtility.SetParentAndAlign(canvas, Selection.activeGameObject);
            Undo.RegisterCreatedObjectUndo(canvas, "Create" + canvas.name);
        }
        
        //为了使操作可以撤销，因此需要先注册
        Undo.RegisterCreatedObjectUndo(obj, "Create" + obj.name);
        Selection.activeGameObject = obj;
    }
}