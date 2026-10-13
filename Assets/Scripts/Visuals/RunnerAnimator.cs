using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;

namespace Kamilunavo.RisingSteps.Visuals
{
 public sealed class RunnerAnimator:MonoBehaviour
 {
  // Existing bindings remain intact; elbows and knees add presentation only.
  public Transform Hips,Head,LeftLeg,RightLeg,LeftArm,RightArm;
  public Transform LeftKnee,RightKnee,LeftElbow,RightElbow;
  public Material Hoodie;public PlayerMotor Motor;
  private float _phase;
  private Vector3 _hipsRest;
  private bool _poseCaptured;
  private readonly Transform[] _crests=new Transform[4];
  private static readonly Color[] StyleColors={new Color(.055f,.32f,.17f),new Color(.045f,.35f,.78f),new Color(.82f,.40f,.06f),new Color(.51f,.16f,.73f),new Color(.98f,.66f,.10f),new Color(.08f,.65f,.90f),new Color(.38f,.16f,.76f),new Color(.035f,.055f,.14f)};

  internal void CaptureRestPose()
  {
   _hipsRest=Hips.localPosition;
   for(int i=0;i<_crests.Length;i++)_crests[i]=Hips.Find("PremiumCrest"+(i+4));
   _poseCaptured=true;
  }
  public void Style(int style)
  {
   if(Hoodie!=null)Hoodie.color=StyleColors[Mathf.Clamp(style,0,7)];
   for(int i=0;i<_crests.Length;i++){
    // Fallback retains compatibility with a manually bound original hierarchy.
    if(_crests[i]==null&&Hips!=null)_crests[i]=Hips.Find("PremiumCrest"+(i+4));
    if(_crests[i]!=null)_crests[i].gameObject.SetActive(i+4==style);
   }
  }
  private void LateUpdate()
  {
   if(Motor==null||Hips==null)return;
   if(!_poseCaptured)CaptureRestPose();
   var velocity=Motor.Velocity;float speed=Motor.Paused?0:new Vector2(velocity.x,velocity.z).magnitude;
   _phase+=Time.deltaTime*speed*2.2f;
   bool reduced=Motor.Course!=null&&Motor.Course.Profile!=null&&Motor.Course.Profile.ReducedMotion;
   float movement=Mathf.Clamp01(speed/3),wave=Mathf.Sin(_phase)*movement;
   float idle=1-movement;float stride=Motor.Grounded?wave*29:-18;
   LeftLeg.localRotation=Quaternion.Euler(stride-3*idle,0,-2*idle);RightLeg.localRotation=Quaternion.Euler(-stride+4*idle,0,2*idle);
   LeftArm.localRotation=Quaternion.Euler(-stride*.72f-7*idle,0,-11);RightArm.localRotation=Quaternion.Euler(stride*.72f+3*idle,0,11);
   if(LeftKnee!=null)LeftKnee.localRotation=Quaternion.Euler(Motor.Grounded?Mathf.Max(0,-wave)*26+6*idle:24,0,0);
   if(RightKnee!=null)RightKnee.localRotation=Quaternion.Euler(Motor.Grounded?Mathf.Max(0,wave)*26+2*idle:12,0,0);
   if(LeftElbow!=null)LeftElbow.localRotation=Quaternion.Euler(-12-Mathf.Max(0,wave)*18-(Motor.Grounded?0:15),0,0);
   if(RightElbow!=null)RightElbow.localRotation=Quaternion.Euler(-12-Mathf.Max(0,-wave)*18-(Motor.Grounded?0:15),0,0);
   Hips.localPosition=_hipsRest+Vector3.up*(reduced?0:Mathf.Abs(Mathf.Sin(_phase))*.018f*movement);
   Hips.localRotation=Quaternion.Euler(Motor.Grounded?Mathf.Clamp(speed*1.2f,0,6):8,0,0);
  }
 }
}
