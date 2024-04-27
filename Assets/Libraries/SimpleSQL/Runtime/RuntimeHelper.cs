using System;
using System.Reflection;

namespace SimpleSQL
{
    static public class RuntimeHelper
    {
        public static Func<Type, object> CreateInstanceFunc
        {
            get => _createInstanceFunc;
            set => _createInstanceFunc = value ?? DefaultCreateInstanceFunc;
        }

        public static Func<PropertyInfo, Type> GetPropertyTypeFunc
        {
            get => _getPropertyTypeFunc;
            set => _getPropertyTypeFunc = value ?? DefaultGetPropertyTypeFunc;
        }

        private static Func<Type, object> _createInstanceFunc = DefaultCreateInstanceFunc;
        private static Func<PropertyInfo, Type> _getPropertyTypeFunc = DefaultGetPropertyTypeFunc;

        private static Type DefaultGetPropertyTypeFunc(PropertyInfo prop)
        {
            return Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        }

        private static object DefaultCreateInstanceFunc(Type arg)
        {
            return Activator.CreateInstance(arg);
        }

        public static object CreateInstance(Type type)
        {
            return CreateInstanceFunc(type);
        }
    }
}