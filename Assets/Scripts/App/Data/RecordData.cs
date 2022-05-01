using System;
using System.Collections.Generic;
using CBK.Product;

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

        // 记录时间
        public DateTime dateTime;
        // 用餐类型
        public EatType eatType;
        // 用餐时间
        public int afterMealTime;
        // 药品名称
        public string medicineName;
        // 剂量
        public int medicineAmount;
        // 备注
        public string notice;
    }


    // 记录数据
    public class RecordData : IData
    {
        private DataTitle m_dataTitle = new DataTitle();
        private List<Record> m_recordData = new List<Record>();

        public DataTitle dataTitle => m_dataTitle;
        public List<Record> recordData => m_recordData;

        public bool Initialize()
        {
            return true;
        }

        public void Destroy()
        {

        }
    }
}