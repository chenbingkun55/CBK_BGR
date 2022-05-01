using UnityEngine;

namespace CBK.Product
{
    public class AppInit : MonoBehaviour 
    {
        private App m_app = new App();

        private void Start()
        {
            // 初始化App
            m_app.Initialize();

            // 加载数据
            m_app.save.LoadSaveRecordData();
        }

        private void OnDestroy()
        {
            // 销毁
            m_app.Destroy();
        }
    }
}