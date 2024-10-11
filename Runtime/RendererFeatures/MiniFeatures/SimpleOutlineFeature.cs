using System.Collections;
using System.Collections.Generic;
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
        if (!CreateMaterial())
        {
            return;
        }
        if (m_SimpleOutlinePass == null)
        {
            return;
        }
        m_SimpleOutlinePass.m_OutlineMaterial = m_Material;
        if (m_SimpleOutlinePass.m_OutlineMaterial == null)
        {
            return;
        }
        m_SimpleOutlinePass.m_FilteringSettings.layerMask = m_LayerMask;
        renderer.EnqueuePass(m_SimpleOutlinePass);

    }

    public override void Create()
    {
#if UNITY_EDITOR
        ResourceReloader.TryReloadAllNullIn(this, UniversalRenderPipelineAsset.packagePath);
#endif
        if (m_SimpleOutlinePass == null)
        {
            m_SimpleOutlinePass = new SimpleOutlinePass();
        }
        CreateMaterial();
        m_SimpleOutlinePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        m_SimpleOutlinePass.m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, m_LayerMask);
    }

    protected override void Dispose(bool disposing)
    {
        m_SimpleOutlinePass = null;
        CoreUtils.Destroy(m_Material);
    }

    private bool CreateMaterial()
    {
        if (m_Material != null)
        {
            return true;
        }
        if (m_Shader == null)
        {
            if (m_Shader == null)
            {
                return false;
            }
        }
        m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
        m_Material.EnableKeyword("ENABLE_VS_SKINNING");
        return m_Material != null;
    }

    private class SimpleOutlinePass : ScriptableRenderPass
    {
        internal Material m_OutlineMaterial;
        internal FilteringSettings m_FilteringSettings;
        private static string m_ProfilerTag = "SimpleOutline Pass";
        private static ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = renderingData.commandBuffer;
            DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque);
            drawingSettings.overrideMaterial = m_OutlineMaterial;
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
            }
        }
    }

}
