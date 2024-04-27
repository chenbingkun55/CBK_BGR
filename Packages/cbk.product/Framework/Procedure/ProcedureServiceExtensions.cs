using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CBK.Framework.Procedure.Attribute;

namespace CBK.Framework.Procedure
{
    public static class ProcedureServiceExtensions
    {
        /// <summary>
        /// 判断流程服务是否正在运行指定类型的流程
        /// </summary>
        public static bool IsProcedureRunning<T>(this IProcedureService service) where T : class, IProcedure
        {
            return service.CurrentProcedureType == typeof(T);
        }

        /// <summary>
        /// 运行流程服务
        /// </summary>
        public static void Run<T>(this IProcedureService service, IEnumerable<IProcedure> procedures) where T : class, IProcedure
        {
            service.Run(procedures, typeof(T));
        }

        /// <summary>
        /// 从指定程序集中通过反射特性注册流程
        /// </summary>
        public static void Run(this IProcedureService service, IEnumerable<Assembly> assemblies)
        {
            var procedureTypes = new List<Type>();
            Type entryProcedureType = null;
            var baseType = typeof(IProcedure);

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract)
                        continue;

                    if (!baseType.IsAssignableFrom(type))
                        continue;

                    var attribute = type.GetCustomAttribute<ProcedureAttribute>();
                    if (attribute == null)
                        continue;

                    procedureTypes.Add(type);

                    if (!attribute.IsEntry)
                        continue;

                    if (entryProcedureType != null)
                        throw new ProcedureMoreThanOneEntryException();

                    entryProcedureType = type;
                }
            }

            if (procedureTypes == null)
                throw new ProcedureMissingException();

            if (entryProcedureType == null)
                throw new ProcedureNoEntryException();

            var procedures = procedureTypes.Select(procedureType => (IProcedure)Activator.CreateInstance(procedureType)).ToList();

            service.Run(procedures, entryProcedureType);
        }
    }
}