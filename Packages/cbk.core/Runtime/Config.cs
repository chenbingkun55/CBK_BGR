using System;
using System.IO;
using UnityEngine;

namespace CBK.Core
{
    public class Config
    {
        // Assets 路径
        public static readonly string kAssetsPath = Application.dataPath;

        // Data 路径
#if UNITY_EDITOR
        public static readonly string kDataPath = Application.persistentDataPath;
#elif UNITY_ANDROID
        public static readonly string kDataPath = "/storage/emulated/0/cbk.data";
#endif
    }

}