using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CBK.Product;
using CBK.Product.Utils;

[RequireComponent(typeof(TMP_Dropdown))]
public class InputMedicineComp : MonoBehaviour
{
    [SerializeField] private Text m_textUnit = default;
    [SerializeField] private TMP_Dropdown m_dropdown = default;

    private Dictionary<MedicineType, string> m_dicMedicineUnit = new Dictionary<MedicineType, string>();


    private void Awake()
    {
        InitUi();
    }

    private void InitUi()
    {
        // 运动类型
        {
            m_dropdown.ClearOptions();

            for(var iType = (int)MedicineType.None; iType < (int)MedicineType.Max; ++iType)
            {
                Debug.Log(((MedicineType)iType).ToString());
                m_dropdown.options.Add(new TMP_Dropdown.OptionData(TranslateUtil.GetText(((MedicineType)iType).ToString())));
            }

            // 默认选择
            OnMedicineSelected((int)MedicineType.Metformin);
            m_dropdown.SetValueWithoutNotify((int)MedicineType.Metformin);
            m_dropdown.onValueChanged.AddListener(OnMedicineSelected);
        }
    }

    /// <summary>
    /// 设置选中
    /// </summary>
    /// <param name="idx"></param>
    public void SetSelected(int idx)
    {
         m_dropdown.value = idx;
    }

    /// <summary>
    /// 获取药品类型
    /// </summary>
    /// <returns></returns>
    public MedicineType GetMedicineType()
    {
        return (MedicineType)m_dropdown.value;
    }

    /// <summary>
    /// 选择药品类型
    /// </summary>
    /// <param name="idx"></param>
    private void OnMedicineSelected(int eType)
    {
        m_textUnit.text = StringUtil.GetMedicineTypeUnit((MedicineType)eType);
    }
}
