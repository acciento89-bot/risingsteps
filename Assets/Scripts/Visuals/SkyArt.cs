using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class SkyArt
 {
  public static void Build(){var sky=new Material(Shader.Find("Rising/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/SkyPanorama");RenderSettings.skybox=sky;RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.62f,.79f,1);RenderSettings.ambientEquatorColor=new Color(.70f,.72f,.58f);RenderSettings.ambientGroundColor=new Color(.30f,.39f,.46f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.75f,.87f,1);RenderSettings.fogDensity=.008f;
   var cloud=new Material(Shader.Find("Rising/Cloud"));cloud.mainTexture=Resources.Load<Texture2D>("Art/CloudBank");
   for(int i=0;i<9;i++){var go=MeshArt.Mesh(null,"CloudBank"+i,new[]{new Vector3(-.5f,-.5f,0),new Vector3(.5f,-.5f,0),new Vector3(.5f,.5f,0),new Vector3(-.5f,.5f,0)},new[]{0,1,2,0,2,3},new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},cloud);go.transform.position=new Vector3((i%2==0?-1:1)*(7+i%3*3),-8+i*.6f,2+i*7);go.transform.localScale=new Vector3(20+i%3*4,13+i%3*2,1);go.AddComponent<CloudBillboard>();}}
 }
}
