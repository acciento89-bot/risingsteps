#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Kamilunavo.RisingSteps.Visuals;
public static class TemplePlatformAssetBake
{
 const string Folder="Assets/Resources/Models",Stage="Assets/Editor/TempleAuthoringStage";
 static readonly string[] Files={"TemplePlatforms-v1.asset","TemplePlatform-Regular-v1.prefab","TemplePlatform-Plaza-v1.prefab","TemplePlatform-Destination-v1.prefab"};
 static readonly Color[] Colors={new(.86f,.76f,.60f),new(.94f,.84f,.69f),new(.88f,.64f,.26f),new(.30f,.24f,.19f),new(.39f,.19f,.40f),new(.49f,.25f,.55f),new(1,.65f,.20f),new(.38f,.34f,.20f)};
 public static void Ensure(){
  var materials=new Material[Colors.Length];for(int i=0;i<materials.Length;i++){
   string path="Assets/Resources/Materials/TemplePreview-"+i+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
   var source=Resources.Load<Material>(i<2?"Materials/TempleMasonrySurface":i==6?"Materials/Glow":"Materials/Surface");if(material==null){material=new Material(source){name="TemplePreview-"+i};AssetDatabase.CreateAsset(material,path);}material.shader=source.shader;material.CopyPropertiesFromMaterial(source);material.color=Colors[i];if(i<2){material.mainTexture=Resources.Load<Texture2D>("Art/TempleSandstone-v1");material.mainTextureScale=Vector2.one;}EditorUtility.SetDirty(material);materials[i]=material;
  }AssetDatabase.SaveAssets();
  using var sha=SHA256.Create();var data=new List<byte>();foreach(string file in new[]{"Assets/Scripts/Gameplay/CoursePatterns.cs","Assets/Scripts/Visuals/TempleRouteArt.cs","Assets/Scripts/Visuals/TemplePlatformBindings.cs","Assets/Scripts/Visuals/TemplePlatformLibrary.cs","Assets/Scripts/Visuals/RisingTempleSanctuary.cs","Assets/Scripts/Visuals/LandscapeGeometry.cs","Assets/Scripts/Visuals/MeshArt.cs","Assets/Editor/TemplePlatformAssetBake.cs","Assets/Editor/RunnerMeshOcclusion.cs","Assets/Resources/RisingTempleMasonry.shader","Assets/Resources/Materials/TempleMasonrySurface.mat","Assets/Resources/Art/TempleSandstone-v1.png","Assets/Resources/Art/TempleSandstone-v1.png.meta"}){data.AddRange(Encoding.UTF8.GetBytes(file));data.AddRange(File.ReadAllBytes(file));}string digest=BitConverter.ToString(sha.ComputeHash(data.ToArray())).Replace("-","").ToLowerInvariant();
  var current=AssetDatabase.LoadAssetAtPath<TemplePlatformLibrary>(Folder+"/"+Files[0]);if(current!=null&&current.SourceDigest==digest&&current.Prefabs?.Length==3&&Array.TrueForAll(current.Prefabs,p=>p!=null))return;
  if(AssetDatabase.IsValidFolder(Stage))throw new InvalidOperationException("Retained Temple authoring stage requires recovery before publication");Directory.CreateDirectory(Stage);AssetDatabase.Refresh();Scene scene=default;GameObject host=null;var installed=new List<string>();var backups=new List<(string source,string backup)>();bool published=false,rollbackFailed=false;
  try{
   scene=EditorSceneManager.NewPreviewScene();host=new GameObject("OriginalTempleAuthoring");SceneManager.MoveGameObjectToScene(host,scene);var library=ScriptableObject.CreateInstance<TemplePlatformLibrary>();AssetDatabase.CreateAsset(library,Stage+"/"+Files[0]);var allMeshes=new List<Mesh>();var prefabs=new GameObject[3];
   for(int variant=0;variant<3;variant++){
    var model=TempleRouteArt.Author(host.transform,variant,materials);int vertices=0;
    foreach(var filter in model.GetComponentsInChildren<MeshFilter>()){
     var mesh=UnityEngine.Object.Instantiate(filter.sharedMesh);mesh.name="Temple-"+variant+"-"+allMeshes.Count+"-"+mesh.name;var v=mesh.vertices;var n=mesh.normals;var uv=mesh.uv;
     if(v.Length!=n.Length||uv.Length!=v.Length)throw new InvalidOperationException("Staged Temple primitive lacks UVs or normals: "+filter.name);
     foreach(var point in v)if(!float.IsFinite(point.x)||!float.IsFinite(point.y)||!float.IsFinite(point.z))throw new InvalidOperationException("Invalid staged Temple geometry");foreach(var normal in n)if(normal.sqrMagnitude<.5f)throw new InvalidOperationException("Invalid staged Temple normals");
     vertices+=mesh.vertexCount;AssetDatabase.AddObjectToAsset(mesh,library);allMeshes.Add(mesh);filter.sharedMesh=mesh;
    }
    if(vertices>14000||model.GetComponentsInChildren<Collider>().Length!=0)throw new InvalidOperationException("Original architectural model must fit14k geometry and contain no baked gameplay collider");
    BakeOcclusion(model.transform);Debug.Log("RISING_TEMPLE_VARIANT_BAKED variant="+variant+" vertices="+vertices+" real8ray masonry occlusion");
    foreach(var lifetime in model.GetComponentsInChildren<ArtLifetime>())UnityEngine.Object.DestroyImmediate(lifetime);
    AssetDatabase.SaveAssets();prefabs[variant]=PrefabUtility.SaveAsPrefabAsset(model.gameObject,Stage+"/"+Files[variant+1],out bool success);if(!success||prefabs[variant]==null)throw new InvalidOperationException("Temple prefab publication failed");
   }
   library.SourceDigest=digest;library.Meshes=allMeshes.ToArray();library.Prefabs=prefabs;EditorUtility.SetDirty(library);AssetDatabase.SaveAssets();
   foreach(string file in Files){string final=Folder+"/"+file;if(AssetDatabase.LoadMainAssetAtPath(final)==null)continue;string backup=Stage+"/Previous-"+file;Move(final,backup);backups.Add((final,backup));}
   foreach(string file in Files){string final=Folder+"/"+file;Move(Stage+"/"+file,final);installed.Add(final);}AssetDatabase.SaveAssets();AssetDatabase.Refresh();
   var saved=AssetDatabase.LoadAssetAtPath<TemplePlatformLibrary>(Folder+"/"+Files[0]);if(saved==null||saved.SourceDigest!=digest||saved.Prefabs.Length!=3)throw new InvalidOperationException("Published original Temple library is incomplete");
   foreach(var prefab in saved.Prefabs)foreach(var filter in prefab.GetComponentsInChildren<MeshFilter>())if(AssetDatabase.GetAssetPath(filter.sharedMesh)!=Folder+"/"+Files[0])throw new InvalidOperationException("Temple prefab references a transient mesh");
   published=true;Debug.Log("RISING_TEMPLE_ASSETS_BAKED meshes="+saved.Meshes.Length+" regular4.2 / plaza5.2 / destination6.0 original persistent square architecture digest="+digest);
  }catch{
   foreach(string path in installed)AssetDatabase.DeleteAsset(path);foreach(var pair in backups){string error=AssetDatabase.MoveAsset(pair.backup,pair.source);if(!string.IsNullOrEmpty(error)){rollbackFailed=true;Debug.LogWarning("Retained Temple recovery: "+error);}}throw;
  }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);if(scene.IsValid())EditorSceneManager.ClosePreviewScene(scene);if(published||!rollbackFailed)AssetDatabase.DeleteAsset(Stage);AssetDatabase.SaveAssets();}
 }
 static void Move(string from,string to){string error=AssetDatabase.MoveAsset(from,to);if(!string.IsNullOrEmpty(error))throw new InvalidOperationException(error);}
 static void BakeOcclusion(Transform root)
 {
  const int samples=8;var visibility=new RunnerMeshOcclusion(root);float minimum=1,maximum=0;
  foreach(var filter in root.GetComponentsInChildren<MeshFilter>()){
   var mesh=filter.sharedMesh;var vertices=mesh.vertices;var normals=mesh.normals;var colors=new Color[vertices.Length];
   for(int i=0;i<vertices.Length;i++){
    var normal=filter.transform.TransformDirection(normals[i]).normalized;var tangent=Vector3.Cross(normal,Mathf.Abs(normal.y)>.85f?Vector3.right:Vector3.up).normalized;var bitangent=Vector3.Cross(normal,tangent);var origin=filter.transform.TransformPoint(vertices[i])+normal*.006f;int blocked=0;
    for(int sample=0;sample<samples;sample++){float z=(sample+.5f)/samples,angle=sample*2.39996323f,radius=Mathf.Sqrt(1-z*z);var direction=normal*z+tangent*(Mathf.Cos(angle)*radius)+bitangent*(Mathf.Sin(angle)*radius);if(visibility.Occluded(origin,direction,.45f))blocked++;}
    float alpha=1-.48f*blocked/samples;colors[i]=new Color(1,1,1,alpha);minimum=Mathf.Min(minimum,alpha);maximum=Mathf.Max(maximum,alpha);
   }mesh.colors=colors;EditorUtility.SetDirty(mesh);
  }
  if(minimum>=.99f||maximum<=.99f)throw new InvalidOperationException("Original masonry occlusion must resolve real creases and exposed surfaces");
 }
}
#endif
