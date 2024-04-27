using System;
using UnityEngine;

namespace FairyGUI
{
    [ExecuteInEditMode]
    [AddComponentMenu("FairyGUI/EMRenderSupport")]
    public class EMRenderMonoBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isPlaying)
                Destroy(this);
        }

        void OnRenderObject()
        {
            //Update和OnGUI在EditMode的调用都不那么及时，OnRenderObject则比较频繁，可以保证界面及时刷新。所以使用OnRenderObject
            if (!Application.isPlaying)
            {
                EMRenderSupport.Update();
            }
        }
    }
}