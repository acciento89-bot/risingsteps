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
   for(int flower=0;flower<3;flower++){
    float a=seed*.41f+flower*2.4f,h=.15f+.06f*Mathf.Sin(seed+flower*4.1f);
    var root=position+new Vector3(Mathf.Cos(a)*.055f,0,Mathf.Sin(a)*.055f);
    var head=root+new Vector3(.028f*Mathf.Sin(a),h*1.2f,.021f*Mathf.Cos(a));
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
  // Folded leaves have an actual raised midrib and independent front/back faces.
  // This keeps sunlight and shadow normals valid without a transparent leaf card.
  static void Leaf(List<Vector3> v,List<Vector2> uv,List<int> t,Vector3 basePoint,Vector3 tip,Vector3 side,float fold)
  {
   var middle=Vector3.Lerp(basePoint,tip,.49f);var ridge=middle+Vector3.up*fold;
   var points=new[]{basePoint,middle-side,tip,middle+side,ridge};
   void Triangle(int a,int b,int c){int k=v.Count;v.AddRange(new[]{points[a],points[b],points[c]});uv.AddRange(new[]{new Vector2(a%2,.1f+a*.17f),new Vector2(b%2,.1f+b*.17f),new Vector2(c%2,.1f+c*.17f)});t.AddRange(new[]{k,k+1,k+2});}
   for(int edge=0;edge<4;edge++){int next=(edge+1)%4;Triangle(edge,4,next);Triangle(next,4,edge);}
  }
  public static void CascadePlants(Transform parent,int index,float size)
  {
   var moss=LandscapePalette.Moss(parent);var leaves=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int plant=0;plant<3;plant++){
    float angle=index*.39f+plant*2.16f;var outward=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle));var across=new Vector3(-outward.z,0,outward.x);
    var basePoint=outward*size*.99f+Vector3.up*.604f;
    for(int frond=0;frond<5;frond++){
     float yaw=(frond-2)*.35f;var direction=(outward*Mathf.Cos(yaw)+across*Mathf.Sin(yaw)).normalized;
     float length=.59f+(frond%3)*.12f;
     for(int pair=0;pair<6;pair++){
      float f=(pair+1)/7f;var center=basePoint+direction*(length*f)+Vector3.up*(Mathf.Sin(f*Mathf.PI*.8f)*.22f+.035f);
      float spread=.17f*(1-f)+.035f;var side=new Vector3(-direction.z,0,direction.x);
      for(int sign=-1;sign<=1;sign+=2)Leaf(leaves,uv,t,center,center+side*(sign*spread)+direction*.075f, direction*.039f,.018f);
     }
    }
   }
   MeshArt.Mesh(parent,"CascadeFernFronds",leaves.ToArray(),t.ToArray(),uv.ToArray(),moss);
   leaves.Clear();uv.Clear();t.Clear();
   for(int vine=0;vine<9;vine++){
    float a=vine*Mathf.PI*2/9+index*.39f;var radial=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a));var across=new Vector3(-radial.z,0,radial.x);
    float length=.76f+(vine%3)*.24f;var path=new Vector3[8];
    for(int node=0;node<8;node++){
     float f=node/7f;path[node]=radial*(size*(.99f-f*.095f)+.025f)+across*Mathf.Sin(f*5+vine)*.065f+Vector3.up*(.58f-length*f);
     if(node>0)for(int shoot=0;shoot<3;shoot++){
      int sign=(node+shoot)%2==0?1:-1;var joint=path[node]+Vector3.up*shoot*.035f+across*sign*.015f;
      Leaf(leaves,uv,t,joint,joint+across*sign*(.20f-shoot*.023f)+Vector3.down*(.11f+shoot*.016f),radial*(.064f-shoot*.006f),.028f);
     }
    }
    Branch(parent,vine==0?"CascadeHangingMoss":"CascadeVineStem",path,new[]{.011f,.011f,.010f,.009f,.008f,.007f,.005f,.003f},moss,5);
   }
   MeshArt.Mesh(parent,"CascadeTrailingLeaves",leaves.ToArray(),t.ToArray(),uv.ToArray(),moss);
  }
  public static void TempleHerbs(Transform parent,int index,float size)
  {
   var leaf=LandscapePalette.Surface(parent,2,3);var petals=MeshArt.Mat("TempleHeatherPetals",new Color(.49f,.25f,.55f));
   var foliage=new List<Vector3>();var fuv=new List<Vector2>();var ft=new List<int>();var flowers=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int plant=0;plant<4;plant++){
    float angle=plant*1.61f+index*.29f;var p=new Vector3(Mathf.Cos(angle)*size*1.02f,.59f,Mathf.Sin(angle)*size*1.02f);
    for(int stem=0;stem<4;stem++){
     float a=stem*2.4f+plant;var lean=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*.12f;float h=.22f+(stem%3)*.055f;
     for(int tier=0;tier<3;tier++){
      var center=p+lean*(tier/3f)+Vector3.up*(.055f+tier*.055f);
      var radial=new Vector3(Mathf.Cos(a+tier),0,Mathf.Sin(a+tier));Leaf(foliage,fuv,ft,center,center+radial*.11f+Vector3.up*.024f,new Vector3(-radial.z,0,radial.x)*.022f,.01f);
     }
     for(int bloom=0;bloom<3;bloom++){
      var center=p+lean+Vector3.up*(h-bloom*.031f);
      for(int petal=0;petal<4;petal++){float b=petal*Mathf.PI*.5f;var radial=new Vector3(Mathf.Cos(b),0,Mathf.Sin(b));Leaf(flowers,uv,t,center,center+radial*.028f+Vector3.up*.018f,new Vector3(-radial.z,0,radial.x)*.015f,.013f);}
     }
    }
   }
   MeshArt.Mesh(parent,"TempleHerbLeaves",foliage.ToArray(),ft.ToArray(),fuv.ToArray(),leaf);MeshArt.Mesh(parent,"TempleHerbFlowers",flowers.ToArray(),t.ToArray(),uv.ToArray(),petals);
  }
  public static void Tree(Transform parent,Vector3 position,int seed,float scale,bool distant=false)
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
    LeafCluster(root.transform,"DeciduousLeafClusters",center,size,cluster%3==0?sun:shade,seed*17+cluster,distant?14:48);
   }
  }
  // An original water-bank tree: low forked trunk, spreading branches and broad
  // leaves. It has no Meadow crown/grass kit and never consumes route randomness.
  public static void CascadeWillow(Transform parent,Vector3 position,int seed,float scale,bool distant=false)
  {
   var root=new GameObject("CascadeWillow");root.transform.SetParent(parent,false);root.transform.localPosition=position;root.transform.localScale=Vector3.one*scale;
   root.transform.localRotation=Quaternion.Euler(0,180,0);
   var bark=MeshArt.Stone("CascadeWillowBark",new Color(.39f,.32f,.20f));
   Branch(root.transform,"WillowTrunk",new[]{Vector3.zero,new Vector3(.06f,.40f,.015f),new Vector3(.19f,.94f,.05f),new Vector3(.28f,1.75f,.10f)},new[]{.11f,.095f,.063f,.014f},bark,8);
   for(int side=-1;side<=1;side+=2)Branch(root.transform,"WillowRoot",new[]{new Vector3(side*.24f,.01f,-.10f),new Vector3(.04f,.26f,0)},new[]{.019f,.065f},bark,5);
   for(int cluster=0;cluster<5;cluster++){
    float a=cluster*2.39996f+seed*.17f;var center=new Vector3(.32f+Mathf.Cos(a)*.43f,1.47f+(cluster%3)*.22f,Mathf.Sin(a)*.42f);
    var fork=new Vector3(.12f,.71f+(cluster%2)*.15f,.03f);
    Branch(root.transform,"WillowFork",new[]{fork,Vector3.Lerp(fork,center,.57f)+Vector3.up*.05f,center},new[]{.043f,.028f,.009f},bark,6);
    LeafCluster(root.transform,"WillowLeafClusters",center,new Vector3(.97f,.70f,.88f),LandscapePalette.Surface(parent,1,cluster%3==0?4:3),seed*19+cluster,distant?14:44);
   }
  }
  // The crown is assembled from overlapping, folded leaf blades on several
  // staggered shells. Individual tips make the silhouette; there is no oval core.
  public static void LeafCluster(Transform root,string name,Vector3 center,Vector3 size,Material material,int seed,int leafCount)
  {
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int leaf=0;leaf<leafCount;leaf++){
    float a=leaf*2.39996f+seed*.31f;float latitude=(leaf+.5f)/leafCount*2-1;
    float reach=Mathf.Sqrt(1-latitude*latitude);var radial=new Vector3(Mathf.Cos(a)*reach,latitude*.78f,Mathf.Sin(a)*reach);
    float shell=.25f+(leaf%3)*.085f;var p=center+Vector3.Scale(radial,size)*shell;
    var direction=new Vector3(Mathf.Cos(a+.25f),.22f+latitude*.30f,Mathf.Sin(a+.25f)).normalized;
    float blade=(leafCount<20?.70f:.37f)*(1+.15f*Mathf.Sin(seed+leaf*1.71f));
    var tip=p+Vector3.Scale(direction,size)*blade;
    var side=Vector3.Scale(new Vector3(-direction.z,0,direction.x),size)*blade*(leafCount<20?.53f:.36f);
    Leaf(v,uv,t,p,tip,side,size.y*.035f);
   }
   MeshArt.Mesh(root,name,v.ToArray(),t.ToArray(),uv.ToArray(),material);
   // Three crossed, two-sided alpha-cut leaf layers fill the existing modeled
   // crown. They are static mesh surfaces, with no camera-facing update or collider.
   var cards=new List<Vector3>();var tex=new List<Vector2>();var faces=new List<int>();
   for(int plane=0;plane<3;plane++){
    float a=plane*Mathf.PI/3+seed*.13f;var side=new Vector3(Mathf.Cos(a),0,Mathf.Sin(a))*size.x*.53f;
    var up=Vector3.up*size.y*.46f;var c=center+Vector3.up*size.y*.04f;
    var corners=new[]{c-side-up,c+side-up,c+side+up,c-side+up};
    for(int face=0;face<2;face++){int k=cards.Count;cards.AddRange(corners);tex.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up});faces.AddRange(face==0?new[]{k,k+1,k+2,k,k+2,k+3}:new[]{k,k+2,k+1,k,k+3,k+2});}
   }
   MeshArt.Mesh(root,"CanopyDepthCards",cards.ToArray(),faces.ToArray(),tex.ToArray(),LandscapePalette.Canopy(root));
  }
  public static void LeafMass(Transform root,Vector3 center,Vector3 size,Material material,int seed)
  {
   const int sides=16,rows=9;var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int row=0;row<=rows;row++)for(int i=0;i<=sides;i++){
    float a=(i%sides)*Mathf.PI*2/sides,latitude=row*Mathf.PI/rows;
    float r=row==0||row==rows?0:Mathf.Sin(latitude)*.5f;
    float ragged=1+.12f*Mathf.Sin(a*5+seed+row*.71f)+.075f*Mathf.Cos(a*3-seed*.63f);
    var p=new Vector3(Mathf.Cos(a)*r*ragged,-Mathf.Cos(latitude)*.5f,Mathf.Sin(a)*r*ragged);
    p.y+=r*.18f*Mathf.Sin(a*3+seed);v.Add(center+Vector3.Scale(p,size));uv.Add(new Vector2((float)i/sides*2,(float)row/rows));
    if(row>0&&i<sides){int k=row*(sides+1)+i,lower=k-(sides+1);t.AddRange(new[]{lower,k,lower+1,lower+1,k,k+1});}
   }
   var foliage=MeshArt.Mesh(root,"LeafCrown",v.ToArray(),t.ToArray(),uv.ToArray(),material);LandscapeGeometry.JoinNormals(foliage.GetComponent<MeshFilter>().sharedMesh);
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
