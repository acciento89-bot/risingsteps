using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.UI;
namespace Kamilunavo.RisingSteps.Visuals
{
 public sealed class RisingFeedback:MonoBehaviour
 {
  private RisingCourse _course;private AudioSource _sfx,_ambient;private AudioClip[] _clips;private ParticleSystem _spark;
#if UNITY_IOS && !UNITY_EDITOR
  [System.Runtime.InteropServices.DllImport("__Internal")]private static extern void RSHaptic(int kind);
#endif
  public void Initialize(RisingCourse course,PlayerMotor motor,RisingHud hud){_course=course;_sfx=gameObject.AddComponent<AudioSource>();_sfx.playOnAwake=false;_sfx.volume=.28f;_ambient=gameObject.AddComponent<AudioSource>();_ambient.loop=true;_ambient.clip=Resources.Load<AudioClip>("Audio/SkyAmbient");_ambient.volume=.10f;_ambient.Play();_clips=new AudioClip[6];string[] names={"Jump","Landing","Perfect","Fall","Menu","Portal"};for(int i=0;i<6;i++)_clips[i]=Resources.Load<AudioClip>("Audio/"+names[i]);
   var go=new GameObject("LandingSparkles",typeof(ParticleSystem));_spark=go.GetComponent<ParticleSystem>();_spark.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=_spark.main;main.playOnAwake=false;main.loop=false;main.duration=.6f;main.simulationSpace=ParticleSystemSimulationSpace.World;main.startLifetime=.45f;main.startSpeed=2.5f;main.startSize=.075f;main.startColor=new Color(1,.85f,.18f);main.gravityModifier=.3f;main.maxParticles=64;var em=_spark.emission;em.enabled=false;var shape=_spark.shape;shape.shapeType=ParticleSystemShapeType.Cone;shape.radius=.6f;shape.angle=40;go.transform.SetParent(transform,false);go.transform.rotation=Quaternion.Euler(-90,0,0);_spark.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var pm=new Material(Resources.Load<Material>("Materials/Sparkles"));pm.SetColor("_Color",new Color(1,.87f,.32f));_spark.GetComponent<ParticleSystemRenderer>().sharedMaterial=pm;ArtLifetime.Own(go,pm);
   motor.Jumped+=()=>Play(0,0);course.Landed+=perfect=>{Play(perfect?2:1,perfect?1:0);if(!course.Profile.ReducedMotion){go.transform.position=course.Player.position+Vector3.up*.15f;_spark.Play();_spark.Emit(perfect?24:10);}};course.Fell+=()=>Play(3,1);hud.UiPressed+=()=>Play(4,0);course.PortalCompleted+=()=>Play(5,2);
  }
  private void Update(){if(_course==null)return;_ambient.mute=!_course.Profile.Sound;_sfx.mute=!_course.Profile.Sound;}
  private void Play(int sound,int haptic){if(_course.Profile.Sound&&_clips[sound]!=null)_sfx.PlayOneShot(_clips[sound]);if(!_course.Profile.Haptics)return;
#if UNITY_IOS && !UNITY_EDITOR
   RSHaptic(haptic);
#elif UNITY_ANDROID && !UNITY_EDITOR
   using(var unity=new AndroidJavaClass("com.unity3d.player.UnityPlayer")){var activity=unity.GetStatic<AndroidJavaObject>("currentActivity");activity.Call("runOnUiThread",new AndroidJavaRunnable(()=>{using var window=activity.Call<AndroidJavaObject>("getWindow");using var view=window.Call<AndroidJavaObject>("getDecorView");view.Call<bool>("performHapticFeedback",haptic==2?16:1);}));}
#endif
  }
 }
}
