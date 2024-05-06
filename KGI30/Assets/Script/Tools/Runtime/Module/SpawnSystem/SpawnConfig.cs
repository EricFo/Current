using System;
using UnityEngine;

namespace UniversalModule.SpawnSystem {
    public class SpawnConfig : ScriptableObject {
        /// <summary>
        /// 所有可回收的对象
        /// </summary>
        public SpawnNode[] SpawnObjects;
        /// <summary>
        /// 相关SpawnItem资源的存放路径
        /// </summary>
        public string cachePath = string.Empty;

        [Serializable]
        public class SpawnNode {
            [Range(3, 100)]
            [Tooltip("对象的缓存数量")]
            public int Count;
            [Tooltip("对象预制体")]
            public SpawnItem Prefab;
        }
    }
}
