using System;
using CBK.Product.Data;

namespace CBK.Product.Utils
{
    public class TimeUtil
    {

        /// <summary>
        /// 日期排序(从小到大)
        /// </summary>
        /// <param name="dataA"></param>
        /// <param name="dataB"></param>
        /// <returns></returns>
        public static int SortDateTimeLt(Record dataA, Record dataB)
        {
            if(dataA.Equals(dataB.dateTime))
            return 0;
                    
            return (dataA.dateTime < dataB.dateTime) ? -1 : 1;
        }

    }
}