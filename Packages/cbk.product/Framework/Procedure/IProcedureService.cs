using System;
using System.Collections.Generic;

namespace CBK.Framework.Procedure
{
    /// <summary>
    /// 流程服务
    /// </summary>
    public interface IProcedureService
    {
        /// <summary>
        /// 当前流程实例
        /// </summary>
        IProcedure CurrentProcedure { get; }
        
        /// <summary>
        /// 当前流程类型
        /// </summary>
        Type CurrentProcedureType { get; }

        /// <summary>
        /// 运行流程服务
        /// </summary>
        void Run(IEnumerable<IProcedure> procedures, Type entryProcedureType);
    }
}