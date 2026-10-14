#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Kamilunavo.RisingSteps.Visuals;

public static class RisingRunnerAssetBake
{
 const string Folder="Assets/Resources/Models",ModelPath=Folder+"/RisingAdventurer-v2.asset",PrefabPath=Folder+"/RisingAdventurer-v2.prefab";
 const string Stage="Assets/Editor/RunnerAuthoringStage",StageModel=Stage+"/RisingAdventurer-v2.asset",StagePrefab=Stage+"/RisingAdventurer-v2.prefab",OldModel=Stage+"/PreviousModel.asset",OldPrefab=Stage+"/PreviousPrefab.prefab";
 const int Samples=12;
 static string Digest(){
  using var hash=SHA256.Create();var bytes=new List<byte>();
  foreach(string path in new[]{"Assets/Scripts/Visuals/RunnerArt.cs","Assets/Scripts/Visuals/RunnerMeshBuilder.cs","Assets/Scripts/Visuals/RunnerAnimator.cs","Assets/Scripts/Visuals/RisingRunnerModel.cs","Assets/Editor/RisingRunnerAssetBake.cs","Assets/Editor/RunnerMeshOcclusion.cs","Assets/Resources/RisingCharacter.shader","Assets/Resources/Materials/CharacterSurface.mat","Assets/Resources/Art/TravelFabric-v1.png","Assets/Resources/Art/TravelFabric-v1.png.meta","Assets/Resources/Art/ChestnutMicro-v2.png","Assets/Resources/Art/ChestnutMicro-v2.png.meta"}){bytes.AddRange(Encoding.UTF8.GetBytes(path));bytes.AddRange(File.ReadAllBytes(path));}
  return BitConverter.ToString(hash.ComputeHash(bytes.ToArray())).Replace("-","").ToLowerInvariant();
 }
 public static void Ensure(){
  string digest=Digest();var existing=AssetDatabase.LoadAssetAtPath<RisingRunnerModel>(ModelPath);
  if(existing!=null&&existing.SourceDigest==digest&&AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath)!=null)return;
  Directory.CreateDirectory(Folder);if(AssetDatabase.IsValidFolder(Stage))throw new InvalidOperationException("Retained authoring stage needs recovery before replacing a good model");Directory.CreateDirectory(Stage);AssetDatabase.Refresh();bool published=false;Scene scene=default;GameObject host=null;
  try{
   scene=EditorSceneManager.NewPreviewScene();host=new GameObject("OriginalRunnerAuthoringRoot");SceneManager.MoveGameObjectToScene(host,scene);
   var runner=RunnerArt.BuildAuthored(host.transform);var model=ScriptableObject.CreateInstance<RisingRunnerModel>();AssetDatabase.CreateAsset(model,StageModel);
   var meshes=new Dictionary<Mesh,Mesh>();var materials=new Dictionary<Material,Material>();var textures=new Dictionary<Texture,Texture>();int vertices=0,triangles=0;
   foreach(var filter in runner.GetComponentsInChildren<MeshFilter>(true)){
    var original=filter.sharedMesh;if(!meshes.TryGetValue(original,out var mesh)){mesh=UnityEngine.Object.Instantiate(original);mesh.name=original.name;meshes.Add(original,mesh);AssetDatabase.AddObjectToAsset(mesh,model);vertices+=mesh.vertexCount;triangles+=mesh.triangles.Length/3;}filter.sharedMesh=mesh;
   }
   BakeOcclusion(runner);
   float darkest=1,lightest=0;
   foreach(var mesh in meshes.Values){
    var points=mesh.vertices;var normals=mesh.normals;var colors=mesh.colors;var indices=mesh.triangles;
    if(points.Length!=normals.Length||points.Length!=colors.Length)throw new InvalidOperationException("Staged mesh has incomplete lighting buffers");
    for(int i=0;i<points.Length;i++){var p=points[i];var n=normals[i];if(!float.IsFinite(p.x)||!float.IsFinite(p.y)||!float.IsFinite(p.z)||!float.IsFinite(n.x)||!float.IsFinite(n.y)||!float.IsFinite(n.z)||n.sqrMagnitude<.5f||!float.IsFinite(colors[i].a)||colors[i].a<.479f||colors[i].a>1)throw new InvalidOperationException("Staged mesh has invalid geometry or lighting");darkest=Mathf.Min(darkest,colors[i].a);lightest=Mathf.Max(lightest,colors[i].a);}
    for(int i=0;i<indices.Length;i+=3)if(Vector3.Cross(points[indices[i+1]]-points[indices[i]],points[indices[i+2]]-points[indices[i]]).sqrMagnitude<=1e-12f)throw new InvalidOperationException("Staged runner has a degenerate triangle: "+mesh.name+" triangle="+i/3);
   }
   if(darkest>=.99f||lightest<=.99f)throw new InvalidOperationException("Genuine vertex occlusion must resolve both occluded creases and open outer surfaces");
   var originalHoodie=runner.Hoodie;
   foreach(var renderer in runner.GetComponentsInChildren<MeshRenderer>(true)){
    var shared=renderer.sharedMaterials;
    for(int slot=0;slot<shared.Length;slot++){
     var original=shared[slot];if(!materials.TryGetValue(original,out var material)){
      material=new Material(original){name=original.name};materials.Add(original,material);AssetDatabase.AddObjectToAsset(material,model);
      if(original.mainTexture!=null){if(!textures.TryGetValue(original.mainTexture,out var texture)){texture=UnityEngine.Object.Instantiate(original.mainTexture);texture.name=original.mainTexture.name;textures.Add(original.mainTexture,texture);AssetDatabase.AddObjectToAsset(texture,model);}material.mainTexture=texture;}
     }shared[slot]=material;
    }renderer.sharedMaterials=shared;
   }
   runner.Hoodie=materials[originalHoodie];
   // These components still own only the temporary prototype sources, not any
   // persisted sub-assets assigned above. Remove them before prefab serialization.
   foreach(var lifetime in runner.GetComponentsInChildren<ArtLifetime>(true))UnityEngine.Object.DestroyImmediate(lifetime);
   if(vertices>=10000||triangles>=10000||materials.Count>8||textures.Count>3)throw new InvalidOperationException("Authored runner exceeds its unchanged mobile budget: "+vertices+"v "+triangles+"tri "+materials.Count+"m "+textures.Count+"tex");
   model.Vertices=vertices;model.Triangles=triangles;model.OcclusionSamples=Samples;model.OcclusionMinimum=darkest;model.OcclusionMaximum=lightest;EditorUtility.SetDirty(model);AssetDatabase.SaveAssets();
   var saved=PrefabUtility.SaveAsPrefabAsset(runner.gameObject,StagePrefab,out bool success);
   if(!success||saved==null||saved.GetComponent<RunnerAnimator>()?.Hoodie==null||saved.GetComponentsInChildren<Collider>(true).Length!=0||saved.GetComponentsInChildren<ArtLifetime>(true).Length!=0)throw new InvalidOperationException("Original articulated runner prefab did not save cleanly");
   foreach(var filter in saved.GetComponentsInChildren<MeshFilter>(true))if(AssetDatabase.GetAssetPath(filter.sharedMesh)!=StageModel)throw new InvalidOperationException("Prefab must reference genuine persisted authored mesh sub-assets");
   model.SourceDigest=digest;EditorUtility.SetDirty(model);AssetDatabase.SaveAssets();
   Publish(digest);published=true;
   Debug.Log("RISING_ORIGINAL_RUNNER_ASSET_BAKED vertices="+vertices+" triangles="+triangles+" real12ray vertex occlusion / authored articulated prefab / immutable mesh sub-assets");
  }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);if(scene.IsValid())EditorSceneManager.ClosePreviewScene(scene);
   // Failed staging can be removed safely only while no prior asset backup is
   // retained. A failed rollback leaves evidence intact for explicit recovery.
   if(published||!File.Exists(OldModel)&&!File.Exists(OldPrefab))AssetDatabase.DeleteAsset(Stage);
  }
 }
 static void Move(string source,string target){string error=AssetDatabase.MoveAsset(source,target);if(!string.IsNullOrEmpty(error))throw new InvalidOperationException("Runner asset publish: "+error);}
 static void Publish(string digest){
  bool oldModel=false,oldPrefab=false,newModel=false,newPrefab=false;
  try{
   if(File.Exists(ModelPath)){Move(ModelPath,OldModel);oldModel=true;}if(File.Exists(PrefabPath)){Move(PrefabPath,OldPrefab);oldPrefab=true;}
   Move(StageModel,ModelPath);newModel=true;Move(StagePrefab,PrefabPath);newPrefab=true;
   var model=AssetDatabase.LoadAssetAtPath<RisingRunnerModel>(ModelPath);var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
   if(model==null||model.SourceDigest!=digest||prefab==null||prefab.GetComponent<RunnerAnimator>()?.Hoodie==null)throw new InvalidOperationException("Published original runner cannot be loaded");
   foreach(var filter in prefab.GetComponentsInChildren<MeshFilter>(true))if(AssetDatabase.GetAssetPath(filter.sharedMesh)!=ModelPath)throw new InvalidOperationException("Published prefab references a stale mesh container");
  }catch{
   if(newPrefab)Move(PrefabPath,StagePrefab);if(newModel)Move(ModelPath,StageModel);if(oldPrefab)Move(OldPrefab,PrefabPath);if(oldModel)Move(OldModel,ModelPath);throw;
  }
  // Prior good assets are discarded only after both replacement assets reload
  // and every prefab mesh resolves to the fully verified replacement container.
  if(oldPrefab)AssetDatabase.DeleteAsset(OldPrefab);if(oldModel)AssetDatabase.DeleteAsset(OldModel);AssetDatabase.SaveAssets();
 }
 static void BakeOcclusion(RunnerAnimator runner){
  var visibility=new RunnerMeshOcclusion(runner.transform);
  foreach(var filter in runner.GetComponentsInChildren<MeshFilter>(true)){
   var mesh=filter.sharedMesh;var vertices=mesh.vertices;var normals=mesh.normals;var colors=new Color[vertices.Length];
   for(int vertex=0;vertex<vertices.Length;vertex++){
    var normal=filter.transform.TransformDirection(normals[vertex]).normalized;var tangent=Vector3.Cross(normal,Mathf.Abs(normal.y)>.85f?Vector3.right:Vector3.up).normalized;var bitangent=Vector3.Cross(normal,tangent);var origin=filter.transform.TransformPoint(vertices[vertex])+normal*.003f;int blocked=0;
    for(int sample=0;sample<Samples;sample++){float z=(sample+.5f)/Samples,angle=sample*2.39996323f;float radius=Mathf.Sqrt(1-z*z);var direction=normal*z+tangent*(Mathf.Cos(angle)*radius)+bitangent*(Mathf.Sin(angle)*radius);if(visibility.Occluded(origin,direction,.16f))blocked++;}
    colors[vertex]=new Color(1,1,1,1-.52f*blocked/Samples);
   }mesh.colors=colors;EditorUtility.SetDirty(mesh);
  }
 }
}
#endif
