using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 // A coherent original water landscape replaces the repeated scaled scenic
 // landing-island templates. Only the existing thirteen route fans collide.
 public sealed class RisingWaterGarden:MonoBehaviour
 {
  Material _pool;RisingCourse _course;float _clock;
  public static void Build(Transform parent)
  {
   var root=new GameObject("OriginalWaterGarden");root.transform.SetParent(parent,false);var pool=LandscapePalette.Pool(parent);var stone=LandscapePalette.Surface(parent,1);
   const int rows=34;var v=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
   for(int row=0;row<=rows;row++){
    float z=-6+row*2.5f,center=Mathf.Sin(z*.072f)*1.35f,width=6.1f+Mathf.Sin(z*.11f)*.75f;
    for(int side=-1;side<=1;side+=2){var p=new Vector3(center+side*width,-1.35f,z);v.Add(p);uv.Add(new Vector2(p.x,p.z)*.22f);}
    if(row<rows){int k=row*2;triangles.AddRange(new[]{k,k+2,k+1,k+1,k+2,k+3});}
   }
   var basin=MeshArt.Mesh(root.transform,"ValleyBasin",v.ToArray(),triangles.ToArray(),uv.ToArray(),pool);basin.GetComponent<Renderer>().shadowCastingMode=ShadowCastingMode.Off;
   for(int bank=0;bank<5;bank++){
    int sign=bank%2==0?-1:1;var center=new Vector3(sign*(8.3f+bank*.12f),bank*1.5f,31+bank*12);
    var cliff=LandscapeGeometry.Rock(root.transform,"CascadeMesa",center,new Vector3(5.5f,8,6),stone,281+bank*19);cliff.name="GardenCliff";var mesh=cliff.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices;
    for(int i=0;i<points.Length;i++)if(points[i].y>2.6f)points[i].y=3.88f;mesh.vertices=points;mesh.RecalculateNormals();mesh.RecalculateBounds();LandscapeGeometry.JoinNormals(mesh);
    MeshArt.Oval(root.transform,"GardenPool",center+Vector3.up*3.92f,new Vector3(4.5f,.08f,5),pool,18);
    for(int bed=0;bed<3;bed++){var ledge=LandscapeGeometry.BeveledBlock(root.transform,"GardenSedimentBed",center+new Vector3(-sign*.10f,-2.4f+bed*2,-2.6f),new Vector3(4.2f,.22f,.75f),stone,.07f);LandscapeGeometry.StoneUv(ledge,.13f);}
    LandscapeBotany.CascadeWillow(root.transform,center+new Vector3(-sign*.2f,3.95f,1.1f),281+bank,1.65f,bank>1);
    if(bank<2)PondPlants(root.transform,center+Vector3.up*3.97f,bank);
    var fall=center+new Vector3(-sign*.45f,3.95f,-2.8f);IslandArt.Waterfall(root.transform,fall,1,2.2f,fall.y+1.35f);
    LandscapeBotany.Branch(root.transform,"GardenTrailingVine",new[]{center+new Vector3(-sign*2.4f,3.8f,-1),center+new Vector3(-sign*2.5f,1.5f,-.8f),center+new Vector3(-sign*2.25f,-.4f,-1.2f)},new[]{.055f,.035f,.01f},LandscapePalette.Moss(parent),6);
   }
   var flow=root.AddComponent<RisingWaterGarden>();flow._pool=pool;flow._course=parent.GetComponentInParent<RisingCourse>();MeshArt.Batch(root);
  }
  static void PondPlants(Transform parent,Vector3 center,int bank){
   var lily=MeshArt.Mat("Realm1Lily",new Color(.27f,.60f,.27f));var bloom=MeshArt.Mat("Realm1Bloom",new Color(1,.97f,.88f));var pollen=MeshArt.Mat("WaterPollen",new Color(1,.72f,.18f));
   for(int flower=0;flower<3;flower++){
    var p=center+new Vector3((flower-1)*.65f,.018f,-1.1f+(flower%2)*.55f);
    var pad=MeshArt.Ring(parent,"LilyPad",.32f,.32f,0,lily,18,325);pad.transform.localPosition=p;pad.transform.localRotation=Quaternion.Euler(0,bank*49+flower*97,0);
    for(int petal=0;petal<6;petal++){float a=petal*Mathf.PI/3;var mesh=MeshArt.Oval(parent,"LilyPetal",p+new Vector3(Mathf.Cos(a)*.08f,.035f,Mathf.Sin(a)*.08f),new Vector3(.15f,.065f,.06f),bloom,6);mesh.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);}
    MeshArt.Oval(parent,"LilyBloom",p+Vector3.up*.06f,new Vector3(.07f,.045f,.07f),pollen,6);
   }
   var reed=MeshArt.Mat("Realm1Reed",new Color(.33f,.49f,.23f));var seed=MeshArt.Mat("Realm1ReedSeed",new Color(.50f,.30f,.14f));var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int j=0;j<8;j++){
    float angle=j*.4f;var p=center+new Vector3(Mathf.Cos(angle)*1.9f,0,Mathf.Sin(angle)*1.8f);float h=.58f+(j%3)*.16f;
    MeshArt.Box(parent,"ReedStem",p+Vector3.up*h*.5f,new Vector3(.018f,h,.018f),reed);MeshArt.Oval(parent,"ReedHead",p+Vector3.up*h,new Vector3(.055f,.16f,.05f),seed,6);
    LandscapeBotany.Blade(v,uv,t,p,angle,h*.8f,.08f);LandscapeBotany.Blade(v,uv,t,p,angle+1.9f,h*.65f,.07f);
   }
   MeshArt.Mesh(parent,"GardenReedLeaves",v.ToArray(),t.ToArray(),uv.ToArray(),reed);
  }
  void Update(){if(_pool==null)return;if((_course==null||!_course.Paused)&&(_course?.Profile==null||!_course.Profile.ReducedMotion))_clock+=Time.deltaTime;_pool.SetFloat("_RippleTime",_clock);}
 }
}
