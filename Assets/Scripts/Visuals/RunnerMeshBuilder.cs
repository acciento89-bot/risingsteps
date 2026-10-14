using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamilunavo.RisingSteps.Visuals
{
 // Character-only shaped surfaces. Detail is accumulated per joint/material, not
 // emitted as one renderer per cord, finger, hair lock or cargo seam.
 internal sealed class RunnerMeshBuilder
 {
  readonly Transform _parent;
  readonly string _name;
  readonly Material _material;
  readonly List<Vector3> _vertices=new();
  readonly List<Vector2> _uv=new();
  readonly List<int> _indices=new();

  public RunnerMeshBuilder(Transform parent,string name,Material material){_parent=parent;_name=name;_material=material;}
  public void Loft(Vector3[] centers,Vector2[] radii,int sides=12,float roundness=1,bool faceted=false)
  {
   if(centers.Length!=radii.Length||centers.Length<2)throw new ArgumentException("Runner loft requires matching cross sections");
   int start=_vertices.Count;
   for(int ring=0;ring<centers.Length;ring++)for(int side=0;side<=sides;side++){
    float angle=side*Mathf.PI*2/sides;
    float x=Mathf.Cos(angle),z=Mathf.Sin(angle);
    _vertices.Add(centers[ring]+new Vector3(Mathf.Sign(x)*Mathf.Pow(Mathf.Abs(x),roundness)*radii[ring].x,0,Mathf.Sign(z)*Mathf.Pow(Mathf.Abs(z),roundness)*radii[ring].y));
    _uv.Add(new Vector2((float)side/sides,(float)ring/(centers.Length-1)));
   }
   Connect(start,centers.Length,sides);
   Cap(start,centers[0],sides,false);Cap(start+(centers.Length-1)*(sides+1),centers[centers.Length-1],sides,true);
   if(faceted)FacetFrom(start);
  }
  public void Profile(Vector3 origin,float[] heights,float[] widths,float[] depths,int sides=12,float roundness=1)
  {
   var centers=new Vector3[heights.Length];var radii=new Vector2[heights.Length];
   for(int i=0;i<heights.Length;i++){centers[i]=origin+Vector3.up*heights[i];radii[i]=new Vector2(widths[i],depths[i]);}
   Loft(centers,radii,sides,roundness);
  }
  // Shallow inset folds run through the authored cloth profile. The original
  // radii remain the outer envelope; no joint, body size or animation changes.
  public void FabricProfile(Vector3 origin,float[] heights,float[] widths,float[] depths,int sides=16,float phase=0)
  {
   int start=_vertices.Count;Profile(origin,heights,widths,depths,sides,.79f);
   for(int ring=0;ring<heights.Length;ring++)for(int side=0;side<=sides;side++){
    int k=start+ring*(sides+1)+side;float a=side*Mathf.PI*2/sides,f=ring/(float)(heights.Length-1);
    float crease=Mathf.Pow(.5f+.5f*Mathf.Sin(a*5+phase+f*.85f),3);
    float inset=(.006f+.004f*Mathf.Sin(f*Mathf.PI))*crease*Mathf.Sin(f*Mathf.PI);
    var p=_vertices[k];p.x-=Mathf.Cos(a)*inset;p.z-=Mathf.Sin(a)*inset;_vertices[k]=p;
   }
  }
  public void FoldedHood()
  {
   const int columns=12;float[] heights={.62f,.605f,.545f,.47f,.425f},widths={.11f,.17f,.18f,.14f,.045f},depths={-.12f,-.17f,-.193f,-.17f,-.15f};
   var points=new Vector3[heights.Length*(columns+1)];
   for(int row=0;row<heights.Length;row++)for(int col=0;col<=columns;col++){
    float u=col/(float)columns*2-1;points[row*(columns+1)+col]=new Vector3(u*widths[row],heights[row]+u*u*.016f,depths[row]+u*u*.022f+Mathf.Cos(u*Mathf.PI*3)*.007f);
   }
   // Independent back surface prevents opposite cloth faces cancelling normals.
   for(int face=0;face<2;face++){
    int start=_vertices.Count;_vertices.AddRange(points);
    for(int row=0;row<heights.Length;row++)for(int col=0;col<=columns;col++)_uv.Add(new Vector2(col/(float)columns,row/(float)(heights.Length-1)));
    for(int row=0;row<heights.Length-1;row++)for(int col=0;col<columns;col++){
     int a=start+row*(columns+1)+col,b=a+columns+1;
     _indices.AddRange(face==0?new[]{a,b,a+1,a+1,b,b+1}:new[]{a,a+1,b,a+1,b+1,b});
    }
   }
  }
  // A path sweep gives a draped hood, bent fingers and authored seams a continuous
  // contour. The cross section follows the path, rather than world vertical.
  public void Sweep(Vector3[] path,float[] widths,float[] depths,int sides=8,bool faceted=false)
  {
   if(path.Length<2||widths.Length!=path.Length||depths.Length!=path.Length)throw new ArgumentException("Runner sweep requires matching path sections");
   int start=_vertices.Count;Vector3 previousRight=Vector3.zero;
   for(int ring=0;ring<path.Length;ring++){
    Vector3 tangent=(path[Mathf.Min(ring+1,path.Length-1)]-path[Mathf.Max(0,ring-1)]).normalized;
    Vector3 axis=Mathf.Abs(Vector3.Dot(tangent,Vector3.forward))>.9f?Vector3.right:Vector3.forward;
    // Parallel transport avoids an abrupt quarter-turn where the hood's path
    // crosses an axis threshold. Elliptical folds retain a coherent orientation.
    Vector3 right=ring==0?Vector3.Cross(tangent,axis):previousRight-tangent*Vector3.Dot(previousRight,tangent);
    if(right.sqrMagnitude<.001f)right=Vector3.Cross(tangent,axis);
    right.Normalize();Vector3 forward=Vector3.Cross(right,tangent).normalized;previousRight=right;
    for(int side=0;side<=sides;side++){
     float angle=side*Mathf.PI*2/sides;
     _vertices.Add(path[ring]+right*Mathf.Cos(angle)*widths[ring]+forward*Mathf.Sin(angle)*depths[ring]);
     _uv.Add(new Vector2((float)side/sides,(float)ring/(path.Length-1)));
    }
   }
   Connect(start,path.Length,sides);Cap(start,path[0],sides,false);Cap(start+(path.Length-1)*(sides+1),path[path.Length-1],sides,true);
   if(faceted)FacetFrom(start);
  }
  public void Stitch(Vector3 from,Vector3 to,float width)
  {Sweep(new[]{from,to},new[]{width,width},new[]{width*.65f,width*.65f},5);}
  public void Pebble(Vector3 center,Vector3 size,int sides=10)
  {Profile(center,new[]{-.5f*size.y,-.32f*size.y,0,.32f*size.y,.5f*size.y},new[]{.18f*size.x,.42f*size.x,.5f*size.x,.42f*size.x,.18f*size.x},new[]{.18f*size.z,.42f*size.z,.5f*size.z,.42f*size.z,.18f*size.z},sides);}
  public void HairCrown()
  {
   const int sides=20,rings=6;int start=_vertices.Count;
   float[] height={-.05f,.045f,.15f,.235f,.285f,.315f},width={.17f,.21f,.215f,.19f,.13f,.04f},depth={.13f,.165f,.17f,.15f,.10f,.035f};
   for(int ring=0;ring<rings;ring++)for(int side=0;side<=sides;side++){
    float angle=side*Mathf.PI*2/sides,x=Mathf.Cos(angle),z=Mathf.Sin(angle),front=Mathf.Max(0,z);
    float ridge=(Mathf.Sin(angle*7+ring*.55f)*.007f+Mathf.Cos(angle*11-ring*.35f)*.004f)*(ring==5?.3f:1);
    float y=height[ring]+(ring==0?front*front*.15f:0)+ridge;
    _vertices.Add(new Vector3(x*(width[ring]+ridge),y,z*(depth[ring]+ridge)-.035f));
    _uv.Add(new Vector2((float)side/sides,(float)ring/(rings-1)));
   }
   Connect(start,rings,sides);Cap(start,new Vector3(0,.025f,-.035f),sides,false);Cap(start+(rings-1)*(sides+1),new Vector3(0,.315f,-.035f),sides,true);
  }
  // Shoe outline is asymmetric from heel to toe, with a broad toe box and
  // narrower heel. Rings lie horizontally; upper/sole share the authored last.
  public void Shoe(Vector3 origin,float bottom,float top,float inset)
  {
   Vector2[] outline={new(-.075f,-.14f),new(.075f,-.14f),new(.106f,-.07f),new(.115f,.07f),new(.107f,.20f),new(.064f,.255f),new(-.064f,.255f),new(-.107f,.20f),new(-.115f,.07f),new(-.106f,-.07f)};
   int start=_vertices.Count,count=outline.Length;
   for(int ring=0;ring<3;ring++)for(int i=0;i<=count;i++){
    Vector2 p=outline[i%count];float scale=ring==0?.96f: ring==1?1:.90f;
    float y=ring==0?bottom:ring==1?Mathf.Lerp(bottom,top,.22f):top-(p.y>.12f?(p.y-.12f)*.18f:0);
    _vertices.Add(origin+new Vector3(p.x*(scale-inset),y,p.y*(scale-inset)));
    _uv.Add(new Vector2((float)i/count,ring*.5f));
   }
   Connect(start,3,count);Cap(start,origin+Vector3.up*bottom,count,false);Cap(start+2*(count+1),origin+Vector3.up*(top-.025f),count,true);
  }
  void Connect(int start,int rings,int sides)
  {for(int ring=0;ring<rings-1;ring++)for(int side=0;side<sides;side++){int a=start+ring*(sides+1)+side,b=a+sides+1;_indices.AddRange(new[]{a,b,a+1,a+1,b,b+1});}}
  void Cap(int start,Vector3 center,int sides,bool top)
  {
   int middle=_vertices.Count;_vertices.Add(center);_uv.Add(new Vector2(.5f,top?1:0));
   for(int side=0;side<sides;side++)_indices.AddRange(top?new[]{start+side,middle,start+side+1}:new[]{start+side,start+side+1,middle});
  }
  void FacetFrom(int start)
  {
   int first=_indices.FindIndex(index=>index>=start);var vertices=new List<Vector3>();var uv=new List<Vector2>();
   for(int i=first;i<_indices.Count;i++){int old=_indices[i];vertices.Add(_vertices[old]);uv.Add(_uv[old]);_indices[i]=start+i-first;}
   _vertices.RemoveRange(start,_vertices.Count-start);_uv.RemoveRange(start,_uv.Count-start);_vertices.AddRange(vertices);_uv.AddRange(uv);
  }
  public GameObject Finish()
  {
   if(_vertices.Count==0)return null;
   // Shared mesh creation supplies normal/bounds checks and the existing mesh
   // lifetime component. No static batching across animated joints.
   var model=MeshArt.Mesh(_parent,_name,_vertices.ToArray(),_indices.ToArray(),_uv.ToArray(),_material);JoinUvNormals(model.GetComponent<MeshFilter>().sharedMesh);return model;
  }
  // UV copies on the same surface share their lighting. Coincident front/back
  // cloth vertices remain separate hemispheres and cannot cancel one another.
  static void JoinUvNormals(Mesh mesh)
  {
   var vertices=mesh.vertices;var source=mesh.normals;var result=new Vector3[source.Length];var groups=new Dictionary<Vector3,List<int>>();
   for(int i=0;i<vertices.Length;i++){if(!groups.TryGetValue(vertices[i],out var group)){group=new List<int>();groups.Add(vertices[i],group);}group.Add(i);}
   for(int i=0;i<vertices.Length;i++){var sum=Vector3.zero;foreach(int peer in groups[vertices[i]])if(Vector3.Dot(source[i],source[peer])>.01f)sum+=source[peer];result[i]=sum.normalized;}
   mesh.normals=result;if(mesh.uv.Length==mesh.vertexCount)mesh.RecalculateTangents();
  }
 }
}
