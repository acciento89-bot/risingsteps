#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;
public static class RisingStoneWaterValidation
{
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("Rising stone/water finish: "+message);}
 public static void Validate()
 {
  foreach(int index in new[]{4,12}){
   var root=new GameObject("StoneFinishFixture");try{
    IslandArt.Build(root.transform,index,2);var fan=root.GetComponentInChildren<MeshCollider>().sharedMesh;
    Check(root.GetComponentsInChildren<Collider>().Length==1,"only the existing landing fan collides");
    int masonry=0,flames=0;float safeRadius=0;foreach(var p in fan.vertices)safeRadius=Mathf.Max(safeRadius,new Vector2(p.x,p.z).magnitude);
    foreach(var mesh in root.GetComponentsInChildren<MeshFilter>()){
     if(mesh.name=="TempleMasonryCourse"){
      masonry++;var m=mesh.sharedMesh;var half=m.bounds.extents;var edges=new Dictionary<(int,int),int>();double volume=0;var v=m.vertices;var t=m.triangles;
      Check(v.Length<=36,"beveled masonry retains the old per-block vertex allowance");
      foreach(var p in v){int planes=(Mathf.Abs(Mathf.Abs(p.x)-half.x)<.00001f?1:0)+(Mathf.Abs(Mathf.Abs(p.y)-half.y)<.00001f?1:0)+(Mathf.Abs(Mathf.Abs(p.z)-half.z)<.00001f?1:0);Check(planes==1,"each authored bevel corner has one flat-face boundary rather than a raw cube corner");}
      for(int i=0;i<t.Length;i+=3){volume+=Vector3.Dot(v[t[i]],Vector3.Cross(v[t[i+1]],v[t[i+2]]))/6.0;for(int j=0;j<3;j++){int a=t[i+j],b=t[i+(j+1)%3];var key=(Math.Min(a,b),Math.Max(a,b));edges.TryGetValue(key,out int uses);edges[key]=uses+1;}}
      foreach(int uses in edges.Values)Check(uses==2,"masonry is closed with every shared edge used twice");Check(volume>0,"stone faces have positive outward winding");Check(mesh.GetComponent<Renderer>().bounds.max.y<IslandArt.Surface,"all bevel geometry remains below the landing plane");
     }
     if(mesh.name=="TempleTrailFlame") {flames++;var bounds=mesh.GetComponent<Renderer>().bounds;foreach(var corner in new[]{bounds.min,bounds.max,new Vector3(bounds.min.x,0,bounds.max.z),new Vector3(bounds.max.x,0,bounds.min.z)}){var p=root.transform.InverseTransformPoint(corner);Check(new Vector2(p.x,p.z).magnitude>safeRadius,"checkpoint flame clears the full landing fan");}}
    }
    Check(masonry==36&&flames==2,"two real braziers accompany all36 beveled perimeter stones");Check(root.GetComponentsInChildren<Light>().Length==0,"braziers do not add active scene lights");
   }finally{UnityEngine.Object.DestroyImmediate(root);}
  }
  var texture=Resources.Load<Texture2D>("Art/PoolCaustics-v1");Check(texture!=null&&texture.isReadable&&texture.width==256&&texture.height==256&&texture.wrapMode==TextureWrapMode.Repeat,"own basin albedo fits the existing readable256square ceiling");
  Check(Resources.Load<Material>("Materials/SolarSurface")?.shader.name=="Kamilunavo/RisingSolar","serialized fog-free sky sun material retains its compiled shader");
  Debug.Log("RISING_STONE_WATER_FINISH_PASS closed24vertex bevels / safe braziers / original basin albedo");
 }
}
#endif
