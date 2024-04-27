using System;
using System.Collections.Generic;
using CatLib.Container;
using CBK.Framework.File;
using CBK.Framework.Path;
using CBK.Framework.Reference;
using SimpleSQL;

namespace CBK.Framework.Database.SQLite
{
    /// <summary>
    /// 基于SQLite的数据库服务实现
    /// </summary>
    internal sealed partial class SQLiteDatabaseServiceImpl : IDisposable, IDatabaseService
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }
        
        [Inject]
        public IPathService PathService { get; set; }
        
        [Inject]
        public IFileService FileService { get; set; }

        public void Dispose()
        {
            foreach (var connection in _rawConnections.Values)
                connection.Dispose();

            _rawConnections.Clear();
            _rawRefCounts.Clear();
        }

        public IDatabaseConnection Allocate(string identifier)
        {
            var connect = ReferenceService.GetReference<SQLiteDatabaseConnection>();
            connect.OnDispose = DisposeConnection;
            connect.Identifier = identifier;
            connect.Connection = AddReference(identifier);

            return connect;
        }

        private SQLiteConnection AddReference(string identifier)
        {
            if (!_rawConnections.TryGetValue(identifier, out var connection))
            {
                connection = new SQLiteConnection(ResolvePath(identifier));
                _rawConnections.Add(identifier, connection);
            }

            if (!_rawRefCounts.TryGetValue(identifier, out var refCount))
                refCount = 0;

            _rawRefCounts[identifier] = refCount + 1;
            return connection;
        }

        private void RemoveReference(string identifier)
        {
            if (!_rawRefCounts.TryGetValue(identifier, out var refCount) || refCount == 0)
                return;

            --refCount;
            _rawRefCounts[identifier] = refCount;
        }

        private string ResolvePath(string identifier)
        {
            var directoryPath = PathService.Resolve(PathType.PersistentDataPath, "Database");
            FileService.CreateDirectory(directoryPath);
            return PathService.Resolve(PathType.PersistentDataPath, "Database", $"{identifier}.db");
        }

        public void CleanUnusedConnections()
        {
            foreach (var (key, refCount) in _rawRefCounts)
            {
                if (refCount > 0)
                    continue;

                if (!_rawConnections.TryGetValue(key, out var connection))
                    continue;

                connection.Dispose();
                _rawConnections.Remove(key);
            }
        }

        private readonly Dictionary<string, SQLiteConnection> _rawConnections = new();
        private readonly Dictionary<string, int> _rawRefCounts = new();
    }
}