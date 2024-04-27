using System;
using System.Collections.Generic;
using System.Reflection;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Reference;
using CBK.Framework.Reflect;
using CBK.Framework.Request;
using CBK.Framework.Utility;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由服务实现
    /// </summary>
    internal sealed class RouteServiceImpl : IInitialize, IDisposable, IRouteService
    {
        [Inject]
        public IReflectService ReflectService { get; set; }

        [Inject]
        public IReferenceService ReferenceService { get; set; }

        /// <summary>
        /// 路由映射表
        /// </summary>
        private readonly Dictionary<string, RequestTypeInfo> m_RouteMap = new();

        public void Initialize()
        {
            ReflectService.ForEachType<IRouteFinder>(type =>
            {
                var finder = (IRouteFinder)Activator.CreateInstance(type);
                finder.FindRouteRequests((route, requestType) =>
                {
                    if (m_RouteMap.ContainsKey(route))
                    {
                        Log.Error($"Route {route} is already exists.");
                        return;
                    }

                    m_RouteMap.Add(route, new RequestTypeInfo(requestType));
                });
            });
        }

        public void Dispose()
        {
            m_RouteMap.Clear();
        }

        public RequestMakeResponse MakeRequest(string route)
        {
            try
            {
                var dict = ReferenceService.GetReference<RefDictionary<string, string>>();
                using var _ = dict.AutoRecycle();

                var routeKey = RouteFormatParser.Parse(route, dict);
                if (string.IsNullOrEmpty(routeKey))
                    return ReferenceService.GetReference<RequestMakeResponse>().SetErrorCode(FrameworkErrorCode.RouteFormatError);

                if (!m_RouteMap.TryGetValue(routeKey, out var info))
                    return ReferenceService.GetReference<RequestMakeResponse>().SetErrorCode(FrameworkErrorCode.RouteUndefined);

                // 检测是否有不可为空的属性没有对应的参数值
                foreach (var key in info.NotNullProperties)
                {
                    if (dict.ContainsKey(key))
                        continue;

                    Log.Error($"Route {routeKey} not found {key} in parameters.");
                    return ReferenceService.GetReference<RequestMakeResponse>().SetErrorCode(FrameworkErrorCode.RouteNotNullParameterUndefined);
                }

                var request = (IRequest)ReferenceService.GetReference(info.Type);
                var code = 0;

                // 参数赋值 若参数类型转换失败则返回错误
                foreach (var pair in dict)
                {
                    if (!info.Properties.TryGetValue(pair.Key, out var propertyInfo))
                        continue;

                    var handler = SetPropertyHandlers[propertyInfo.PropertyType];
                    if (handler(request, propertyInfo, pair.Value))
                        continue;

                    code = (int)FrameworkErrorCode.RouteParameterTypeNotMatch;
                    break;
                }

                if (code != 0)
                {
                    request.Recycle();
                    return ReferenceService.GetReference<RequestMakeResponse>().SetErrorCode(code);
                }

                var response = ReferenceService.GetReference<RequestMakeResponse>();
                response.Request = request;
                return response;
            }
            catch (Exception e)
            {
                Log.Exception(e);
                return ReferenceService.GetReference<RequestMakeResponse>().SetErrorCode(FrameworkErrorCode.RequestCatchException);
            }
        }

        /// <summary>
        /// 请求类型信息
        /// </summary>
        private sealed class RequestTypeInfo
        {
            /// <summary>
            /// 类型实例
            /// </summary>
            public Type Type { get; }

            /// <summary>
            /// 不可为空的属性列表
            /// </summary>
            public List<string> NotNullProperties
            {
                get
                {
                    if (m_NotNullProperties == null)
                        Initialize();

                    return m_NotNullProperties;
                }
            }

            /// <summary>
            /// 属性列表
            /// </summary>
            public Dictionary<string, PropertyInfo> Properties
            {
                get
                {
                    if (m_Properties == null)
                        Initialize();

                    return m_Properties;
                }
            }

            public RequestTypeInfo(Type type)
            {
                Type = type;
            }

            private void Initialize()
            {
                if (m_Properties != null)
                    return;

                m_NotNullProperties = new List<string>();
                m_Properties = new Dictionary<string, PropertyInfo>();

                var properties = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
                foreach (var propertyInfo in properties)
                {
                    // 需要同时支持getter和setter
                    if (!propertyInfo.CanRead || !propertyInfo.CanWrite)
                        continue;
                    
                    // 跳过忽略的属性
                    if (propertyInfo.GetCustomAttribute<PropertyIgnoreAttribute>() != null)
                        continue;

                    // 跳过不支持的属性类型
                    if (!IsSupportRoutePropertyType(propertyInfo.PropertyType))
                    {
                        Log.Error($"Not support property type {propertyInfo.PropertyType.Name} in route type {Type.Name}, property name is {propertyInfo.Name}.");
                        continue;
                    }

                    var keyAttribute = propertyInfo.GetCustomAttribute<PropertyKeyAttribute>();
                    // 若未指定key 则使用属性名
                    var key = keyAttribute != null ? keyAttribute.Key : propertyInfo.Name;

                    // 重复的key
                    if (m_Properties.ContainsKey(key))
                    {
                        Log.Error($"Duplicate key {key} in route type {Type.Name}.");
                        continue;
                    }

                    m_Properties.Add(key, propertyInfo);

                    // 不可为空的属性记录
                    if (propertyInfo.GetCustomAttribute<PropertyNotNullAttribute>() != null)
                        m_NotNullProperties.Add(key);
                }
            }

            private bool IsSupportRoutePropertyType(Type type)
            {
                return SetPropertyHandlers.ContainsKey(type);
            }

            private List<string> m_NotNullProperties;
            private Dictionary<string, PropertyInfo> m_Properties;
        }

        private delegate bool TrySetPropertyHandler(object obj, PropertyInfo propertyInfo, string str);

        private static readonly Dictionary<Type, TrySetPropertyHandler> SetPropertyHandlers = new Dictionary<Type, TrySetPropertyHandler>()
        {
            { typeof(string), SetStringProperty },
            { typeof(int), SetIntProperty },
            { typeof(long), SetInt64Property },
            { typeof(float), SetFloatProperty },
            { typeof(double), SetDoubleProperty },
            { typeof(bool), SetBoolProperty },
            { typeof(uint), SetUIntProperty },
            { typeof(ulong), SetUInt64Property },
            { typeof(short), SetInt16Property },
            { typeof(ushort), SetUInt16Property },
            { typeof(byte), SetByteProperty },
            { typeof(sbyte), SetSByteProperty },
            { typeof(char), SetCharProperty },
            { typeof(decimal), SetDecimalProperty },
        };

        #region [SetPropertyHandler]

        private static bool SetDecimalProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!decimal.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetCharProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!char.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetSByteProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!sbyte.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetByteProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!byte.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetUInt16Property(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!ushort.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetInt16Property(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!short.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetUInt64Property(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!ulong.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetUIntProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!uint.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetBoolProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!bool.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetDoubleProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!double.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetFloatProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!float.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetInt64Property(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!long.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetIntProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            if (!int.TryParse(str, out var value))
                return false;

            propertyInfo.SetValue(obj, value);
            return true;
        }

        private static bool SetStringProperty(object obj, PropertyInfo propertyInfo, string str)
        {
            propertyInfo.SetValue(obj, str);
            return true;
        }

        #endregion
    }
}