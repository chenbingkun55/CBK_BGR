using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;

namespace CBK.Framework.Request
{
    /// <summary>
    /// 请求接口
    /// </summary>
    public interface IRequest : IReference
    {
        /// <summary>
        /// 执行请求
        /// </summary>
        UniTask<IResponse> Execute(System.Threading.CancellationToken cancellationToken = default);
    }
}