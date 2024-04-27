using System;
using System.Net;
using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using CBK.Framework.Request;
using UnityEngine.Networking;

namespace CBK.Framework.Http
{
    /// <summary>
    /// 基于UnityWebRequest的Http服务
    /// </summary>
    internal sealed class UnityHttpService : IHttpService
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }
        
        public async UniTask<HttpResponse> Get(string api, CancellationToken token)
        {
            using var request = UnityWebRequest.Get(api);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;
            
            await request.SendWebRequest().ToUniTask(cancellationToken: token);
            
            var response = ReferenceService.GetReference<HttpResponse>();
            if (token.IsCancellationRequested)
                return response.SetErrorCode(FrameworkErrorCode.RequestHasBeenCanceled);

            response.ErrorCode = ConvertErrorCode(request.responseCode);
            response.Content = request.downloadHandler.text;
            
            return response;
        }

        public async UniTask<HttpResponse> Post(string api, string param, CancellationToken token)
        {
            // param是json字符串
            var bytes = !string.IsNullOrEmpty(param) ? System.Text.Encoding.UTF8.GetBytes(param) : Array.Empty<byte>();
            using var request = new UnityWebRequest(api, UnityWebRequest.kHttpVerbPOST);
            request.SetRequestHeader("Content-Type", "application/json");
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeDownloadHandlerOnDispose = true;
            
            await request.SendWebRequest().ToUniTask(cancellationToken: token);
            
            var response = ReferenceService.GetReference<HttpResponse>();
            if (token.IsCancellationRequested)
                return response.SetErrorCode(FrameworkErrorCode.RequestHasBeenCanceled);

            response.ErrorCode = ConvertErrorCode(request.responseCode);
            response.Content = request.downloadHandler.text;
            return response;
        }

        /// <summary>
        /// 将Http状态码转成错误码
        /// </summary>
        private int ConvertErrorCode(long statusCode)
        {
            // TODO: 这里需要补充
            return (int)FrameworkErrorCode.Success;
        }
    }
}