namespace CBK.Framework.Fsm.Exception
{
    /// <summary>
    /// 状态机操作时运行状态异常
    /// </summary>
    public class FsmOperateStatusException : System.Exception
    {
        public FsmStatus[] Excepted { get; set; }
        public FsmStatus Result { get; set; }

        public FsmOperateStatusException(FsmStatus excepted, FsmStatus result)
        {
            Excepted = new FsmStatus[] { excepted };
            Result = result;
        }

        public FsmOperateStatusException(FsmStatus[] excepted, FsmStatus result)
        {
            Excepted = excepted;
            Result = result;
        }
    }
}