using System;
using CBK.Framework.Database;
using CBK.Framework.Reference;

namespace CBK.Framework.Persistence.Database
{
    internal partial class DatabasePersistenceServiceImpl
    {
        private void DisposePersistenceData(PersistenceData persistenceData)
        {
            _persistenceDatas.Remove(persistenceData);

            persistenceData.DatabaseConnection?.Dispose();
            ReferenceService.Recycle(persistenceData);
        }

        private sealed class PersistenceData : IPersistenceData, IReference
        {
            public Action<PersistenceData> OnDispose { get; set; }
            public IDatabaseConnection DatabaseConnection { get; set; }
            public IReferenceService ReferenceService { get; set; }
            public int SerialId { get; set; }

            public void Dispose()
            {
                OnDispose?.Invoke(this);
            }

            public void OnRecycle()
            {
                OnDispose = null;
                DatabaseConnection = null;
            }

            public bool HasKey(string key)
            {
                return DatabaseConnection?.Find<PersistenceObject>(key) != null;
            }

            public void DeleteKey(string key)
            {
                DatabaseConnection?.DeleteByPK<PersistenceObject>(key);
            }

            public void DeleteAll()
            {
                DatabaseConnection?.DeleteAll<PersistenceObject>();
            }

            public void SetInt(string key, int value)
            {
                if (DatabaseConnection == null)
                    return;

                var po = ReferenceService.GetReference<PersistenceObject>();
                po.Key = key;
                po.IntValue = value;
                DatabaseConnection.InsertOrUpdate(po);
                ReferenceService.Recycle(po);
            }

            public int GetInt(string key, int defaultValue = 0)
            {
                if (DatabaseConnection == null)
                    return defaultValue;

                var po = DatabaseConnection.Find<PersistenceObject>(key);
                if (po == null)
                    return defaultValue;

                var intValue = po.IntValue;
                ReferenceService.Recycle(po);
                return intValue;
            }

            public void SetFloat(string key, float value)
            {
                if (DatabaseConnection == null)
                    return;

                var po = ReferenceService.GetReference<PersistenceObject>();
                po.Key = key;
                po.FloatValue = value;
                DatabaseConnection.InsertOrUpdate(po);
                ReferenceService.Recycle(po);
            }

            public float GetFloat(string key, float defaultValue = 0)
            {
                if (DatabaseConnection == null)
                    return defaultValue;

                var po = DatabaseConnection.Find<PersistenceObject>(key);
                if (po == null)
                    return defaultValue;

                var floatValue = po.FloatValue;
                ReferenceService.Recycle(po);
                return floatValue;
            }

            public void SetString(string key, string value)
            {
                if (DatabaseConnection == null)
                    return;

                var po = ReferenceService.GetReference<PersistenceObject>();
                po.Key = key;
                po.StringValue = value;
                DatabaseConnection.InsertOrUpdate(po);
                ReferenceService.Recycle(po);
            }

            public string GetString(string key, string defaultValue = "")
            {
                if (DatabaseConnection == null)
                    return defaultValue;

                var po = DatabaseConnection.Find<PersistenceObject>(key);
                if (po == null)
                    return defaultValue;

                var stringValue = po.StringValue;
                ReferenceService.Recycle(po);
                return stringValue;
            }
        }
    }
}