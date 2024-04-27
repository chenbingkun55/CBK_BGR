using System;
using System.Reflection;
using CatLib.Container;
using CBK.Entry;

namespace CBK.Framework.Reflect
{
    /// <summary>
    /// 反射服务实现类
    /// </summary>
    internal class ReflectServiceImpl : IReflectService
    {
        [Inject]
        public IEntryLoader EntryLoader { get; set; }

        public void ForEachType(Type baseType, Action<Type> action)
        {
            foreach (var assembly in EntryLoader.CurrentAssembles)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    if (!baseType.IsAssignableFrom(type))
                        continue;

                    action(type);
                }
            }
        }

        public void ForEachTypeWithAttribute<T>(Action<Type, T> action) where T : Attribute
        {
            foreach (var assembly in EntryLoader.CurrentAssembles)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    var attribute = type.GetCustomAttribute<T>();
                    if (attribute == null)
                        continue;

                    action(type, attribute);
                }
            }
        }

        public void ForEachTypeWithAttribute<T>(Type baseType, Action<Type, T> action) where T : Attribute
        {
            foreach (var assembly in EntryLoader.CurrentAssembles)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    if (!baseType.IsAssignableFrom(type))
                        continue;

                    var attribute = type.GetCustomAttribute<T>();
                    if (attribute == null)
                        continue;

                    action(type, attribute);
                }
            }
        }
    }
}