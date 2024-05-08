using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using System.Collections.Generic;

namespace UniversalModule.AudioSystem {
    [CustomEditor(typeof(AudioConfig))]
    public class AudioConfigEditor : Editor {
        private AudioConfig audio;
        private List<float> elementHeight;
        private ReorderableList reorderableList;
        private SerializedProperty audioCollection;

        private void OnEnable() {
            audio = target as AudioConfig;
            elementHeight = new List<float>();
            audioCollection = serializedObject.FindProperty("AudioCollection");
            reorderableList = new ReorderableList(serializedObject, audioCollection);

            //绘制元素回调
            reorderableList.onAddCallback += OnAddElementEvent;
            reorderableList.onRemoveCallback += OnRemoveElementEvent;
            reorderableList.onCanRemoveCallback += OnCanRemoveElementEvent;

            reorderableList.drawElementCallback += OnDrawElementEvent;
            reorderableList.elementHeightCallback += OnDrawElementHeightEvent;
        }
        public override void OnInspectorGUI() {
            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            GUILayout.BeginHorizontal();
            GUILayout.Label("加载路径：");
            GUILayout.Space(-250);
            audio.CachePath = GUILayout.TextField(audio.CachePath);
            GUILayout.EndHorizontal();

            GUI.skin.button.fontSize = 16;
            GUIStyle style = GUI.skin.GetStyle("Button");
            if (GUILayout.Button("更新列表", style, new[] { GUILayout.Height(50) })) {
                List<AudioConfig.AudioClipNode> items = new List<AudioConfig.AudioClipNode>();
                var path = Path.Combine(Application.dataPath, audio.CachePath);
                if (Directory.Exists(path)) {
                    UpdateAudioList(path);
                }
            }
            GUI.skin.button.fontSize = 12;
        }

        #region UI绘制事件
        /// <summary>
        /// 绘制UI元素事件
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="isActive"></param>
        /// <param name="isFocused"></param>
        private void OnDrawElementEvent(Rect rect, int index, bool isActive, bool isFocused) {
            elementHeight[index] = 20;
            Rect elementRect = new Rect(rect.x + 8, rect.y + 1, rect.width - 16, 20);
            var element = audioCollection.GetArrayElementAtIndex(index);
            if (EditorGUI.PropertyField(elementRect, element)) {
                elementHeight[index] = 70;
                SerializedProperty audioClip = element.FindPropertyRelative("audioClip");
                SerializedProperty audioName = element.FindPropertyRelative("audioName");
                Rect nameRect = new Rect(elementRect.x, elementRect.y + 20, elementRect.width, 20);
                Rect clipRect = new Rect(elementRect.x, elementRect.y + 45, elementRect.width, 20);
                EditorGUI.indentLevel += 1;
                EditorGUI.PropertyField(nameRect, audioName);
                EditorGUI.PropertyField(clipRect, audioClip);
                EditorGUI.indentLevel -= 1;
            }
        }
        /// <summary>
        /// UI元素高度变化事件
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private float OnDrawElementHeightEvent(int index) {
            if (elementHeight.Count > index) {
                return elementHeight[index];
            } else {
                elementHeight.Add(20);
                return elementHeight[index];
            }
        }
        /// <summary>
        /// 添加UI元素事件
        /// </summary>
        /// <param name="list"></param>
        private void OnAddElementEvent(ReorderableList list) {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            elementHeight.Add(20);
        }
        /// <summary>
        /// 移除UI元素事件
        /// </summary>
        /// <param name="list"></param>
        private void OnRemoveElementEvent(ReorderableList list) {
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
            elementHeight.RemoveAt(elementHeight.Count - 1);
        }
        /// <summary>
        /// 是否可以移除元素事件
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool OnCanRemoveElementEvent(ReorderableList list) {
            return list.count >= 1;
        }
        #endregion

        /// <summary>
        /// 更新音频资源列表
        /// </summary>
        private void UpdateAudioList(string root) {
            List<AudioConfig.AudioClipNode> items = new List<AudioConfig.AudioClipNode>();
            string[] paths = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            for (int i = 0; i < paths.Length; i++) {
                string path = paths[i].Substring(paths[i].IndexOf("Assets"));
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                AudioConfig.AudioClipNode existedNode = null;
                if (audio.AudioCollection != null) {
                    existedNode = audio.AudioCollection.FirstOrDefault(item => item.audioClip == clip);
                }
                if (existedNode == null && clip != null) {
                    var node = new AudioConfig.AudioClipNode() {
                        audioName = clip.name,
                        audioClip = clip,
                    };
                    items.Add(node);
                } else if (existedNode != null) {
                    items.Add(existedNode);
                }
            }
            for (int i = 0; i < items.Count;) {
                if (items[i].audioClip == null) {
                    items.Remove(items[i]);
                    continue;
                }
                i++;
            }

            audio.AudioCollection = items.ToArray();
            EditorUtility.SetDirty(audio);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// 创建音频配置列表
        /// </summary>
        [MenuItem("Assets/Scriptable Object/Create Audio Config")]
        public static void CreateAudioConfig() {
            string path = AssetDatabase.GetAssetPath(Selection.instanceIDs[0]) + "/AudioConfig.asset";
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AudioConfig>(), path);
            AssetDatabase.Refresh();
        }
    }
}
