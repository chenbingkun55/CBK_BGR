using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CBK.Core;
using CBK.Product.Data;
using CBK.Product.Utils;

namespace CBK.Product.Ui
{
    public class PanelRecord : MonoBehaviour
    {
        //
        [SerializeField] private InputDateComp m_inputDate = default;
        [SerializeField] private InputEatTypeComp m_inputEatType = default;
        [SerializeField] private InputAfterMealTimeComp m_inputAfterMealTime = default;
        [SerializeField] private InputMedicineComp m_inputMedicine = default;
        [SerializeField] private TMP_InputField m_inputMedicineAmount = default;
        [SerializeField] private InputSportComp m_inputSport = default;
        [SerializeField] private TMP_InputField m_inputSportAmount = default;
        [SerializeField] private TMP_InputField m_inputMonitorValue = default;
        [SerializeField] private TMP_InputField m_inputNotice = default;
        [SerializeField] private TMP_Text m_textGUID = default;
        [SerializeField] private Text m_textBtnUpdate = default;

        // 属性
        private RecordData m_recordData = default;

        // Start is called before the first frame update
        void Start()
        {
            InitUi();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnEnable()
        {
            if(m_recordData == default)
                return;

            // 刷新UI
            m_recordData.GetWillAlterRecordData(out var alterRecordData);
            RefreshUi(alterRecordData);
        }

        public void InitUi()
        {
            m_recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            
            // 刷新UI
            m_recordData.GetWillAlterRecordData(out var alterRecordData);
            RefreshUi(alterRecordData);
        }

        private void RefreshUi(Record alterRecord = default)
        {
            var hasAlter = !Record.Empty.Equals(alterRecord);
            if(hasAlter)
            {
                m_textGUID.text = alterRecord.GUID;
                m_inputDate.SetDateTime(alterRecord.dateTime.ToString());
                m_inputEatType.SetSelected((int)alterRecord.eatType);
                m_inputAfterMealTime.SetSelected((int)alterRecord.afterMealTime);
                m_inputMedicine.SetSelected((int)alterRecord.medicineType);
                m_inputMedicineAmount.text = alterRecord.medicineAmount.ToString();
                m_inputSport.SetSelected((int)alterRecord.sportType);
                m_inputSportAmount.text = alterRecord.sportAmount.ToString();
                m_inputMonitorValue.text = alterRecord.monitorValue.ToString();
                m_inputNotice.text = alterRecord.notice;
                m_textBtnUpdate.text = TranslateUtil.GetText("Alter");
            }
            else
            {
                m_textGUID.text = Guid.NewGuid().ToString();
                m_inputDate.SetDateTime(DateTime.Now.ToString());
                m_inputEatType.SetSelected((int)EatType.Breakfast);
                m_inputAfterMealTime.SetSelected((int)AfterMealTime.BeforeMeal);
                m_inputMedicine.SetSelected((int)MedicineType.None);
                m_inputMedicineAmount.text = string.Empty;
                m_inputSport.SetSelected((int)SportType.None);
                m_inputSportAmount.text = string.Empty;
                m_inputMonitorValue.text = string.Empty;
                m_inputNotice.text = string.Empty;
                m_textBtnUpdate.text = TranslateUtil.GetText("Add");
            }
        }

        public void OnAddRecordClick()
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            var record = new Record();
            
            record.GUID = m_textGUID.text;
            record.dateTime = m_inputDate.GetDateTime();
            record.afterMealTime = m_inputAfterMealTime.GetAfterMealTime();
            record.eatType = m_inputEatType.GetEatType();
            record.medicineType = m_inputMedicine.GetMedicineType();
            record.medicineAmount = string.IsNullOrEmpty(m_inputMedicineAmount.text) ? 0 : int.Parse(m_inputMedicineAmount.text);
            record.sportType = m_inputSport.GetSportType();
            record.sportAmount = string.IsNullOrEmpty(m_inputSportAmount.text) ? 0 : int.Parse(m_inputSportAmount.text);
            record.monitorValue = string.IsNullOrEmpty(m_inputMonitorValue.text) ? 0 : float.Parse(m_inputMonitorValue.text);
            record.notice = m_inputNotice.text;

            // 更新或添加记录
            recordData.UpdateRecordData(record);

            // 保存到Disk
            App.Instantiate.save.SaveDataToDisk();
        }
    }
}
