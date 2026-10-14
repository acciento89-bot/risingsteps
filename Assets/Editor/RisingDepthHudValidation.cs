#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.UI;
using Kamilunavo.RisingSteps.Visuals;
public static class RisingDepthHudValidation
{
 static readonly BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static void Check(bool value,string message){if(!value)throw new InvalidOperationException("Rising depth/HUD: "+message);}
 public static void Validate()
 {
  foreach(var pane in new[]{new Vector2(375,627),new Vector2(734,343),new Vector2(667,375),new Vector2(600,343),new Vector2(190,627)})foreach(string lang in new[]{"de","en"}){
   var root=new GameObject("DepthHudFixture");Canvas canvas=null;try{
    var course=root.AddComponent<RisingCourse>();typeof(RisingCourse).GetField("<Profile>k__BackingField",Private).SetValue(course,new RisingProfile{Language=lang,Realm=1,TutorialDone=true});course.Player=new GameObject("Player").transform;course.Player.SetParent(root.transform);
    var hud=root.AddComponent<RisingHud>();hud.Initialize(course);var safe=(RectTransform)typeof(RisingHud).GetField("_safe",Private).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=3;UiMetrics.QaPointScale=3;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=Vector2.one*.5f;safe.sizeDelta=pane;typeof(RisingHud).GetMethod("Layout",Private).Invoke(hud,null);hud.Close();Canvas.ForceUpdateCanvases();
    var bounds=UiMetrics.ScreenRect(safe);var groups=new List<RectTransform>();foreach(string name in new[]{"RealmBadge","Menu","View","Height","Crystals","RouteHint"})groups.Add((RectTransform)safe.Find(name));groups.Add((RectTransform)hud.Joystick.transform);groups.Add((RectTransform)hud.Jump.transform);
    for(int i=0;i<groups.Count;i++){var r=UiMetrics.ScreenRect(groups[i]);Check(r.xMin>=bounds.xMin-.5f&&r.xMax<=bounds.xMax+.5f&&r.yMin>=bounds.yMin-.5f&&r.yMax<=bounds.yMax+.5f,"gameplay display/control stays inside safe pane "+pane+lang+groups[i].name);for(int j=i+1;j<groups.Count;j++)Check(!r.Overlaps(UiMetrics.ScreenRect(groups[j])),"gameplay groups never overlap "+pane+groups[i].name+groups[j].name);}
    foreach(var control in new[]{safe.Find("Menu"),safe.Find("View"),hud.Joystick.transform,hud.Jump.transform}){var r=UiMetrics.ScreenRect((RectTransform)control);Check(r.width/3>=48&&r.height/3>=48,"all live controls retain48point targets");}
    var badge=safe.Find("RealmBadge");Check(!badge.GetComponent<Image>().raycastTarget&&badge.GetComponentInChildren<Text>().text==RisingRealmCatalog.Name(1,lang=="de"),"world name is genuine localized state without a dead touch surface");
    var height=safe.Find("Height");var mountain=height.Find("MountainIcon").GetComponentInChildren<HudIcons>();Check(mountain.Icon==HudIcons.Kind.Mountain&&!mountain.raycastTarget,"Concept mountain silhouette accompanies live height without intercepting camera input");var value=height.Find("Value").GetComponent<Text>();Check(UiMetrics.ScreenRect((RectTransform)mountain.transform).xMax<UiMetrics.ScreenRect((RectTransform)value.transform).xMin,"mountain and localized number have separate visible space");
    foreach(int step in new[]{0,4,12}){course.Profile.Step=step;hud.Refresh();Canvas.ForceUpdateCanvases();Check(value.text.StartsWith(lang=="de"?"HÖHE":"HEIGHT")&&value.text.Replace(" ","").EndsWith(step+"/12"),"real localized live height0/4/12");Check(Mathf.Abs(height.Find("Track/Fill").GetComponent<Image>().fillAmount-step/12f)<.0001f,"actual progress bar tracks earned height");
     var rect=value.rectTransform.rect;var settings=value.GetGenerationSettings(rect.size);using var generator=new TextGenerator();Check(generator.Populate(value.text,settings),"height text genuinely generates in "+pane+lang+step);// TextGenerator reports its chosen size in scaled pixels; settings expect canvas font units.
     int actualSize=value.resizeTextForBestFit?Mathf.RoundToInt(generator.fontSizeUsedForBestFit/settings.scaleFactor):value.fontSize;settings.fontSize=actualSize;settings.resizeTextForBestFit=false;settings.horizontalOverflow=HorizontalWrapMode.Overflow;settings.verticalOverflow=VerticalWrapMode.Overflow;float neededHeight=generator.GetPreferredHeight(value.text,settings)/value.pixelsPerUnit;Check(neededHeight<=rect.height+.1f,"complete localized height fits vertically "+pane+lang+step+" actualFont="+actualSize+" scale="+settings.scaleFactor+" pixelsPerUnit="+value.pixelsPerUnit+" required="+neededHeight+" available="+rect.height);foreach(string line in value.text.Split('\n'))Check(generator.GetPreferredWidth(line,settings)/value.pixelsPerUnit<=rect.width+.1f,"every complete height line fits horizontally "+pane+lang+step);
    }
   }finally{UiMetrics.QaPointScale=null;if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}
  }
  foreach(int realm in new[]{0,1,2}){var root=new GameObject("HighlandDepthFixture");try{string name=realm==0?"MeadowHighland":realm==1?"CascadeMesa":"TempleAcropolis";var stone=LandscapePalette.Surface(root.transform,realm);var sample=LandscapeGeometry.Rock(root.transform,name,Vector3.zero,new Vector3(15,17,14),stone,19).GetComponent<MeshFilter>().sharedMesh;Check(sample.vertexCount<=670,"dense standalone authored cliff retains the670vertex sculpt envelope");var levels=new HashSet<float>();foreach(var p in sample.vertices)levels.Add(p.y);Check(levels.Count>12,"distant landmass has eroded vertical bands beyond the former3ring shape");RealmArt.BuildBackdrop(root.transform,realm);if(realm==1)RisingWaterGarden.Build(root.transform);int masses=0;foreach(var filter in root.GetComponentsInChildren<MeshFilter>())if(filter.name is "CascadeMesa" or "TempleAcropolis" or "MeadowHighland" or "GardenCliff")masses++;Check(masses>=7,"each original realm retains at least seven actual eroded landmasses, including the new foreground garden");Check(root.GetComponentsInChildren<Collider>().Length==0,"deeper horizon remains render-only");}finally{UnityEngine.Object.DestroyImmediate(root);}}
  Debug.Log("RISING_DEPTH_HUD_PASS compact live indicators / nonoverlapping48point controls / authored eroded highlands / foreground garden landmasses");
 }
}
#endif
