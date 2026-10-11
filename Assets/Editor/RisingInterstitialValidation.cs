#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Monetization;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
public static class RisingInterstitialValidation
{
 const string RemoveAds="com.kamilunavo.risingsteps.removeads";
 static int checks;static List<string> failures;
 static void Check(bool ok,string label){checks++;if(!ok)failures.Add(label);}
 public static void Validate()
 {
  checks=0;failures=new();
  Check(CommerceRules.KnownProduct(RemoveAds),"permanent RemoveAds is a recognized nonconsumable entitlement");
  var p=new RisingProfile{Crystals=321,Style=1};p.Styles[1]=true;p.Normalize();
  CommerceRules.FulfillPending(p,RemoveAds,"remove1",_=>{});Check((p.Commerce.Entitlements&4)!=0&&p.Crystals==321&&p.Style==1,"RemoveAds grants bit4 without currency or style changes");
  CommerceRules.FulfillPending(p,RemoveAds,"remove1",_=>{});Check(p.Commerce.FulfilledTransactions.Count==1&&p.Crystals==321,"duplicate RemoveAds transaction is idempotent");
  var restore=new RisingProfile();CommerceRules.RestoreEntitlement(restore,RemoveAds);Check((restore.Commerce.Entitlements&4)!=0&&restore.Crystals==0,"restore grants permanent RemoveAds without starter currency");
  CommerceRules.ReconcileEntitlements(restore,new[]{RemoveAds});Check((restore.Commerce.Entitlements&4)!=0,"confirmed active catalog retains RemoveAds");CommerceRules.ReconcileEntitlements(restore,Array.Empty<string>());Check((restore.Commerce.Entitlements&4)==0,"confirmed revocation removes entitlement");
  var rollback=new RisingProfile();try{CommerceRules.FulfillPending(rollback,RemoveAds,"diskfail",_=>throw new Exception("disk"));}catch{}Check(rollback.Commerce.Entitlements==0&&rollback.Commerce.FulfilledTransactions.Count==0,"failed persistence rolls back RemoveAds transaction");
  var saved=RisingSave.Parse(JsonUtility.ToJson(p));Check((saved.Commerce.Entitlements&4)!=0&&saved.Crystals==321,"RemoveAds survives profile normalization and save round trip");
  var old=RisingSave.Parse("{\"Schema\":2,\"Crystals\":77,\"Commerce\":{\"Entitlements\":3,\"StarterCreditClaimed\":true},\"LevelStars\":[3]}");Check(old.Commerce.Entitlements==3&&old.Crystals==77,"legacy save never invents RemoveAds entitlement");
  var policyType=typeof(RisingProfile).Assembly.GetType("Kamilunavo.RisingSteps.Monetization.InterstitialPolicy");Check(policyType!=null,"session interstitial policy exists");
  if(policyType!=null)Policy(policyType);
  var adapterType=typeof(RisingProfile).Assembly.GetType("Kamilunavo.RisingSteps.Monetization.InterstitialAds");Check(adapterType!=null,"owned SDK interstitial adapter exists");
  Check(typeof(MonetizationConfig).GetProperty("InterstitialId")!=null,"interstitial configuration has separate production/test unit selection");
  Check(string.IsNullOrEmpty(new MonetizationConfig{InternalTestAds=false}.InterstitialId),"unconfigured production interstitial fails closed");
  var catalog=(List<ProductDefinition>)typeof(StorePurchases).GetMethod("Catalog",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);bool nonconsumable=false;foreach(var product in catalog)if(product.id==RemoveAds)nonconsumable=product.type==ProductType.NonConsumable;Check(nonconsumable,"real IAP catalog registers RemoveAds as nonconsumable");
  p.Commerce.Entitlements=4;Check(RewardRules.CanClaim(p,DateTime.UtcNow),"RemoveAds leaves voluntarily chosen bonus videos available");
  CommerceRules.ReconcileCatalog(p,Array.Empty<string>(),new[]{CommerceRules.Starter,CommerceRules.Collection});Check((p.Commerce.Entitlements&4)!=0,"partial old catalog cannot revoke missing RemoveAds SKU");CommerceRules.ReconcileCatalog(p,Array.Empty<string>(),new[]{RemoveAds});Check((p.Commerce.Entitlements&4)==0,"fetched RemoveAds SKU can reconcile confirmed revocation");
  foreach(var pane in new[]{new Vector2(375,627),new Vector2(734,343),new Vector2(190,627)})foreach(var language in new[]{"de","en"})try{Boundary(pane,language);}catch(Exception e){failures.Add(pane+language+": "+e.GetBaseException().Message);}
  try{RestoreException();}catch(Exception e){failures.Add("RESTORE_EXCEPTION: "+e.GetBaseException().Message);}
  if(failures.Count>0)throw new Exception("RISING_INTERSTITIAL_REGRESSION: "+string.Join("; ",failures));
  Debug.Log("RISING_INTERSTITIAL_PASS checks="+checks);
 }
 public static void ValidateRestoreException()
 {
  checks=0;failures=new();RestoreException();if(failures.Count>0)throw new Exception("RISING_RESTORE_EXCEPTION: "+string.Join("; ",failures));Debug.Log("RISING_RESTORE_EXCEPTION_PASS checks="+checks);
 }
 static void RestoreException()
 {
  var root=new GameObject("RestoreExceptionFixture");Canvas canvas=null;StorePurchases store=null;string previousKey=RisingSave.QaKey;const string key="kamilunavo.risingsteps.qa.restore.exception";string previous=PlayerPrefs.GetString(key,"");
  try{
   RisingSave.QaKey=key;RisingSave.Load();var game=root.AddComponent<RisingCourse>();var profile=new RisingProfile{TutorialDone=true,Crystals=73};typeof(RisingCourse).GetField("<Profile>k__BackingField",Private).SetValue(game,profile);game.Player=new GameObject("Player").transform;game.Player.SetParent(root.transform);var hud=root.AddComponent<RisingHud>();hud.Initialize(game);game.Hud=hud;canvas=((RectTransform)typeof(RisingHud).GetField("_safe",Private).GetValue(hud)).GetComponentInParent<Canvas>();store=root.AddComponent<StorePurchases>();game.Store=store;store.Initialize(game);hud.ShowShop();
   var controller=(StoreController)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(StoreController));var purchase=new ThrowingRestore();typeof(StoreController).GetField("m_PurchaseService",Private).SetValue(controller,purchase);typeof(StorePurchases).GetField("_store",Private).SetValue(store,controller);typeof(StorePurchases).GetField("<Ready>k__BackingField",Private).SetValue(store,true);string frozen=JsonUtility.ToJson(profile);int notifications=0;store.Changed+=()=>notifications++;
   // No native SDK: the actual StoreController delegates to a throwing IPurchaseService.
   RisingSave.QaKey=null;bool escaped=false;try{store.Restore();}catch(InvalidOperationException){escaped=true;}
   Check(purchase.Calls==1&&!escaped,"synchronous SDK restore exception is caught at the actual Restore entry");Check(!store.Busy&&!store.IsPresenting&&!game.NativePresentationActive,"restore exception releases provider and navigation locks");Check(store.Status==hud.T("Wiederherstellung nicht verfügbar","Restore unavailable")&&notifications>=2,"restore exception publishes a retryable status");Check(JsonUtility.ToJson(profile)==frozen&&hud.ShopOpen&&game.Paused,"failed restore preserves wallet/ownership and paused Shop");
   RisingSave.QaKey=key;store.Restore();Check(purchase.Calls==1,"QA restore cannot reach the injected SDK service");
   RisingSave.QaKey=null;purchase.Hold=true;store.Restore();Check(store.IsPresenting&&game.NativePresentationActive,"pending actual SDK restore retains native ownership");
   typeof(StorePurchases).GetField("<Busy>k__BackingField",Private).SetValue(store,false);typeof(StorePurchases).GetMethod("SetStatus",Private).Invoke(store,new object[]{"incidental purchase status"});Check(store.IsPresenting&&game.NativePresentationActive,"incidental status cannot dismiss still-pending restore");
   var confirmed=(ConfirmedOrder)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(ConfirmedOrder));typeof(StorePurchases).GetMethod("Confirmed",Private).Invoke(store,new object[]{confirmed});Check(store.IsPresenting&&game.NativePresentationActive,"incidental confirmed purchase cannot dismiss still-pending restore");
   purchase.Callback(true,null);Check(!store.IsPresenting&&!game.NativePresentationActive&&hud.ShopOpen,"own successful SDK callback releases native restore ownership");
   store.Restore();purchase.Callback(false,"cancel");Check(!store.Busy&&!store.IsPresenting&&!game.NativePresentationActive,"own cancellation callback releases busy/native restore ownership");Check(JsonUtility.ToJson(profile)==frozen,"overlapping restore callbacks preserve wallet and entitlements");
   store.Restore();purchase.Callback(true,null);Check(store.Busy&&!store.IsPresenting&&!game.NativePresentationActive,"successful callback releases native surface while purchase fetch may remain busy");
  }finally{if(store!=null)typeof(StorePurchases).GetField("_store",Private).SetValue(store,null);if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);if(string.IsNullOrEmpty(previous))PlayerPrefs.DeleteKey(key);else PlayerPrefs.SetString(key,previous);RisingSave.QaKey=previousKey;RisingSave.Load();}
 }
 #pragma warning disable CS0067
 sealed class ThrowingRestore:IPurchaseService
 {
  public int Calls;public bool Hold;public Action<bool,string> Callback;public IAppleStoreExtendedPurchaseService Apple=>null;public IGooglePlayStoreExtendedPurchaseService Google=>null;public IPaymentProvidersExtendedPurchaseService PaymentProviders=>null;
  public event Action<PendingOrder> OnPurchasePending;public event Action<Order> OnPurchaseConfirmed;public event Action<FailedOrder> OnPurchaseFailed;public event Action<DeferredOrder> OnPurchaseDeferred;public event Action<Orders> OnPurchasesFetched;public event Action<PurchasesFetchFailureDescription> OnPurchasesFetchFailed;public event Action<Entitlement> OnCheckEntitlement;
  public void RestoreTransactions(Action<bool,string> callback){Calls++;if(Hold){Callback=callback;return;}throw new InvalidOperationException("injected native restore failure");}
  public void PurchaseProduct(Product product){}public void PurchaseProduct(string id){}public void Purchase(ICart cart){}public void ConfirmPurchase(PendingOrder order){}public void FetchPurchases(){}public void CheckEntitlement(Product product){}public void ProcessPendingOrdersOnPurchasesFetched(bool process){}public System.Collections.ObjectModel.ReadOnlyObservableCollection<Order> GetPurchases()=>null;
 }
 #pragma warning restore CS0067
 static void Policy(Type type)
 {
  object policy=Activator.CreateInstance(type,new object[]{100d});
  var record=type.GetMethod("RecordCompletion");var eligible=type.GetMethod("Eligible");var shown=type.GetMethod("MarkShown");
  Check(record!=null&&eligible!=null&&shown!=null,"policy exposes actual completion, timing and shown transitions");if(record==null||eligible==null||shown==null)return;
  var p=new RisingProfile{TutorialDone=true,Step=12,Completed=true};
  string run="run0";bool Record(bool transition=true,bool tutorial=false)=>(bool)record.Invoke(policy,new object[]{p,run,transition,tutorial});
  bool Ready(double now,bool tutorial=false)=>(bool)eligible.Invoke(policy,new object[]{p,now,tutorial});
  Check(!Ready(1000),"elapsed time alone cannot trigger ad");Check(!Record(false),"result reopen is not a successful completion transition");Check(!Record(true,true),"tutorial completion never counts");
  p.Challenge=true;Check(!Record(),"daily challenge cannot farm forced ads");p.Challenge=false;
  Check(Record(),"first full campaign completion counts");Check(!Record(),"duplicate completion callback cannot farm count");run="run1";p.Level=1;Check(Record()&&!Ready(1000),"two full levels remain below threshold");run="run2";p.Level=2;Check(Record()&&!Ready(279.999)&&Ready(280),"third completed full run and exactly180seconds permit result-boundary opportunity");
  Check(!Ready(500,true),"active tutorial suppresses presentation");p.TutorialDone=false;Check(!Ready(500),"unfinished tutorial suppresses presentation");p.TutorialDone=true;
  p.Commerce.Entitlements=4;Check(!Ready(500),"RemoveAds blocks every forced interstitial");p.Commerce.Entitlements=0;
  p.Completed=false;Check(!Ready(500),"mid-course cannot present");p.Completed=true;
  shown.Invoke(policy,new object[]{280d});Check(!Ready(1000),"shown ad resets three-completion spacing");
  for(int level=3;level<6;level++){run="run"+level;p.Level=0;Check(Record(),"legitimate fully played replay counts once "+level);}Check(!Ready(459.999)&&Ready(460),"next ad also requires180seconds since last actual show");
  var voluntary=type.GetMethod("DelayAfterVoluntaryAd");Check(voluntary!=null,"voluntary fullscreen has a separate quota-preserving cooldown");if(voluntary!=null){voluntary.Invoke(policy,new object[]{500d});Check(!Ready(679.999)&&Ready(680),"voluntary opening delays180seconds while preserving earned three-run quota");voluntary.Invoke(policy,new object[]{499d});voluntary.Invoke(policy,new object[]{double.NaN});voluntary.Invoke(policy,new object[]{double.PositiveInfinity});Check(!Ready(679.999)&&Ready(680),"invalid or older voluntary timestamps cannot rewind cooldown");}
  run="run2";p.Level=2;Check(!Record(),"previously completed run token remains deduplicated after shown ad");
  run="restarted-but-unfinished";p.Step=1;p.Completed=false;Check(!Record(),"restart without a full completion never counts");
 }
 static readonly BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
 static void ConsentLoadIsolation(RisingCourse game)
 {
  var type=typeof(InterstitialAds);var reader=type.GetField("readConsent",Private);Check(reader!=null,"interstitial consent reader can prove SDK isolation at the actual Load entry");if(reader==null)return;
  var original=reader.GetValue(game.Interstitials);int calls=0;Func<bool> poison=()=>{calls++;throw new InvalidOperationException("SDK consent must stay isolated");};reader.SetValue(game.Interstitials,poison);
  try{
   type.GetMethod("Load",Private).Invoke(game.Interstitials,null);Check(calls==0,"actual desktop/QA Load never evaluates SDK consent");var prepare=type.GetMethod("TryPrepareBackgroundLoad",Private);Check(prepare!=null,"actual preload validates managed gates before reading consent");if(prepare==null)return;
   typeof(RewardedVideos).GetField("_initialized",Private).SetValue(game.Videos,true);Check(!(bool)prepare.Invoke(game.Interstitials,new object[]{false})&&calls==0,"network-disabled preload never evaluates poison consent");
   SetNative(game.Videos,true);Check(!(bool)prepare.Invoke(game.Interstitials,new object[]{true})&&calls==0,"privacy/video native ownership short-circuits SDK consent");SetNative(game.Videos,false);SetNative(game.Store,true);Check(!(bool)prepare.Invoke(game.Interstitials,new object[]{true})&&calls==0,"purchase native ownership short-circuits SDK consent");SetNative(game.Store,false);
   Check(!(bool)prepare.Invoke(game.Interstitials,new object[]{true})&&calls==1,"synchronous SDK consent exception fails closed");Check((double)type.GetField("retryAt",Private).GetValue(game.Interstitials)>=Time.realtimeSinceStartupAsDouble+59,"consent exception adds bounded60second retry backoff");Check(!(bool)prepare.Invoke(game.Interstitials,new object[]{true})&&calls==1,"backoff skips repeated SDK reads");type.GetField("retryAt",Private).SetValue(game.Interstitials,0d);reader.SetValue(game.Interstitials,(Func<bool>)(()=>{calls++;return true;}));Check((bool)prepare.Invoke(game.Interstitials,new object[]{true})&&calls==2,"available unlocked preload can read consent without showing an ad");
  }finally{reader.SetValue(game.Interstitials,original);type.GetField("retryAt",Private).SetValue(game.Interstitials,0d);typeof(RewardedVideos).GetField("_initialized",Private).SetValue(game.Videos,false);SetNative(game.Videos,false);SetNative(game.Store,false);}
 }
 static void SetNative(object owner,bool value)=>owner.GetType().GetField("<IsPresenting>k__BackingField",Private).SetValue(owner,value);
 static void Boundary(Vector2 pane,string language)
 {
  GameObject root=new GameObject("InterstitialBoundaryFixture");Canvas canvas=null;string previousKey=RisingSave.QaKey;const string key="kamilunavo.risingsteps.qa.interstitial.fixture";string previous=PlayerPrefs.GetString(key,"");
  try{
   RisingSave.QaKey=key;RisingSave.Load();var game=root.AddComponent<RisingCourse>();var profile=new RisingProfile{TutorialDone=true,Language=language,Crystals=432};typeof(RisingCourse).GetField("<Profile>k__BackingField",Private).SetValue(game,profile);game.Player=new GameObject("Player").transform;game.Player.SetParent(root.transform);
   var hud=root.AddComponent<RisingHud>();hud.Initialize(game);game.Hud=hud;var safe=(RectTransform)typeof(RisingHud).GetField("_safe",Private).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();canvas.GetComponent<CanvasScaler>().enabled=false;canvas.scaleFactor=3;UiMetrics.QaPointScale=3;safe.GetComponent<SafeAreaFitter>().enabled=false;safe.anchorMin=safe.anchorMax=Vector2.one*.5f;safe.sizeDelta=pane;typeof(RisingHud).GetMethod("Layout",Private).Invoke(hud,null);
   game.Store=root.AddComponent<StorePurchases>();game.Videos=root.AddComponent<RewardedVideos>();game.Store.Initialize(game);game.Videos.Initialize(game);game.Interstitials=root.AddComponent<InterstitialAds>();game.Interstitials.Initialize(game);hud.AttachCommerce();ConsentLoadIsolation(game);var loadGate=typeof(InterstitialAds).GetMethod("CanStartBackgroundLoad",Private);Check(loadGate!=null,"actual interstitial preload has a provider guard before cached creative access");if(loadGate!=null){SetNative(game.Videos,true);Check(!(bool)loadGate.Invoke(game.Interstitials,new object[]{true,true,true,Time.realtimeSinceStartupAsDouble}),"SDK-ready/focus callbacks cannot preload while privacy/video owns native surface");SetNative(game.Videos,false);SetNative(game.Store,true);Check(!(bool)loadGate.Invoke(game.Interstitials,new object[]{true,true,true,Time.realtimeSinceStartupAsDouble}),"cached interstitial load cannot bypass native purchase ownership");SetNative(game.Store,false);Check((bool)loadGate.Invoke(game.Interstitials,new object[]{true,true,true,Time.realtimeSinceStartupAsDouble}),"native closure permits preload without showing an ad");}Check(MonetizationConfig.QaNetworkDisabled&&!game.Store.CanRetry&&!game.Videos.CanWatch,"development QA disables store retry and ad network actions");hud.ShowHome();Canvas.ForceUpdateCanvases();var shop=safe.Find("ModalBackdrop/ModalCard/ShopEntry").GetComponent<Button>();var bounds=UiMetrics.ScreenRect((RectTransform)shop.transform);Check(bounds.width/3>=49&&bounds.height/3>=49,"fixed direct Shop entry has49point target "+pane+language);shop.onClick.Invoke();Canvas.ForceUpdateCanvases();Check(hud.ShopOpen,"direct fixed Shop entry opens actual Shop");var content=safe.Find("ModalBackdrop/ModalCard/Viewport/Content");
   foreach(string id in new[]{RemoveAds,CommerceRules.Starter,CommerceRules.Collection}){var product=content.Find("ProductCard"+id).Find(id).GetComponent<Button>();bounds=UiMetrics.ScreenRect((RectTransform)product.transform);Check(bounds.width/3>=49&&bounds.height/3>=49,"compact premium action meets49points "+id+pane+language);Check(!product.interactable&&!product.GetComponentInChildren<Text>().text.Contains("€"),"unavailable metadata disables purchase without fabricated price");}
   var first=content.Find("ProductCard"+RemoveAds);Check(first.GetSiblingIndex()<content.Find("Style0").GetSiblingIndex(),"RemoveAds and premium products precede earned style scroll");Check(content.Find("Restore")!=null,"Restore remains direct Shop action");
   profile.Step=12;profile.Completed=true;hud.ShowCompletion();var next=safe.Find("ModalBackdrop/ModalCard/Viewport/Content/NextLevel").GetComponent<Button>().onClick;var policy=new InterstitialPolicy(Time.realtimeSinceStartupAsDouble-181);typeof(InterstitialAds).GetField("policy",Private).SetValue(game.Interstitials,policy);for(int i=0;i<3;i++)policy.RecordCompletion(profile,"fixture-run"+i,true,false);
   var present=typeof(InterstitialAds).GetMethod("PresentAtResult",Private);int shows=0;Action show=()=>shows++;
   Check(!(bool)present.Invoke(game.Interstitials,new object[]{false,show})&&shows==0&&!game.Interstitials.IsPresenting,"no-fill skips immediately without opening native presenter");
   typeof(InterstitialAds).GetMethod("OnApplicationFocus",Private).Invoke(game.Interstitials,new object[]{false});Check(!(bool)present.Invoke(game.Interstitials,new object[]{true,show})&&shows==0,"background result cannot present");typeof(InterstitialAds).GetMethod("OnApplicationFocus",Private).Invoke(game.Interstitials,new object[]{true});Check(shows==0,"foreground return never queues a surprise show");
   profile.Commerce.Entitlements=4;Check(!(bool)present.Invoke(game.Interstitials,new object[]{true,show})&&shows==0,"RemoveAds blocks actual presenter boundary");profile.Commerce.Entitlements=0;
   Check((bool)present.Invoke(game.Interstitials,new object[]{true,show})&&shows==1&&game.NativePresentationActive&&game.Paused,"ready earned boundary opens one paused native presenter");string frozen=JsonUtility.ToJson(profile);next.Invoke();shop.onClick.Invoke();hud.Back();hud.Close();hud.ShowSettings();game.StartRun(0,false);game.StartLevel(0,0);Check(JsonUtility.ToJson(profile)==frozen&&hud.ResultOpen&&game.Paused,"actual stale callbacks and direct run APIs cannot mutate run or dismiss paused result during ad");
   typeof(InterstitialAds).GetMethod("FinishPresentation",Private).Invoke(game.Interstitials,null);Check(!game.NativePresentationActive&&hud.ResultOpen&&game.Paused,"native close returns to the same paused result without forced resume");
   policy.MarkShown(Time.realtimeSinceStartupAsDouble);hud.ShowCompletion();game.CompletePortal();Check(!policy.Eligible(profile,Time.realtimeSinceStartupAsDouble+500,false)&&profile.Crystals==432,"result reopen and duplicate portal completion count no run or reward");
   Check(!(bool)present.Invoke(game.Interstitials,new object[]{true,show})&&shows==1,"consumed completion spacing cannot present again");
   var transitionPolicy=new InterstitialPolicy(Time.realtimeSinceStartupAsDouble-181);typeof(InterstitialAds).GetField("policy",Private).SetValue(game.Interstitials,transitionPolicy);transitionPolicy.RecordCompletion(profile,"prior1",true,false);transitionPolicy.RecordCompletion(profile,"prior2",true,false);profile.Completed=false;Check(!transitionPolicy.Eligible(profile,Time.realtimeSinceStartupAsDouble,false),"two earned transitions are insufficient");game.CompletePortal();Check(transitionPolicy.Eligible(profile,Time.realtimeSinceStartupAsDouble,false),"actual successful Course completion delivers third run transition");var rewardedOpened=(Action)typeof(RewardedVideos).GetField("AdOpened",Private).GetValue(game.Videos);rewardedOpened.Invoke();Check(!transitionPolicy.Eligible(profile,Time.realtimeSinceStartupAsDouble+179,false)&&transitionPolicy.Eligible(profile,Time.realtimeSinceStartupAsDouble+181,false),"actual rewarded-open signal delays180seconds and preserves completed-run quota without replaying another three levels");
  }finally{UiMetrics.QaPointScale=null;if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);if(string.IsNullOrEmpty(previous))PlayerPrefs.DeleteKey(key);else PlayerPrefs.SetString(key,previous);RisingSave.QaKey=previousKey;RisingSave.Load();}
 }
}
#endif
