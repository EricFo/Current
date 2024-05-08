using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class KeyframeEvent : MonoBehaviour {
    private Animator animator;
    private Dictionary<string, Dictionary<int, Action>> callbacks;
    private List<AnimationEvent> animCache;

    /// <summary>
    /// 注册动画事件
    /// </summary>
    /// <param name="events"></param>
    /// <param name="animName"></param>
    /// <param name="frame"></param>
    /// <param name="frameRate"></param>
    public void Register(Action events, string animName, int frame) {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (callbacks == null)
            callbacks = new Dictionary<string, Dictionary<int, Action>>(5);
        if (!callbacks.ContainsKey(animName))
            callbacks.Add(animName, new Dictionary<int, Action>());

        if (!callbacks[animName].ContainsKey(frame)) {
            AddAnimationEvent(animName, frame);
            callbacks[animName].Add(frame, events);
        } else {
            callbacks[animName][frame] = events;
        }
    }
    /// <summary>
    /// 移除某一帧的事件
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="frame"></param>
    public void Remove(string animName, int frame) {
        if (animCache == null)
            animCache = new List<AnimationEvent>(5);
        else
            animCache.Clear();

        if (callbacks.ContainsKey(animName) && callbacks[animName].ContainsKey(frame))
            callbacks[animName].Remove(frame);
        else
            Debug.LogError("需要移除的事件不存在！");

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i].name.Equals(animName)) {
                float time = frame / clips[i].frameRate;
                float interval = 1 / clips[i].frameRate;
                for (int j = 0; j < clips[i].events.Length; j++) {
                    if (Mathf.Abs(clips[i].events[j].time - time) < interval) {
                        animCache.AddRange(clips[i].events);
                        animCache.RemoveAt(j);
                        clips[i].events = animCache.ToArray();
                        break;
                    }
                }
                break;
            }
        }
    }
    /// <summary>
    /// 移除所有事件
    /// </summary>
    /// <param name="animName"></param>
    public void RemoveAll(string animName) {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i].name.Equals(animName)) {
                clips[i].events = null;
                break;
            }
        }
        callbacks.Clear();
    }
    /// <summary>
    /// 添加动画事件
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="frame"></param>
    /// <param name="frameRate"></param>
    private void AddAnimationEvent(string animName, int frame) {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        bool isFind = false;
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i].name.Equals(animName)) {
                isFind = true;
                AnimationEvent events = new AnimationEvent();
                events.time = frame / clips[i].frameRate;
                string param = string.Format("{0},{1}", animName, frame);
                events.stringParameter = param;
                events.functionName = "KeyframeCallBack";
                bool isContains = false;
                for (int j = 0; j < clips[i].events.Length; j++) {
                    if (clips[i].events[j].stringParameter.Equals(param)) {
                        isContains = true;
                        clips[i].events[j] = events;
                        break;
                    }
                }
                if (isContains == false)
                    clips[i].AddEvent(events);
                break;
            }
        }
        if (isFind == false)
            Debug.LogError("未找到动画：" + animName);
    }
    /// <summary>
    /// 关键帧事件回调
    /// </summary>
    private void KeyframeCallBack(string param) {
        if (callbacks != null) {
            string[] split = param.Split(',');
            callbacks[split[0]][int.Parse(split[1])]?.Invoke();
        }
    }
}