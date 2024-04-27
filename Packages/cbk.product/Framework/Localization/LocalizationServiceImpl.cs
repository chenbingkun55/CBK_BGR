using System;
using System.Collections.Generic;
using CatLib;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using CBK.Framework.Event;
using CBK.Framework.Extensions;
using CBK.Framework.Persistence;
using CBK.Framework.Reference;
using CBK.Framework.Reflect;
using CBK.Framework.Res;
using UnityEngine;
using Application = UnityEngine.Application;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化服务实现
    /// </summary>
    internal sealed partial class LocalizationServiceImpl : IInitialize, IDisposable, ILocalizationService
    {
        [Inject]
        public IReflectService ReflectService { get; set; }

        [Inject]
        public IApplication Container { get; set; }

        [Inject]
        public IEventService EventService { get; set; }

        [Inject]
        public IReferenceService ReferenceService { get; set; }

        [Inject]
        public IPersistenceData PersistenceData { get; set; }

        [Inject]
        public IResService ResService { get; set; }

        public IReadOnlyList<ILocalizationConfig> Configs => m_Configs;
        public ILocalizationConfig CurrentConfig => m_CurrentConfig;
        public LocalizationStatus Status { get; private set; }

        private const string PersistenceIdentifier = "Localization";

        private readonly List<ILocalizationConfig> m_Configs = new();
        private readonly Dictionary<int, ILocalizationConfig> m_DictConfigs = new();
        private ILocalizationConfig m_CurrentConfig;
        
        private readonly List<ILocalization> m_Localizations = new();
        private readonly List<UniTask> m_Buffer = new();

        public void Initialize()
        {
            LoadConfig();

            var baseType = typeof(ILocalization);

            ReflectService.ForEachType<ILocalization>(type =>
            {
                if (type.IsAbstract)
                    return;

                Type bindInterfaceType = null;
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (@interface == baseType)
                        continue;

                    if (!baseType.IsAssignableFrom(@interface))
                        continue;

                    bindInterfaceType = @interface;
                }

                if (bindInterfaceType == null)
                    return;

                var instance = (ILocalization)Activator.CreateInstance(type);
                m_Localizations.Add(instance);

                Container.Inject(instance);
                Container.BindSingletonInstance(instance, bindInterfaceType);
            });
        }

        public void Dispose()
        {
            Clean();

            foreach (var localization in m_Localizations)
                Container.Release(localization);

            m_Localizations.Clear();
            m_Configs.Clear();
            m_DictConfigs.Clear();
        }

        public UniTask<int> SwitchConfig(int id)
        {
            if (!m_DictConfigs.TryGetValue(id, out var config) || config == null)
                return UniTask.FromResult((int)FrameworkErrorCode.LocalizationNotSupport);
            
            PersistenceData.SetInt(PersistenceIdentifier, id);
            
            m_CurrentConfig = config;
            Clean();

            return UniTask.FromResult(0);
        }

        public async UniTask<int> LoadAsync()
        {
            if (Status == LocalizationStatus.Loaded)
                return 0;

            if (Status == LocalizationStatus.Loading)
            {
                await UniTask.WaitWhile(() => Status == LocalizationStatus.Loading);

                return Status == LocalizationStatus.Loaded ? 0 : (int)FrameworkErrorCode.LocalizationLoadFailed;
            }

            Status = LocalizationStatus.Loading;

            m_Buffer.Clear();
            foreach (var localization in m_Localizations)
                m_Buffer.Add(localization.LoadAsync(m_CurrentConfig));

            await UniTask.WhenAll(m_Buffer);

            if (Status != LocalizationStatus.Loading)
                return (int)FrameworkErrorCode.LocalizationLoadFailed;

            Status = LocalizationStatus.Loaded;
            EventService.Raise(this, ReferenceService.GetReference<LocalizationLoadDoneEvent>());
            return 0;
        }

        public void Clean()
        {
            Status = LocalizationStatus.Unload;

            foreach (var localization in m_Localizations)
                localization.Clean();
        }
    }
}