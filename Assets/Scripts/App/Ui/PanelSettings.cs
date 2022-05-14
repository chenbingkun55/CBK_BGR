using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBK.Product.Ui
{
    public class PanelSettings : MonoBehaviour
    {
        public void Open()
        {
            this.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.gameObject.SetActive(false);
        }

        // 清空数据
        public void onBtnClearDataClick()
        {
            App.Instantiate.save.ClearData();
        }
    }
}
