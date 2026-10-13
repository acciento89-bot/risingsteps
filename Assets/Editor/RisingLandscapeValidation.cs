#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;

// Run alongside RisingConceptValidation; rendering is a separate visual gate.
public static class RisingLandscapeValidation
{
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("Landscape: "+message);}
 public static void Validate()
 {
  int maxVertices=0;
  for(int realm=0;realm<3;realm++)for(int index=0;index<=12;index++){
   var root=new GameObject("LandscapeProbe");var assets=new HashSet<UnityEngine.Object>();
   try{
    IslandArt.Build(root.transform,index,realm);
    var cliff=root.transform.Find("LimestoneCliff").GetComponent<MeshFilter>().sharedMesh;
    ValidateLandform(cliff,realm,index);
    var cliffMaterial=root.transform.Find("LimestoneCliff").GetComponent<Renderer>().sharedMaterial;
    Check(cliffMaterial.mainTexture!=null&&cliffMaterial.mainTexture.name=="LandscapeMineralAtlas","cliff uses the bounded mineral finish");
    int count=0;foreach(var f in root.GetComponentsInChildren<MeshFilter>()){count+=f.sharedMesh.vertexCount;assets.Add(f.sharedMesh);}
    if(index==12){PortalArt.Build(root.transform,null);count=0;foreach(var f in root.GetComponentsInChildren<MeshFilter>()){count+=f.sharedMesh.vertexCount;assets.Add(f.sharedMesh);}}
    Check(count<30000,"route island and destination stay below the existing vertex ceiling");maxVertices=Math.Max(maxVertices,count);
    Check(root.GetComponentsInChildren<Collider>().Length==1,"the original landing collider is still the only collision");
    RisingWorldRebuildValidation.ValidateLegacySafety(root.GetComponentInChildren<MeshCollider>().sharedMesh,index,realm);
    foreach(var renderer in root.GetComponentsInChildren<Renderer>())if(renderer.sharedMaterial.name.StartsWith("Landscape")){assets.Add(renderer.sharedMaterial);if(renderer.sharedMaterial.mainTexture!=null)assets.Add(renderer.sharedMaterial.mainTexture);}
    MeshArt.Batch(root);foreach(var f in root.GetComponentsInChildren<MeshFilter>())assets.Add(f.sharedMesh);
   }finally{UnityEngine.Object.DestroyImmediate(root);}
   foreach(var asset in assets)Check(asset==null,"generated mesh/material/texture is released with its owner");
  }
  Debug.Log("RISING_LANDSCAPE_PASS maxIslandVertices="+maxVertices+" islands=39");
 }
 // The ten-world contract measures actual mesh topology, not menu names or tint.
 public static void ValidateTenRealms()
 {
  string[] landmarks={"MeadowBlades","CascadePool","TempleColumn","JungleRootMantle","CrystalGeode","FrostIcicle","DesertWindArch","VolcanoLavaChannel","CloudCoastSeaArch","StarlitCrescent"};
  string[] horizons={"MeadowHorizon","CascadeHorizon","TempleHorizon","JungleHorizon","CrystalHorizon","FrostHorizon","DesertHorizon","VolcanoHorizon","CloudCoastHorizon","StarlitHorizon"};
  var signatures=new HashSet<string>();var horizonsSeen=new HashSet<string>();int maxVertices=0,maxBackdrop=0;
  var priorSky=RenderSettings.skybox;var ambientSky=RenderSettings.ambientSkyColor;var ambientEquator=RenderSettings.ambientEquatorColor;var ambientGround=RenderSettings.ambientGroundColor;
  var ambientMode=RenderSettings.ambientMode;float ambientIntensity=RenderSettings.ambientIntensity;var fog=RenderSettings.fog;var fogMode=RenderSettings.fogMode;var fogColor=RenderSettings.fogColor;float fogDensity=RenderSettings.fogDensity;
  var sun=GameObject.Find("Sun")?.GetComponent<Light>();var sunColor=sun!=null?sun.color:Color.white;float sunIntensity=sun!=null?sun.intensity:1;
  try{
   for(int realm=0;realm<10;realm++){
    foreach(int index in new[]{4,12}){
     var root=new GameObject("TenRealmIslandProbe");var owned=new HashSet<UnityEngine.Object>();
     try{
      IslandArt.Build(root.transform,index,realm);if(index==12)PortalArt.Build(root.transform,null);
      Check(Array.Exists(root.GetComponentsInChildren<Transform>(),t=>t.name==landmarks[realm]),"realm "+realm+" owns recognizable actual landmark "+landmarks[realm]);
      int vertices=CaptureAssets(root,owned);maxVertices=Math.Max(vertices,maxVertices);
      Check(vertices<30000,"each new realm stays within the route-island vertex ceiling");
      Check(root.GetComponentsInChildren<Collider>().Length==1,"ten realms have only their original landing collider");
      RisingWorldRebuildValidation.ValidateLegacySafety(root.GetComponentInChildren<MeshCollider>().sharedMesh,index,realm);
      if(index==4)Check(signatures.Add(Topology(root)),"ten worlds have structurally distinct actual landmark geometry");
      MeshArt.Batch(root);CaptureAssets(root,owned);
     }finally{UnityEngine.Object.DestroyImmediate(root);}
     foreach(var asset in owned)Check(asset==null,"ten-realm island assets release with their owner");
    }
   }
   for(int realm=0;realm<10;realm++){
    var root=new GameObject("TenRealmBackdropProbe");var owned=new HashSet<UnityEngine.Object>();
    try{
     SkyArt.Build(root.transform,realm);owned.Add(RenderSettings.skybox);
     Check(Array.Exists(root.GetComponentsInChildren<Transform>(),t=>t.name==horizons[realm]),"realm "+realm+" owns actual horizon landscape "+horizons[realm]);
     Check(root.GetComponentsInChildren<Collider>().Length==0,"background landscape never adds collisions");
     int vertices=CaptureAssets(root,owned);maxBackdrop=Math.Max(vertices,maxBackdrop);Check(vertices<70000,"world backdrop has a finite mobile geometry budget");
     Check(horizonsSeen.Add(Topology(root)),"ten horizons have different actual geometry rather than different colors");
     foreach(var renderer in root.GetComponentsInChildren<Renderer>())if(renderer.sharedMaterial.shader.name=="Rising/Cloud")owned.Add(renderer.sharedMaterial);
     Check(root.GetComponentsInChildren<CloudBillboard>().Length==9,"one bounded cloud bank per active realm");
     MeshArt.Batch(root);CaptureAssets(root,owned);
    }finally{UnityEngine.Object.DestroyImmediate(root);}
    foreach(var asset in owned)Check(asset==null,"ten-realm sky/background assets release with their owner");
   }
  }finally{RenderSettings.skybox=priorSky;RenderSettings.ambientSkyColor=ambientSky;RenderSettings.ambientEquatorColor=ambientEquator;RenderSettings.ambientGroundColor=ambientGround;RenderSettings.ambientMode=ambientMode;RenderSettings.ambientIntensity=ambientIntensity;RenderSettings.fog=fog;RenderSettings.fogMode=fogMode;RenderSettings.fogColor=fogColor;RenderSettings.fogDensity=fogDensity;if(sun!=null){sun.color=sunColor;sun.intensity=sunIntensity;}}
  Debug.Log("RISING_TEN_REALM_ART_PASS distinctLandmarks="+signatures.Count+" distinctHorizons="+horizonsSeen.Count+" maxIslandVertices="+maxVertices+" maxBackdropVertices="+maxBackdrop);
 }
 static int CaptureAssets(GameObject root,HashSet<UnityEngine.Object> owned)
 {
  int count=0;var seenMeshes=new HashSet<Mesh>();var materials=new HashSet<Material>();var textures=new HashSet<Texture>();
  foreach(var f in root.GetComponentsInChildren<MeshFilter>()){
   var mesh=f.sharedMesh;owned.Add(mesh);if(!seenMeshes.Add(mesh))continue;count+=mesh.vertexCount;
   foreach(var v in mesh.vertices)Check(float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z),"finite ten-realm generated coordinates");
   foreach(var normal in mesh.normals)Check(normal.sqrMagnitude>.5f,"valid ten-realm generated normals");
  }
  foreach(var renderer in root.GetComponentsInChildren<Renderer>())if(renderer.sharedMaterial.name.StartsWith("Landscape")){
   var material=renderer.sharedMaterial;materials.Add(material);owned.Add(material);if(material.mainTexture!=null){textures.Add(material.mainTexture);owned.Add(material.mainTexture);}
  }
  Check(materials.Count<=12&&textures.Count<=5,"active realm uses at most twelve palette materials and five textures");
  foreach(var texture in textures)Check(texture.width*texture.height<=65536,"each procedural texture remains at most 256 squared");
  return count;
 }
 static string Topology(GameObject root)
 {
  var parts=new List<string>();foreach(var f in root.GetComponentsInChildren<MeshFilter>())parts.Add(f.sharedMesh.vertexCount+":"+f.sharedMesh.triangles.Length);
  parts.Sort(StringComparer.Ordinal);return string.Join("|",parts);
 }
 // Shape constraints come from the failed actual render. More rings did not make a
 // landform: broad lower bases and complete repeated collars still looked stacked.
 static void ValidateLandform(Mesh mesh,int realm,int index)
 {
  var vertices=mesh.vertices;var triangles=mesh.triangles;var unique=new HashSet<Vector3>(vertices);float bottom=0,topRadius=0;
  foreach(var p in unique){bottom=Mathf.Min(bottom,p.y);if(p.y>.47f)topRadius=Mathf.Max(topRadius,new Vector2(p.x,p.z).magnitude);}
  var low=new List<Vector3>();foreach(var p in unique)if(p.y<bottom*.78f)low.Add(p);
  float lowRadius=0;var lowCenter=Vector3.zero;foreach(var p in low){lowRadius+=new Vector2(p.x,p.z).magnitude;lowCenter+=p;}
  lowRadius/=low.Count;lowCenter/=low.Count;
  Debug.Log("LANDSCAPE_SHAPE realm="+realm+" index="+index+" lowerWidth="+(lowRadius/topRadius).ToString("F3")+" tipOffset="+(new Vector2(lowCenter.x,lowCenter.z).magnitude/topRadius).ToString("F3"));
  // Measure width about the lower centroid; offset does not itself make a wide tip.
  float localWidth=0;foreach(var p in low)localWidth+=new Vector2(p.x-lowCenter.x,p.z-lowCenter.z).magnitude;
  if(realm==1||realm==2)Check(localWidth/low.Count>topRadius*.30f&&localWidth/low.Count<topRadius*.80f,"water and temple retain broad eroded cliff bases rather than identical pointed cones");else Check(localWidth/low.Count<topRadius*.25f,"meadow lower fifth retains its irregular suspended tip");
  Check(new Vector2(lowCenter.x,lowCenter.z).magnitude>topRadius*.18f,"floating rock tip is visibly offset from the landing disk");
  var profile=new List<float>();
  for(int row=0;row<22;row++){
   float y=Mathf.Lerp(.42f,bottom*.74f,row/21f),sum=0;int hits=0;
   for(int direction=0;direction<24;direction++){
    float angle=direction*Mathf.PI/12;float radius=RayRadius(vertices,triangles,new Vector3(0,y,0),new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle)));
    if(radius>0){sum+=radius;hits++;}
   }
   if(hits>=18)profile.Add(sum/hits);
  }
  int widening=0;for(int i=1;i<profile.Count;i++)if(profile[i]-profile[i-1]>topRadius*.016f)widening++;
  Check(widening<=2,"cliff does not repeat complete outward collars down its full height");
 }
 static float RayRadius(Vector3[] vertices,int[] triangles,Vector3 origin,Vector3 direction)
 {
  float furthest=0;
  for(int i=0;i<triangles.Length;i+=3){
   var a=vertices[triangles[i]];var edge1=vertices[triangles[i+1]]-a;var edge2=vertices[triangles[i+2]]-a;
   var p=Vector3.Cross(direction,edge2);float det=Vector3.Dot(edge1,p);if(Mathf.Abs(det)<.000001f)continue;
   float inverse=1/det;var offset=origin-a;float u=Vector3.Dot(offset,p)*inverse;if(u<0||u>1)continue;
   var q=Vector3.Cross(offset,edge1);float v=Vector3.Dot(direction,q)*inverse;if(v<0||u+v>1)continue;
   float distance=Vector3.Dot(edge2,q)*inverse;if(distance>furthest)furthest=distance;
  }
  return furthest;
 }
}
#endif
