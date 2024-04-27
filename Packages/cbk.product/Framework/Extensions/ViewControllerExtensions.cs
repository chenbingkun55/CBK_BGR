using System.Collections.Generic;
using CBK.Framework.Business;

namespace CBK.Framework.Extensions
{
    /// <summary>
    /// 视图控制器拓展方法
    /// </summary>
    public static class ViewControllerExtensions
    {
        /// <summary>
        /// 添加视图控制器
        /// </summary>
        public static void Add<T>(this List<IViewController> buffer) where T : IViewController, new()
        {
            buffer.Add(new T());
        }
    }
}