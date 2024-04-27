using System.Collections.Generic;
using CatLib.Container;
using FairyGUI;
using CBK.Framework.Camera;
using UnityEngine;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI 界面组辅助器。
    /// </summary>
    internal sealed class FairyGUIGroupHelper : IUIGroupHelper
    {
        [Inject]
        public ICameraService CameraService { get; set; }
        
        [Inject]
        public IFairyGUIConfiguration Configuration { get; set; }
        
        public GComponent GroupRoot { get; private set; }

        private GObject m_MaskLayer;
        private RenderTexture m_MaskRenderTexture;
        private bool m_EnableCapture;
        
        private FairyGUIForm m_LastMaskLayerForm;
        private List<UnityEngine.Camera> m_CameraBuffer;

        public void Initialize(string groupName)
        {
            var groot = GRoot.inst;
            GroupRoot = new GComponent();
            GroupRoot.gameObjectName = GroupRoot.displayObject.name = GroupRoot.name = $"Group - {groupName}";
            GroupRoot.size = groot.size;
            GroupRoot.Center(true);
            GroupRoot.AddRelation(groot, RelationType.Size);

            groot.AddChild(GroupRoot);
        }

        public void Dispose()
        {
            GroupRoot?.Dispose();
            GroupRoot = null;
            m_MaskLayer?.Dispose();
            m_MaskLayer = null;

            if (m_MaskRenderTexture != null)
            {
                RenderTexture.ReleaseTemporary(m_MaskRenderTexture);
                m_MaskRenderTexture = null;
            }
        }

        public void SetDepth(int depth)
        {
            GroupRoot.sortingOrder = depth + 1;
        }

        public void SetMaskLayerForm(IUIForm uiForm)
        {
            var fairyGUIForm = uiForm as FairyGUIForm;
            if (fairyGUIForm == null)
            {
                m_MaskLayer?.RemoveFromParent();
                m_LastMaskLayerForm = null;
                return;
            }

            CreateMaskLayer();

            var isDifferent = m_LastMaskLayerForm != fairyGUIForm;
            m_LastMaskLayerForm = fairyGUIForm;
            
            if (isDifferent)
                UpdateRenderTexture();

            m_MaskLayer.sortingOrder = fairyGUIForm.DepthInUIGroup << 1;
            if (m_MaskLayer.parent == null)
                GroupRoot.AddChild(m_MaskLayer);
        }

        private void UpdateRenderTexture()
        {
            if (m_LastMaskLayerForm == null)
                return;
            
            if (m_MaskLayer == null)
                return;
            
            if (!m_EnableCapture)
                return;

            // 截图前处理
            m_MaskLayer.visible = false;
            m_LastMaskLayerForm.ContentPane.visible = false;

            // 获取相机
            m_CameraBuffer ??= new List<UnityEngine.Camera>();
            CameraService.GetCamerasInStack(m_CameraBuffer);
                
            var renderTexture = RenderTexture.GetTemporary(m_MaskRenderTexture.width, m_MaskRenderTexture.height, 0, m_MaskRenderTexture.format);
                
            foreach (var camera in m_CameraBuffer)
            {
                camera.targetTexture = renderTexture;
                camera.Render();
                camera.targetTexture = null;
                    
                Graphics.Blit(renderTexture, m_MaskRenderTexture);
            }
                
            RenderTexture.ReleaseTemporary(renderTexture);

            m_MaskLayer.visible = true;
            m_LastMaskLayerForm.ContentPane.visible = true;
        }

        private void CreateMaskLayer()
        {
            if (m_MaskLayer != null)
                return;

            var w = (int)GroupRoot.width;
            var h = (int)GroupRoot.height;

            var maskComponent = new GComponent();
            maskComponent.name = maskComponent.gameObjectName = "MaskLayer";
            maskComponent.xy = Vector2.zero;
            maskComponent.SetSize(w, h);
            maskComponent.AddRelation(GroupRoot, RelationType.Size);
            maskComponent.SetHome(GroupRoot);
            maskComponent.onClick.Add(OnMaskLayerClick);

            // graph
            var graph = new GGraph();
            graph.name = graph.gameObjectName = "Graph";
            graph.DrawRect(w, h, 0, Color.white, Configuration.MaskLayerColor);
            graph.xy = Vector2.zero;
            graph.AddRelation(maskComponent, RelationType.Size);
            maskComponent.AddChild(graph);

            // blur
            m_EnableCapture = Configuration.MaskLayerBlurSize > 0f;
            if (m_EnableCapture)
            {
                var captureWidth = w;
                var captureHeight = h;

                if (Configuration.MaskLayerDownSample > 0)
                {
                    captureWidth >>= Configuration.MaskLayerDownSample;
                    captureHeight >>= Configuration.MaskLayerDownSample;
                }
                
                m_MaskRenderTexture = RenderTexture.GetTemporary(captureWidth, captureHeight, 0, RenderTextureFormat.RGB565);
                var blurLayer = new GImage();
                blurLayer.name = blurLayer.gameObjectName = "CaptureScreen";
                blurLayer.texture = new NTexture(m_MaskRenderTexture);
                blurLayer.SetSize(w, h);
                blurLayer.filter = new BlurFilter
                {
                    blurSize = Configuration.MaskLayerBlurSize,
                    downSample = 0, // 这里不需要降采样 因为在截图时已经降采样了
                };
                blurLayer.xy = Vector2.zero;
                blurLayer.AddRelation(maskComponent, RelationType.Size);
                maskComponent.AddChild(blurLayer);
            }
            
            m_MaskLayer = maskComponent;
        }

        private void OnMaskLayerClick()
        {
            m_LastMaskLayerForm?.OnMaskLayerClicked();
        }
    }
}