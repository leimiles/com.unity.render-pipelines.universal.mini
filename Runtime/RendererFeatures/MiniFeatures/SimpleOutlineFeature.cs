using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DisallowMultipleRendererFeature("Simple Outline")]
public class SimpleOutlineFeature : ScriptableRendererFeature
{
    [SerializeField]
    [Range(0.0f, 2.0f)]
    internal float m_OutlineWidth = 0.5f;
    [SerializeField]
    LayerMask m_LayerMask = -1;

    [SerializeField]
    [HideInInspector]
    [Reload("Shaders/MiniOutline.shader")]
    private Shader m_Shader;
    private Material m_Material;
    private SimpleOutlinePass m_SimpleOutlinePass = null;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (m_SimpleOutlinePass == null)
        {
            return;
        }
        renderer.EnqueuePass(m_SimpleOutlinePass);
        m_SimpleOutlinePass.m_FilteringSettings.layerMask = m_LayerMask;
    }

    public override void Create()
    {
#if UNITY_EDITOR
        ResourceReloader.TryReloadAllNullIn(this, UniversalRenderPipelineAsset.packagePath);
#endif
        CreateMaterial();
        if (m_SimpleOutlinePass == null)
        {
            m_SimpleOutlinePass = new SimpleOutlinePass();
            m_SimpleOutlinePass.m_OutlineMaterial = m_Material;
            m_SimpleOutlinePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            m_SimpleOutlinePass.m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, m_LayerMask);
        }
    }

    protected override void Dispose(bool disposing)
    {
        m_SimpleOutlinePass = null;
        //CoreUtils.Destroy(m_Material);
    }

    private void CreateMaterial()
    {
        if (m_Material == null && m_Shader != null)
        {
            m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        }
    }

    private class SimpleOutlinePass : ScriptableRenderPass
    {
        internal Material m_OutlineMaterial;
        internal FilteringSettings m_FilteringSettings;
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = renderingData.commandBuffer;
            DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
            drawingSettings.overrideMaterial = m_OutlineMaterial;
            if (drawingSettings.overrideMaterial == null)
            {
                Debug.LogError("simple outline material is not ready");
                return;
            }
            using (new ProfilingScope(cmd, new ProfilingSampler("SimpleOutline Pass")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
            }

        }
    }

}
