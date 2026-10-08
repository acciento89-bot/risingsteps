using System;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.Input;
using Kamilunavo.RisingSteps.Visuals;
namespace Kamilunavo.RisingSteps.UI
{
 public sealed class RisingHud:MonoBehaviour
 {
  public RisingCourse Course{get;private set;}public VirtualJoystick Joystick{get;private set;}public PressButton Jump{get;private set;}public bool ModalOpen=>_modal!=null;public bool ShopOpen=>_page=="shop";public event Action UiPressed;
  private RectTransform _safe,_modal,_content;private Text _height,_coins,_hint;private Image _progress;private Button _menu;private int _width,_heightScreen;private Vector2 _safeSize;private string _page="";private float _hintUntil,_nextVideoPoll;private Button _watchButton;
  private static readonly Color Navy=new(.025f,.085f,.18f,.95f),Gold=new(1,.73f,.12f),Purple=new(.40f,.10f,.86f),Blue=new(.04f,.48f,.90f);
  private RisingProfile P=>Course.Profile;public string T(string de,string en)=>P.Language=="en"?en:de;
  public void Initialize(RisingCourse course){Course=course;var canvas=UiFactory.Canvas();_safe=UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);_safe.gameObject.AddComponent<SafeAreaFitter>();
   _menu=UiFactory.Button(_safe,"Menu","",Navy,Color.white,Vector2.zero,Vector2.one,ShowHome);HudIcons.Add(_menu.transform,HudIcons.Kind.Menu);
   var height=UiFactory.Panel(_safe,"Height",Navy,Vector2.zero,Vector2.one);_height=UiFactory.Label(height,"Value","",20,new Vector2(.05f,.33f),new Vector2(.95f,.97f),TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);_progress=UiFactory.Progress(height,new Vector2(.10f,.14f),new Vector2(.90f,.25f),new Color(.16f,.23f,.36f),Gold);
   var coins=UiFactory.Panel(_safe,"Crystals",Navy,Vector2.zero,Vector2.one);var gem=UiFactory.Panel(coins,"CrystalIcon",Color.clear,new Vector2(.04f,.18f),new Vector2(.31f,.82f));HudIcons.Add(gem,HudIcons.Kind.Crystal);gem.GetComponentInChildren<HudIcons>().color=Gold;_coins=UiFactory.Label(coins,"Value","",20,new Vector2(.30f,.05f),new Vector2(.95f,.95f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
   _hint=UiFactory.Label(_safe,"RouteHint","",17,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);
   var outline=_hint.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.01f,.04f,.09f,.95f);outline.effectDistance=new Vector2(1,-1);
   Joystick=VirtualJoystick.Create(_safe,Vector2.zero,Vector2.one);Jump=PressButton.Create(_safe,"",Vector2.zero,Vector2.one);HudIcons.Add(Jump.transform,HudIcons.Kind.Jump);
   course.Changed+=Refresh;course.Landed+=perfect=>Hint(perfect?T("PERFEKT! +5","PERFECT! +5"):T("Etappe geschafft","Step cleared"));course.Fell+=()=>Hint(T("Zurück am Kontrollpunkt","Back at checkpoint"));course.PortalCompleted+=ShowCompletion;
   Layout();Refresh();ShowHome();
  }
  private static void Place(RectTransform r,float x,float y,float w,float h,bool bottom=false){r.anchorMin=r.anchorMax=new Vector2(0,bottom?0:1);r.pivot=new Vector2(0,bottom?0:1);r.anchoredPosition=new Vector2(x,bottom?y:-y);r.sizeDelta=new Vector2(w,h);}
  private void Layout(){Canvas.ForceUpdateCanvases();float w=_safe.rect.width,h=_safe.rect.height;float target=UiMetrics.TargetSize(_safe,52),headerX=target+22;Place((RectTransform)_menu.transform,12,12,target,target);Place((RectTransform)_height.transform.parent,headerX,12,Mathf.Max(90,w-headerX-118),target);Place((RectTransform)_coins.transform.parent,w-108,12,96,target);Place((RectTransform)_hint.transform,headerX,target+18,w-headerX-74,26);
   if(w<280){Place((RectTransform)_height.transform.parent,headerX,12,w-headerX-12,target);Place((RectTransform)_coins.transform.parent,w-90,target+20,78,target);Place((RectTransform)_hint.transform,12,target*2+26,w-24,24);}
   float stick=w<220?68:h<420?90:112,jump=w<220?68:h<420?82:94;stick=UiMetrics.TargetSize(_safe,stick);jump=UiMetrics.TargetSize(_safe,jump);Place((RectTransform)Joystick.transform,16,16,stick,stick,true);Place((RectTransform)Jump.transform,w-jump-16,20,jump,jump,true);
   _width=Screen.width;_heightScreen=Screen.height;_safeSize=_safe.rect.size;
  }
  private void Update(){if(ShopOpen&&_watchButton!=null&&Time.unscaledTime>=_nextVideoPoll){_nextVideoPoll=Time.unscaledTime+1;_watchButton.interactable=Course.Videos.CanWatch;}if(_width!=Screen.width||_heightScreen!=Screen.height||_safeSize!=_safe.rect.size){Layout();if(ModalOpen)Reopen();}if(!ModalOpen&&Time.unscaledTime>_hintUntil)_hint.text=Course.Height==12?T("Betritt das goldene Portal","Enter the golden portal"):T("Nächste Insel: ","Next island: ")+(Course.Height+1);}
  public void Refresh(){if(Course==null||P==null)return;UiFactory.HighContrast=P.HighContrast;var navy=Navy;navy.a=P.HighContrast?1:.95f;_menu.GetComponent<Image>().color=navy;_height.transform.parent.GetComponent<Image>().color=navy;_coins.transform.parent.GetComponent<Image>().color=navy;Jump.GetComponent<Image>().color=new Color(.03f,.06f,.10f,P.HighContrast?1:.74f);Joystick.GetComponent<Image>().color=new Color(.03f,.06f,.10f,P.HighContrast?.94f:.62f);
   foreach(var step in Course.Steps){var contrast=step.transform.parent.Find("RouteContrastRim");if(contrast!=null)contrast.gameObject.SetActive(P.HighContrast);}
   _height.text=T("HÖHE ","HEIGHT ")+Course.Height+" / 12";_coins.text=P.Crystals.ToString("N0",System.Globalization.CultureInfo.GetCultureInfo(P.Language=="de"?"de-DE":"en-US"));_progress.fillAmount=Course.Height/12f;var runner=Course.Player.GetComponentInChildren<RunnerAnimator>();runner?.Style(P.Style);}
  private void Hint(string text){_hint.text=text;_hintUntil=Time.unscaledTime+2.2f;}
  private void Tap(){UiPressed?.Invoke();}
  public void Close(){if(Course.Store?.IsPresenting==true||Course.Videos?.IsPresenting==true)return;Tap();if(_modal!=null){_modal.gameObject.SetActive(false);Destroy(_modal.gameObject);}_modal=null;_content=null;_page="";Course.Paused=false;}
  private void Begin(string page,string title){UiFactory.HighContrast=P.HighContrast;Tap();if(_modal!=null){_modal.gameObject.SetActive(false);Destroy(_modal.gameObject);}_page=page;Course.Paused=true;
   _modal=UiFactory.Panel(_safe,"ModalBackdrop",new Color(.015f,.045f,.09f,.52f),Vector2.zero,Vector2.one);
   float sw=_safe.rect.width,sh=_safe.rect.height,w=Mathf.Min(sw-24,540),h=sh-32,closeSize=UiMetrics.TargetSize(_safe,52);var card=UiFactory.Panel(_modal,"ModalCard",Navy,Vector2.zero,Vector2.one);Place(card,(sw-w)/2,16,w,h);
   UiFactory.Label(card,"Title",title,28,new Vector2(.07f,.86f),new Vector2(.93f,.98f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
   var viewport=UiFactory.Panel(card,"Viewport",Color.clear,Vector2.zero,Vector2.one);viewport.GetComponent<Image>().raycastTarget=true;Place(viewport,16,h*.15f,w-32,h*.85f-closeSize-30);viewport.gameObject.AddComponent<RectMask2D>();
   var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;scroll.viewport=viewport;scroll.movementType=ScrollRect.MovementType.Clamped;
   var cg=new GameObject("Content",typeof(RectTransform),typeof(VerticalLayoutGroup),typeof(ContentSizeFitter));cg.transform.SetParent(viewport,false);_content=(RectTransform)cg.transform;_content.anchorMin=new Vector2(0,1);_content.anchorMax=Vector2.one;_content.pivot=new Vector2(.5f,1);_content.offsetMin=_content.offsetMax=Vector2.zero;
   var layout=cg.GetComponent<VerticalLayoutGroup>();layout.spacing=10;layout.childControlHeight=true;layout.childControlWidth=true;layout.childForceExpandHeight=false;layout.padding=new RectOffset(0,0,0,8);cg.GetComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.content=_content;
   var close=UiFactory.Button(card,"Close",T("ZURÜCK ZUM SPIEL","BACK TO GAME"),Blue,Color.white,Vector2.zero,Vector2.one,Close);Place((RectTransform)close.transform,16,h-closeSize-14,w-32,closeSize);
  }
  private void Row(string name,string label,Color color,Action action,bool enabled=true){var button=UiFactory.Button(_content,name,label,color,Color.white,Vector2.zero,Vector2.one,()=>{Tap();action?.Invoke();});button.gameObject.AddComponent<LayoutElement>().preferredHeight=UiMetrics.TargetSize(_safe,56);button.interactable=enabled;if(name=="RewardVideo")_watchButton=button;}
  private void Note(string label,int height=62){var t=UiFactory.Label(_content,"Description",label,19,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,new Color(.83f,.91f,1));t.gameObject.AddComponent<LayoutElement>().preferredHeight=height;}
  public void ShowHome(){Begin("home",T("RISING STEPS · REICHE","RISING STEPS · REALMS"));Note(T("12 Himmelsinseln. Ein strahlendes Ziel.\nBewege dich links, springe rechts, drehe die Kamera im freien Bild.","12 sky islands. One radiant destination.\nMove left, jump right, rotate the camera on the open view."),92);
   string[] de={"Wiesenhimmel","Wasserfall-Reich","Tempelpfad"},en={"Meadow Sky","Waterfall Realm","Temple Trail"};for(int i=0;i<3;i++){int realm=i;bool unlocked=i<=P.UnlockedRealm;Row("Realm"+i,(unlocked?T(de[i],en[i])+"  ·  "+P.Stars[i]+" / 3 "+T("Sterne","stars"):T("Gesperrt · ","Locked · ")+T(de[i],en[i])),unlocked?Blue:Navy,()=>{Course.StartRun(realm,false);Close();},unlocked);}
   Row("DailyChallenge",T("TAGESROUTE · 8 perfekte Landungen: +75","DAILY ROUTE · 8 perfect landings: +75"),Purple,()=>{int seed=DateTime.UtcNow.DayOfYear%(P.UnlockedRealm+1);Course.StartRun(seed,true);Close();});
   Row("Shop",T("SHOP · DESIGN & KRISTALLE","SHOP · DESIGNS & CRYSTALS"),Purple,ShowShop);Row("Daily",T("TAGESGESCHENK","DAILY GIFT"),new Color(1,.40f,.06f),ShowDaily);Row("Style",T("DEIN STIL","YOUR STYLE"),Blue,ShowStyle);Row("Achievements",T("ERFOLGE","ACHIEVEMENTS"),Navy,ShowAchievements);Row("Settings",T("EINSTELLUNGEN","SETTINGS"),Navy,ShowSettings);
  }
  public void ShowShop(){Begin("shop",T("DESIGN-SHOP","DESIGN SHOP"));Note(T("Verdiene Kristalle beim Springen.\nFreigeschaltete Designs bleiben dauerhaft dein.","Earn crystals by jumping.\nUnlocked designs remain yours."),76);StyleRows();CommerceRows();}
  public void ShowStyle(){Begin("style",T("DEIN LÄUFER","YOUR RUNNER"));StyleRows();}
  private void StyleRows(){string[] de={"Waldgrün","Himmelsblau","Sonnenkupfer","Auroraviolett","Goldener Pfad","Himmelsflügel","Nordlicht","Sternennacht"},en={"Forest Green","Sky Blue","Sun Copper","Aurora Violet","Golden Trail","Sky Wings","Northern Lights","Starry Night"};int[] prices={0,75,150,250};for(int i=0;i<8;i++){int s=i;Row("Style"+i,T(de[i],en[i])+"  ·  "+(P.Style==i?T("AKTIV","ACTIVE"):P.Styles[i]?T("AUSWÄHLEN","SELECT"):i>=4?T("EXTRA-DESIGN","EXTRA DESIGN"):prices[i]+T(" Kristalle"," crystals")),i==P.Style?Blue:Purple,()=>{if(Course.SelectStyle(s)){Reopen();}else Note(T("Nicht genug Kristalle. Spiele die nächsten Etappen!","Not enough crystals. Play the next steps!"));},P.Styles[i]||(i<4&&P.Crystals>=prices[i]));}}
  public void AttachCommerce(){Course.Store.Changed+=CommerceChanged;Course.Videos.Changed+=CommerceChanged;}
  private void CommerceChanged(){Refresh();if(ShopOpen)ShowShop();}
  private void CommerceRows(){var store=Course.Store;var videos=Course.Videos;if(store==null||videos==null)return;
   Note(T("EXTRA-DESIGNS · FREIWILLIG","EXTRA DESIGNS · OPTIONAL"),52);
   void Product(string id,string title,string description){Note(description,76);var price=store.Price(id);Row(id,title+" · "+(store.Owned(id)?T("GEKAUFT","OWNED"):string.IsNullOrWhiteSpace(price)?T("NICHT VERFÜGBAR","UNAVAILABLE"):price),Purple,()=>store.Buy(id),store.CanBuy(id));}
   Product(Kamilunavo.RisingSteps.Monetization.CommerceRules.Starter,T("STARTERPAKET","STARTER PACK"),T("Goldener Pfad als dauerhaftes Design und einmalig 500 Kristalle.","Golden Trail permanent design and 500 crystals once."));
   Product(Kamilunavo.RisingSteps.Monetization.CommerceRules.Collection,T("HIMMELSKOLLEKTION","SKY COLLECTION"),T("Drei dauerhafte Designs: Himmelsflügel, Nordlicht und Sternennacht.","Three permanent designs: Sky Wings, Northern Lights and Starry Night."));
   Note(store.Status,72);Row("Restore",T("KÄUFE WIEDERHERSTELLEN","RESTORE PURCHASES"),Navy,store.Restore,store.Ready&&!store.Busy);
   Note(videos.Status,76);int remaining=Kamilunavo.RisingSteps.Monetization.RewardRules.Remaining(P,DateTime.UtcNow);Row("RewardVideo",T("FREIWILLIGES VIDEO · +50 KRISTALLE","OPTIONAL VIDEO · +50 CRYSTALS")+" · "+remaining+"/5",Blue,videos.Watch,videos.CanWatch);
   Row("PrepareVideo",T("VIDEO VORBEREITEN","PREPARE VIDEO"),Navy,videos.Prepare,!videos.IsPresenting);if(videos.PrivacyRequired)Row("PrivacyOptions",T("DATENSCHUTZEINSTELLUNGEN","PRIVACY OPTIONS"),Navy,videos.ShowPrivacy,!videos.IsPresenting);
   if(Kamilunavo.RisingSteps.Monetization.MonetizationConfig.Load().InternalTestAds)Note(T("Interner Test: Videos verwenden Testanzeigen.","Internal test: videos use test ads."),62);
  }
  public void ShowDaily(){Begin("daily",T("TAGESGESCHENK","DAILY GIFT"));bool ready=string.CompareOrdinal(RisingRules.Day(DateTime.UtcNow),P.DailyDay)>0;Note(T("Einmal je UTC-Tag. Deine Serie erhöht die Belohnung bis auf 160 Kristalle.","Once each UTC day. Your streak increases the reward up to 160 crystals."),100);Note(T("Serie: ","Streak: ")+P.DailyStreak+" / 7");Row("ClaimDaily",ready?T("GESCHENK ABHOLEN","CLAIM GIFT"):T("HEUTE BEREITS ABGEHOLT","ALREADY CLAIMED TODAY"),new Color(1,.4f,.06f),()=>{Course.ClaimDaily();ShowDaily();},ready);}
  public void ShowSettings(){Begin("settings",T("EINSTELLUNGEN","SETTINGS"));void Toggle(Action act){act();RisingSave.Save(P);Refresh();ShowSettings();}
   Row("Language",T("SPRACHE: DEUTSCH","LANGUAGE: ENGLISH"),Blue,()=>Toggle(()=>P.Language=P.Language=="de"?"en":"de"));
   Row("Sound",T("TON: ","SOUND: ")+On(P.Sound),Navy,()=>Toggle(()=>P.Sound=!P.Sound));Row("Haptics",T("HAPTIK: ","HAPTICS: ")+On(P.Haptics),Navy,()=>Toggle(()=>P.Haptics=!P.Haptics));Row("Motion",T("WENIGER BEWEGUNG: ","REDUCED MOTION: ")+On(P.ReducedMotion),Navy,()=>Toggle(()=>P.ReducedMotion=!P.ReducedMotion));Row("Contrast",T("HOHER KONTRAST: ","HIGH CONTRAST: ")+On(P.HighContrast),Navy,()=>Toggle(()=>P.HighContrast=!P.HighContrast));}
  private string On(bool b)=>b?T("AN","ON"):T("AUS","OFF");
  public void ShowAchievements(){Begin("achievements",T("ERFOLGE","ACHIEVEMENTS"));Note(T("Dein Fortschritt wird auf diesem Gerät gespeichert.","Your progress is saved on this device."));string Mark(bool done)=>done?T("GESCHAFFT · ","DONE · "):T("ZIEL · ","GOAL · ");Note(Mark(P.Stars[0]>0)+T("Erstes Reich","First realm"));Note(Mark(P.Stars[0]>0&&P.Stars[1]>0&&P.Stars[2]>0)+T("Alle drei Reiche","All three realms"));Note(Mark(P.BestPerfects==12)+T("12 perfekte Landungen in einer Route","12 perfect landings in one route"));Note(Mark(P.BestDailyStreak==7)+T("Sieben Tage in Folge","Seven days in a row"));}
  public void ShowCompletion(){Begin("complete",T("PORTAL ERREICHT!","PORTAL REACHED!"));int stars=P.Falls>0?1:P.Elapsed<=180?3:2;Note(stars+" / 3 "+T("Sterne","stars")+"\n"+P.Perfects+" "+T("perfekte Landungen","perfect landings")+" · "+P.Falls+" "+T("Stürze","falls"),110);if(!P.Challenge&&P.Realm<2)Row("NextRealm",T("NÄCHSTES REICH","NEXT REALM"),Blue,()=>{Course.StartRun(P.Realm+1,false);Close();});Row("Replay",T("NOCH EINMAL","PLAY AGAIN"),Purple,()=>{Course.StartRun(P.Realm,P.Challenge);Close();});Row("Home",T("ALLE REICHE","ALL REALMS"),Navy,ShowHome);}
  private void Reopen(){switch(_page){case "settings":ShowSettings();break;case "daily":ShowDaily();break;case "style":ShowStyle();break;case "shop":ShowShop();break;case "complete":ShowCompletion();break;case "achievements":ShowAchievements();break;default:ShowHome();break;}}
 }
}
