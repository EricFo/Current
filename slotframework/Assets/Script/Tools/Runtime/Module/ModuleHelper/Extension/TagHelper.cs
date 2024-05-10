#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
[InitializeOnLoad]
public class TagHelper
{
    static TagHelper() {
        AddTag("Popup");
        AddTag("StateMachine");
        AddLayer("Bezier");
        Tools.lockedLayers = 0;
    }
    public static void AddTag(string tag) {
        if (!IsHasTag(tag)) {
            InternalEditorUtility.AddTag(tag);
        }
    }
    public static void AddLayer(string layer) {
        if (!IsHasLayer(layer)) {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset"));
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true)) {
                if (it.name.Equals("layers")) {
                    if (it.arraySize < 8) {
                        break;
                    }
                    else {
                        for (int i = 8; i < it.arraySize; i++) {
                            SerializedProperty sp = it.GetArrayElementAtIndex(i);
                            if (string.IsNullOrEmpty(sp.stringValue)) {
                                sp.stringValue = layer;
                                tagManager.ApplyModifiedProperties();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    public static bool IsHasTag(string tag) {
        return InternalEditorUtility.tags.Contains(tag);
    }
    public static bool IsHasLayer(string layer) {
        return InternalEditorUtility.layers.Contains(layer);
    }
}
#endif
