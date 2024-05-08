using System;
using UnityEngine;
using DG.Tweening;

namespace UniversalModule.AudioSystem {
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour, IAudioPlayer {
        /// <summary>
        /// 播放声音的AudioSource组件
        /// </summary>
        private AudioSource source;
        /// <summary>
        /// 播放声音的AudioSource组件访问接口
        /// </summary>
        public AudioSource Source {
            get {
                if (source == null) {
                    source = GetComponent<AudioSource>();
                }
                return source;
            }
        }
        /// <summary>
        /// 是否允许回收
        /// </summary>
        internal bool isAllowRecycle = false;
        /// <summary>
        /// 音频名称
        /// </summary>
        public string clipName { get; private set; }
        /// <summary>
        /// 对象池回收事件
        /// </summary>
        internal static Action<AudioPlayer> RecycleCallback;

        /// <summary>
        /// 音效渐入
        /// </summary>
        /// <param name="clip">音效</param>
        /// <param name="duration">效果持续时间</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="volume">音量</param>
        /// <param name="clipName">音效名</param>
        /// <param name="clipLength">音效时长</param>
        public void FadeIn(AudioClip clip, float duration, bool isLoop, float volume, string clipName, float clipLength = -1) {
            Source.DOKill();
            Play(clip, isLoop, volume, clipName, clipLength);
            Source.DOFade(volume, duration).SetAutoKill();
        }
        /// <summary>
        /// 音效渐出
        /// </summary>
        /// <param name="fade">渐隐时长</param>
        public void FadeOut(float duration) {
            Source.DOFade(0, duration).OnComplete(Stop).SetAutoKill(true);
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="isLoop"></param>
        /// <param name="clip"></param>
        public void Play(AudioClip clip, bool isLoop, float volume, string clipName, float clipLength = -1) {
            Source.clip = clip;
            Source.loop = isLoop;
            Source.volume = volume;
            this.clipName = clipName;

            clipLength = clipLength == -1 ? clip.length : clipLength;
            if (isLoop == false) {
                Invoke("Stop", clipLength);
            }
              
            Source.Play();
        }
        /// <summary>
        /// 停止音频播放器
        /// </summary>
        public void Stop() {
            Source.Stop();
            Source.DOKill();
            if (isAllowRecycle == true) {
                RecycleCallback(this);
            }
            CancelInvoke("Stop");
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        public void Pause() {
            Source.Pause();
        }
        /// <summary>
        /// 暂停后继续播放
        /// </summary>
        public void Continue() {
            Source.UnPause();
        }
        /// <summary>
        /// 获取音频时长
        /// </summary>
        /// <returns></returns>
        public float GetAudioLength() {
            if (Source != null && Source.clip != null) {
                return Source.clip.length;
            }
            return default;
        }
        /// <summary>
        /// 获取音效的剩余时长
        /// </summary>
        /// <returns></returns>
        public float GetRemainTime() {
            if (Source != null && Source.clip != null) {
                return Source.clip.length - source.time;
            }
            return default;
        }
    }
}