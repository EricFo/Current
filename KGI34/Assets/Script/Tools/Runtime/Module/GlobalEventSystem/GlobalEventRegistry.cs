using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UniversalModule.GlobalEventSystem {
    public static class GlobalEventRegistry {
        private static bool IsInitialize = false;
        private static Dictionary<string, EventHandler> EventCache;

        /// <summary>
        /// 事件系统初始化
        /// </summary>
        internal static void Initialize() {
            if (IsInitialize == false) {
                IsInitialize = true;
                EventCache = new Dictionary<string, EventHandler>();
            }
        }

        /// <summary>
        /// 注册全局事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">需要注册的事件</param>
        public static void Register(string eventName, EventHandler handler) {
            if (IsInitialize == true ) {
                if (!EventCache.ContainsKey(eventName)) {
                    EventCache.Add(eventName, handler);
                } else {
                    EventCache[eventName] -= handler;
                    EventCache[eventName] += handler;
                }
            } else {
                throw new Exception("Global Event Registry 尚未初始化，无法添加事件");
            }
        }
        /// <summary>
        /// 注销全局事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">需要注销的事件</param>
        public static void Unregister(string eventName, EventHandler handler) {
            if (IsInitialize == true && EventCache.ContainsKey(eventName)) {
                EventCache[eventName] -= handler;
            }
        }
        /// <summary>
        /// 注销所有全局事件
        /// </summary>
        public static void UnregisterAll() {
            if (IsInitialize == true) {
                foreach (var eventName in EventCache.Keys) {
                    EventCache[eventName] = null;
                }
                EventCache.Clear();
            }
        }

        /// <summary>
        /// 全局事件调用
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">调用对象</param>
        /// <param name="args">事件参数</param>
        /// <exception cref="Exception"></exception>
        public static void Invoke(string eventName, object sender, EventArgs args) {
            if (IsInitialize == true) {
                if (EventCache.ContainsKey(eventName)) {
                    EventCache[eventName]?.Invoke(sender, args);
                } else {
                    throw new Exception(string.Format("\"{0}\" 事件尚未注册!", eventName));
                }
            } else {
                throw new Exception("Global Event Registry 尚未初始化，无法调用事件!");
            }
        }
    }
}
