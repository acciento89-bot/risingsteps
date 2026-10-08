#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
public static class RisingArtImports
{
 public static void Ensure(){Directory.CreateDirectory("Assets/Resources/Materials");Create("Surface","Standard");Create("Sparkles","Rising/Sparkles");Create("Glow","Standard");var glow=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/Glow.mat");glow.EnableKeyword("_EMISSION");glow.SetColor("_EmissionColor",new Color(2.5f,1.8f,.3f));EditorUtility.SetDirty(glow);
  foreach(string path in new[]{"Assets/Resources/Art/TerrainAtlas.png","Assets/Resources/Art/SkyPanorama.png","Assets/Resources/Art/CloudBank.png"}){var t=AssetImporter.GetAtPath(path) as TextureImporter;if(t==null)continue;t.maxTextureSize=2048;if(path.Contains("CloudBank")){t.alphaIsTransparency=true;t.wrapMode=TextureWrapMode.Clamp;}t.mipmapEnabled=true;t.anisoLevel=4;t.textureCompression=TextureImporterCompression.Compressed;t.wrapMode=path.Contains("Atlas")||path.Contains("CloudBank")?TextureWrapMode.Clamp:TextureWrapMode.Repeat;t.SaveAndReimport();}var icon=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Art/RisingStepsIcon.png");if(icon!=null)PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown,new[]{icon});AssetDatabase.SaveAssets();}
 private static void Create(string name,string shader){string path="Assets/Resources/Materials/"+name+".mat";var existing=AssetDatabase.LoadAssetAtPath<Material>(path);var found=Shader.Find(shader);if(found==null)throw new System.Exception("Missing required shader "+shader);if(existing!=null){if(existing.shader!=found){existing.shader=found;EditorUtility.SetDirty(existing);}return;}AssetDatabase.CreateAsset(new Material(found),path);}
}
#endif
