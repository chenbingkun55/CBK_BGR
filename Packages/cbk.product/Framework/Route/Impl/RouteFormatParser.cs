using System;
using System.Collections.Generic;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由格式解析器
    /// </summary>
    internal static class RouteFormatParser
    {
        /// <summary>
        /// 从字符串中解析路由键值与参数
        /// </summary>
        /// <param name="route">用于解析的字符串</param>
        /// <param name="parameters">解析得出的参数键值对</param>
        /// <returns>路由键值</returns>
        public static string Parse(string route, Dictionary<string, string> parameters)
        {
            parameters.Clear();
            
            var routeSplitIndex = route.IndexOf(RouteFormat.RouteSplit, StringComparison.Ordinal);
            // 若不存在分隔符 则代表没有参数
            if (routeSplitIndex < 0)
                return route;
            
            var routeKey = route[..routeSplitIndex];
            // 若分隔符在最后一位 则也代表没有参数
            if (routeSplitIndex == route.Length - 1)
                return routeKey;
            
            var beginIndex = routeSplitIndex + 1;
            while (beginIndex < route.Length)
            {
                var parameterSplitIndex = route.IndexOf(RouteFormat.RouteParameterSplit, beginIndex, StringComparison.Ordinal);
                if (parameterSplitIndex < 0)
                {
                    // 若不存在分隔符 则代表只有一个参数
                    ParseParameter(route[beginIndex..], parameters);
                    break;
                }
                
                // 若分隔符在第一位 则跳过本次参数解析
                if (parameterSplitIndex == beginIndex)
                {
                    beginIndex = parameterSplitIndex + 1;
                    continue;
                }
                
                var parameter = route[beginIndex..parameterSplitIndex];
                ParseParameter(parameter, parameters);
                beginIndex = parameterSplitIndex + 1;
            }
            
            return routeKey;
        }

        /// <summary>
        /// 解析单个参数 若解析成功则添加到参数键值对中
        /// </summary>
        private static void ParseParameter(string str, Dictionary<string, string> parameters)
        {
            if (!ParseParameter(str, out var key, out var value)) 
                return;
            
            parameters[key] = value;
        }

        /// <summary>
        /// 解析单个参数
        /// </summary>
        private static bool ParseParameter(string str, out string key, out string value)
        {
            var keyValueSplitIndex = str.IndexOf(RouteFormat.RouteParameterKeyValueSplit, StringComparison.Ordinal);
            if (keyValueSplitIndex <= 0 || keyValueSplitIndex == str.Length - 1)
            {
                // 若不存在分隔符、分隔符是第一位或最后一位 则代表参数格式不合法 直接抛弃
                key = str;
                value = string.Empty;
                return true;
            }

            key = str[..keyValueSplitIndex];
            value = str[(keyValueSplitIndex + 1)..];
            return true;
        }
    }
}