using System;
using CBK.Core;
using CBK.Product.Data;
using CBK.Product.Config;
using UnityEngine;

namespace CBK.Product.Save
{
    public class SaveManager
    {
        private readonly Save m_saveData = new Save();
        
        public bool Initialize()
        {
            LoadSaveDataFromDisk();

            return true;
        }

        public void Destroy()
        {

        }

        /// <summary>
        /// 加载文件数据
        /// </summary>
        public void LoadSaveDataFromDisk()
        {
            if(FileManager.ReadFile(out string json, AppConfig.kSaveFileName))
            {
                m_saveData.LoadFromJson(json);
            }
        }

        /// <summary>
        /// 加载记录数据
        /// </summary>
        public void LoadSaveRecordData()
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;

            // Title
            recordData.dataTitle.formatVersion = m_saveData.dataTitleSer.formatVersion;

            // Data
            recordData.recordData.Clear();
            foreach(var dataSer in m_saveData.listRecordDataSer)
            {
                var record = new Record();
                record.dateTime = DateTime.Parse(dataSer.strDateTime);
                record.afterMealTime = dataSer.afterMealTime;
                record.eatType = (EatType)dataSer.eatType;
                record.medicineName = dataSer.medicineName;
                record.medicineAmount = dataSer.medicineAmount;
                record.notice = dataSer.notice;

                recordData.recordData.Add(record);
            }
        }

        /// <summary>
        /// 保存记录数据
        /// </summary>
        public void SaveDataToDisk()
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;

            // Title
            m_saveData.dataTitleSer.formatVersion = AppConfig.kRecordDataVersion;

            // Data
            m_saveData.listRecordDataSer.Clear();
            foreach (var data in recordData.recordData)
            {
                var recordDataSer = new RecordDataSerializable();
                
                recordDataSer.strDateTime = data.dateTime.ToString("s");
                recordDataSer.afterMealTime = data.afterMealTime;
                recordDataSer.eatType = (int)data.eatType;
                recordDataSer.medicineName = data.medicineName;
                recordDataSer.medicineAmount = data.medicineAmount;
                recordDataSer.notice = data.notice;

                m_saveData.listRecordDataSer.Add(recordDataSer);
            }

            if (FileManager.WriteFile(AppConfig.kSaveFileName, m_saveData.ToJson()))
            {
                Debug.Log("Save successful " + AppConfig.kSaveFileName);
            }
        }
    }
}