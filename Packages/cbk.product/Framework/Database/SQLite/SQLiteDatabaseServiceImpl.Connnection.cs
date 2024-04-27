using System;
using System.Collections.Generic;
using CBK.Framework.Reference;
using SimpleSQL;

namespace CBK.Framework.Database.SQLite
{
    internal partial class SQLiteDatabaseServiceImpl
    {
        private void DisposeConnection(SQLiteDatabaseConnection connection)
        {
            if (!string.IsNullOrEmpty(connection.Identifier))
                RemoveReference(connection.Identifier);
            
            ReferenceService.Recycle(connection);
        }
        
        private sealed class SQLiteDatabaseConnection : AReference, IDatabaseConnection
        {
            public string Identifier { get; set; }
            public SQLiteConnection Connection { get; set; }
            
            public Action<SQLiteDatabaseConnection> OnDispose { get; set; }

            public void Dispose()
            {
                OnDispose?.Invoke(this);
            }

            public override void OnRecycle()
            {
                Identifier = string.Empty;
                Connection = null;
                OnDispose = null;
            }

            public T Find<T>(object pk) where T : class
            {
                CheckTableExists<T>();
                return Connection?.Find(pk, typeof(T)) as T;
            }

            public bool Update<T>(T obj) where T : class
            {
                CheckTableExists<T>();
                return Connection?.Update(obj, typeof(T)) > 0;
            }

            public bool Insert<T>(T obj) where T : class
            {
                CheckTableExists<T>();
                return Connection?.Insert(obj, typeof(T), out _) > 0;
            }

            public bool InsertOrUpdate<T>(T obj) where T : class
            {
                CheckTableExists<T>();
                return Connection?.InsertOrUpdate(obj, typeof(T), out _) > 0;
            }

            public bool Delete<T>(T obj) where T : class
            {
                CheckTableExists<T>();
                return Connection?.Delete(obj, typeof(T)) > 0;
            }

            public bool DeleteByPK<T>(object pk) where T : class
            {
                CheckTableExists<T>();
                return Connection?.DeleteByPK(pk, typeof(T)) > 0;
            }

            public bool DeleteAll<T>() where T : class
            {
                if (Connection == null)
                    return false;

                var mapping = Connection.GetMapping(typeof(T));
                
                Connection.Execute("DROP TABLE IF EXISTS " + mapping.TableName);
                return true;
            }

            public void Query<T>(string query, List<T> result) where T : class
            {
                CheckTableExists<T>();
                var objects = Connection.Query(typeof(T), query);
                foreach (var obj in objects)
                {
                    if (!(obj is T inst))
                        continue;

                    result.Add(inst);
                }
            }

            public int ExecuteRawQuery(string query)
            {
                return Connection?.Execute(query) ?? 0;
            }

            private void CheckTableExists<T>() where T : class
            {
                Connection?.CreateTable(typeof(T));
            }
        }
    }
}