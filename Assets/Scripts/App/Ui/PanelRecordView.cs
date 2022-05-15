using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
        // 修改记录Tab
        [SerializeField] private Toggle m_togglePanelRecord = default;


        // 属性
        private float m_lastTime = default;

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

        public void RefreshUi(DateTime begin, DateTime end)
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;

            // 获取数据
            recordData.GetRecordData(out var filterRecord, begin, end);

            // 显示数据
            var count = Math.Max(filterRecord.Count, m_listView.childCount);
            for(var idx = 0; idx < count; ++idx)
            {
                // 准备UI
                RecordItem item = default;
                if(idx < m_listView.childCount)
                    item = m_listView.GetChild(idx).GetComponent<RecordItem>();
                else
                    item = GameObject.Instantiate(m_itemView, m_listView).GetComponent<RecordItem>();

                // 注册点击
                var btnItem = item.GetComponent<Button>();
                btnItem.onClick.RemoveAllListeners();
                btnItem.onClick.AddListener(() => OnRecordItemDoubleClick(item));

                // 填充数据
                item.gameObject.SetActive(idx < filterRecord.Count);
                if(item.gameObject.activeSelf)
                {
                    var data = filterRecord[idx];

                    item.GUID = data.GUID;
                    item.dateTime.text = data.dateTime.ToString("yyyy-MM-dd HH:mm");
                    item.eatType.text = TranslateUtil.GetText(data.eatType.ToString());
                    item.afterMealTime.text = TranslateUtil.GetText(data.afterMealTime.ToString());
                    item.medicineType.text = TranslateUtil.GetText(data.medicineType.ToString());
                    item.medicineAmount.text = string.Format("{0} {1}", data.medicineAmount, StringUtil.GetMedicineTypeUnit(data.medicineType));
                    item.sportType.text = TranslateUtil.GetText(data.sportType.ToString());
                    item.sportAmount.text = string.Format("{0} {1}", data.sportAmount, StringUtil.GetSportTypeUnit(data.sportType));
                    item.monitorValue.text = string.Format("{0} {1}", data.monitorValue.ToString(), TranslateUtil.GetText("mmol/L"));
                    item.notice.text = data.notice;
                }
            }
        }

        // 过滤今日
        private void FilterToday()
        {
            m_filterDateBegin.SetTodayBegin();
            m_filterDateEnd.SetTodayEnd();
            RefreshUi(m_filterDateBegin.GetDateTime(), m_filterDateEnd.GetDateTime());
        }

        // 过滤本周
        private void FilterThisweek()
        {
            m_filterDateBegin.SetThisweekBegin();
            m_filterDateEnd.SetThisweekEnd();
            RefreshUi(m_filterDateBegin.GetDateTime(), m_filterDateEnd.GetDateTime());
        }

        // 过滤本月
        private void FilterThismonth()
        {
            m_filterDateBegin.SetThismonthBegin();
            m_filterDateEnd.SetThismonthEnd();
            RefreshUi(m_filterDateBegin.GetDateTime(), m_filterDateEnd.GetDateTime());
        }

        // 过滤所有
        private void FilterAll()
        {
            m_filterDateBegin.ResetMin();
            m_filterDateEnd.ResetMax();
            RefreshUi(m_filterDateBegin.GetDateTime(), m_filterDateEnd.GetDateTime());
        }

        /// <summary>
        /// 搜索
        /// </summary>
        public void OnBtnFilterClick()
        {
            RefreshUi(m_filterDateBegin.GetDateTime(), m_filterDateEnd.GetDateTime());
        }

        /// <summary>
        /// 今日
        /// </summary>
        public void OnBtnTodayClick()
        {
            FilterToday();
        }

        /// <summary>
        /// 本周
        /// </summary>
        public void OnBtnThisweekClick()
        {
            FilterThisweek();
        }

        /// <summary>
        /// 本月
        /// </summary>
        public void OnBtnThismonthClick()
        {
            FilterThismonth();
        }

        /// <summary>
        /// 所有
        /// </summary>
        public void OnBtnAllClick()
        {
            FilterAll();
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        public void ToAlterRecordItem(string GUID)
        {
            Debug.Log("Alter " + GUID);

            // 设置修改GUID
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            recordData.alterGUID = GUID;

            m_togglePanelRecord.isOn = true;
        }

        /// <summary>
        /// 记录双击修改
        /// </summary>
        public void OnRecordItemDoubleClick(RecordItem recordItem)
        {
            if(Time.realtimeSinceStartup - m_lastTime < 0.3f)
            {
                ToAlterRecordItem(recordItem.GUID);
            }

            m_lastTime = Time.realtimeSinceStartup;
        }
    }
}
