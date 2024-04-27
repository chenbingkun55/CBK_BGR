using System;

namespace CBK.Framework.Fsm.Exception
{
    /// <summary>
    /// 状态机 状态未找到的异常
    /// </summary>
    public sealed class FsmStateNotFoundException : System.Exception
    {
        public Type stateType { get; set; }

        public FsmStateNotFoundException(Type stateType)
        {
            this.stateType = stateType;
        }
    }
}