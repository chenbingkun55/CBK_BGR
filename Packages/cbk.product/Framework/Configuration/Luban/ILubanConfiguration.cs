using System.Collections.Generic;
using Bright.Serialization;

namespace CBK.Framework.Configuration.Luban
{
    /// <summary>
    /// Luban配置实例接口
    /// </summary>
    public interface ILubanConfiguration : IConfiguration
    {
        /// <summary>
        /// 资源名
        /// </summary>
        string AssetName { get; }
        
        /// <summary>
        /// 类型全名
        /// </summary>
        string FullName { get; }
        
        /// <summary>
        /// 通过二进制数据加载配置
        /// </summary>
        void LoadFromBytes(ByteBuf buffer);

        /// <summary>
        /// Luban生命周期 - 处理依赖关系
        /// </summary>
        void Resolve(Dictionary<string, object> tables);
        
        /// <summary>
        /// Luban生命周期 - 处理文本翻译
        /// </summary>
        void TranslateText(System.Func<string, string, string> translator);
    }
}