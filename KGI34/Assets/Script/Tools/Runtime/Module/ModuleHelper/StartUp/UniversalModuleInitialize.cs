using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UniversalModule.ObjectPool;
using UniversalModule.DelaySystem;
using UniversalModule.AudioSystem;
using UniversalModule.SpawnSystem;
using UniversalModule.GlobalEventSystem;

namespace UniversalModule.Initialize {
    public class UniversalModuleInitialize {
        [RuntimeInitializeOnLoadMethod()]
        private static void Initialize() {
            GameObject Tool = new GameObject("Tools");
            MonoBehaviour.DontDestroyOnLoad(Tool);

            PoolBuilder.Parent = Tool.transform;
            Tool.AddComponent<DelayCallback>();

            AudioManager.Initialize();
            SpawnFactory.Initialize();
            GlobalEventRegistry.Initialize();

            LoaderStartUp();
        }
        private class InitMethod {
            public int order;
            public MethodInfo method;

            public InitMethod(int order, MethodInfo method) {
                this.order = order;
                this.method = method;
            }
        }

        private static void LoaderStartUp() {
            List<InitMethod> infos = new List<InitMethod>();
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblys) {
                if (!assembly.IsDynamic) {
                    Type[] types = assembly.GetExportedTypes();
                    foreach (Type type in types) {
                        MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                        if (methods.Length > 0) {
                            foreach (MethodInfo method in methods) {
                                object[] attributes = method.GetCustomAttributes(typeof(AutoLoadAttribute), true);
                                if (attributes.Length > 0) {
                                    foreach (var item in attributes) {
                                        AutoLoadAttribute loader = item as AutoLoadAttribute;
                                        infos.Add(new InitMethod(loader.order, method));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            infos.Sort((a, b) => { return a.order.CompareTo(b.order); });
            foreach (InitMethod item in infos) {
                item.method.Invoke(null, null);
            }
        }
    }
}
