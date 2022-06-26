namespace CBK.Product
{
    // 用餐类型
    public enum EatType
    {
        None = 0,
        Breakfast, // 早餐
        Lunch, // 午餐
        Dinner, // 晚餐
        Snack, // 加餐
        Bedtime, // 睡前
        WeeHours, // 凌晨
        Limosis, // 空腹
        Random, // 随机
        Max,
    }

    // 餐后
    public enum AfterMealTime
    {
        None = 0,
        BeforeMeal, // 餐前
        Hour_1, // 1小时
        Hour_2, // 2小时
        Hour_3, // 3小时
        AfterMeal, // 餐后
        Max,
    }

    // 运动类型
    public enum SportType
    {
        None = 0,
        Walk,   // 走路
        Running, // 跑步
        RideBike, // 骑单车
        Dumbbell, // 哑铃
        Aerobics, // 健身操
        Max
    }

    // 药品
    public enum MedicineType
    {
        None,
        Metformin, // 二甲双胍
        Insulin, // 胰岛素
        Max
    }

    // 配置
    public enum ConfigType
    {
        None = 0,
        Language, // 多语言
    }
    // 数据
    public enum DataType
    {
        None = 0,
        RecordData, // 记录数据
    }
}