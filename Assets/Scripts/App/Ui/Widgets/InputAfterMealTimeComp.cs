using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CBK.Product;
using CBK.Product.Utils;

public class InputAfterMealTimeComp : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown m_dropdown = default;


    private void Awake()
    {
        InitUi();
    }

    private void InitUi()
    {
        // 用餐类型
        {
            m_dropdown.ClearOptions();

            for(var iType = (int)AfterMealTime.None; iType < (int)AfterMealTime.Max; ++iType)
            {
                Debug.Log(((AfterMealTime)iType).ToString());
                m_dropdown.options.Add(new TMP_Dropdown.OptionData(TranslateUtil.GetText(((AfterMealTime)iType).ToString())));
            }

            // 默认选择
            // m_dropdown.SetValueWithoutNotify((int)AfterMealTime.BeforeMeal);
        }
    }

    /// <summary>
    /// 获取餐后时间
    /// </summary>
    /// <returns></returns>
    public AfterMealTime GetAfterMealTime()
    {
        return (AfterMealTime)m_dropdown.value;
    }

    /// <summary>
    /// 设置选中
    /// </summary>
    /// <param name="idx"></param>
    public void SetSelected(int idx)
    {
         m_dropdown.value = idx;
    }
}
