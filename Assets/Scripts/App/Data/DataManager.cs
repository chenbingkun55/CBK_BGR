using System;
using System.Collections.Generic;

namespace CBK.Product.Data
{
    public class DataManager
    {
        private Dictionary<DataType, IData> m_datas = new Dictionary<DataType, IData>();

        public DataManager()
        {
            // New 数据
            m_datas.Add(DataType.RecordData, new RecordData());
        }

        public bool Initialize()
        {
            // 初始化数据
            foreach(var data in m_datas)
                data.Value.Initialize();

            return true;
        }

        public void Destroy()
        {
            // 销毁数据
            foreach(var data in m_datas)
                data.Value.Destroy();
                
            m_datas.Clear();
        }

        /// <summary>
        /// 数据访问索引器
        /// </summary>
        /// <value></value>
        public IData this[DataType eType]
        {
            get => m_datas[eType];
        }
    }
}