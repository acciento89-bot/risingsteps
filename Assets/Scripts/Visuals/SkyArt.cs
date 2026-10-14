using UnityEngine;
using Kamilunavo.RisingSteps.Core;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class SkyArt
 {
  static readonly Color[] Horizon={new(.78f,.89f,.97f),new(.77f,.88f,.95f),new(.94f,.77f,.70f),new(.70f,.84f,.67f),new(.40f,.37f,.58f),new(.79f,.86f,.94f),new(.98f,.80f,.57f),new(.75f,.48f,.39f),new(.73f,.92f,.94f),new(.20f,.25f,.41f)};
  static readonly Color[] Zenith={new(.24f,.57f,.84f),new(.30f,.61f,.84f),new(.56f,.43f,.65f),new(.16f,.43f,.38f),new(.13f,.14f,.30f),new(.29f,.50f,.75f),new(.45f,.67f,.82f),new(.31f,.24f,.34f),new(.23f,.64f,.84f),new(.035f,.045f,.13f)};
  static readonly Color[] Below={new(.90f,.96f,1),new(.90f,.96f,1),new(.88f,.76f,.82f),new(.49f,.65f,.43f),new(.26f,.29f,.47f),new(.86f,.94f,1),new(.97f,.83f,.64f),new(.49f,.32f,.30f),new(.59f,.85f,.85f),new(.21f,.27f,.44f)};
  public static void Build(Transform parent,int realm){realm=(int)RisingRealmCatalog.Theme(realm);var sky=new Material(Shader.Find("Rising/Sky")){name="Realm"+realm+"Sky"};
   sky.SetColor("_Horizon",Horizon[realm]);
   sky.SetColor("_Zenith",Zenith[realm]);
   sky.SetColor("_Below",Below[realm]);ArtLifetime.Own(parent.gameObject,sky);RenderSettings.skybox=sky;
   RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientIntensity=.82f;RenderSettings.ambientSkyColor=realm==2?new Color(.47f,.44f,.55f):new Color(.40f,.50f,.60f);RenderSettings.ambientEquatorColor=realm==2?new Color(.57f,.47f,.35f):new Color(.43f,.49f,.45f);RenderSettings.ambientGroundColor=realm==2?new Color(.27f,.25f,.28f):new Color(.28f,.34f,.36f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=realm==2?.009f:realm==1?.010f:.008f;
   var sun=GameObject.Find("Sun")?.GetComponent<Light>();if(sun!=null){sun.color=realm==1?new Color(.96f,.98f,1):realm==2?new Color(1,.88f,.70f):new Color(1,.97f,.91f);sun.intensity=realm==2?.98f:1.04f;sun.transform.rotation=Quaternion.Euler(realm==2?28:42,-28,0);}RenderSettings.fogColor=realm==2?new Color(.91f,.78f,.76f):realm==1?new Color(.79f,.91f,.96f):new Color(.79f,.91f,1);

   if(realm>=3){
    RenderSettings.ambientSkyColor=Color.Lerp(Zenith[realm],new Color(.55f,.62f,.70f),.55f);
    RenderSettings.ambientEquatorColor=Color.Lerp(Horizon[realm],new Color(.45f,.48f,.50f),.55f);
    RenderSettings.ambientGroundColor=Color.Lerp(Below[realm],new Color(.22f,.25f,.29f),.72f);
    RenderSettings.fogColor=Horizon[realm];RenderSettings.fogDensity=realm==4?.0065f:realm==9?.0045f:.0075f;
    if(sun!=null){sun.color=realm==9?new Color(.68f,.79f,1):realm==7?new Color(1,.79f,.59f):realm==6?new Color(1,.92f,.73f):realm==3?new Color(.92f,1,.85f):new Color(.95f,.99f,1);sun.intensity=realm==9?.76f:realm==4?.82f:.92f;}
   }
   var cloud=new Material(Shader.Find("Rising/Cloud"));cloud.mainTexture=Resources.Load<Texture2D>("Art/CloudBank");cloud.SetColor("_Color",realm==9?new Color(.46f,.54f,.76f,.65f):realm==4?new Color(.58f,.57f,.74f,.66f):realm==7?new Color(.77f,.66f,.67f,.80f):realm==2?new Color(.90f,.82f,.91f,.82f):realm==1?new Color(.92f,.97f,1,.84f):new Color(.97f,.99f,1,.86f));ArtLifetime.Own(parent.gameObject,cloud);
   for(int i=0;i<9;i++){var go=MeshArt.Mesh(parent,"CloudBank"+i,new[]{new Vector3(-.5f,-.5f,0),new Vector3(.5f,-.5f,0),new Vector3(.5f,.5f,0),new Vector3(-.5f,.5f,0)},new[]{0,1,2,0,2,3},new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up},cloud);go.transform.position=i<5?new Vector3((i%2==0?-1:1)*(9+i%3*4),-8+i*.6f,6+i*12):new Vector3((i%2==0?-1:1)*(26+i%3*7),17+i%2*7,39+i*10);go.transform.localScale=i<5?new Vector3(23+i%3*4,13+i%3*2,1):new Vector3(28+i%3*6,10+i%2*3,1);go.AddComponent<CloudBillboard>();if(i<5){go.transform.position=new Vector3(i%2==0?-4:8,-10-i*2,40+i*36);go.transform.localScale=new Vector3(46+i*4,14,1);}if(i>=5){int layer=i-5;go.transform.position=new Vector3(layer==0?-15:layer==1?19:layer==2?-8:8,16+layer*4.4f,62+layer*29);go.transform.localScale=new Vector3(29+layer*3,14+layer*2,1);}}
   if(realm==2){var solar=new Material(Resources.Load<Material>("Materials/SolarSurface")){color=new Color(1,.91f,.55f)};ArtLifetime.Own(parent.gameObject,solar);var disk=MeshArt.Oval(parent,"TempleSettingSun",new Vector3(22,25,160),Vector3.one*7,solar,20);disk.GetComponent<Renderer>().shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.Off;disk.GetComponent<Renderer>().receiveShadows=false;}
   RealmArt.BuildBackdrop(parent,realm);}
 }
}
