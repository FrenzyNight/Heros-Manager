using System.Collections.Generic;
using UnityEngine;

/// <summary>Scene-local meadow materials; the shared palette and other chapters remain authored assets.</summary>
public sealed class MeadowSurfaceTreatment : MonoBehaviour
{
    readonly List<(MeshRenderer Renderer, Material[] Originals)> bindings = new();
    readonly List<Material> ownedMaterials = new();
    public int TreatedRenderers => bindings.Count;

    void OnEnable()
    {
        if (!InGameMgr.IsCreateInstance() || InGameMgr.Instance.stage != 1) return;
        var land = GameObject.Find("Map/Land");
        if (land == null) return;
        var shader = Resources.Load<Shader>("Shaders/MeadowSurface");
        if (shader == null || !shader.isSupported)
        {
            Debug.LogError("The meadow surface shader is missing or unsupported.", this);
            return;
        }
        foreach (var renderer in land.GetComponentsInChildren<MeshRenderer>(true))
        {
            bool ground = renderer.name == "terrain(blenderre)";
            bool foliage = renderer.name == "tree_re";
            bool rock = renderer.name == "rock_re";
            if (!ground && !foliage && !rock) continue;
            var originals = renderer.sharedMaterials;
            var replacements = (Material[])originals.Clone();
            bool changed = false;
            for (int slot = 0; slot < originals.Length; slot++)
            {
                var source = originals[slot];
                if (source == null || source.shader.name != "Universal Render Pipeline/Lit") continue;
                var material = new Material(shader) { name = source.name + " (Meadow)", enableInstancing = true };
                material.SetTexture("_BaseMap", source.GetTexture("_BaseMap"));
                material.SetTextureScale("_BaseMap", source.GetTextureScale("_BaseMap"));
                material.SetTextureOffset("_BaseMap", source.GetTextureOffset("_BaseMap"));
                material.SetColor("_BaseColor", source.GetColor("_BaseColor"));
                material.SetFloat("_GroundBlend", ground ? 1 : 0);
                material.SetFloat("_FoliageBlend", foliage ? 1 : 0);
                material.SetFloat("_Variation", ground ? 0.24f : foliage ? 0.10f : 0.12f);
                material.SetFloat("_Smoothness", foliage ? 0.16f : 0.06f);
                replacements[slot] = material;
                ownedMaterials.Add(material);
                changed = true;
            }
            if (!changed) continue;
            bindings.Add((renderer, originals));
            renderer.sharedMaterials = replacements;
        }
    }

    void OnDisable()
    {
        foreach (var binding in bindings)
            if (binding.Renderer != null) binding.Renderer.sharedMaterials = binding.Originals;
        bindings.Clear();
        foreach (var material in ownedMaterials) Destroy(material);
        ownedMaterials.Clear();
    }
}
