using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class PortalArt
 {
  public static void Build(Transform parent,RisingCourse course){var root=new GameObject("TempleOfLight",typeof(PortalArrival));root.transform.SetParent(parent,false);root.GetComponent<PortalArrival>().Course=course;
   int realm=course?.Profile?.Realm??0;var stone=MeshArt.Stone("Realm"+realm+"PortalStone",realm==2?new Color(.83f,.72f,.55f):new Color(.81f,.84f,.78f));var gold=MeshArt.Mat("TempleGold",new Color(.86f,.59f,.18f),-1,true,.65f);
   MeshArt.Box(root.transform,"TempleFoundation",new Vector3(0,.66f,.45f),new Vector3(2.5f,.12f,2.0f),stone);
   for(int side=-1;side<=1;side+=2){var pos=new Vector3(side*1.28f,.7f,.60f);var pillar=MeshArt.Lathe(root.transform,"CarvedPillar",new[]{0f,.12f,.22f,2.6f,2.72f,2.84f},new[]{.28f,.28f,.17f,.17f,.28f,.28f},8,stone,Vector3.one,true);pillar.transform.localPosition=pos;
    for(int i=0;i<3;i++)MeshArt.Box(root.transform,"PillarBand",pos+Vector3.up*(.28f+i*1.15f),new Vector3(.38f,.07f,.38f),gold);
    MeshArt.Oval(root.transform,"GoldenCrown",pos+Vector3.up*3,new Vector3(.14f,.36f,.14f),gold,12);}
   // Thick segmented masonry vault, with an open center and visible depth.
   for(int segment=0;segment<13;segment++){
    float angle=(segment+.5f)*Mathf.PI/13;var p=new Vector3(Mathf.Cos(angle)*1.30f,2.10f+Mathf.Sin(angle)*1.30f,.60f);
    var block=MeshArt.Box(root.transform,"ArchVoussoir",p,new Vector3(.34f,.29f,.38f),stone);block.transform.localRotation=Quaternion.Euler(0,0,angle*Mathf.Rad2Deg-90);
    var inset=MeshArt.Box(root.transform,"ArchGoldCarving",p+new Vector3(0,0,-.20f),new Vector3(.20f,.13f,.025f),gold);inset.transform.localRotation=block.transform.localRotation;
   }
   for(int side=-1;side<=1;side+=2){MeshArt.Box(root.transform,"ArchJamb",new Vector3(side*1.30f,1.38f,.60f),new Vector3(.28f,1.45f,.38f),stone);MeshArt.Box(root.transform,"ArchJambInlay",new Vector3(side*1.30f,1.38f,.40f),new Vector3(.07f,1.30f,.025f),gold);}
   var ring=MeshArt.Ring(root.transform,"RadiantArch",1.23f,.045f,0,gold,48,180);ring.transform.localRotation=Quaternion.Euler(-90,0,0);ring.transform.localPosition=new Vector3(0,2.10f,.39f);
   var back=MeshArt.Ring(root.transform,"RadiantArchBack",1.23f,.045f,0,gold,48,180);back.transform.localRotation=Quaternion.Euler(-90,180,0);back.transform.localPosition=new Vector3(0,2.10f,.82f);
   MeshArt.Box(root.transform,"GoldenKeystone",new Vector3(0,3.44f,.59f),new Vector3(.30f,.30f,.44f),gold).transform.localRotation=Quaternion.Euler(0,0,45);
   MeshArt.Box(root.transform,"PortalEntablature",new Vector3(0,3.76f,.60f),new Vector3(3.12f,.17f,.53f),stone);
   MeshArt.Box(root.transform,"PortalCornice",new Vector3(0,3.90f,.60f),new Vector3(3.30f,.10f,.62f),gold);
   var star=MeshArt.Box(root.transform,"PortalStar",new Vector3(0,2.18f,.58f),new Vector3(.10f,.8f,.10f),gold);star.transform.localRotation=Quaternion.Euler(0,0,35);
   var cross=MeshArt.Box(root.transform,"PortalStarCross",new Vector3(0,2.18f,.58f),new Vector3(.08f,.65f,.08f),gold);cross.transform.localRotation=Quaternion.Euler(0,0,-35);
   for(int i=0;i<3;i++)MeshArt.Box(root.transform,"ArrivalSteps",new Vector3(0,.65f+i*.04f,-.7f+i*.28f),new Vector3(1.6f,.06f,.23f),gold);
  }
 }
 public sealed class PortalArrival:MonoBehaviour{public RisingCourse Course;private void Update(){if(Course==null||Course.Paused||Course.Height!=12||Course.Profile.Completed)return;var d=Course.Player.position-(transform.position+new Vector3(0,.6f,.5f));if(new Vector2(d.x,d.z).magnitude<.8f&&Mathf.Abs(d.y)<1)Course.CompletePortal();}}
}
