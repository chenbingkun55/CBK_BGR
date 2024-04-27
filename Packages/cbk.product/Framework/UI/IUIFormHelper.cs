using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面辅助器接口
    /// </summary>
    public interface IUIFormHelper
    {
        /// <summary>
        /// 创建界面
        /// </summary>
        UniTask<IUIForm> CreateUIForm(string uiKey, CancellationToken token);
        
        /// <summary>
        /// 释放界面
        /// </summary>
        void RecycleUIForm(IUIForm uiForm);
    }
}