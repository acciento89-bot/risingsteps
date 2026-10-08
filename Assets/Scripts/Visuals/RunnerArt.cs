using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class RunnerArt
 {
  public static RunnerAnimator Build(Transform parent){var root=new GameObject("HoodieRunner",typeof(RunnerAnimator));root.transform.SetParent(parent,false);var a=root.GetComponent<RunnerAnimator>();
   var skin=MeshArt.Mat("WarmSkin",new Color(.91f,.54f,.28f));var dark=MeshArt.Mat("CharcoalFabric",new Color(.09f,.12f,.15f));var white=MeshArt.Mat("ShoeIvory",new Color(.94f,.97f,.92f));var green=new Material(MeshArt.Mat("HoodieBase",new Color(.035f,.43f,.23f)));a.Hoodie=green;
   a.Hips=Pivot(root.transform,"Hips",new Vector3(0,.76f,0));
   MeshArt.Oval(a.Hips,"CargoShorts",new Vector3(0,.08f,0),new Vector3(.63f,.38f,.39f),dark);
   MeshArt.Lathe(a.Hips,"HoodieTorso",new[]{.14f,.20f,.39f,.61f,.71f},new[]{.30f,.31f,.30f,.27f,.18f},20,green,new Vector3(1,1,.72f));
   MeshArt.Oval(a.Hips,"HoodFold",new Vector3(0,.66f,-.10f),new Vector3(.5f,.24f,.40f),white);
   MeshArt.Oval(a.Hips,"HoodLining",new Vector3(0,.70f,-.13f),new Vector3(.36f,.14f,.30f),green);
   for(int side=-1;side<=1;side+=2){var leg=Pivot(a.Hips,side<0?"LeftLeg":"RightLeg",new Vector3(side*.16f,-.05f,0));if(side<0)a.LeftLeg=leg;else a.RightLeg=leg;
    MeshArt.Lathe(leg,"CargoLeg",new[]{-.48f,-.42f,-.20f,0f},new[]{.11f,.14f,.16f,.14f},16,dark,new Vector3(1,1,.95f));
    MeshArt.Box(leg,"CargoPocket",new Vector3(side*.13f,-.14f,.01f),new Vector3(.08f,.17f,.18f),dark);
    MeshArt.Oval(leg,"Trainer",new Vector3(0,-.57f,.10f),new Vector3(.29f,.23f,.49f),white);
    MeshArt.Oval(leg,"RubberSole",new Vector3(0,-.65f,.12f),new Vector3(.30f,.085f,.5f),MeshArt.Mat("RubberSole",new Color(.75f,.76f,.67f)));
    MeshArt.Box(leg,"TrainerAccent",new Vector3(side*.13f,-.56f,.08f),new Vector3(.025f,.08f,.24f),green);
    for(int k=0;k<3;k++)MeshArt.Box(leg,"Lace",new Vector3(0,-.49f,.10f+k*.045f),new Vector3(.17f,.017f,.022f),white);
    var arm=Pivot(a.Hips,side<0?"LeftArm":"RightArm",new Vector3(side*.28f,.55f,0));if(side<0)a.LeftArm=arm;else a.RightArm=arm;arm.localRotation=Quaternion.Euler(0,0,side*12);
    MeshArt.Lathe(arm,"RolledSleeve",new[]{-.33f,-.28f,-.05f,.06f},new[]{.11f,.14f,.14f,.06f},16,green,Vector3.one);
    MeshArt.Lathe(arm,"Forearm",new[]{-.55f,-.50f,-.28f},new[]{.07f,.085f,.085f},14,skin,Vector3.one);MeshArt.Oval(arm,"Hand",new Vector3(0,-.56f,.015f),new Vector3(.18f,.20f,.15f),skin);
    MeshArt.Lathe(arm,"WristBand",new[]{-.48f,-.44f},new[]{.087f,.087f},14,dark,Vector3.one);
   }
   a.Head=Pivot(a.Hips,"Head",new Vector3(0,.90f,0));MeshArt.Oval(a.Head,"Face",Vector3.zero,new Vector3(.55f,.61f,.50f),skin,24);
   MeshArt.Oval(a.Head,"Nose",new Vector3(0,-.04f,.26f),new Vector3(.095f,.10f,.08f),skin);
   for(int side=-1;side<=1;side+=2){MeshArt.Oval(a.Head,"Ear",new Vector3(side*.27f,-.035f,0),new Vector3(.11f,.16f,.10f),skin);MeshArt.Oval(a.Head,"EyeWhite",new Vector3(side*.12f,.04f,.235f),new Vector3(.10f,.13f,.035f),white);MeshArt.Oval(a.Head,"Eye",new Vector3(side*.12f,.035f,.252f),new Vector3(.05f,.075f,.02f),dark);}
   MeshArt.Oval(a.Head,"HairCap",new Vector3(0,.19f,-.025f),new Vector3(.63f,.37f,.56f),MeshArt.Mat("HairChestnut",new Color(.20f,.065f,.018f)),24);
   for(int i=0;i<19;i++){float angle=i*2.399963f;float r=i<7?.13f:.25f;var pos=new Vector3(Mathf.Cos(angle)*r,.20f+(i%3)*.035f,Mathf.Sin(angle)*r);var clump=MeshArt.Lathe(a.Head,"SweptHairLock",new[]{0f,.08f,.19f,.28f},new[]{.105f,.11f,.07f,0f},10,MeshArt.Mat("HairLock"+(i%3),new Color(.29f+.045f*(i%3),.11f+.025f*(i%3),.025f)),new Vector3(1,1,.65f));clump.transform.localPosition=pos;clump.transform.localRotation=Quaternion.Euler(Mathf.Sin(angle)*45,angle*Mathf.Rad2Deg,Mathf.Cos(angle)*-38);}
   return a;
  }
  private static Transform Pivot(Transform p,string name,Vector3 at){var t=new GameObject(name).transform;t.SetParent(p,false);t.localPosition=at;return t;}
 }
}
