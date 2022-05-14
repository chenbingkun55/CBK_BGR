using System;
using CBK.Product;
using CBK.Product.Data;

namespace CBK.Product.Utils
{
    public class StringUtil
    {
        // 运动单位
        public static string GetSportTypeUnit(SportType eType)
        {
            string retStr = string.Empty;
            switch (eType)
            {
                case SportType.Walk:
                case SportType.Running:
                case SportType.RideBike:
                    retStr = TranslateUtil.GetText("Kilometer");
                    break;
                case SportType.Dumbbell:
                    retStr = TranslateUtil.GetText("Minute");
                    break;
                default:
                    break;
            }

            return retStr;
        }

        // 药品单位
        public static string GetMedicineTypeUnit(MedicineType eType)
        {
            string retStr = string.Empty;
            switch (eType)
            {
                case MedicineType.Insulin:
                    retStr = TranslateUtil.GetText("ml");
                    break;
                case MedicineType.Metformin:
                    retStr = TranslateUtil.GetText("grain");
                    break;
                default:
                    break;
            }

            return retStr;
        }
    }
}