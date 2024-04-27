using System;

namespace CBK.Framework.Reflect
{
    public static class ReflectServiceExtensions
    {
        /// <summary>
        /// 遍历指定基类的所有子类
        /// </summary>
        public static void ForEachType<T>(this IReflectService service, Action<Type> action)
        {
            service.ForEachType(typeof(T), action);
        }
    }
}