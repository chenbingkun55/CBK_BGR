using System.IO;
using System.Collections;
using UnityEngine;
using CBK.Core;

namespace CBK.Product
{
    public class AppInit : MonoBehaviour 
    {
        private void Start()
        {
            // 打印初始化信息
            PrintAppInfo();
            // 初始化App
            App.Instantiate.Initialize();
            // // 初始化App Async
            // StartCoroutine("InitializeAsync");

            // 加载数据
            App.Instantiate.save.LoadSaveRecordData();
        }

        IEnumerable InitializeAsync()
        {
            yield return App.Instantiate.InitializeAsync();
        }

        private void OnDestroy()
        {
            // 销毁
            App.Instantiate.Destroy();
        }

        private void PrintAppInfo()
        {
            Debug.Log("===== CBK_BGR App Start Init.");
            Debug.Log("===== App Data Version: " + AppConst.kRecordDataVersion);
            Debug.Log("===== App Data Path: " + Path.Combine(Core.Config.kDataPath, AppConst.kSaveFileName));
        }
    }
}