using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBK.Product.Ui
{
    public class PanelTitle : MonoBehaviour
    {
        public void Close()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
        }
    }
}
