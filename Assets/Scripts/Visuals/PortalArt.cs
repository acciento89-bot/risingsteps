using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class PortalArt
 {
  public static void Build(Transform parent,RisingCourse course){var root=new GameObject("TempleOfLight",typeof(PortalArrival));root.transform.SetParent(parent,false);root.GetComponent<PortalArrival>().Course=course;
   var stone=MeshArt.Mat("TempleIvory",new Color(.85f,.82f,.65f),1);var gold=MeshArt.Mat("TempleGold",new Color(1,.72f,.16f),-1,true);
   MeshArt.Box(root.transform,"TempleFoundation",new Vector3(0,.66f,.45f),new Vector3(2.5f,.12f,2.0f),stone);
   for(int side=-1;side<=1;side+=2){var pos=new Vector3(side*1.28f,.7f,.60f);var pillar=MeshArt.Lathe(root.transform,"CarvedPillar",new[]{0f,.12f,.22f,2.6f,2.72f,2.84f},new[]{.28f,.28f,.17f,.17f,.28f,.28f},8,stone,Vector3.one,true);pillar.transform.localPosition=pos;
    for(int i=0;i<3;i++)MeshArt.Box(root.transform,"PillarBand",pos+Vector3.up*(.28f+i*1.15f),new Vector3(.38f,.07f,.38f),gold);
    MeshArt.Oval(root.transform,"GoldenCrown",pos+Vector3.up*3,new Vector3(.14f,.36f,.14f),gold,12);}
   var ring=MeshArt.Ring(root.transform,"RadiantArch",1.23f,.11f,0,gold,80);ring.transform.localRotation=Quaternion.Euler(90,0,0);ring.transform.localPosition=new Vector3(0,2.13f,.60f);
   var outer=MeshArt.Ring(root.transform,"IvoryArch",1.36f,.10f,0,stone,80);outer.transform.localRotation=ring.transform.localRotation;outer.transform.localPosition=ring.transform.localPosition;
   var star=MeshArt.Box(root.transform,"PortalStar",new Vector3(0,2.18f,.58f),new Vector3(.10f,.8f,.10f),gold);star.transform.localRotation=Quaternion.Euler(0,0,35);
   var cross=MeshArt.Box(root.transform,"PortalStarCross",new Vector3(0,2.18f,.58f),new Vector3(.08f,.65f,.08f),gold);cross.transform.localRotation=Quaternion.Euler(0,0,-35);
   for(int i=0;i<3;i++)MeshArt.Box(root.transform,"ArrivalSteps",new Vector3(0,.65f+i*.04f,-.7f+i*.28f),new Vector3(1.6f,.06f,.23f),gold);
  }
 }
 public sealed class PortalArrival:MonoBehaviour{public RisingCourse Course;private void Update(){if(Course==null||Course.Paused||Course.Height!=12||Course.Profile.Completed)return;var d=Course.Player.position-(transform.position+new Vector3(0,.6f,.5f));if(new Vector2(d.x,d.z).magnitude<.8f&&Mathf.Abs(d.y)<1)Course.CompletePortal();}}
}
