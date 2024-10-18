using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class MaterialIDEditor
{
    static Vector2 buttonSize = new Vector2(80, 20);
    static Vector2 infoSize = new Vector2(300, 150);
    static MaterialIDEditor()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
#if (!WX_PERFORMANCE_MODE || WX_PREVIEW_SCENE_MODE)
        return;
#endif
        DrawButtons();
        DrawInfos();
    }

    private static void DrawButtons()
    {
        Handles.BeginGUI();
        if (GUI.Button(new Rect(Screen.width / 2 - buttonSize.x * 2 - buttonSize.x / 2, 0, buttonSize.x, buttonSize.y), "Normal"))
        {
            MiniRPController.DebugDraw(MiniRPController.DrawMode.Normal);
            MaterialIDManager.ClearMaterialBlock();
        }
        if (GUI.Button(new Rect(Screen.width / 2 - buttonSize.x * 1 - buttonSize.x / 2, 0, buttonSize.x, buttonSize.y), "Material ID"))
        {
            MaterialIDManager.SetColorIDsByMaterials();
            MiniRPController.DebugDraw(MiniRPController.DrawMode.MaterialID);
        }
        if (GUI.Button(new Rect(Screen.width / 2 - buttonSize.x / 2, 0, buttonSize.x, buttonSize.y), "Mesh ID"))
        {
            MiniRPController.DebugDraw(MiniRPController.DrawMode.MeshID);
        }
        if (GUI.Button(new Rect(Screen.width / 2 + buttonSize.x * 1 - buttonSize.x / 2, 0, buttonSize.x, buttonSize.y), "Shader ID"))
        {
            MiniRPController.DebugDraw(MiniRPController.DrawMode.ShaderID);
        }
        if (GUI.Button(new Rect(Screen.width / 2 + buttonSize.x * 2 - buttonSize.x / 2, 0, buttonSize.x, buttonSize.y), "Variant ID"))
        {
            MiniRPController.DebugDraw(MiniRPController.DrawMode.VariantID);
        }
        Handles.EndGUI();
    }

    private static void DrawInfos()
    {
        Handles.BeginGUI();
        Rect rect = new Rect(10, Screen.height - infoSize.y, infoSize.x, infoSize.y);
        switch (MiniRPController.CurrentDrawMode)
        {
            case MiniRPController.DrawMode.Normal:
                break;
            case MiniRPController.DrawMode.MaterialID:
                GUI.Label(rect, "MaterialID Count: " + 0);
                break;
            case MiniRPController.DrawMode.MeshID:
                GUI.Label(rect, "MeshID Count: " + 0);
                break;
            case MiniRPController.DrawMode.ShaderID:
                GUI.Label(rect, "ShaderID Count: " + 0);
                break;
            case MiniRPController.DrawMode.VariantID:
                GUI.Label(rect, "VariantID Count: " + 0);
                break;
        }
        Handles.EndGUI();
    }
}