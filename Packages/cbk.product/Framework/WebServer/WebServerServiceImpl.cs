using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Http;
using CBK.Framework.Json;
using CBK.Framework.Reference;
using CBK.Framework.Request;

namespace CBK.Framework.WebServer
{
    /// <summary>
    /// 中心服服务实现
    /// </summary>
    internal sealed class WebServerServiceImpl : IWebServerService
    {
        [Inject]
        public IWebServerConfiguration Configuration { get; set; }

        [Inject]
        public IHttpService HttpService { get; set; }

        [Inject]
        public IReferenceService ReferenceService { get; set; }

        [Inject]
        public IJsonService JsonService { get; set; }

        public async UniTask<T> SendAsync<T>(WebServerReq<T> req, CancellationToken token) where T : WebServerResp, new()
        {
            using var jsonCore = JsonService.Allocate();

            var api = $"{Configuration.Domain}/game?api={req.Route}";
            var param = req.ToJson(jsonCore);

            var httpResp = await HttpService.Post(api, param, token);
            using var _ = httpResp.AutoRecycle();

            if (!httpResp.IsSuccess())
            {
                var resp = ReferenceService.GetReference<T>();
                resp.ErrorCode = httpResp.ErrorCode;
                return resp;
            }

            if (string.IsNullOrEmpty(httpResp.Content))
            {
                var resp = ReferenceService.GetReference<T>();
                return resp.SetErrorCode(FrameworkErrorCode.ResponseIsNull);
            }

            return httpResp.Content.FromJson<T>(jsonCore);
        }
    }
}