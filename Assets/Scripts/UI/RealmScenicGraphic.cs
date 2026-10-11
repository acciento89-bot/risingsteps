using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Core;
namespace Kamilunavo.RisingSteps.UI
{
 // Original native vector scenes. Each new realm owns a different silhouette and landmark kit.
 public sealed class RealmScenicGraphic:MaskableGraphic
 {
  public int Realm;public bool Hero;
  VertexHelper mesh;Vector2 origin;float scale;
  static readonly Color Ink=new(.08f,.18f,.24f),Gold=new(1,.78f,.32f),Cream=new(.96f,.97f,.91f);
  public static Graphic Add(Transform parent,int realm,Vector2 min,Vector2 max,bool hero){var go=new GameObject("RealmArt",typeof(RectTransform),typeof(CanvasRenderer),typeof(RealmScenicGraphic));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;var art=go.GetComponent<RealmScenicGraphic>();art.Realm=realm;art.Hero=hero;art.raycastTarget=false;art.material=RoundedPanel.GalleryMaterial;return art;}
  protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();mesh=vh;var rect=rectTransform.rect;scale=Mathf.Min(rect.width/240,rect.height/180);origin=new Vector2(rect.center.x-scale*120,rect.center.y-scale*90);switch(RisingRealmCatalog.Theme(Realm)){case RisingRealmTheme.Jungle:Jungle();break;case RisingRealmTheme.Crystal:Crystal();break;case RisingRealmTheme.Frost:Frost();break;case RisingRealmTheme.Desert:Desert();break;case RisingRealmTheme.Volcano:Volcano();break;case RisingRealmTheme.CloudCoast:Coast();break;default:Stars();break;}RoundedPanel.SetGalleryCoordinates(mesh,rect);}
  Vector2 P(Vector2 point)=>origin+point*scale;
  void Poly(Color tint,params Vector2[] points){int start=mesh.currentVertCount;foreach(var p in points)mesh.AddVert(P(p),tint,Vector2.zero);for(int i=1;i<points.Length-1;i++)mesh.AddTriangle(start,start+i,start+i+1);}
  void Box(float x,float y,float w,float h,Color tint)=>Poly(tint,new(x,y),new(x+w,y),new(x+w,y+h),new(x,y+h));
  void Circle(float x,float y,float radius,Color tint,int sides=24){int start=mesh.currentVertCount;mesh.AddVert(P(new(x,y)),tint,Vector2.zero);for(int i=0;i<=sides;i++){float a=i*Mathf.PI*2/sides;mesh.AddVert(P(new(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius)),tint,Vector2.zero);if(i>0)mesh.AddTriangle(start,start+i,start+i+1);}}
  void Line(float ax,float ay,float bx,float by,float width,Color tint){Vector2 a=new(ax,ay),b=new(bx,by),n=new Vector2(-(b-a).y,(b-a).x).normalized*width/2;Poly(tint,a-n,b-n,b+n,a+n);}
  void Sky(Color bottom,Color top){var r=rectTransform.rect;int start=mesh.currentVertCount;mesh.AddVert(new Vector3(r.xMin,r.yMin),bottom,Vector2.zero);mesh.AddVert(new Vector3(r.xMax,r.yMin),bottom,Vector2.zero);mesh.AddVert(new Vector3(r.xMax,r.yMax),top,Vector2.zero);mesh.AddVert(new Vector3(r.xMin,r.yMax),top,Vector2.zero);mesh.AddTriangle(start,start+1,start+2);mesh.AddTriangle(start,start+2,start+3);}
  void Cloud(float x,float y,float amount){Circle(x,y,10*amount,Cream);Circle(x+12*amount,y+5*amount,14*amount,Cream);Circle(x+29*amount,y,11*amount,Cream);Box(x,y-8*amount,30*amount,14*amount,Cream);}
  void Rock(float x,float y,float width,float depth,Color stone,Color cap){Poly(stone,new(x,y),new(x+width,y),new(x+width*.82f,y-depth*.48f),new(x+width*.45f,y-depth),new(x+width*.16f,y-depth*.56f));Poly(new Color(stone.r*.77f,stone.g*.77f,stone.b*.77f),new(x+width*.46f,y),new(x+width,y),new(x+width*.82f,y-depth*.48f),new(x+width*.45f,y-depth));Box(x-2,y,width+4,7,cap);}
  void Tree(float x,float y,float height,Color leaf){Line(x,y,x+height*.10f,y+height*.73f,6,new Color(.32f,.24f,.15f));Circle(x,y+height,height*.32f,leaf);Circle(x-height*.17f,y+height*.88f,height*.27f,leaf);Circle(x+height*.22f,y+height*.85f,height*.24f,new Color(leaf.r*1.20f,leaf.g*1.15f,leaf.b));}
  void Jungle(){Sky(new(.80f,.89f,.61f),new(.23f,.60f,.63f));Circle(194,145,23,new Color(.99f,.83f,.43f));Cloud(7,140,.75f);var leaf=new Color(.10f,.40f,.27f);Rock(35,61,153,47,new(.34f,.42f,.32f),new(.27f,.52f,.29f));Tree(62,68,54,leaf);Tree(132,70,72,leaf);Tree(180,71,49,leaf);Line(61,112,90,91,3,new(.42f,.60f,.30f));Line(90,91,133,128,3,new(.42f,.60f,.30f));for(int i=0;i<4;i++){Circle(86+i*17,70,4,new Color(.93f,.44f,.32f));Box(43+i*36,56,5,13,Gold);}Rock(189,108,39,20,new(.34f,.42f,.32f),leaf);}
  void Shard(float x,float y,float height,float width,Color tint){Poly(tint,new(x-width/2,y),new(x-width/2,y+height*.70f),new(x,y+height),new(x+width/2,y+height*.70f),new(x+width/2,y));Poly(new Color(tint.r*.65f,tint.g*.73f,tint.b*.83f),new(x,y),new(x,y+height),new(x+width/2,y+height*.70f),new(x+width/2,y));Line(x-width*.30f,y+height*.66f,x,y+height*.92f,2,Cream);}
  void Crystal(){Sky(new(.28f,.34f,.54f),new(.08f,.17f,.29f));Poly(new(.13f,.20f,.29f),new(0,180),new(50,180),new(28,128),new(11,65),new(0,40));Poly(new(.13f,.20f,.29f),new(240,180),new(196,180),new(219,125),new(229,71),new(240,40));Rock(30,56,174,42,new(.31f,.30f,.44f),new(.45f,.43f,.65f));Shard(68,63,71,34,new(.36f,.80f,.88f));Shard(106,63,111,39,new(.61f,.48f,.85f));Shard(149,62,83,36,new(.36f,.80f,.88f));Shard(177,63,44,25,new(.70f,.56f,.89f));for(int i=0;i<7;i++){float x=36+i*28,y=110+(i*23%50);Line(x-3,y,x+3,y,1.6f,Gold);Line(x,y-3,x,y+3,1.6f,Gold);}}
  void Pine(float x,float y,float height){Box(x-2,y,4,height*.8f,new(.28f,.34f,.37f));Poly(new(.15f,.39f,.42f),new(x-height*.3f,y+height*.25f),new(x,y+height),new(x+height*.3f,y+height*.25f));Poly(Cream,new(x-height*.14f,y+height*.68f),new(x,y+height),new(x+height*.14f,y+height*.68f));}
  void Frost(){Sky(new(.89f,.95f,.96f),new(.42f,.67f,.80f));Poly(new(.67f,.79f,.83f),new(0,75),new(54,166),new(112,76),new(180,148),new(240,73));Poly(Cream,new(35,135),new(54,166),new(76,131),new(58,142),new(51,133));Rock(33,59,180,36,new(.48f,.62f,.67f),Cream);Pine(67,65,66);Pine(173,66,78);Pine(135,66,35);Box(94,62,30,7,new(.39f,.79f,.89f));for(int i=0;i<10;i++)Circle(14+i*23,128+(i*31%37),1.8f,Cream,8);Cloud(131,155,.61f);}
  void Ellipse(float x,float y,float width,float height,Color tint)
  {
   int start=mesh.currentVertCount;mesh.AddVert(P(new(x,y)),tint,Vector2.zero);
   for(int i=0;i<=24;i++){float angle=i*Mathf.PI*2/24;mesh.AddVert(P(new(x+Mathf.Cos(angle)*width,y+Mathf.Sin(angle)*height)),tint,Vector2.zero);if(i>0)mesh.AddTriangle(start,start+i,start+i+1);}
  }
  void Palm(float x,float y,float height)
  {
   var trunk=new Color(.40f,.31f,.18f);float crownX=x+height*.19f,crownY=y+height;
   Poly(trunk,new(x-3,y),new(x+3,y),new(crownX+2,crownY),new(crownX-2,crownY),new(x+height*.08f,y+height*.48f));
   for(int side=-1;side<=1;side+=2)for(int frond=0;frond<3;frond++){
    float reach=height*(.49f-frond*.10f),rise=height*(.03f+frond*.18f);var leaf=frond==1?new Color(.24f,.46f,.27f):new Color(.16f,.37f,.24f);
    // Broad curved fronds taper into tips, rather than a fan of straight spokes.
    Poly(leaf,new(crownX,crownY),new(crownX+side*reach*.47f,crownY+rise+height*.08f),new(crownX+side*reach,crownY+rise-height*.14f),new(crownX+side*reach*.46f,crownY+rise-height*.01f));
   }
   Circle(crownX-2,crownY-3,3,trunk,8);Circle(crownX+3,crownY-4,2.5f,trunk,8);
  }
  void Desert()
  {
   Sky(new(1,.82f,.55f),new(.94f,.60f,.37f));Circle(180,139,24,new Color(1,.91f,.65f));
   Poly(new(.86f,.61f,.38f),new(0,73),new(42,101),new(99,77),new(164,107),new(240,74),new(240,37),new(0,37));
   Poly(new(.93f,.72f,.44f),new(0,68),new(43,91),new(89,71),new(111,73),new(77,55),new(0,48));
   Poly(new(.94f,.74f,.47f),new(174,79),new(208,94),new(240,73),new(240,51),new(208,64));
   Rock(32,61,175,40,new(.63f,.38f,.24f),new(.95f,.75f,.44f));
   Poly(new(.77f,.49f,.28f),new(79,67),new(132,137),new(178,67));Poly(new(.92f,.65f,.34f),new(79,67),new(132,137),new(132,67));
   // The oasis rim, shallow water and reflected light establish a readable foreground.
   Ellipse(57,67,30,10,new(.91f,.70f,.39f));Ellipse(57,68,27,7,new(.18f,.58f,.62f));Ellipse(60,70,19,3,new(.34f,.75f,.74f));
   Line(41,69,55,69,1.5f,new(.80f,.91f,.80f));Line(60,66,75,66,1.5f,new(.80f,.91f,.80f));
   Palm(36,67,49);Palm(64,67,34);
   for(int i=0;i<4;i++)Poly(new(.70f,.49f,.30f),new(88+i*14,64),new(92+i*14,66),new(98+i*14,65),new(95+i*14,62));
  }
  void Volcano(){Sky(new(.93f,.48f,.32f),new(.21f,.22f,.35f));Circle(178,145,24,new(.96f,.63f,.33f));Poly(new(.30f,.26f,.31f),new(0,58),new(45,126),new(78,67),new(128,159),new(195,65),new(240,104),new(240,0),new(0,0));Poly(new(1,.59f,.17f),new(111,136),new(128,159),new(148,137),new(138,143),new(129,138),new(119,143));Rock(28,56,183,35,new(.19f,.22f,.28f),new(.28f,.28f,.30f));Line(43,53,83,58,7,new(1,.42f,.14f));Line(83,58,102,46,7,new(1,.42f,.14f));Line(102,46,161,53,7,new(1,.42f,.14f));for(int i=0;i<5;i++)Box(60+i*26,63,9,16+(i*9%23),new(.39f,.35f,.39f));for(int i=0;i<8;i++)Circle(30+i*27,108+(i*29%58),1.8f,Gold,8);}
  void Coast(){Sky(new(.63f,.86f,.88f),new(.34f,.66f,.79f));Cloud(10,139,1);Cloud(153,155,.85f);Rock(28,57,102,34,new(.58f,.64f,.59f),new(.90f,.93f,.81f));Rock(157,102,63,29,new(.58f,.64f,.59f),Cream);Box(43,57,58,6,new(.15f,.68f,.73f));for(int i=0;i<6;i++){float x=114+i*10,y=70+i*6;Box(x,y,8,3,Gold);}Box(181,108,13,42,Cream);Box(178,141,19,10,Gold);Poly(new(.14f,.37f,.44f),new(176,151),new(187,164),new(199,151));Line(68,65,68,110,3,Ink);Poly(Cream,new(72,107),new(72,72),new(98,72));Poly(new(.92f,.68f,.40f),new(64,105),new(41,72),new(64,72));Cloud(167,42,1.1f);}
  void Stars(){Sky(new(.30f,.29f,.52f),new(.07f,.12f,.27f));Circle(184,139,27,new(.98f,.91f,.64f));Circle(196,147,24,new(.12f,.17f,.32f));for(int i=0;i<16;i++){float x=10+i*14,y=90+(i*37%79);Line(x-2,y,x+2,y,1.2f,Cream);Line(x,y-2,x,y+2,1.2f,Cream);}Rock(39,57,140,34,new(.30f,.31f,.49f),new(.55f,.53f,.72f));Box(86,64,61,29,new(.70f,.67f,.72f));Circle(117,92,30,new(.30f,.47f,.59f));Box(86,62,61,30,new(.70f,.67f,.72f));Box(108,66,17,29,Ink);Line(126,108,151,136,6,Gold);Circle(155,140,7,Gold);for(int i=0;i<4;i++)Box(47+i*11,63,7,3,Gold);Line(24,115,59,134,1,Gold);Line(59,134,86,116,1,Gold);Circle(24,115,3,Gold);Circle(59,134,3,Gold);Circle(86,116,3,Gold);}
 }
}
