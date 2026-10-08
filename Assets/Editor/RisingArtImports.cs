#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
public static class RisingArtImports
{
 public static void Ensure(){Directory.CreateDirectory("Assets/Resources/Materials");Create("Surface","Standard");Create("Sparkles","Particles/Standard Unlit");
  foreach(string path in new[]{"Assets/Resources/Art/TerrainAtlas.png","Assets/Resources/Art/SkyPanorama.png"}){var t=AssetImporter.GetAtPath(path) as TextureImporter;if(t==null)continue;t.maxTextureSize=2048;t.mipmapEnabled=true;t.anisoLevel=4;t.textureCompression=TextureImporterCompression.Compressed;t.wrapMode=path.Contains("Atlas")?TextureWrapMode.Clamp:TextureWrapMode.Repeat;t.SaveAndReimport();}AssetDatabase.SaveAssets();}
 private static void Create(string name,string shader){string path="Assets/Resources/Materials/"+name+".mat";if(AssetDatabase.LoadAssetAtPath<Material>(path)!=null)return;var found=Shader.Find(shader);if(found==null)throw new System.Exception("Missing required shader "+shader);AssetDatabase.CreateAsset(new Material(found),path);}
}
#endif
