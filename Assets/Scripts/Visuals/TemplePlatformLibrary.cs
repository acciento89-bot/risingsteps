using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public sealed class TemplePlatformLibrary:ScriptableObject
 {
  public string SourceDigest;public Mesh[] Meshes;public GameObject[] Prefabs;
  static TemplePlatformLibrary _cached;static System.Collections.Generic.HashSet<Mesh> _borrowedMeshes;
  static bool TryLoad(){if(_cached!=null)return true;_cached=Resources.Load<TemplePlatformLibrary>("Models/TemplePlatforms-v1");if(_cached==null)return false;_borrowedMeshes=new();foreach(var mesh in _cached.Meshes)if(mesh!=null)_borrowedMeshes.Add(mesh);return true;}
  public static TemplePlatformLibrary Load(){if(!TryLoad())throw new System.InvalidOperationException("Original Temple platform assets must be baked in Unity before play");return _cached;}
  public static bool IsTemplateMesh(Object asset)=>asset!=null&&asset is Mesh mesh&&TryLoad()&&_borrowedMeshes.Contains(mesh);
 }
}
