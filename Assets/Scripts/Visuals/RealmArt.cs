using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Perimeter kits are real geometry. The landing fan/collider stays authoritative.
 public static class RealmArt
 {
  public static Color Cliff(int realm)=>realm==1?new Color(.76f,.82f,.81f):realm==2?new Color(.73f,.60f,.45f):new Color(.76f,.77f,.72f);
  public static Color Ground(int realm)=>realm==1?new Color(.81f,.85f,.83f):realm==2?new Color(.78f,.67f,.53f):new Color(.39f,.55f,.34f);
  public static void Dress(Transform parent,int index,int realm,float size,bool scenic)
  {
   if(realm==1)WaterGarden(parent,index,size,scenic);
   if(realm==2)Temple(parent,index,size,scenic);
  }
  static void WaterGarden(Transform parent,int index,float size,bool scenic)
  {
   var water=MeshArt.Mat("Realm1Pool",new Color(.15f,.74f,.80f),3);
   var stone=MeshArt.Stone("Realm1PoolStone",new Color(.79f,.84f,.81f));
   var pool=new GameObject("CascadePool");pool.transform.SetParent(parent,false);pool.transform.localPosition=new Vector3(-size*.89f,.62f,size*.52f);
   MeshArt.Oval(pool.transform,"AzurePool",Vector3.zero,new Vector3(size*.48f,.035f,size*.36f),water,20);
   // Terraced shoulders visibly carry the shallow pool and waterfall source.
   for(int tier=0;tier<3;tier++){
    var ledge=MeshArt.Lathe(pool.transform,"WaterTerrace",new[]{-.30f-tier*.15f,-.13f-tier*.15f,-.10f-tier*.15f},new[]{.36f+tier*.05f,.37f+tier*.05f,.32f+tier*.05f},16,stone,new Vector3(size,1,size*.76f),true);
    ledge.transform.localPosition=Vector3.down*.02f;
   }
   for(int j=0;j<9;j++){float angle=j*Mathf.PI*2/9;MeshArt.Oval(pool.transform,"PoolBank",new Vector3(Mathf.Cos(angle)*size*.23f,.002f,Mathf.Sin(angle)*size*.17f),new Vector3(.15f,.055f,.11f),stone,8);}
   var lily=MeshArt.Mat("Realm1Lily",new Color(.27f,.60f,.27f));var bloom=MeshArt.Mat("Realm1Bloom",new Color(1,.97f,.88f));
   for(int j=0;j<3;j++){
    var p=new Vector3((j-1)*.15f,.026f,(j%2==0?1:-1)*.06f);
    MeshArt.Ring(pool.transform,"WaterRipple",.10f,.007f,.024f,MeshArt.Mat("Realm1Ripple",new Color(.64f,.95f,.92f)),20).transform.localPosition=new Vector3(p.x-.035f,0,p.z);
    var pad=MeshArt.Ring(pool.transform,"LilyPad",.085f,.085f,0,lily,14,325);pad.transform.localPosition=p;pad.transform.localRotation=Quaternion.Euler(0,j*97,0);
    for(int petal=0;petal<6;petal++){float a=petal*Mathf.PI/3;var flower=MeshArt.Oval(pool.transform,"LilyPetal",p+new Vector3(Mathf.Cos(a)*.033f,.023f,Mathf.Sin(a)*.033f),new Vector3(.065f,.035f,.027f),bloom,6);flower.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);}
    MeshArt.Oval(pool.transform,"LilyBloom",p+Vector3.up*.038f,new Vector3(.035f,.028f,.035f),MeshArt.Mat("WaterPollen",new Color(1,.72f,.18f)),6);
   }
   var reed=MeshArt.Mat("Realm1Reed",new Color(.33f,.49f,.23f));var seed=MeshArt.Mat("Realm1ReedSeed",new Color(.50f,.30f,.14f));
   for(int j=0;j<(scenic?12:7);j++){
    float a=1.7f+j*.17f;var p=new Vector3(Mathf.Cos(a)*size*.25f,.03f,Mathf.Sin(a)*size*.19f);float h=.36f+(j%3)*.10f;
    var stem=MeshArt.Box(pool.transform,"ReedStem",p+Vector3.up*h*.5f,new Vector3(.018f,h,.018f),reed);stem.transform.localRotation=Quaternion.Euler(0,j*41,(j%3-1)*7);
    MeshArt.Oval(pool.transform,"ReedHead",p+Vector3.up*h,new Vector3(.042f,.12f,.04f),seed,6);
    var leaf=MeshArt.Box(pool.transform,"ReedLeaf",p+new Vector3(.03f,h*.44f,0),new Vector3(.018f,h*.62f,.048f),reed);leaf.transform.localRotation=Quaternion.Euler(18,j*37,-18);
   }
   IslandArt.Waterfall(parent,pool.transform.localPosition+new Vector3(-size*.18f,-.035f,size*.05f),1);
   if(scenic||index%3==0)IslandArt.Waterfall(parent,new Vector3(size*.83f,.58f,-size*.47f),1);
  }
  static void Temple(Transform parent,int index,float size,bool scenic)
  {
   var stone=MeshArt.Stone("Realm2CarvedStone",new Color(.79f,.68f,.54f));
   var dark=MeshArt.Mat("Realm2TileJoint",new Color(.47f,.30f,.21f));var gold=MeshArt.Mat("Realm2Inlay",new Color(.94f,.65f,.19f));
   // Baked stone texture and flat mortar seams give the existing landing disk paving.
   for(int j=-2;j<=2;j++){
    float x=j*size*.30f;float span=Mathf.Sqrt(size*size*.90f-x*x)*2;
    MeshArt.Box(parent,"TempleTileJoint",new Vector3(x,.602f,0),new Vector3(.012f,.002f,span),dark);
    MeshArt.Box(parent,"TempleTileJoint",new Vector3(0,.602f,x),new Vector3(span,.002f,.012f),dark);
   }
   for(int j=0;j<8;j++){
    float a=j*Mathf.PI/4;var p=new Vector3(Mathf.Cos(a)*size*.90f,.61f,Mathf.Sin(a)*size*.90f);
    var tile=MeshArt.Box(parent,"TemplePaving",p,new Vector3(.40f,.07f,.30f),stone);tile.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);
    var carving=MeshArt.Box(parent,"CarvedFrieze",p+new Vector3(0,-.13f,0),new Vector3(.29f,.12f,.04f),gold);carving.transform.localRotation=tile.transform.localRotation;
    MeshArt.Box(carving.transform,"CarvedDiamond",Vector3.zero,new Vector3(.068f,.068f,.05f),dark).transform.localRotation=Quaternion.Euler(0,0,45);
   }
   for(int j=0;j<2;j++){
    var pillar=new GameObject("TempleColumn");pillar.transform.SetParent(parent,false);pillar.transform.localPosition=new Vector3((j==0?-1:1)*size*.83f,.60f,-size*.48f);
    float h=(scenic?2.25f:1.10f)+(index%3)*.19f;
    MeshArt.Box(pillar.transform,"ColumnFoot",new Vector3(0,.06f,0),new Vector3(.37f,.12f,.37f),stone);
    MeshArt.Lathe(pillar.transform,"FlutedColumn",new[]{.12f,.22f,h*.43f,h-.15f,h},new[]{.17f,.13f,.12f,.13f,.18f},12,stone,Vector3.one,true);
    MeshArt.Box(pillar.transform,"ColumnCapital",new Vector3(0,h,0),new Vector3(.36f,.14f,.36f),stone);
    if(scenic||index%4==0){
     var banner=MeshArt.Mat("TempleBanner",new Color(.39f,.19f,.40f));
     MeshArt.Box(pillar.transform,"BannerArm",new Vector3(.25f,h-.18f,0),new Vector3(.56f,.045f,.045f),gold);
     MeshArt.Mesh(pillar.transform,"TempleBanner",new[]{new Vector3(.15f,h-.20f,0),new Vector3(.51f,h-.20f,0),new Vector3(.51f,h-.95f,.025f),new Vector3(.33f,h-.82f,.04f),new Vector3(.15f,h-.95f,.025f)},new[]{0,1,3,1,2,3,0,3,4,3,1,0,3,2,1,4,3,0},null,banner);
     MeshArt.Box(pillar.transform,"BannerSigil",new Vector3(.33f,h-.47f,-.018f),new Vector3(.10f,.10f,.014f),gold).transform.localRotation=Quaternion.Euler(0,0,45);
    }
    for(int band=0;band<3;band++)MeshArt.Ring(pillar.transform,"ColumnInlay",.165f,.026f,.19f+band*(h-.30f)*.5f,gold,16);
    for(int carving=0;carving<4;carving++){var ornament=MeshArt.Box(pillar.transform,"CapitalCarving",new Vector3(0,h+.02f,-.19f),new Vector3(.065f,.065f,.018f),gold);ornament.transform.localPosition+=Vector3.right*(carving-1.5f)*.08f;ornament.transform.localRotation=Quaternion.Euler(0,0,45);}
   }
   // Small weathered stair flights remain on the perimeter, outside the gold target.
   for(int stair=0;stair<3;stair++)MeshArt.Box(parent,"TempleStair",new Vector3(size*.81f,.48f-stair*.11f,size*(.52f+stair*.10f)),new Vector3(.43f,.12f,.24f),stone);
   if(scenic){
    MeshArt.Box(parent,"RuinedLintel",new Vector3(-size*.20f,3.02f,-size*.48f),new Vector3(size*1.30f,.28f,.36f),stone);
    MeshArt.Box(parent,"BrokenLintel",new Vector3(size*.68f,1.34f,-size*.48f),new Vector3(.65f,.26f,.38f),stone).transform.localRotation=Quaternion.Euler(0,0,-24);
    MeshArt.Box(parent,"LintelInlay",new Vector3(-size*.20f,3.02f,-size*.67f),new Vector3(size*1.2f,.055f,.035f),gold);
    for(int rubble=0;rubble<4;rubble++)MeshArt.Box(parent,"RuinedBlock",new Vector3(-size*.85f+rubble*.17f,.69f,-size*.4f),new Vector3(.25f,.18f,.24f),stone).transform.localRotation=Quaternion.Euler(0,rubble*31,rubble*7);
   }
  }
 }
}
