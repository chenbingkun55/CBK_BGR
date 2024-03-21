using HybridCLR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CBK.Mono
{
  public class MainMono : MonoBehaviour
  {
    void Start()
    {
      // 初始化资源加载
      Addressables.InitializeAsync();

      // 先补充元数据
      LoadMetadataForAOTAssemblies();

      // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
    var config = Addressables.LoadAssetAsync<HotfixDllConfig>("ScriptableObject/HotfixDllConfig.asset").WaitForCompletion();
    foreach (var dll in config.hotfixDlls)
    {
      var hotfixAsset = Addressables.LoadAssetAsync<TextAsset>($"HotfixDll/{dll}.bytes").WaitForCompletion();
      if (hotfixAsset == null)
      {
        Debug.LogError($"MainEntry.Start hotfixAsset load failure. path: HotfixDll/{dll}.bytes");
        return;
      }
      
      Assembly.Load(hotfixAsset.bytes);
    }
#endif

      // Type type = hotfixUiAss.GetType("Hello");
      // type.GetMethod("Run").Invoke(null, null);
      // 

      var mainHotfixPrefab = Addressables.LoadAssetAsync<GameObject>("MainHotfix").WaitForCompletion(); // 获得HotUpdatePrefab.prefab所在的AssetBundle
      if (mainHotfixPrefab == null)
      {
        Debug.LogError($"MainEntry.Start mainHotfixPrefab load failure.");
        return;
      }

      // 起动Hotfix
      var mainHotfix = Instantiate(mainHotfixPrefab);
      mainHotfix.transform.SetSiblingIndex(1);
      mainHotfix.name = "MainHotfix";

      // 卸载MainMono
      GameObject.Destroy(this.gameObject);
    }

    private static void LoadMetadataForAOTAssemblies()
    {
      var listMetadata = new List<string>
      {
        // "mscorlib.dll",
        // "System.dll",
        // "System.Core.dll", // 如果使用了Linq，需要这个
        // "Newtonsoft.Json.dll", 
        // "protobuf-net.dll",
      };

      foreach (var metadata in listMetadata)
      {
        var dataAsset = Addressables.LoadAssetAsync<TextAsset>($"{metadata}.bytes").WaitForCompletion();

        int err = (int)HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(dataAsset.bytes, HomologousImageMode.SuperSet);
        Debug.Log($"LoadMetadataForAOTAssembly: {metadata}. ret:{err}");
      }
    }
  }
}
