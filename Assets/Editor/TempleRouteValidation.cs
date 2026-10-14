#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;
using Kamilunavo.RisingSteps.Gameplay;
public static class TempleRouteValidation
{
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("Square Temple route: "+message);}
 public static void ValidateSeparation(){
  var roots=new GameObject[13];float narrowest=float.PositiveInfinity;
  try{
   for(int index=0;index<roots.Length;index++){roots[index]=new GameObject("TempleGapFixture"+index);IslandArt.Build(roots[index].transform,index,2);}
   for(int level=-30;level<10;level++){
    var points=level<0?CoursePatterns.Points(2,-level):CoursePatterns.LevelPoints(2,level);
    for(int index=0;index<roots.Length;index++)roots[index].transform.position=points[index];
    for(int index=1;index<roots.Length;index++){
     Bounds BoundsOf(GameObject root){var renderers=root.GetComponentsInChildren<MeshRenderer>();var result=renderers[0].bounds;foreach(var renderer in renderers)result.Encapsulate(renderer.bounds);return result;}
     // Measure complete real authored geometry, including cornices, stairs,
     // banners and herbs; a collider-only gap would still look like a bridge.
     float gap=BoundsOf(roots[index]).min.z-BoundsOf(roots[index-1]).max.z;narrowest=Mathf.Min(narrowest,gap);
     Check(gap>=.90f,"visible open gap between complete Temple models level="+level+" step="+index+" actual="+gap);
    }
   }
  }finally{foreach(var root in roots)if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
  Debug.Log("RISING_TEMPLE_OPEN_GAPS_PASS minimum actual complete-model gap="+narrowest+" ten courses plus30daily seeds /480 adjacent pairs");
 }
 public static void ValidateModel(GameObject root,int index){
  Check(root.transform.Find("LimestoneCliff")==null,"Temple no longer uses a small tapered island body");var model=root.GetComponentInChildren<TemplePlatformBindings>();Check(model!=null&&model.Width==CoursePatterns.TempleRoofWidth(index),"genuine persisted broad architectural model");
  var collider=root.GetComponentInChildren<MeshCollider>();if(collider!=null){var mesh=collider.sharedMesh;float area=0;var v=mesh.vertices;var t=mesh.triangles;for(int i=0;i<t.Length;i+=3)area+=Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).magnitude*.5f;Check(Mathf.Abs(mesh.bounds.size.x-mesh.bounds.size.z)<.0001f&&area/(mesh.bounds.size.x*mesh.bounds.size.z)>.999f,"playable top is genuinely square, not a rounded disk");Check(Mathf.Abs(mesh.bounds.size.x-model.Width)<.0001f,"actual roof and visible prefab footprint agree");}
  int paving=0,masonry=0;foreach(var renderer in model.GetComponentsInChildren<MeshRenderer>()){if(renderer.name=="TemplePaving"){paving++;Check(renderer.bounds.max.y<=root.transform.position.y+IslandArt.Surface+.0001f,"paving top respects the playable plane");}if(renderer.name=="TempleMasonryCourse")masonry++;}
  Check(paving==9&&masonry==48,"original complete square paving and masonry faces exist");
  float darkest=1,lightest=0;Check(model.Surfaces.Length>50,"actual articulated 3D architectural components");foreach(var surface in model.Surfaces)if(surface.Role is TempleMaterialRole.Stone or TempleMaterialRole.Floor){var material=surface.Renderer.sharedMaterial;var mesh=surface.Renderer.GetComponent<MeshFilter>().sharedMesh;Check(material.mainTexture?.name=="TempleSandstoneAtlas"&&material.GetTexture("_BumpMap")?.name=="TempleSandstoneNormal"&&material.shader.name=="Kamilunavo/RisingTempleMasonry","own original sandstone and normal/occlusion response are actually bound");Check(mesh.colors.Length==mesh.vertexCount&&mesh.tangents.Length==mesh.vertexCount,"persisted Temple meshes contain genuine lighting buffers");foreach(var color in mesh.colors){Check(float.IsFinite(color.a)&&color.a>=.519f&&color.a<=1,"finite true authored mesh occlusion");darkest=Mathf.Min(darkest,color.a);lightest=Mathf.Max(lightest,color.a);}}
  Check(darkest<.99f&&lightest>.99f,"real stone creases and open faces have distinct sampled ambient occlusion");
  int gardens=0;foreach(var renderer in model.GetComponentsInChildren<MeshRenderer>())if(renderer.name=="TempleHerbFlowers"||renderer.name=="TempleHerbLeaves"){gardens++;var bounds=renderer.bounds;Check(bounds.min.x>root.transform.position.x+model.Width*.5f||bounds.max.x<root.transform.position.x-model.Width*.5f,"own folded leaves/florets stay outside the entire playable square");}Check(gardens==4,"both architectural edges carry original dry herb geometry");
 }
 public static void Validate(){
  ValidateSeparation();
  for(int index=0;index<=12;index++){var root=new GameObject("SquareTempleFixture");try{IslandArt.Build(root.transform,index,2);ValidateModel(root,index);RisingWorldRebuildValidation.ValidateLegacySafety(root.GetComponentInChildren<MeshCollider>().sharedMesh,index,2);}finally{UnityEngine.Object.DestroyImmediate(root);}}
  // Deliberately register a shared asset, including a renderer that cannot
  // participate in the normal active static batch. Neither consumer may own it.
  var library=TemplePlatformLibrary.Load();var borrowed=library.Meshes[0];var consumer=new GameObject("BorrowedMeshConsumer");ArtLifetime.Own(consumer,borrowed);UnityEngine.Object.DestroyImmediate(consumer);Check(borrowed!=null&&borrowed.vertexCount>0,"explicit ownership registration cannot destroy a persisted shared model mesh");
  var inactive=new GameObject("UncombinedTemplateFixture");var original=UnityEngine.Object.Instantiate(library.Prefabs[0],inactive.transform);foreach(var renderer in original.GetComponentsInChildren<MeshRenderer>())renderer.enabled=false;MeshArt.Batch(inactive);UnityEngine.Object.DestroyImmediate(inactive);foreach(var mesh in library.Meshes)Check(mesh!=null&&mesh.vertexCount>0,"noncombined/disabled template renderers survive scene cleanup");
  Debug.Log("RISING_SQUARE_TEMPLE_PASS actual4.2/5.2/6.0 roofs / persistent original3D masonry and herbs / real8ray occlusion / genuine open gaps without bridging foundation / legacy roof safety / borrowed cleanup");
 }
}
#endif
