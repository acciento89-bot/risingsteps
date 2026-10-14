using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // One broad continuous ruined landmass supports the complete masonry course.
 // It is below the scored roofs; immediate recovery prevents entering it.
 public static class TempleRouteFoundation
 {
  public static float TopY(Vector3[] points,float z){int i=0;while(i<points.Length-2&&z>points[i+1].z)i++;float f=Mathf.InverseLerp(points[i].z,points[i+1].z,z);return Mathf.Lerp(points[i].y,points[i+1].y,f)+IslandArt.Surface-2.4f;}
  public static void Build(Transform parent,Vector3[] points)
  {
   var root=new GameObject("ConnectedTempleFoundation");root.transform.SetParent(parent,false);var stone=LandscapePalette.Surface(parent,2);var v=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();int rows=56;float start=points[0].z-5,end=points[^1].z+6;
   float CenterX(float z){int i=0;while(i<points.Length-2&&z>points[i+1].z)i++;return Mathf.Lerp(points[i].x,points[i+1].x,Mathf.InverseLerp(points[i].z,points[i+1].z,z));}
   for(int ring=0;ring<4;ring++)for(int row=0;row<=rows;row++){
    float z=Mathf.Lerp(start,end,row/(float)rows),top=TopY(points,z),width=ring==0?5.8f:ring==1?6.35f:ring==2?5.8f:4.5f;float y=top-(ring==0?0:ring==1?2.6f:ring==2?6.4f:9.0f);
    for(int side=-1;side<=1;side+=2){float jagged=(ring==0?.20f:.45f)*Mathf.Sin(row*.79f+ring*1.7f);var p=new Vector3(CenterX(z)+side*(width+jagged),y,z);v.Add(p);uv.Add(new Vector2(p.x+p.z*.24f,p.y)*.15f);}
   }
   int stride=(rows+1)*2;
   for(int row=0;row<rows;row++){
    int k=row*2;triangles.AddRange(new[]{k,k+2,k+1,k+1,k+2,k+3});int bottom=3*stride+k;triangles.AddRange(new[]{bottom,bottom+1,bottom+2,bottom+1,bottom+3,bottom+2});
    for(int ring=0;ring<3;ring++){int a=ring*stride+k,b=a+stride;triangles.AddRange(new[]{a,b,a+2,a+2,b,b+2,a+1,a+3,b+1,a+3,b+3,b+1});}
   }
   for(int cap=0;cap<2;cap++){int k=cap==0?0:rows*2;for(int ring=0;ring<3;ring++){int a=ring*stride+k,b=a+stride;if(cap==0)triangles.AddRange(new[]{a,a+1,b,a+1,b+1,b});else triangles.AddRange(new[]{a,b,a+1,a+1,b,b+1});}}
   var mass=MeshArt.Mesh(root.transform,"TempleFoundationRock",v.ToArray(),triangles.ToArray(),uv.ToArray(),stone);LandscapeGeometry.JoinNormals(mass.GetComponent<MeshFilter>().sharedMesh);
   for(int bed=0;bed<3;bed++)for(int block=0;block<12;block++){
    float z=Mathf.Lerp(start+1,end-1,block/11f);int sign=block%2==0?-1:1;var p=new Vector3(CenterX(z)+sign*5.7f,TopY(points,z)-.30f-bed*1.7f,z);var ledge=LandscapeGeometry.BeveledBlock(root.transform,"TempleFoundationMasonry",p,new Vector3(1.1f,.48f,3.6f),stone,.075f);LandscapeGeometry.StoneUv(ledge,.15f);
   }
   MeshArt.Batch(root);
  }
 }
}
