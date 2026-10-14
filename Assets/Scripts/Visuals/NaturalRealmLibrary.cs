using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public sealed class NaturalRealmLibrary:ScriptableObject
 {
  public string SourceDigest;public Mesh[] Meshes;public GameObject[] Prefabs;public Bounds[] Bounds;
  static NaturalRealmLibrary _cached;static System.Collections.Generic.HashSet<Mesh> _borrowed;
  static bool TryLoad(){if(_cached!=null)return true;_cached=Resources.Load<NaturalRealmLibrary>("Models/NaturalRealms-v1");if(_cached==null)return false;_borrowed=new();foreach(var mesh in _cached.Meshes)if(mesh!=null)_borrowed.Add(mesh);return true;}
  public static NaturalRealmLibrary Load(){if(!TryLoad())throw new System.InvalidOperationException("Original natural realm models must be authored in Unity before play");return _cached;}
  public static bool IsTemplateMesh(Object asset)=>asset!=null&&asset is Mesh mesh&&TryLoad()&&_borrowed.Contains(mesh);
 }
}
