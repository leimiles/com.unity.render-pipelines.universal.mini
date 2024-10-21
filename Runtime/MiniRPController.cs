using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class MiniRPController
{
    static DrawMode currentDrawMode = DrawMode.Normal;
    public static DrawMode CurrentDrawMode
    {
        get
        {
            return currentDrawMode;
        }
    }
    public enum DrawMode
    {
        Normal,
        MaterialID,
        MeshID,
        ShaderID,
        VariantID
    }
    private static UniversalRenderPipelineAsset currentAsset;
    internal static UniversalRenderPipelineAsset CurrentAsset
    {
        get => currentAsset;
        set
        {
            currentAsset = value;
        }
    }
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

    public static void DebugDraw(DrawMode drawMode)
    {
        currentDrawMode = drawMode;
        if (CurrentRendererData != null && CurrentAsset != null)
        {
            if (CurrentDrawMode != DrawMode.Normal)
            {

                CurrentRendererData.opaqueLayerMask = 0;
                CurrentRendererData.transparentLayerMask = 0;
                /*
                if (CurrentRendererData.rendererFeatures.Count > 0)
                {
                    foreach (var feature in CurrentRendererData.rendererFeatures)
                    {
                        feature.SetActive(false);
                    }
                }
                */
                CurrentAsset.supportsMainLightShadows = false;
            }
            else
            {
                CurrentRendererData.opaqueLayerMask = -1;
                CurrentRendererData.transparentLayerMask = -1;
                /*
                for (int i = 0; i < CurrentRendererData.rendererFeatures.Count; i++)
                {
                    CurrentRendererData.rendererFeatures[i].SetActive(true);
                }
                CurrentAsset.supportsMainLightShadows = true;
                */

            }
        }
    }
}
