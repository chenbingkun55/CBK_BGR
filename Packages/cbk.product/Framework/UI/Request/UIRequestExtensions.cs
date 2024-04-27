using CBK.Framework.Reference;
using CBK.Framework.Request;

namespace CBK.Framework.UI
{
    /// <summary>
    /// UI请求拓展方法
    /// </summary>
    public static class UIRequestExtensions
    {
        /// <summary>
        /// 为请求设置应答包错误吗
        /// </summary>
        public static T SetResponse<T>(this T request, int errorCode) where T : IUIRequest
        {
            request.SetResponse(ReferenceService.That.GetReference<CommonResponse>().SetErrorCode(errorCode));
            return request;
        }
        
        /// <summary>
        /// 为请求设置应答包错误吗
        /// </summary>
        public static T SetResponse<T>(this T request, FrameworkErrorCode errorCode) where T : IUIRequest
        {
            request.SetResponse(ReferenceService.That.GetReference<CommonResponse>().SetErrorCode(errorCode));
            return request;
        }
    }
}