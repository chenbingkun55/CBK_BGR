using System;
using System.Collections.Generic;
using Bright.Serialization;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Res;
using UnityEngine;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 基于二进制文件的文本本地化
    /// </summary>
    internal sealed class BinaryTextLocalization : ITextLocalization
    {
        [Inject]
        public IResService ResService { get; set; }
        
        private readonly Dictionary<string, string> m_Dict = new Dictionary<string, string>();

        public async UniTask<int> LoadAsync(ILocalizationConfig config)
        {
            m_Dict.Clear();
            
            var bytes = await ResService.LoadRawAsync(LocalizationUtility.ResolveAssetKey($"text_{config.LangCode}"));
            var buffer = new ByteBuf(bytes);
            
            for (var n = buffer.ReadSize(); n > 0; --n)
            {
                var key = buffer.ReadString();
                var value = buffer.ReadString();
                
                m_Dict.Add(key, value);
            }

            return 0;
        }

        public void Clean()
        {
            m_Dict.Clear();
        }

        public string GetText(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;
            
            if (m_Dict.TryGetValue(key, out var value))
                return value;
            
            Log.Error($"BinaryTextLocalization: key not found: {key}");
            return key;
        }
    }
}