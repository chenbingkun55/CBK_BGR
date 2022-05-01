using System;

namespace CBK.Product.Save
{
    [Serializable]
    public class RecordDataSerializable
    {
        // 记录时间
        public string strDateTime;
        // 用餐类型
        public int eatType;
        // 用餐时间
        public int afterMealTime;
        // 药品名称
        public string medicineName;
        // 剂量
        public int medicineAmount;
        // 备注
        public string notice; 
    }
}