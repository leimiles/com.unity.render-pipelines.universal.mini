using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class MaterialIDManager
{
    static Renderer[] renderers;

    static void SetRenders()
    {
        renderers = GameObject.FindObjectsOfType<Renderer>();
        Debug.Log(renderers.Length);
        SetErrorMaterial();
    }
    static MaterialPropertyBlock materialPropertyBlock;
    static Material errorMaterial;
    static void SetErrorMaterial()
    {
        if (errorMaterial == null)
        {
            errorMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/InternalErrorShader"));
        }
    }
    public static void SetColorIDsByMaterials()
    {
        SetRenders();
        if (materialPropertyBlock != null)
        {
            materialPropertyBlock.Clear();
        }
        else
        {
            materialPropertyBlock = new MaterialPropertyBlock();
        }
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderer.GetPropertyBlock(materialPropertyBlock, i);
                materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), Color.yellow);
                renderer.SetPropertyBlock(materialPropertyBlock, i);
            }
        }
    }
}
