using System;
using System.Collections.Generic;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.ObjectPool;
using CBK.Framework.Reference;
using IngameDebugConsole;
using UnityEngine;

namespace CBK.Framework.DebugCommand
{
    /// <summary>
    /// 基于DebugLogConsole实现的调试命令服务。
    /// </summary>
    internal class DebugLogConsoleDebugCommandService : IDebugCommandService, IInitialize, IDisposable
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }
        
        [Inject]
        public IObjectPoolService ObjectPoolService { get; set; }
        
        private readonly HashSet<string> m_RegisterCommands = new HashSet<string>();
        private readonly HashSet<Type> m_RegisterCustomParameterTypes = new HashSet<Type>();

        private GameObject m_GameObject;

        public bool IsAvailable => true;

        public void Initialize()
        {
            m_GameObject = ObjectPoolService.Instantiate("Prefab_IngameDebugConsole");
        }

        public void Dispose()
        {
            foreach (var command in m_RegisterCommands)
                DebugLogConsole.RemoveCommand(command);
            
            foreach (var parameterType in m_RegisterCustomParameterTypes)
                DebugLogConsole.RemoveCustomParameterType(parameterType);

            if (m_GameObject != null)
                ObjectPoolService.Release(m_GameObject);
        }

        public void RegisterCommand(string command, string description, Action method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, method);
        }

        public void RegisterCommand<T1>(string command, string description, Action<T1> method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, method);
        }

        public void RegisterCommand<T1, T2>(string command, string description, Action<T1, T2> method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, method);
        }

        public void RegisterCommand<T1, T2, T3>(string command, string description, Action<T1, T2, T3> method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, method);
        }

        public void RegisterCommand<T1, T2, T3, T4>(string command, string description, Action<T1, T2, T3, T4> method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, method);
        }

        public void RegisterCommand<T1>(string command, string description, Func<T1> func)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, func);
        }

        public void RegisterCommand<T1, T2>(string command, string description, Func<T1, T2> func)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, func);
        }

        public void RegisterCommand<T1, T2, T3>(string command, string description, Func<T1, T2, T3> func)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, func);
        }

        public void RegisterCommand<T1, T2, T3, T4>(string command, string description, Func<T1, T2, T3, T4> func)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, func);
        }

        public void RegisterCommand<T1, T2, T3, T4, T5>(string command, string description, Func<T1, T2, T3, T4, T5> func)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, func);
        }

        public void RegisterCommand(string command, string description, Delegate method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddCommand(command, description, method);
        }

        public void RegisterMappingCommand<T>(string command, string description, Action<T> method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddMappingCommand(command, description, method);
        }

        public void RegisterMappingCommand<T1, T2>(string command, string description, Func<T1, T2> method)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddMappingCommand(command, description, method);
        }

        public void RegisterMappingCommand(string command, string description, Delegate method, Type parameterType)
        {
            m_RegisterCommands.Add(command);
            DebugLogConsole.AddMappingCommand(command, description, method, parameterType);
        }

        public void RegisterCustomParameterType(Type parameterType, Func<string, object> parseMethod)
        {
            m_RegisterCustomParameterTypes.Add(parameterType);
            DebugLogConsole.AddCustomParameterType(parameterType, (string input, out object output) =>
            {
                output = parseMethod(input);
                return output != null;
            });
        }

        public void RegisterCustomParameterType(Type parameterType, Func<List<string>, object> parseMethod)
        {
            m_RegisterCustomParameterTypes.Add(parameterType);
            DebugLogConsole.AddCustomParameterType(parameterType, (string input, out object output) =>
            {
                using var arguments = ReferenceService.GetListReference<string>();
                DebugLogConsole.FetchArgumentsFromCommand(input, arguments);
                output = parseMethod(arguments);
                return output != null;
            });
        }
    }
}