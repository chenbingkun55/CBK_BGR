using System;
using System.Collections.Generic;
using UnityEngine;

namespace CBK.Product.Save
{
    [Serializable]
    public class Save
    {
        public DataTitleSerializable dataTitleSer = new DataTitleSerializable();
        public List<RecordDataSerializable> listRecordDataSer = new List<RecordDataSerializable>();

        public void ClearRecordData()
        {
            listRecordDataSer.Clear();
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadFromJson(string strJson)
        {
            JsonUtility.FromJsonOverwrite(strJson, this);
        }
    }
}