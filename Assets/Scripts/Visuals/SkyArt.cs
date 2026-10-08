using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class SkyArt
 {
  public static void Build(){var sky=new Material(Shader.Find("Rising/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/SkyPanorama");RenderSettings.skybox=sky;RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.62f,.79f,1);RenderSettings.ambientEquatorColor=new Color(.70f,.72f,.58f);RenderSettings.ambientGroundColor=new Color(.30f,.39f,.46f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.75f,.87f,1);RenderSettings.fogDensity=.008f;}
 }
}
