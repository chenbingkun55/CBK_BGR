using System;
using System.Reflection;
using CatLib.Container;

namespace CBK.Framework.Extensions
{
    public static class CatLibContainerExtensions
    {
        /// <summary>
        /// Inject target instance.
        /// </summary>
        public static void Inject(this IContainer container, object target)
        {
            var type = target.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttribute<InjectAttribute>(true);
                if (attr == null)
                    continue;

                var name = container.Type2Service(property.PropertyType);
                var inst = container.CanMake(name) ? container.Make(name) : null;

                if (inst != null)
                {
                    property.SetValue(target, inst);
                    continue;
                }

                if (attr.Required)
                    throw new UnresolvableException($"{type.Name} dependence on {name}, but {name} is not register.");
            }
        }
        
        /// <summary>
        /// Bind singleton instance.
        /// </summary>
        public static IBindData BindSingletonInstance<T>(this IContainer container, T instance)
        {
            return container.BindSingletonInstance(instance, typeof(T));
        }
        
        /// <summary>
        /// Bind singleton instance.
        /// </summary>
        public static IBindData BindSingletonInstance(this IContainer container, object instance, Type type)
        {
            var name = container.Type2Service(type);
            var bindData = container.Bind(name, instance.GetType(), true);
            container.Instance(name, instance);
            return bindData;
        }
    }
}