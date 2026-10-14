using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class TempleRouteArt
 {
  public static StepMarker Build(Transform parent,int index,bool scenic)
  {
   int variant=index==12?2:index%4==0?1:0;var library=TemplePlatformLibrary.Load();var model=Object.Instantiate(library.Prefabs[variant],parent,false);model.name="OriginalTemplePlatform";var bindings=model.GetComponent<TemplePlatformBindings>();bindings.Apply(parent);float half=bindings.Width*.5f;
   const int n=24;var v=new Vector3[n+1];var uv=new Vector2[n+1];var t=new int[n*3];v[0]=Vector3.up*IslandArt.Surface;
   for(int i=0;i<n;i++){float a=i*Mathf.PI*2/n,r=half/Mathf.Max(Mathf.Abs(Mathf.Cos(a)),Mathf.Abs(Mathf.Sin(a)));v[i+1]=new Vector3(Mathf.Cos(a)*r,IslandArt.Surface,Mathf.Sin(a)*r);uv[i+1]=new Vector2(v[i+1].x,v[i+1].z)*.15f;t[i*3]=0;t[i*3+1]=(i+1)%n+1;t[i*3+2]=i+1;}
   var ground=MeshArt.Mesh(parent,"TempleSurface",v,t,uv,MeshArt.Stone("Realm2Ground",RealmArt.Ground(2)),!scenic);ground.GetComponent<Renderer>().enabled=false;
   if(!scenic){var marker=ground.AddComponent<StepMarker>();marker.Index=index;Frame(parent,"GoldenLandingRim",half-.22f,.020f,MeshArt.Mat("RouteGold",new Color(.91f,.63f,.15f),-1,true,.60f));var contrast=Frame(parent,"RouteContrastRim",half-.18f,.030f,MeshArt.Mat("RouteContrast",new Color(.025f,.07f,.10f)));contrast.SetActive(false);Frame(parent,"InnerRouteRim",half-.26f,.008f,MeshArt.Mat("RouteLight",new Color(.95f,.77f,.36f),-1,true,.30f));return marker;}
   return null;
  }
  static GameObject Frame(Transform p,string name,float half,float width,Material material)
  {
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();foreach(float radius in new[]{half,half-width})foreach(var c in new[]{new Vector2(-1,-1),new Vector2(-1,1),new Vector2(1,1),new Vector2(1,-1)}){v.Add(new Vector3(c.x*radius,.609f,c.y*radius));uv.Add(c*radius*.15f);}
   for(int i=0;i<4;i++){int next=(i+1)%4;t.AddRange(new[]{i,next,i+4,next,next+4,i+4});}return MeshArt.Mesh(p,name,v.ToArray(),t.ToArray(),uv.ToArray(),material);
  }
#if UNITY_EDITOR
  public static TemplePlatformBindings Author(Transform parent,int variant,Material[] materials)
  {
   float width=variant==2?6.0f:variant==1?5.2f:4.2f,half=width*.5f;var root=new GameObject("TemplePlatform"+variant);root.transform.SetParent(parent,false);
   GameObject Block(string name,Vector3 p,Vector3 size,int role,float bevel=.045f){var block=LandscapeGeometry.BeveledBlock(root.transform,name,p,size,materials[role],bevel);LandscapeGeometry.StoneUv(block,.20f);return block;}
   // Broad rectilinear masonry piers have vertical faces; no circular cliff/tip.
   Block("TemplePierCore",new Vector3(0,-1.7f,0),new Vector3(width-.22f,4.4f,width-.22f),0,.085f);
   Block("TemplePierFoot",new Vector3(0,-3.95f,0),new Vector3(width+.16f,.34f,width+.16f),0,.07f);
   Block("TempleCornice",new Vector3(0,.27f,0),new Vector3(width+.20f,.36f,width+.20f),0,.05f);
   for(int row=0;row<3;row++)for(int side=0;side<4;side++)for(int brick=0;brick<4;brick++){
    bool xFace=side<2;float sign=side%2==0?-1:1,along=(brick-1.5f)*(width/4f)+(row%2)*.03f;var p=xFace?new Vector3(sign*(half-.05f),-.28f-row*.56f,along):new Vector3(along,-.28f-row*.56f,sign*(half-.05f));
    Block("TempleMasonryCourse",p,xFace?new Vector3(.22f,.50f,width/4-.035f):new Vector3(width/4-.035f,.50f,.22f),0);
   }
   float tile=width/3;for(int x=0;x<3;x++)for(int z=0;z<3;z++)Block("TemplePaving",new Vector3((x-1)*tile,.505f,(z-1)*tile),new Vector3(tile-.016f,.19f,tile-.016f),1,.018f);
   for(int side=0;side<4;side++)for(int motif=0;motif<5;motif++){
    float a=side*Mathf.PI*.5f;var outwards=new Vector3(Mathf.Sin(a),0,Mathf.Cos(a));var across=new Vector3(outwards.z,0,-outwards.x);var point=outwards*(half+.12f)+across*(motif-2)*width*.17f+Vector3.up*.23f;
    var plate=Block("CarvedFrieze",point,new Vector3(.36f,.13f,.018f),2,.004f);plate.transform.localRotation=Quaternion.Euler(0,a*Mathf.Rad2Deg,0);
    var diamond=Block("FriezeDiamond",point+outwards*.012f,new Vector3(.11f,.11f,.022f),3,.012f);diamond.transform.localRotation=Quaternion.Euler(0,a*Mathf.Rad2Deg,45);
   }
   for(int square=0;square<3;square++){var seal=MeshArt.Ring(root.transform,"TempleSeal",.63f-square*.16f,.045f,.611f,materials[2],4);seal.transform.localRotation=Quaternion.Euler(0,square==1?45:0,0);var mesh=seal.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices;var tex=new Vector2[points.Length];for(int i=0;i<points.Length;i++)tex[i]=new Vector2(points[i].x,points[i].z);mesh.uv=tex;mesh.RecalculateTangents();}
   for(int side=-1;side<=1;side+=2){float height=variant==0?1.25f:side==-1?4.5f:3.2f;var p=new Vector3(side*(half+.90f),.50f,-half*.42f);
    Block("TempleColumnWing",p-Vector3.up*.25f,new Vector3(1.65f,.50f,1.65f),0);
    RisingTempleSanctuary.Column(root.transform,p,height,materials[0],materials[2],side,true);
    if(variant>0)RisingTempleSanctuary.Banner(root.transform,p+new Vector3(side*.58f,height-.42f,0),side,materials[2],variant);
   }
   for(int stair=0;stair<4;stair++)Block("TempleStair",new Vector3(half+.15f+stair*.20f,.43f-stair*.13f,half*.55f),new Vector3(.45f,.14f,1.10f),0);
   for(int side=-1;side<=1;side+=2){var ledge=new Vector3(side*(half+.46f),.36f,half*.47f);Block("TempleHerbShelf",ledge,new Vector3(.68f,.34f,1.58f),0);Herbs(root.transform,ledge+Vector3.up*.17f,side,materials[7],materials[5]);}
   var bindings=root.AddComponent<TemplePlatformBindings>();bindings.Width=width;var surfaces=new List<TempleSurfaceBinding>();
   foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>()){
    int role=System.Array.IndexOf(materials,renderer.sharedMaterial);if(role<0)role=renderer.sharedMaterial.name=="TempleBanner"?4:5;renderer.sharedMaterial=materials[role];surfaces.Add(new TempleSurfaceBinding{Renderer=renderer,Role=(TempleMaterialRole)role});
   }bindings.Surfaces=surfaces.ToArray();return bindings;
  }
  static void Herbs(Transform parent,Vector3 center,int seed,Material leaves,Material flowers)
  {
   var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();var fv=new List<Vector3>();var fu=new List<Vector2>();var ft=new List<int>();
   void Face(Vector3 a,Vector3 b,Vector3 c){int k=v.Count;v.AddRange(new[]{a,b,c,c,b,a});uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.up,Vector2.right,Vector2.zero});t.AddRange(new[]{k,k+1,k+2,k+3,k+4,k+5});}
   for(int cluster=0;cluster<3;cluster++)for(int stem=0;stem<5;stem++){
    float a=stem*2.39996f+cluster*.70f+seed;var p=center+new Vector3(Mathf.Cos(a)*.07f,.0f,(cluster-1)*.42f+Mathf.Sin(a)*.08f);float height=.34f+stem%3*.08f;var top=p+new Vector3(Mathf.Cos(a)*.10f,height,Mathf.Sin(a)*.10f);var right=new Vector3(Mathf.Cos(a+.7f),0,Mathf.Sin(a+.7f))*.012f;
    Face(p-right,p+right,top);
    for(int pair=0;pair<3;pair++)for(int side=-1;side<=1;side+=2){var basePoint=Vector3.Lerp(p,top,.22f+pair*.19f);var d=new Vector3(Mathf.Cos(a+pair*.9f),.40f,Mathf.Sin(a+pair*.9f))*side*.12f;var across=Vector3.Cross(d,Vector3.up).normalized*.026f;var tip=basePoint+d;var middle=(basePoint+tip)*.5f+Vector3.up*.018f;Face(basePoint,tip,middle+across);Face(basePoint,middle-across,tip);}
    for(int blossom=0;blossom<4;blossom++){
     var c=Vector3.Lerp(p,top,.75f+blossom*.075f);float radius=.038f-(blossom%2)*.006f;int k=fv.Count;fv.AddRange(new[]{c+Vector3.up*radius,c+Vector3.down*radius,c+Vector3.right*radius,c+Vector3.forward*radius,c+Vector3.left*radius,c+Vector3.back*radius});for(int j=0;j<6;j++)fu.Add(new Vector2(j%2,j/2f));
     for(int side=0;side<4;side++){int x=k+2+side,next=k+2+(side+1)%4;ft.AddRange(new[]{k,next,x,k+1,x,next});}
    }
   }
   MeshArt.Mesh(parent,"TempleHerbLeaves",v.ToArray(),t.ToArray(),uv.ToArray(),leaves);MeshArt.Mesh(parent,"TempleHerbFlowers",fv.ToArray(),ft.ToArray(),fu.ToArray(),flowers);
  }
#endif
 }
}
