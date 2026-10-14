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
     for(int i=0;i<7;i++){
      var p=GardenPoint(i);Highland(root.transform,p,realm,i,stone);
      MeshArt.Oval(root.transform,"HighlandMeadow",p+Vector3.up*7.9f,new Vector3(11.8f,.34f,10),LandscapePalette.Surface(parent,realm,1),18);
      for(int tree=0;tree<3;tree++)LandscapeBotany.Tree(root.transform,p+new Vector3((tree-1)*3.3f,8.05f,tree%2*2),i*4+tree,2.3f+(i+tree)%3*.45f,true);
     }break;
    case RisingRealmTheme.Waterfall:
     for(int i=0;i<7;i++){
      var p=GardenPoint(i);if(i==0)p.x-=5;Highland(root.transform,p,realm,i,stone);
      MeshArt.Oval(root.transform,"HighlandLake",p+Vector3.up*7.94f,new Vector3(10,.13f,8.8f),LandscapePalette.Pool(parent),24);
      var fall=new Vector3((i%2==0?1:-1)*2.8f,8,-5.3f);
      IslandArt.Waterfall(root.transform,p+fall,1,2.1f+i%3*.6f,13+i%3*2);
      for(int terrace=0;terrace<2;terrace++){
       var ledge=p+new Vector3((terrace==0?1:-1)*5.4f,3.5f-terrace*3,-1.7f+terrace*3.1f);
       var support=LandscapeGeometry.Rock(root.transform,"CascadeSideTerrace",ledge,new Vector3(6.4f,2.6f,5.5f),stone,i*7+terrace);
       var supportMesh=support.GetComponent<MeshFilter>().sharedMesh;var supportVertices=supportMesh.vertices;
       for(int vertex=0;vertex<supportVertices.Length;vertex++)if(supportVertices[vertex].y>.6f)supportVertices[vertex].y=1.18f;
       supportMesh.vertices=supportVertices;supportMesh.RecalculateNormals();supportMesh.RecalculateBounds();LandscapeGeometry.JoinNormals(supportMesh);
       MeshArt.Oval(root.transform,"TerraceLake",ledge+Vector3.up*1.23f,new Vector3(4.4f,.09f,3.5f),LandscapePalette.Pool(parent),18);
       IslandArt.Waterfall(root.transform,ledge+new Vector3(0,1.3f,-2),1,1.15f,5.5f);
      }
      // Spend the same mobile kit on a visible foreground bank rather than
      // three tiny shrubs on the farthest mesa behind the destination.
      for(int shrub=0;shrub<(i==6?0:3);shrub++)LandscapeBotany.LeafCluster(root.transform,"CascadeRidgeLeafClusters",p+new Vector3((shrub-1)*3.7f,8.15f,3.1f+Mathf.Sin(shrub)*.5f),new Vector3(3.0f,1.0f,2.5f),LandscapePalette.Moss(parent),i*13+shrub,16);
      for(int tree=0;tree<(i%2==0?2:1);tree++)LandscapeBotany.CascadeWillow(root.transform,i==0&&tree==0?new Vector3(-11.3f,5.6f,47):p+new Vector3((tree==0?-1:1)*3.4f,7.95f,2.8f+tree*.5f),i*7+tree,2.3f+(i+tree)%2*.35f,true);
      LandscapeBotany.Branch(root.transform,"CascadeCliffVine",new[]{p+new Vector3(4.6f,7.8f,-5.1f),p+new Vector3(4.9f,3,-5.7f),p+new Vector3(4.2f,-1.5f,-5.2f)},new[]{.16f,.12f,.035f},LandscapePalette.Moss(parent),6);
     }
     CascadeForegroundBank(root.transform,stone,parent);break;
    case RisingRealmTheme.Temple:
     for(int i=0;i<6;i++){
      var p=GardenPoint(i);Highland(root.transform,p,realm,i,stone);
      var facade=new GameObject("TempleSkySanctuary");facade.transform.SetParent(root.transform,false);facade.transform.localPosition=p+Vector3.up*7.9f;
      // Broad stepped podiums, articulated blocks and a real pitched pediment
      // remain readable in the side silhouette and through the forward camera.
      for(int terrace=0;terrace<3;terrace++)MeshArt.Box(facade.transform,"AcropolisPodium",new Vector3(0,terrace*.42f,0),new Vector3(13.2f-terrace*.85f,.40f,9.0f-terrace*.6f),stone);
      for(int block=0;block<14;block++)MeshArt.Box(facade.transform,"AcropolisMasonry",new Vector3((block%7-3)*1.74f,.28f+(block/7)*.43f,-4.38f),new Vector3(1.69f,.38f,.22f),stone);
      for(int j=0;j<5;j++){
       var col=MeshArt.Lathe(facade.transform,"SanctuaryColumn",new[]{0f,.3f,.55f,5.2f,5.5f},new[]{.56f,.56f,.36f,.36f,.6f},16,stone,Vector3.one,true);col.transform.localPosition=new Vector3((j-2)*2.25f,1.10f,-1.9f);
       for(int band=0;band<2;band++)MeshArt.Ring(col.transform,"SanctuaryColumnGold",.41f,.035f,.65f+band*4.4f,accent,16);
      }
      MeshArt.Box(facade.transform,"SanctuaryEntablature",new Vector3(0,6.65f,-1.9f),new Vector3(11.4f,.65f,3.2f),stone);
      MeshArt.Box(facade.transform,"SanctuaryGoldFrieze",new Vector3(0,6.51f,-3.53f),new Vector3(10.9f,.16f,.10f),accent);
      MeshArt.Mesh(facade.transform,"SanctuaryPediment",new[]{new Vector3(-5.9f,7,-3.6f),new Vector3(5.9f,7,-3.6f),new Vector3(0,9.1f,-3.6f),new Vector3(-5.9f,7,-.2f),new Vector3(5.9f,7,-.2f),new Vector3(0,9.1f,-.2f)},new[]{0,2,1,3,4,5,0,3,5,0,5,2,1,2,5,1,5,4,0,1,4,0,4,3},new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.zero,Vector2.right,Vector2.up},stone);
      for(int tooth=0;tooth<13;tooth++)MeshArt.Box(facade.transform,"SanctuaryDentil",new Vector3((tooth-6)*.80f,6.92f,-3.54f),new Vector3(.23f,.17f,.18f),accent);
      var banner=MeshArt.Mat("TempleHorizonBanner",new Color(.41f,.23f,.45f));
      for(int side=-1;side<=1;side+=2)MeshArt.Box(facade.transform,"SanctuaryBanner",new Vector3(side*4.5f,4.7f,-2.38f),new Vector3(.75f,2.5f,.045f),banner);
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
  // Route x remains inside +/-3; the nearest stone edge stays beyond +/-10.
  // Height rises with depth so a finished course still has scenery above its horizon.
  static Vector3 GardenPoint(int i)=>new Vector3((i%2==0?-1:1)*(18+i%3*4),1+i*2.2f,42+i*14);
  static void CascadeForegroundBank(Transform root,Material stone,Transform paletteOwner)
  {
   var bank=LandscapeGeometry.Rock(root,"CascadeForegroundBank",new Vector3(-13,.7f,47),new Vector3(5.8f,10,7),stone,73);
   var mesh=bank.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices;for(int i=0;i<points.Length;i++)if(points[i].y>2)points[i].y=4.9f;mesh.vertices=points;mesh.RecalculateNormals();mesh.RecalculateBounds();LandscapeGeometry.JoinNormals(mesh);
   MeshArt.Oval(root,"CascadeForegroundPool",new Vector3(-13,5.62f,47),new Vector3(4.6f,.09f,5.2f),LandscapePalette.Pool(paletteOwner),18);
   IslandArt.Waterfall(root,new Vector3(-13,5.67f,43.6f),1,1.7f,9.4f);
  }
  static void Highland(Transform parent,Vector3 position,int realm,int seed,Material stone)
  {
   var mesa=LandscapeGeometry.Rock(parent,realm==1?"CascadeMesa":realm==2?"TempleAcropolis":"MeadowHighland",position,new Vector3(15,17,14),stone,seed*19);
   // Flatten the real summit below the lakes/podiums. Otherwise an irregular
   // rock cap would hide the water and vegetation inside its highest triangles.
   var mesh=mesa.GetComponent<MeshFilter>().sharedMesh;var vertices=mesh.vertices;
   for(int vertex=0;vertex<vertices.Length;vertex++)if(vertices[vertex].y>6)vertices[vertex].y=7.75f;
   mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();LandscapeGeometry.JoinNormals(mesh);
   for(int face=0;face<2;face++){
    var p=position+new Vector3((face-.5f)*6.4f,-1.6f+(face%2)*1.0f,-4.4f);
    var rock=LandscapeGeometry.Rock(parent,"HighlandFractureFace",p,new Vector3(3.8f,13+(face%3)*2,4.3f),stone,seed*13+face);
    rock.transform.localRotation=Quaternion.Euler(2+face*2,face*9,-5+face*3);
   }
   // Unequal chipped sediment ledges break the broad walls into horizontal
   // beds. They are real beveled meshes, outside the entire gameplay corridor.
   for(int bed=0;bed<5;bed++){
    var p=position+new Vector3(Mathf.Sin(seed+bed*2.1f)*2.1f,-5.8f+bed*2.6f,-6.8f);
    var ledge=LandscapeGeometry.BeveledBlock(parent,"HighlandSedimentLedge",p,new Vector3(4.8f+bed%3*.9f,.28f+bed%2*.14f,1.15f),stone,.08f);
    ledge.transform.localRotation=Quaternion.Euler(2,Mathf.Sin(seed+bed)*7,Mathf.Cos(seed+bed*1.6f)*3);LandscapeGeometry.StoneUv(ledge,.13f);
   }
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
