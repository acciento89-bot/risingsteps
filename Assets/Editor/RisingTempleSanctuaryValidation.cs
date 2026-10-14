#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.Core;
public static class RisingTempleSanctuaryValidation
{
 public static void Validate()
 {
  var root=new GameObject("OriginalTempleSanctuaryFixture");var owned=new HashSet<Mesh>();int vertices=0,terraces=0,columns=0,arches=0;
  try{
   RisingTempleSanctuary.Build(root.transform);var kit=root.transform.Find("OriginalTempleSanctuary");Check(kit!=null,"genuine owned sanctuary scene root");
   Check(kit.GetComponentsInChildren<Collider>().Length==0&&kit.GetComponentsInChildren<StepMarker>().Length==0,"scenery adds no playable/colliding surface");
   float lastZ=float.NegativeInfinity;for(int realm=0;realm<10;realm++)for(int level=0;level<10;level++)foreach(var point in CoursePatterns.LevelPoints(realm,level))lastZ=Mathf.Max(lastZ,point.z);
   foreach(var transform in kit.GetComponentsInChildren<Transform>()){if(transform.name=="SanctuaryTerrace")terraces++;if(transform.name=="TempleColumn")columns++;if(transform.name=="SanctuaryArchVoussoir")arches++;}
   Check(terraces==5&&columns>=20&&arches==13,"real terraced masonry, columns and closed segmented arch replace copied islands");
   foreach(var renderer in kit.GetComponentsInChildren<Renderer>()){
    var b=renderer.bounds;Check(b.min.x>4.8f||b.max.x<-4.8f||b.min.z>lastZ+8,"architectural bounds protect every campaign jump: "+renderer.name+" "+b);
   }
   foreach(var filter in kit.GetComponentsInChildren<MeshFilter>()){
    var mesh=filter.sharedMesh;if(!owned.Add(mesh))continue;vertices+=mesh.vertexCount;var points=mesh.vertices;var normals=mesh.normals;
    Check(mesh.uv.Length==points.Length&&normals.Length==points.Length,"real meshes have their own UV and normals: "+filter.name);
    foreach(var point in points)Check(float.IsFinite(point.x)&&float.IsFinite(point.y)&&float.IsFinite(point.z),"finite authored geometry");
    foreach(var normal in normals)Check(float.IsFinite(normal.x)&&normal.sqrMagnitude>.9f,"usable authored surface normals");
    var triangles=mesh.triangles;for(int i=0;i<triangles.Length;i+=3)Check(Vector3.Cross(points[triangles[i+1]]-points[triangles[i]],points[triangles[i+2]]-points[triangles[i]]).sqrMagnitude>1e-18f,"nondegenerate authored triangles");
   }
   Check(vertices<35000,"sanctuary retains bounded35k geometry: "+vertices);
   MeshArt.Batch(root);foreach(var f in root.GetComponentsInChildren<MeshFilter>())owned.Add(f.sharedMesh);
   Debug.Log("RISING_TEMPLE_SANCTUARY_PASS vertices="+vertices+" terraces="+terraces+" columns="+columns+" archStones="+arches+" lastCampaignZ="+lastZ+" protected route / no collisions / original3D scenery");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
  foreach(var mesh in owned)Check(mesh==null,"sanctuary releases every owned mesh");
 }
 static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException("Temple sanctuary: "+message);}
}
#endif
