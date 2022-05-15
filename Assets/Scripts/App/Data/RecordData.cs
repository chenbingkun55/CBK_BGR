using System;
using System.Collections.Generic;
using CBK.Product.Utils;

namespace CBK.Product.Data
{
    public class DataTitle
    {
        public int formatVersion;
    }

    // 记录数据结构
    public struct Record
    {
        // public Record()
        // {
        //     // 填充
        //     this.dateTime = DateTime.Now;
        //     this.eatType = EatType.None;
        //     this.afterMealTime = 0;
        //     this.medicineName = string.Empty;
        //     this.medicineAmount = 0;
        //     this.notice = string.Empty;
        // }

        // // 转成记录字符
        // public override string ToString()
        // {
        //     return string.Format("{0},{1},{2},{3},{4},{5}",
        //         dateTime.ToString("s"),
        //         ((int)eatType).ToString(),
        //         afterMealTime.ToString(),
        //         medicineName,
        //         medicineAmount.ToString(),
        //         notice);
        // }

        // 记录唯一ID
        public string GUID;
        // 记录时间
        public DateTime dateTime;
        // 用餐类型
        public EatType eatType;
        // 用餐时间
        public AfterMealTime afterMealTime;
        // 药品名称
        public MedicineType medicineType;
        // 剂量
        public int medicineAmount;
        // 运动类型
        public SportType sportType;
        // 运动量
        public int sportAmount;

        // 监测血糖值
        public float monitorValue;
        // 备注
        public string notice;

        // 空数据
        public static Record Empty = new ();
    }


    // 记录数据
    public class RecordData : IData
    {
        private DataTitle m_dataTitle = new DataTitle();
        private Dictionary<string, Record> m_recordData = new Dictionary<string, Record>();


        // 属性
        public string alterGUID = string.Empty; // 修改GUID记录
        public DataTitle dataTitle => m_dataTitle;
        public Dictionary<string, Record> recordData => m_recordData;

        public bool Initialize()
        {
            return true;
        }

        public void Destroy()
        {

        }

        /// <summary>
        /// 更新记录
        /// </summary>
        public void UpdateRecordData(Record record)
        {
            // 不存在则创建数据
            if(!m_recordData.ContainsKey(record.GUID))
            {
                m_recordData.Add(record.GUID, default);
            }
                
            m_recordData[record.GUID] = record;
        }

        /// <summary>
        /// 获取等待修改的记录
        /// </summary>
        /// <param name="outAlterRecord"></param>
        public void GetWillAlterRecordData(out Record outAlterRecord)
        {
            GetRecordData(out outAlterRecord, alterGUID);
            alterGUID = string.Empty;
        }

        /// <summary>
        /// 按GUID获取记录
        /// </summary>
        /// <param name="outRecord"></param>
        /// <param name="guid"></param>
        public void GetRecordData(out Record outRecord, string guid)
        {
            if(!m_recordData.ContainsKey(guid))
            {
                outRecord = Record.Empty;
                return;
            }

            m_recordData.TryGetValue(guid, out outRecord);
        }

        /// <summary>
        /// 按日期获取记录数据
        /// </summary>
        /// <param name="outList"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void GetRecordData(out List<Record> outList, DateTime begin, DateTime end)
        {
            outList = new List<Record>();

            foreach(var pr in m_recordData)
            {
                var record = pr.Value;
                if(record.dateTime >= begin && record.dateTime <= end)
                    outList.Add(record);
            }

            // 按时间排序
            outList.Sort(TimeUtil.SortDateTimeLt);
        }
    }
}