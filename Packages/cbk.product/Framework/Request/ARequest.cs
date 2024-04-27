using System;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using CBK.Framework.Request.Exception;

namespace CBK.Framework.Request
{
    /// <summary>
    /// 请求实例抽象类
    /// </summary>
    public abstract class ARequest : AReference, IRequest
    {
        public async UniTask<IResponse> Execute(System.Threading.CancellationToken cancellationToken = default)
        {
            if (SerialId == 0)
                throw new RequestShouldBeGetByReferenceServiceException();

            IResponse response;

            try
            {
                response = await OnExecute(cancellationToken);
            }
            catch (System.Exception e)
            {
                Log.Exception(e);
                response = ReferenceService.GetReference<CommonResponse>().SetErrorCode(FrameworkErrorCode.RequestCatchException);
            }

            // 执行完请求后回收请求实例
            this.Recycle();

            if (response != null)
                return response;

            // 应答包为空
            response = ReferenceService.GetReference<CommonResponse>().SetErrorCode(FrameworkErrorCode.ResponseIsNull);

            return response;
        }

        protected abstract UniTask<IResponse> OnExecute(System.Threading.CancellationToken cancellationToken);
    }
}