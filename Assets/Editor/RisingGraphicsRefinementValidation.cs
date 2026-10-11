#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;

// Structural safeguards for the approved Oct10 comparison. Actual screenshot
// resemblance is a separate root-owned graphics gate, never inferred from counts.
public static class RisingGraphicsRefinementValidation
{
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new InvalidOperationException("Rising graphics refinement: "+message);}
 public static void Validate()
 {
  checks=0;int maxIsland=0,maxHorizon=0;
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
    if(realm==1){
     var pool=Find(root,"AzurePool");Check(pool!=null&&pool.GetComponent<Renderer>().sharedMaterial.mainTexture?.name=="LandscapePoolCaustics","pool uses the authored shallow-water finish");
     var moss=Find(root,"CascadeHangingMoss");Check(moss!=null&&moss.GetComponent<MeshFilter>().sharedMesh.bounds.size.y>.4f,"moss forms hanging cliff contours below the bank");
     Check(Find(root,"PoolShallowStone")!=null&&Find(root,"PoolCausticArc")!=null,"shoals and interrupted caustics articulate pool depth");
     Check(Find(root,"CascadeFoamThread")!=null,"fall has broken curved foam rather than an unbroken flat curtain");
    }
    if(realm==2){
     int courses=0;foreach(var t in root.GetComponentsInChildren<Transform>())if(t.name=="TempleMasonryCourse"){
      courses++;var bounds=t.GetComponent<Renderer>().bounds;Check(bounds.max.y<.6f,"masonry stays below the landing disk");
     }
     Check(courses==36,"two staggered eighteen-block masonry courses remain visible");
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
    bool visibleUpperLayer=false;
    foreach(var renderer in root.GetComponentsInChildren<Renderer>()){
     var bounds=renderer.bounds;Check(bounds.min.x>7||bounds.max.x<-7,"midground stays outside the full central route corridor");
     visibleUpperLayer|=bounds.max.y>12&&bounds.center.z<105&&Mathf.Abs(bounds.center.x)<32;
    }
    Check(visibleUpperLayer,"layered scenery rises into view beside the completed climb");
    if(realm==1){
     Check(Find(root,"TerraceLake")!=null&&Find(root,"CascadeCliffVine")!=null,"water horizon has layered basins and vertical vegetation");
     var terrace=Find(root,"CascadeSideTerrace").GetComponent<Renderer>().bounds;
     var lake=Find(root,"TerraceLake").GetComponent<Renderer>().bounds;
     Check(lake.max.y>terrace.max.y,"terrace water is exposed above its real stone support");
    }
    if(realm==2)Check(Find(root,"SanctuaryPediment")!=null&&Find(root,"AcropolisMasonry")!=null,"Temple horizon has architectural volume and articulated masonry");
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
  }
  Check(materials.Count<=12&&textures.Count<=5,"each realm retains the finite palette/texture ceiling");
  foreach(var t in textures)Check(t.width*t.height<=65536,"owned procedural textures stay at most 256 squared");
  return count;
 }
}
#endif
