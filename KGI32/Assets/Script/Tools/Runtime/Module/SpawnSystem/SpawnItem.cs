using System;
using UnityEngine;

namespace UniversalModule.SpawnSystem {
    public class SpawnItem : MonoBehaviour {
        /// <summary>
        /// inspector面板设置的对象名称
        /// </summary>
        public string ItemName;
        /// <summary>
        /// 是否允许被回收，之后对象从对象池取出的时候才为true
        /// </summary>
        public bool IsAllowRecycle { get; internal set; }

        /// <summary>
        /// 对象回收事件监听
        /// </summary>
        private static Action<SpawnItem> OnRecycleListener;
        /// <summary>
        /// 对象回收事件，内部包含了防止多次注册的机制
        /// </summary>
        internal static event Action<SpawnItem> OnRecycleEvent {
            add {
                if (OnRecycleListener == null) {
                    OnRecycleListener += value;
                }
            }
            remove {
                OnRecycleListener -= value;
            }
        }
        /// <summary>
        /// 回收当前对象
        /// </summary>
        /// <exception cref="Exception">若已经回收过，则不允许再次回收它</exception>
        public virtual void Recycle() {
            if (IsAllowRecycle == true) {
                IsAllowRecycle = false;
                OnRecycleListener?.Invoke(this);
            } else {
                throw new Exception(string.Format("对象“{0}”已被回收，但是你仍然在尝试回收它", ItemName));
            }
        }
    }
}
