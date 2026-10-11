using System;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Core
{
 public static class RisingSave
 {
  public static string Key{
   get{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
    if(!string.IsNullOrEmpty(QaKey))return QaKey;
#endif
    return "kamilunavo.risingsteps.profile.v1";
   }
  }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
  public static string QaKey;
#endif
  public static RisingProfile Parse(string json){try{var p=JsonUtility.FromJson<RisingProfile>(json);if(p==null||(p.Schema!=1&&p.Schema!=2&&p.Schema!=3))return new RisingProfile();if(!json.Contains("\"Music\""))p.Music=true;p.Normalize();if(!json.Contains("\"TutorialDone\"")&&(p.Step>0||p.Stars[0]>0||p.Stars[1]>0||p.Stars[2]>0))p.TutorialDone=true;return p;}catch{return new RisingProfile();}}
  public static bool Writable{get;private set;}=true;
  public static RisingProfile Load(){string raw=PlayerPrefs.GetString(Key,"");Writable=true;try{var header=JsonUtility.FromJson<RisingProfile>(raw);if(header!=null&&(header.Schema!=1&&header.Schema!=2&&header.Schema!=3))Writable=false;}catch{}return Parse(raw);}
  public static void Save(RisingProfile p){if(!Writable)throw new InvalidOperationException("Newer save schema is preserved. Update the app before saving.");p.Normalize();PlayerPrefs.SetString(Key,JsonUtility.ToJson(p));PlayerPrefs.Save();}
 }
}
