#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;

public static class RisingConceptValidation
{
 static int _checks;
 static void Check(bool value,string message){_checks++;if(!value)throw new InvalidOperationException("Concept world: "+message);}
 static bool Has(GameObject root,string name)=>Array.Exists(root.GetComponentsInChildren<Transform>(),t=>t.name==name);
 // The cliff may change below the lip; the original seeded landing boundary may not.
 static void ValidateCliff(GameObject root,int index,int realm){
  var mesh=root.transform.Find("LimestoneCliff").GetComponent<MeshFilter>().sharedMesh;var vertices=mesh.vertices;var normals=mesh.normals;
  var random=new System.Random(117+index*79+realm*1901);float size=index==12?2.05f:1.55f;var radius=new float[24];for(int i=0;i<24;i++)radius[i]=size*(.91f+(float)random.NextDouble()*.13f);
  int upper=0;float sumX=0,sumZ=0;int lower=0;var heights=new HashSet<int>();var faces=new HashSet<string>();
  for(int i=0;i<vertices.Length;i++){var v=vertices[i];var normal=normals[i];Check(float.IsFinite(v.x)&&float.IsFinite(v.y)&&float.IsFinite(v.z),"finite organic cliff coordinates");
   if(v.y>=.4799f){bool found=false;for(int k=0;k<24;k++){float angle=k*Mathf.PI*2/24;var expected=new Vector3(Mathf.Cos(angle)*radius[k]*(v.y>.59f?.99f:1),v.y>.59f?.6f:.48f,Mathf.Sin(angle)*radius[k]*(v.y>.59f?.99f:1));if((v-expected).sqrMagnitude<.000001f){found=true;break;}}Check(found,"both cliff lip rings retain original radius and elevation");upper++;}
   else{sumX+=v.x;sumZ+=v.z;lower++;heights.Add(Mathf.RoundToInt(v.y*1000));faces.Add(Mathf.RoundToInt(normal.x*20)+":"+Mathf.RoundToInt(normal.y*20)+":"+Mathf.RoundToInt(normal.z*20));}
  }
  Check(upper>=48,"original two cliff lip rings exist");Check(lower>0&&new Vector2(sumX/lower,sumZ/lower).magnitude>.025f,"lower rock body has an asymmetric mass rather than a centered cone");
  Check(heights.Count>24,"lower strata have seeded uneven ledge heights");Check(faces.Count>36,"rock strata have varied faceted lighting normals");Check(mesh.vertexCount<2048,"organic rock remains a bounded mobile mesh");
  Check(root.transform.Find("LimestoneCliff").GetComponent<Collider>()==null,"rock shape never owns landing collision");
 }
 public static void Validate(){_checks=0;var priorSky=RenderSettings.skybox;var skyColor=RenderSettings.ambientSkyColor;var equator=RenderSettings.ambientEquatorColor;var ground=RenderSettings.ambientGroundColor;var fog=RenderSettings.fog;var fogMode=RenderSettings.fogMode;var fogColor=RenderSettings.fogColor;var fogDensity=RenderSettings.fogDensity;var ambientMode=RenderSettings.ambientMode;float ambientIntensity=RenderSettings.ambientIntensity;var sun=GameObject.Find("Sun")?.GetComponent<Light>();var sunColor=sun!=null?sun.color:Color.white;float sunIntensity=sun!=null?sun.intensity:1;
  try{for(int realm=0;realm<3;realm++){
   var root=new GameObject("ConceptWorld"+realm);try{
    var marker=IslandArt.Build(root.transform,4,realm);Check(marker.Index==4,"landing progression survives world art");ValidateCliff(root,4,realm);
    Check(root.GetComponentsInChildren<Collider>().Length==1,"route island has exactly its existing landing collider");
    var surface=root.transform.Find(realm==0?"MeadowSurface":realm==1?"CascadeSurface":"TempleSurface").GetComponent<Renderer>().sharedMaterial;
    if(realm!=0){Check(surface.mainTexture!=null&&surface.mainTexture.name=="NeutralBakedStoneDetail","mineral/dry surfaces never reuse painted meadow moss");Check(surface.mainTexture.width*surface.mainTexture.height<=65536,"stone detail remains a small shared mobile material");}
    var collider=root.GetComponentInChildren<MeshCollider>();foreach(var v in collider.sharedMesh.vertices)Check(Mathf.Abs(v.y-IslandArt.Surface)<.0001f,"landing plane remains unchanged");
    Check(!Has(root,"Waterfall")||realm==1,"falls belong to water world only");
    Check(Has(root,"MeadowBlades")== (realm==0),"meadow grass never copied into water/temple");
    Check(Has(root,"DaisyPetal")== (realm==0),"daisies belong to meadow");
    Check(Has(root,"WindTree")== (realm==0),"meadow tree belongs to meadow");
    if(realm==1){Check(Has(root,"ReedStem")&&Has(root,"LilyPad")&&Has(root,"WaterTerrace"),"water has reeds, lilies and physical terraces");Check(Has(root,"CascadePool")&&Has(root,"WaterRibbon"),"water surface and falling curtains exist in 3D");}
    if(realm==2){Check(Has(root,"TemplePaving")&&Has(root,"TempleColumn")&&Has(root,"CarvedFrieze")&&Has(root,"TempleStair"),"temple has actual carved paving, columns and stairs");Check(!Has(root,"CascadePool")&&!Has(root,"LilyPad")&&!Has(root,"WaterRibbon"),"temple is dry architecture");}
    PortalArt.Build(root.transform,null);Check(Has(root,"ArchVoussoir")&&Has(root,"GoldenKeystone"),"destination is thick carved masonry with gold keystone");
    int vertices=0;foreach(var filter in root.GetComponentsInChildren<MeshFilter>()){vertices+=filter.sharedMesh.vertexCount;foreach(var normal in filter.sharedMesh.normals)Check(normal.sqrMagnitude>.5f,"valid generated lighting normals");}
    Check(vertices<30000,"bounded route-island geometry budget");
    var meshes=new HashSet<Mesh>();foreach(var f in root.GetComponentsInChildren<MeshFilter>())meshes.Add(f.sharedMesh);MeshArt.Batch(root);foreach(var f in root.GetComponentsInChildren<MeshFilter>())meshes.Add(f.sharedMesh);UnityEngine.Object.DestroyImmediate(root);foreach(var mesh in meshes)Check(mesh==null,"original and batched meshes released together");
   }finally{if(root!=null)UnityEngine.Object.DestroyImmediate(root);}
   var scenery=new GameObject("ConceptScenery"+realm);try{IslandArt.Build(scenery.transform,4,realm,true);Check(scenery.GetComponentsInChildren<Collider>().Length==0,"distant architecture never collides");SkyArt.Build(scenery.transform,realm);var sky=RenderSettings.skybox;Check(!sky.HasProperty("_MainTex"),"gameplay sky contains no painted course background");Check(sky.HasProperty("_Horizon")&&sky.HasProperty("_Zenith"),"sky owns physical-world gradient palette");var clouds=scenery.GetComponentsInChildren<CloudBillboard>();Check(clouds.Length==9,"one cloud set per world");var cloud=clouds[0].GetComponent<Renderer>().sharedMaterial;UnityEngine.Object.DestroyImmediate(scenery);Check(sky==null&&cloud==null,"sky/cloud instance materials released on world destruction");}finally{if(scenery!=null)UnityEngine.Object.DestroyImmediate(scenery);}
  }}finally{RenderSettings.skybox=priorSky;RenderSettings.ambientSkyColor=skyColor;RenderSettings.ambientEquatorColor=equator;RenderSettings.ambientGroundColor=ground;RenderSettings.ambientMode=ambientMode;RenderSettings.ambientIntensity=ambientIntensity;RenderSettings.fog=fog;RenderSettings.fogMode=fogMode;RenderSettings.fogColor=fogColor;RenderSettings.fogDensity=fogDensity;if(sun!=null){sun.color=sunColor;sun.intensity=sunIntensity;}}
  Debug.Log("RISING_CONCEPT_WORLD_PASS checks="+_checks);
 }
}
#endif
