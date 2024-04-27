namespace CBK.Framework.Request.Exception
{
    /// <summary>
    /// 当执行一个请求时 若请求实例不是通过ReferenceService创建的 则会抛出此异常
    /// </summary>
    public sealed class RequestShouldBeGetByReferenceServiceException : System.Exception
    {
    }
}