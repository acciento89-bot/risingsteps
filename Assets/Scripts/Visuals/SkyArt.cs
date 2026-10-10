using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class SkyArt
 {
  public static void Build(Transform parent,int realm){var sky=new Material(Shader.Find("Rising/Sky")){name="Realm"+realm+"Sky"};
   sky.SetColor("_Horizon",realm==2?new Color(.88f,.72f,.73f):realm==1?new Color(.77f,.88f,.95f):new Color(.78f,.89f,.97f));
   sky.SetColor("_Zenith",realm==2?new Color(.46f,.43f,.64f):realm==1?new Color(.36f,.59f,.80f):new Color(.28f,.53f,.81f));
   sky.SetColor("_Below",realm==2?new Color(.88f,.76f,.82f):new Color(.90f,.96f,1));ArtLifetime.Own(parent.gameObject,sky);RenderSettings.skybox=sky;
   RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientIntensity=.82f;RenderSettings.ambientSkyColor=realm==2?new Color(.47f,.44f,.55f):new Color(.45f,.55f,.65f);RenderSettings.ambientEquatorColor=realm==2?new Color(.51f,.46f,.40f):new Color(.50f,.55f,.51f);RenderSettings.ambientGroundColor=realm==2?new Color(.27f,.25f,.28f):new Color(.28f,.34f,.36f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=realm==2?.009f:realm==1?.010f:.008f;
   var sun=GameObject.Find("Sun")?.GetComponent<Light>();if(sun!=null){sun.color=realm==1?new Color(.94f,.98f,1):realm==2?new Color(1,.91f,.80f):new Color(1,.97f,.91f);sun.intensity=realm==2?.88f:.92f;}RenderSettings.fogColor=realm==2?new Color(.87f,.75f,.82f):realm==1?new Color(.79f,.91f,.96f):new Color(.79f,.91f,1);

   var cloud=new Material(Shader.Find("Rising/Cloud"));cloud.mainTexture=Resources.Load<Texture2D>("Art/CloudBank");cloud.SetColor("_Color",realm==2?new Color(.90f,.82f,.91f,.82f):realm==1?new Color(.92f,.97f,1,.84f):new Color(.97f,.99f,1,.86f));ArtLifetime.Own(parent.gameObject,cloud);
   for(int i=0;i<9;i++){var go=MeshArt.Mesh(parent,"CloudBank"+i,new[]{new Vector3(-.5f,-.5f,0),new Vector3(.5f,-.5f,0),new Vector3(.5f,.5f,0),new Vector3(-.5f,.5f,0)},new[]{0,1,2,0,2,3},new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},cloud);go.transform.position=i<5?new Vector3((i%2==0?-1:1)*(9+i%3*4),-8+i*.6f,6+i*12):new Vector3((i%2==0?-1:1)*(26+i%3*7),14+i%2*8,48+i*9);go.transform.localScale=i<5?new Vector3(23+i%3*4,13+i%3*2,1):new Vector3(24+i%3*5,9+i%2*3,1);go.AddComponent<CloudBillboard>();}}
 }
}
