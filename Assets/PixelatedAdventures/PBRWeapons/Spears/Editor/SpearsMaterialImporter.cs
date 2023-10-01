using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class SpearsMaterialImporter
{
  private enum RenderPipelines
  {
    None,
    BuiltIn,
    URP,
    HDRP
  }

  static SpearsMaterialImporter() => EditorSceneManager.sceneOpened += OnSceneOpened;

  private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
  {
    RenderPipelines currentSRP = DetectRenderPipeline();
    if (EditorApplication.isCompiling == false && currentSRP != RenderPipelines.None)
    {
      string[] allAssets = AssetDatabase.GetAllAssetPaths();

      AssetDatabase.DisallowAutoRefresh();

      UpdateMaterial(currentSRP, "Spear_Common.mat", "Spear_Common_Color", "Spear_Common_Normal", "Spear_Common_Occlusion", "Spear_Common_Metallic", allAssets);
      UpdateMaterial(currentSRP, "Spear_Rare.mat", "Spear_Rare_Color", "Spear_Rare_Normal", "Spear_Rare_Occlusion", "Spear_Rare_Metallic", allAssets);
      UpdateMaterial(currentSRP, "Spear_Epic.mat", "Spear_Epic_Color", "Spear_Epic_Normal", "Spear_Epic_Occlusion", "Spear_Epic_Metallic", allAssets);
      UpdateMaterial(currentSRP, "Spear_Legendary.mat", "Spear_Legendary_Color", "Spear_Legendary_Normal", "Spear_Legendary_Occlusion", "Spear_Legendary_Metallic", allAssets);

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      AssetDatabase.AllowAutoRefresh();
    }
  }

  private static RenderPipelines DetectRenderPipeline()
  {
    RenderPipelines currentPipeline = RenderPipelines.BuiltIn;
    if (GraphicsSettings.renderPipelineAsset != null)
    {
      string srpType = GraphicsSettings.renderPipelineAsset.GetType().ToString();
      if (srpType.Contains("HDRenderPipelineAsset") == true)
        currentPipeline = RenderPipelines.HDRP;
      else if (srpType.Contains("UniversalRenderPipelineAsset") == true || srpType.Contains("LightweightRenderPipelineAsset") == true)
        currentPipeline = RenderPipelines.URP;
      else
      {
        Debug.LogWarning("RenderPipeline not detected.");

        currentPipeline = RenderPipelines.None;
      }
    }

    return currentPipeline;
  }

  private static void UpdateMaterial(RenderPipelines currentSRP, string materialName, string albedoTexture, string normalTexture, string occlusionTexture, string metallicTexture, string[] assets)
  {
    for (int i = 0; i < assets.Length; ++i)
    {
      if (Path.GetFileName(assets[i]) == materialName)
      {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(assets[i]);
        switch (currentSRP)
        {
          case RenderPipelines.BuiltIn:
            material.shader = Shader.Find("Standard");
            material.SetTexture("_MainTex", LoadTexture(albedoTexture, assets));
            material.SetTexture("_BumpMap", LoadTexture(normalTexture, assets));
            material.SetTexture("_OcclusionMap", LoadTexture(occlusionTexture, assets));
            material.SetTexture("_MetallicGlossMap", LoadTexture(metallicTexture, assets));
            break;
          case RenderPipelines.URP:
            material.shader = Shader.Find("Universal Render Pipeline/Lit");
            material.SetFloat("_WorkflowMode", 1.0f);
            material.SetFloat("_Smoothness", 0.75f);
            material.SetTexture("_BaseMap", LoadTexture(albedoTexture, assets));
            material.SetTexture("_BumpMap", LoadTexture(normalTexture, assets));
            material.SetTexture("_OcclusionMap", LoadTexture(occlusionTexture, assets));
            material.SetTexture("_MetallicGlossMap", LoadTexture(metallicTexture, assets));
            break;
          case RenderPipelines.HDRP:
            material.shader = Shader.Find("HDRP/Lit");
            material.SetFloat("_MetallicRemapMax", 0.8f);
            material.SetTexture("_BaseColorMap", LoadTexture(albedoTexture, assets));
            material.SetTexture("_NormalMap", LoadTexture(normalTexture, assets));
            material.SetTexture("_MaskMap", LoadTexture(metallicTexture, assets));
            break;
        }
        
        EditorUtility.SetDirty(material);
      }
    }
  }

  private static Texture LoadTexture(string name, string[] assets)
  {
    Texture texture = null;

    for (int i = 0; i < assets.Length; ++i)
    {
      string fileName = Path.GetFileNameWithoutExtension(assets[i]);
      if (fileName == name)
        texture = AssetDatabase.LoadAssetAtPath<Texture>(assets[i]);
    }

    return texture;
  }
}
