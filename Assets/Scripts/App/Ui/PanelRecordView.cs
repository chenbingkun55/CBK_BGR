using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CBK.Core;
using CBK.Product.Data;

namespace CBK.Product.Ui
{
    public class PanelRecordView : MonoBehaviour
    {

        [SerializeField] private InputField m_searchText = default;
        // 列表项
        [SerializeField] private Transform m_itemView = default;
        // 显示列表
        [SerializeField] private Transform m_listView = default;

        private RecordData m_recordData = default;

        // Start is called before the first frame update
        void Start()
        {
            m_searchText.gameObject.SetActive(true);
            m_itemView.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            if(m_recordData == default)
                return;

            var count = Math.Max(m_recordData.recordData.Count, m_listView.childCount);
            for(var idx = 0; idx < count; ++idx)
            {
                // 准备UI
                RecordItem item = default;
                if(idx < m_listView.childCount)
                    item = m_listView.GetChild(idx).GetComponent<RecordItem>();
                else
                    item = GameObject.Instantiate(m_itemView, m_listView).GetComponent<RecordItem>();

                // 填充数据
                item.gameObject.SetActive(idx < m_recordData.recordData.Count);
                if(item.gameObject.activeSelf)
                {
                    var data = m_recordData.recordData[idx];
                    item.dateTime.text = data.dateTime.ToString("s");
                    item.eatType.text = data.eatType.ToString();
                    item.afterMealTime.text = data.afterMealTime.ToString();
                    item.medicineName.text = data.medicineName;
                    item.medicineAmount.text = data.medicineAmount.ToString();
                    item.notice.text = data.notice;
                }
            }
        }
    }
}
