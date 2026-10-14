#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;
using Kamilunavo.RisingSteps.Gameplay;

// Structural safeguards for the approved Oct10 comparison. Actual screenshot
// resemblance is a separate root-owned graphics gate, never inferred from counts.
public static class RisingGraphicsRefinementValidation
{
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new InvalidOperationException("Rising graphics refinement: "+message);}
 public static void Validate()
 {
  checks=0;int maxIsland=0,maxHorizon=0;var albedo=Resources.Load<Texture2D>("Art/StratifiedLimestone-v3");Check(albedo!=null&&albedo.isReadable&&albedo.width==256&&albedo.height==256&&albedo.wrapMode==TextureWrapMode.Repeat,"original generated limestone albedo uses the retained256square material budget");Check(Resources.Load<Shader>("RisingMineral")?.isSupported==true,"authored mineral shader compiles on the current graphics backend");Check(Resources.Load<Material>("Materials/MineralSurface")?.shader.name=="Kamilunavo/RisingMineral","serialized material retains the explicit linear RGB normal decoder on all targets");
  for(int realm=0;realm<3;realm++)foreach(int index in new[]{4,12}){
   var root=new GameObject("GraphicsRefinementIsland");var owned=new HashSet<UnityEngine.Object>();
   try{
    IslandArt.Build(root.transform,index,realm);
    var mesh=root.transform.Find("LimestoneCliff").GetComponent<MeshFilter>().sharedMesh;
    var points=new HashSet<Vector3>(mesh.vertices);float bottom=0,top=0;var shoulder=new List<Vector3>();
    foreach(var v in points){bottom=Mathf.Min(bottom,v.y);if(v.y>.47f)top=Mathf.Max(top,new Vector2(v.x,v.z).magnitude);}
    foreach(var v in points)if(v.y>bottom*.50f&&v.y<bottom*.30f)shoulder.Add(v);
    var center=Vector3.zero;foreach(var v in shoulder)center+=v;center/=Math.Max(1,shoulder.Count);
    float width=0;foreach(var v in shoulder)width+=new Vector2(v.x-center.x,v.z-center.z).magnitude;
    Check(shoulder.Count>12&&width/shoulder.Count>top*.72f,"upper cliff retains a sheer shoulder instead of a uniform pointed cone");
    Check(root.GetComponentsInChildren<Collider>().Length==1,"only the original landing fan collides");
    var plates=Find(root,"MineralShoulderPlates");Check(plates!=null,"original beveled mineral plates articulate the cliff shoulder");
    Check(plates.GetComponent<Renderer>().bounds.max.y<IslandArt.Surface,"modeled stone remains below the authoritative landing plane");
    var mineral=root.transform.Find("LimestoneCliff").GetComponent<Renderer>().sharedMaterial;
    var normal=mineral.GetTexture("_BumpMap");Check(normal is Texture2D normalTexture&&normal.name=="LandscapeMineralNormal"&&normal.width==256&&normal.height==256&&normalTexture.format==TextureFormat.RGBA32&&normal.graphicsFormat==UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm&&mineral.shader.name=="Kamilunavo/RisingMineral","mineral has an owned original normal-response asset");owned.Add(normal);
    var tangents=mesh.tangents;Check(tangents.Length==mesh.vertexCount,"cliff normal map has tangent coordinates");foreach(var tangent in tangents)Check(float.IsFinite(tangent.x)&&float.IsFinite(tangent.y)&&float.IsFinite(tangent.z)&&new Vector3(tangent.x,tangent.y,tangent.z).sqrMagnitude>.5f,"finite tangent basis");
    if(realm==1){
     var pool=Find(root,"AzurePool");Check(pool!=null&&pool.GetComponent<Renderer>().sharedMaterial.mainTexture?.name=="LandscapePoolCaustics","pool uses the authored shallow-water finish");
     var moss=Find(root,"CascadeHangingMoss");Check(moss!=null&&moss.GetComponent<MeshFilter>().sharedMesh.bounds.size.y>.4f,"moss forms hanging cliff contours below the bank");
     Check(Find(root,"PoolShallowStone")!=null&&Find(root,"PoolCausticArc")!=null,"shoals and interrupted caustics articulate pool depth");
     Check(Find(root,"CascadeFoamThread")!=null,"fall has broken curved foam rather than an unbroken flat curtain");
     Check(Find(root,"CascadeFernFronds")!=null&&Find(root,"CascadeTrailingLeaves")!=null,"lush Cascade includes modeled pinnate ferns and leaf vines");
     var sapling=Find(root,"CascadeWillow");Check(sapling!=null&&Find(root,"WillowFork")!=null,"water ledges carry their own branched willow mesh kit");
     float landingRadius=0;foreach(var vertex in root.GetComponentInChildren<MeshCollider>().sharedMesh.vertices)landingRadius=Mathf.Max(landingRadius,new Vector2(vertex.x,vertex.z).magnitude);
     foreach(var leaf in sapling.GetComponentsInChildren<MeshFilter>())if(leaf.name=="WillowLeafClusters"){
      var crown=leaf.sharedMesh;Check(crown.vertexCount>=576,"willow crown consists of folded leaf surfaces rather than an ellipsoid");
      foreach(var vertex in crown.vertices){var p=root.transform.InverseTransformPoint(leaf.transform.TransformPoint(vertex));Check(new Vector2(p.x,p.z).magnitude>landingRadius,"willow leaves clear the full collision fan");}
     }
     Check(Find(root,"CascadeTrailingLeaves").GetComponent<MeshFilter>().sharedMesh.vertexCount>=3000,"cliff vines carry paired broad leaves instead of isolated small sprigs");
    }
    if(realm==0)Check(Find(root,"DeciduousLeafClusters")!=null&&Find(root,"LeafCrown")==null,"Meadow trees have individually folded clustered leaves");
    if(realm==2){
     int courses=0;foreach(var t in root.GetComponentsInChildren<Transform>())if(t.name=="TempleMasonryCourse"){
      courses++;var bounds=t.GetComponent<Renderer>().bounds;Check(bounds.max.y<.6f,"masonry stays below the landing disk");
     }
     Check(courses==36,"two staggered eighteen-block masonry courses remain visible");
     Check(Find(root,"TempleHerbFlowers")!=null,"dry Temple perimeter has original purple herb florets");
    }
    int count=Capture(root,owned);maxIsland=Math.Max(maxIsland,count);Check(count<30000,"route art stays within the established mobile vertex budget");
   }finally{UnityEngine.Object.DestroyImmediate(root);}
   foreach(var asset in owned)Check(asset==null,"island owns and disposes its generated art");
  }
  for(int realm=0;realm<3;realm++){
   var root=new GameObject("GraphicsRefinementHorizon");var owned=new HashSet<UnityEngine.Object>();
   try{
    RealmArt.BuildBackdrop(root.transform,realm);int count=Capture(root,owned);maxHorizon=Math.Max(maxHorizon,count);
    Check(count<70000&&root.GetComponentsInChildren<Collider>().Length==0,"rich horizon remains bounded render-only scenery");
    bool visibleUpperLayer=false;float lastCampaignZ=float.NegativeInfinity;
    if(realm==2)for(int r=0;r<10;r++)for(int level=0;level<10;level++)foreach(var point in CoursePatterns.LevelPoints(r,level))lastCampaignZ=Mathf.Max(lastCampaignZ,point.z);
    foreach(var renderer in root.GetComponentsInChildren<Renderer>()){
     var bounds=renderer.bounds;// New Temple terraces frame the same x±4.8 protected route as WaterGarden.
     // Only architecture beyond every course can occupy its central horizon.
     Check(realm==2?(bounds.min.x>4.8f||bounds.max.x<-4.8f||bounds.min.z>lastCampaignZ+8):(bounds.min.x>7||bounds.max.x<-7),"midground stays outside the full central route corridor");
     visibleUpperLayer|=bounds.max.y>12&&bounds.center.z<105&&Mathf.Abs(bounds.center.x)<32;
    }
    Check(visibleUpperLayer,"layered scenery rises into view beside the completed climb");
    if(realm==1){
     Check(Find(root,"TerraceLake")!=null&&Find(root,"CascadeCliffVine")!=null,"water horizon has layered basins and vertical vegetation");
     var terrace=Find(root,"CascadeSideTerrace").GetComponent<Renderer>().bounds;
     var lake=Find(root,"TerraceLake").GetComponent<Renderer>().bounds;
     Check(lake.max.y>terrace.max.y,"terrace water is exposed above its real stone support");
     Check(Find(root,"CascadeWillow")!=null&&Find(root,"CascadeRidgeLeafClusters")!=null,"water horizon has original branched trees and leafy ridge silhouettes");
    }
    if(realm==2)Check(Find(root,"SanctuaryArchVoussoir")!=null&&Find(root,"SanctuaryMasonry")!=null,"Temple horizon has architectural volume and articulated masonry");
   }finally{UnityEngine.Object.DestroyImmediate(root);}
   foreach(var asset in owned)Check(asset==null,"horizon owns and disposes its generated art");
  }
  Debug.Log("RISING_GRAPHICS_REFINEMENT_PASS checks="+checks+" maxIsland="+maxIsland+" maxHorizon="+maxHorizon+" actualRenderAcceptance=pending");
 }
 static Transform Find(GameObject root,string name){foreach(var t in root.GetComponentsInChildren<Transform>())if(t.name==name)return t;return null;}
 static int Capture(GameObject root,HashSet<UnityEngine.Object> owned)
 {
  int count=0;var seen=new HashSet<Mesh>();var materials=new HashSet<Material>();var textures=new HashSet<Texture>();
  foreach(var f in root.GetComponentsInChildren<MeshFilter>()){
   var mesh=f.sharedMesh;owned.Add(mesh);if(!seen.Add(mesh))continue;count+=mesh.vertexCount;
   foreach(var v in mesh.vertices)Check(float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z),"finite decorative coordinates");
   foreach(var normal in mesh.normals)Check(normal.sqrMagnitude>.5f,"valid fractured face normals");
  }
  foreach(var r in root.GetComponentsInChildren<Renderer>())foreach(var material in r.sharedMaterials)if(material.name.StartsWith("Landscape")){
   materials.Add(material);owned.Add(material);if(material.mainTexture!=null){textures.Add(material.mainTexture);owned.Add(material.mainTexture);}
   var normal=material.HasProperty("_BumpMap")?material.GetTexture("_BumpMap"):null;if(normal!=null){textures.Add(normal);owned.Add(normal);}
  }
  Check(materials.Count<=12&&textures.Count<=5,"each realm retains the finite palette/texture ceiling");
  foreach(var t in textures)Check(t.width*t.height<=65536,"owned procedural textures stay at most 256 squared");
  return count;
 }
}
#endif
