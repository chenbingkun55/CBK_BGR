using System;
using System.Collections.Generic;

namespace CBK.Framework.DebugCommand
{
    /// <summary>
    /// 游戏调试命令服务
    /// </summary>
    public interface IDebugCommandService
    {
        /// <summary>
        /// 调试命令服务是否可用
        /// </summary>
        bool IsAvailable { get; }
        
        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand(string command, string description, Action method);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1>(string command, string description, Action<T1> method);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2>(string command, string description, Action<T1, T2> method);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2, T3>(string command, string description, Action<T1, T2, T3> method);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2, T3, T4>(string command, string description, Action<T1, T2, T3, T4> method);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1>(string command, string description, Func<T1> func);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2>(string command, string description, Func<T1, T2> func);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2, T3>(string command, string description, Func<T1, T2, T3> func);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2, T3, T4>(string command, string description, Func<T1, T2, T3, T4> func);

        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand<T1, T2, T3, T4, T5>(string command, string description, Func<T1, T2, T3, T4, T5> func);
        
        /// <summary>
        /// 注册调试命令
        /// </summary>
        void RegisterCommand(string command, string description, Delegate method);

        /// <summary>
        /// 注册映射命令 会根据函数参数类型自动解析构造参数实例
        /// </summary>
        void RegisterMappingCommand<T>(string command, string description, Action<T> method);
        
        /// <summary>
        /// 注册映射命令 会根据函数参数类型自动解析构造参数实例
        /// </summary>
        void RegisterMappingCommand<T1, T2>(string command, string description, Func<T1, T2> method);
        
        /// <summary>
        /// 注册映射命令 会根据函数参数类型自动解析构造参数实例
        /// </summary>
        void RegisterMappingCommand(string command, string description, Delegate method, Type parameterType);
        
        /// <summary>
        /// 注册自定义参数类型
        /// </summary>
        void RegisterCustomParameterType(Type parameterType, Func<string, object> parseMethod);
        
        /// <summary>
        /// 注册自定义参数类型
        /// </summary>
        void RegisterCustomParameterType(Type parameterType, Func<List<string>, object> parseMethod);
    }
}