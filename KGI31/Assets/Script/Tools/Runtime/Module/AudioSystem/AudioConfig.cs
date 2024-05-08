using System;
using UnityEngine;

namespace UniversalModule.AudioSystem {
    public class AudioConfig : ScriptableObject {
        /// <summary>
        /// 相关音频文件集合
        /// </summary>
        public AudioClipNode[] AudioCollection;
        public string CachePath = string.Empty;

        [Serializable]
        public class AudioClipNode {
            [Tooltip("声音名称，默认使用音频文件名")]
            public string audioName;
            [Tooltip("音频文件")]
            public AudioClip audioClip;
        }
    }
}