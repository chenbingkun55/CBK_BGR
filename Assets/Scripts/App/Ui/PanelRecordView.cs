using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CBK.Core;
using CBK.Product.Data;
using CBK.Product.Utils;

namespace CBK.Product.Ui
{
    public class PanelRecordView : MonoBehaviour
    {

        [SerializeField] private InputDateComp m_filterDateBegin = default;
        [SerializeField] private InputDateComp m_filterDateEnd = default;
        // 列表项
        [SerializeField] private Transform m_itemView = default;
        // 显示列表
        [SerializeField] private Transform m_listView = default;

        // Start is called before the first frame update
        void Start()
        {
            m_itemView.gameObject.SetActive(false);

            InitUi();
        }

        // 初始化
        private void InitUi()
        {
            FilterToday();
        }

        // 过滤今日
        private void FilterToday()
        {
            m_filterDateBegin.SetTodayBegin();
            m_filterDateEnd.SetTodayEnd();
            RefreshUi(m_filterDateBegin.GetDate(), m_filterDateEnd.GetDate());
        }

        // 过滤本周
        private void FilterThisweek()
        {
            m_filterDateBegin.SetThisweekBegin();
            m_filterDateEnd.SetThisweekEnd();
            RefreshUi(m_filterDateBegin.GetDate(), m_filterDateEnd.GetDate());
        }

        // 过滤本月
        private void FilterThismonth()
        {
            m_filterDateBegin.SetThismonthBegin();
            m_filterDateEnd.SetThismonthEnd();
            RefreshUi(m_filterDateBegin.GetDate(), m_filterDateEnd.GetDate());
        }

        // 过滤所有
        private void FilterAll()
        {
            m_filterDateBegin.ResetMin();
            m_filterDateEnd.ResetMax();
            RefreshUi(m_filterDateBegin.GetDate(), m_filterDateEnd.GetDate());
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void onBtnFilterClick()
        {
            RefreshUi(m_filterDateBegin.GetDate(), m_filterDateEnd.GetDate());
        }

        /// <summary>
        /// 今日
        /// </summary>
        public void onBtnTodayClick()
        {
            FilterToday();
        }

        /// <summary>
        /// 本周
        /// </summary>
        public void onBtnThisweekClick()
        {
            FilterThisweek();
        }

        /// <summary>
        /// 本月
        /// </summary>
        public void onBtnThismonthClick()
        {
            FilterThismonth();
        }

        /// <summary>
        /// 所有
        /// </summary>
        public void onBtnAllClick()
        {
            FilterAll();
        }

        public void RefreshUi(DateTime begin, DateTime end)
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            if(recordData == default)
                return;

            // 获取数据
            List<Record> filterRecord;
            if(begin == DateTime.MinValue && end == DateTime.MaxValue)
            {
                filterRecord = recordData.recordData;
            }
            else
            {
                recordData.GetRecordData(out filterRecord, begin, end.AddDays(1));
            }

            var count = Math.Max(filterRecord.Count, m_listView.childCount);
            for(var idx = 0; idx < count; ++idx)
            {
                // 准备UI
                RecordItem item = default;
                if(idx < m_listView.childCount)
                    item = m_listView.GetChild(idx).GetComponent<RecordItem>();
                else
                    item = GameObject.Instantiate(m_itemView, m_listView).GetComponent<RecordItem>();

                // 填充数据
                item.gameObject.SetActive(idx < filterRecord.Count);
                if(item.gameObject.activeSelf)
                {
                    var data = filterRecord[idx];
                    item.dateTime.text = data.dateTime.ToString("s");
                    item.eatType.text = TranslateUtil.GetText(data.eatType.ToString());
                    item.afterMealTime.text = TranslateUtil.GetText(data.afterMealTime.ToString());
                    item.medicineType.text = TranslateUtil.GetText(data.medicineType.ToString());
                    item.medicineAmount.text = string.Format("{0}{1}", data.medicineAmount, StringUtil.GetMedicineTypeUnit(data.medicineType));
                    item.sportType.text = TranslateUtil.GetText(data.sportType.ToString());
                    item.sportAmount.text = string.Format("{0}{1}", data.sportAmount, StringUtil.GetSportTypeUnit(data.sportType));
                    item.monitorValue.text = data.monitorValue.ToString();
                    item.notice.text = data.notice;
                }
            }
        }
    }
}
