using System;
using UnityEngine;
using UnityEditor;

internal class MiniLitShaderGUI : ShaderGUI
{
    private string[] renderingModes = new string[] {
        "Normal",
        "Debug_Albedo",
        "Debug_Normal",
        "Debug_Metallic",
        "Debug_AO",
        "Debug_Roughness",
        "Debug_Emission",
        "Debug_Light",
        "Debug_BakedGI"
        };
    private int selectedRenderingMode = 0;
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        materialEditor.EmissionEnabledProperty();
        DrawDebugArea(materialEditor);
    }

    void DrawDebugArea(MaterialEditor materialEditor)
    {
        selectedRenderingMode = EditorGUILayout.Popup("Debug Option", selectedRenderingMode, renderingModes);
        Material targetMaterial = materialEditor.target as Material;
        switch (selectedRenderingMode)
        {
            case 0:
                // normal rendering
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 1:
                // debug albedo
                targetMaterial.EnableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 2:
                // debug normal
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.EnableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 3:
                // debug metallic
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.EnableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 4:
                // debug ao
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.EnableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 5:
                // debug roughness
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.EnableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 6:
                // debug emission
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.EnableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 7:
                // debug light color
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.EnableKeyword("Debug_Light");
                targetMaterial.DisableKeyword("Debug_BakedGI");
                break;
            case 8:
                // debug baked gi
                targetMaterial.DisableKeyword("Debug_Albedo");
                targetMaterial.DisableKeyword("Debug_Normal");
                targetMaterial.DisableKeyword("Debug_Metallic");
                targetMaterial.DisableKeyword("Debug_AO");
                targetMaterial.DisableKeyword("Debug_Roughness");
                targetMaterial.DisableKeyword("Debug_Emission");
                targetMaterial.DisableKeyword("Debug_Light");
                targetMaterial.EnableKeyword("Debug_BakedGI");
                break;
        }
    }
    /*
        // Properties
        private SimpleLitGUI.SimpleLitProperties shadingModelProperties;

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            shadingModelProperties = new SimpleLitGUI.SimpleLitProperties(properties);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, SimpleLitGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            SimpleLitGUI.Inputs(shadingModelProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        public override void DrawAdvancedOptions(Material material)
        {
            SimpleLitGUI.Advanced(shadingModelProperties);
            base.DrawAdvancedOptions(material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);
        }
        */
}

