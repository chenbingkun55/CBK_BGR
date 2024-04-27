using System;
using System.Collections.Generic;
using System.Threading;
using CatLib;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using FairyGUI;
using FairyGUI.Dynamic;
using FairyGUI.Utils;
using CBK.Framework.Core;
using CBK.Framework.DynamicText;
using CBK.Framework.Extensions;
using CBK.Framework.Json;
using CBK.Framework.Localization;
using CBK.Framework.Reflect;
using Libraries.FairyGUI.Runtime.Utils;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI界面辅助器。
    /// </summary>
    internal sealed class FairyGUIFormHelper : IInitialize, IDisposable, IUIFormHelper
    {
        [Inject]
        public IReflectService ReflectService { get; set; }

        [Inject]
        public IUIAssetManagerConfiguration UIAssetManagerConfiguration { get; set; }
        
        [Inject]
        public IFairyGUIConfiguration FairyGUIConfiguration { get; set; }
        
        [Inject]
        public IApplication Application { get; set; }
        
        [Inject]
        public ILocalizationService LocalizationService { get; set; }
        
        [Inject]
        public ITextLocalization TextLocalization { get; set; }
        
        [Inject]
        public IJsonService JsonService { get; set; }
        
        [Inject]
        public IDynamicTextService DynamicTextService { get; set; }

        private readonly IUIAssetManager m_UIAssetManager = new UIAssetManager();
        private readonly Dictionary<string, FairyGUIFormInfo> m_FairyGUIFormInfos = new(StringComparer.Ordinal);

        public void Initialize()
        {
            GRoot.inst.SetContentScaleFactor(FairyGUIConfiguration.DesignResolution.x, FairyGUIConfiguration.DesignResolution.y, FairyGUIConfiguration.ScreenMatchMode switch
            {
                ScreenMatchMode.Constant => UIContentScaler.ScreenMatchMode.MatchWidthOrHeight,
                ScreenMatchMode.MatchWidth => UIContentScaler.ScreenMatchMode.MatchWidth,
                ScreenMatchMode.MatchHeight => UIContentScaler.ScreenMatchMode.MatchHeight,
                ScreenMatchMode.MatchWidthOrHeight => UIContentScaler.ScreenMatchMode.MatchWidthOrHeight,
                ScreenMatchMode.ConstantWidthAndLimitHeight => UIContentScaler.ScreenMatchMode.MatchWidthOrHeight,
                ScreenMatchMode.ConstantHeightAndLimitWidth => UIContentScaler.ScreenMatchMode.MatchWidthOrHeight,
                _ => throw new ArgumentOutOfRangeException()
            });
            
            m_UIAssetManager.Initialize(UIAssetManagerConfiguration);

            ReflectService.ForEachTypeWithAttribute<FairyGUIFormAttribute>(typeof(AFairyGUIFormLogic), (type, attribute) => { m_FairyGUIFormInfos.Add(attribute.UIKey, new FairyGUIFormInfo(attribute.PackageName, attribute.ComponentName, type)); });
            ReflectService.ForEachTypeWithAttribute<FairyGUIComponentAttribute>((type, attribute) =>
            {
                UIObjectFactory.SetPackageItemExtension(attribute.URL, type);
            });
            
            // 分支设置
            UIPackage.branch = LocalizationService.CurrentConfig.LangCode;
            // 反序列化委托设置
            SerializeUtils.DeserializeFunc = FairyGUIDeserializeFunc;
            // 翻译委托设置
            TranslationHelper.GetTextFunc = FairyGUIGetTextFunc;
            // 动态文本委托设置
            StringUtils.ParseTemplateHandler = FairyGUIParseTemplateFunc;
        }

        private string FairyGUIParseTemplateFunc(string template, IReadOnlyDictionary<string, string> vars)
        {
            return DynamicTextService
                .Allocate(template)
                .SetVars(vars)
                .FlushVars();
        }

        public void Dispose()
        {
            UIObjectFactory.Clear();
            m_FairyGUIFormInfos.Clear();
            m_UIAssetManager.Dispose();
            UIPackage.branch = null;
            SerializeUtils.DeserializeFunc = null;
            TranslationHelper.GetTextFunc = null;
        }

        public async UniTask<IUIForm> CreateUIForm(string uiKey, CancellationToken token)
        {
            if (!m_FairyGUIFormInfos.TryGetValue(uiKey, out var fairyGUIFormInfo))
                return null;

            if (await FairyGUIUtility.CreateObjectAsync(fairyGUIFormInfo.PackageName, fairyGUIFormInfo.ComponentName, token) is not GComponent gObject)
                return null;

            var viewLogic = (AFairyGUIFormLogic)Activator.CreateInstance(fairyGUIFormInfo.FormLogicType);
            Application.Inject(viewLogic);
            return new FairyGUIForm(uiKey, gObject, viewLogic);
        }

        public void RecycleUIForm(IUIForm uiForm)
        {
            if (uiForm.Handle is not GComponent gComponent)
                return;

            gComponent.Dispose();
        }

        private string FairyGUIGetTextFunc(string key, string fallbackText)
        {
            return TextLocalization.GetText(key);
        }

        private Dictionary<string, object> FairyGUIDeserializeFunc(string str)
        {
            return JsonService.Deserialize<Dictionary<string, object>>(str);
        }

        private sealed class FairyGUIFormInfo
        {
            public string PackageName { get; }
            public string ComponentName { get; }
            public Type FormLogicType { get; }

            public FairyGUIFormInfo(string packageName, string componentName, Type formLogicType)
            {
                PackageName = packageName;
                ComponentName = componentName;
                FormLogicType = formLogicType;
            }
        }
    }
}