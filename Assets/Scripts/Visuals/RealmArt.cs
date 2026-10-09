using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Decorative geometry stays outside the landing rim; course colliders never change.
 public static class RealmArt
 {
  public static Color Cliff(int realm)=>realm==1?new Color(.47f,.66f,.73f):realm==2?new Color(.88f,.70f,.43f):new Color(.88f,.86f,.78f);
  public static Color Ground(int realm)=>realm==1?new Color(.30f,.64f,.53f):realm==2?new Color(.96f,.84f,.61f):new Color(.66f,.87f,.54f);
  public static void Dress(Transform parent,int index,int realm,float size,bool scenic)
  {
   if(realm==1){
    var water=MeshArt.Mat("Realm1Pool",new Color(.16f,.65f,.79f),3);
    var lip=MeshArt.Mat("Realm1PoolStone",new Color(.39f,.63f,.64f),1);
    var pool=new GameObject("CascadePool");pool.transform.SetParent(parent,false);pool.transform.localPosition=new Vector3(-size*.71f,.63f,size*.48f);
    MeshArt.Oval(pool.transform,"AzurePool",Vector3.zero,new Vector3(size*.65f,.035f,size*.38f),water,18);
    for(int j=0;j<7;j++){float angle=j*Mathf.PI*2/7;MeshArt.Oval(pool.transform,"PoolBank",new Vector3(Mathf.Cos(angle)*size*.3f,.012f,Mathf.Sin(angle)*size*.18f),new Vector3(.22f,.08f,.16f),lip,8);}
    MeshArt.Oval(pool.transform,"LilyPad",new Vector3(.16f,.035f,0),new Vector3(.19f,.018f,.17f),MeshArt.Mat("Realm1Lily",new Color(.33f,.76f,.40f)),8);
    MeshArt.Oval(pool.transform,"LilyBloom",new Vector3(.16f,.055f,0),new Vector3(.065f,.065f,.065f),MeshArt.Mat("Realm1Bloom",new Color(1,.79f,.90f)),8);
    if(scenic||index%2==0)IslandArt.Waterfall(parent,new Vector3(size*.68f,.52f,-size*.45f),realm);
   }
   if(realm==2){
    var stone=MeshArt.Mat("Realm2CarvedStone",new Color(.92f,.80f,.59f),1);
    var gold=MeshArt.Mat("Realm2Inlay",new Color(.98f,.67f,.15f));
    // Narrow perimeter paving and inscriptions keep the central landing disk open.
    for(int j=0;j<6;j++){float a=j*Mathf.PI/3;var tile=MeshArt.Box(parent,"TemplePaving",new Vector3(Mathf.Cos(a)*size*.90f,.625f,Mathf.Sin(a)*size*.90f),new Vector3(.34f,.05f,.26f),stone);tile.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);}
    for(int j=0;j<2;j++){
     var pillar=new GameObject("TempleColumn");pillar.transform.SetParent(parent,false);pillar.transform.localPosition=new Vector3((j==0?-1:1)*size*.78f,.63f,-size*.48f);
     float h=(scenic?1.85f:1.05f)+(index%3)*.18f;
     MeshArt.Box(pillar.transform,"ColumnFoot",new Vector3(0,.07f,0),new Vector3(.38f,.14f,.38f),stone);
     MeshArt.Lathe(pillar.transform,"FlutedColumn",new[]{.14f,h*.35f,h*.75f,h},new[]{.15f,.12f,.12f,.16f},12,stone,Vector3.one,true);
     MeshArt.Box(pillar.transform,"ColumnCapital",new Vector3(0,h,0),new Vector3(.36f,.10f,.36f),stone);
     MeshArt.Ring(pillar.transform,"ColumnInlay",.157f,.025f,.18f,gold,16);
    }
    if(scenic){MeshArt.Box(parent,"RuinedLintel",new Vector3(0,2.62f,-size*.48f),new Vector3(size*1.8f,.22f,.34f),stone);MeshArt.Box(parent,"LintelInlay",new Vector3(0,2.65f,-size*.66f),new Vector3(size*1.65f,.055f,.04f),gold);}
   }
  }
 }
}
