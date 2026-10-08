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
  public static RisingProfile Parse(string json){try{var p=JsonUtility.FromJson<RisingProfile>(json);if(p==null||p.Schema!=1)return new RisingProfile();p.Normalize();return p;}catch{return new RisingProfile();}}
  public static bool Writable{get;private set;}=true;
  public static RisingProfile Load(){string raw=PlayerPrefs.GetString(Key,"");Writable=true;try{var header=JsonUtility.FromJson<RisingProfile>(raw);if(header!=null&&header.Schema!=1)Writable=false;}catch{}return Parse(raw);}
  public static void Save(RisingProfile p){if(!Writable)throw new InvalidOperationException("Newer save schema is preserved. Update the app before saving.");p.Normalize();PlayerPrefs.SetString(Key,JsonUtility.ToJson(p));PlayerPrefs.Save();}
 }
}
