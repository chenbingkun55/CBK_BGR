using System;

namespace CBK.Framework.Reflect
{
    /// <summary>
    /// 反射服务 为项目提供反射相关的功能
    /// </summary>
    public interface IReflectService
    {
        /// <summary>
        /// 遍历指定基类的所有子类
        /// </summary>
        void ForEachType(Type baseType, Action<Type> action);

        /// <summary>
        /// 遍历标注了指定特性的所有类
        /// </summary>
        void ForEachTypeWithAttribute<T>(Action<Type, T> action) where T : Attribute;

        /// <summary>
        /// 遍历指定基类的所有标注了指定特性的子类
        /// </summary>
        void ForEachTypeWithAttribute<T>(Type baseType, Action<Type, T> action) where T : Attribute;
    }
}