#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
public static class RisingWaterGardenValidation
{
 public static void Validate(){
  var type=typeof(RisingCourse).Assembly.GetType("Kamilunavo.RisingSteps.Visuals.RisingWaterGarden");if(type==null)throw new InvalidOperationException("Original coherent Waterfall garden kit is missing");
  var root=new GameObject("OriginalWaterGardenFixture");var meshes=new HashSet<Mesh>();try{
   type.GetMethod("Build",BindingFlags.Public|BindingFlags.Static).Invoke(null,new object[]{root.transform});var garden=root.transform.Find("OriginalWaterGarden");if(garden==null)throw new InvalidOperationException("Original water garden has no owned scene root");
   var basin=garden.Find("ValleyBasin").GetComponent<MeshFilter>();var surface=basin.GetComponent<Renderer>();var material=surface.sharedMaterial;if(material.shader.name!="Kamilunavo/RisingWater"||material.mainTexture?.name!="LandscapePoolCaustics")throw new InvalidOperationException("Basin must use its shared original caustic material and genuine water shader");
   var b=surface.bounds;if(b.size.x<9||b.size.z<65||b.max.y>=IslandArtSurface()-.5f)throw new InvalidOperationException("Broad basin must remain below every lowest safe landing");
   if(garden.GetComponentsInChildren<Collider>().Length!=0||garden.GetComponentsInChildren<StepMarker>().Length!=0)throw new InvalidOperationException("Decorative water garden cannot add a playable surface");
   foreach(var renderer in garden.GetComponentsInChildren<Renderer>())if(renderer.name!="ValleyBasin"){var bounds=renderer.bounds;if(!(bounds.min.x>4.8f||bounds.max.x<-4.8f))throw new InvalidOperationException("Bank/flora/waterfall obscures the full route corridor: "+renderer.name+" "+bounds);}
   int vertices=0;foreach(var filter in garden.GetComponentsInChildren<MeshFilter>()){var mesh=filter.sharedMesh;if(meshes.Add(mesh))vertices+=mesh.vertexCount;foreach(var v in mesh.vertices){var p=filter.transform.TransformPoint(v);if(!float.IsFinite(p.x)||!float.IsFinite(p.y)||!float.IsFinite(p.z))throw new InvalidOperationException("Water garden has invalid geometry");}}
   if(vertices>35000)throw new InvalidOperationException("Replacement water garden exceeds its35k extra scene budget: "+vertices);
   int banks=0,trees=0,falls=0;foreach(var t in garden.GetComponentsInChildren<Transform>()){if(t.name=="GardenCliff")banks++;if(t.name=="CascadeWillow")trees++;if(t.name=="Waterfall")falls++;}if(banks<5||trees<5||falls<5)throw new InvalidOperationException("Garden needs actual terraced banks, trees and broad falls rather than a background picture");
   Debug.Log("RISING_WATER_GARDEN_PASS vertices="+vertices+" shared original basin / real terraces and trees / no colliders / below landing floor");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
  foreach(var mesh in meshes)if(mesh!=null)throw new InvalidOperationException("Original water garden did not release its owned mesh");
  ValidateWaterBasis();
 }
 static void ValidateWaterBasis(){
  // Inspect actual unbatched water primitives: combined meshes can hide missing
  // UV/tangent channels by borrowing channels from unrelated stone geometry.
  var fixture=new GameObject("WaterTangentFixture");var owned=new HashSet<Mesh>();int waterMeshes=0;
  try{
   Kamilunavo.RisingSteps.Visuals.RealmArt.Dress(fixture.transform,3,1,1.6f,false);
   foreach(var filter in fixture.GetComponentsInChildren<MeshFilter>()){
    var mesh=filter.sharedMesh;owned.Add(mesh);var material=filter.GetComponent<Renderer>().sharedMaterial;if(material.shader.name!="Kamilunavo/RisingWater")continue;waterMeshes++;
    var uv=mesh.uv;var normals=mesh.normals;var tangents=mesh.tangents;
    if(uv.Length!=mesh.vertexCount||tangents.Length!=mesh.vertexCount)throw new InvalidOperationException("Real water primitive lacks UVs/tangents: "+filter.name);
    for(int i=0;i<mesh.vertexCount;i++){
     var tangent=new Vector3(tangents[i].x,tangents[i].y,tangents[i].z);
     if(!float.IsFinite(uv[i].x)||!float.IsFinite(uv[i].y)||!float.IsFinite(tangent.x)||!float.IsFinite(tangent.y)||!float.IsFinite(tangent.z)||Mathf.Abs(tangents[i].w)!=1||Mathf.Abs(tangent.magnitude-1)>.02f||Vector3.Cross(normals[i],tangent).magnitude<.98f)throw new InvalidOperationException("Invalid water tangent basis: "+filter.name+" vertex "+i);
    }
   }
   if(waterMeshes!=4)throw new InvalidOperationException("Fixture must inspect pool, outlet, feed basin and spillway, observed "+waterMeshes);
   Debug.Log("RISING_WATER_BASIS_PASS four genuine unbatched pool/spill primitives have finite orthogonal UV/tangent frames");
  }finally{UnityEngine.Object.DestroyImmediate(fixture);}
  foreach(var mesh in owned)if(mesh!=null)throw new InvalidOperationException("Water tangent fixture leaked its owned mesh");
 }
 static float IslandArtSurface()=>Kamilunavo.RisingSteps.Visuals.IslandArt.Surface;
}
#endif
