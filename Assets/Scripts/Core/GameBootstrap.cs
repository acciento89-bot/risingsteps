using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.RisingSteps.CameraSystem;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.UI;
using Kamilunavo.RisingSteps.Visuals;
namespace Kamilunavo.RisingSteps
{
 public sealed class GameBootstrap:MonoBehaviour
 {
  private void Start(){
#if DEVELOPMENT_BUILD || UNITY_EDITOR
   Kamilunavo.RisingSteps.QA.RisingRuntimeQa.Configure();
#endif
Screen.orientation=ScreenOrientation.AutoRotation;Screen.autorotateToPortrait=true;Screen.autorotateToPortraitUpsideDown=false;Screen.autorotateToLandscapeLeft=true;Screen.autorotateToLandscapeRight=true;Application.targetFrameRate=60;QualitySettings.vSyncCount=0;QualitySettings.shadows=ShadowQuality.All;QualitySettings.shadowDistance=24;
   if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));Lighting();
   var player=new GameObject("Runner",typeof(CharacterController),typeof(PlayerMotor));var cc=player.GetComponent<CharacterController>();cc.height=1.96f;cc.radius=.25f;cc.center=new Vector3(0,.98f,0);cc.stepOffset=.15f;cc.skinWidth=.025f;
   var motor=player.GetComponent<PlayerMotor>();var runner=RunnerArt.Build(player.transform);runner.Motor=motor;
   var cameraObject=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitCamera));cameraObject.tag="MainCamera";var camera=cameraObject.GetComponent<Camera>();camera.fieldOfView=58;camera.nearClipPlane=.12f;camera.farClipPlane=260;camera.clearFlags=CameraClearFlags.Skybox;camera.allowHDR=true;var orbit=cameraObject.GetComponent<OrbitCamera>();orbit.Target=player.transform;orbit.Height=1.9f;orbit.Distance=7.2f;
   var course=new GameObject("RisingCourse",typeof(RisingCourse)).GetComponent<RisingCourse>();course.Player=player.transform;motor.Course=course;motor.CameraTransform=camera.transform;course.Build();cameraObject.transform.position=player.transform.position+new Vector3(0,3,-7);
   var hud=new GameObject("RisingHud",typeof(RisingHud)).GetComponent<RisingHud>();hud.Initialize(course);motor.Joystick=hud.Joystick;motor.Jump=hud.Jump;
   var feedback=new GameObject("RisingFeedback",typeof(RisingFeedback)).GetComponent<RisingFeedback>();feedback.Initialize(course,motor,hud);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
   Kamilunavo.RisingSteps.QA.RisingRuntimeQa.MaybeStart(course,hud,motor);
#endif
  }
  private static void Lighting(){var sun=new GameObject("Sun",typeof(Light)).GetComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.35f;sun.color=new Color(1,.89f,.70f);sun.shadows=LightShadows.Soft;sun.shadowStrength=.55f;sun.transform.rotation=Quaternion.Euler(42,-28,0);}
 }
}
