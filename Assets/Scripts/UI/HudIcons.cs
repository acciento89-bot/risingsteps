using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.RisingSteps.UI
{
 public sealed class HudIcons:MaskableGraphic
 {
  public enum Kind{Menu,Jump,Crystal,Arrow}public Kind Icon;
  public static void Add(Transform parent,Kind kind){var go=new GameObject("Icon",typeof(RectTransform),typeof(CanvasRenderer),typeof(HudIcons));go.transform.SetParent(parent,false);var r=(RectTransform)go.transform;r.anchorMin=Vector2.one*.20f;r.anchorMax=Vector2.one*.80f;r.offsetMin=r.offsetMax=Vector2.zero;var g=go.GetComponent<HudIcons>();g.Icon=kind;g.color=Color.white;g.raycastTarget=false;}
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();var r=rectTransform.rect;Vector2 P(float x,float y)=>new(r.xMin+x*r.width,r.yMin+y*r.height);
   void Poly(params Vector2[] points){int start=v.currentVertCount;foreach(var p in points)v.AddVert(p,color,Vector2.zero);for(int i=1;i<points.Length-1;i++)v.AddTriangle(start,start+i,start+i+1);}
   if(Icon==Kind.Menu){for(int i=0;i<3;i++){float y=.20f+i*.26f;Poly(P(.12f,y),P(.88f,y),P(.88f,y+.10f),P(.12f,y+.10f));}}
   else if(Icon==Kind.Crystal)Poly(P(.5f,.05f),P(.95f,.5f),P(.5f,.95f),P(.05f,.5f));
   else {Poly(P(.36f,.08f),P(.64f,.08f),P(.64f,.56f),P(.36f,.56f));Poly(P(.10f,.51f),P(.90f,.51f),P(.5f,.95f));}
  }
 }
}
