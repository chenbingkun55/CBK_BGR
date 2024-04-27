using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.UI
{
    /// <summary>
    /// UI服务接口
    /// </summary>
    public interface IUIService
    {
        /// <summary>
        /// 是否存在界面。
        /// </summary>
        /// <param name="uiKey">界面键值。</param>
        /// <returns>是否存在界面。</returns>
        bool HasUIForm(string uiKey);

        /// <summary>
        /// 是否正在加载界面。
        /// </summary>
        /// <param name="uiKey">界面键值。</param>
        /// <returns>是否正在加载界面。</returns>
        bool IsLoadingUIForm(string uiKey);

        /// <summary>
        /// 打开UI视图
        /// </summary>
        /// <param name="uiKey">界面键值。</param>
        /// <param name="userdata">用户自定义数据。</param>
        /// <param name="cancellationToken">取消令牌标记。</param>
        /// <returns>打开过程的错误码 0代表成功</returns>
        UniTask<int> OpenUIForm(string uiKey, object userdata = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="uiKey">界面键值。</param>
        /// <param name="userData">用户自定义数据。</param>
        void CloseUIForm(string uiKey, object userData = null);

        /// <summary>
        /// 关闭所有界面。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void CloseAllUIForms(object userData = null);

        /// <summary>
        /// 销毁未使用的界面。
        /// </summary>
        void DestroyUnusedUIForms();

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="uiKey">界面键值。</param>
        /// <param name="userData">用户自定义数据。</param>
        void RefocusUIForm(string uiKey, object userData = null);
    }
}