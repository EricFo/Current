using System;
using UnityEngine;
using System.Collections.Generic;
using UniversalModule.ObjectPool;
using UniversalModule.SpawnSystem;

namespace UniversalModule.AudioSystem {
    /// <summary>
    /// 音频管理，基于对象池缓存，用于控制音效播放
    /// </summary>
    public static class AudioManager {
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private static bool isInitalized = false;
        /// <summary>
        /// 循环音效对象池
        /// </summary>
        private static IObjectPool<AudioPlayer> AudioPools;
        /// <summary>
        /// 所有的音频资源
        /// </summary>
        private static Dictionary<string, AudioClip> audioClips;
        /// <summary>
        /// 播放器缓存
        /// </summary>
        private static Dictionary<string, AudioPlayer> cachePlayers;

        #region 配置管理
        /// <summary>
        /// 音频管理初始化
        /// </summary>
        /// <param name="audioCount">循环音乐数量</param>
        /// <param name="soundCount">单次播放音效数量</param>
        internal static void Initialize() {
            if (isInitalized == false) {
                isInitalized = true;

                //创建模板
                GameObject audio = new GameObject("Audio");
                AudioPlayer template = audio.AddComponent<AudioPlayer>();
                AudioPlayer.RecycleCallback = Recycle;
                template.Source.playOnAwake = false;

                //创建对象池缓存
                AudioPools = new ComponentPoolOfQueue<AudioPlayer>(template);
                template.transform.SetParent(PoolBuilder.Parent);
                template.transform.ResetLocalProperty();
                template.gameObject.SetActive(false);

                //创建音频列表
                audioClips = new Dictionary<string, AudioClip>();
                cachePlayers = new Dictionary<string, AudioPlayer>();

                var config = Resources.Load<AudioConfig>("ScriptableObject/AudioConfig");
                Append(config);
            }
        }
        /// <summary>
        /// 附加额外的音频配置
        /// </summary>
        /// <param name="config">新的音频配置表</param>
        public static void Append(AudioConfig config) {
            if (isInitalized == true) {
                foreach (var clip in config.AudioCollection) {
                    if (!audioClips.ContainsKey(clip.audioName)) {
                        audioClips.Add(clip.audioName, clip.audioClip);
                    }
                }
            } else {
                throw new Exception("AudioManager 尚未初始化，无法附加音频");
            }
        }
        /// <summary>
        /// 卸载所有资源，重置音频系统
        /// </summary>
        public static void Unload() {
            isInitalized = false;
            foreach (var player in cachePlayers.Values) {
                player.Stop();
            }
            AudioPools.Release();
            cachePlayers.Clear();
            audioClips.Clear();
        }
        #endregion

        #region 播放
        /// <summary>
        /// 当前音效在播放完成之前无法重复播放
        /// </summary>
        /// <param name="clipName">音效名</param>
        /// <param name="clipLength">音效播放时长 -1为默认时长</param>
        /// <param name="volume">音量</param>
        /// <returns></returns>
        public static void PlayOneShot(string clipName, float clipLength = -1, float volume = 1) {
            CheckIsInitialize();
            if (!cachePlayers.ContainsKey(clipName)) {
                AudioPlayer audio = AudioPools.GetObject();
                audio.Play(audioClips[clipName], false, volume, clipName, clipLength);
                cachePlayers.Add(clipName, audio);
                audio.isAllowRecycle = true;
            }
        }
        /// <summary>
        /// 循环播放音效
        /// </summary>
        /// <param name="clipName">音效名称</param>
        /// <param name="clipLength">音效时长</param>
        /// <param name="volume">音量</param>
        public static void LoopPlayback(string clipName, float clipLength = -1, float volume = 1) {
            CheckIsInitialize();
            if (!cachePlayers.ContainsKey(clipName)) {
                AudioPlayer audio = AudioPools.GetObject();
                audio.Play(audioClips[clipName], true, volume, clipName, clipLength);
                cachePlayers.Add(clipName, audio);
                audio.isAllowRecycle = true;
            } else {
                cachePlayers[clipName].Play(audioClips[clipName], true, volume, clipName, clipLength);
            }
        }
        /// <summary>
        /// 正常播放音效(不循环，但是能同时播多次)
        /// </summary>
        /// <param name="clipName">音效名称</param>
        /// <param name="clipLength">音效时长</param>
        /// <param name="volume">音量</param>
        public static IAudioPlayer Playback(string clipName, float clipLength = -1, float volume = 1) {
            CheckIsInitialize();
            AudioPlayer audio = AudioPools.GetObject();
            audio.Play(audioClips[clipName], false, volume, clipName, clipLength);
            audio.isAllowRecycle = true;
            return audio;
        }
        /// <summary>
        /// 继续播放，用于暂停后恢复
        /// </summary>
        /// <param name="clipName"></param>
        public static void Continue(string clipName) {
            CheckIsInitialize();
            if (cachePlayers.ContainsKey(clipName)) {
                cachePlayers[clipName].Continue();
            }
        }
        #endregion

        #region 停止/暂停
        /// <summary>
        /// 暂停一个音效
        /// </summary>
        /// <param name="clipName"></param>
        public static void Pause(string clipName) {
            CheckIsInitialize();
            if (cachePlayers.ContainsKey(clipName)) {
                cachePlayers[clipName].Pause();
            }
        }
        /// <summary>
        /// 停止播放器
        /// </summary>
        /// <param name="clipName">音效名</param>
        public static void Stop(string clipName) {
            CheckIsInitialize();
            if (cachePlayers.ContainsKey(clipName)) {
                cachePlayers[clipName].Stop();
            }
        }
        #endregion

        #region 音效处理
        /// <summary>
        /// 声音渐入
        /// </summary>
        /// <param name="clipName">音效名</param>
        /// <param name="isLoop">渐入后是否循环</param>
        /// <param name="duration">渐入持续时间</param>
        /// <param name="clipLength">音效时长</param>
        /// <param name="volume">音量</param>
        /// <returns></returns>
        public static IAudioPlayer FadeIn(string clipName, bool isLoop, float duration, float clipLength = -1, float volume = 1) {
            CheckIsInitialize();
            if (!cachePlayers.ContainsKey(clipName)) {
                AudioPlayer audio = AudioPools.GetObject();
                audio.FadeIn(audioClips[clipName], duration, isLoop, volume, clipName, clipLength);
                cachePlayers.Add(clipName, audio);
                audio.isAllowRecycle = true;
                return audio;
            } else {
                cachePlayers[clipName].FadeIn(audioClips[clipName], duration, isLoop, volume, clipName, clipLength);
                return cachePlayers[clipName];
            }
        }
        /// <summary>
        /// 声音渐出
        /// </summary>
        /// <param name="clipName">音效名</param>
        /// <param name="duration">渐出持续时间</param>
        public static void FadeOut(string clipName, float duration) {
            CheckIsInitialize();
            if (cachePlayers.ContainsKey(clipName)) {
                cachePlayers[clipName].FadeOut(duration);
            }
        }
        #endregion

        #region 工具
        /// <summary>
        /// 回收节点
        /// </summary>
        /// <param name="isLoop"></param>
        /// <param name="audio"></param>
        private static void Recycle(AudioPlayer audio) {
            audio.isAllowRecycle = false;
            AudioPools.RecycleObject(audio);
            if (cachePlayers.ContainsKey(audio.clipName)) {
                cachePlayers.Remove(audio.clipName);
            }
        }
        /// <summary>
        /// 获取音效时长
        /// </summary>
        /// <param name="clipName">音效名</param>
        /// <returns></returns>
        public static float GetAudioTime(string clipName) {
            CheckIsInitialize();
            if (audioClips.ContainsKey(clipName)) {
                return audioClips[clipName].length;
            }
            throw new Exception(string.Format("无法获取音效时长，因为没有找到{0}音频文件", clipName));
        }
        /// <summary>
        /// 检查是否已经初始化
        /// </summary>
        /// <exception cref="Exception">Audio Manager 尚未初始化</exception>
        private static void CheckIsInitialize() {
            if (isInitalized == false) {
                throw new Exception("Audio Manager 尚未初始化！");
            }
        }
        #endregion
    }
}

