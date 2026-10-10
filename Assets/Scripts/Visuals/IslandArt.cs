using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class IslandArt
 {
  public const float Surface=.6f;
  public static StepMarker Build(Transform parent,int index,int realm,bool scenic=false){var random=new System.Random(117+index*79+realm*1901);const int n=24;float size=index==12?2.05f:1.55f;var radius=new float[n];for(int i=0;i<n;i++)radius[i]=size*(.91f+(float)random.NextDouble()*.13f);
   BuildCliff(parent,radius,index,realm);
   var gv=new Vector3[n+1];var guv=new Vector2[n+1];var gt=new int[n*3];gv[0]=Vector3.up*Surface;guv[0]=Vector2.one*.5f;
   for(int i=0;i<n;i++){float a=i*Mathf.PI*2/n;gv[i+1]=new Vector3(Mathf.Cos(a)*radius[i],Surface,Mathf.Sin(a)*radius[i]);guv[i+1]=new Vector2(gv[i+1].x/size,gv[i+1].z/size)*.5f+Vector2.one*.5f;gt[i*3]=0;gt[i*3+1]=(i+1)%n+1;gt[i*3+2]=i+1;}
   var ground=MeshArt.Mesh(parent,realm==2?"TempleSurface":realm==1?"CascadeSurface":"MeadowSurface",gv,gt,guv,realm==0?MeshArt.Mat("Realm0Ground",RealmArt.Ground(0),0):MeshArt.Stone("Realm"+realm+"Ground",RealmArt.Ground(realm)),!scenic);StepMarker marker=null;if(!scenic){marker=ground.AddComponent<StepMarker>();marker.Index=index;
    MeshArt.Ring(parent,"GoldenLandingRim",size*.74f,.032f,Surface+.018f,MeshArt.Mat("RouteGold",new Color(.91f,.63f,.15f),-1,true,.60f));var contrast=MeshArt.Ring(parent,"RouteContrastRim",size*.76f,.025f,Surface+.017f,MeshArt.Mat("RouteContrast",new Color(.025f,.07f,.10f)));contrast.SetActive(false);
    MeshArt.Ring(parent,"InnerRouteRim",size*.70f,.006f,Surface+.02f,MeshArt.Mat("RouteLight",new Color(.95f,.77f,.36f),-1,true,.30f));}
   Vegetation(parent,radius,size,random,realm,index);RealmArt.Dress(parent,index,realm,size,scenic);
   return marker;
  }
  // Separate seed preserves the landing boundary and the existing vegetation random stream.
  private static void BuildCliff(Transform parent,float[] radius,int index,int realm){const int n=24;var random=new System.Random(7919+index*193+realm*3571);
   float[] heights=realm==1?new[]{-3.5f,-2.9f,-2.45f,-1.95f,-1.6f,-1.1f,-.7f,.10f,.48f,.6f}:realm==2?new[]{-2.6f,-2.3f,-1.55f,-1.42f,-.7f,-.56f,.48f,.6f}:new[]{-3.1f,-2.58f,-2.30f,-1.54f,-1.32f,-.65f,.48f,.6f};
   float[] taper=realm==1?new[]{.20f,.42f,.43f,.66f,.67f,.86f,.87f,.97f,1,.99f}:realm==2?new[]{.29f,.46f,.62f,.76f,.81f,.94f,1,.99f}:new[]{.21f,.36f,.56f,.62f,.80f,.90f,1,.99f};
   float phase=(float)random.NextDouble()*Mathf.PI*2;float shearX=.10f+(float)random.NextDouble()*.13f,shearZ=.07f+(float)random.NextDouble()*.12f;
   var rings=new Vector3[heights.Length,n];for(int j=0;j<heights.Length;j++){
    bool lip=j>=heights.Length-2;float depth=1-(float)j/(heights.Length-2);float ringPhase=phase+j*.31f;float shift=depth*(.58f+Mathf.Sin(j*.92f+phase)*.16f);
    for(int i=0;i<n;i++){float angle=i*Mathf.PI*2/n;float variation=lip?1:1+.12f*Mathf.Sin(angle*3+phase)+.075f*Mathf.Cos(angle*5-ringPhase)+.08f*Mathf.Sin(angle*2+j*.83f);
     float r=radius[i]*taper[j]*variation;float uneven=lip?0:(.025f+.025f*depth)*Mathf.Sin(angle*3+ringPhase)+.025f*Mathf.Cos(angle*5-phase);
     rings[j,i]=new Vector3(Mathf.Cos(angle)*r+(lip?0:shearX*shift),heights[j]+uneven,Mathf.Sin(angle)*r+(lip?0:shearZ*shift));
    }
   }
   var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
   // Each strata face owns its vertices: actual mesh normals carry broad rock planes,
   // while restrained material grain does not substitute painted outlines for volume.
   for(int j=0;j<heights.Length-1;j++)for(int i=0;i<n;i++){int next=(i+1)%n,start=vertices.Count;vertices.AddRange(new[]{rings[j,i],rings[j+1,i],rings[j,next],rings[j+1,next]});
    float u=(float)i/n,uNext=(float)(i+1)/n,v=(float)j/(heights.Length-1),vNext=(float)(j+1)/(heights.Length-1);uv.AddRange(new[]{new Vector2(u,v),new Vector2(u,vNext),new Vector2(uNext,v),new Vector2(uNext,vNext)});triangles.AddRange(new[]{start,start+1,start+2,start+2,start+1,start+3});
   }
   MeshArt.Mesh(parent,"LimestoneCliff",vertices.ToArray(),triangles.ToArray(),uv.ToArray(),MeshArt.Stone("Realm"+realm+"Cliff",RealmArt.Cliff(realm)));
  }
  private static void Vegetation(Transform parent,float[] radii,float size,System.Random random,int realm,int index){if(realm!=0)return;var grass=new List<Vector3>();var uv=new List<Vector2>();var tri=new List<int>();
   for(int i=0;i<80;i++){float a=(float)random.NextDouble()*Mathf.PI*2;float r=size*(.76f+(float)random.NextDouble()*.16f);var p=new Vector3(Mathf.Cos(a)*r,.61f,Mathf.Sin(a)*r);float h=.08f+(float)random.NextDouble()*.14f;int start=grass.Count;grass.Add(p+new Vector3(-.025f,0,0));grass.Add(p+new Vector3(.015f,h,.04f));grass.Add(p+new Vector3(.025f,0,0));uv.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.right});tri.AddRange(new[]{start,start+1,start+2,start+2,start+1,start});}
   MeshArt.Mesh(parent,"MeadowBlades",grass.ToArray(),tri.ToArray(),uv.ToArray(),MeshArt.Mat("Realm"+realm+"GrassBlades",new Color(.21f,.34f,.16f)));
   for(int i=0;i<10;i++){float a=(float)random.NextDouble()*Mathf.PI*2;var p=new Vector3(Mathf.Cos(a)*size*.88f,.68f,Mathf.Sin(a)*size*.88f);for(int k=0;k<5;k++){float b=k*Mathf.PI*2/5;MeshArt.Oval(parent,"DaisyPetal",p+new Vector3(Mathf.Cos(b)*.045f,0,Mathf.Sin(b)*.045f),new Vector3(.085f,.025f,.04f),MeshArt.Mat("Petal",new Color(1,.98f,.84f)),8);}MeshArt.Oval(parent,"DaisyCenter",p+Vector3.up*.018f,new Vector3(.04f,.028f,.04f),MeshArt.Mat("Pollen",new Color(1,.69f,.10f)),8);}
   if(index%4==0){var tree=new GameObject("WindTree");tree.transform.SetParent(parent,false);tree.transform.localPosition=new Vector3(size*.78f,.6f,-size*.42f);tree.transform.localScale=Vector3.one*(index==12?.8f:.5f);
    MeshArt.Lathe(tree.transform,"CurvedTrunk",new[]{0f,.4f,1.2f,1.7f},new[]{.17f,.13f,.10f,.015f},8,MeshArt.Mat("Bark",new Color(.40f,.24f,.09f)),Vector3.one,true);
    for(int i=0;i<5;i++){float a=i*Mathf.PI*2/5;MeshArt.Oval(tree.transform,"LeafCrown",new Vector3(Mathf.Cos(a)*.55f,1.6f+(i%2)*.24f,Mathf.Sin(a)*.55f),new Vector3(1.35f,.95f,1.3f),MeshArt.Mat("Realm"+realm+"Foliage",new Color(.37f,.51f,.31f),2),12);}}
  }
  public static void Waterfall(Transform parent,Vector3 position,int realm){var root=new GameObject("Waterfall");root.transform.SetParent(parent,false);root.transform.localPosition=position;var mat=MeshArt.Mat("Waterfall",new Color(.68f,.91f,.95f),3);var foam=MeshArt.Mat("CascadeFoam",new Color(.88f,.99f,1));
   for(int i=0;i<5;i++){float x=(i-2)*.10f;float length=4.6f+(i%3)*.30f;MeshArt.Mesh(root.transform,"WaterRibbon",new[]{new Vector3(x-.07f,0,0),new Vector3(x+.07f,0,0),new Vector3(x+.09f,-length,.14f),new Vector3(x-.08f,-length,.12f)},new[]{0,1,2,0,2,3,2,1,0,3,2,0},new[]{Vector2.up,Vector2.one,Vector2.right,Vector2.zero},mat);
    MeshArt.Mesh(root.transform,"WaterHighlight",new[]{new Vector3(x-.012f,-.05f,-.012f),new Vector3(x+.012f,-.05f,-.012f),new Vector3(x+.018f,-length,.105f),new Vector3(x-.015f,-length,.095f)},new[]{0,1,2,0,2,3,2,1,0,3,2,0},null,foam);
   }
   MeshArt.Oval(root.transform,"CascadeLipFoam",new Vector3(0,.015f,0),new Vector3(.59f,.035f,.12f),foam,10);root.AddComponent<WaterfallFlow>();
  }
 }
 public sealed class WaterfallFlow:MonoBehaviour{private RisingCourse _course;private Renderer[] _r;private MaterialPropertyBlock _b;private void Awake(){_course=FindFirstObjectByType<RisingCourse>();var ribbons=new List<Renderer>();foreach(var renderer in GetComponentsInChildren<Renderer>())if(renderer.name=="WaterRibbon")ribbons.Add(renderer);_r=ribbons.ToArray();_b=new MaterialPropertyBlock();}private void Update(){
   float light=_course!=null&&_course.Profile!=null&&_course.Profile.ReducedMotion?1:.96f+.04f*Mathf.Sin(Time.time*1.8f+transform.localPosition.x);_b.SetColor("_Color",new Color(.68f*light,.91f*light,.95f*light));foreach(var renderer in _r)renderer.SetPropertyBlock(_b);}}
}
