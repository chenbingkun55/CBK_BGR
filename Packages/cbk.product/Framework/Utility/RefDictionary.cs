using System.Collections.Generic;
using CBK.Framework.Reference;

namespace CBK.Framework.Utility
{
    /// <summary>
    /// 引用字典
    /// </summary>
    public sealed class RefDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IReference
    {
        public int SerialId { get; set; }
        public IReferenceService ReferenceService { get; set; }

        public RefDictionary()
        {
            m_ValueIsReference = typeof(IReference).IsAssignableFrom(typeof(TValue));
        }

        public void OnRecycle()
        {
            RecycleItems();
        }

        /// <summary>
        /// 回收所有值 并清空字典
        /// </summary>
        public void RecycleItems()
        {
            if (m_ValueIsReference)
            {
                foreach (var value in Values)
                {
                    if (value == null)
                        continue;

                    ((IReference)value).Recycle();
                }
            }

            Clear();
        }

        private readonly bool m_ValueIsReference;
    }
}