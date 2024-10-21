using System.Collections.Generic;
using UnityEngine;

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
        materialPropertyBlock.Clear();
        colorIDsByMaterial.Clear();
        colorIDsByMesh.Clear();
        colorIDsByShader.Clear();
        colorIDsByShaderVariant.Clear();
    }
    static Dictionary<string, Color> colorIDsByShaderVariant = new Dictionary<string, Color>();
    public static void SetColorIDsByShaderVariants()
    {
        SetRenders();
        materialPropertyBlock.Clear();
        colorIDsByShaderVariant.Clear();
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderer.GetPropertyBlock(materialPropertyBlock, i);
                if (renderer.sharedMaterials[i] == null)
                {
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), Color.magenta);
                    Debug.LogError(renderer.name + " 's material is null");
                }
                else
                {
                    string shaderAndVariantsName = renderer.sharedMaterials[i].shader.name; // use shader name first
                    shaderAndVariantsName += string.Join(' ', renderer.sharedMaterials[i].shaderKeywords);      // only local keywords
                    SetColorIDsByShaderVariant(shaderAndVariantsName);
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), colorIDsByShaderVariant[shaderAndVariantsName]);
                }
                renderer.SetPropertyBlock(materialPropertyBlock, i);
            }
        }
    }

    static void SetColorIDsByShaderVariant(string shaderAndVariantsName)
    {
        if (!colorIDsByShaderVariant.ContainsKey(shaderAndVariantsName))
        {
            colorIDsByShaderVariant[shaderAndVariantsName] = GetNewColor();
        }
    }

    static Dictionary<Shader, Color> colorIDsByShader = new Dictionary<Shader, Color>();
    public static void SetColorIDsByShaders()
    {
        SetRenders();
        materialPropertyBlock.Clear();
        colorIDsByShader.Clear();
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderer.GetPropertyBlock(materialPropertyBlock, i);
                if (renderer.sharedMaterials[i] == null)
                {
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), Color.magenta);
                    Debug.LogError(renderer.name + " 's material is null");
                }
                else
                {
                    SetColorIDsByShader(renderer.sharedMaterials[i].shader);
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), colorIDsByShader[renderer.sharedMaterials[i].shader]);
                }
                renderer.SetPropertyBlock(materialPropertyBlock, i);
            }
        }
    }

    static void SetColorIDsByShader(Shader shader)
    {
        if (!colorIDsByShader.ContainsKey(shader))
        {
            colorIDsByShader[shader] = GetNewColor();
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
                    Debug.LogError(renderer.name + " 's material is null");
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
    static Dictionary<Mesh, Color> colorIDsByMesh = new Dictionary<Mesh, Color>();
    public static void SetColorIDsByMeshes()
    {
        SetRenders();
        materialPropertyBlock.Clear();
        colorIDsByMesh.Clear();
        foreach (Renderer renderer in renderers)
        {
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderer.GetPropertyBlock(materialPropertyBlock, i);
                Mesh mesh;
                mesh = (renderer as MeshRenderer)?.GetComponent<MeshFilter>().sharedMesh;
                if (mesh == null)
                {
                    if (renderer.GetComponent<SkinnedMeshRenderer>() != null)
                    {
                        mesh = renderer.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    }
                }
                if (mesh == null)
                {
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), Color.magenta);
                    Debug.LogError(renderer.name + " 's mesh is null");
                }
                else
                {
                    SetColorIDsByMesh(mesh);
                    materialPropertyBlock.SetColor(Shader.PropertyToID("_ColorID"), colorIDsByMesh[mesh]);
                }
                renderer.SetPropertyBlock(materialPropertyBlock, i);
            }
        }
    }

    static void SetColorIDsByMesh(Mesh mesh)
    {
        if (!colorIDsByMesh.ContainsKey(mesh))
        {
            colorIDsByMesh[mesh] = GetNewColor();
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
        return Random.ColorHSV(0.0f, 1.0f, 0.5f, 1.0f, 0.4f, 0.9f);
    }
    public static int MaterialIDsCount
    {
        get
        {
            return colorIDsByMaterial.Count;
        }
    }
    public static int MeshIDsCount
    {
        get
        {
            return colorIDsByMesh.Count;
        }
    }
    public static int VariantIDsCount
    {
        get
        {
            return colorIDsByShaderVariant.Count;
        }
    }
    public static int ShaderIDsCount
    {
        get
        {
            return colorIDsByShader.Count;
        }
    }
}
