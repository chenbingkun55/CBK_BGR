using System;
using CBK.Framework.Reference;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 事件参数抽象类
    /// </summary>
    public abstract class AEventArgs : EventArgs, IReference
    {
        public int SerialId { get; set; }
        public IReferenceService ReferenceService { get; set; }
        public abstract void OnRecycle();
    }
}