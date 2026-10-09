// Standalone reproduction: compile against the actual OrbitCamera source, with this input boundary.
using System;
using System.Reflection;
using Kamilunavo.RisingSteps.CameraSystem;
namespace UnityEngine {
 public class MonoBehaviour { public Transform transform=new Transform(); public T GetComponent<T>() where T:new()=>new T(); }
 public class Transform { public Vector3 position;public Quaternion rotation;public T GetComponent<T>() where T:new()=>new T(); }
 public struct Vector2 {public float x,y;public Vector2(float x,float y){this.x=x;this.y=y;}public static Vector2 operator -(Vector2 a,Vector2 b)=>new Vector2(a.x-b.x,a.y-b.y);public static Vector2 operator /(Vector2 a,float v)=>new Vector2(a.x/v,a.y/v);}
 public struct Vector3 {public static Vector3 up=>new Vector3();public static Vector3 forward=>new Vector3();public static Vector3 operator +(Vector3 a,Vector3 b)=>a;public static Vector3 operator -(Vector3 a,Vector3 b)=>a;public static Vector3 operator *(Vector3 a,float b)=>a;public static Vector3 Lerp(Vector3 a,Vector3 b,float t)=>a;}
 public struct Quaternion {public static Quaternion Euler(float x,float y,float z)=>new Quaternion();public static Vector3 operator *(Quaternion a,Vector3 b)=>b;}
 public struct Rect {public bool Contains(Vector2 v)=>true;}
 public class Camera {public Rect pixelRect;}
 public static class Mathf {public static float Exp(float f)=>(float)Math.Exp(f);public static float Clamp(float v,float min,float max)=>Math.Max(min,Math.Min(max,v));}
 public static class Time {public static float deltaTime=.016f;}
 public enum TouchPhase {Began,Moved,Stationary,Ended,Canceled}
 public struct Touch {public int fingerId;public TouchPhase phase;public Vector2 position;}
 public static class Application {public static bool isMobilePlatform=true;}
 public static class Input {public static Touch[] touches=new Touch[0];public static int touchCount=>touches.Length;public static Touch GetTouch(int n)=>touches[n];public static bool mousePresent=true;public static bool RightDown;public static float MouseX,MouseY;public static bool GetMouseButton(int n)=>n==1&&RightDown;public static float GetAxis(string n)=>n=="Mouse X"?MouseX:MouseY;}
}
namespace UnityEngine.EventSystems {public interface IPointerDownHandler{}public interface IDragHandler{}public interface IPointerUpHandler{}public class PointerEventData {public int pointerId;public UnityEngine.Vector2 position;}public class EventSystem {public static EventSystem current=new EventSystem(); public bool IsPointerOverGameObject(int id)=>id==11||id==22;}}
namespace Kamilunavo.RisingSteps.Gameplay {public class PlayerMotor {public bool Paused;}}
namespace Kamilunavo.RisingSteps.UI {public static class UiMetrics {public static float PointScale=>3;}}
class TouchCameraProbe {
 static UnityEngine.Touch T(int id,UnityEngine.TouchPhase phase,float x,float y)=>new UnityEngine.Touch{fingerId=id,phase=phase,position=new UnityEngine.Vector2(x,y)};
 static void Frame(OrbitCamera c,params UnityEngine.Touch[] t){UnityEngine.Input.touches=t;typeof(OrbitCamera).GetMethod("LateUpdate",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(c,null);}
 static int Main(){var camera=new OrbitCamera{Target=new UnityEngine.Transform()};
  // Unity legacy input can report a held movement touch plus a new jump touch
  // as right mouse down. Exercise the real LateUpdate, not only UI handlers.
  Frame(camera,T(11,UnityEngine.TouchPhase.Began,55,100));
  UnityEngine.Input.RightDown=true;UnityEngine.Input.MouseX=-10;UnityEngine.Input.MouseY=4;
  Frame(camera,T(11,UnityEngine.TouchPhase.Moved,55,140),T(22,UnityEngine.TouchPhase.Began,330,100));
  if(Math.Abs(camera.Yaw)>.001f){Console.WriteLine("FAIL move-before-jump synthesized right mouse rotated yaw="+camera.Yaw);return 1;}
  Frame(camera,T(22,UnityEngine.TouchPhase.Began,330,100),T(11,UnityEngine.TouchPhase.Moved,55,140));
  if(Math.Abs(camera.Yaw)>.001f){Console.WriteLine("FAIL reversed touch order rotated camera");return 1;}
  Frame(camera); // stale synthesized mouse state must not rotate on mobile either.
  if(Math.Abs(camera.Yaw)>.001f){Console.WriteLine("FAIL mobile mouse tail rotated camera");return 1;}
  UnityEngine.Application.isMobilePlatform=false;
  Frame(camera,T(11,UnityEngine.TouchPhase.Moved,55,140),T(22,UnityEngine.TouchPhase.Began,330,100));
  if(Math.Abs(camera.Yaw)>.001f){Console.WriteLine("FAIL desktop touchscreen leaked synthesized mouse");return 1;}
  Frame(camera);if(Math.Abs(camera.Yaw+30)>.001f){Console.WriteLine("FAIL genuine desktop right mouse stopped orbiting yaw="+camera.Yaw);return 1;}
  camera.ResetView();UnityEngine.Application.isMobilePlatform=true;UnityEngine.Input.RightDown=false;
  Console.WriteLine("PASS movement-before-jump, reversed touch order, mobile mouse tail, touchscreen and genuine desktop orbit");
  // 11 is the held joystick, 22 the simultaneous jump, 33 an unintended free-view brush.
  Frame(camera,T(11,UnityEngine.TouchPhase.Began,55,100),T(22,UnityEngine.TouchPhase.Began,330,100),T(33,UnityEngine.TouchPhase.Began,210,350));
  Frame(camera,T(11,UnityEngine.TouchPhase.Moved,55,140),T(22,UnityEngine.TouchPhase.Stationary,330,100),T(33,UnityEngine.TouchPhase.Moved,10,350));
  if(Math.Abs(camera.Yaw)>.001f){Console.WriteLine("FAIL ambient third finger rotated camera during movement+jump: yaw="+camera.Yaw);return 1;}
  Console.WriteLine("PASS ambient finger cannot rotate movement+jump camera");
#if OWNERSHIP
  var pad=new Kamilunavo.RisingSteps.Input.CameraLookControl{Camera=camera};
  var foreign=new UnityEngine.EventSystems.PointerEventData{pointerId=11,position=new UnityEngine.Vector2(400,200)};pad.OnDrag(foreign);if(camera.Yaw!=0)throw new Exception("unclaimed joystick finger rotated camera");
  var owner=new UnityEngine.EventSystems.PointerEventData{pointerId=33,position=new UnityEngine.Vector2(100,200)};pad.OnPointerDown(owner);pad.OnPointerDown(foreign);pad.OnDrag(foreign);pad.OnPointerUp(foreign);if(camera.Yaw!=0)throw new Exception("foreign finger stole View ownership");
  owner.position=new UnityEngine.Vector2(220,200);pad.OnDrag(owner);if(Math.Abs(camera.Yaw-6)>.001f)throw new Exception("Retina3x 40point drag should rotate6degrees, got "+camera.Yaw);
  pad.OnPointerUp(owner);owner.position=new UnityEngine.Vector2(340,200);pad.OnDrag(owner);if(Math.Abs(camera.Yaw-6)>.001f)throw new Exception("released owner still rotates");
  pad.OnPointerDown(owner);pad.ResetInput();owner.position=new UnityEngine.Vector2(460,200);pad.OnDrag(owner);if(Math.Abs(camera.Yaw-6)>.001f)throw new Exception("reset owner still rotates");camera.ResetView();if(camera.Yaw!=0)throw new Exception("route reset did not face forward");
  Console.WriteLine("PASS View ownership, foreign release, logical-point sensitivity, own release, pause reset, forward reset");
#endif
  return 0;
 }
}
