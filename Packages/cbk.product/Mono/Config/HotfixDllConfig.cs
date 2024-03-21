using System;
using System.Collections.Generic;
using UnityEngine;

namespace CBK.Mono
{
    /// <summary>
    /// Hotfix Dll 配置
    /// </summary>
    [CreateAssetMenu(fileName = "HotfixDllConfig", menuName = "CBK Config/HotfixDllConfig", order = 0)]
    public class HotfixDllConfig : ScriptableObject
    {
        /// <summary>
        /// Hotfix Dll 配置
        /// </summary>
        [SerializeField]
        private List<string> _hotfixDlls = new List<string>();
        public List<string> hotfixDlls => _hotfixDlls;
    }
}