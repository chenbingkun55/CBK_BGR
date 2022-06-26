using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CBK.Product;
using CBK.Product.Utils;

[RequireComponent(typeof(TMP_Dropdown))]
public class InputEatTypeComp : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown m_dropdown = default;
    [SerializeField] private InputAfterMealTimeComp m_inputAfterMealTime = default;

    private void Awake()
    {
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
            m_dropdown.onValueChanged.AddListener(OnSelectionChanged);
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

    /// <summary>
    /// 设置选中
    /// </summary>
    /// <param name="idx"></param>
    public void SetSelected(int idx)
    {
         m_dropdown.value = idx;
    }

    private void OnSelectionChanged(int index)
    {
        switch ((EatType)index)
        {
            case EatType.None:
            case EatType.Bedtime:
            case EatType.Limosis:
            case EatType.WeeHours:
                m_inputAfterMealTime.SetSelected((int)AfterMealTime.None);
                break;
            default:
                m_inputAfterMealTime.SetSelected((int)AfterMealTime.BeforeMeal);
            break;
        }
    }
}
