using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CBK.Product;
using CBK.Product.Utils;

[RequireComponent(typeof(TMP_Dropdown))]
public class InputSportComp : MonoBehaviour
{
    [SerializeField] private Text m_textUnit = default;
    private TMP_Dropdown m_dropdown = default;

    private Dictionary<SportType, string> m_dicSportUnit = new Dictionary<SportType, string>();

    // Start is called before the first frame update
    void Start()
    {
        m_dropdown = GetComponent<TMP_Dropdown>();

        InitUi();
    }

    private void InitUi()
    {
        // 运动类型
        {
            m_dropdown.ClearOptions();

            for(var iType = (int)SportType.None; iType < (int)SportType.Max; ++iType)
            {
                Debug.Log(((SportType)iType).ToString());
                m_dropdown.options.Add(new TMP_Dropdown.OptionData(TranslateUtil.GetText(((SportType)iType).ToString())));
            }

            // 默认选择
            OnSportSelected((int)SportType.Walk);
            m_dropdown.SetValueWithoutNotify((int)SportType.Walk);
            m_dropdown.onValueChanged.AddListener(OnSportSelected);
        }
    }

    /// <summary>
    /// 选择运动类型
    /// </summary>
    /// <param name="idx"></param>
    private void OnSportSelected(int eType)
    {
        m_textUnit.text = StringUtil.GetSportTypeUnit((SportType)eType);
    }

    /// <summary>
    /// 获取运动类型
    /// </summary>
    /// <returns></returns>
    public SportType GetSportType()
    {
        return (SportType)m_dropdown.value;
    }
}
