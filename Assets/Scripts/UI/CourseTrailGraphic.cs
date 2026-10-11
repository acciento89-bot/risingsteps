using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.UI
{
 public sealed class CourseTrailGraphic:MaskableGraphic
 {
  public int Realm,Level;
  protected override void OnPopulateMesh(VertexHelper mesh){mesh.Clear();var points=CoursePatterns.LevelPoints(Realm,Level);var rect=rectTransform.rect;float end=Mathf.Max(1,points[12].z);Vector2 Map(Vector3 p)=>new(rect.xMin+rect.width*(.5f+p.x/9),rect.yMin+rect.height*(.1f+p.z/end*.8f));
   for(int i=1;i<points.Length;i++){var a=Map(points[i-1]);var b=Map(points[i]);var normal=new Vector2(-(b-a).y,(b-a).x).normalized*1.25f;Quad(mesh,a-normal,a+normal,b+normal,b-normal,color);}
   for(int i=0;i<points.Length;i++){var p=Map(points[i]);float r=i==12?4:2.4f;Quad(mesh,p+new Vector2(-r,-r),p+new Vector2(-r,r),p+new Vector2(r,r),p+new Vector2(r,-r),i==12?new Color(.9f,.61f,.1f):color);}
  }
  static void Quad(VertexHelper mesh,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color tint){int start=mesh.currentVertCount;foreach(var p in new[]{a,b,c,d}){var v=UIVertex.simpleVert;v.position=p;v.color=tint;mesh.AddVert(v);}mesh.AddTriangle(start,start+1,start+2);mesh.AddTriangle(start,start+2,start+3);}
 }
}
