#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.UI;
using Kamilunavo.RisingSteps.Monetization;
public static class RisingNavigationValidation
{
 static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception("RISING_NAVIGATION: "+message);}
 public static void Validate(){checks=0;foreach(var pane in new[]{new Vector2(375,627),new Vector2(734,343),new Vector2(190,627)})foreach(var language in new[]{"de","en"})Fixture(pane,language);Debug.Log("RISING_NAVIGATION_PASS checks="+checks+" actual callbacks, ten-realm selection, scroll, refresh, language, provider locks, pause, resume");}
 static void Fixture(Vector2 pane,string language)
 {
  var root=new GameObject("NavigationFixture");Canvas canvas=null;string previousKey=RisingSave.QaKey;bool previousWritable=RisingSave.Writable;string key="qa.navigation."+Guid.NewGuid().ToString("N");RisingSave.QaKey=key;
  try{
   RisingSave.Load();var profile=new RisingProfile{TutorialDone=true,Language=language,UnlockedRealm=9,Realm=9,Crystals=428};for(int i=0;i<profile.LevelStars.Length;i++)profile.LevelStars[i]=1;
   var course=root.AddComponent<RisingCourse>();typeof(RisingCourse).GetField("<Profile>k__BackingField",Flags).SetValue(course,profile);
   course.Player=new GameObject("FixturePlayer").transform;course.Player.SetParent(root.transform);
   var hud=root.AddComponent<RisingHud>();hud.Initialize(course);course.Hud=hud;
   var safe=(RectTransform)typeof(RisingHud).GetField("_safe",Flags).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=3;UiMetrics.QaPointScale=3;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=Vector2.one*.5f;safe.sizeDelta=pane;
   void Layout()=>typeof(RisingHud).GetMethod("Layout",Flags).Invoke(hud,null);
   void Settled(){Canvas.ForceUpdateCanvases();typeof(RisingHud).GetMethod("LateUpdate",Flags).Invoke(hud,null);Canvas.ForceUpdateCanvases();}
   RectTransform Modal()=> (RectTransform)typeof(RisingHud).GetField("_modal",Flags).GetValue(hud);
   string Page()=> (string)typeof(RisingHud).GetField("_page",Flags).GetValue(hud);
   ScrollRect Scroll()=>Modal().GetComponentInChildren<ScrollRect>();
   Button Find(string name){foreach(var b in Modal().GetComponentsInChildren<Button>())if(b.name==name)return b;return null;}
   void Click(string name){var b=Find(name);Check(b!=null,"button "+name+" exists on "+Page());b.onClick.Invoke();Settled();}
   Layout();hud.ShowHome();Settled();
   hud.ShowSettings();Settled();Check(Find("Back")!=null,"settings offers previous-menu Back");Click("Back");Check(Page()=="home"&&hud.ModalOpen&&course.Paused,"settings Back returns home paused");
   Scroll().verticalNormalizedPosition=.15f;Click("Realm9");Check(Page()=="levels"&&(int)typeof(RisingHud).GetField("_levelRealm",Flags).GetValue(hud)==9,"last realm opens its own ten-level picker");Scroll().verticalNormalizedPosition=.3f;
   hud.ShowSettings();Settled();bool sound=profile.Sound;Click("Music");Check(!profile.Music&&profile.Sound==sound&&!RisingSave.Load().Music,"music toggle persists independently of master sound");Click("Language");Check(profile.Language!=language,"actual language callback switches locale");
   safe.sizeDelta=pane.x>pane.y?new Vector2(375,627):new Vector2(734,343);Layout();typeof(RisingHud).GetMethod("Reopen",Flags).Invoke(hud,null);Settled();Check(Page()=="settings"&&course.Paused,"orientation refresh retains settings and pause");Click("Back");Check(Page()=="levels"&&course.Paused&&(int)typeof(RisingHud).GetField("_levelRealm",Flags).GetValue(hud)==9,"language and orientation retain exact selected realm parent");Check(Mathf.Abs(Scroll().verticalNormalizedPosition-.3f)<.002f,"realm picker retains scroll after refresh");Click("Back");Check(Page()=="home"&&course.Paused,"realm picker returns to collection");Check(Mathf.Abs(Scroll().verticalNormalizedPosition-.15f)<.002f,"collection retains scroll after returning from last realm");
   hud.ShowShop();hud.ShowStyle();hud.ShowSettings();Settled();typeof(RisingHud).GetMethod("Reopen",Flags).Invoke(hud,null);Settled();Click("Back");Check(Page()=="style"&&course.Paused,"refresh retains style parent");Click("Back");Check(Page()=="shop"&&course.Paused,"nested style returns shop");Click("Back");Check(Page()=="home"&&course.Paused,"shop returns home");
   hud.ShowHome();Settled();var staleRealmAction=Find("Realm9").onClick;hud.ShowLevels(9);Settled();var staleRouteAction=Find("Level3").onClick;hud.ShowSettings();Settled();
   var videos=root.AddComponent<RewardedVideos>();course.Videos=videos;var presenting=typeof(RewardedVideos).GetProperty("IsPresenting").GetSetMethod(true);hud.ShowSettings();Settled();presenting.Invoke(videos,new object[]{true});bool music=profile.Music;string runBefore=JsonUtility.ToJson(profile);staleRealmAction.Invoke();staleRouteAction.Invoke();Check(JsonUtility.ToJson(profile)==runBefore&&Page()=="settings"&&course.Paused,"stale realm and route callbacks cannot mutate a run while native provider presents");Click("Music");Click("Back");Click("Close");hud.ShowLevels(9);typeof(RisingHud).GetMethod("Reopen",Flags).Invoke(hud,null);Check(Page()=="settings"&&course.Paused&&profile.Music==music,"native provider keeps settings, pause and music preference locked");presenting.Invoke(videos,new object[]{false});Click("Close");Check(!hud.ModalOpen&&!course.Paused,"explicit Resume closes and resumes");Check(profile.Crystals==428&&profile.Realm==9,"navigation never changes wallet or selected campaign realm");
  }finally{UiMetrics.QaPointScale=null;if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);PlayerPrefs.DeleteKey(key);RisingSave.QaKey=previousKey;typeof(RisingSave).GetProperty("Writable").GetSetMethod(true).Invoke(null,new object[]{previousWritable});PlayerPrefs.Save();}
 }
}
#endif
