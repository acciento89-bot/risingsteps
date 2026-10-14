using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.RisingSteps.UI
{
 public sealed class HudIcons:MaskableGraphic
 {
  public enum Kind{Menu,Jump,Crystal,Arrow,Eye,Pause,Mountain}public Kind Icon;
  public static void Add(Transform parent,Kind kind){var go=new GameObject("Icon",typeof(RectTransform),typeof(CanvasRenderer),typeof(HudIcons));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=Vector2.one*.20f;r.anchorMax=Vector2.one*.80f;r.offsetMin=r.offsetMax=Vector2.zero;var g=go.GetComponent<HudIcons>();g.Icon=kind;g.color=Color.white;g.raycastTarget=false;}
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();var r=rectTransform.rect;Vector2 P(float x,float y)=>new(r.xMin+x*r.width,r.yMin+y*r.height);
   void Poly(params Vector2[] points){int start=v.currentVertCount;foreach(var p in points)v.AddVert(p,color,Vector2.zero);for(int i=1;i<points.Length-1;i++)v.AddTriangle(start,start+i,start+i+1);}
   void Stroke(float x,float y,float xx,float yy,float width){var a=P(x,y);var b=P(xx,yy);var d=(b-a).normalized;var n=new Vector2(-d.y,d.x)*Mathf.Min(r.width,r.height)*width*.5f;Poly(a-n,b-n,b+n,a+n);}
   void Circle(float x,float y,float radius){var p=new Vector2[18];for(int i=0;i<p.Length;i++){float a=i*Mathf.PI*2/p.Length;p[i]=P(x+Mathf.Cos(a)*radius,y+Mathf.Sin(a)*radius);}Poly(p);}
   if(Icon==Kind.Menu){for(int i=0;i<3;i++){float y=.20f+i*.26f;Poly(P(.12f,y),P(.88f,y),P(.88f,y+.10f),P(.12f,y+.10f));}}
   else if(Icon==Kind.Pause){Poly(P(.25f,.18f),P(.39f,.18f),P(.39f,.82f),P(.25f,.82f));Poly(P(.61f,.18f),P(.75f,.18f),P(.75f,.82f),P(.61f,.82f));}
   else if(Icon==Kind.Jump){Circle(.61f,.81f,.10f);Stroke(.58f,.65f,.43f,.40f,.13f);Stroke(.57f,.60f,.77f,.56f,.09f);Stroke(.77f,.56f,.86f,.70f,.09f);Stroke(.51f,.58f,.30f,.52f,.09f);Stroke(.30f,.52f,.17f,.65f,.09f);Stroke(.45f,.39f,.65f,.25f,.11f);Stroke(.65f,.25f,.84f,.27f,.10f);Stroke(.43f,.39f,.35f,.23f,.11f);Stroke(.35f,.23f,.15f,.18f,.10f);}
   else if(Icon==Kind.Eye){for(int i=0;i<24;i++){float a=i*Mathf.PI*2/24,b=(i+1)*Mathf.PI*2/24;Stroke(.5f+Mathf.Cos(a)*.39f,.5f+Mathf.Sin(a)*.25f,.5f+Mathf.Cos(b)*.39f,.5f+Mathf.Sin(b)*.25f,.07f);}Circle(.5f,.5f,.105f);}
   else if(Icon==Kind.Crystal)Poly(P(.5f,.05f),P(.95f,.5f),P(.5f,.95f),P(.05f,.5f));
   else if(Icon==Kind.Mountain){Poly(P(.03f,.12f),P(.39f,.94f),P(.76f,.12f));Poly(P(.48f,.12f),P(.74f,.73f),P(.98f,.12f));}
   else {Poly(P(.36f,.08f),P(.64f,.08f),P(.64f,.56f),P(.36f,.56f));Poly(P(.10f,.51f),P(.90f,.51f),P(.5f,.95f));}
  }
 }
}
