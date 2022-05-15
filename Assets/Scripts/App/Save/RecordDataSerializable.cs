using System;

namespace CBK.Product.Save
{
    [Serializable]
    public class RecordDataSerializable
    {
        // GUID
        public string GUID;
        // 记录时间
        public string strDateTime;
        // 用餐类型
        public int eatType;
        // 用餐时间
        public int afterMealTime;
        // 药品名称
        public int medicineType;
        // 剂量
        public int medicineAmount;
        // 运动类型
        public int sportType;
        // 运动量
        public int sportAmount;
        // 监测血糖值
        public float monitorValue;
        // 备注
        public string notice; 
    }
}