using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class MaterialIDPass : ScriptableRenderPass
{
    RTHandle[] m_ColorTargetIndentifiers;
    RTHandle m_DepthTargetIndentifiers;
    List<ShaderTagId> shaderTagIds = new List<ShaderTagId>
    {
        new("UniversalForward"),
        new("SRPDefaultUnlit"),
        new("UniversalForwardOnly")
    };
    Material m_OverrideMaterial;
    static string m_ProfilerTag = "MaterialID Pass";
    static ProfilingSampler m_ProfilingSampler = new ProfilingSampler(m_ProfilerTag);
    static FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all);
    public MaterialIDPass()
    {
        m_OverrideMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/SoFunny/Utils/MaterialID"));
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
    }

    public void Setup(RTHandle colorAttachment, RTHandle renderingLayersTexture, RTHandle depthAttachment)
    {
        m_ColorTargetIndentifiers[0] = colorAttachment;
        m_ColorTargetIndentifiers[1] = renderingLayersTexture;
        m_DepthTargetIndentifiers = depthAttachment;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer commandBuffer = CommandBufferPool.Get();
        using (new ProfilingScope(commandBuffer, m_ProfilingSampler))
        {
            context.ExecuteCommandBuffer(commandBuffer);
            commandBuffer.Clear();
            Draw(context, ref renderingData);
        }
        context.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
        CommandBufferPool.Release(commandBuffer);
    }
    /*
    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        ConfigureTarget(m_ColorTargetIndentifiers, m_DepthTargetIndentifiers);
    }
    */

    void Draw(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, ref renderingData, SortingCriteria.RenderQueue);
        drawSettings.overrideMaterial = m_OverrideMaterial;
        context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
    }
}
