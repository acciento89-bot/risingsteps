using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Core;

namespace Kamilunavo.RisingSteps.Visuals
{
 // Perimeter landmarks keep the authoritative landing disk completely separate.
 public static class RealmLandscapeThemes
 {
  public static void Dress(Transform parent,int index,int realm,float size,bool scenic=false)
  {
   switch(RisingRealmCatalog.Theme(realm)){
    case RisingRealmTheme.Jungle:Jungle(parent,index,realm,size,scenic);break;
    case RisingRealmTheme.Crystal:Crystal(parent,index,realm,size);break;
    case RisingRealmTheme.Frost:Frost(parent,index,realm,size,scenic);break;
    case RisingRealmTheme.Desert:Desert(parent,index,realm,size);break;
    case RisingRealmTheme.Volcano:Volcano(parent,index,realm,size);break;
    case RisingRealmTheme.CloudCoast:Coast(parent,index,realm,size,scenic);break;
    case RisingRealmTheme.Starlit:Starlit(parent,index,realm,size);break;
   }
  }
  static GameObject Root(Transform parent,string name,Vector3 position)
  {var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=position;return go;}
  static void Jungle(Transform p,int seed,int realm,float size,bool scenic=false)
  {
   var bark=LandscapePalette.Surface(p,realm,5);var leaf=LandscapePalette.Surface(p,realm,3);var sun=LandscapePalette.Surface(p,realm,4);
   var mantle=Root(p,"JungleRootMantle",Vector3.zero);
   for(int i=0;i<5;i++){
    float a=i*1.256637f+seed*.19f;var radial=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));
    LandscapeBotany.Branch(mantle.transform,"ClingingRoot",new[]{radial*size*.87f+Vector3.up*.61f,radial*size*1.05f+Vector3.down*.35f,radial*size*.72f+Vector3.down*1.8f,radial*size*.40f+Vector3.down*3.1f},new[]{.085f,.12f,.065f,.013f},bark,7);
   }
   var pos=new Vector3(-size*.87f,.61f,-size*.42f);var tree=Root(p,"JungleCanopy",pos);float h=scenic?3.65f:2.60f;
   LandscapeBotany.Branch(tree.transform,"ButtressTrunk",new[]{Vector3.zero,new Vector3(-.07f,h*.4f,.05f),new Vector3(.22f,h,.12f)},new[]{.21f,.13f,.08f},bark,10);
   for(int j=0;j<3;j++){
    float a=j*2.094f;var branch=new Vector3(Mathf.Cos(a)*.62f,h-.16f+j*.14f,Mathf.Sin(a)*.6f);
    LandscapeBotany.Branch(tree.transform,"CanopyBough",new[]{Vector3.up*h*.62f,branch},new[]{.09f,.025f},bark,7);
    LandscapeBotany.LeafMass(tree.transform,branch,new Vector3(1.20f,.80f,1.09f),j==0?sun:leaf,seed+j);
    BroadLeaves(tree.transform,"JungleBroadLeaf",branch,1.20f,j+seed,leaf,7);
    var vine=branch+Vector3.right*.30f;LandscapeBotany.Branch(tree.transform,"HangingLiana",new[]{vine,vine+new Vector3(.12f,-.9f,.10f),vine+new Vector3(-.12f,-1.65f,.09f)},new[]{.018f,.015f,.008f},bark,5);
   }
   for(int fern=0;fern<3;fern++){
    float a=.5f+fern*2.1f;var point=new Vector3(Mathf.Cos(a)*size*.89f,.61f,Mathf.Sin(a)*size*.89f);
    BroadLeaves(p,"JungleFern",point,.55f,seed+fern,sun,8);
   }
  }
  static void Crystal(Transform p,int seed,int realm,float size)
  {
   var stone=LandscapePalette.Surface(p,realm);var gem=LandscapePalette.Surface(p,realm,5);var light=LandscapePalette.Surface(p,realm,7);
   var geode=Root(p,"CrystalGeode",new Vector3(size*.86f,.51f,-size*.42f));
   // Open mineral half-shell reveals the smaller prisms recessed inside it.
   for(int i=0;i<9;i++){
    float a=i*Mathf.PI/8;LandscapeGeometry.Rock(geode.transform,"GeodeShell",new Vector3(Mathf.Cos(a)*.48f,Mathf.Sin(a)*.52f,0),new Vector3(.25f,.30f,.35f),stone,seed+i);
    var shard=Spire(geode.transform,"GeodePrism",new Vector3(Mathf.Cos(a)*.32f,Mathf.Sin(a)*.23f,-.12f),.055f,.23f+(i%3)*.08f,6,light,seed+i);shard.transform.localRotation=Quaternion.Euler(0,0,Mathf.Cos(a)*-30);
   }
   for(int cluster=0;cluster<3;cluster++){
    float a=cluster*2.11f+seed*.21f;var point=new Vector3(Mathf.Cos(a)*size*.93f,.58f,Mathf.Sin(a)*size*.93f);
    for(int k=0;k<3;k++){
     var prism=Spire(p,"CrystalSpire",point+new Vector3((k-1)*.15f,0,k*.07f),.12f+(k%2)*.035f,.58f+(k%3)*.38f,6,k==1?light:gem,seed+k);prism.transform.localRotation=Quaternion.Euler((k-1)*12,k*17,(k-1)*15);
    }
   }
   for(int i=0;i<4;i++){var shard=Spire(p,"UndercliffCrystal",new Vector3((i-1.5f)*.35f,-.35f,-size*.72f),.17f,.9f+i*.14f,5,gem,seed+i);shard.transform.localRotation=Quaternion.Euler(167,0,i*14-21);}
  }
  static void Frost(Transform p,int seed,int realm,float size,bool scenic=false)
  {
   var snow=LandscapePalette.Surface(p,realm,1);var ice=LandscapePalette.Surface(p,realm,5);var pine=LandscapePalette.Surface(p,realm,3);var trunk=LandscapePalette.Surface(p,realm,6);
   for(int i=0;i<8;i++){
    float a=i*Mathf.PI/4+.15f;var point=new Vector3(Mathf.Cos(a)*size*.91f,.59f,Mathf.Sin(a)*size*.91f);
    LandscapeGeometry.Rock(p,"SnowOverhang",point,new Vector3(.60f,.17f,.42f),snow,seed+i);
    var icicle=Spire(p,"FrostIcicle",point-Vector3.up*.06f,.085f,.48f+(i%3)*.27f,5,ice,seed+i);icicle.transform.localRotation=Quaternion.Euler(180,0,0);
   }
   var tree=Root(p,"FrostFir",new Vector3(-size*.88f,.62f,-size*.37f));float h=scenic?3.4f:2.1f;
   LandscapeBotany.Branch(tree.transform,"FirTrunk",new[]{Vector3.zero,Vector3.up*h},new[]{.10f,.025f},trunk,7);
   for(int i=0;i<4;i++){
    float r=(.70f-i*.13f)*(scenic?1.3f:1);var layer=Spire(tree.transform,"FirBough",Vector3.up*(.37f+i*h*.16f),r,h*.37f,10,pine,seed+i);
    Spire(layer.transform,"FirSnow",Vector3.up*h*.08f,r*.88f,h*.30f,10,snow,seed+i);
   }
  }
  static void Desert(Transform p,int seed,int realm,float size)
  {
   var sand=LandscapePalette.Surface(p,realm,1);var stone=LandscapePalette.Surface(p,realm);var cactus=LandscapePalette.Surface(p,realm,3);
   var arch=Root(p,"DesertWindArch",new Vector3(size*.91f,.55f,-size*.43f));arch.transform.localRotation=Quaternion.Euler(0,-18,0);
   for(int i=0;i<9;i++)LandscapeGeometry.ArchStone(arch.transform,"ErodedArchStone",i*Mathf.PI/9,(i+1)*Mathf.PI/9,.39f,.61f,.33f,0,stone,.21f);
   for(int side=-1;side<=1;side+=2)LandscapeGeometry.Rock(arch.transform,"ArchFoot",new Vector3(side*.50f,.11f,0),new Vector3(.26f,.44f,.47f),stone,seed+side);
   for(int i=0;i<4;i++){
    float a=.75f+i*.55f;var ridge=Root(p,"DuneRipple",new Vector3(Mathf.Cos(a)*size*.82f,.61f,Mathf.Sin(a)*size*.82f));ridge.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);
    Ridge(ridge.transform,"SandCrest",.52f,.12f,.16f,7,sand,seed+i);
   }
   var plant=Root(p,"DesertCactus",new Vector3(-size*.88f,.60f,-size*.45f));
   LandscapeBotany.Branch(plant.transform,"CactusStem",new[]{Vector3.zero,Vector3.up*.62f,Vector3.up*.98f},new[]{.10f,.105f,.024f},cactus,9);
   for(int side=-1;side<=1;side+=2)LandscapeBotany.Branch(plant.transform,"CactusArm",new[]{Vector3.up*.38f,new Vector3(side*.25f,.44f,0),new Vector3(side*.26f,.72f,0)},new[]{.075f,.065f,.025f},cactus,8);
   for(int i=0;i<3;i++)LandscapeGeometry.Rock(p,"DesertTalus",new Vector3(size*.53f,-.45f-i*.51f,-size*.76f),new Vector3(.76f-i*.14f,.58f,.51f),stone,seed+i);
  }
  static void Volcano(Transform p,int seed,int realm,float size)
  {
   var basalt=LandscapePalette.Surface(p,realm);var cooled=LandscapePalette.Surface(p,realm,6);var lava=LandscapePalette.Surface(p,realm,7);
   var channel=Root(p,"VolcanoLavaChannel",new Vector3(-size*.82f,.611f,size*.15f));channel.transform.localRotation=Quaternion.Euler(0,31,0);
   Ribbon(channel.transform,"LavaRiver",new[]{new Vector3(-.35f,0,-.65f),new Vector3(-.12f,0,-.27f),new Vector3(.09f,0,.12f),new Vector3(.19f,0,.67f)},.17f,lava);
   for(int i=0;i<4;i++)for(int side=-1;side<=1;side+=2)LandscapeGeometry.Rock(channel.transform,"CooledLavaBank",new Vector3(side*.22f+(i-1.5f)*.11f,-.03f,(i-1.5f)*.39f),new Vector3(.18f,.12f,.35f),cooled,seed+i);
   for(int i=0;i<5;i++){
    float a=i*1.257f+.3f;var spire=Spire(p,"BasaltTooth",new Vector3(Mathf.Cos(a)*size*.91f,.57f,Mathf.Sin(a)*size*.91f),.21f,.53f+(i%3)*.28f,5,basalt,seed+i);spire.transform.localRotation=Quaternion.Euler(8,0,i*6-13);
   }
   Ribbon(p,"MoltenCliffFissure",new[]{new Vector3(size*.76f,.45f,-.58f),new Vector3(size*.69f,-.52f,-.56f),new Vector3(size*.36f,-1.44f,-.37f),new Vector3(.33f,-2.54f,-.20f)},.055f,lava);
  }
  static void Coast(Transform p,int seed,int realm,float size,bool scenic=false)
  {
   var chalk=LandscapePalette.Surface(p,realm);var water=LandscapePalette.Surface(p,realm,2);var sand=LandscapePalette.Surface(p,realm,1);
   var arch=Root(p,"CloudCoastSeaArch",new Vector3(0,-1.52f,-size*.71f));
   for(int i=0;i<11;i++)LandscapeGeometry.ArchStone(arch.transform,"SeaArchStone",i*Mathf.PI/11,(i+1)*Mathf.PI/11,.46f,.73f,0,0,chalk,.31f);
   for(int side=-1;side<=1;side+=2)LandscapeGeometry.Rock(arch.transform,"SeaArchFoot",new Vector3(side*.56f,-.23f,0),new Vector3(.39f,.58f,.68f),chalk,seed+side);
   var shelf=new Vector3(size*.92f,.55f,size*.40f);LandscapeGeometry.Rock(p,"CoastalBeachShelf",shelf-Vector3.up*.11f,new Vector3(.86f,.23f,.74f),sand,seed);
   MeshArt.Oval(p,"TidePool",shelf+Vector3.up*.06f,new Vector3(.57f,.021f,.42f),water,18);
   for(int i=0;i<3;i++){
    var wave=MeshArt.Ring(p,"TideFoam",.14f+i*.052f,.008f,.615f,LandscapePalette.Surface(p,realm,4),18,190);wave.transform.localPosition=new Vector3(shelf.x,0,shelf.z);
   }
   Palm(p,new Vector3(-size*.88f,.61f,-size*.43f),scenic?3.0f:2.0f,realm,seed);
   for(int i=0;i<4;i++)MeshArt.Oval(p,"BeachPebble",shelf+new Vector3((i-1.5f)*.14f,.052f,-.20f),new Vector3(.085f,.045f,.055f),chalk,6);
  }
  static void Starlit(Transform p,int seed,int realm,float size)
  {
   var stone=LandscapePalette.Surface(p,realm);var silver=LandscapePalette.Surface(p,realm,5);var glow=LandscapePalette.Surface(p,realm,7);
   var crescent=Root(p,"StarlitCrescent",new Vector3(size*.87f,.64f,-size*.48f));crescent.transform.localRotation=Quaternion.Euler(0,-24,18);
   for(int i=0;i<14;i++){float a=(i*.105f+.16f)*Mathf.PI;LandscapeGeometry.ArchStone(crescent.transform,"MoonStone",a,a+.10f*Mathf.PI,.49f,.67f,.45f,0,silver,.10f);}
   for(int i=0;i<5;i++){
    float a=i*Mathf.PI*.4f;var pos=new Vector3(Mathf.Cos(a)*size*.91f,.65f,Mathf.Sin(a)*size*.91f);
    var plinth=Spire(p,"AstralObelisk",pos,.11f,.43f+(i%2)*.21f,4,stone,seed+i);
    MeshArt.Oval(plinth.transform,"StarLantern",new Vector3(0,.42f+(i%2)*.21f,0),Vector3.one*.11f,glow,6);
    Star(p,"StarGroundInlay",pos+Vector3.up*.002f,.16f,glow,5);
   }
   var orbit=MeshArt.Ring(p,"MoonOrbitRuin",size*.67f,.024f,-1.33f,silver,32,270);orbit.transform.localRotation=Quaternion.Euler(21,seed*13,-16);
  }
  public static void Palm(Transform p,Vector3 position,float height,int realm,int seed)
  {
   var root=Root(p,"CoastalPalm",position);var bark=LandscapePalette.Surface(p,realm,5);var leaf=LandscapePalette.Surface(p,realm,3);
   LandscapeBotany.Branch(root.transform,"PalmTrunk",new[]{Vector3.zero,new Vector3(.09f,height*.43f,0),new Vector3(.38f,height,0)},new[]{.12f,.085f,.045f},bark,9);
   BroadLeaves(root.transform,"PalmFrond",new Vector3(.38f,height,0),height*.57f,seed,leaf,9);
  }
  static void BroadLeaves(Transform p,string name,Vector3 center,float length,int seed,Material material,int leaves)
  {
   var v=new List<Vector3>();var t=new List<int>();
   for(int i=0;i<leaves;i++){
    float a=i*Mathf.PI*2/leaves+seed*.23f;var forward=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var side=new Vector3(-forward.z,0,forward.x);int k=v.Count;
    float l=length*(.79f+.15f*Mathf.Sin(seed+i));var middle=center+forward*l*.55f+Vector3.up*l*.19f;
    v.AddRange(new[]{center,middle-side*l*.19f,center+forward*l+Vector3.down*l*.20f,middle+side*l*.19f,middle+Vector3.up*l*.065f});
    t.AddRange(new[]{k,k+1,k+4,k+1,k+2,k+4,k+2,k+3,k+4,k+3,k,k+4,k+4,k+1,k,k+4,k+2,k+1,k+4,k+3,k+2,k+4,k,k+3});
   }
   MeshArt.Mesh(p,name,v.ToArray(),t.ToArray(),null,material);
  }
  static GameObject Spire(Transform p,string name,Vector3 pos,float radius,float height,int sides,Material mat,int seed)
  {
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int ring=0;ring<3;ring++)for(int i=0;i<sides;i++){
    float a=i*Mathf.PI*2/sides;float r=radius*(ring==2?.17f:ring==1?.86f:1)*(1+.08f*Mathf.Sin(i*2.1f+seed));
    v.Add(new Vector3(Mathf.Cos(a)*r,ring==0?0:ring==1?height*.72f:height,Mathf.Sin(a)*r));uv.Add(new Vector2((float)i/sides,ring*.5f));
   }
   for(int ring=0;ring<2;ring++)for(int i=0;i<sides;i++){int next=(i+1)%sides,k=ring*sides+i,n=ring*sides+next;t.AddRange(new[]{k,k+sides,n,n,k+sides,n+sides});}
   int bottom=v.Count;v.Add(Vector3.zero);uv.Add(Vector2.zero);int top=v.Count;v.Add(Vector3.up*(height+.02f));uv.Add(Vector2.one);
   for(int i=0;i<sides;i++){int next=(i+1)%sides;t.AddRange(new[]{bottom,i,next,top,2*sides+next,2*sides+i});}
   // Split faces deliberately: angular mineral prisms are distinct from smooth leaf volumes.
   var expanded=new Vector3[t.Count];var tex=new Vector2[t.Count];var tri=new int[t.Count];for(int i=0;i<t.Count;i++){expanded[i]=v[t[i]];tex[i]=uv[t[i]];tri[i]=i;}
   var go=MeshArt.Mesh(p,name,expanded,tri,tex,mat);go.transform.localPosition=pos;return go;
  }
  static void Ribbon(Transform p,string name,Vector3[] path,float width,Material material)
  {
   var v=new List<Vector3>();var t=new List<int>();
   for(int i=0;i<path.Length;i++){
    var d=path[Mathf.Min(i+1,path.Length-1)]-path[Mathf.Max(i-1,0)];var side=Vector3.Cross(d.normalized,Vector3.up).normalized*width*.5f;if(side.sqrMagnitude<.0001f)side=Vector3.right*width*.5f;
    v.Add(path[i]-side);v.Add(path[i]+side);if(i>0){int k=i*2;t.AddRange(new[]{k-2,k,k-1,k-1,k,k+1,k-1,k,k-2,k+1,k,k-1});}
   }
   MeshArt.Mesh(p,name,v.ToArray(),t.ToArray(),null,material);
  }
  static void Ridge(Transform p,string name,float length,float height,float width,int sections,Material material,int seed)
  {
   var v=new List<Vector3>();var t=new List<int>();
   for(int i=0;i<=sections;i++){
    float f=(float)i/sections,x=(f-.5f)*length,h=Mathf.Sin(f*Mathf.PI)*height;var z=Mathf.Sin(f*5+seed)*width*.15f;
    v.Add(new Vector3(x,0,z-width));v.Add(new Vector3(x,h,z));v.Add(new Vector3(x,0,z+width));
    if(i>0){int k=i*3;t.AddRange(new[]{k-3,k-2,k,k,k-2,k+1,k-2,k-1,k+1,k+1,k-1,k+2});}
   }
   var go=MeshArt.Mesh(p,name,v.ToArray(),t.ToArray(),null,material);LandscapeGeometry.JoinNormals(go.GetComponent<MeshFilter>().sharedMesh);
  }
  static void Star(Transform p,string name,Vector3 pos,float radius,Material mat,int points)
  {
   var v=new List<Vector3>{pos};var t=new List<int>();for(int i=0;i<points*2;i++){float a=i*Mathf.PI/points,r=radius*(i%2==0?1:.43f);v.Add(pos+new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r));t.AddRange(new[]{0,(i+1)%(points*2)+1,i+1});}
   MeshArt.Mesh(p,name,v.ToArray(),t.ToArray(),null,mat);
  }
 }
}
