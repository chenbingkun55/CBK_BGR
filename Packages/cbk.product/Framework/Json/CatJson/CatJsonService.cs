using System;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Reference;

namespace CBK.Framework.Json
{
    /// <summary>
    /// 基于CatJson实现的Json服务
    /// </summary>
    internal sealed partial class CatJsonService : IInitialize, IDisposable, IJsonService
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }

        private IJsonCore m_JsonCore;

        public void Initialize()
        {
            m_JsonCore = Allocate();
        }

        public void Dispose()
        {
            m_JsonCore.Dispose();
        }

        public IJsonCore Allocate()
        {
            return ReferenceService.GetReference<CatJsonCore>();
        }

        public string Serialize<T>(T obj)
        {
            lock (m_JsonCore)
            {
                return m_JsonCore.Serialize(obj);
            }
        }

        public T Deserialize<T>(string json)
        {
            lock (m_JsonCore)
            {
                return m_JsonCore.Deserialize<T>(json);
            }
        }
    }
}