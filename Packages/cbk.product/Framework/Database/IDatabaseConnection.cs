using System;
using System.Collections.Generic;

namespace CBK.Framework.Database
{
    /// <summary>
    /// 数据库连接接口 用于与数据库交互
    /// </summary>
    public interface IDatabaseConnection : IDisposable
    {
        /// <summary>
        /// 数据库标识符
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// 根据主键获取数据实例 如果不存在则返回null
        /// </summary>
        T Find<T>(object pk) where T : class;

        /// <summary>
        /// 更新数据实例
        /// </summary>
        bool Update<T>(T obj) where T : class;

        /// <summary>
        /// 插入数据实例
        /// </summary>
        bool Insert<T>(T obj) where T : class;
        
        /// <summary>
        /// 插入或更新数据实例
        /// </summary>
        bool InsertOrUpdate<T>(T obj) where T : class;

        /// <summary>
        /// 删除数据实例
        /// </summary>
        bool Delete<T>(T obj) where T : class;

        /// <summary>
        /// 根据主键删除数据实例
        /// </summary>
        bool DeleteByPK<T>(object pk) where T : class;
        
        /// <summary>
        /// 删除所有数据实例
        /// </summary>
        bool DeleteAll<T>() where T : class;

        /// <summary>
        /// 根据命令查询数据
        /// </summary>
        void Query<T>(string query, List<T> result) where T : class;
        
        /// <summary>
        /// 执行原始命令
        /// </summary>
        int ExecuteRawQuery(string query);
    }
}