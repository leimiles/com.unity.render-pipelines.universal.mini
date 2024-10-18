using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class MiniRPController
{
    public static DrawMode CurrentDrawMode = DrawMode.Normal;
    public enum DrawMode
    {
        Normal,
        MaterialID,
        MeshID,
        ShaderID,
        VariantID
    }
    private static UniversalRenderPipelineAsset currentAsset;
    private static UniversalRendererData currentRendererData;

    internal static UniversalRendererData CurrentRendererData
    {
        get => currentRendererData;
        set
        {
            currentRendererData = value;
#if (WX_PERFORMANCE_MODE || !WX_PREVIEW_SCENE_MODE)
            currentRendererData.debugShaders.debugReplacementPS = null;
            currentRendererData.debugShaders.hdrDebugViewPS = null;
#endif
        }
    }
    internal static UniversalRenderPipelineAsset CurrentAsset { get => currentAsset; set => currentAsset = value; }

    public static void DebugMainLightShadow()
    {
        if (CurrentAsset != null)
        {
            if (CurrentAsset.supportsMainLightShadows == true)
            {
                CurrentAsset.supportsMainLightShadows = false;
            }
            else
            {
                CurrentAsset.supportsMainLightShadows = true;
            }
        }
    }

    /*
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
    */

    public static void DebugDraw(DrawMode drawMode)
    {
        CurrentDrawMode = drawMode;
    }
}
