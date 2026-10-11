using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.RisingSteps.Visuals
{
 // Original mesh foliage. Each course owns two shared leaf materials; each plant
 // owns its mesh. Seeds style the plants without consuming the gameplay RNG.
 public static class LandscapeBotany
 {
  public static void Blade(List<Vector3> v,List<Vector2> uv,List<int> triangles,Vector3 p,float angle,float height,float width)
  {
   int k=v.Count;var side=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*width*.5f;
   var bend=new Vector3(-Mathf.Sin(angle),0,Mathf.Cos(angle))*height*.40f;
   v.AddRange(new[]{p-side,p+Vector3.up*height*.55f+bend*.25f-side*.56f,p+Vector3.up*height+bend,p+Vector3.up*height*.55f+bend*.25f+side*.56f,p+side});
   uv.AddRange(new[]{Vector2.zero,Vector2.up*.5f,Vector2.up,Vector2.one*.5f,Vector2.right});
   triangles.AddRange(new[]{k,k+1,k+3,k,k+3,k+4,k+1,k+2,k+3,k+3,k+1,k,k+4,k+3,k,k+3,k+2,k+1});
  }
  public static void Flowers(Transform parent,Vector3 position,int seed)
  {
   var stems=LandscapePalette.Foliage(parent,false);var petal=MeshArt.Mat("Petal",new Color(1,.98f,.84f));var pollen=MeshArt.Mat("Pollen",new Color(1,.69f,.10f));
   var vertices=new List<Vector3>();var triangles=new List<int>();
   for(int flower=0;flower<2;flower++){
    float a=seed*.41f+flower*2.4f,h=.15f+.06f*Mathf.Sin(seed+flower*4.1f);
    var root=position+new Vector3(Mathf.Cos(a)*.055f,0,Mathf.Sin(a)*.055f);
    var head=root+new Vector3(.028f*Mathf.Sin(a),h,.021f*Mathf.Cos(a));
    Branch(parent,"DaisyStem",new[]{root,root+Vector3.up*h*.55f,head},new[]{.008f,.006f,.0035f},stems,5);
    for(int k=0;k<6;k++){
     float angle=k*Mathf.PI/3+a;var outwards=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));var side=new Vector3(-outwards.z,0,outwards.x);
     int start=vertices.Count;vertices.AddRange(new[]{head,head+outwards*.043f+side*.026f+Vector3.up*.004f,head+outwards*.083f-Vector3.up*.012f,head+outwards*.043f-side*.026f+Vector3.up*.004f});
     triangles.AddRange(new[]{start,start+1,start+2,start,start+2,start+3,start+2,start+1,start,start+3,start+2,start});
    }
    MeshArt.Oval(parent,"DaisyCenter",head+Vector3.up*.007f,new Vector3(.042f,.025f,.042f),pollen,6);
   }
   MeshArt.Mesh(parent,"DaisyPetal",vertices.ToArray(),triangles.ToArray(),null,petal);
  }
  public static void Tree(Transform parent,Vector3 position,int seed,float scale)
  {
   var root=new GameObject("WindTree");root.transform.SetParent(parent,false);root.transform.localPosition=position;root.transform.localScale=Vector3.one*scale;
   bool upright=(seed/4)%2==1;var bark=MeshArt.Stone("MeadowBarkStone",new Color(.40f,.25f,.12f));
   float lean=upright?.14f:.42f,top=upright?2.72f:2.20f;
   Branch(root.transform,"CurvedTrunk",new[]{Vector3.zero,new Vector3(.06f,.58f,.025f),new Vector3(lean*.55f,1.19f,.04f),new Vector3(lean,top-.25f,.07f)},new[]{.15f,.115f,.072f,.016f},bark,9);
   // Root flares ground the curved trunk and keep its silhouette from a straight peg.
   for(int i=0;i<3;i++){float a=i*Mathf.PI*2/3;Branch(root.transform,"TreeRoot",new[]{new Vector3(Mathf.Cos(a)*.27f,.016f,Mathf.Sin(a)*.27f),new Vector3(.035f,.28f,.016f)},new[]{.018f,.077f},bark,6);}
   var shade=LandscapePalette.Foliage(parent,false);var sun=LandscapePalette.Foliage(parent,true);
   for(int cluster=0;cluster<6;cluster++){
    float a=cluster*2.39f+seed*.19f;
    float spread=upright?.34f:.62f;float h=top-.32f+(cluster%3-1)*(upright?.32f:.19f);
    var center=new Vector3(lean+Mathf.Cos(a)*spread,h,Mathf.Sin(a)*spread*.72f);
    var branchStart=new Vector3(lean*.49f,1.12f+(cluster%2)*.22f,.04f);
    Branch(root.transform,"TreeBranch",new[]{branchStart,Vector3.Lerp(branchStart,center,.58f)+Vector3.down*.10f,center},new[]{.047f,.028f,.008f},bark,7);
    var size=upright?new Vector3(.74f,.94f,.72f):new Vector3(.88f,.85f,.78f);
    size*=.86f+.16f*Mathf.Sin(seed*.7f+cluster*2.1f);
    LeafMass(root.transform,center,size,cluster%3==0?sun:shade,seed*17+cluster);
    LeafSprigs(root.transform,center,size,sun,seed*17+cluster);
   }
  }
  public static void LeafMass(Transform root,Vector3 center,Vector3 size,Material material,int seed)
  {
   const int sides=16,rows=9;var v=new List<Vector3>();var t=new List<int>();
   for(int row=0;row<=rows;row++)for(int i=0;i<=sides;i++){
    float a=(i%sides)*Mathf.PI*2/sides,latitude=row*Mathf.PI/rows;
    float r=row==0||row==rows?0:Mathf.Sin(latitude)*.5f;
    float ragged=1+.12f*Mathf.Sin(a*5+seed+row*.71f)+.075f*Mathf.Cos(a*3-seed*.63f);
    var p=new Vector3(Mathf.Cos(a)*r*ragged,-Mathf.Cos(latitude)*.5f,Mathf.Sin(a)*r*ragged);
    p.y+=r*.18f*Mathf.Sin(a*3+seed);v.Add(center+Vector3.Scale(p,size));
    if(row>0&&i<sides){int k=row*(sides+1)+i,lower=k-(sides+1);t.AddRange(new[]{lower,k,lower+1,lower+1,k,k+1});}
   }
   var foliage=MeshArt.Mesh(root,"LeafCrown",v.ToArray(),t.ToArray(),null,material);LandscapeGeometry.JoinNormals(foliage.GetComponent<MeshFilter>().sharedMesh);
  }
  static void LeafSprigs(Transform root,Vector3 center,Vector3 size,Material material,int seed)
  {
   var v=new List<Vector3>();var t=new List<int>();
   for(int leaf=0;leaf<14;leaf++){
    float a=leaf*2.39996f+seed*.31f,y=Mathf.Sin(leaf*1.7f+seed)*.35f;
    var radial=new Vector3(Mathf.Cos(a),y,Mathf.Sin(a)).normalized;
    var p=center+Vector3.Scale(radial,size)*.43f;var side=Vector3.Cross(radial,Vector3.up).normalized*.034f;
    var tip=p+radial*.11f;int k=v.Count;v.AddRange(new[]{p,p+radial*.055f+side,tip,p+radial*.055f-side,p+radial*.055f+Vector3.up*.011f});
    t.AddRange(new[]{k,k+1,k+4,k+1,k+2,k+4,k+2,k+3,k+4,k+3,k,k+4,k+4,k+1,k,k+4,k+2,k+1,k+4,k+3,k+2,k+4,k,k+3});
   }
   MeshArt.Mesh(root,"LeafSprigs",v.ToArray(),t.ToArray(),null,material);
  }
  public static void Branch(Transform root,string name,Vector3[] path,float[] radii,Material material,int sides)
  {
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int ring=0;ring<path.Length;ring++){
    var tangent=(path[Mathf.Min(path.Length-1,ring+1)]-path[Mathf.Max(0,ring-1)]).normalized;
    var across=Vector3.Cross(tangent,Vector3.forward).normalized;if(across.sqrMagnitude<.1f)across=Vector3.Cross(tangent,Vector3.up).normalized;
    var depth=Vector3.Cross(across,tangent).normalized;
    for(int i=0;i<=sides;i++){
     float a=(i%sides)*Mathf.PI*2/sides;v.Add(path[ring]+(across*Mathf.Cos(a)+depth*Mathf.Sin(a))*radii[ring]);uv.Add(new Vector2((float)i/sides,ring*.55f));
     if(ring>0&&i<sides){int k=ring*(sides+1)+i,lower=k-(sides+1);t.AddRange(new[]{lower,k,lower+1,lower+1,k,k+1});}
    }
   }
   int first=v.Count;v.Add(path[0]);uv.Add(Vector2.zero);int last=v.Count;v.Add(path[path.Length-1]);uv.Add(Vector2.one);
   for(int i=0;i<sides;i++){int end=(path.Length-1)*(sides+1)+i;t.AddRange(new[]{first,i,i+1,last,end+1,end});}
   var branch=MeshArt.Mesh(root,name,v.ToArray(),t.ToArray(),uv.ToArray(),material);LandscapeGeometry.JoinNormals(branch.GetComponent<MeshFilter>().sharedMesh);
  }
 }
}
