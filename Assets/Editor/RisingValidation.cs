#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
public static class RisingValidation
{
 public static void ValidateAll(){
 var loggedErrors=new System.Collections.Generic.List<string>();Application.LogCallback capture=(message,stack,type)=>{if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)loggedErrors.Add(message);};Application.logMessageReceived+=capture;
 try{Validate();ValidateUI();ValidateInput();ValidateGeometry();ValidateLifetime();ValidateReviewRegressions();RisingTouchTutorialValidation.Validate();Canvas.ForceUpdateCanvases();}
 finally{Application.logMessageReceived-=capture;}
 if(loggedErrors.Count>0)throw new InvalidOperationException("Validation logged unexpected errors: "+string.Join("; ",loggedErrors));
 }
 static int checks;
 static void Check(bool ok,string name){checks++;if(!ok)throw new Exception(name);}
 public static void ValidateInput(){
 var canvas=Kamilunavo.RisingSteps.UI.UiFactory.Canvas();var j=Kamilunavo.RisingSteps.Input.VirtualJoystick.Create(canvas.transform,Vector2.zero,Vector2.one);
 var r=(RectTransform)j.transform;r.anchorMin=r.anchorMax=Vector2.zero;r.pivot=Vector2.zero;r.sizeDelta=new Vector2(112,112);Canvas.ForceUpdateCanvases();
 var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){pointerId=100,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};typeof(Kamilunavo.RisingSteps.Input.VirtualJoystick).GetMethod("Awake",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(j,null);j.OnPointerDown(pointer);Check(j.Value.sqrMagnitude<.001f,"joystick center is neutral with bottom-left pivot");
 pointer.position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center+Vector2.left*50));j.OnDrag(pointer);Check(j.Value.x<-.8f&&Mathf.Abs(j.Value.y)<.01f,"joystick can move left");j.ResetInput();UnityEngine.Object.DestroyImmediate(canvas.gameObject);Debug.Log("RISING_INPUT_PASS checks="+checks);
 }
 public static void ValidateReviewRegressions(){var errors=new System.Collections.Generic.List<string>();try{ValidateScroll();}catch(Exception e){errors.Add(e.Message);}try{ValidateCompact();}catch(Exception e){errors.Add(e.Message);}try{ValidateGallery();}catch(Exception e){errors.Add(e.Message);}try{ValidateTutorialControls();}catch(Exception e){errors.Add(e.Message);}if(errors.Count>0)throw new Exception(string.Join("; ",errors));Debug.Log("RISING_REVIEW_REGRESSIONS_PASS checks="+checks);}
 static Kamilunavo.RisingSteps.UI.RisingHud Fixture(out GameObject root,out Canvas canvas){root=new GameObject("HudProbe");var course=root.AddComponent<RisingCourse>();typeof(RisingCourse).GetField("<Profile>k__BackingField",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(course,new RisingProfile());course.Player=new GameObject("ProbePlayer").transform;course.Player.SetParent(root.transform);var hud=root.AddComponent<Kamilunavo.RisingSteps.UI.RisingHud>();hud.Initialize(course);var safe=(RectTransform)typeof(Kamilunavo.RisingSteps.UI.RisingHud).GetField("_safe",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();var modal=typeof(Kamilunavo.RisingSteps.UI.RisingHud).GetField("_modal",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic);UnityEngine.Object.DestroyImmediate(((RectTransform)modal.GetValue(hud)).gameObject);modal.SetValue(hud,null);return hud;}
 public static void ValidateScroll(){var hud=Fixture(out var root,out var canvas);try{hud.ShowAchievements();var viewport=canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Viewport");Check(viewport.GetComponent<UnityEngine.UI.Image>().raycastTarget,"achievement descriptions and gaps accept scrolling");Check(!canvas.transform.Find("SafeArea").GetComponent<UnityEngine.UI.Image>().raycastTarget,"gameplay root still accepts camera touches");}finally{UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateCompact(){var hud=Fixture(out var root,out var canvas);try{canvas.GetComponent<UnityEngine.UI.CanvasScaler>().enabled=false;canvas.scaleFactor=3*Mathf.Sqrt(375f/390f*667f/844f);Kamilunavo.RisingSteps.UI.UiMetrics.QaPointScale=3;typeof(Kamilunavo.RisingSteps.UI.RisingHud).GetMethod("Layout",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(hud,null);hud.ShowAchievements();
 var menu=(RectTransform)canvas.transform.Find("SafeArea/Menu");var close=(RectTransform)canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Close");Check(menu.rect.width*canvas.scaleFactor/3>=47.999f&&menu.rect.height*canvas.scaleFactor/3>=47.999f,"compact375x667 menu >=48 logical points");Check(close.rect.height*canvas.scaleFactor/3>=47.999f,"compact375x667 close >=48 logical points");}finally{Kamilunavo.RisingSteps.UI.UiMetrics.QaPointScale=null;UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateGallery(){var hud=Fixture(out var root,out var canvas);try{hud.ShowHome();Canvas.ForceUpdateCanvases();var content=canvas.transform.Find("SafeArea/ModalBackdrop/ModalCard/Viewport/Content");var hero=content.Find("SkyAtlas");var image=(RectTransform)hero.Find("RealmArt");foreach(var name in new[]{"Eyebrow","Story","Checkpoint"}){var text=(RectTransform)hero.Find(name);Check(text.anchorMax.x<image.anchorMin.x,"hero "+name+" remains outside scenic image bounds");}
 var meadow=content.Find("Realm0/RealmArt").GetComponent<Kamilunavo.RisingSteps.UI.RealmIllustration>();var waterfall=content.Find("Realm1/RealmArt").GetComponent<Kamilunavo.RisingSteps.UI.RealmIllustration>();var temple=content.Find("Realm2/RealmArt").GetComponent<Kamilunavo.RisingSteps.UI.RealmIllustration>();Check(meadow.texture!=null&&waterfall.texture!=null&&temple.texture!=null,"gallery scenic textures load from Resources");Check(meadow.uvRect!=waterfall.uvRect&&temple.texture!=meadow.texture,"chapter art shows distinct meadow waterfall and temple subjects");Check(hero.GetComponent<Kamilunavo.RisingSteps.UI.RoundedPanel>().material.shader.name=="Rising/Gallery","gallery uses subtle10unit card corners instead of gold pill frame");}finally{UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateTutorialControls(){var hud=Fixture(out var root,out var canvas);try{var safe=(RectTransform)canvas.transform.Find("SafeArea");safe.GetComponent<Kamilunavo.RisingSteps.UI.SafeAreaFitter>().enabled=false;
 foreach(var size in new[]{new Vector2(390,844),new Vector2(375,667),new Vector2(844,390),new Vector2(196,760)}){safe.anchorMin=safe.anchorMax=new Vector2(.5f,.5f);safe.sizeDelta=size;Canvas.ForceUpdateCanvases();typeof(Kamilunavo.RisingSteps.UI.RisingHud).GetMethod("Layout",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(hud,null);hud.ReplayTutorial();Canvas.ForceUpdateCanvases();var tray=Kamilunavo.RisingSteps.UI.UiMetrics.ScreenRect((RectTransform)safe.Find("TutorialTray"));Check(!tray.Overlaps(Kamilunavo.RisingSteps.UI.UiMetrics.ScreenRect((RectTransform)hud.Joystick.transform)),"tutorial does not cover joystick at "+size);Check(!tray.Overlaps(Kamilunavo.RisingSteps.UI.UiMetrics.ScreenRect((RectTransform)hud.Jump.transform)),"tutorial does not cover jump at "+size);
 var joystick=(RectTransform)hud.Joystick.transform;var edge=joystick.rect.center+Vector2.up*joystick.rect.height*.5f;Check(edge.y==joystick.rect.yMax,"old camera fixture began exactly on excluded top boundary");var inside=joystick.rect.center+(edge-joystick.rect.center)*.75f;Check(joystick.rect.Contains(inside),"camera fixture begins inside before owner drags to full radius");}
 }finally{UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}}
 public static void ValidateLifetime(){
 var root=new GameObject("LifetimeProbe");Kamilunavo.RisingSteps.Visuals.IslandArt.Build(root.transform,0,0);
 var originals=new System.Collections.Generic.HashSet<Mesh>();foreach(var f in root.GetComponentsInChildren<MeshFilter>())originals.Add(f.sharedMesh);
 Kamilunavo.RisingSteps.Visuals.MeshArt.Batch(root);var combined=new System.Collections.Generic.HashSet<Mesh>();foreach(var f in root.GetComponentsInChildren<MeshFilter>())combined.Add(f.sharedMesh);
 UnityEngine.Object.DestroyImmediate(root);
 foreach(var mesh in originals)Check(mesh==null,"rebuilt island releases original mesh");foreach(var mesh in combined)Check(mesh==null,"rebuilt island releases batched mesh");Debug.Log("RISING_LIFETIME_PASS checks="+checks);
 }
 public static void ValidateGeometry(){
 var root=new GameObject("GeometryProbe");Kamilunavo.RisingSteps.Visuals.IslandArt.Build(root.transform,0,0);
 foreach(var filter in root.GetComponentsInChildren<MeshFilter>())foreach(var normal in filter.sharedMesh.normals)Check(normal.sqrMagnitude>.5f,"valid lighting normal "+filter.name);
 UnityEngine.Object.DestroyImmediate(root);Debug.Log("RISING_GEOMETRY_PASS checks="+checks);
 }
 public static void ValidateUI(){
 var safe=new Rect(0,0,1,1);var division=new Rect(.48f,0,.04f,1);var pane=Kamilunavo.RisingSteps.UI.UsableRegion.Choose(safe,division);Check(pane.xMax<=division.xMin+.001f||pane.xMin>=division.xMax-.001f,"either maximum-area pane excludes division despite float tie");
 var canvas=Kamilunavo.RisingSteps.UI.UiFactory.Canvas();var root=Kamilunavo.RisingSteps.UI.UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);
 Check(!root.GetComponent<UnityEngine.UI.Image>().raycastTarget,"transparent root camera touch");
 var label=Kamilunavo.RisingSteps.UI.UiFactory.Label(root,"Label","Text",30,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white);
 Check(!label.raycastTarget,"label does not intercept touch");
 var art=Kamilunavo.RisingSteps.UI.RealmIllustration.Add(root,0,Vector2.zero,Vector2.one);Check(art.GetComponent<CanvasRenderer>()!=null,"illustrated realm graphic owns CanvasRenderer");Kamilunavo.RisingSteps.UI.HudIcons.Add(root,Kamilunavo.RisingSteps.UI.HudIcons.Kind.Menu);Check(root.GetComponentInChildren<Kamilunavo.RisingSteps.UI.HudIcons>().GetComponent<CanvasRenderer>()!=null,"HUD icon graphic owns CanvasRenderer");UnityEngine.Object.DestroyImmediate(canvas.gameObject);Debug.Log("RISING_UI_PASS checks="+checks);
 }
 public static void Validate(){
 var p=new RisingProfile(); var day=new DateTime(2026,10,8,0,0,0,DateTimeKind.Utc);
 Check(RisingRules.ClaimDaily(p,day)&&p.Crystals==100,"first daily");
 Check(!RisingRules.ClaimDaily(p,day)&&!RisingRules.ClaimDaily(p,day.AddDays(-1)),"duplicate/earlier daily");
 for(int i=1;i<9;i++)RisingRules.ClaimDaily(p,day.AddDays(i));
 Check(p.DailyStreak==7&&p.Crystals==1230,"streak cap");
 p=new RisingProfile();Check(!RisingRules.Land(p,2,true),"skip blocked");
 for(int i=1;i<=12;i++){Check(RisingRules.Land(p,i,true),"ordered land");Check(!RisingRules.Land(p,i,true),"duplicate land");}
 Check(p.Crystals==336,"perfect rewards");p.Elapsed=100;
 Check(RisingRules.Complete(p,day)==3&&p.UnlockedRealm==1,"stars/unlock");
 var challenge=new RisingProfile{Challenge=true,RunDay=RisingRules.Day(day),Step=12,Perfects=8};
 RisingRules.Complete(challenge,day);Check(challenge.Crystals==75,"challenge bonus");RisingRules.Complete(challenge,day);Check(challenge.Crystals==75,"challenge duplicate");
 challenge=new RisingProfile{Challenge=true,RunDay=RisingRules.Day(day.AddDays(-1)),Step=12,Perfects=12};RisingRules.Complete(challenge,day);Check(challenge.Crystals==0,"old challenge no bonus");
 int coins=p.Crystals;RisingRules.Complete(p,day);Check(p.Crystals==coins,"completion idempotence");
 Check(RisingRules.SelectStyle(p,1)&&p.Crystals==coins-75,"style purchase");
 Check(RisingRules.SelectStyle(p,1)&&p.Crystals==coins-75,"owned style no charge");
 var restored=RisingSave.Parse(JsonUtility.ToJson(p));Check(restored.Step==12&&restored.Styles[1]&&restored.Stars[0]==3,"roundtrip");
 Check(RisingSave.Parse("garbage").Step==0,"corrupt fallback");
 restored=RisingSave.Parse("{\"Schema\":1,\"Step\":90,\"Crystals\":-3,\"Styles\":[],\"Stars\":[]}");
 Check(restored.Step==12&&restored.Crystals==0&&restored.Styles.Length==8&&restored.Stars.Length==3,"normalization");
 for(int realm=0;realm<3;realm++)for(int seed=0;seed<30;seed++){
 var a=CoursePatterns.Points(realm,seed);var b=CoursePatterns.Points(realm,seed);Check(a.Length==13,"13 islands");
 for(int i=1;i<a.Length;i++){Check(a[i]==b[i],"determinism");var d=a[i]-a[i-1];
 float flight=(9.2f+Mathf.Sqrt(9.2f*9.2f-44*d.y))/22;
 Check(d.y>0&&d.y<1.3f&&new Vector2(d.x,d.z).magnitude-2.2f<6.2f*flight-.35f,"reachable edge gap");}}
 Debug.Log("RISING_VALIDATION_PASS checks="+checks);
 }
}
#endif
