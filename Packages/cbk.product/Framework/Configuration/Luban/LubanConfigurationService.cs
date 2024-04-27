using System;
using System.Collections.Generic;
using Bright.Serialization;
using CatLib;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using CBK.Framework.Localization;
using CBK.Framework.Reflect;
using CBK.Framework.Res;

namespace CBK.Framework.Configuration.Luban
{
    /// <summary>
    /// Luban配置服务实现
    /// </summary>
    internal sealed class LubanConfigurationService : IInitialize, IDisposable, IConfigurationService
    {
        public ConfigurationStatus Status { get; private set; }

        [Inject]
        public IReflectService ReflectService { get; set; }

        [Inject]
        public IApplication Container { get; set; }

        [Inject]
        public IResService ResService { get; set; }
        
        [Inject]
        public ITextLocalization TextLocalization { get; set; }

        public void Initialize()
        {
            Status = ConfigurationStatus.Unload;

            ReflectService.ForEachType<ILubanConfiguration>(type =>
            {
                if (type.IsAbstract)
                    return;

                var instance = (ILubanConfiguration)Activator.CreateInstance(type);
                _configurations.Add(type, instance);
                _lubanMapping.Add(instance.FullName, instance);

                var name = Container.Type2Service(type);
                // bind ioc
                Container.Instance(name, instance);
            });
        }

        public void Dispose()
        {
            Clean();

            foreach (var type in _configurations.Keys)
                Container.Unbind(Container.Type2Service(type));

            _configurations.Clear();
            _lubanMapping.Clear();
        }

        public async UniTask<int> LoadAsync()
        {
            if (Status == ConfigurationStatus.Loaded)
                return 0;

            if (Status == ConfigurationStatus.Loading)
            {
                await UniTask.WaitWhile(() => Status == ConfigurationStatus.Loading);
                return Status == ConfigurationStatus.Loaded ? 0 : -1;
            }

            Status = ConfigurationStatus.Loading;

            try
            {
                _buffer.Clear();
                foreach (var configuration in _configurations.Values)
                    _buffer.Add(LoadConfiguration(configuration));

                await UniTask.WhenAll(_buffer);
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Status = ConfigurationStatus.Failed;
                return -1;
            }

            if (Status != ConfigurationStatus.Loading)
                return -1;
            
            foreach (var configuration in _configurations.Values)
                configuration.TranslateText((key, _) => TextLocalization.GetText(key));

            foreach (var configuration in _configurations.Values)
                configuration.Resolve(_lubanMapping);

            Status = ConfigurationStatus.Loaded;
            return 0;
        }

        public T Get<T>() where T : class, IConfiguration
        {
            var type = typeof(T);
            if (!_configurations.TryGetValue(type, out var configuration))
                return null;

            return (T)configuration;
        }

        public void Clean()
        {
            Status = ConfigurationStatus.Unload;
        }

        private async UniTask LoadConfiguration(ILubanConfiguration configuration)
        {
            var resKey = ResolveResKey(configuration.AssetName);
            var bytes = await ResService.LoadRawAsync(resKey);
            configuration.LoadFromBytes(new ByteBuf(bytes));
        }

        private string ResolveResKey(string assetName)
        {
            return $"Configuration_{assetName}";
        }

        private readonly Dictionary<Type, ILubanConfiguration> _configurations = new();
        private readonly Dictionary<string, object> _lubanMapping = new();
        private readonly List<UniTask> _buffer = new();
    }
}