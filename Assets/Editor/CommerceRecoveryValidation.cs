#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.UI;
using Kamilunavo.RisingSteps.Monetization;
namespace Kamilunavo.RisingSteps.Editor
{
 public static class CommerceRecoveryValidation
 {
  static int checks;
  static void Check(bool ok,string message){checks++;if(!ok)throw new InvalidOperationException(message);}
  public static void Validate()
  {
   checks=0;var failures=new List<string>();
   try{Consent();}catch(Exception e){failures.Add("CONSENT: "+e.Message);}
   try{Reconnect();}catch(Exception e){failures.Add("STORE: "+e.Message);}
   if(failures.Count>0)throw new InvalidOperationException(string.Join("; ",failures));
   CommerceValidation.Validate();Debug.Log("RISING_COMMERCE_RECOVERY_PASS checks="+checks);
  }
  static void Reconnect()
  {
   var type=typeof(StorePurchases).Assembly.GetType("Kamilunavo.RisingSteps.Monetization.StoreReconnectGate");Check(type!=null,"missing same-session reconnect gate");
   var gate=Activator.CreateInstance(type);var begin=type.GetMethod("TryBegin");var end=type.GetMethod("EndAttempt");var fail=type.GetMethod("Failed");
   Check((bool)begin.Invoke(gate,new object[]{0d}),"initial connection");Check(!(bool)begin.Invoke(gate,new object[]{1d}),"reject duplicate pending connect");
   fail.Invoke(gate,new object[]{1d});Check(!(bool)begin.Invoke(gate,new object[]{20d}),"failure callback cannot overlap outstanding await");end.Invoke(gate,null);
   Check(!(bool)begin.Invoke(gate,new object[]{5d}),"failure cooldown");Check((bool)begin.Invoke(gate,new object[]{6d}),"retry after bounded cooldown");end.Invoke(gate,null);
  }
  static void Set(object target,string property,object value)=>target.GetType().GetField("<"+property+">k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(target,value);
  static void Consent()
  {
   var method=typeof(RewardedVideos).GetMethod("TryPresentConsent",BindingFlags.Instance|BindingFlags.NonPublic);Check(method!=null,"missing active-shop consent gate");
   GameObject root=null;Canvas canvas=null;var previous=RisingSave.QaKey;RisingSave.QaKey="kamilunavo.risingsteps.qa.commerce-recovery";
   try
   {
    var fixture=typeof(global::RisingValidation).GetMethod("Fixture",BindingFlags.Static|BindingFlags.NonPublic);object[] args={null,null};var hud=(RisingHud)fixture.Invoke(null,args);root=(GameObject)args[0];canvas=(Canvas)args[1];var game=hud.Course;game.Hud=hud;
    var store=root.AddComponent<StorePurchases>();game.Store=store;store.Initialize(game);
    var videos=root.AddComponent<RewardedVideos>();game.Videos=videos;videos.Initialize(game);int shown=0;
    bool opened=(bool)method.Invoke(videos,new object[]{(Action)(()=>shown++)});Check(!opened&&shown==0&&!videos.IsPresenting,"late consent after leaving shop never presents");
    hud.ShowShop();opened=(bool)method.Invoke(videos,new object[]{(Action)(()=>shown++)});Check(opened&&shown==1&&videos.IsPresenting&&hud.ModalOpen&&game.Paused,"shop consent pauses gameplay");
    hud.Close();Check(hud.ModalOpen,"native consent remains paused after shop closes");
    typeof(RewardedVideos).GetProperty("IsPresenting").GetSetMethod(true).Invoke(videos,new object[]{false});
    if(!hud.ShopOpen)hud.ShowShop();opened=(bool)method.Invoke(videos,new object[]{(Action)(()=>throw new InvalidOperationException("presenter unavailable"))});Check(!opened&&!videos.IsPresenting,"failed presenter unlocks UI");
   }
   finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);RisingSave.QaKey=previous;}
  }
 }
}
#endif
