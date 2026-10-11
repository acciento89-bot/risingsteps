using UnityEngine;
using Kamilunavo.RisingSteps.Core;

namespace Kamilunavo.RisingSteps.Visuals
{
 public static class RealmLandscapeBackdrop
 {
  static readonly string[] Names={"MeadowHorizon","CascadeHorizon","TempleHorizon","JungleHorizon","CrystalHorizon","FrostHorizon","DesertHorizon","VolcanoHorizon","CloudCoastHorizon","StarlitHorizon"};
  public static void Build(Transform parent,int realm)
  {
   var root=new GameObject(Names[(int)RisingRealmCatalog.Theme(realm)]);root.transform.SetParent(parent,false);
   var stone=LandscapePalette.Surface(parent,realm);var accent=LandscapePalette.Surface(parent,realm,5);
   // Distant silhouettes sit beside and beyond the whole ascending route, never on it.
   switch(RisingRealmCatalog.Theme(realm)){
    case RisingRealmTheme.Meadow:
     for(int i=0;i<6;i++){
      var p=Point(i,-13);LandscapeGeometry.Rock(root.transform,"MeadowHighland",p,new Vector3(24,14,21),stone,i);
      LandscapeBotany.Tree(root.transform,p+Vector3.up*6,i*4,3.0f+i%2);
     }break;
    case RisingRealmTheme.Waterfall:
     for(int i=0;i<5;i++){
      var p=Point(i,-12);LandscapeGeometry.Rock(root.transform,"CascadeMesa",p,new Vector3(23,19,21),stone,i);
      MeshArt.Oval(root.transform,"HighlandLake",p+Vector3.up*9,new Vector3(14,.15f,10),LandscapePalette.Surface(parent,realm,2),24);
      IslandArt.Waterfall(root.transform,p+new Vector3(0,9,-7),1,3.8f);
     }break;
    case RisingRealmTheme.Temple:
     for(int i=0;i<4;i++){
      var p=Point(i,-6);LandscapeGeometry.Rock(root.transform,"TempleAcropolis",p-Vector3.up*7,new Vector3(26,15,21),stone,i);
      var facade=new GameObject("TempleSkySanctuary");facade.transform.SetParent(root.transform,false);facade.transform.localPosition=p;
      for(int j=0;j<5;j++){var col=MeshArt.Lathe(facade.transform,"SanctuaryColumn",new[]{0f,.6f,1f,8f,8.6f},new[]{.85f,.85f,.57f,.57f,.9f},12,stone,Vector3.one,true);col.transform.localPosition=new Vector3((j-2)*3.5f,0,0);}
      MeshArt.Box(facade.transform,"SanctuaryEntablature",new Vector3(0,8.7f,0),new Vector3(17.5f,1.1f,2.4f),stone);
      MeshArt.Box(facade.transform,"SanctuaryGoldFrieze",new Vector3(0,8.5f,-1.23f),new Vector3(16,.22f,.10f),accent);
     }break;
    case RisingRealmTheme.Jungle:
     for(int i=0;i<7;i++){
      var p=Point(i,-17);LandscapeGeometry.Rock(root.transform,"JungleKarst",p,new Vector3(20,20+i%3*3,19),stone,i);
      LandscapeBotany.Branch(root.transform,"AncientCanopyTrunk",new[]{p,p+new Vector3(-1,12,0),p+new Vector3(3,23,1)},new[]{1.6f,1.0f,.50f},accent,12);
      for(int j=0;j<3;j++)LandscapeBotany.LeafMass(root.transform,p+new Vector3((j-1)*5,22+j%2*2,j*3),new Vector3(17,10,15),LandscapePalette.Surface(parent,realm,3),i*9+j);
     }break;
    case RisingRealmTheme.Crystal:
     for(int i=0;i<5;i++){
      var p=Point(i,-11);var grotto=new GameObject("CrystalCavernRib");grotto.transform.SetParent(root.transform,false);grotto.transform.localPosition=p;
      for(int j=0;j<9;j++)LandscapeGeometry.ArchStone(grotto.transform,"CavernVault",j*Mathf.PI/9,(j+1)*Mathf.PI/9,7,9,4,0,stone,3.2f);
      RealmLandscapeThemes.Dress(grotto.transform,i,realm,7,false);
     }break;
    case RisingRealmTheme.Frost:
     for(int i=0;i<8;i++){
      var p=Point(i,-18);Peak(root.transform,"GlacialMountain",p,new Vector3(18,29+i%3*5,17),stone,7,i);
      Peak(root.transform,"MountainSnowCap",p+Vector3.up*(12+i%3*2),new Vector3(10,19+i%3*3,9),LandscapePalette.Surface(parent,realm,1),7,i);
     }break;
    case RisingRealmTheme.Desert:
     for(int i=0;i<6;i++){
      var p=Point(i,-15);var dune=MeshArt.Oval(root.transform,"DistantDune",p,new Vector3(32,12,23),LandscapePalette.Surface(parent,realm,1),20);dune.transform.localRotation=Quaternion.Euler(0,i*19,-7+i%3*5);
      if(i%2==0){var arch=new GameObject("DuneWindGate");arch.transform.SetParent(root.transform,false);arch.transform.localPosition=p+Vector3.up*5;RealmLandscapeThemes.Dress(arch.transform,i,realm,5);}
     }break;
    case RisingRealmTheme.Volcano:
     for(int i=0;i<4;i++){
      var p=Point(i,-18);Caldera(root.transform,p,14+i,28+i*2,stone);
      MeshArt.Oval(root.transform,"CalderaLavaLake",p+Vector3.up*(28+i*2-.18f),new Vector3(8,.15f,7),LandscapePalette.Surface(parent,realm,7),20);
      for(int tooth=0;tooth<3;tooth++)LandscapeGeometry.Rock(root.transform,"VolcanicBasaltRidge",p+new Vector3((tooth-1)*6,7,5),new Vector3(5,19+tooth*2,6),stone,i+tooth);
     }break;
    case RisingRealmTheme.CloudCoast:
     // A true distant water plane below all route geometry, with cut chalk sea gates.
     MeshArt.Mesh(root.transform,"HorizonOcean",new[]{new Vector3(-180,-32,-20),new Vector3(180,-32,-20),new Vector3(180,-32,220),new Vector3(-180,-32,220)},new[]{0,2,1,0,3,2},new[]{Vector2.zero,Vector2.right*16,Vector2.one*16,Vector2.up*16},LandscapePalette.Surface(parent,realm,2));
     for(int i=0;i<5;i++){
      var p=Point(i,-22);LandscapeGeometry.Rock(root.transform,"CoastalHeadland",p,new Vector3(27,19,21),stone,i);
      var gate=new GameObject("DistantSeaGate");gate.transform.SetParent(root.transform,false);gate.transform.localPosition=p+new Vector3(0,3,-8);gate.transform.localScale=Vector3.one*4;
      RealmLandscapeThemes.Dress(gate.transform,i,realm,2,true);
     }break;
    case RisingRealmTheme.Starlit:
     for(int i=0;i<4;i++){
      var p=Point(i,-5);LandscapeGeometry.Rock(root.transform,"MoonlitSkyFragment",p-Vector3.up*6,new Vector3(19,8,15),stone,i);
      var ruin=new GameObject("OrbitalMoonSanctuary");ruin.transform.SetParent(root.transform,false);ruin.transform.localPosition=p;ruin.transform.localScale=Vector3.one*4;
      RealmLandscapeThemes.Dress(ruin.transform,i,realm,2);
     }
     MeshArt.Oval(root.transform,"DistantMoon",new Vector3(-42,37,132),Vector3.one*13,LandscapePalette.Surface(parent,realm,4),24);
     var random=new System.Random(9187);for(int i=0;i<42;i++){
      var pos=new Vector3(-120+(float)random.NextDouble()*240,27+(float)random.NextDouble()*63,120+(float)random.NextDouble()*40);
      var star=MeshArt.Oval(root.transform,"HorizonStar",pos,Vector3.one*(.13f+i%4*.045f),LandscapePalette.Surface(parent,realm,7),4);star.transform.localRotation=Quaternion.Euler(i*13,i*19,0);
     }break;
   }
   MeshArt.Batch(root);
  }
  static Vector3 Point(int i,float height)=>new Vector3((i%2==0?-1:1)*(37+i%3*10),height+i%3*3,24+i*18);
  static void Peak(Transform p,string name,Vector3 pos,Vector3 size,Material material,int sides,int seed)
  {
   var v=new System.Collections.Generic.List<Vector3>();var t=new System.Collections.Generic.List<int>();
   for(int i=0;i<sides;i++){
    float a=i*Mathf.PI*2/sides,b=(i+1)*Mathf.PI*2/sides;int k=v.Count;
    var tip=new Vector3(size.x*.13f,size.y,size.z*.09f);var left=new Vector3(Mathf.Cos(a)*size.x*.5f,0,Mathf.Sin(a)*size.z*.5f);var right=new Vector3(Mathf.Cos(b)*size.x*.5f,0,Mathf.Sin(b)*size.z*.5f);
    left.y=(i%2)*size.y*.07f;right.y=((i+1)%2)*size.y*.07f;v.AddRange(new[]{left,tip,right});t.AddRange(new[]{k,k+1,k+2});
   }
   var go=MeshArt.Mesh(p,name,v.ToArray(),t.ToArray(),null,material);go.transform.localPosition=pos;
  }
  static void Caldera(Transform p,Vector3 position,float radius,float height,Material material)
  {
   const int n=24;var v=new System.Collections.Generic.List<Vector3>();var uv=new System.Collections.Generic.List<Vector2>();var t=new System.Collections.Generic.List<int>();
   float[] r={radius,radius*.64f,radius*.37f,radius*.25f};float[] y={0,height*.68f,height,height-.70f};
   for(int ring=0;ring<4;ring++)for(int i=0;i<=n;i++){
    float a=(i%n)*Mathf.PI*2/n;v.Add(new Vector3(Mathf.Cos(a)*r[ring],y[ring]+(ring==3?0:Mathf.Sin(a*3)*height*.025f),Mathf.Sin(a)*r[ring]));uv.Add(new Vector2((float)i/n*4,ring));
    if(ring>0&&i<n){int k=ring*(n+1)+i,lower=k-(n+1);t.AddRange(new[]{lower,k,lower+1,lower+1,k,k+1});}
   }
   var go=MeshArt.Mesh(p,"VolcanoCaldera",v.ToArray(),t.ToArray(),uv.ToArray(),material);go.transform.localPosition=position;LandscapeGeometry.JoinNormals(go.GetComponent<MeshFilter>().sharedMesh);
  }
 }
}
