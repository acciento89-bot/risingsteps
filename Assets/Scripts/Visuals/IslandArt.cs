using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.Core;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class IslandArt
 {
  public const float Surface=.6f;
  // Expand the old seeded polygon; never remove an already safe landing point.
  public static float ContourFactor(int realm,float angle)=>realm==2?1/Mathf.Pow(Mathf.Pow(Mathf.Abs(Mathf.Cos(angle)),6)+Mathf.Pow(Mathf.Abs(Mathf.Sin(angle)),6),1f/6):realm==1?1/Mathf.Sqrt(Mathf.Pow(Mathf.Cos(angle)/1.18f,2)+Mathf.Pow(Mathf.Sin(angle),2)):1;
  public static StepMarker Build(Transform parent,int index,int realm,bool scenic=false){var random=new System.Random(117+index*79+realm*1901);const int n=24;float size=index==12?2.05f:1.55f;var radius=new float[n];for(int i=0;i<n;i++)radius[i]=size*(.91f+(float)random.NextDouble()*.13f)*ContourFactor(realm,i*Mathf.PI*2/n);
   BuildCliff(parent,radius,index,realm);
   var gv=new Vector3[n+1];var guv=new Vector2[n+1];var gt=new int[n*3];gv[0]=Vector3.up*Surface;guv[0]=Vector2.one*.5f;
   for(int i=0;i<n;i++){float a=i*Mathf.PI*2/n;gv[i+1]=new Vector3(Mathf.Cos(a)*radius[i],Surface,Mathf.Sin(a)*radius[i]);guv[i+1]=new Vector2(gv[i+1].x/size,gv[i+1].z/size)*.5f+Vector2.one*.5f;gt[i*3]=0;gt[i*3+1]=(i+1)%n+1;gt[i*3+2]=i+1;}
   var ground=MeshArt.Mesh(parent,realm==2?"TempleSurface":realm==1?"CascadeSurface":realm==0?"MeadowSurface":RisingRealmCatalog.Theme(realm)+"Surface",gv,gt,guv,realm==0?MeshArt.Mat("Realm0Ground",RealmArt.Ground(0),0):MeshArt.Stone("Realm"+realm+"Ground",RealmArt.Ground(realm)),!scenic);StepMarker marker=null;if(!scenic){marker=ground.AddComponent<StepMarker>();marker.Index=index;
    var rim=MeshArt.Ring(parent,"GoldenLandingRim",size*(realm==2?1.10f:.74f),.032f,Surface+.018f,MeshArt.Mat("RouteGold",new Color(.91f,.63f,.15f),-1,true,.60f),realm==2?4:64);var contrast=MeshArt.Ring(parent,"RouteContrastRim",size*(realm==2?1.12f:.76f),.025f,Surface+.017f,MeshArt.Mat("RouteContrast",new Color(.025f,.07f,.10f)),realm==2?4:64);contrast.SetActive(false);
    var inner=MeshArt.Ring(parent,"InnerRouteRim",size*(realm==2?1.06f:.70f),.006f,Surface+.02f,MeshArt.Mat("RouteLight",new Color(.95f,.77f,.36f),-1,true,.30f),realm==2?4:64);if(realm==2){rim.transform.localRotation=contrast.transform.localRotation=inner.transform.localRotation=Quaternion.Euler(0,45,0);}}
   SurfaceFinish(parent,ground,gv,gt,guv,realm);Vegetation(parent,radius,size,random,realm,index);RealmArt.Dress(parent,index,realm,size,scenic);
   return marker;
  }
  // Both rendered lips follow the same newly authored collision contour.
  // Lower sections share normals and move in height independently around the cliff.
  private static void BuildCliff(Transform parent,float[] radius,int index,int realm)
  {
   const int n=24;var random=new System.Random(7919+index*193+realm*3571);
   var theme=RisingRealmCatalog.Theme(realm);
   float nominalDepth=theme switch {RisingRealmTheme.Waterfall=>3.7f,RisingRealmTheme.Temple=>3.0f,RisingRealmTheme.Jungle=>4.15f,RisingRealmTheme.Crystal=>3.55f,RisingRealmTheme.Frost=>4.45f,RisingRealmTheme.Desert=>2.9f,RisingRealmTheme.Volcano=>3.8f,RisingRealmTheme.CloudCoast=>3.45f,RisingRealmTheme.Starlit=>2.65f,_=>3.25f};
   float depth=nominalDepth*(.88f+(float)random.NextDouble()*.24f);
   float[] levels={-1,-.88f,-.66f,-.40f,-.17f,-.035f,.48f,.6f};
   float[] taper=theme switch {
    RisingRealmTheme.Meadow=>new[]{.055f,.14f,.57f,.83f,.94f,.97f,1,.99f},
    RisingRealmTheme.Waterfall=>new[]{.40f,.65f,.76f,.92f,.98f,.99f,1,.99f},
    RisingRealmTheme.Temple=>new[]{.35f,.57f,.77f,.91f,.96f,.97f,1,.99f},
    RisingRealmTheme.Crystal=>new[]{.06f,.19f,.34f,.53f,.69f,.89f,1,.99f},
    RisingRealmTheme.Frost=>new[]{.035f,.11f,.27f,.51f,.77f,.96f,1,.99f},
    RisingRealmTheme.Desert=>new[]{.07f,.20f,.38f,.63f,.82f,.97f,1,.99f},
    RisingRealmTheme.Volcano=>new[]{.045f,.16f,.36f,.63f,.83f,.93f,1,.99f},
    RisingRealmTheme.CloudCoast=>new[]{.055f,.17f,.36f,.59f,.86f,.96f,1,.99f},
    RisingRealmTheme.Starlit=>new[]{.025f,.10f,.25f,.46f,.73f,.91f,1,.99f},
    _=>new[]{.055f,.14f,.31f,.56f,.78f,.94f,1,.99f}};
   float phase=(float)random.NextDouble()*Mathf.PI*2;
   var tip=new Vector3(Mathf.Cos(phase)*.70f,0,Mathf.Sin(phase)*.70f);
   var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
   for(int j=0;j<levels.Length;j++)for(int i=0;i<=n;i++){
    int point=i%n;bool lip=j>=6;float angle=point*Mathf.PI*2/n,below=1-j/6f;
    float twist=lip?0:.05f*Mathf.Sin(angle*2+phase+j*.41f);
    float variation=lip?1:1+(realm<3&&j>=2?.16f:.11f)*Mathf.Sin(angle*3+phase)+.07f*Mathf.Cos(angle*5-phase+j*.31f);
    // Local shoulders die away around the side; they never trace a full collar.
    float shoulder=!lip&&j>=3?.08f*Mathf.Pow(Mathf.Max(0,Mathf.Cos(angle-phase-j*.48f)),4):0;
    float r=radius[point]*(taper[j]*variation+shoulder);
    float uneven=lip?0:depth*(.065f*Mathf.Sin(angle*2+phase+j*.43f)+.025f*Mathf.Sin(angle*5-phase))*(j==0?.18f:1);
    var shift=lip?Vector3.zero:tip*below+new Vector3(Mathf.Sin(j*.8f+phase),0,Mathf.Cos(j*.61f+phase))*.07f*below;
    var p=new Vector3(Mathf.Cos(angle+twist)*r,lip?levels[j]:levels[j]*depth+uneven,Mathf.Sin(angle+twist)*r)+shift;
    if(!lip&&realm>=3){
     float rib=theme==RisingRealmTheme.Crystal?.09f:theme==RisingRealmTheme.Volcano?.11f:.035f;
     p.x+=Mathf.Cos(angle*4+phase)*rib*below;p.z*=theme==RisingRealmTheme.Desert?1-.14f*below:theme==RisingRealmTheme.Starlit?1-.21f*below:1;
    }
    vertices.Add(p);uv.Add(new Vector2((float)i/n*3+phase,p.y*.34f+Mathf.Sin(angle*2+phase)*.08f));
    if(j>0&&i<n){int k=j*(n+1)+i,lower=k-(n+1);triangles.AddRange(new[]{lower,k,lower+1,lower+1,k,k+1});}
   }
   int toe=vertices.Count;vertices.Add(tip+Vector3.down*(depth+.08f));uv.Add(new Vector2(phase,-depth*.34f));
   for(int i=0;i<n;i++)triangles.AddRange(new[]{toe,i,i+1});
   var stone=LandscapePalette.Surface(parent,realm);
   var cliff=MeshArt.Mesh(parent,"LimestoneCliff",vertices.ToArray(),triangles.ToArray(),uv.ToArray(),stone);
   if(realm<3)LandscapeGeometry.FractureNormals(cliff.GetComponent<MeshFilter>().sharedMesh,.57f);
   else LandscapeGeometry.JoinNormals(cliff.GetComponent<MeshFilter>().sharedMesh);
   if(realm<3)LandscapeGeometry.MineralPlates(parent,radius,index,stone);
   // Two unequal longitudinal outcrops, rather than repeated horizontal shelves.
   for(int k=0;k<2;k++){
    float angle=phase+.85f+k*2.25f;float r=radius[(index+k*7)%n]*.56f;
    var rock=LandscapeGeometry.Rock(parent,"CliffButtress",new Vector3(Mathf.Cos(angle)*r,-.53f-k*.54f,Mathf.Sin(angle)*r),new Vector3(realm<3?.72f:.56f,1.16f+k*.34f,realm<3?.59f:.47f),stone,index*11+k);
    rock.transform.localRotation=Quaternion.Euler(11,k*71+index*13,-15+k*27);
   }
  }
  private static void SurfaceFinish(Transform parent,GameObject ground,Vector3[] fan,int[] triangles,Vector2[] originalUv,int realm)
  {
   // Rendering is separate from the unchanged authoritative ground mesh/material/collider.
   var vertices=(Vector3[])fan.Clone();var uv=(Vector2[])originalUv.Clone();
   for(int i=0;i<vertices.Length;i++){vertices[i].y+=.002f;uv[i]*=realm==0?3.2f:2.0f;}
   ground.GetComponent<Renderer>().enabled=false;
   var finish=MeshArt.Mesh(parent,"LandscapeTerrainFinish",vertices,triangles,uv,LandscapePalette.Surface(parent,realm,realm==0||realm>=3?1:0));finish.GetComponent<MeshFilter>().sharedMesh.RecalculateTangents();
   if(realm!=0&&realm!=3&&realm!=5&&realm!=8)return;
   var v=new List<Vector3>();var t=new List<int>();var tex=new List<Vector2>();const int n=24;
   for(int ring=0;ring<3;ring++)for(int i=0;i<=n;i++){
    var p=fan[i%n+1]*(ring==0?.978f:ring==1?1.003f:1.006f);
    p.y=ring==0?.603f:ring==1?.59f:.548f+.014f*Mathf.Sin((i%n)*1.91f);
    v.Add(p);tex.Add(new Vector2(p.x*.72f,p.z*.72f));
    if(ring>0&&i<n){int k=ring*(n+1)+i,inner=k-(n+1);t.AddRange(new[]{inner,inner+1,k,inner+1,k+1,k});}
   }
   var lip=MeshArt.Mesh(parent,realm==0?"MeadowTurfLip":realm==3?"JungleMossLip":realm==5?"FrostSnowLip":"CoastalSandLip",v.ToArray(),t.ToArray(),tex.ToArray(),LandscapePalette.Surface(parent,realm,1));
   LandscapeGeometry.JoinNormals(lip.GetComponent<MeshFilter>().sharedMesh);
  }
  private static void Vegetation(Transform parent,float[] radii,float size,System.Random random,int realm,int index)
  {
   if(realm!=0)return;var grass=new List<Vector3>();var uv=new List<Vector2>();var tri=new List<int>();
   // Exactly the original 80 angle/radius/height draws; styling has its own seed.
   for(int i=0;i<80;i++){
    float a=(float)random.NextDouble()*Mathf.PI*2,r=size*(.76f+(float)random.NextDouble()*.16f);
    float h=.08f+(float)random.NextDouble()*.14f;var p=new Vector3(Mathf.Cos(a)*r,.607f,Mathf.Sin(a)*r);
    if(i%2==0)for(int blade=0;blade<3;blade++)LandscapeBotany.Blade(grass,uv,tri,p,a+blade*1.8f+index*.43f,h*(1+blade*.12f),.045f);
   }
   MeshArt.Mesh(parent,"MeadowBlades",grass.ToArray(),tri.ToArray(),uv.ToArray(),LandscapePalette.Foliage(parent,false));
   // Ten original flower-angle draws; each authored cluster is independent of RNG.
   for(int i=0;i<10;i++){
    float a=(float)random.NextDouble()*Mathf.PI*2;var p=new Vector3(Mathf.Cos(a)*size*.88f,.607f,Mathf.Sin(a)*size*.88f);
    LandscapeBotany.Flowers(parent,p,index*71+i*13);
   }
   if(index%4==0)LandscapeBotany.Tree(parent,new Vector3(size*.78f,.6f,-size*.42f),index,index==12?.74f:.78f);
  }
  public static void Waterfall(Transform parent,Vector3 position,int realm,float width=.78f,float fallLength=0)
  {
   var root=new GameObject("Waterfall");root.transform.SetParent(parent,false);root.transform.localPosition=position;
   var outward=new Vector3(position.x,0,position.z);if(outward.sqrMagnitude>.01f)root.transform.localRotation=Quaternion.LookRotation(outward);
   var mat=LandscapePalette.Surface(parent,1,2);var foam=MeshArt.Mat("CascadeFoam",new Color(.88f,.99f,1));
   // Continuous curved curtain, not five unrelated straight luminous wires.
   const int columns=14,rows=12;var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   float length=fallLength>0?fallLength:3.45f+Mathf.Abs(position.x)*.40f;
   for(int row=0;row<=rows;row++)for(int col=0;col<=columns;col++){
    float f=(float)row/rows,u=(float)col/columns,span=width*(1+f*.10f);
    float x=(u-.5f)*span+.022f*Mathf.Sin(col*1.7f+row*.46f)*f;
    float curl=.016f+f*f*.31f+.046f*Mathf.Sin(col*.8f+row*.55f)*f;
    v.Add(new Vector3(x,-f*length,curl));uv.Add(new Vector2(u,f*2.7f));
    if(row<rows&&col<columns){int k=row*(columns+1)+col;t.AddRange(new[]{k,k+columns+1,k+1,k+1,k+columns+1,k+columns+2});}
   }
   // Separate shared back vertices preserve smooth front/back lighting normals.
   int frontVertices=v.Count,frontIndices=t.Count;
   for(int i=0;i<frontVertices;i++){v.Add(v[i]);uv.Add(uv[i]);}
   for(int i=0;i<frontIndices;i+=3)t.AddRange(new[]{t[i]+frontVertices,t[i+2]+frontVertices,t[i+1]+frontVertices});
   MeshArt.Mesh(root.transform,"WaterRibbon",v.ToArray(),t.ToArray(),uv.ToArray(),mat);
   MeshArt.Oval(root.transform,"CascadeLipFoam",new Vector3(0,.018f,.025f),new Vector3(width*1.03f,.032f,.13f),foam,10);
   for(int drop=0;drop<6;drop++)MeshArt.Oval(root.transform,"CascadeSpray",new Vector3((drop-2.5f)*width*.15f,-length+.05f+(drop%2)*.13f,.32f+Mathf.Sin(drop)*.09f),new Vector3(.09f,.20f,.075f),foam,6);
   // Four broken foamy rivulets vary in depth; they share the finite foam finish.
   for(int stream=0;stream<4;stream++){
    var ribbon=new List<Vector3>();var tex=new List<Vector2>();var faces=new List<int>();
    for(int row=0;row<=8;row++){
     float f=row/8f,x=(stream-1.5f)*width*.205f+Mathf.Sin(row*.81f+stream)*width*.026f;
     float z=.057f+f*f*.31f,w=width*(.040f+.070f*Mathf.Sin(row*.93f+stream)*Mathf.Sin(row*.93f+stream));
     ribbon.Add(new Vector3(x-w,-length*f,z));ribbon.Add(new Vector3(x+w,-length*f,z));tex.Add(new Vector2(0,f));tex.Add(new Vector2(1,f));
     if(row<8){int k=row*2;faces.AddRange(new[]{k,k+2,k+1,k+1,k+2,k+3});}
    }
    MeshArt.Mesh(root.transform,"CascadeFoamThread",ribbon.ToArray(),faces.ToArray(),tex.ToArray(),foam);
   }
   root.AddComponent<WaterfallFlow>();
  }

 }
 public sealed class WaterfallFlow:MonoBehaviour{private RisingCourse _course;private Renderer[] _r;private MaterialPropertyBlock _b;private void Awake(){_course=GetComponentInParent<RisingCourse>();var ribbons=new List<Renderer>();foreach(var renderer in GetComponentsInChildren<Renderer>())if(renderer.name=="WaterRibbon")ribbons.Add(renderer);_r=ribbons.ToArray();_b=new MaterialPropertyBlock();}private void Update(){
   float light=_course!=null&&_course.Profile!=null&&_course.Profile.ReducedMotion?1:.96f+.04f*Mathf.Sin(Time.time*1.8f+transform.localPosition.x);_b.SetColor("_Color",new Color(light,light,light));foreach(var renderer in _r)renderer.SetPropertyBlock(_b);}}
}
