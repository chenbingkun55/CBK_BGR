using System.Collections.Generic;

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面分组配置
    /// </summary>
    public interface IUIGroupConfiguration
    {
        /// <summary>
        /// 界面组名称集合
        /// </summary>
        IEnumerable<string> GroupNames { get; }
    }
}