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
   if(mesh.uv.Length==vertices.Length)mesh.RecalculateTangents();
  }
  // Preserve positions/UVs but introduce shallow fracture planes in the render
  // mesh. The collision fan is a separate mesh and never enters this method.
  public static void FractureNormals(Mesh mesh,float strength)
  {
   JoinNormals(mesh);var source=mesh.vertices;var uv=mesh.uv;var normal=mesh.normals;var indices=mesh.triangles;
   var vertices=new Vector3[indices.Length];var tex=new Vector2[indices.Length];var normals=new Vector3[indices.Length];var triangles=new int[indices.Length];
   for(int i=0;i<indices.Length;i+=3){
    var face=Vector3.Cross(source[indices[i+1]]-source[indices[i]],source[indices[i+2]]-source[indices[i]]).normalized;
    for(int j=0;j<3;j++){int k=i+j,old=indices[k];vertices[k]=source[old];tex[k]=uv[old];normals[k]=Vector3.Lerp(normal[old],face,strength).normalized;triangles[k]=k;}
   }
   mesh.Clear();mesh.vertices=vertices;mesh.uv=tex;mesh.triangles=triangles;mesh.normals=normals;mesh.RecalculateBounds();mesh.RecalculateTangents();
  }
  // Project masonry grain at a consistent world scale. Adjacent faces sample
  // different deposits instead of each triangular box face repeating one stamp.
  public static void StoneUv(GameObject block,float scale=.8f)
  {
   var mesh=block.GetComponent<MeshFilter>().sharedMesh;var vertices=mesh.vertices;var normals=mesh.normals;var uv=new Vector2[vertices.Length];
   for(int i=0;i<vertices.Length;i++){
    var point=block.transform.localRotation*vertices[i]+block.transform.localPosition;var n=block.transform.localRotation*normals[i];
    uv[i]=(Mathf.Abs(n.y)>.7f?new Vector2(point.x,point.z):Mathf.Abs(n.x)>Mathf.Abs(n.z)?new Vector2(point.z,point.y):new Vector2(point.x,point.y))*scale;
   }
   mesh.uv=uv;mesh.RecalculateTangents();
  }
  // An original modeled shoulder kit, merged into one mesh per island. Small
  // bevels catch the sun; gaps and unequal beds avoid a cylindrical stone collar.
  public static void MineralPlates(Transform parent,float[] boundary,int index,Material stone)
  {
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   void Face(Vector3 a,Vector3 b,Vector3 c){int k=v.Count;v.AddRange(new[]{a,b,c});foreach(var p in new[]{a,b,c})uv.Add(new Vector2(p.x+p.z*.37f,p.y)*.8f);t.AddRange(new[]{k,k+1,k+2});}
   for(int bed=0;bed<2;bed++)for(int segment=0;segment<12;segment++){
    if((segment+index+bed)%4==0)continue;
    float angle=(segment+bed*.43f)*Mathf.PI/6,phase=segment*2.13f+index*.71f+bed;
    var outward=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));var across=new Vector3(outward.z,0,-outward.x);
    float radius=boundary[(segment*2+bed)%boundary.Length]*(bed==0?.995f:.94f);
    float width=.47f+.12f*Mathf.Sin(phase),height=.34f+.10f*Mathf.Cos(phase*1.3f);
    var center=outward*radius+Vector3.up*(bed==0?.31f:-.24f)+Vector3.up*.027f*Mathf.Sin(phase);
    var outline=new[]{new Vector2(-.42f,-.5f),new Vector2(.42f,-.5f),new Vector2(.5f,-.36f),new Vector2(.5f,.36f),new Vector2(.42f,.5f),new Vector2(-.42f,.5f),new Vector2(-.5f,.36f),new Vector2(-.5f,-.36f)};
    var back=new Vector3[8];var front=new Vector3[8];
    for(int corner=0;corner<8;corner++){
     var p=across*(outline[corner].x*width)+Vector3.up*(outline[corner].y*height);
     back[corner]=center+p-outward*.12f;front[corner]=center+p*.86f+outward*(.10f+.019f*Mathf.Sin(corner*2.4f+phase));
    }
    for(int corner=0;corner<8;corner++){int next=(corner+1)%8;Face(center+outward*.13f,front[corner],front[next]);Face(center-outward*.12f,back[next],back[corner]);Face(back[corner],back[next],front[corner]);Face(front[corner],back[next],front[next]);}
   }
   var go=MeshArt.Mesh(parent,"MineralShoulderPlates",v.ToArray(),t.ToArray(),uv.ToArray(),stone);go.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
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
   var go=MeshArt.Mesh(parent,name,v.ToArray(),t.ToArray(),uv.ToArray(),material);go.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();go.transform.localPosition=position;return go;
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
