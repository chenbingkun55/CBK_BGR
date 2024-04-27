using System;
using System.Collections.Generic;
using CatJson;
using CBK.Framework.Reference;
using CBK.Framework.Route;
using SimpleSQL;

namespace CBK.Framework.Collections
{
    /// <summary>
    /// 引用列表实例 该类将允许通过引用服务进行创建与回收 使用完毕后请调用Recycle或者Dispose进行回收
    /// </summary>
    public sealed class ListReference<T> : List<T>, IReference, IDisposable
    {
        [PropertyIgnore]
        [Ignore]
        [JsonIgnore]
        public int SerialId { get; set; }

        [PropertyIgnore]
        [Ignore]
        [JsonIgnore]
        public IReferenceService ReferenceService { get; set; }

        public void OnRecycle()
        {
            Clear();
        }

        public void Dispose()
        {
            this.Recycle();
        }
    }
}