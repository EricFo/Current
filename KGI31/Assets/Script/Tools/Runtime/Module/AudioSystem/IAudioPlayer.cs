using UnityEngine;
namespace UniversalModule.AudioSystem {
    public interface IAudioPlayer {
        /// <summary>
        /// 获取音频时长
        /// </summary>
        /// <returns></returns>
        float GetAudioLength();
        /// <summary>
        /// 获取音频剩余时长
        /// </summary>
        /// <returns>剩余时长</returns>
        float GetRemainTime();
        /// <summary>
        /// 继续播放
        /// </summary>
        void Continue();
        /// <summary>
        /// 淡出
        /// </summary>
        /// <param name="duration">淡出时间</param>
        void FadeOut(float duration);
        /// <summary>
        /// 暂停播放
        /// </summary>
        void Pause();
        /// <summary>
        /// 停止播放
        /// </summary>
        void Stop();
    }
}
