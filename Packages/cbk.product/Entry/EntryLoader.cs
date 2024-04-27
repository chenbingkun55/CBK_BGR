using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HybridCLR;
using UnityEngine;
using YooAsset;

namespace CBK.Entry
{
    public class EntryLoader : MonoBehaviour, IEntryLoader
    {
        /// <summary>
        /// 加载起配置实例
        /// </summary>
        public EntryLoaderConfiguration configuration;

        /// <summary>
        /// 当前程序集版本号
        /// </summary>
        public string CurrentAssemblyVersion { get; private set; }

        /// <summary>
        /// 当前游戏逻辑程序集列表
        /// </summary>
        public IReadOnlyList<Assembly> CurrentAssembles => _currentAssembles;
        private readonly List<Assembly> _currentAssembles = new List<Assembly>();

        /// <summary>
        /// 加载游戏逻辑程序集
        /// </summary>
        public async UniTask LoadGameAssemblies()
        {
            var resourcePackage = await GetOrCreateResourcePackage();

            var operation = resourcePackage.LoadAssetSync<AssemblyManifest>(configuration.assemblyManifestAssetKey);
            var manifest = operation.GetAssetObject<AssemblyManifest>();
            if (manifest == null)
                throw new Exception($"加载程序集清单文件失败: {operation.LastError}");

            // 补充元数据
            LoadAOTPatchMetadata(resourcePackage, manifest);

            // 加载程序集
            LoadAssemblies(resourcePackage, manifest);

            var entryTypeName = manifest.logicEntryTypeName;
            var entryMethodName = manifest.logicEntryStaticMethodName;

            operation.Release();

            // 启动游戏逻辑程序集入口
            LaunchGameEntry(entryTypeName, entryMethodName);
        }

        #region [内部实现]

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            BetterStreamingAssets.Initialize();
            YooAssets.Initialize();

            if (configuration == null)
                throw new Exception("请配置加载器配置实例");

            LoadGameAssemblies().Forget();
        }

        private void OnDestroy()
        {
            YooAssets.Destroy();
        }

        /// <summary>
        /// 
        /// </summary>
        private async UniTask<ResourcePackage> GetOrCreateResourcePackage()
        {
            var resourcePackage = YooAssets.TryGetPackage(configuration.assetPackageName);
            if (resourcePackage != null)
                return resourcePackage;

            resourcePackage = YooAssets.CreatePackage(configuration.assetPackageName);
            YooAssets.SetDefaultPackage(resourcePackage);

            // ReSharper disable once JoinDeclarationAndInitializer
            InitializeParameters initParameters;
#if UNITY_EDITOR && !SIMULATE_RES
            initParameters = new EditorSimulateModeParameters
            {
                SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(configuration.assetPackageName)
            };
#else
                    initParameters = new HostPlayModeParameters
                    {
                        DefaultHostServer = configuration.mainHostServer,
                        FallbackHostServer = configuration.fallbackHostServer,
                        QueryServices = this,
                    };
                    if (!string.IsNullOrEmpty(configuration.decryptionServiceTypeName))
                    {
                        var type = Type.GetType(configuration.decryptionServiceTypeName);
                        if (type != null && typeof(IDecryptionServices).IsAssignableFrom(type))
                            initParameters.DecryptionServices = (IDecryptionServices)Activator.CreateInstance(type);
                    }
#endif
            var initOperation = resourcePackage.InitializeAsync(initParameters);
            await initOperation.ToUniTask();

            if (initOperation.Status != EOperationStatus.Succeed)
                throw new Exception($"初始化资源包失败: {initOperation.Error}");

            return resourcePackage;
        }

        /// <summary>
        /// 补充元数据
        /// </summary>
        private void LoadAOTPatchMetadata(ResourcePackage resourcePackage, AssemblyManifest manifest)
        {
            foreach (var assemblyName in manifest.aotPatchAssemblyNames)
            {
                var assetKey = manifest.GetAOTPatchAssemblyAssetKey(assemblyName);
                var rawOperation = resourcePackage.LoadRawFileSync(assetKey);

                if (rawOperation.Status != EOperationStatus.Succeed)
                    throw new Exception($"初始化游戏失败: {rawOperation.LastError}");

                var bytes = rawOperation.GetRawFileData();
                var err = RuntimeApi.LoadMetadataForAOTAssembly(bytes, HomologousImageMode.SuperSet);
                if (err != LoadImageErrorCode.OK)
                    throw new Exception($"初始化游戏失败: {err}");

                rawOperation.Release();
            }
        }

        /// <summary>
        /// 加载游戏程序集
        /// </summary>
        private void LoadAssemblies(ResourcePackage resourcePackage, AssemblyManifest manifest)
        {
            // 版本未发生变化时 无需加载程序集
            if (string.Equals(CurrentAssemblyVersion, manifest.version))
                return;

            CurrentAssemblyVersion = manifest.version;
            _currentAssembles.Clear();

#if UNITY_EDITOR
            _currentAssembles.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(assembly => manifest.hotfixAssemblyNames.Contains(assembly.GetName().Name)));
#else
            foreach (var assemblyName in manifest.gameAssemblyNames)
            {
                var assetKey = manifest.GetGameAssemblyAssetKey(assemblyName);
                var rawOperation = resourcePackage.LoadRawFileSync(assetKey);
                if (rawOperation.Status != EOperationStatus.Succeed)
                    throw new Exception($"初始化游戏失败: {rawOperation.LastError}");
                var bytes = rawOperation.GetRawFileData();
                _currentAssembles.Add(Assembly.Load(bytes));
                rawOperation.Release();
            }
#endif

        }

        /// <summary>
        /// 启动游戏逻辑程序集入口
        /// </summary>
        private void LaunchGameEntry(string entryTypeName, string entryMethodName)
        {
            Type gameEntryType = null;
            foreach (var assembly in CurrentAssembles)
            {
                gameEntryType = assembly.GetType(entryTypeName);
                if (gameEntryType != null)
                    break;
            }

            if (gameEntryType == null)
                throw new Exception($"初始化游戏失败: 找不到游戏逻辑入口类:{entryTypeName}");

            var method =
                gameEntryType.GetMethod(entryMethodName, BindingFlags.Public | BindingFlags.Static);

            if (method == null)
                throw new Exception($"初始化游戏失败: 找不到游戏逻辑入口静态函数:{entryMethodName}");

            method.Invoke(null, new object[] { this });
        }

        public bool QueryStreamingAssets(string fileName)
        {
            var folderName = YooAssets.GetStreamingAssetBuildinFolderName();
            return BetterStreamingAssets.FileExists($"{folderName}/{fileName}");
        }

        #endregion
    }
}