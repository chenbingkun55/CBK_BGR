using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;

namespace CBK.Framework.Request
{
    /// <summary>
    /// 响应实例拓展
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// 设置错误码
        /// </summary>
        public static T SetErrorCode<T>(this T response, int errorCode) where T : IResponse
        {
            response.ErrorCode = errorCode;
            return response;
        }

        /// <summary>
        /// 设置框架错误码
        /// </summary>
        public static T SetErrorCode<T>(this T response, FrameworkErrorCode errorCode) where T : IResponse
        {
            response.ErrorCode = (int)errorCode;
            return response;
        }

        /// <summary>
        /// 响应是否成功
        /// </summary>
        public static bool IsSuccess(this IResponse response)
        {
            return response.ErrorCode == (int)FrameworkErrorCode.Success;
        }

        /// <summary>
        /// 摘取应答包的错误码
        /// </summary>
        public static async UniTask<int> PickCode(this UniTask<IResponse> task)
        {
            var response = await task;
            var code = response.ErrorCode;
            response.Recycle();
            return code;
        }

        /// <summary>
        /// 摘取应答包的错误码
        /// </summary>
        public static async UniTask<int> PickCode<T>(this UniTask<T> task) where T : IResponse
        {
            var response = await task;
            var code = response.ErrorCode;
            response.Recycle();
            return code;
        }

        /// <summary>
        /// 判断应答包类型 若应答包类型不匹配则会返回对应的错误码
        /// </summary>
        public static T ParseResponse<T>(this IResponse response) where T : IResponse, new()
        {
            // 应答包类型与期望相同 直接返回
            if (response is T destResponse)
                return destResponse;

            destResponse = ReferenceService.That.GetReference<T>();

            // 应答包为空
            if (response == null)
                return destResponse.SetErrorCode(FrameworkErrorCode.ResponseIsNull);

            // 原始应答包本身有返回错误码时 返回其对应的错误码 否则返回应答包类型不匹配的错误码
            destResponse.ErrorCode = response.IsSuccess() ? (int)FrameworkErrorCode.ResponseTypeNotMatch : response.ErrorCode;
            response.Recycle();
            return destResponse;
        }

        /// <summary>
        /// 异步等待任务 并判断应答包类型 若应答包类型不匹配则会返回对应的错误码
        /// </summary>
        public static async UniTask<T> ParseResponse<T>(this UniTask<IResponse> task) where T : IResponse, new()
        {
            return (await task).ParseResponse<T>();
        }
    }
}