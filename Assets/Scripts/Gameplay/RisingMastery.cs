using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Core.Mastery;

namespace Kamilunavo.RisingSteps.Gameplay
{
 // Clock advancement precedes PlayerMotor.Update; only accepted course events
 // supply splits. This observer never moves the player or awards progression.
 [DefaultExecutionOrder(-20)]
 public sealed class RisingMastery:MonoBehaviour
 {
  RisingCourse _course;MasteryFileArchive _archive;MasteryBook _book=MasteryBook.Empty;MasteryTrial _trial;MasteryAttempt _pending;
  MasteryGuide _guide;MasteryKey _key;bool _subscribed,_requested;int _realm,_level,_skipThroughFrame;
  GameObject _marker;Mesh _mesh;Material _material;Renderer _renderer;
  public MasteryBook Book=>_book;
  public MasteryAttempt LastCompleted{get;private set;}
  public string Error{get;private set;}
  public bool Recording=>_trial!=null&&!_trial.Ended;
  public bool HasPending=>_pending!=null;
  public double Elapsed=>_trial==null?0:_trial.Elapsed;
  public event Action<int,double> Split;
  public void Initialize(RisingCourse course)
  {
   if(_course!=null)throw new InvalidOperationException("Journal already initialized");_course=course;
   try{string filename="risingsteps-mastery-v1.json";
#if DEVELOPMENT_BUILD || UNITY_EDITOR
   if(!string.IsNullOrEmpty(RisingSave.QaKey)){using(var sha=SHA256.Create())filename="risingsteps-mastery-qa-"+BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(RisingSave.QaKey))).Replace("-","").ToLowerInvariant()+".json";}
#endif
   _archive=new MasteryFileArchive(Path.Combine(Application.persistentDataPath,filename),new MasteryUnityCodec());Reload();}
   catch(Exception error){Fail(error);}
   Subscribe();
  }
  void Reload(){if(_archive==null){Error="Journal storage unavailable";return;}var state=_archive.Load();Error=state.Error;if(state.Writable)_book=state.Book;}
  void Subscribe(){if(_subscribed||_course==null)return;_course.RunStarted+=RunStarted;_course.Landed+=Landed;_course.Fell+=Fell;_course.PortalCompleted+=Completed;_subscribed=true;}
  void Unsubscribe(){if(!_subscribed)return;_course.RunStarted-=RunStarted;_course.Landed-=Landed;_course.Fell-=Fell;_course.PortalCompleted-=Completed;_subscribed=false;}
  void OnEnable(){Subscribe();}
  void OnDisable(){Unsubscribe();Cancel();}
  static void Release(UnityEngine.Object value){if(value==null)return;if(Application.isPlaying)Destroy(value);else DestroyImmediate(value);}
  void OnDestroy(){Unsubscribe();Release(_marker);Release(_mesh);Release(_material);}
  public bool StartRecorded(int realm,int level)
  {
   if(_course==null||_course.NativePresentationActive||!_course.Profile.TutorialDone||!RisingCampaign.IsUnlocked(_course.Profile,realm,level)||_pending!=null)return false;
   Reload();if(Error!=null)return false;
   _requested=true;_realm=realm;_level=level;
   try{_course.StartLevel(realm,level);}catch(Exception error){Fail(error);}
   finally{_requested=false;}
   return Recording;
  }
  void RunStarted()
  {
   Cancel();LastCompleted=null;
   if(!_requested)return;
   try
   {
    var profile=_course.Profile;
    if(profile.Realm!=_realm||profile.Level!=_level||profile.Step!=0||profile.Challenge||!profile.TutorialDone||!RisingCampaign.IsUnlocked(profile,_realm,_level))return;
    _key=CourseKey(_course);_trial=new MasteryTrial(_key,Guid.NewGuid().ToString("N"),0,false);
    _trial.Advance(0,true);Capture(true);
    // The next motor update settles the rebuilt route; its long loading delta
    // must not count toward a newly requested recorded climb.
    _skipThroughFrame=Time.frameCount+1;
    var entry=MasteryBookRules.Find(_book,_key);
    if(entry!=null&&entry.Best!=null&&entry.Best.Samples.Length>0)_guide=new MasteryGuide(_key,entry.Best.Samples);
   }
   catch(Exception error){Fail(error);Cancel();}
  }
  void Update()
  {
   if(!Recording)return;
   try{bool active=Time.frameCount>_skipThroughFrame&&!_course.Paused&&_course.Foreground&&!_course.NativePresentationActive;
    _trial.Advance(Time.deltaTime,active);if(_trial.Ended){Error="Recording reached the one-hour limit";Hide();}}
   catch(Exception error){Fail(error);Cancel();}
  }
  void LateUpdate()
  {
   if(!Recording||_course.Paused||!_course.Foreground||_course.NativePresentationActive){Hide();return;}
   try
   {
    Capture(false);
    if(!_book.GuideEnabled||_course.Profile.ReducedMotion||_guide==null){Hide();return;}
    EnsureMarker();var point=_guide.At(_trial.Elapsed,_key);_marker.transform.position=new Vector3((float)point.X,(float)point.Y+.85f,(float)point.Z);_renderer.enabled=true;
   }
   catch(Exception error){Fail(error);Cancel();}
  }
  void Capture(bool force){var point=_course.Player.position;_trial.Capture(point.x,point.y,point.z,force);}
  void Landed(bool perfect)
  {
   if(!Recording)return;
   try{int step=_course.Height;if(!_trial.Land(step,perfect)){Error="Landing sequence was interrupted; this climb is not recorded";Cancel();return;}Split?.Invoke(step,_trial.Elapsed);}
   catch(Exception error){Fail(error);Cancel();}
  }
  void Fell(){if(Recording)_trial.Fall();}
  void Completed()
  {
   if(!Recording)return;
   try{var point=_course.Player.position;var result=_trial.Complete(DateTime.UtcNow.ToString("O",CultureInfo.InvariantCulture),point.x,point.y,point.z);if(result==null)return;LastCompleted=result;_pending=result;Hide();RetryPending();}
   catch(Exception error){Fail(error);Cancel();}
  }
  public bool RetryPending()
  {
   if(_pending==null){Reload();return Error==null;}
   try
   {
    Reload();if(Error!=null)return false;var next=MasteryBookRules.Record(_book,_pending);
    if(next.Revision!=_book.Revision)_archive.Save(next,_book.Revision);
    _book=next;_pending=null;Error=null;return true;
   }
   catch(Exception error){Fail(error);return false;}
  }
  public bool SetGuide(bool enabled)
  {
   if(_pending!=null)return false;
   try{Reload();if(Error!=null)return false;var next=MasteryBookRules.SetGuideEnabled(_book,enabled);if(next.Revision!=_book.Revision)_archive.Save(next,_book.Revision);_book=next;Error=null;if(!enabled)Hide();return true;}
   catch(Exception error){Fail(error);return false;}
  }
  void Cancel(){if(_trial!=null)_trial.Cancel();_trial=null;_guide=null;Hide();}
  void Hide(){if(_renderer!=null)_renderer.enabled=false;}
  void Fail(Exception error){Error=error.Message;Debug.LogWarning("Trial journal unavailable: "+error.Message);}
  void OnApplicationPause(bool paused){if(_trial!=null)_trial.Advance(0,false);Hide();}
  void OnApplicationFocus(bool focused){if(_trial!=null)_trial.Advance(0,false);Hide();}
  void EnsureMarker()
  {
   if(_marker!=null)return;
   var source=Resources.Load<Material>("Materials/Glow");if(source==null)throw new InvalidOperationException("Pace marker material unavailable");
   _material=new Material(source){name="OwnedPersonalPaceMarker",color=new Color(.3f,.8f,.9f)};
   _mesh=new Mesh{name="OwnedPersonalPaceMesh"};
   _mesh.vertices=new[]{new Vector3(0,.22f,0),new Vector3(-.13f,0,0),new Vector3(0,0,.13f),new Vector3(.13f,0,0),new Vector3(0,0,-.13f),new Vector3(0,-.22f,0)};
   _mesh.triangles=new[]{0,2,1,0,3,2,0,4,3,0,1,4,5,1,2,5,2,3,5,3,4,5,4,1};_mesh.RecalculateNormals();_mesh.RecalculateBounds();
   _marker=new GameObject("PersonalPaceMarker",typeof(MeshFilter),typeof(MeshRenderer));_marker.layer=2;_marker.transform.SetParent(transform,false);
   _marker.GetComponent<MeshFilter>().sharedMesh=_mesh;_renderer=_marker.GetComponent<MeshRenderer>();_renderer.sharedMaterial=_material;_renderer.shadowCastingMode=ShadowCastingMode.Off;_renderer.receiveShadows=false;_renderer.enabled=false;
  }
  public static MasteryKey CourseKey(RisingCourse course)
  {
   if(course.Steps.Count!=13)throw new InvalidOperationException("Incomplete route geometry");
   using(var stream=new MemoryStream())using(var writer=new BinaryWriter(stream))
   {
    writer.Write("risingsteps-full-climb-v1");
    foreach(var step in course.Steps)
    {
     if(step==null)throw new InvalidOperationException("Missing route landing");writer.Write(step.Index);
     var collider=step.GetComponent<MeshCollider>();if(collider==null||collider.sharedMesh==null||!collider.enabled)throw new InvalidOperationException("Unrecognized landing geometry");
     var mesh=collider.sharedMesh;var vertices=mesh.vertices;var triangles=mesh.triangles;writer.Write(vertices.Length);
     foreach(var vertex in vertices){var point=collider.transform.TransformPoint(vertex);if(!MasteryBounds.Pose(point.x,point.y,point.z))throw new InvalidOperationException("Invalid landing geometry");writer.Write(point.x);writer.Write(point.y);writer.Write(point.z);}
     writer.Write(triangles.Length);foreach(int index in triangles)writer.Write(index);
    }
    writer.Flush();using(var sha=SHA256.Create()){var result=new StringBuilder(64);foreach(byte value in sha.ComputeHash(stream.ToArray()))result.Append(value.ToString("x2"));return new MasteryKey(course.Profile.Realm,course.Profile.Level,result.ToString());}
   }
  }
 }
}
