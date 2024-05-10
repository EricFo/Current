using System;
using UnityEngine;

namespace SlotGame.Core.Singleton {
    public class GlobalSingleton<T> : MonoBehaviour, IGlobalSingleton where T : GlobalSingleton<T> {
        /// <summary>
        /// 是否已经初始化的标记
        /// </summary>
        protected bool IsInitialized = false;

        protected static T Instance = null;

        public virtual void Initialize() {
            if (IsInitialized == false) {
                if (this is T) {
                    Instance = this as T;
                } else {
                    Debug.LogErrorFormat("无法将类型转换为:{0}", typeof(T).ToString());
                }

                IsInitialized = true;
            } else {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// 检查是否已经初始化过，如果没有将会抛出异常，因为初始化之前不允许执行任何操作
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected void CheckIsInitialized() {
            if (IsInitialized == false) {
                throw new Exception("UIController 尚未初始化");
            }
        }
    }
}