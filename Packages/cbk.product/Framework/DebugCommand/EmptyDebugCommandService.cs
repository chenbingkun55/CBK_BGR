using System;
using System.Collections.Generic;

namespace CBK.Framework.DebugCommand
{
    /// <summary>
    /// 空实现的调试命令服务。
    /// </summary>
    internal sealed class EmptyDebugCommandService : IDebugCommandService
    {
        public bool IsAvailable => false;

        public void RegisterCommand(string command, string description, Action method)
        {
        }

        public void RegisterCommand<T1>(string command, string description, Action<T1> method)
        {
        }

        public void RegisterCommand<T1, T2>(string command, string description, Action<T1, T2> method)
        {
        }

        public void RegisterCommand<T1, T2, T3>(string command, string description, Action<T1, T2, T3> method)
        {
        }

        public void RegisterCommand<T1, T2, T3, T4>(string command, string description, Action<T1, T2, T3, T4> method)
        {
        }

        public void RegisterCommand<T1>(string command, string description, Func<T1> func)
        {
        }

        public void RegisterCommand<T1, T2>(string command, string description, Func<T1, T2> func)
        {
        }

        public void RegisterCommand<T1, T2, T3>(string command, string description, Func<T1, T2, T3> func)
        {
        }

        public void RegisterCommand<T1, T2, T3, T4>(string command, string description, Func<T1, T2, T3, T4> func)
        {
        }

        public void RegisterCommand<T1, T2, T3, T4, T5>(string command, string description, Func<T1, T2, T3, T4, T5> func)
        {
        }

        public void RegisterCommand(string command, string description, Delegate method)
        {
        }

        public void RegisterMappingCommand<T>(string command, string description, Action<T> method)
        {
        }

        public void RegisterMappingCommand<T1, T2>(string command, string description, Func<T1, T2> method)
        {
        }

        public void RegisterMappingCommand(string command, string description, Delegate method, Type parameterType)
        {
        }

        public void RegisterCustomParameterType(Type parameterType, Func<string, object> parseMethod)
        {
        }

        public void RegisterCustomParameterType(Type parameterType, Func<List<string>, object> parseMethod)
        {
        }
    }
}