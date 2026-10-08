using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class IslandArt
 {
  public const float Surface=.6f;
  public static StepMarker Build(Transform parent,int index,int realm,bool scenic=false){var random=new System.Random(117+index*79+realm*1901);const int n=24;float size=index==12?2.05f:1.55f;var radius=new float[n];for(int i=0;i<n;i++)radius[i]=size*(.91f+(float)random.NextDouble()*.13f);
   var rock=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();float[] y={-3.1f,-2.2f,-.75f,.48f,.6f};float[] taper={.05f,.48f,.88f,1,.99f};
   for(int j=0;j<5;j++)for(int i=0;i<=n;i++){float a=i*Mathf.PI*2/n;float r=radius[i%n]*taper[j];rock.Add(new Vector3(Mathf.Cos(a)*r,y[j],Mathf.Sin(a)*r));uv.Add(new Vector2((float)i/n,(float)j/4));}
   for(int j=0;j<4;j++)for(int i=0;i<n;i++){int a=j*(n+1)+i,b=a+n+1;triangles.AddRange(new[]{a,b,a+1,a+1,b,b+1});}
   MeshArt.Mesh(parent,"LimestoneCliff",rock.ToArray(),triangles.ToArray(),uv.ToArray(),MeshArt.Mat("Limestone",new Color(.88f,.86f,.78f),1));
   var gv=new Vector3[n+1];var guv=new Vector2[n+1];var gt=new int[n*3];gv[0]=Vector3.up*Surface;guv[0]=Vector2.one*.5f;
   for(int i=0;i<n;i++){float a=i*Mathf.PI*2/n;gv[i+1]=new Vector3(Mathf.Cos(a)*radius[i],Surface,Mathf.Sin(a)*radius[i]);guv[i+1]=new Vector2(gv[i+1].x/size,gv[i+1].z/size)*.5f+Vector2.one*.5f;gt[i*3]=0;gt[i*3+1]=(i+1)%n+1;gt[i*3+2]=i+1;}
   var ground=MeshArt.Mesh(parent,"MeadowSurface",gv,gt,guv,MeshArt.Mat("Meadow",new Color(.66f,.87f,.54f),0),!scenic);StepMarker marker=null;if(!scenic){marker=ground.AddComponent<StepMarker>();marker.Index=index;
    MeshArt.Ring(parent,"GoldenLandingRim",size*.74f,.075f,Surface+.018f,MeshArt.Mat("RouteGold",new Color(1,.76f,.12f),-1,true));var contrast=MeshArt.Ring(parent,"RouteContrastRim",size*.76f,.025f,Surface+.017f,MeshArt.Mat("RouteContrast",new Color(.025f,.07f,.10f)));contrast.SetActive(false);
    MeshArt.Ring(parent,"InnerRouteRim",size*.70f,.015f,Surface+.02f,MeshArt.Mat("RouteLight",new Color(1,.95f,.56f),-1,true));}
   Vegetation(parent,radius,size,random,realm,index);
   if(index%3==0||scenic)Waterfall(parent,new Vector3(-size*.72f,.52f,size*.5f),realm);
   return marker;
  }
  private static void Vegetation(Transform parent,float[] radii,float size,System.Random random,int realm,int index){var grass=new List<Vector3>();var uv=new List<Vector2>();var tri=new List<int>();
   for(int i=0;i<80;i++){float a=(float)random.NextDouble()*Mathf.PI*2;float r=size*(.76f+(float)random.NextDouble()*.16f);var p=new Vector3(Mathf.Cos(a)*r,.61f,Mathf.Sin(a)*r);float h=.12f+(float)random.NextDouble()*.20f;int start=grass.Count;grass.Add(p+new Vector3(-.055f,0,0));grass.Add(p+new Vector3(.015f,h,.04f));grass.Add(p+new Vector3(.055f,0,0));uv.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.right});tri.AddRange(new[]{start,start+1,start+2,start+2,start+1,start});}
   MeshArt.Mesh(parent,"MeadowBlades",grass.ToArray(),tri.ToArray(),uv.ToArray(),MeshArt.Mat("GrassBlades",new Color(.24f,.47f,.08f)));
   for(int i=0;i<7;i++){float a=(float)random.NextDouble()*Mathf.PI*2;var p=new Vector3(Mathf.Cos(a)*size*.88f,.68f,Mathf.Sin(a)*size*.88f);for(int k=0;k<5;k++){float b=k*Mathf.PI*2/5;MeshArt.Oval(parent,"DaisyPetal",p+new Vector3(Mathf.Cos(b)*.045f,0,Mathf.Sin(b)*.045f),new Vector3(.085f,.025f,.04f),MeshArt.Mat("Petal",new Color(1,.98f,.84f)),8);}MeshArt.Oval(parent,"DaisyCenter",p+Vector3.up*.018f,new Vector3(.04f,.028f,.04f),MeshArt.Mat("Pollen",new Color(1,.69f,.10f)),8);}
   if(index%4==0){var tree=new GameObject("WindTree");tree.transform.SetParent(parent,false);tree.transform.localPosition=new Vector3(size*.78f,.6f,-size*.42f);tree.transform.localScale=Vector3.one*(index==12?.8f:.5f);
    MeshArt.Lathe(tree.transform,"CurvedTrunk",new[]{0f,.4f,1.2f,1.7f},new[]{.17f,.13f,.10f,.015f},8,MeshArt.Mat("Bark",new Color(.40f,.24f,.09f)),Vector3.one,true);
    for(int i=0;i<5;i++){float a=i*Mathf.PI*2/5;MeshArt.Oval(tree.transform,"LeafCrown",new Vector3(Mathf.Cos(a)*.55f,1.6f+(i%2)*.24f,Mathf.Sin(a)*.55f),new Vector3(1.35f,.95f,1.3f),MeshArt.Mat("Foliage",realm==1?new Color(.62f,.9f,.84f):new Color(.69f,.95f,.48f),2),12);}}
  }
  public static void Waterfall(Transform parent,Vector3 position,int realm){var root=new GameObject("Waterfall");root.transform.SetParent(parent,false);root.transform.localPosition=position;var mat=MeshArt.Mat("Waterfall",new Color(.72f,.94f,1),3);
   for(int i=0;i<3;i++){float x=(i-1)*.13f;var go=MeshArt.Mesh(root.transform,"WaterRibbon",new[]{new Vector3(x-.11f,0,0),new Vector3(x+.11f,0,0),new Vector3(x+.18f,-5.2f,.13f),new Vector3(x-.12f,-5.2f,.1f)},new[]{0,1,2,0,2,3,2,1,0,3,2,0},new[]{Vector2.up,Vector2.one,Vector2.right,Vector2.zero},mat);go.AddComponent<WaterfallFlow>();}
  }
 }
 public sealed class WaterfallFlow:MonoBehaviour{private RisingCourse _course;private Renderer _r;private MaterialPropertyBlock _b;private void Awake(){_course=FindFirstObjectByType<RisingCourse>();_r=GetComponent<Renderer>();_b=new MaterialPropertyBlock();}private void Update(){// Geometry stays stable; gentle texture shimmer is optional decoration.
   float light=_course!=null&&_course.Profile.ReducedMotion?1:.94f+.06f*Mathf.Sin(Time.time*1.8f+transform.localPosition.x);_b.SetColor("_Color",new Color(.72f*light,.94f*light,light));_r.SetPropertyBlock(_b);}}
}
