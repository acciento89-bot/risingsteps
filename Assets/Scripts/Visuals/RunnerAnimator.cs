using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Visuals
{
 public sealed class RunnerAnimator:MonoBehaviour
 {
  public Transform Hips,Head,LeftLeg,RightLeg,LeftArm,RightArm;public Material Hoodie;public PlayerMotor Motor;private float phase;
  public void Style(int style){Color[] colors={new Color(.035f,.43f,.23f),new Color(.045f,.35f,.78f),new Color(.82f,.40f,.06f),new Color(.51f,.16f,.73f)};Hoodie.color=colors[Mathf.Clamp(style,0,3)];}
  private void LateUpdate(){if(Motor==null||Hips==null)return;var v=Motor.Velocity;float speed=Motor.Paused?0:new Vector2(v.x,v.z).magnitude;phase+=Time.deltaTime*speed*2.2f;bool reduced=Motor.Course!=null&&Motor.Course.Profile.ReducedMotion;float wave=Mathf.Sin(phase)*Mathf.Clamp01(speed/3),stride=Motor.Grounded?wave*34:-18;
   LeftLeg.localRotation=Quaternion.Euler(stride,0,0);RightLeg.localRotation=Quaternion.Euler(-stride,0,0);LeftArm.localRotation=Quaternion.Euler(-stride*.8f,0,-12);RightArm.localRotation=Quaternion.Euler(stride*.8f,0,12);
   Hips.localPosition=new Vector3(0,.76f+(reduced?0:Mathf.Abs(Mathf.Sin(phase))*.025f*Mathf.Clamp01(speed)),0);Hips.localRotation=Quaternion.Euler(Motor.Grounded?Mathf.Clamp(speed*1.6f,0,8):8,0,0);}
 }
}
