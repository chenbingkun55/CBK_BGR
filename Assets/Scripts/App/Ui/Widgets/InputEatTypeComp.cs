using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CBK.Product;
using CBK.Product.Utils;

[RequireComponent(typeof(TMP_Dropdown))]
public class InputEatTypeComp : MonoBehaviour
{
    private TMP_Dropdown m_dropdown = default;

    // Start is called before the first frame update
    void Start()
    {
        m_dropdown = GetComponent<TMP_Dropdown>();

        InitUi();
    }

    private void InitUi()
    {
        // 用餐类型
        {
            m_dropdown.ClearOptions();

            for(var iType = (int)EatType.None; iType < (int)EatType.Max; ++iType)
            {
                Debug.Log(((EatType)iType).ToString());
                m_dropdown.options.Add(new TMP_Dropdown.OptionData(TranslateUtil.GetText(((EatType)iType).ToString())));
            }

            // 默认选择
            m_dropdown.SetValueWithoutNotify((int)EatType.Breakfast);
        }
    }

    /// <summary>
    /// 获取用餐类型
    /// </summary>
    /// <returns></returns>
    public EatType GetEatType()
    {
        return (EatType)m_dropdown.value;
    }
}
