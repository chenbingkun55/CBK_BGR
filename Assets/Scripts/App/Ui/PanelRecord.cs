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


        // Start is called before the first frame update
        void Start()
        {
            InitUi();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void InitUi()
        {

        }

        public void OnAddRecordClick()
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            var record = new Record();
            
            record.dateTime = m_inputDate.GetDateTime();
            record.afterMealTime = m_inputAfterMealTime.GetAfterMealTime();
            record.eatType = m_inputEatType.GetEatType();
            record.medicineType = m_inputMedicine.GetMedicineType();
            record.medicineAmount = string.IsNullOrEmpty(m_inputMedicineAmount.text) ? 0 : int.Parse(m_inputMedicineAmount.text);
            record.sportType = m_inputSport.GetSportType();
            record.sportAmount = string.IsNullOrEmpty(m_inputSportAmount.text) ? 0 : int.Parse(m_inputSportAmount.text);
            record.monitorValue = float.Parse(m_inputMonitorValue.text);
            record.notice = m_inputNotice.text;

            recordData.recordData.Add(record);

            // 保存到Disk
            App.Instantiate.save.SaveDataToDisk();
        }
    }
}
