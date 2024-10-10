using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MiniRPController
{
    public static UniversalRenderPipelineAsset currentAsset;

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
