using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class MiniRPController
{
    internal static UniversalRenderPipelineAsset currentAsset;
    internal static UniversalRendererData currentRendererData;

    public static void DebugMainLightShadow()
    {
        if (currentAsset != null)
        {
            if (currentAsset.supportsMainLightShadows == true)
            {
                currentAsset.supportsMainLightShadows = false;
            }
            else
            {
                currentAsset.supportsMainLightShadows = true;
            }
        }
    }

    static int m_RenderingMode = 0;
    public static void DebugRenderingPath()
    {
        if (currentRendererData != null)
        {
            currentRendererData.renderingMode = (RenderingMode)(m_RenderingMode);
            m_RenderingMode++;
            if (m_RenderingMode > 2)
            {
                m_RenderingMode = 0;
            }
        }
    }
}
