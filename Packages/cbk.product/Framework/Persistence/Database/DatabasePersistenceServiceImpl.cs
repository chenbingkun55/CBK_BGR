using System;
using System.Collections.Generic;
using CatLib.Container;
using CBK.Framework.Database;
using CBK.Framework.Reference;

namespace CBK.Framework.Persistence.Database
{
    /// <summary>
    /// 基于数据库服务实现的持久化数据服务
    /// </summary>
    internal sealed partial class DatabasePersistenceServiceImpl : IDisposable, IPersistenceService
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }
        
        [Inject]
        public IDatabaseService DatabaseService { get; set; }
        
        public void Dispose()
        {
            foreach (var persistenceData in _persistenceDatas)
                persistenceData.DatabaseConnection?.Dispose();
            
            _persistenceDatas.Clear();
        }

        public IPersistenceData Allocate(string identifier)
        {
            var persistenceData = ReferenceService.GetReference<PersistenceData>();
            persistenceData.OnDispose = DisposePersistenceData;
            persistenceData.DatabaseConnection = DatabaseService.Allocate(identifier);
            persistenceData.ReferenceService = ReferenceService;
            _persistenceDatas.Add(persistenceData);
            
            return persistenceData;
        }

        private readonly HashSet<PersistenceData> _persistenceDatas = new();
    }
}