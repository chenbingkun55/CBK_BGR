namespace CBK.Framework.CoroutineLock.Exception
{
    /// <summary>
    /// 协程锁类型越界异常
    /// </summary>
    public sealed class CoroutineLockTypeOutOfRangeException : System.Exception
    {
        public uint Min { get; }
        public uint Max { get; }
        public uint Value { get; }

        public CoroutineLockTypeOutOfRangeException(uint min, uint max, uint value)
        {
            Min = min;
            Max = max;
            Value = value;
        }
    }
}