#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.UI;
namespace Kamilunavo.RisingSteps.QA
{
 public sealed class RisingRuntimeQa:MonoBehaviour
 {
  private RisingCourse _course;private RisingHud _hud;private PlayerMotor _motor;private string _out;private int _checks;private static bool Enabled=>Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-qaRising")||Environment.GetEnvironmentVariable("RISING_QA")=="1";
  public static void Configure(){if(Enabled)RisingSave.QaKey="kamilunavo.risingsteps.qa."+(Environment.GetEnvironmentVariable("RISING_QA_ID")??"local");}
  public static void MaybeStart(RisingCourse course,RisingHud hud,PlayerMotor motor){if(!Enabled)return;var qa=new GameObject("RisingRuntimeQa").AddComponent<RisingRuntimeQa>();qa._course=course;qa._hud=hud;qa._motor=motor;qa.StartCoroutine(qa.Run());}
  private static string Arg(string name,string fallback){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,name);return i>=0&&i+1<args.Length?args[i+1]:fallback;}
  private void Check(bool value,string name){_checks++;if(value){Debug.Log("QA_PASS "+name);return;}File.WriteAllText(Path.Combine(_out,"FAIL.txt"),name);Debug.LogError("RISING_QA_FAIL "+name);throw new Exception(name);}
  private IEnumerator Capture(string name){yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(Path.Combine(_out,name+".png"));yield return null;}
  private IEnumerator Run(){_out=Arg("-qaOutput",Path.Combine(Application.persistentDataPath,"RisingQA"));Directory.CreateDirectory(_out);yield return new WaitForSecondsRealtime(1);
   Check(_hud.ModalOpen&&_course.Paused,"initial home paused");yield return Capture("home");_hud.Close();_course.StartRun(0,false);yield return new WaitForSeconds(.5f);
   var center=new Vector2(Screen.width*.55f,Screen.height*.55f);var e=new PointerEventData(EventSystem.current){position=center};var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(e,hits);Check(hits.Count==0,"world route does not intercept camera input");
   _motor.Jump.OnPointerDown(e);_hud.ShowSettings();float before=_course.Profile.Elapsed;yield return new WaitForSeconds(.15f);Check(Mathf.Abs(before-_course.Profile.Elapsed)<.01f&&_motor.Paused,"modal pauses motor and timer");_hud.Close();Check(!_motor.Jump.Consume(),"modal clears stale jump");
   var jr=(RectTransform)_hud.Joystick.transform;Vector2 jc=RectTransformUtility.WorldToScreenPoint(null,jr.TransformPoint(jr.rect.center));var first=new PointerEventData(EventSystem.current){pointerId=100,position=jc+Vector2.up*35};_hud.Joystick.OnPointerDown(first);Vector2 value=_hud.Joystick.Value;var other=new PointerEventData(EventSystem.current){pointerId=101,position=jc+Vector2.right*35};_hud.Joystick.OnPointerDown(other);_hud.Joystick.OnPointerUp(other);Check(_hud.Joystick.Value==value,"second finger cannot steal joystick");_hud.Joystick.OnPointerUp(first);
   for(int realm=0;realm<3;realm++){_course.StartRun(realm,false);yield return new WaitForSeconds(.5f);for(int step=1;step<=12;step++){yield return Climb(step);yield return Capture("realm"+realm+"-step"+step);}
    yield return WalkTo(_course.Steps[12].transform.position+Vector3.forward*.5f,2);yield return new WaitForSeconds(.2f);Check(_course.Profile.Completed&&_hud.ModalOpen,"actual portal completion realm"+realm);Check(_course.Profile.Stars[realm]>0,"realm stars saved");_hud.Close();}
   var saved=RisingSave.Load();Check(saved.Step==12&&saved.Completed&&saved.UnlockedRealm==2,"saved checkpoint/realm completion");
   _course.StartRun(0,false);yield return Climb(1);_motor.ResetInput();var cc=_motor.GetComponent<CharacterController>();cc.enabled=false;_motor.transform.position+=Vector3.down*12;cc.enabled=true;yield return new WaitForSeconds(.2f);Check(_course.Profile.Falls==1&&Vector3.Distance(_motor.transform.position,_course.SafePosition)<1,"fall recovers checkpoint");
   int coins=_course.Profile.Crystals;bool claim=_course.ClaimDaily();int after=_course.Profile.Crystals;_course.ClaimDaily();Check(after==_course.Profile.Crystals&&(!claim||after>=coins+100),"daily guard runtime");_hud.ShowDaily();yield return Capture("daily");_hud.ShowStyle();yield return Capture("style");_hud.ShowSettings();yield return Capture("settings");_hud.Close();yield return Capture("gameplay");
   File.WriteAllText(Path.Combine(_out,"PASS.txt"),"RISING_QA_PASS checks="+_checks+" actual controller 36 steps; isolated save; no FPS claim");Debug.Log("RISING_QA_PASS checks="+_checks);
   if(Array.Exists(Environment.GetCommandLineArgs(),a=>a=="-qaExit"))Application.Quit();
  }
  private void Drive(Vector3 direction,bool start=false){direction.y=0;var f=_motor.CameraTransform.forward;f.y=0;f.Normalize();var r=_motor.CameraTransform.right;r.y=0;r.Normalize();var axis=new Vector2(Vector3.Dot(direction.normalized,r),Vector3.Dot(direction.normalized,f));var rect=(RectTransform)_hud.Joystick.transform;var local=rect.rect.center+(Vector2)axis*Mathf.Min(rect.rect.width,rect.rect.height)*.5f;var ev=new PointerEventData(EventSystem.current){pointerId=200,position=RectTransformUtility.WorldToScreenPoint(null,rect.TransformPoint(local))};if(start)_hud.Joystick.OnPointerDown(ev);else _hud.Joystick.OnDrag(ev);}
  private IEnumerator Climb(int step){var origin=_course.Steps[step-1].transform.position;var target=_course.Steps[step].transform.position;bool jumped=false;float start=Time.realtimeSinceStartup;Drive(target-_motor.transform.position,true);
   while(_course.Height<step&&Time.realtimeSinceStartup-start<6){var delta=target-_motor.transform.position;delta.y=0;if(delta.magnitude<.12f)_hud.Joystick.ResetInput();else{if(_hud.Joystick.Value==Vector2.zero)Drive(delta,true);else Drive(delta);}
    var travel=_motor.transform.position-origin;travel.y=0;if(!jumped&&Vector3.Dot(travel,new Vector3(target.x-origin.x,0,target.z-origin.z).normalized)>.72f&&_motor.Grounded){_hud.Jump.OnPointerDown(new PointerEventData(EventSystem.current));jumped=true;}yield return null;}
   _motor.ResetInput();Check(_course.Height==step,"controller lands step"+step+" position="+_motor.transform.position+" grounded="+_motor.Grounded+" jumped="+jumped+" paused="+_course.Paused);yield return new WaitForSeconds(.05f);
  }
  private IEnumerator WalkTo(Vector3 goal,float seconds){float start=Time.realtimeSinceStartup;Drive(goal-_motor.transform.position,true);while(Time.realtimeSinceStartup-start<seconds&&!_course.Profile.Completed){var d=goal-_motor.transform.position;d.y=0;if(d.magnitude<.12f)break;Drive(d);yield return null;}_motor.ResetInput();}
 }
}
#endif
