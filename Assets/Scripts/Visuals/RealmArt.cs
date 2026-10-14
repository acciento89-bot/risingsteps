using UnityEngine;
using Kamilunavo.RisingSteps.Core;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Perimeter kits are real geometry. The landing fan/collider stays authoritative.
 public static class RealmArt
 {
  public static void BuildBackdrop(Transform parent,int realm)=>RealmLandscapeBackdrop.Build(parent,realm);
  public static Color Cliff(int realm)=>realm==1?new Color(.76f,.82f,.81f):realm==2?new Color(.73f,.60f,.45f):new Color(.76f,.77f,.72f);
  public static Color Ground(int realm)=>realm==1?new Color(.81f,.85f,.83f):realm==2?new Color(.78f,.67f,.53f):new Color(.39f,.55f,.34f);
  public static void Dress(Transform parent,int index,int realm,float size,bool scenic)
  {
   if(realm==1)WaterGarden(parent,index,size,scenic);
   if(realm==2)Temple(parent,index,size,scenic);
   if(realm>=3)RealmLandscapeThemes.Dress(parent,index,realm,size,scenic);
  }
  static void WaterGarden(Transform parent,int index,float size,bool scenic)
  {
   var water=LandscapePalette.Pool(parent);var stone=LandscapePalette.Surface(parent,1);
   var pool=new GameObject("CascadePool");pool.transform.SetParent(parent,false);pool.transform.localPosition=new Vector3(-size*.92f,.64f,size*.68f);
   // The water is exposed above its support, rather than hidden inside three terraces.
   LandscapeGeometry.Rock(pool.transform,"WaterTerrace",new Vector3(.07f,-.25f,.02f),new Vector3(size*1.02f,.36f,size*.84f),stone,index*7);
   var buttress=LandscapeGeometry.Rock(pool.transform,"PoolCliffShoulder",new Vector3(.22f,-.70f,-.11f),new Vector3(size*.65f,.91f,size*.55f),stone,index*7+2);buttress.transform.localRotation=Quaternion.Euler(-8,23,-13);
   const int n=24;var contour=new Vector3[n];var basin=new Vector3[n+1];var basinUv=new Vector2[n+1];var bt=new int[n*3];basin[0]=Vector3.up*.014f;basinUv[0]=Vector2.one*.5f;
   for(int i=0;i<n;i++){
    float a=i*Mathf.PI*2/n,r=1+.045f*Mathf.Sin(i*1.9f+index);
    contour[i]=new Vector3(Mathf.Cos(a)*size*.41f*r,.014f,Mathf.Sin(a)*size*.34f*r);basin[i+1]=contour[i];basinUv[i+1]=new Vector2(contour[i].x,contour[i].z)*.8f+Vector2.one*.5f;
    bt[i*3]=0;bt[i*3+1]=(i+1)%n+1;bt[i*3+2]=i+1;
   }
   MeshArt.Mesh(pool.transform,"AzurePool",basin,bt,basinUv,water);
   // Warm exposed shoals and bright shoreline arcs suggest a shallow stone basin
   // around the darker caustic center; all remain below the decorative bank.
   var shoal=LandscapePalette.Surface(parent,1,5);
   for(int pebble=0;pebble<5;pebble++){
    float a=pebble*1.21f+index*.4f;
    LandscapeGeometry.Rock(pool.transform,"PoolShallowStone",new Vector3(Mathf.Cos(a)*size*.35f,.019f,Mathf.Sin(a)*size*.27f),new Vector3(.15f,.018f,.10f),shoal,index*17+pebble);
   }
   for(int arc=0;arc<3;arc++){
    var ripple=MeshArt.Ring(pool.transform,"PoolCausticArc",size*(.18f+arc*.095f),.004f,.022f,MeshArt.Mat("CascadeCausticLight",new Color(.43f,.83f,.77f)),24,130+arc*17);
    ripple.transform.localRotation=Quaternion.Euler(0,index*29+arc*81,0);ripple.transform.localScale=new Vector3(1,1,.79f);
   }
   var shore=new System.Collections.Generic.List<Vector3>();var st=new System.Collections.Generic.List<int>();var suv=new System.Collections.Generic.List<Vector2>();
   for(int ring=0;ring<3;ring++)for(int i=0;i<=n;i++){
    var p=contour[i%n]*(ring==0?1:1.18f);p.y=ring==0?.026f:ring==1?.067f+.018f*Mathf.Sin((i%n)*2.7f+index):-.09f;
    shore.Add(p);suv.Add(new Vector2(p.x,p.z));
    // This open shoreline faces the actual spillway; no bank crosses the channel.
    if(ring>0&&i<n&&(i<8||i>11)){int k=ring*(n+1)+i,inner=k-(n+1);st.AddRange(new[]{inner,inner+1,k,inner+1,k+1,k});}
   }
   MeshArt.Mesh(pool.transform,"PoolBank",shore.ToArray(),st.ToArray(),suv.ToArray(),stone);
   var outward=new Vector3(pool.transform.localPosition.x,0,pool.transform.localPosition.z).normalized;var across=new Vector3(outward.z,0,-outward.x)*size*.26f;
   var inlet=outward*size*.26f+Vector3.up*.018f;var outlet=outward*size*.52f+Vector3.up*.005f;
   MeshArt.Mesh(pool.transform,"PoolOutlet",new[]{inlet-across,inlet+across,outlet-across,outlet+across},new[]{0,2,1,1,2,3},new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one},water);
   var lily=MeshArt.Mat("Realm1Lily",new Color(.27f,.60f,.27f));var bloom=MeshArt.Mat("Realm1Bloom",new Color(1,.97f,.88f));
   for(int j=0;j<3;j++){
    var p=new Vector3((j-1)*.21f,.027f,(j%2==0?1:-1)*.09f);
    MeshArt.Ring(pool.transform,"WaterRipple",.14f,.006f,.023f,MeshArt.Mat("Realm1Ripple",new Color(.60f,.86f,.80f)),24).transform.localPosition=new Vector3(p.x-.035f,0,p.z);
    var pad=MeshArt.Ring(pool.transform,"LilyPad",.115f,.115f,0,lily,18,325);pad.transform.localPosition=p;pad.transform.localRotation=Quaternion.Euler(0,j*97,0);
    for(int petal=0;petal<6;petal++){float a=petal*Mathf.PI/3;var flower=MeshArt.Oval(pool.transform,"LilyPetal",p+new Vector3(Mathf.Cos(a)*.040f,.023f,Mathf.Sin(a)*.040f),new Vector3(.079f,.035f,.032f),bloom,6);flower.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);}
    MeshArt.Oval(pool.transform,"LilyBloom",p+Vector3.up*.038f,new Vector3(.042f,.028f,.042f),MeshArt.Mat("WaterPollen",new Color(1,.72f,.18f)),6);
   }
   var reed=MeshArt.Mat("Realm1Reed",new Color(.33f,.49f,.23f));var seed=MeshArt.Mat("Realm1ReedSeed",new Color(.50f,.30f,.14f));var blades=new System.Collections.Generic.List<Vector3>();var buv=new System.Collections.Generic.List<Vector2>();var btri=new System.Collections.Generic.List<int>();
   for(int j=0;j<(scenic?12:8);j++){
    float a=-.55f+j*.14f;var p=new Vector3(Mathf.Cos(a)*size*.43f,.039f,Mathf.Sin(a)*size*.35f);float h=.37f+(j%3)*.10f;
    var stem=MeshArt.Box(pool.transform,"ReedStem",p+Vector3.up*h*.5f,new Vector3(.014f,h,.014f),reed);stem.transform.localRotation=Quaternion.Euler(0,j*41,(j%3-1)*7);
    MeshArt.Oval(pool.transform,"ReedHead",p+Vector3.up*h,new Vector3(.037f,.11f,.035f),seed,6);
    LandscapeBotany.Blade(blades,buv,btri,p,a,h*.72f,.042f);LandscapeBotany.Blade(blades,buv,btri,p,a+1.9f,h*.83f,.037f);
   }
   MeshArt.Mesh(pool.transform,"ReedLeaves",blades.ToArray(),btri.ToArray(),buv.ToArray(),reed);
   LandscapeBotany.CascadePlants(parent,index,size);
   if(index%4==0){
    var treePosition=new Vector3(-size*1.83f,.58f,-size*.72f);
    LandscapeGeometry.Rock(parent,"WillowRootLedge",treePosition-Vector3.up*.19f,new Vector3(.70f,.34f,.64f),stone,index*31+7);
    LandscapeBotany.CascadeWillow(parent,treePosition,index,.68f);
   }

   IslandArt.Waterfall(parent,pool.transform.localPosition+outward*size*.52f+Vector3.up*.004f,1,size*.52f);
   if(scenic||index%3==0){
    var feed=new Vector3(size*.84f,.66f,-size*.48f);var flow=new Vector3(feed.x,0,feed.z).normalized;var edge=new Vector3(flow.z,0,-flow.x)*.40f;
    LandscapeGeometry.Rock(parent,"CascadeFeedShoulder",feed-Vector3.up*.23f,new Vector3(.89f,.32f,.69f),stone,index+41);
    MeshArt.Oval(parent,"CascadeFeedBasin",feed,new Vector3(1.02f,.024f,.48f),water,18);
    MeshArt.Mesh(parent,"CascadeFeedSpillway",new[]{feed-edge,feed+edge,feed+flow*.32f-edge,feed+flow*.32f+edge},new[]{0,2,1,1,2,3},new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one},water);
    for(int bank=0;bank<2;bank++)LandscapeGeometry.Rock(parent,"CascadeFeedBank",feed+edge*(bank==0?-1.2f:1.2f)-flow*.02f,new Vector3(.14f,.09f,.30f),stone,index+bank);
    IslandArt.Waterfall(parent,feed+flow*.32f,1,.85f);
   }
  }
  static void Temple(Transform parent,int index,float size,bool scenic)
  {
   var stone=LandscapePalette.Surface(parent,2);
   LandscapeBotany.TempleHerbs(parent,index,size);
   var dark=MeshArt.Mat("Realm2TileJoint",new Color(.47f,.30f,.21f));var gold=MeshArt.Mat("Realm2Inlay",new Color(.94f,.65f,.19f));
   // Staggered independent blocks expose weathered faces below the landing disk.
   // They are render-only perimeter detail: no block changes the authoritative fan.
   for(int course=0;course<2;course++)for(int block=0;block<18;block++){
    float angle=(block+(course%2)*.5f)*Mathf.PI*2/18,r=size*(.925f-course*.055f)*IslandArt.ContourFactor(2,angle);
    var masonry=LandscapeGeometry.BeveledBlock(parent,"TempleMasonryCourse",new Vector3(Mathf.Cos(angle)*r,.37f-course*.255f+.016f*Mathf.Sin(block*2.9f+index),Mathf.Sin(angle)*r),new Vector3(size*.32f,.225f,.24f),stone);
    masonry.transform.localRotation=Quaternion.Euler(0,90-angle*Mathf.Rad2Deg,(block%5-2)*.8f);LandscapeGeometry.StoneUv(masonry);
   }
   // Fine joints and offset weathered tile engraving give the landing disk paving.
   for(int j=-2;j<=2;j++){
    float x=j*size*.30f;float span=Mathf.Sqrt(size*size*.90f-x*x)*2;
    MeshArt.Box(parent,"TempleTileJoint",new Vector3(x,.602f,0),new Vector3(.012f,.002f,span),dark);
    MeshArt.Box(parent,"TempleTileJoint",new Vector3(0,.602f,x),new Vector3(span,.002f,.012f),dark);
   }
   var engraving=MeshArt.Ring(parent,"TempleSeal",size*.30f,.012f,.606f,gold,4);engraving.transform.localRotation=Quaternion.Euler(0,45,0);
   for(int j=0;j<8;j++){
    float a=j*Mathf.PI/4;var p=new Vector3(Mathf.Cos(a)*size*.90f,.61f,Mathf.Sin(a)*size*.90f);
    var tile=MeshArt.Box(parent,"TemplePaving",p,new Vector3(.40f,.07f,.30f),stone);tile.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);LandscapeGeometry.StoneUv(tile);
    var carving=MeshArt.Box(parent,"CarvedFrieze",p+new Vector3(0,-.13f,0),new Vector3(.29f,.12f,.04f),gold);carving.transform.localRotation=tile.transform.localRotation;
    MeshArt.Box(carving.transform,"CarvedDiamond",Vector3.zero,new Vector3(.068f,.068f,.05f),dark).transform.localRotation=Quaternion.Euler(0,0,45);
   }
   for(int j=0;j<2;j++){
    var pillar=new GameObject("TempleColumn");pillar.transform.SetParent(parent,false);pillar.transform.localPosition=new Vector3((j==0?-1:1)*size*.83f,.60f,-size*.48f);
    float h=(scenic?2.25f:1.10f)+(index%3)*.19f;if(j==1&&index%3==1)h*=.56f;
    MeshArt.Box(pillar.transform,"ColumnFoot",new Vector3(0,.06f,0),new Vector3(.37f,.12f,.37f),stone);
    FlutedColumn(pillar.transform,h,stone);
    MeshArt.Box(pillar.transform,"ColumnCapital",new Vector3(0,h,0),new Vector3(.36f,.14f,.36f),stone);
    if(scenic||index%4==0){
     var banner=MeshArt.Mat("TempleBanner",new Color(.39f,.19f,.40f));
     MeshArt.Box(pillar.transform,"BannerArm",new Vector3(.25f,h-.18f,0),new Vector3(.56f,.045f,.045f),gold);
     MeshArt.Mesh(pillar.transform,"TempleBanner",new[]{new Vector3(.15f,h-.20f,0),new Vector3(.51f,h-.20f,0),new Vector3(.51f,h-.95f,.025f),new Vector3(.33f,h-.82f,.04f),new Vector3(.15f,h-.95f,.025f)},new[]{0,1,3,1,2,3,0,3,4,3,1,0,3,2,1,4,3,0},null,banner);
     MeshArt.Box(pillar.transform,"BannerSigil",new Vector3(.33f,h-.47f,-.018f),new Vector3(.10f,.10f,.014f),gold).transform.localRotation=Quaternion.Euler(0,0,45);
    }
    for(int band=0;band<3;band++)MeshArt.Ring(pillar.transform,"ColumnInlay",.165f,.016f,.19f+band*(h-.30f)*.5f,gold,16);
    if(j==1&&index%3==1)pillar.transform.localRotation=Quaternion.Euler(-4,0,7);
    for(int carving=0;carving<4;carving++){var ornament=MeshArt.Box(pillar.transform,"CapitalCarving",new Vector3(0,h+.02f,-.19f),new Vector3(.065f,.065f,.018f),gold);ornament.transform.localPosition+=Vector3.right*(carving-1.5f)*.08f;ornament.transform.localRotation=Quaternion.Euler(0,0,45);}
   }
   if(!scenic&&index%4==0)for(int side=-1;side<=1;side+=2){
    var basePoint=new Vector3(side*size*1.45f,.60f,-size*.38f);
    LandscapeGeometry.BeveledBlock(parent,"TempleTrailBrazierBase",basePoint+Vector3.up*.10f,new Vector3(.28f,.20f,.28f),stone);
    var stem=MeshArt.Lathe(parent,"TempleTrailBrazierStem",new[]{0f,.08f,.38f,.45f},new[]{.09f,.065f,.065f,.12f},12,gold,Vector3.one);stem.transform.localPosition=basePoint+Vector3.up*.20f;
    var bowl=MeshArt.Lathe(parent,"TempleTrailBrazierBowl",new[]{0f,.07f,.12f},new[]{.11f,.18f,.19f},12,gold,Vector3.one);bowl.transform.localPosition=basePoint+Vector3.up*.65f;
    MeshArt.Oval(parent,"TempleTrailFlame",basePoint+new Vector3(0,.90f,0),new Vector3(.18f,.40f,.18f),MeshArt.Mat("PortalFlame",new Color(1,.65f,.20f),-1,true,1.25f),10);
   }
   // Small weathered stair flights remain on the perimeter, outside the gold target.
   for(int stair=0;stair<3;stair++)MeshArt.Box(parent,"TempleStair",new Vector3(size*.81f,.48f-stair*.11f,size*(.52f+stair*.10f)),new Vector3(.43f,.12f,.24f),stone);
   for(int chip=0;chip<4;chip++){float a=(chip*.27f+.30f)*Mathf.PI;LandscapeGeometry.Rock(parent,"TempleWeatheredBlock",new Vector3(Mathf.Cos(a)*size*.92f,.58f,Mathf.Sin(a)*size*.92f),new Vector3(.27f,.14f,.22f),stone,index*13+chip);}
   if(index%3==2){var broken=MeshArt.Lathe(parent,"FallenColumn",new[]{0f,.55f},new[]{.14f,.14f},10,stone,Vector3.one,true);broken.transform.localPosition=new Vector3(-size*.92f,.73f,size*.21f);broken.transform.localRotation=Quaternion.Euler(0,15,86);}
   if(scenic){
    MeshArt.Box(parent,"RuinedLintel",new Vector3(-size*.20f,3.02f,-size*.48f),new Vector3(size*1.30f,.28f,.36f),stone);
    MeshArt.Box(parent,"BrokenLintel",new Vector3(size*.68f,1.34f,-size*.48f),new Vector3(.65f,.26f,.38f),stone).transform.localRotation=Quaternion.Euler(0,0,-24);
    MeshArt.Box(parent,"LintelInlay",new Vector3(-size*.20f,3.02f,-size*.67f),new Vector3(size*1.2f,.055f,.035f),gold);
    for(int rubble=0;rubble<4;rubble++)MeshArt.Box(parent,"RuinedBlock",new Vector3(-size*.85f+rubble*.17f,.69f,-size*.4f),new Vector3(.25f,.18f,.24f),stone).transform.localRotation=Quaternion.Euler(0,rubble*31,rubble*7);
   }
  }
  static void FlutedColumn(Transform parent,float height,Material stone)
  {
   const int sides=24;float[] y={.12f,.22f,height*.46f,height-.15f,height};float[] r={.17f,.13f,.12f,.13f,.18f};
   var v=new System.Collections.Generic.List<Vector3>();var uv=new System.Collections.Generic.List<Vector2>();var t=new System.Collections.Generic.List<int>();
   for(int ring=0;ring<y.Length-1;ring++)for(int side=0;side<sides;side++){
    int start=v.Count;for(int corner=0;corner<4;corner++){
     int j=ring+(corner%2),i=side+(corner/2);float a=i*Mathf.PI*2/sides,groove=1+.085f*Mathf.Cos(i*Mathf.PI);
     v.Add(new Vector3(Mathf.Cos(a)*r[j]*groove,y[j],Mathf.Sin(a)*r[j]*groove));uv.Add(new Vector2((float)i/sides*2,y[j]*.8f));
    }t.AddRange(new[]{start,start+1,start+2,start+2,start+1,start+3});
   }
   MeshArt.Mesh(parent,"FlutedColumn",v.ToArray(),t.ToArray(),uv.ToArray(),stone);
  }

 }
}
