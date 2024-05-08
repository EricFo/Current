using System;
using UnityEngine;

namespace ParticleShader
{ 
    public class CheckDestory : MonoBehaviour
    {
        public Action<Renderer> OnDestoryEvent;

        private void OnDestroy()
        {
            OnDestoryEvent?.Invoke(GetComponent<Renderer>());
        }
    }
}
