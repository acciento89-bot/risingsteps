using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 [System.Serializable] public struct NaturalSurfaceBinding{public MeshRenderer Renderer;public int Role;}
 public sealed class NaturalRealmBindings:MonoBehaviour
 {
  public int Realm;public NaturalSurfaceBinding[] Surfaces;
  public void Apply(Transform owner){foreach(var binding in Surfaces)binding.Renderer.sharedMaterial=LandscapePalette.Surface(owner,Realm,binding.Role);}
 }
}
