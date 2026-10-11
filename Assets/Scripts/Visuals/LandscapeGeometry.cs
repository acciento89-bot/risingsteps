using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.RisingSteps.Visuals
{
 public static class LandscapeGeometry
 {
  // A UV seam must not become a lighting crease on a continuous rock or leaf mass.
  public static void JoinNormals(Mesh mesh)
  {
   var vertices=mesh.vertices;var normals=mesh.normals;var joined=new Dictionary<Vector3,Vector3>();
   for(int i=0;i<vertices.Length;i++){joined.TryGetValue(vertices[i],out var sum);joined[vertices[i]]=sum+normals[i];}
   for(int i=0;i<vertices.Length;i++)normals[i]=joined[vertices[i]].normalized;
   mesh.normals=normals;
  }
  // Closed small eroded outcrops, deliberately irregular rather than sphere/cylinder props.
  public static GameObject Rock(Transform parent,string name,Vector3 position,Vector3 size,Material material,int seed)
  {
   const int sides=8;var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   var points=new Vector3[3,sides];
   for(int ring=0;ring<3;ring++)for(int i=0;i<sides;i++){
    float a=i*Mathf.PI*2/sides,r=(ring==1?.52f:.40f)*(1+.17f*Mathf.Sin(i*3.7f+seed*.91f));
    points[ring,i]=Vector3.Scale(new Vector3(Mathf.Cos(a)*r,(ring-1)*.50f+(ring==1?0:.06f*Mathf.Sin(i*2.1f+seed)),Mathf.Sin(a)*r),size);
   }
   void Triangle(Vector3 a,Vector3 b,Vector3 c){int start=v.Count;v.AddRange(new[]{a,b,c});uv.AddRange(new[]{new Vector2(a.x+a.z,a.y),new Vector2(b.x+b.z,b.y),new Vector2(c.x+c.z,c.y)});t.AddRange(new[]{start,start+1,start+2});}
   for(int j=0;j<2;j++)for(int i=0;i<sides;i++){int next=(i+1)%sides;Triangle(points[j,i],points[j+1,i],points[j,next]);Triangle(points[j,next],points[j+1,i],points[j+1,next]);}
   for(int i=0;i<sides;i++){int next=(i+1)%sides;Triangle(Vector3.up*size.y*.49f,points[2,next],points[2,i]);Triangle(Vector3.down*size.y*.51f,points[0,i],points[0,next]);}
   var go=MeshArt.Mesh(parent,name,v.ToArray(),t.ToArray(),uv.ToArray(),material);go.transform.localPosition=position;return go;
  }
  // True annular wedge masonry, including front, back, intrados and extrados.
  public static GameObject ArchStone(Transform parent,string name,float start,float end,float inner,float outer,float centerY,float z,Material material,float halfDepth=.26f)
  {
   Vector3 P(float a,float r,float depth)=>new Vector3(Mathf.Cos(a)*r,centerY+Mathf.Sin(a)*r,z+depth);
   var p=new[]{P(start,inner,-halfDepth),P(end,inner,-halfDepth),P(end,outer,-halfDepth),P(start,outer,-halfDepth),P(start,inner,halfDepth),P(end,inner,halfDepth),P(end,outer,halfDepth),P(start,outer,halfDepth)};
   int[] faces={0,2,1,0,3,2,4,5,6,4,6,7,0,1,5,0,5,4,3,7,6,3,6,2,0,4,7,0,7,3,1,2,6,1,6,5};
   for(int i=0;i<faces.Length;i+=3){int swap=faces[i+1];faces[i+1]=faces[i+2];faces[i+2]=swap;}
   var v=new Vector3[faces.Length];var uv=new Vector2[faces.Length];var t=new int[faces.Length];
   for(int i=0;i<faces.Length;i++){v[i]=p[faces[i]];uv[i]=new Vector2(v[i].x*.7f,v[i].y*.7f+v[i].z*.2f);t[i]=i;}
   return MeshArt.Mesh(parent,name,v,t,uv,material);
  }
 }
}
