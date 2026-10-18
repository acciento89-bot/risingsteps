#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
public static class RisingArtImports
{
 public static void Ensure(){Directory.CreateDirectory("Assets/Resources/Materials");Create("Surface","Standard");Create("Sparkles","Rising/Sparkles");Create("Glow","Standard");var glow=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/Glow.mat");glow.EnableKeyword("_EMISSION");glow.SetColor("_EmissionColor",new Color(2.5f,1.8f,.3f));EditorUtility.SetDirty(glow);
  foreach(string path in new[]{"Assets/Resources/Art/TerrainAtlas.png","Assets/Resources/Art/SkyPanorama.png","Assets/Resources/Art/CloudBank.png"}){var t=AssetImporter.GetAtPath(path) as TextureImporter;if(t==null)continue;t.maxTextureSize=2048;if(path.Contains("CloudBank")){t.alphaIsTransparency=true;t.wrapMode=TextureWrapMode.Clamp;}t.mipmapEnabled=true;t.anisoLevel=4;t.textureCompression=TextureImporterCompression.Compressed;t.wrapMode=path.Contains("Atlas")||path.Contains("CloudBank")?TextureWrapMode.Clamp:TextureWrapMode.Repeat;t.SaveAndReimport();}
  ImportOriginal("Assets/Resources/Art/AuthoredLimestoneAlbedo-v2.png",256,true,true);
  ImportOriginal("Assets/Resources/Art/StratifiedLimestone-v3.png",256,true,true);
  ImportOriginal("Assets/Resources/Art/TempleSandstone-v1.png",256,true,true);
  ImportOriginal("Assets/Resources/Art/RealmMinerals-v1.png",2048,true,false);var realmAtlas=AssetImporter.GetAtPath("Assets/Resources/Art/RealmMinerals-v1.png") as TextureImporter;realmAtlas.npotScale=TextureImporterNPOTScale.None;realmAtlas.SaveAndReimport();
  ImportOriginal("Assets/Resources/Art/SkyAtlasHero-v2.png",1024,false,false);
  ImportOriginal("Assets/Resources/Art/AtlasHero-v3.png",1024,false,false);
  ImportOriginal("Assets/Resources/Art/TempleChapter-v2.png",512,false,false);
  ImportOriginal("Assets/Resources/Art/RealmChapterAtlas-v1.png",1024,false,false);
  ImportOriginal("Assets/Resources/Art/MeadowChapter-v2.png",1024,false,false);
  ImportOriginal("Assets/Resources/Art/CascadeChapter-v2.png",1024,false,false);
  ImportOriginal("Assets/Resources/Art/CanopyLeaves-v1.png",256,true,false);
  ImportOriginal("Assets/Resources/Art/HairFlow-v1.png",64,true,true);
  ImportOriginal("Assets/Resources/Art/ChestnutMicro-v2.png",64,true,true);
  ImportOriginal("Assets/Resources/Art/TravelFabric-v1.png",64,true,true);
  ImportOriginal("Assets/Resources/Art/RisingWordmark-v1.png",512,false,false);
  var wordmark=AssetImporter.GetAtPath("Assets/Resources/Art/RisingWordmark-v1.png") as TextureImporter;wordmark.alphaIsTransparency=true;wordmark.npotScale=TextureImporterNPOTScale.None;wordmark.textureCompression=TextureImporterCompression.Uncompressed;wordmark.SaveAndReimport();
  Create("CharacterSurface","Kamilunavo/RisingCharacter");Create("TempleMasonrySurface","Kamilunavo/RisingTempleMasonry");
  ImportOriginal("Assets/Resources/Art/PoolCaustics-v1.png",256,true,true);
  ImportOriginal("Assets/Resources/Art/CascadeFlow-v1.png",256,true,true);
  Create("WaterSurface","Kamilunavo/RisingWater");var water=AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/WaterSurface.mat");water.EnableKeyword("_NORMALMAP");EditorUtility.SetDirty(water);
  var leaves=AssetImporter.GetAtPath("Assets/Resources/Art/CanopyLeaves-v1.png") as TextureImporter;leaves.alphaIsTransparency=true;leaves.SaveAndReimport();Create("CanopySurface","Kamilunavo/RisingCanopy");
  Create("PortalLight","Kamilunavo/RisingPortal");
  Create("SolarSurface","Kamilunavo/RisingSolar");
  var icon=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Art/RisingStepsIcon.png");if(icon==null)throw new System.InvalidOperationException("Missing approved Rising Steps store icon");PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown,new[]{icon});ConfigureAndroidIcons(icon);AssetDatabase.SaveAssets();TemplePlatformAssetBake.Ensure();NaturalRealmAssetBake.Ensure();RisingRunnerAssetBake.Ensure();}
 private static void ConfigureAndroidIcons(Texture2D artwork)
 {
#if UNITY_ANDROID
  // Empty Android override slots export Unity's logo instead of the default icon.
  // Use the same approved artwork for every launcher shape and density.
  var kind=UnityEditor.Android.AndroidPlatformIconKind.Adaptive;
  var icons=PlayerSettings.GetPlatformIcons(UnityEditor.Build.NamedBuildTarget.Android,kind);
  if(icons.Length==0)throw new System.InvalidOperationException("Android launcher icon slots are unavailable");
  foreach(var slot in icons)slot.SetTextures(new[]{artwork,artwork});
  PlayerSettings.SetPlatformIcons(UnityEditor.Build.NamedBuildTarget.Android,kind,icons);
#endif
 }
 private static void ImportOriginal(string path,int max,bool readable,bool repeat){var t=AssetImporter.GetAtPath(path) as TextureImporter;if(t==null)throw new System.Exception("Missing original artwork "+path);t.maxTextureSize=max;t.textureType=TextureImporterType.Default;t.sRGBTexture=true;t.isReadable=readable;t.mipmapEnabled=true;t.wrapMode=repeat?TextureWrapMode.Repeat:TextureWrapMode.Clamp;t.anisoLevel=2;if(path.Contains("Chapter-")||path.Contains("ChapterAtlas-")||path.Contains("AtlasHero-"))t.npotScale=TextureImporterNPOTScale.None;t.textureCompression=readable?TextureImporterCompression.Uncompressed:TextureImporterCompression.Compressed;t.SaveAndReimport();}
 private static void Create(string name,string shader){string path="Assets/Resources/Materials/"+name+".mat";var existing=AssetDatabase.LoadAssetAtPath<Material>(path);var found=Shader.Find(shader);if(found==null)throw new System.Exception("Missing required shader "+shader);if(existing!=null){if(existing.shader!=found){existing.shader=found;EditorUtility.SetDirty(existing);}return;}AssetDatabase.CreateAsset(new Material(found),path);}
}
#if UNITY_ANDROID
public sealed class RisingAndroidStoreIconFallback : UnityEditor.Android.IPostGenerateGradleAndroidProject
{
 public int callbackOrder=>1000;
 public void OnPostGenerateGradleAndroidProject(string path)
 {
  // Unity 6.6 only exposes adaptive slots, but still emits a legacy fallback PNG.
  // Keep that fallback accurate too, for store tooling and non-adaptive consumers.
  var target=Path.GetFullPath(Path.Combine(path,"..","launcher","src","main","res","mipmap-mdpi","app_icon.png"));
  if(!File.Exists(target))throw new UnityEditor.Build.BuildFailedException("Expected Android launcher fallback icon was not generated");
  File.Copy("Assets/Resources/Art/RisingStepsIcon.png",target,true);
 }
}
#endif
#endif
