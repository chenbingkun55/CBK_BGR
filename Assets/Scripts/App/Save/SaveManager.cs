using System;
using CBK.Core;
using CBK.Product.Data;
using CBK.Product.Config;
using CBK.Product.Utils;
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

        // 清空所有数据
        public void ClearData()
        {
            m_saveData.ClearRecordData();

            LoadSaveRecordData();
        }

        /// <summary>
        /// 加载文件数据
        /// </summary>
        public void LoadSaveDataFromDisk()
        {
            if(FileManager.ReadFile(out string json, AppConst.kSaveFileName))
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
                record.afterMealTime = (AfterMealTime)dataSer.afterMealTime;
                record.eatType = (EatType)dataSer.eatType;
                record.medicineType = (MedicineType)dataSer.medicineType;
                record.medicineAmount = dataSer.medicineAmount;
                record.sportType = (SportType)dataSer.sportType;
                record.sportAmount = dataSer.sportAmount;
                record.monitorValue = dataSer.monitorValue;
                record.notice = dataSer.notice;

                recordData.recordData.Add(record);
            }

            // 按时间排序
            recordData.recordData.Sort(TimeUtil.SortDateTimeLt);
        }

        /// <summary>
        /// 保存记录数据
        /// </summary>
        public void SaveDataToDisk()
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;

            // Title
            m_saveData.dataTitleSer.formatVersion = AppConst.kRecordDataVersion;

            // Data
            m_saveData.listRecordDataSer.Clear();
            foreach (var data in recordData.recordData)
            {
                var recordDataSer = new RecordDataSerializable();
                
                recordDataSer.strDateTime = data.dateTime.ToString("s");
                recordDataSer.afterMealTime = (int)data.afterMealTime;
                recordDataSer.eatType = (int)data.eatType;
                recordDataSer.medicineType = (int)data.medicineType;
                recordDataSer.medicineAmount = data.medicineAmount;
                recordDataSer.sportType = (int)data.sportType;
                recordDataSer.sportAmount = data.sportAmount;
                recordDataSer.monitorValue = data.monitorValue;
                recordDataSer.notice = data.notice;

                m_saveData.listRecordDataSer.Add(recordDataSer);
            }

            if (FileManager.WriteFile(AppConst.kSaveFileName, m_saveData.ToJson()))
            {
                Debug.Log("Save successful " + AppConst.kSaveFileName);
            }
        }
    }
}