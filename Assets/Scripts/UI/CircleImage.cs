using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.RisingSteps.UI
{
 public sealed class CircleImage:Image
 {
  protected override void OnPopulateMesh(VertexHelper v){v.Clear();var r=rectTransform.rect;var center=r.center;float radius=Mathf.Min(r.width,r.height)*.5f;int sides=64;v.AddVert(center,color,new Vector2(.5f,.5f));for(int i=0;i<=sides;i++){float a=i*Mathf.PI*2/sides;var p=new Vector2(Mathf.Cos(a),Mathf.Sin(a));v.AddVert(center+p*radius,color,p*.5f+Vector2.one*.5f);if(i<sides)v.AddTriangle(0,i+1,i+2);}int start=v.currentVertCount;var edge=new Color(.85f,.95f,1,.85f);for(int i=0;i<=sides;i++){float a=i*Mathf.PI*2/sides;var p=new Vector2(Mathf.Cos(a),Mathf.Sin(a));v.AddVert(center+p*radius,edge,Vector2.zero);v.AddVert(center+p*(radius-2),edge,Vector2.zero);if(i<sides){int k=start+i*2;v.AddTriangle(k,k+1,k+2);v.AddTriangle(k+2,k+1,k+3);}}}
 }
}
