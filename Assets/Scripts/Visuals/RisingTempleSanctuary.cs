using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.RisingSteps.Visuals
{
 // Original architectural kit, replacing scaled copies of playable islands.
 // Near terraces frame the route; the central gate is beyond every campaign fan.
 public static class RisingTempleSanctuary
 {
  public static void Build(Transform parent)
  {
   var root=new GameObject("OriginalTempleSanctuary");root.transform.SetParent(parent,false);
   var stone=LandscapePalette.Surface(parent,2);var gold=LandscapePalette.Surface(parent,2,5);
   for(int bank=0;bank<5;bank++){
    int sign=bank%2==0?-1:1;var p=new Vector3(sign*(9.2f+bank*.25f),-2+bank*1.9f,22+bank*13);
    var terrace=new GameObject("SanctuaryTerrace");terrace.transform.SetParent(root.transform,false);terrace.transform.localPosition=p;
    Cliff(terrace.transform,Vector3.zero,new Vector3(7.2f,10,8.5f),4.75f,281+bank*29,stone);
    for(int level=0;level<3;level++)Block(terrace.transform,"SanctuaryPodium",new Vector3(0,4.88f+level*.23f,0),new Vector3(7.1f-level*.38f,.24f,8.3f-level*.38f),stone,.06f);
    for(int row=0;row<2;row++)for(int brick=0;brick<6;brick++)Block(terrace.transform,"SanctuaryMasonry",new Vector3((brick-2.5f)*1.1f+(row==0?0:.20f),3.96f+row*.47f,-4.08f),new Vector3(1.06f,.44f,.30f),stone,.045f);
    for(int stair=0;stair<5;stair++)Block(terrace.transform,"TempleStair",new Vector3(-sign*1.65f,4.1f+stair*.23f,-3.5f+stair*.38f),new Vector3(2.0f,.24f,.42f),stone,.035f);
    var pillar=new Vector3(-sign*2.05f,5.48f,-.8f);Column(terrace.transform,pillar,bank==3?3.7f:5.3f,stone,gold,bank);
    if(bank<3){Banner(terrace.transform,pillar+new Vector3(sign*.60f,4.55f,0),sign,gold,bank);Brazier(terrace.transform,new Vector3(-sign*2.10f,5.50f,-3.05f),stone,gold);}
    if(bank%2==0){var fallen=Column(terrace.transform,new Vector3(.6f,5.61f,1.6f),1.5f,stone,gold,7+bank);fallen.transform.localRotation=Quaternion.Euler(0,bank*23,78);}
    for(int block=0;block<3;block++){var debris=Block(terrace.transform,"RuinedLintel",new Vector3(sign*.4f+block*.38f,5.48f,-1.8f+block*.57f),new Vector3(.9f,.38f,.55f),stone,.065f);debris.transform.localRotation=Quaternion.Euler(block*3,bank*17+block*39,block*4);}
    var garden=new GameObject("TempleDryGarden");garden.transform.SetParent(terrace.transform,false);garden.transform.localPosition=new Vector3(sign*.4f,4.94f,1);DryHerbs(garden.transform,bank);
   }
   for(int ruin=0;ruin<3;ruin++){
    var p=new Vector3((ruin%2==0?-1:1)*(20+ruin*3),5+ruin*3,83+ruin*18);
    var acropolis=new GameObject("DistantAcropolis");acropolis.transform.SetParent(root.transform,false);acropolis.transform.localPosition=p;
    Cliff(acropolis.transform,Vector3.zero,new Vector3(15,17,14),7.75f,457+ruin*19,stone);
    for(int step=0;step<3;step++)Block(acropolis.transform,"AcropolisPodium",new Vector3(0,8+step*.38f,0),new Vector3(13-step,.40f,10-step*.7f),stone,.08f);
    for(int col=0;col<4;col++)Column(acropolis.transform,new Vector3((col-1.5f)*2.7f,8.95f,-2.3f),ruin==1&&col==3?2.6f:6,stone,gold,ruin+col,true);
    Block(acropolis.transform,"RuinedLintel",new Vector3(-1.35f,15.1f,-2.3f),new Vector3(8.2f,.65f,2.1f),stone,.09f);
    Block(acropolis.transform,"SanctuaryGoldFrieze",new Vector3(-1.35f,15.08f,-3.39f),new Vector3(7.8f,.13f,.075f),gold,.015f);
   }
   Gateway(root.transform,stone,gold);
   // Genuine low-detail distant mountain silhouettes, rather than a painted sky.
   var mountain=LandscapePalette.Surface(parent,2,6);
   for(int i=0;i<6;i++){
    var p=new Vector3(-78+i*30,-9+(i%2)*4,174+(i%3)*9);var v=new List<Vector3>();var t=new List<int>();var uv=new List<Vector2>();
    for(int face=0;face<9;face++){float a=face*Mathf.PI*2/9,b=(face+1)*Mathf.PI*2/9;int k=v.Count;v.AddRange(new[]{new Vector3(Mathf.Cos(a)*22,0,Mathf.Sin(a)*14),new Vector3(5,24+i%3*5,2),new Vector3(Mathf.Cos(b)*22,0,Mathf.Sin(b)*14)});t.AddRange(new[]{k,k+1,k+2});uv.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.right});}
    MeshArt.Mesh(root.transform,"SunsetMountain",v.ToArray(),t.ToArray(),uv.ToArray(),mountain).transform.localPosition=p;
   }
  }
  static void Cliff(Transform parent,Vector3 p,Vector3 size,float top,int seed,Material stone)
  {
   var cliff=LandscapeGeometry.Rock(parent,"TempleAcropolis",p,size,stone,seed);var mesh=cliff.GetComponent<MeshFilter>().sharedMesh;var vertices=mesh.vertices;
   for(int i=0;i<vertices.Length;i++)if(vertices[i].y>size.y*.35f)vertices[i].y=top;
   mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();LandscapeGeometry.JoinNormals(mesh);
   for(int bed=0;bed<4;bed++){var ledge=Block(parent,"TempleSedimentBed",p+new Vector3(Mathf.Sin(seed+bed)*.9f,-size.y*.35f+bed*size.y*.21f,-size.z*.47f),new Vector3(size.x*.72f,.24f,size.z*.12f),stone,.07f);ledge.transform.localRotation=Quaternion.Euler(0,Mathf.Sin(seed+bed)*6,Mathf.Cos(seed+bed)*3);}
  }
  static GameObject Block(Transform p,string name,Vector3 center,Vector3 size,Material material,float bevel)
  {var block=LandscapeGeometry.BeveledBlock(p,name,center,size,material,bevel);LandscapeGeometry.StoneUv(block,.13f);return block;}
  static GameObject Column(Transform parent,Vector3 p,float height,Material stone,Material gold,int seed,bool distant=false)
  {
   var root=new GameObject("TempleColumn");root.transform.SetParent(parent,false);root.transform.localPosition=p;
   Block(root.transform,"CarvedColumnPlinth",new Vector3(0,.13f,0),new Vector3(1.12f,.26f,1.12f),stone,.065f);
   float[] levels={.26f,.40f,.55f,height*.32f,height*.70f,height-.43f,height-.27f,height-.16f};
   float[] radii={.47f,.47f,.35f,.32f,.30f,.34f,.48f,.50f};int sides=distant?24:48;
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int ring=0;ring<levels.Length;ring++)for(int side=0;side<=sides;side++){
    float a=(side%sides)*Mathf.PI*2/sides,r=radii[ring]*(1+.055f*Mathf.Cos(a*12));v.Add(new Vector3(Mathf.Cos(a)*r,levels[ring],Mathf.Sin(a)*r));uv.Add(new Vector2(side/(float)sides*2,levels[ring]*.13f));
    if(ring>0&&side<sides){int k=ring*(sides+1)+side,b=k-(sides+1);t.AddRange(new[]{b,k,b+1,b+1,k,k+1});}
   }
   var shaft=MeshArt.Mesh(root.transform,"SanctuaryFlutedShaft",v.ToArray(),t.ToArray(),uv.ToArray(),stone);LandscapeGeometry.JoinNormals(shaft.GetComponent<MeshFilter>().sharedMesh);
   Block(root.transform,"CarvedColumnCapital",new Vector3(0,height-.06f,0),new Vector3(1.15f,.20f,1.15f),stone,.065f);
   for(int face=0;face<4;face++)for(int ornament=0;ornament<3;ornament++){
    float a=face*Mathf.PI*.5f;var outward=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));var across=new Vector3(outward.z,0,-outward.x);
    var carving=Block(root.transform,"CapitalGoldCarving",outward*.58f+across*(ornament-1)*.25f+Vector3.up*(height-.10f),new Vector3(.115f,.115f,.035f),gold,.013f);carving.transform.localRotation=Quaternion.Euler(0,a*Mathf.Rad2Deg,45);
   }
   for(int band=0;band<2;band++)Ring(root.transform,"ColumnInlay",band==0?.47f:.37f,.028f,band==0?.43f:height-.38f,gold,32);
   return root;
  }
  static void Banner(Transform parent,Vector3 p,int sign,Material gold,int seed)
  {
   var root=new GameObject("TempleBannerMount");root.transform.SetParent(parent,false);root.transform.localPosition=p;
   Block(root.transform,"BannerArm",new Vector3(sign*.32f,.10f,0),new Vector3(.95f,.065f,.065f),gold,.013f);
   var material=MeshArt.Mat("TempleBanner",new Color(.39f,.19f,.40f));var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   const int rows=8;for(int row=0;row<=rows;row++)for(int side=0;side<3;side++){
    float f=row/(float)rows,x=sign*(.02f+side*.38f),y=-f*2.5f+(row==rows&&side==1?.30f:0),z=Mathf.Sin(f*5+side*.9f+seed)*.06f*f;v.Add(new Vector3(x,y,z));uv.Add(new Vector2(side*.5f,f));
    if(row<rows&&side<2){int k=row*3+side;t.AddRange(new[]{k,k+3,k+1,k+1,k+3,k+4,k+1,k+3,k,k+4,k+3,k+1});}
   }
   MeshArt.Mesh(root.transform,"TempleBanner",v.ToArray(),t.ToArray(),uv.ToArray(),material);
   var sigil=Ring(root.transform,"BannerSunSigil",.19f,.025f,0,gold,24);sigil.transform.localPosition=new Vector3(sign*.38f,-.78f,-.07f);sigil.transform.localRotation=Quaternion.Euler(90,0,0);
   for(int ray=0;ray<8;ray++){float a=ray*Mathf.PI*.25f;var carving=Block(root.transform,"BannerSunRay",new Vector3(sign*.38f+Mathf.Cos(a)*.28f,-.78f+Mathf.Sin(a)*.28f,-.075f),new Vector3(.10f,.025f,.018f),gold,.004f);carving.transform.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg);}
  }
  static void DryHerbs(Transform parent,int seed)
  {
   var leaves=LandscapePalette.Surface(parent,2,3);var petals=MeshArt.Mat("TempleHeatherPetals",new Color(.49f,.25f,.55f));
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();var flowers=new List<Vector3>();var fu=new List<Vector2>();var ft=new List<int>();
   for(int plant=0;plant<3;plant++)for(int stem=0;stem<3;stem++){
    float a=plant*2.3f+stem*.77f+seed;var p=new Vector3(Mathf.Cos(plant*2.3f)*1.8f+stem*.06f,.6f,Mathf.Sin(plant*2.3f)*1.8f);float h=.30f+stem*.07f;
    LandscapeBotany.Blade(v,uv,t,p,a,h,.11f);LandscapeBotany.Blade(v,uv,t,p,a+2.1f,h*.7f,.09f);
    for(int blossom=0;blossom<4;blossom++){
     var center=p+new Vector3(Mathf.Sin(a)*.09f,h-blossom*.045f,Mathf.Cos(a)*.09f);int k=flowers.Count;var left=new Vector3(.038f,0,0);var up=Vector3.up*.031f;
     flowers.AddRange(new[]{center-left,center+up,center+left,center-up});fu.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.one,Vector2.right});ft.AddRange(new[]{k,k+1,k+2,k,k+2,k+3,k+2,k+1,k,k+3,k+2,k});
    }
   }
   MeshArt.Mesh(parent,"TempleHerbLeaves",v.ToArray(),t.ToArray(),uv.ToArray(),leaves);MeshArt.Mesh(parent,"TempleHerbFlowers",flowers.ToArray(),ft.ToArray(),fu.ToArray(),petals);
  }
  static GameObject Ring(Transform parent,string name,float radius,float width,float y,Material material,int sides)
  {
   var ring=MeshArt.Ring(parent,name,radius,width,y,material,sides);var mesh=ring.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices;var uv=new Vector2[points.Length];
   for(int i=0;i<points.Length;i++)uv[i]=new Vector2(points[i].x,points[i].z)*.13f;mesh.uv=uv;mesh.RecalculateTangents();return ring;
  }
  static void Brazier(Transform p,Vector3 center,Material stone,Material gold)
  {
   Block(p,"BrazierPlinth",center+Vector3.up*.14f,new Vector3(.8f,.28f,.8f),stone,.055f);
   var bowl=MeshArt.Lathe(p,"TempleBrazier",new[]{0f,.18f,.36f,.43f},new[]{.22f,.35f,.57f,.60f},24,gold,Vector3.one);bowl.transform.localPosition=center+Vector3.up*.28f;
   var fire=MeshArt.Mat("PortalFlame",new Color(1,.65f,.20f),-1,true,1.25f);
   for(int flame=0;flame<3;flame++)MeshArt.Oval(p,"SanctuaryFlame",center+new Vector3((flame-1)*.18f,1.02f,flame%2*.13f),new Vector3(.22f,.60f+(flame%2)*.2f,.21f),fire,8);
  }
  static void Gateway(Transform parent,Material stone,Material gold)
  {
   var root=new GameObject("DistantSanctuaryGateway");root.transform.SetParent(parent,false);root.transform.localPosition=new Vector3(0,12,110);
   Cliff(root.transform,Vector3.down*5,new Vector3(22,10,15),4.75f,812,stone);
   for(int step=0;step<6;step++)Block(root.transform,"GatewayStair",new Vector3(0,.15f+step*.33f,-7.1f+step*.55f),new Vector3(9.0f,.35f,.60f),stone,.07f);
   for(int side=-1;side<=1;side+=2){Column(root.transform,new Vector3(side*4.4f,2.0f,0),7.2f,stone,gold,side,true);for(int brick=0;brick<5;brick++)Block(root.transform,"GatewayJamb",new Vector3(side*3.7f,2.3f+brick*1.0f,0),new Vector3(1.25f,.95f,1.5f),stone,.07f);Brazier(root.transform,new Vector3(side*6.5f,.30f,-4),stone,gold);}
   for(int brick=0;brick<13;brick++){float a=brick*Mathf.PI/13+.018f,b=(brick+1)*Mathf.PI/13-.018f;LandscapeGeometry.ArchStone(root.transform,"SanctuaryArchVoussoir",a,b,3.1f,4.4f,7,0,stone,.78f);}
   Block(root.transform,"GatewayCornice",new Vector3(0,11.6f,0),new Vector3(10.4f,.55f,2),stone,.08f);
   Block(root.transform,"GatewayGoldFrieze",new Vector3(0,11.55f,-1.06f),new Vector3(10,.13f,.06f),gold,.013f);
  }
 }
}
