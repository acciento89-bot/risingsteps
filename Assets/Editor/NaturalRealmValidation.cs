#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kamilunavo.RisingSteps.Visuals;
public static class NaturalRealmValidation
{
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("Natural realm quality: "+message);}
 public static void Validate()
 {
  var library=NaturalRealmLibrary.Load();Check(library.Prefabs.Length==10&&library.Prefabs[2]==null,"nine independent natural model slots preserve existing Temple");var original=Resources.Load<Texture2D>("Art/RealmMinerals-v1");Check(original!=null&&original.isReadable&&original.width==original.height&&original.width%3==0,"original nine-cell source atlas preserves exact tile alignment");
  for(int realm=0;realm<10;realm++)if(realm!=2)foreach(int index in new[]{4,12}){
   var root=new GameObject("NaturalRealmQualityFixture");try{
    IslandArt.Build(root.transform,index,realm);var detail=root.GetComponentInChildren<NaturalRealmBindings>();Check(detail!=null&&detail.Realm==realm,"selected realm owns its genuine original model");var roof=root.GetComponentInChildren<MeshCollider>().bounds;bool right=detail.transform.localPosition.x>0;int vertices=0;float low=1,high=0;
    var shelf=root.transform.Find("NaturalSculptedShelf").GetComponent<MeshRenderer>().bounds;var model=library.Bounds[realm];model.center+=detail.transform.localPosition;
    Check(Mathf.Abs(shelf.max.y-IslandArt.Surface)<.0001f&&shelf.min.x<model.min.x&&shelf.max.x>model.max.x&&shelf.min.z<model.min.z&&shelf.max.z>model.max.z,"the whole original model has a real supporting stone shoulder at its base");Check(right?shelf.min.x<roof.max.x-.10f:shelf.max.x>roof.min.x+.10f,"stone support overlaps the existing cliff instead of floating beside it");
    if(realm==1){var basin=root.transform.Find("CascadeLilyBasin").GetComponent<MeshRenderer>().bounds;Check(basin.min.x<model.min.x&&basin.max.x>model.max.x&&basin.min.z<model.min.z&&basin.max.z>model.max.z&&Mathf.Abs(basin.center.y-IslandArt.Surface)<.001f,"water lilies have their own genuine shallow water basin");}
    foreach(var filter in detail.GetComponentsInChildren<MeshFilter>()){
     var mesh=filter.sharedMesh;vertices+=mesh.vertexCount;Check(AssetDatabase.GetAssetPath(mesh)=="Assets/Resources/Models/NaturalRealms-v1.asset"&&mesh.colors.Length==mesh.vertexCount&&mesh.uv.Length==mesh.vertexCount&&mesh.tangents.Length==mesh.vertexCount,"actual prefab uses immutable persisted original geometry/visibility/UV/tangents");
     foreach(var color in mesh.colors){Check(float.IsFinite(color.a)&&color.a>=.519f&&color.a<=1,"finite real baked8ray visibility");low=Mathf.Min(low,color.a);high=Mathf.Max(high,color.a);}
     var v=mesh.vertices;var triangles=mesh.triangles;foreach(var p in v)Check(float.IsFinite(p.x)&&float.IsFinite(p.y)&&float.IsFinite(p.z),"finite original model shape");for(int t=0;t<triangles.Length;t+=3)Check(Vector3.Cross(v[triangles[t+1]]-v[triangles[t]],v[triangles[t+2]]-v[triangles[t]]).sqrMagnitude>1e-15f,"no degenerate original natural model faces");
    }
    Check(vertices<=(realm==1?512:5000)&&vertices>100,"retained original model geometry limits");Check(low<.99f&&high>.99f,"true occluded creases and open model faces");
    foreach(var renderer in detail.GetComponentsInChildren<MeshRenderer>()){var b=renderer.bounds;Check(right?b.min.x>roof.max.x+.10f:b.max.x<roof.min.x-.10f,"entire original model remains outside the complete real roof");}
    Check(root.GetComponentsInChildren<Collider>().Length==1,"no decorative collision added");var stone=root.transform.Find("LimestoneCliff").GetComponent<MeshRenderer>().sharedMaterial;Check(stone.shader.name=="Kamilunavo/RisingMineral"&&stone.mainTexture?.width==256&&stone.GetTexture("_BumpMap")?.width==256,"real owned256albedo/linear-normal mineral response in every natural realm");
    var textures=new HashSet<Texture>();var materials=new HashSet<Material>();foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>())if(renderer.sharedMaterial.name.StartsWith("Landscape")){var material=renderer.sharedMaterial;materials.Add(material);foreach(string slot in new[]{"_MainTex","_BumpMap"})if(material.HasProperty(slot)&&material.GetTexture(slot) is Texture texture&&texture.width>16)textures.Add(texture);}
    Check(materials.Count<=12&&textures.Count<=5,"actual albedo AND normal textures remain inside original12material/5texture ceiling");MeshArt.Batch(root);
    Debug.Log("RISING_NATURAL_REALM_QUALITY realm="+realm+" step="+index+" modelVertices="+vertices+" palette="+materials.Count+" actualTextures="+textures.Count);
   }finally{UnityEngine.Object.DestroyImmediate(root);}
   foreach(var mesh in library.Meshes)Check(mesh!=null&&AssetDatabase.Contains(mesh),"destroying original/batched scene consumers preserves every immutable model mesh");
  }
  for(int realm=0;realm<10;realm++)if(realm!=1&&realm!=2){var root=new GameObject("NaturalNearBankFixture");try{NaturalRealmLandscape.Build(root.transform,realm);foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>()){var b=renderer.bounds;Check(b.max.x<-4.8f||b.min.x>4.8f,"near-field realm banks retain the entire protected playable corridor");}Check(root.GetComponentsInChildren<Collider>().Length==0,"richer near banks add no colliders");}finally{UnityEngine.Object.DestroyImmediate(root);}}
  var borrowed=library.Meshes[0];var consumer=new GameObject("ExplicitNaturalBorrower");ArtLifetime.Own(consumer,borrowed);UnityEngine.Object.DestroyImmediate(consumer);Check(borrowed!=null,"explicit lifetime registration cannot destroy a shared original model");
  Debug.Log("RISING_NATURAL_REALM_QUALITY_PASS nine original native-bound model subjects / source atlas / genuine visibility / empty landing fan / ownership and retained budgets");
 }
}
#endif
