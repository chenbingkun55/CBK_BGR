using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputDateComp : MonoBehaviour
{
        [SerializeField] private TMP_Dropdown m_dropdownYear = default;
        [SerializeField] private TMP_Dropdown m_dropdownMonth = default;
        [SerializeField] private TMP_Dropdown m_dropdownDay = default;
        [SerializeField] private TMP_Dropdown m_dropdownHour = default;
        [SerializeField] private TMP_Dropdown m_dropdownMinute = default;

        // 选项
        private List<int> m_years = new List<int>() { 2020, 2021, 2022, 2023, 2024, 2025 };
        private List<int> m_months = new List<int>() { 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12 };
        private List<int> m_days = new List<int>() { 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        private List<int> m_hours = new List<int>() { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        private List<int> m_minutes = new List<int>() { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };

    private void Awake()
    {
        InitUi();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitUi()
    {
        // 年
        {
            m_dropdownYear.ClearOptions();
            foreach(var year in m_years)
                m_dropdownYear.options.Add(new TMP_Dropdown.OptionData(year.ToString("D4")));

            m_dropdownYear.value = m_years.IndexOf(int.Parse(DateTime.Now.ToString("yyyy")));
        }

        // 月
        {
            m_dropdownMonth.ClearOptions();
            foreach(var month in m_months)
                m_dropdownMonth.options.Add(new TMP_Dropdown.OptionData(month.ToString("D2")));

            m_dropdownMonth.value = m_months.IndexOf(int.Parse(DateTime.Now.ToString("MM")));
        }

        // 日
        {
            m_dropdownDay.ClearOptions();
            foreach(var day in m_days)
                m_dropdownDay.options.Add(new TMP_Dropdown.OptionData(day.ToString("D2")));

            m_dropdownDay.value = m_days.IndexOf(int.Parse(DateTime.Now.ToString("dd")));
        }

        // 时
        {
            m_dropdownHour.ClearOptions();
            foreach(var hour in m_hours)
                m_dropdownHour.options.Add(new TMP_Dropdown.OptionData(hour.ToString("D2")));

            m_dropdownHour.value = m_hours.IndexOf(int.Parse(DateTime.Now.ToString("HH")));
        }

        // 分
        {
            m_dropdownMinute.ClearOptions();
            foreach(var minute in m_minutes)
                m_dropdownMinute.options.Add(new TMP_Dropdown.OptionData(minute.ToString("D2")));

            m_dropdownMinute.value = m_minutes.IndexOf(int.Parse(DateTime.Now.ToString("mm")));
        }
    }

    public void ResetMin()
    {
        m_dropdownYear.value = 0;
        m_dropdownMonth.value = 0;
        m_dropdownDay.value = 0;
        m_dropdownHour.value = 0;
        m_dropdownMinute.value = 0;
    }

    public void ResetMax()
    {
        m_dropdownYear.value = m_dropdownYear.options.Count -1;
        m_dropdownMonth.value = m_dropdownMonth.options.Count -1;
        m_dropdownDay.value = m_dropdownDay.options.Count -1;
        m_dropdownHour.value = m_dropdownHour.options.Count -1;
        m_dropdownMinute.value = m_dropdownMinute.options.Count -1;
    }

    /// <summary>
    /// 设置今日开始
    /// </summary>
    public void SetTodayBegin()
    {
        var beginDate = DateTime.Today;

        SetDateTime(beginDate.ToString());
    }

    /// <summary>
    /// 设置今日结束
    /// </summary>
    public void SetTodayEnd()
    {
        var endDate = DateTime.Today.AddDays(1).AddSeconds(-1);

        SetDateTime(endDate.ToString());
    }

    /// <summary>
    /// 设置本周起始
    /// </summary>
    public void SetThisweekBegin()
    {
        int week = (int)DateTime.Today.DayOfWeek;
        if (week == 0) week = 7; //周⽇
        var beginDate = DateTime.Today.AddDays(-(week - 1));

        SetDateTime(beginDate.ToString());
    }

    /// <summary>
    /// 设置本周结束
    /// </summary>
    public void SetThisweekEnd()
    {
        int week = (int)DateTime.Today.DayOfWeek;
        if (week == 0) week = 7; //周⽇
        var beginDate = DateTime.Today.AddDays(-(week - 1));
        var endDate = beginDate.AddDays(6);

        SetDateTime(endDate.ToString());
    }

    /// <summary>
    /// 设置本月起始
    /// </summary>
    public void SetThismonthBegin()
    {
        var year = DateTime.Today.Year;
        var month = DateTime.Today.Month;

        var beginDate = new DateTime(year, month, 1);

        SetDateTime(beginDate.ToString());
    }

    /// <summary>
    /// 设置本月结束
    /// </summary>
    public void SetThismonthEnd()
    {
        var year = DateTime.Today.Year;
        var month = DateTime.Today.Month;

        var beginDate = new DateTime(year, month, 1);
        var endDate = beginDate.AddMonths(1).AddDays(-1);

        SetDateTime(endDate.ToString());
    }

    /// <summary>
    /// 设置日期&时间
    /// </summary>
    /// <param name="strDateTime"></param>
    public void SetDateTime(string strDateTime)
    {
        var dateTime = DateTime.Parse(strDateTime);

        m_dropdownYear.value = m_years.IndexOf(int.Parse(dateTime.ToString("yyyy")));
        m_dropdownMonth.value = m_months.IndexOf(int.Parse(dateTime.ToString("MM")));
        m_dropdownDay.value = m_days.IndexOf(int.Parse(dateTime.ToString("dd")));
        m_dropdownHour.value = m_hours.IndexOf(int.Parse(dateTime.ToString("HH")));
        m_dropdownMinute.value = m_minutes.IndexOf(int.Parse(dateTime.ToString("mm")));
    }

    /// <summary>
    /// 获取日期&时间
    /// </summary>
    /// <returns></returns>
    public DateTime GetDateTime()
    {
        var strDateTime =  string.Format("{0}-{1}-{2} {3}:{4}", m_years[m_dropdownYear.value].ToString(), m_months[m_dropdownMonth.value].ToString(), m_days[m_dropdownDay.value].ToString(), m_hours[m_dropdownHour.value].ToString(), m_hours[m_dropdownHour.value].ToString());

        return DateTime.Parse(strDateTime);
    }

    /// <summary>
    /// 获取日期
    /// </summary>
    /// <returns></returns>
    public DateTime GetDate()
    {
        var strDate = string.Format("{0}-{1}-{2}", m_years[m_dropdownYear.value].ToString(), m_months[m_dropdownMonth.value].ToString(), m_days[m_dropdownDay.value].ToString());

        return DateTime.Parse(strDate);
    }
}
