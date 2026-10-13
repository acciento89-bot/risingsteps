#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;

public static class RisingWorldRebuildValidation
{
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("World rebuild: "+message);}
 public static void Validate(){
  RisingArtImports.Ensure();
  for(int realm=0;realm<10;realm++)foreach(int index in new[]{4,12}){
   var root=new GameObject("ContourAcceptance");
   try{
    IslandArt.Build(root.transform,index,realm);var mesh=root.GetComponentInChildren<MeshCollider>().sharedMesh;
    ValidateLegacySafety(mesh,index,realm);
    if(realm==2){float area=0;var v=mesh.vertices;var t=mesh.triangles;for(int i=0;i<t.Length;i+=3)area+=Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).magnitude*.5f;Check(area/(mesh.bounds.size.x*mesh.bounds.size.z)>.84f,"Temple cap reads as a rounded square rather than a circular disk");}
   }finally{UnityEngine.Object.DestroyImmediate(root);}
  }
  var portal=new GameObject("PortalAcceptance");try{PortalArt.Build(portal.transform,null);Check(portal.transform.Find("TempleOfLight/PortalRadiance")?.GetComponent<MeshRenderer>()!=null,"portal owns an arched luminous real mesh");Check(portal.GetComponentsInChildren<Light>().Length==1,"one bounded warm cabin-independent portal light");Check(portal.GetComponentsInChildren<Collider>().Length==0,"portal art never adds a collider");}finally{UnityEngine.Object.DestroyImmediate(portal);}
  Debug.Log("RISING_WORLD_REBUILD_PASS legacy landing safety / shaped cap / luminous arch");
 }
 public static void ValidateLegacySafety(Mesh mesh,int index,int realm){
  Check(mesh.vertexCount==25&&mesh.triangles.Length==72,"24-point authoritative fan topology preserved");
  foreach(var vertex in mesh.vertices)Check(Mathf.Abs(vertex.y-.6f)<.00001f,"saved safe-plane elevation preserved");
  var random=new System.Random(117+index*79+realm*1901);float size=index==12?2.05f:1.55f;
  for(int i=0;i<24;i++){float r=size*(.91f+(float)random.NextDouble()*.13f),a=i*Mathf.PI*2/24;Check(Contains(mesh,new Vector2(Mathf.Cos(a)*r,Mathf.Sin(a)*r)),"every original seeded safe boundary point remains inside actual new collision");}
 }
 static bool Contains(Mesh mesh,Vector2 p){var vertices=mesh.vertices;var triangles=mesh.triangles;for(int i=0;i<triangles.Length;i+=3){Vector2 V(int k)=>new(vertices[k].x,vertices[k].z);var a=V(triangles[i]);var b=V(triangles[i+1]);var c=V(triangles[i+2]);float Cross(Vector2 x,Vector2 y)=>x.x*y.y-x.y*y.x;float s=Cross(b-a,p-a),t=Cross(c-b,p-b),u=Cross(a-c,p-c);if((s>=-.00001f&&t>=-.00001f&&u>=-.00001f)||(s<=.00001f&&t<=.00001f&&u<=.00001f))return true;}return false;}
}
#endif
