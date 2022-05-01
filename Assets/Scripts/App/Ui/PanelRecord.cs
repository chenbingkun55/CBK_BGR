using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CBK.Core;
using CBK.Product.Data;

namespace CBK.Product.Ui
{
    public class PanelRecord : MonoBehaviour
    {
        //
        [SerializeField] private InputField m_inputDate = default;
        [SerializeField] private InputField m_inputEatType = default;
        [SerializeField] private InputField m_inputAfterMealTime = default;
        [SerializeField] private InputField m_inputMedicineName = default;
        [SerializeField] private InputField m_inputMedicineAmount = default;
        [SerializeField] private InputField m_inputNotice = default;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OnAddRecordClick()
        {
            var recordData = App.Instantiate.data[DataType.RecordData] as RecordData;
            var record = new Record();
            
            record.dateTime = DateTime.Parse(m_inputDate.text);
            record.afterMealTime = int.Parse(m_inputAfterMealTime.text);
            record.eatType = (EatType)(int.Parse(m_inputEatType.text));
            record.medicineName = m_inputMedicineName.text;
            record.medicineAmount = int.Parse(m_inputMedicineAmount.text);
            record.notice = m_inputNotice.text;

            recordData.recordData.Add(record);

            // 保存到Disk
            App.Instantiate.save.SaveDataToDisk();
        }
    }
}
