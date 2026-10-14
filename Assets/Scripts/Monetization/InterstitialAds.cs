using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
namespace Kamilunavo.RisingSteps.Monetization
{
 public sealed class InterstitialAds:MonoBehaviour
 {
  RisingCourse game;MonetizationConfig config;InterstitialPolicy policy;InterstitialAd ad;
  readonly AdLoadGeneration loads=new();bool loading,focused=true,applicationPaused;double loadedAt,retryAt;
  Func<bool> readConsent=()=>ConsentInformation.CanRequestAds();
  public bool IsPresenting{get;private set;}
  public event Action Changed;
  bool Foreground=>focused&&!applicationPaused&&isActiveAndEnabled;
  bool Removed=>game?.Profile?.Commerce!=null&&(game.Profile.Commerce.Entitlements&4)!=0;
  bool NetworkAllowed
  {
   get{
    if(!Application.isMobilePlatform||MonetizationConfig.QaNetworkDisabled||game==null||Removed||!RisingSave.Writable)return false;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    if(!string.IsNullOrEmpty(RisingSave.QaKey))return false;
#endif
    return !string.IsNullOrWhiteSpace(config?.InterstitialId);
   }
  }
  public void Initialize(RisingCourse course)
  {
   game=course;config=MonetizationConfig.Load();policy=new InterstitialPolicy(Time.realtimeSinceStartupAsDouble);
   game.Videos.SdkReady+=Load;game.Videos.ConsentReset+=InvalidateBackgroundLoad;game.Videos.AdOpened+=RewardedOpened;game.Changed+=ProfileChanged;if(game.Videos.SdkInitialized)Load();
  }
  void Main(Action action,Action abandoned=null)=>MobileAdsEventExecutor.ExecuteInUpdate(()=>{if(this!=null)action();else abandoned?.Invoke();});
  void RewardedOpened()=>policy.DelayAfterVoluntaryAd(Time.realtimeSinceStartupAsDouble);
  void ProfileChanged(){if(Removed&&!IsPresenting){loads.Invalidate();loading=false;DestroyAd();}}
  // Shared by the actual cache/load entry and editor provider-overlap checks.
  internal bool CanStartBackgroundLoad(bool networkAllowed,bool sdkReady,bool consentAllowed,double now)
  {
   return networkAllowed&&Foreground&&game!=null&&!game.NativePresentationActive&&!IsPresenting&&!loading&&sdkReady&&consentAllowed&&now>=retryAt;
  }
  internal bool TryPrepareBackgroundLoad(bool networkAllowed)
  {
   if(!CanStartBackgroundLoad(networkAllowed,game?.Videos?.SdkInitialized==true,true,Time.realtimeSinceStartupAsDouble))return false;
   return ReadConsentSafely();
  }
  bool ReadConsentSafely()
  {
   try{return readConsent!=null&&readConsent();}catch{retryAt=Time.realtimeSinceStartupAsDouble+60;return false;}
  }
  void Load()
  {
   if(!TryPrepareBackgroundLoad(NetworkAllowed))return;
   if(ad!=null&&ad.CanShowAd()&&Time.realtimeSinceStartupAsDouble-loadedAt<3500)return;
   DestroyAd();loading=true;long generation=loads.Begin();
   try{InterstitialAd.Load(config.InterstitialId,MonetizationConfig.NonPersonalizedRequest(),(creative,error)=>{
    if(this==null){creative?.Destroy();return;}
    Main(()=>{
     if(!loads.IsCurrent(generation)||!Foreground||!NetworkAllowed||game.NativePresentationActive||!ReadConsentSafely()){creative?.Destroy();if(loads.IsCurrent(generation))loading=false;return;}
     loading=false;if(error!=null||creative==null){creative?.Destroy();retryAt=Time.realtimeSinceStartupAsDouble+60;return;}
     ad=creative;loadedAt=Time.realtimeSinceStartupAsDouble;
     creative.OnAdFullScreenContentOpened+=()=>Main(()=>{if(ad==creative&&IsPresenting)policy.MarkShown(Time.realtimeSinceStartupAsDouble);});
     creative.OnAdFullScreenContentClosed+=()=>Main(()=>{if(ad==creative)FinishPresentation();});
     creative.OnAdFullScreenContentFailed+=failure=>Main(()=>{if(ad==creative)FinishPresentation();});
    },()=>creative?.Destroy());
   });}catch{loads.Invalidate();loading=false;retryAt=Time.realtimeSinceStartupAsDouble+60;}
  }
  public void CompletedRun(string runToken)
  {
   if(policy==null||game.Hud==null||!policy.RecordCompletion(game.Profile,runToken,true,game.Hud.TutorialActive))return;
   TryShowAtResult();if(!IsPresenting)game.Videos.Prepare();
  }
  void TryShowAtResult()
  {
   bool ready=NetworkAllowed&&ReadConsentSafely()&&ad!=null&&ad.CanShowAd()&&Time.realtimeSinceStartupAsDouble-loadedAt<3500;
   var creative=ad;
   if(!PresentAtResult(ready,()=>creative.Show()))Load();
  }
  // The SDK calls Show only here, synchronously at this earned result boundary.
  // A missed opportunity is never queued, including consent/load/focus callbacks.
  internal bool PresentAtResult(bool ready,Action show)
  {
   if(!ready||!Foreground||game==null||game.Hud==null||!game.Hud.ResultOpen||!game.Paused||game.NativePresentationActive||!RisingSave.Writable||!policy.Eligible(game.Profile,Time.realtimeSinceStartupAsDouble,game.Hud.TutorialActive))return false;
   game.Save();IsPresenting=true;game.Paused=true;game.RefreshProfile();Changed?.Invoke();
   try{show();return true;}catch{FinishPresentation();return false;}
  }
  void FinishPresentation()
  {
   IsPresenting=false;DestroyAd();retryAt=Time.realtimeSinceStartupAsDouble+2;game?.RefreshProfile();Changed?.Invoke();Load();
   // Result page remains paused. Only the player's next/resume action starts play.
  }
  void DestroyAd(){ad?.Destroy();ad=null;}
  void InvalidateBackgroundLoad(){loads.Invalidate();loading=false;if(!IsPresenting)DestroyAd();}
  void OnApplicationFocus(bool value){focused=value;if(!value)InvalidateBackgroundLoad();else Load();}
  void OnApplicationPause(bool value){applicationPaused=value;if(value)InvalidateBackgroundLoad();else Load();}
  void OnDisable(){InvalidateBackgroundLoad();}
  void OnDestroy(){loads.Invalidate();DestroyAd();IsPresenting=false;if(game!=null){game.Changed-=ProfileChanged;if(game.Videos!=null){game.Videos.SdkReady-=Load;game.Videos.ConsentReset-=InvalidateBackgroundLoad;game.Videos.AdOpened-=RewardedOpened;}}}
 }
}
