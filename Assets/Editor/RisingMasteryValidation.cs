#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.Core.Mastery;
using UnityEngine;

public static class RisingMasteryValidation
{
 static void Check(bool value,string label){if(!value)throw new Exception("RISING_MASTERY: "+label);}
 static void Reject(Action action,string label){try{action();}catch{return;}throw new Exception("RISING_MASTERY expected rejection: "+label);}
 public static void Validate()
 {
  var codec=new MasteryUnityCodec();var key=new MasteryKey(2,9,new string('a',64));
  var run=new MasteryTrial(key,Guid.NewGuid().ToString("N"),0,false);run.Advance(0,true);run.Capture(1.125,-2.25,3.5);
  for(int i=1;i<=12;i++){run.Advance(.312345678901,true);run.Capture(i+.125,i*.125,-i*.25);Check(run.Land(i,i%2==0),"real split accepted");}
  var attempt=run.Complete(DateTime.UtcNow.ToString("O"),12.125,1.5,-3);
  var book=MasteryBookRules.Record(MasteryBook.Empty,attempt);
  var json=codec.Encode(book);var restored=codec.Decode(json);
  Check(restored.Revision==1&&restored.Entries.Length==1&&MasteryBookRules.SameFacts(restored.Entries[0].Best,attempt),"Unity JSON exact double/splits/key/count round trip");
  Check(restored.Entries[0].Best.Samples.Length==attempt.Samples.Length&&restored.Entries[0].Best.Samples[0].X==1.125,"actual samples retained");
  Check(codec.Encode(restored)==json,"stable Unity canonical representation");
  for(int i=0;i<attempt.Samples.Length;i++){var before=attempt.Samples[i];var after=restored.Entries[0].Best.Samples[i];Check(BitConverter.DoubleToInt64Bits(before.Seconds)==BitConverter.DoubleToInt64Bits(after.Seconds)&&BitConverter.DoubleToInt64Bits(before.X)==BitConverter.DoubleToInt64Bits(after.X)&&BitConverter.DoubleToInt64Bits(before.Y)==BitConverter.DoubleToInt64Bits(after.Y)&&BitConverter.DoubleToInt64Bits(before.Z)==BitConverter.DoubleToInt64Bits(after.Z),"every measured sample bit retained");}
  Check(codec.Decode(codec.Encode(MasteryBook.Empty)).Entries.Length==0,"empty book round trip");
  var fallen=attempt.Copy();fallen.ID=Guid.NewGuid().ToString("N");fallen.Falls=1;var noBest=MasteryBookRules.Record(MasteryBook.Empty,fallen);Check(codec.Decode(codec.Encode(noBest)).Entries[0].Best==null,"fall-only history retains absent best");
  Reject(()=>codec.Decode("{}"),"missing archive header");Reject(()=>codec.Decode("null"),"null archive");Reject(()=>codec.Decode("{\"Schema\":1}"),"incomplete header");
  Reject(()=>codec.Decode(json.Replace("\"Schema\":1","\"Schema\":99")),"future envelope schema");
  Reject(()=>codec.Decode(json.Replace("3ff2000000000000","3ff2000000000001")),"modified payload checksum");
  var future=book.Copy();future.Schema=99;Reject(()=>codec.Encode(future),"future book cannot write");
  string directory=Path.Combine(Path.GetTempPath(),"rising-unity-mastery-"+Guid.NewGuid().ToString("N"));
  try{var archive=new MasteryFileArchive(Path.Combine(directory,"journal.json"),codec);archive.Save(book,0);Check(MasteryBookRules.SameFacts(archive.Load().Book.Entries[0].Best,attempt),"Unity codec durable file round trip");var second=MasteryBookRules.SetGuideEnabled(book,true);archive.Save(second,1);Check(archive.Load().Book.GuideEnabled,"Unity codec replacement save");}
  finally{if(Directory.Exists(directory))Directory.Delete(directory,true);}
  ValidateMarker();
  Debug.Log("RISING_MASTERY_UNITY_CODEC_PASS exact facts/samples, malformed/future/checksum rejection and atomic files");
 }
 static void ValidateMarker()
 {
  var owner=new GameObject("OwnedTrialMarkerValidation");Mesh mesh=null;Material material=null;
  try
  {
   var observer=owner.AddComponent<RisingMastery>();
   typeof(RisingMastery).GetMethod("EnsureMarker",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(observer,null);
   Check(owner.GetComponentsInChildren<Collider>(true).Length==0&&owner.GetComponentsInChildren<Rigidbody>(true).Length==0&&owner.GetComponentsInChildren<StepMarker>(true).Length==0&&owner.GetComponentsInChildren<Light>(true).Length==0,"marker has no physics/progression/light authority");
   var renderer=owner.GetComponentInChildren<MeshRenderer>(true);Check(renderer!=null&&!renderer.enabled,"pace marker initially hidden");
   mesh=owner.GetComponentInChildren<MeshFilter>(true).sharedMesh;material=renderer.sharedMaterial;
  }
  finally{
   // Edit-mode components have not run Awake; Unity does not dispatch their
   // runtime OnDestroy. Exercise the exact cleanup hook explicitly here.
   if(owner!=null){var observer=owner.GetComponent<RisingMastery>();if(observer!=null)typeof(RisingMastery).GetMethod("OnDestroy",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(observer,null);UnityEngine.Object.DestroyImmediate(owner);}
  }
  Check(mesh==null&&material==null,"direct cleanup hook releases owned marker mesh/material");
 }
}
#endif
