using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class SkyArt
 {
  public static void Build(Transform parent,int realm){var sky=new Material(Shader.Find("Rising/Sky"));sky.mainTexture=Resources.Load<Texture2D>("Art/SkyPanorama");sky.SetColor("_Tint",realm==1?new Color(.79f,.94f,1):realm==2?new Color(1,.88f,.72f):Color.white);ArtLifetime.Own(parent.gameObject,sky);RenderSettings.skybox=sky;RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.62f,.79f,1);RenderSettings.ambientEquatorColor=new Color(.70f,.72f,.58f);RenderSettings.ambientGroundColor=new Color(.30f,.39f,.46f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogColor=new Color(.75f,.87f,1);RenderSettings.fogDensity=.008f;var sun=GameObject.Find("Sun")?.GetComponent<Light>();if(sun!=null){sun.color=realm==1?new Color(.84f,.94f,1):realm==2?new Color(1,.80f,.55f):new Color(1,.89f,.70f);sun.intensity=realm==1?1.15f:1.35f;}RenderSettings.fogColor=realm==1?new Color(.65f,.86f,.95f):realm==2?new Color(.90f,.82f,.68f):new Color(.75f,.87f,1);
   var cloud=new Material(Shader.Find("Rising/Cloud"));cloud.mainTexture=Resources.Load<Texture2D>("Art/CloudBank");ArtLifetime.Own(parent.gameObject,cloud);
   for(int i=0;i<9;i++){var go=MeshArt.Mesh(parent,"CloudBank"+i,new[]{new Vector3(-.5f,-.5f,0),new Vector3(.5f,-.5f,0),new Vector3(.5f,.5f,0),new Vector3(-.5f,.5f,0)},new[]{0,1,2,0,2,3},new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},cloud);go.transform.position=new Vector3((i%2==0?-1:1)*(7+i%3*3),-8+i*.6f,2+i*7);go.transform.localScale=new Vector3(20+i%3*4,13+i%3*2,1);go.AddComponent<CloudBillboard>();}}
 }
}
