using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>Explicit, repeatable authoring command. Never runs on import or player startup.</summary>
public static class CampLightingSetup
{
    [MenuItem("Tools/Heros Manager/Apply Forest Lighting Art Direction")]
    public static void Apply()
    {
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene("Assets/01.Scenes/InGame.unity");
        var lighting = Object.FindAnyObjectByType<LightController>();
        lighting.maxSunLightIntensity = 1.15f;
        lighting.maxMoonLightIntensity = 0.55f;
        lighting.dayAmbientLight = new Color(0.60f, 0.66f, 0.72f);
        lighting.nightAmbientLight = new Color(0.27f, 0.34f, 0.48f);
        lighting.fireRange = 18;
        lighting.fireIntensity = 32;
        EditorUtility.SetDirty(lighting);
        const string profilePath = "Assets/01.Scenes/InGameLighting.asset";
        var profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);
        }
        if (!profile.TryGet<Tonemapping>(out var tone))
        { tone = profile.Add<Tonemapping>(); AssetDatabase.AddObjectToAsset(tone, profile); }
        tone.mode.Override(TonemappingMode.Neutral);
        if (!profile.TryGet<ColorAdjustments>(out var color))
        { color = profile.Add<ColorAdjustments>(); AssetDatabase.AddObjectToAsset(color, profile); }
        color.saturation.Override(-22);
        color.contrast.Override(0);
        color.postExposure.Override(0);
        EditorUtility.SetDirty(tone);
        EditorUtility.SetDirty(color);
        EditorUtility.SetDirty(profile);
        var volume = lighting.GetComponent<Volume>();
        if (volume == null) volume = lighting.gameObject.AddComponent<Volume>();
        volume.isGlobal = true;
        volume.priority = 5;
        volume.sharedProfile = profile;
        EditorUtility.SetDirty(volume);
        Camera.main.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        EditorUtility.SetDirty(Camera.main.GetUniversalAdditionalCameraData());
        var pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        pipeline.supportsHDR = true;
        var pipelineSettings = new SerializedObject(pipeline);
        pipelineSettings.FindProperty("m_SoftShadowsSupported").boolValue = true;
        pipelineSettings.ApplyModifiedPropertiesWithoutUndo();
        pipeline.msaaSampleCount = 4;
        EditorUtility.SetDirty(pipeline);
        ConfigureContactShading();
        EditorSceneManager.MarkSceneDirty(lighting.gameObject.scene);
        EditorSceneManager.SaveScene(lighting.gameObject.scene);
        AssetDatabase.SaveAssets();
        Debug.Log("CAMP LIGHTING SETUP COMPLETE");
    }

    static void ConfigureContactShading()
    {
        const string path = "Assets/03.Prefabs/Camp/Objects_re/ObjectsRE/UniversalRenderPipelineAsset_Renderer.asset";
        var renderer = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(path);
        ScreenSpaceAmbientOcclusion occlusion = null;
        foreach (var feature in renderer.rendererFeatures)
            if (feature is ScreenSpaceAmbientOcclusion existing) occlusion = existing;
        if (occlusion == null)
        {
            occlusion = ScriptableObject.CreateInstance<ScreenSpaceAmbientOcclusion>();
            occlusion.name = "Forest Contact Shading";
            AssetDatabase.AddObjectToAsset(occlusion, renderer);
            renderer.rendererFeatures.Add(occlusion);
        }
        var serialized = new SerializedObject(occlusion);
        var settings = serialized.FindProperty("m_Settings");
        settings.FindPropertyRelative("Intensity").floatValue = 0.65f;
        settings.FindPropertyRelative("Radius").floatValue = 0.65f;
        settings.FindPropertyRelative("DirectLightingStrength").floatValue = 0.15f;
        settings.FindPropertyRelative("Downsample").boolValue = true;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        occlusion.SetActive(true);
        renderer.SetDirty();
        EditorUtility.SetDirty(renderer);
    }
}
