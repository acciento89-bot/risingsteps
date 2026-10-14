using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Runtime meshes and instance materials belong to the scene objects that create them.
 [ExecuteAlways] public sealed class ArtLifetime:MonoBehaviour
 {
  private readonly HashSet<Object> _owned=new();
  public static void Own(GameObject owner,Object asset){if(asset==null||IsBorrowed(asset))return;var lifetime=owner.GetComponent<ArtLifetime>()??owner.AddComponent<ArtLifetime>();lifetime._owned.Add(asset);}
  public static bool IsBorrowed(Object asset)=>asset is Mesh&&(TemplePlatformLibrary.IsTemplateMesh(asset)||NaturalRealmLibrary.IsTemplateMesh(asset));
  private void OnDestroy(){foreach(var asset in _owned)if(asset!=null){if(Application.isPlaying)Destroy(asset);else DestroyImmediate(asset);}_owned.Clear();}
 }
}
