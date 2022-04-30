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
        }

        private void OnDestroy()
        {
            // 销毁
            m_app.Destroy();
        }
    }
}