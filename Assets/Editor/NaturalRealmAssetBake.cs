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
public static class NaturalRealmAssetBake
{
 const string Folder="Assets/Resources/Models",Stage="Assets/Editor/NaturalRealmAuthoringStage";
 static readonly string[] Names={"MeadowGarden-v1.prefab","CascadeBlossoms-v1.prefab",null,"JungleOrchids-v1.prefab","CrystalShrine-v1.prefab","FrostSpruce-v1.prefab","DesertCactus-v1.prefab","VolcanoVent-v1.prefab","CoastalBeacon-v1.prefab","StarlitObelisk-v1.prefab"};
 static readonly Color[] Colors={new(.70f,.70f,.66f),new(.86f,.88f,.85f),new(.28f,.65f,.75f),new(.17f,.39f,.19f),new(.48f,.65f,.31f),new(.50f,.35f,.20f),new(.25f,.28f,.30f),new(.66f,.85f,1),new(.97f,.84f,.87f)};
 public static void Ensure()
 {
  var materials=new Material[9];for(int role=0;role<9;role++){string path="Assets/Resources/Materials/NaturalPreview-"+role+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);if(material==null){material=new Material(Resources.Load<Material>("Materials/MineralSurface"));AssetDatabase.CreateAsset(material,path);}material.color=Colors[role];material.SetFloat("_Glossiness",.12f);EditorUtility.SetDirty(material);materials[role]=material;}AssetDatabase.SaveAssets();
  using var hash=SHA256.Create();var data=new List<byte>();foreach(string path in new[]{"Assets/Scripts/Visuals/NaturalRealmArt.cs","Assets/Scripts/Visuals/NaturalRealmBindings.cs","Assets/Scripts/Visuals/NaturalRealmLibrary.cs","Assets/Scripts/Visuals/LandscapeGeometry.cs","Assets/Scripts/Visuals/LandscapeBotany.cs","Assets/Scripts/Visuals/MeshArt.cs","Assets/Editor/NaturalRealmAssetBake.cs","Assets/Editor/RunnerMeshOcclusion.cs","Assets/Resources/RisingMineral.shader"}){data.AddRange(Encoding.UTF8.GetBytes(path));data.AddRange(File.ReadAllBytes(path));}string digest=BitConverter.ToString(hash.ComputeHash(data.ToArray())).Replace("-","").ToLowerInvariant();var current=AssetDatabase.LoadAssetAtPath<NaturalRealmLibrary>(Folder+"/NaturalRealms-v1.asset");if(current!=null&&current.SourceDigest==digest&&current.Prefabs.Length==10)return;
  if(AssetDatabase.IsValidFolder(Stage))throw new InvalidOperationException("Retained natural model authoring stage needs recovery");Directory.CreateDirectory(Stage);AssetDatabase.Refresh();Scene scene=default;GameObject host=null;var installed=new List<string>();var backups=new List<(string file,string backup)>();bool published=false,rollbackFailed=false;
  try{
   scene=EditorSceneManager.NewPreviewScene();host=new GameObject("OriginalNaturalModelsAuthoring");SceneManager.MoveGameObjectToScene(host,scene);var library=ScriptableObject.CreateInstance<NaturalRealmLibrary>();AssetDatabase.CreateAsset(library,Stage+"/NaturalRealms-v1.asset");var meshes=new List<Mesh>();var prefabs=new GameObject[10];var bounds=new Bounds[10];
   for(int realm=0;realm<10;realm++)if(realm!=2){
    var model=NaturalRealmArt.Author(host.transform,realm,materials);int count=0;var visibility=new RunnerMeshOcclusion(model.transform);
    foreach(var filter in model.GetComponentsInChildren<MeshFilter>()){
     var source=filter.sharedMesh;var mesh=UnityEngine.Object.Instantiate(source);mesh.name="Natural-"+realm+"-"+meshes.Count+"-"+source.name;var v=mesh.vertices;var normals=mesh.normals;var colors=new Color[v.Length];
     for(int vertex=0;vertex<v.Length;vertex++){
      var normal=filter.transform.TransformDirection(normals[vertex]).normalized;var tangent=Vector3.Cross(normal,Mathf.Abs(normal.y)>.85f?Vector3.right:Vector3.up).normalized;var bitangent=Vector3.Cross(normal,tangent);var p=filter.transform.TransformPoint(v[vertex])+normal*.002f;int blocked=0;
      for(int sample=0;sample<8;sample++){float z=(sample+.5f)/8,angle=sample*2.39996323f,r=Mathf.Sqrt(1-z*z);if(visibility.Occluded(p,normal*z+(tangent*Mathf.Cos(angle)+bitangent*Mathf.Sin(angle))*r,.35f))blocked++;}colors[vertex]=new Color(1,1,1,1-.48f*blocked/8);
     }
     mesh.colors=colors;mesh.RecalculateTangents();if(mesh.uv.Length!=v.Length||mesh.tangents.Length!=v.Length)throw new InvalidOperationException("Original natural model lighting buffers incomplete");foreach(var p in v)if(!float.IsFinite(p.x)||!float.IsFinite(p.y)||!float.IsFinite(p.z))throw new InvalidOperationException("Natural model coordinate is invalid");foreach(var n in normals)if(n.sqrMagnitude<.5f)throw new InvalidOperationException("Natural model normal is invalid");
     AssetDatabase.AddObjectToAsset(mesh,library);meshes.Add(mesh);filter.sharedMesh=mesh;count+=mesh.vertexCount;
    }
    if(count>(realm==1?512:5000)||model.GetComponentsInChildren<Collider>().Length>0)throw new InvalidOperationException("Original natural model exceeds unchanged geometry/collision budget: "+realm+" vertices="+count);
    bool first=true;foreach(var renderer in model.GetComponentsInChildren<MeshRenderer>()){if(first){bounds[realm]=renderer.bounds;first=false;}else bounds[realm].Encapsulate(renderer.bounds);}foreach(var lifetime in model.GetComponentsInChildren<ArtLifetime>())UnityEngine.Object.DestroyImmediate(lifetime);
    AssetDatabase.SaveAssets();prefabs[realm]=PrefabUtility.SaveAsPrefabAsset(model.gameObject,Stage+"/"+Names[realm],out bool success);if(!success||prefabs[realm]==null)throw new InvalidOperationException("Original natural prefab did not save");Debug.Log("RISING_NATURAL_MODEL_BAKED realm="+realm+" vertices="+count+" genuine8ray mesh occlusion");
   }
   library.SourceDigest=digest;library.Meshes=meshes.ToArray();library.Prefabs=prefabs;library.Bounds=bounds;EditorUtility.SetDirty(library);AssetDatabase.SaveAssets();var files=new List<string>{"NaturalRealms-v1.asset"};foreach(string name in Names)if(name!=null)files.Add(name);
   foreach(string file in files){string final=Folder+"/"+file;if(File.Exists(final)){string backup=Stage+"/Previous-"+file;Move(final,backup);backups.Add((final,backup));}}
   foreach(string file in files){string final=Folder+"/"+file;Move(Stage+"/"+file,final);installed.Add(final);}AssetDatabase.SaveAssets();AssetDatabase.Refresh();var saved=AssetDatabase.LoadAssetAtPath<NaturalRealmLibrary>(Folder+"/NaturalRealms-v1.asset");if(saved==null||saved.SourceDigest!=digest)throw new InvalidOperationException("Original natural model library cannot reload");
   for(int realm=0;realm<10;realm++)if(realm!=2){if(saved.Prefabs[realm]==null||saved.Prefabs[realm].GetComponent<NaturalRealmBindings>()?.Surfaces==null)throw new InvalidOperationException("Original natural prefab bindings lost");foreach(var filter in saved.Prefabs[realm].GetComponentsInChildren<MeshFilter>())if(AssetDatabase.GetAssetPath(filter.sharedMesh)!=Folder+"/NaturalRealms-v1.asset")throw new InvalidOperationException("Natural prefab retained a transient mesh");}published=true;Debug.Log("RISING_NATURAL_MODELS_PASS nine original editable models / meshes="+meshes.Count+" digest="+digest);
  }catch{foreach(string path in installed)AssetDatabase.DeleteAsset(path);foreach(var pair in backups)if(!string.IsNullOrEmpty(AssetDatabase.MoveAsset(pair.backup,pair.file)))rollbackFailed=true;throw;}
  finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);if(scene.IsValid())EditorSceneManager.ClosePreviewScene(scene);if(published||!rollbackFailed)AssetDatabase.DeleteAsset(Stage);AssetDatabase.SaveAssets();}
 }
 static void Move(string from,string to){string error=AssetDatabase.MoveAsset(from,to);if(!string.IsNullOrEmpty(error))throw new InvalidOperationException(error);}
}
#endif
