using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
namespace UniversalModule.SpawnSystem {
    [CustomEditor(typeof(SpawnConfig))]
    public class SpawnConfigEditor : Editor {
        private SpawnConfig spawn;
        private List<float> elementHeight;
        private ReorderableList reorderableList;
        private SerializedProperty spawnObjects;

        private void OnEnable() {
            spawn = target as SpawnConfig;
            elementHeight = new List<float>();
            spawnObjects = serializedObject.FindProperty("SpawnObjects");
            reorderableList = new ReorderableList(serializedObject, spawnObjects);

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
            spawn.cachePath = GUILayout.TextField(spawn.cachePath);
            GUILayout.EndHorizontal();

            GUIStyle style = GUI.skin.GetStyle("Button");
            GUI.skin.button.fontSize = 16;
            if (GUILayout.Button("更新列表", style, new[] { GUILayout.Height(50) })) {
                var path = Path.Combine(Application.dataPath, spawn.cachePath);
                if (Directory.Exists(path)) {
                    UpdateSpawnList(path);
                }
                EditorUtility.SetDirty(spawn);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
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
            var element = spawnObjects.GetArrayElementAtIndex(index);
            if (EditorGUI.PropertyField(elementRect, element)) {
                elementHeight[index] = 70;
                SerializedProperty count = element.FindPropertyRelative("Count");
                SerializedProperty prefab = element.FindPropertyRelative("Prefab");
                Rect countRect = new Rect(elementRect.x, elementRect.y + 20, elementRect.width, 20);
                Rect prefabRect = new Rect(elementRect.x, elementRect.y + 45, elementRect.width, 20);
                EditorGUI.indentLevel += 1;
                EditorGUI.PropertyField(countRect, count);
                EditorGUI.PropertyField(prefabRect, prefab);
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
            var element = spawnObjects.GetArrayElementAtIndex(list.count - 1);
            SerializedProperty count = element.FindPropertyRelative("Count");
            count.intValue = 15;
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
        /// 从路径查找SpawnItem，更新Spawn列表信息
        /// </summary>
        /// <param name="root"></param>
        private void UpdateSpawnList(string root) {
            List<SpawnItem> items = new List<SpawnItem>();
            string[] allPath = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            for (int i = 0; i < allPath.Length; i++) {
                string path = allPath[i].Substring(allPath[i].IndexOf("Assets"));
                SpawnItem item = AssetDatabase.LoadAssetAtPath<SpawnItem>(path);
                if (item != null) {
                    items.Add(item);
                }
            }

            List<SpawnConfig.SpawnNode> nodes = new List<SpawnConfig.SpawnNode>(items.Count);
            for (int i = 0; i < items.Count; i++) {
                int count = 15;
                for (int j = 0; j < spawn.SpawnObjects.Length; j++) {
                    if (spawn.SpawnObjects[j].Prefab.ItemName.Equals(items[i].ItemName)) {
                        count = spawn.SpawnObjects[j].Count;
                        break;
                    }
                }
                SpawnConfig.SpawnNode node = new SpawnConfig.SpawnNode();
                node.Prefab = items[i];
                node.Count = count;
                nodes.Add(node);
            }

            spawn.SpawnObjects = nodes.ToArray();
        }

        /// <summary>
        /// 创建Spawn配置列表
        /// </summary>
        [MenuItem("Assets/Scriptable Object/Create Spawn Config")]
        public static void CreateSpawnConfig() {
            string path = AssetDatabase.GetAssetPath(Selection.instanceIDs[0]) + "/SpawnConfig.asset";
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SpawnConfig>(), path);
            AssetDatabase.Refresh();
        }
    }
}
