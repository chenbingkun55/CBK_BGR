using System;
using UnityEngine;

namespace CBK.Framework.Update
{
    internal partial class UpdateServiceImpl
    {
        private sealed class Driver : MonoBehaviour
        {
            public delegate void UpdateEvent(float elapseSeconds, float realElapseSeconds);
            
            public event UpdateEvent OnUpdated;
            public event UpdateEvent OnLateUpdated;
            public event UpdateEvent OnFixedUpdated;

            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
            }

            private void Update()
            {
                OnUpdated?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
            }

            private void LateUpdate()
            {
                OnLateUpdated?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
            }

            private void FixedUpdate()
            {
                OnFixedUpdated?.Invoke(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
            }
        }
    }
}