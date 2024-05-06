using System;
using UnityEngine;
using System.Collections.Generic;
using UniversalModule.ObjectPool;

namespace UniversalModule.SpawnSystem {
    /// <summary>
    /// 孵化器，用于生产继承于SpawnItem的对象
    /// </summary>
    public static class SpawnFactory {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private static bool isInitialized = false;
        /// <summary>
        /// 缓存每种对象的对象池
        /// </summary>
        private static Dictionary<string, IObjectPool<SpawnItem>> SpawnCache;

        #region 配置管理
        /// <summary>
        /// 孵化器初始化
        /// </summary>
        internal static void Initialize() {
            if (isInitialized == false) {
                isInitialized = true;
                SpawnItem.OnRecycleEvent += RecycleObject;
                SpawnCache = new Dictionary<string, IObjectPool<SpawnItem>>();
                var config = Resources.Load<SpawnConfig>("ScriptableObject/SpawnConfig");
                Append(config);
            }
        }
        /// <summary>
        /// 附加额外的孵化器缓存
        /// </summary>
        /// <param name="config"></param>
        public static void Append(SpawnConfig config) {
            if (isInitialized == true) {
                if (config != null) {
                    foreach (var item in config.SpawnObjects) {
                        if (string.IsNullOrEmpty(item.Prefab.ItemName)) {
                            throw new Exception(string.Format("对象名不能为空！检查预制体 {0} 的ItemName属性", item.Prefab.name));
                        } else if (SpawnCache.ContainsKey(item.Prefab.ItemName)) {
                            throw new Exception(string.Format("对象名冲突，无法重复创建！检查预制体 {0} 的ItemName属性", item.Prefab.name));
                        } else {
                            SpawnCache.Add(item.Prefab.ItemName, new ComponentPoolOfStack<SpawnItem>(item.Count, item.Prefab));
                        }
                    }
                }
            } else {
                throw new Exception("SpawnFactory 尚未初始化，无法附加额外的缓存");
            }
        }
        /// <summary>
        /// 卸载所有资源，重置孵化系统
        /// </summary>
        public static void Unload() {
            isInitialized = false;
            foreach (var cache in SpawnCache.Values) {
                cache.Release();
            }
            SpawnCache.Clear();
            SpawnCache = null;
        }
        #endregion

        #region 功能接口
        /// <summary>
        /// 取出对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetObject<T>(string name) where T : SpawnItem {
            if (isInitialized == true) {
                SpawnItem item = SpawnCache[name].GetObject();
                item.IsAllowRecycle = true;
                if (item is T) {
                    return item as T;
                } else {
                    return null;
                }
            } else {
                throw new Exception("Spawn Factory 尚未初始化！");
            }
        }
        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="symbol"></param>
        private static void RecycleObject(SpawnItem symbol) {
            if (isInitialized == true) {
                SpawnCache[symbol.ItemName].RecycleObject(symbol);
            } else {
                throw new Exception("Spawn Factory 尚未初始化！");
            }
        }
        #endregion
    }
}
