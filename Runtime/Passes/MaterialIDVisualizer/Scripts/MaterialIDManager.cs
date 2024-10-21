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

    }
    static MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

    public static void ClearMaterialBlock()
    {
        if (materialPropertyBlock != null)
        {
            materialPropertyBlock.Clear();
        }
    }

    static Dictionary<Material, Color> colorIDsByMaterial = new Dictionary<Material, Color>();
    public static void SetColorIDsByMaterials()
    {
        SetRenders();
        materialPropertyBlock.Clear();
        colorIDsByMaterial.Clear();
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderer.GetPropertyBlock(materialPropertyBlock, i);
                if (renderer.sharedMaterials[i] == null)
                {
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), Color.magenta);

                }
                else
                {
                    SetColorIDsByMaterial(renderer.sharedMaterials[i]);
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), colorIDsByMaterial[renderer.sharedMaterials[i]]);
                }
                renderer.SetPropertyBlock(materialPropertyBlock, i);
            }
        }
    }

    static void SetColorIDsByMaterial(Material material)
    {
        if (!colorIDsByMaterial.ContainsKey(material))
        {
            colorIDsByMaterial[material] = GetNewColor();
        }
    }
    static Color GetNewColor()
    {
        return Random.ColorHSV(0.0f, 1.0f, 0.0f, 1.0f, 0.5f, 1.0f);
    }
    public static int MaterialIDsCount
    {
        get
        {
            return colorIDsByMaterial.Count;
        }
    }
}
