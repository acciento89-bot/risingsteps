using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class PortalArt
 {
  public static void Build(Transform parent,RisingCourse course){var root=new GameObject("TempleOfLight",typeof(PortalArrival));root.transform.SetParent(parent,false);root.GetComponent<PortalArrival>().Course=course;
   int realm=course?.Profile?.Realm??0;var stone=LandscapePalette.Surface(parent,realm);var gold=MeshArt.Mat("TempleGold",new Color(.86f,.59f,.18f),-1,true,.65f);
   MeshArt.Box(root.transform,"TempleFoundation",new Vector3(0,.66f,.45f),new Vector3(2.5f,.12f,2.0f),stone);
   for(int side=-1;side<=1;side+=2){var pos=new Vector3(side*1.28f,.7f,.60f);var pillar=MeshArt.Lathe(root.transform,"CarvedPillar",new[]{0f,.12f,.22f,2.6f,2.72f,2.84f},new[]{.28f,.28f,.17f,.17f,.28f,.28f},8,stone,Vector3.one,true);pillar.transform.localPosition=pos;
    for(int i=0;i<3;i++)MeshArt.Box(root.transform,"PillarBand",pos+Vector3.up*(.28f+i*1.15f),new Vector3(.38f,.07f,.38f),gold);
    MeshArt.Oval(root.transform,"GoldenCrown",pos+Vector3.up*3,new Vector3(.14f,.36f,.14f),gold,12);}
   // The vault is cut annular masonry: no disconnected rotated rectangular beads.
   for(int segment=0;segment<13;segment++){
    float start=segment*Mathf.PI/13+.006f,end=(segment+1)*Mathf.PI/13-.006f;
    LandscapeGeometry.ArchStone(root.transform,"ArchVoussoir",start,end,1.13f,1.51f,2.10f,.60f,stone);
    LandscapeGeometry.ArchStone(root.transform,"ArchGoldCarving",start+.014f,end-.014f,1.30f,1.34f,2.10f,.32f,gold,.016f);
   }
   for(int side=-1;side<=1;side+=2){MeshArt.Box(root.transform,"ArchJamb",new Vector3(side*1.30f,1.38f,.60f),new Vector3(.38f,1.45f,.52f),stone);MeshArt.Box(root.transform,"ArchJambInlay",new Vector3(side*1.30f,1.38f,.40f),new Vector3(.07f,1.30f,.025f),gold);}
   var ring=MeshArt.Ring(root.transform,"RadiantArch",1.23f,.045f,0,gold,48,180);ring.transform.localRotation=Quaternion.Euler(-90,0,0);ring.transform.localPosition=new Vector3(0,2.10f,.39f);
   var back=MeshArt.Ring(root.transform,"RadiantArchBack",1.23f,.045f,0,gold,48,180);back.transform.localRotation=Quaternion.Euler(-90,180,0);back.transform.localPosition=new Vector3(0,2.10f,.82f);
   MeshArt.Box(root.transform,"GoldenKeystone",new Vector3(0,3.44f,.59f),new Vector3(.30f,.30f,.44f),gold).transform.localRotation=Quaternion.Euler(0,0,45);
   MeshArt.Box(root.transform,"PortalEntablature",new Vector3(0,3.76f,.60f),new Vector3(3.12f,.17f,.53f),stone);
   MeshArt.Box(root.transform,"PortalCornice",new Vector3(0,3.90f,.60f),new Vector3(3.30f,.10f,.62f),gold);
   MeshArt.Mesh(root.transform,"PortalPediment",new[]{new Vector3(-1.55f,3.98f,.30f),new Vector3(1.55f,3.98f,.30f),new Vector3(0,4.44f,.30f),new Vector3(-1.55f,3.98f,.90f),new Vector3(1.55f,3.98f,.90f),new Vector3(0,4.44f,.90f)},new[]{0,2,1,3,4,5,0,3,5,0,5,2,1,2,5,1,5,4,0,1,4,0,4,3},new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.zero,Vector2.right,Vector2.up},stone);
   var medallion=MeshArt.Ring(root.transform,"PortalSunCarving",.15f,.022f,0,gold,24);medallion.transform.localRotation=Quaternion.Euler(-90,0,0);medallion.transform.localPosition=new Vector3(0,4.17f,.285f);
   for(int side=-1;side<=1;side+=2){var bowl=MeshArt.Lathe(root.transform,"ArrivalBrazier",new[]{0f,.08f,.12f},new[]{.11f,.17f,.17f},10,gold,Vector3.one,true);bowl.transform.localPosition=new Vector3(side*1.54f,.95f,-.47f);MeshArt.Oval(root.transform,"ArrivalFlame",new Vector3(side*1.54f,1.16f,-.47f),new Vector3(.13f,.33f,.13f),MeshArt.Mat("PortalFlame",new Color(1,.65f,.20f),-1,true,1.25f),8);}
   var radiance=new Material(Resources.Load<Material>("Materials/PortalLight"));ArtLifetime.Own(root,radiance);
   var contour=new System.Collections.Generic.List<Vector3>{new(-1.10f,.70f,.59f),new(1.10f,.70f,.59f)};
   for(int segment=0;segment<=20;segment++){float a=segment*Mathf.PI/20;contour.Add(new Vector3(Mathf.Cos(a)*1.10f,2.10f+Mathf.Sin(a)*1.10f,.59f));}
   var vertices=new Vector3[contour.Count+1];var uv=new Vector2[vertices.Length];var triangles=new int[contour.Count*3];vertices[0]=new Vector3(0,1.85f,.59f);uv[0]=new Vector2(.5f,.46f);
   for(int i=0;i<contour.Count;i++){vertices[i+1]=contour[i];uv[i+1]=new Vector2((contour[i].x+1.10f)/2.20f,(contour[i].y-.70f)/2.50f);triangles[i*3]=0;triangles[i*3+1]=i+1;triangles[i*3+2]=(i+1)%contour.Count+1;}
   MeshArt.Mesh(root.transform,"PortalRadiance",vertices,triangles,uv,radiance);
   var lamp=new GameObject("PortalWarmLight",typeof(Light));lamp.transform.SetParent(root.transform,false);lamp.transform.localPosition=new Vector3(0,2.1f,-.25f);var light=lamp.GetComponent<Light>();light.type=LightType.Point;light.color=new Color(1,.73f,.34f);light.intensity=.85f;light.range=6;light.shadows=LightShadows.None;
   for(int i=0;i<3;i++)MeshArt.Box(root.transform,"ArrivalSteps",new Vector3(0,.65f+i*.04f,-.7f+i*.28f),new Vector3(1.6f,.06f,.23f),gold);
  }
 }
 public sealed class PortalArrival:MonoBehaviour{public RisingCourse Course;private void Update(){if(Course==null||Course.Paused||Course.Height!=12||Course.Profile.Completed)return;var d=Course.Player.position-(transform.position+new Vector3(0,.6f,.5f));if(new Vector2(d.x,d.z).magnitude<.8f&&Mathf.Abs(d.y)<1)Course.CompletePortal();}}
}
