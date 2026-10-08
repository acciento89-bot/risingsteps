#if UNITY_EDITOR
using System;
using UnityEngine;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
public static class RisingValidation
{
 public static void ValidateAll(){Validate();ValidateUI();ValidateInput();ValidateGeometry();}
 static int checks;
 static void Check(bool ok,string name){checks++;if(!ok)throw new Exception(name);}
 public static void ValidateInput(){
 var canvas=Kamilunavo.RisingSteps.UI.UiFactory.Canvas();var j=Kamilunavo.RisingSteps.Input.VirtualJoystick.Create(canvas.transform,Vector2.zero,Vector2.one);
 var r=(RectTransform)j.transform;r.anchorMin=r.anchorMax=Vector2.zero;r.pivot=Vector2.zero;r.sizeDelta=new Vector2(112,112);Canvas.ForceUpdateCanvases();
 var pointer=new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current){pointerId=100,position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center))};j.SendMessage("Awake");j.OnPointerDown(pointer);Check(j.Value.sqrMagnitude<.001f,"joystick center is neutral with bottom-left pivot");
 pointer.position=RectTransformUtility.WorldToScreenPoint(null,r.TransformPoint(r.rect.center+Vector2.left*50));j.OnDrag(pointer);Check(j.Value.x<-.8f&&Mathf.Abs(j.Value.y)<.01f,"joystick can move left");j.ResetInput();UnityEngine.Object.DestroyImmediate(canvas.gameObject);Debug.Log("RISING_INPUT_PASS checks="+checks);
 }
 public static void ValidateGeometry(){
 var root=new GameObject("GeometryProbe");Kamilunavo.RisingSteps.Visuals.IslandArt.Build(root.transform,0,0);
 foreach(var filter in root.GetComponentsInChildren<MeshFilter>())foreach(var normal in filter.sharedMesh.normals)Check(normal.sqrMagnitude>.5f,"valid lighting normal "+filter.name);
 UnityEngine.Object.DestroyImmediate(root);Debug.Log("RISING_GEOMETRY_PASS checks="+checks);
 }
 public static void ValidateUI(){
 var canvas=Kamilunavo.RisingSteps.UI.UiFactory.Canvas();var root=Kamilunavo.RisingSteps.UI.UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);
 Check(!root.GetComponent<UnityEngine.UI.Image>().raycastTarget,"transparent root camera touch");
 var label=Kamilunavo.RisingSteps.UI.UiFactory.Label(root,"Label","Text",30,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white);
 Check(!label.raycastTarget,"label does not intercept touch");UnityEngine.Object.DestroyImmediate(canvas.gameObject);Debug.Log("RISING_UI_PASS checks="+checks);
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
 Check(restored.Step==12&&restored.Crystals==0&&restored.Styles.Length==4&&restored.Stars.Length==3,"normalization");
 for(int realm=0;realm<3;realm++)for(int seed=0;seed<30;seed++){
 var a=CoursePatterns.Points(realm,seed);var b=CoursePatterns.Points(realm,seed);Check(a.Length==13,"13 islands");
 for(int i=1;i<a.Length;i++){Check(a[i]==b[i],"determinism");var d=a[i]-a[i-1];
 float flight=(9.2f+Mathf.Sqrt(9.2f*9.2f-44*d.y))/22;
 Check(d.y>0&&d.y<1.3f&&new Vector2(d.x,d.z).magnitude-2.2f<6.2f*flight-.35f,"reachable edge gap");}}
 Debug.Log("RISING_VALIDATION_PASS checks="+checks);
 }
}
#endif
