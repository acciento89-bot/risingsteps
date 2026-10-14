using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public enum TempleMaterialRole {Stone,Floor,Gold,Shade,Banner,Flower,Fire,Leaf}
 [System.Serializable] public struct TempleSurfaceBinding {public MeshRenderer Renderer;public TempleMaterialRole Role;}
 public sealed class TemplePlatformBindings:MonoBehaviour
 {
  public float Width;public TempleSurfaceBinding[] Surfaces;
  public void Apply(Transform owner){
   foreach(var binding in Surfaces)binding.Renderer.sharedMaterial=binding.Role switch{
    TempleMaterialRole.Stone=>LandscapePalette.TempleMasonry(owner,0),TempleMaterialRole.Floor=>LandscapePalette.TempleMasonry(owner,1),TempleMaterialRole.Gold=>LandscapePalette.Surface(owner,2,5),TempleMaterialRole.Shade=>LandscapePalette.Surface(owner,2,6),
    TempleMaterialRole.Banner=>MeshArt.Mat("TempleBanner",new Color(.39f,.19f,.40f)),TempleMaterialRole.Flower=>MeshArt.Mat("TempleHeatherPetals",new Color(.49f,.25f,.55f)),TempleMaterialRole.Leaf=>LandscapePalette.Surface(owner,2,3),_=>MeshArt.Mat("PortalFlame",new Color(1,.65f,.20f),-1,true,1.25f)};
  }
 }
}
