using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class MaterialIDPass : ScriptableRenderPass
{
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
        m_OverrideMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("SoFunny/Utils/MaterialID"));
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
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

    void Draw(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var drawSettings = RenderingUtils.CreateDrawingSettings(shaderTagIds, ref renderingData, SortingCriteria.RenderQueue);
        drawSettings.overrideMaterial = m_OverrideMaterial;
        context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
    }
}
