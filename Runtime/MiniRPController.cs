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
}
