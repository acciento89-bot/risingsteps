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
  public RisingCourse Course{get;private set;}public VirtualJoystick Joystick{get;private set;}public PressButton Jump{get;private set;}public CameraLookControl Look{get;private set;}public bool ModalOpen=>_modal!=null;public bool ShopOpen=>_page=="shop";public event Action UiPressed;
  private RectTransform _safe,_modal,_content;private Text _height,_coins,_hint;private Image _progress;private Button _menu;private int _width,_heightScreen;private Vector2 _safeSize;private string _page="";private float _hintUntil,_nextVideoPoll;private Button _watchButton,_retryStoreButton;
  private static readonly Color Navy=new(.025f,.085f,.18f,.95f),Gold=new(1,.73f,.12f),Purple=new(.40f,.10f,.86f),Blue=new(.04f,.48f,.90f);
  private RisingTutorial _tutorial;private static readonly Color Ink=new(.10f,.23f,.26f),Paper=new(.96f,.97f,.91f),Leaf=new(.22f,.43f,.31f);
  private RisingProfile P=>Course.Profile;public string T(string de,string en)=>P.Language=="en"?en:de;
  public void Initialize(RisingCourse course){Course=course;var canvas=UiFactory.Canvas();_safe=UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);_safe.gameObject.AddComponent<SafeAreaFitter>();
   _menu=UiFactory.Button(_safe,"Menu","",Navy,Color.white,Vector2.zero,Vector2.one,ShowHome);HudIcons.Add(_menu.transform,HudIcons.Kind.Menu);
   var height=UiFactory.Panel(_safe,"Height",Navy,Vector2.zero,Vector2.one);_height=UiFactory.Label(height,"Value","",20,new Vector2(.05f,.33f),new Vector2(.95f,.97f),TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);_progress=UiFactory.Progress(height,new Vector2(.10f,.14f),new Vector2(.90f,.25f),new Color(.16f,.23f,.36f),Gold);
   var coins=UiFactory.Panel(_safe,"Crystals",Navy,Vector2.zero,Vector2.one);var gem=UiFactory.Panel(coins,"CrystalIcon",Color.clear,new Vector2(.04f,.18f),new Vector2(.31f,.82f));HudIcons.Add(gem,HudIcons.Kind.Crystal);gem.GetComponentInChildren<HudIcons>().color=Gold;_coins=UiFactory.Label(coins,"Value","",20,new Vector2(.30f,.05f),new Vector2(.95f,.95f),TextAnchor.MiddleCenter,Gold,FontStyle.Bold);
   _hint=UiFactory.Label(_safe,"RouteHint","",17,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Color.white,FontStyle.Bold);
   var outline=_hint.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.01f,.04f,.09f,.95f);outline.effectDistance=new Vector2(1,-1);
   Joystick=VirtualJoystick.Create(_safe,Vector2.zero,Vector2.one);Jump=PressButton.Create(_safe,"",Vector2.zero,Vector2.one);HudIcons.Add(Jump.transform,HudIcons.Kind.Jump);
   var view=UiFactory.Button(_safe,"View",T("BLICK","VIEW"),Navy,Color.white,Vector2.zero,Vector2.one,null);Look=view.gameObject.AddComponent<CameraLookControl>();
   _tutorial=gameObject.AddComponent<RisingTutorial>();_tutorial.Initialize(course,this,_safe);
   course.Changed+=Refresh;course.Landed+=perfect=>Hint(perfect?T("PERFEKT! +5","PERFECT! +5"):T("Etappe geschafft","Step cleared"));course.Fell+=()=>Hint(T("Zurück am Kontrollpunkt","Back at checkpoint"));course.PortalCompleted+=ShowCompletion;
   Layout();Refresh();ShowHome();
  }
  private static void Place(RectTransform r,float x,float y,float w,float h,bool bottom=false){r.anchorMin=r.anchorMax=new Vector2(0,bottom?0:1);r.pivot=new Vector2(0,bottom?0:1);r.anchoredPosition=new Vector2(x,bottom?y:-y);r.sizeDelta=new Vector2(w,h);}
  private void Layout(){Canvas.ForceUpdateCanvases();float w=_safe.rect.width,h=_safe.rect.height;float target=UiMetrics.TargetSize(_safe,52),headerX=target+22;Place((RectTransform)_menu.transform,12,12,target,target);Place((RectTransform)_height.transform.parent,headerX,12,Mathf.Max(90,w-headerX-118),target);Place((RectTransform)_coins.transform.parent,w-108,12,96,target);Place((RectTransform)_hint.transform,headerX,target+18,w-headerX-74,26);
   if(w<280){Place((RectTransform)_height.transform.parent,headerX,12,w-headerX-12,target);Place((RectTransform)_coins.transform.parent,w-90,target+20,78,target);Place((RectTransform)_hint.transform,12,target*2+26,w-24,24);}
   float stick=w<220?68:h<420?90:112,jump=w<220?68:h<420?82:94;stick=UiMetrics.TargetSize(_safe,stick);jump=UiMetrics.TargetSize(_safe,jump);Place((RectTransform)Joystick.transform,16,16,stick,stick,true);Place((RectTransform)Jump.transform,w-jump-16,20,jump,jump,true);
   Place((RectTransform)Look.transform,12,target+24,target,target);
   _width=Screen.width;_heightScreen=Screen.height;_safeSize=_safe.rect.size;
  }
  private void Update(){if(ShopOpen&&Time.unscaledTime>=_nextVideoPoll){_nextVideoPoll=Time.unscaledTime+1;if(_watchButton!=null)_watchButton.interactable=Course.Videos.CanWatch;if(_retryStoreButton!=null)_retryStoreButton.interactable=Course.Store.CanRetry;}if(_width!=Screen.width||_heightScreen!=Screen.height||_safeSize!=_safe.rect.size){Layout();if(ModalOpen)Reopen();}if(!ModalOpen&&Time.unscaledTime>_hintUntil)_hint.text=Course.Height==12?T("Betritt das goldene Portal","Enter the golden portal"):T("Nächste Insel: ","Next island: ")+(Course.Height+1);}
  public void Refresh(){if(Course==null||P==null)return;UiFactory.HighContrast=P.HighContrast;var navy=Navy;navy.a=P.HighContrast?1:.95f;_menu.GetComponent<Image>().color=navy;_height.transform.parent.GetComponent<Image>().color=navy;_coins.transform.parent.GetComponent<Image>().color=navy;Jump.GetComponent<Image>().color=new Color(.03f,.06f,.10f,P.HighContrast?1:.74f);Joystick.GetComponent<Image>().color=new Color(.03f,.06f,.10f,P.HighContrast?.94f:.62f);
   foreach(var step in Course.Steps){var contrast=step.transform.parent.Find("RouteContrastRim");if(contrast!=null)contrast.gameObject.SetActive(P.HighContrast);}
   _height.text=T("HÖHE ","HEIGHT ")+Course.Height+" / 12";_coins.text=P.Crystals.ToString("N0",System.Globalization.CultureInfo.GetCultureInfo(P.Language=="de"?"de-DE":"en-US"));_progress.fillAmount=Course.Height/12f;var runner=Course.Player.GetComponentInChildren<RunnerAnimator>();runner?.Style(P.Style);}
  private void Hint(string text){_hint.text=text;_hintUntil=Time.unscaledTime+2.2f;}
  private void Tap(){UiPressed?.Invoke();}
  public void Play(){if(P.Completed)Course.StartRun(P.Realm,false);Close();if(!P.TutorialDone)_tutorial.Begin();}
  public void ReplayTutorial(){if(P.Completed||P.Step>=12)Course.StartRun(P.Realm,false);Close();_tutorial.Begin();}
  public void Close(){if(Course.Store?.IsPresenting==true||Course.Videos?.IsPresenting==true)return;Tap();if(_modal!=null){_modal.gameObject.SetActive(false);Destroy(_modal.gameObject);}_modal=null;_content=null;_page="";Course.Paused=false;}
  private void Begin(string page,string title){UiFactory.HighContrast=P.HighContrast;Tap();if(_modal!=null){_modal.gameObject.SetActive(false);Destroy(_modal.gameObject);}_page=page;Course.Paused=true;
   _modal=UiFactory.Panel(_safe,"ModalBackdrop",new Color(.32f,.65f,.73f,.58f),Vector2.zero,Vector2.one);
   float sw=_safe.rect.width,sh=_safe.rect.height,w=Mathf.Min(sw-24,540),h=sh-32,closeSize=UiMetrics.TargetSize(_safe,52);var card=UiFactory.Panel(_modal,"ModalCard",Paper,Vector2.zero,Vector2.one);Place(card,(sw-w)/2,16,w,h);
   UiFactory.Label(card,"Title",title,28,new Vector2(.07f,.89f),new Vector2(.93f,.98f),TextAnchor.MiddleCenter,Ink,FontStyle.Bold);
   var viewport=UiFactory.Panel(card,"Viewport",Color.clear,Vector2.zero,Vector2.one);viewport.GetComponent<Image>().raycastTarget=true;Place(viewport,16,h*.115f,w-32,h*.885f-closeSize-30);viewport.gameObject.AddComponent<RectMask2D>();
   var scroll=viewport.gameObject.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;scroll.viewport=viewport;scroll.movementType=ScrollRect.MovementType.Clamped;
   var cg=new GameObject("Content",typeof(RectTransform),typeof(VerticalLayoutGroup),typeof(ContentSizeFitter));cg.transform.SetParent(viewport,false);_content=(RectTransform)cg.transform;_content.anchorMin=new Vector2(0,1);_content.anchorMax=Vector2.one;_content.pivot=new Vector2(.5f,1);_content.offsetMin=_content.offsetMax=Vector2.zero;
   var layout=cg.GetComponent<VerticalLayoutGroup>();layout.spacing=10;layout.childControlHeight=true;layout.childControlWidth=true;layout.childForceExpandHeight=false;layout.padding=new RectOffset(0,0,0,8);cg.GetComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.content=_content;
   var close=UiFactory.Button(card,"Close",T("ZURÜCK ZUM SPIEL","BACK TO GAME"),Leaf,Color.white,Vector2.zero,Vector2.one,Close);Place((RectTransform)close.transform,16,h-closeSize-14,w-32,closeSize);
  }
  private void Row(string name,string label,Color color,Action action,bool enabled=true){var button=UiFactory.Button(_content,name,label,Color.white,Ink,Vector2.zero,Vector2.one,()=>{Tap();action?.Invoke();});var text=button.GetComponentInChildren<Text>();((RectTransform)text.transform).anchorMin=new Vector2(.07f,0);((RectTransform)text.transform).anchorMax=new Vector2(.97f,1);UiFactory.Panel(button.transform,"Accent",color,new Vector2(.025f,.18f),new Vector2(.04f,.82f)).GetComponent<Image>().raycastTarget=false;button.gameObject.AddComponent<LayoutElement>().preferredHeight=UiMetrics.TargetSize(_safe,56);button.interactable=enabled;if(name=="RewardVideo")_watchButton=button;if(name=="RetryStore")_retryStoreButton=button;}
  private void Note(string label,int height=62){var t=UiFactory.Label(_content,"Description",label,19,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,Ink);t.gameObject.AddComponent<LayoutElement>().preferredHeight=height;}
  public void ShowHome(){Begin("home",T("RISING STEPS","RISING STEPS"));
   var card=(RectTransform)_content.parent.parent;card.GetComponent<Image>().color=Paper;var title=card.Find("Title").GetComponent<Text>();title.color=Ink;title.alignment=TextAnchor.MiddleLeft;
   var hero=UiFactory.Panel(_content,"SkyAtlas",new Color(.88f,.94f,.91f),Vector2.zero,Vector2.one);hero.gameObject.AddComponent<LayoutElement>().preferredHeight=148;RealmIllustration.Add(hero,P.Realm,new Vector2(.57f,.055f),new Vector2(.98f,.945f),true);
   UiFactory.Label(hero,"Eyebrow",T("DEIN WEG ZUM HIMMEL","YOUR PATH TO THE SKY"),12,new Vector2(.045f,.76f),new Vector2(.53f,.94f),TextAnchor.MiddleLeft,Ink,FontStyle.Bold);
   UiFactory.Label(hero,"Story",T("Insel für Insel.\nBis zum Tempel.","Island by island.\nReach the temple."),27,new Vector2(.045f,.28f),new Vector2(.53f,.75f),TextAnchor.MiddleLeft,Ink,FontStyle.Bold);
   UiFactory.Label(hero,"Checkpoint",T("Etappe ","Step ")+P.Step+" / 12  ·  "+T("Reich ","Realm ")+(P.Realm+1),14,new Vector2(.045f,.08f),new Vector2(.53f,.25f),TextAnchor.MiddleLeft,Ink);
   Section(T("WÄHLE DEIN REICH","CHOOSE YOUR REALM"));
   string[] de={"Wiesenhimmel","Wasserfall-Reich","Tempelpfad"},en={"Meadow Sky","Waterfall Realm","Temple Trail"};string[] descriptionsDe={"Blühende Wiesen · Heller Himmel","Klare Becken · Rauschende Fälle","Alte Ruinen · Goldene Wege"},descriptionsEn={"Flowering meadows · Open blue skies","Clear pools · Cascading waterfalls","Ancient ruins · Golden paths"};
   for(int i=0;i<3;i++){int realm=i;bool unlocked=i<=P.UnlockedRealm;var button=UiFactory.Button(_content,"Realm"+i,"",unlocked?Color.white:new Color(.85f,.89f,.86f),Ink,Vector2.zero,Vector2.one,()=>{Course.StartRun(realm,false);Play();});button.interactable=unlocked;button.gameObject.AddComponent<LayoutElement>().preferredHeight=UiMetrics.TargetSize(_safe,86);button.gameObject.AddComponent<RectMask2D>();RealmIllustration.Add(button.transform,i,new Vector2(.025f,.09f),new Vector2(.31f,.91f));
    UiFactory.Label(button.transform,"RealmName",T(de[i],en[i]),21,new Vector2(.35f,.52f),new Vector2(.96f,.88f),TextAnchor.MiddleLeft,unlocked?Ink:Leaf,FontStyle.Bold);
    UiFactory.Label(button.transform,"RealmDetail",unlocked?T(descriptionsDe[i],descriptionsEn[i]):T("Schließe das vorige Reich ab","Complete the previous realm"),14,new Vector2(.35f,.27f),new Vector2(.97f,.56f),TextAnchor.MiddleLeft,Leaf);
    UiFactory.Label(button.transform,"RealmStatus",unlocked?(P.Stars[i]>0?P.Stars[i]+" / 3 "+T("STERNE","STARS"):T("12 INSELN · ENTDECKEN","12 ISLANDS · EXPLORE")):T("GESPERRT","LOCKED"),12,new Vector2(.35f,.06f),new Vector2(.94f,.29f),TextAnchor.MiddleLeft,Leaf,FontStyle.Bold);}
   Section(T("DEIN ABENTEUER","YOUR ADVENTURE"));
   TilePair("DailyChallenge",T("Tagesroute","Daily route"),T("8 perfekte Landungen · +75","8 perfect landings · +75"),()=>{Course.StartRun(DateTime.UtcNow.DayOfYear%(P.UnlockedRealm+1),true);Play();},"Daily",T("Geschenk","Daily gift"),T("Deine tägliche Belohnung","Your daily reward"),ShowDaily);
   TilePair("Shop",T("Design-Shop","Design shop"),T("Neue Farben entdecken","Discover new colours"),ShowShop,"Style",T("Dein Läufer","Your runner"),T("Deinen Stil auswählen","Choose your style"),ShowStyle);
   TilePair("Achievements",T("Erfolge","Achievements"),T("Deine Reise in Sternen","Your journey in stars"),ShowAchievements,"Settings",T("Einstellungen","Settings"),T("Sprache, Ton & Komfort","Language, sound & comfort"),ShowSettings);
   var tutorial=UiFactory.Button(_content,"Tutorial",T("KURZE SPIELSCHULE WIEDERHOLEN","REPLAY THE SHORT TUTORIAL"),new Color(.84f,.91f,.87f),Ink,Vector2.zero,Vector2.one,ReplayTutorial);tutorial.gameObject.AddComponent<LayoutElement>().preferredHeight=UiMetrics.TargetSize(_safe,49);
   var play=card.Find("Close").GetComponent<Button>();play.GetComponent<Image>().color=Gold;play.GetComponentInChildren<Text>().text=P.Step>0&&!P.Completed?T("WEITER · ETAPPE ","CONTINUE · STEP ")+P.Step:T("AUFSTEIGEN","START CLIMBING");play.GetComponentInChildren<Text>().color=Ink;play.onClick.RemoveAllListeners();play.onClick.AddListener(Play);foreach(var panel in _modal.GetComponentsInChildren<RoundedPanel>())panel.UseGallery();
  }
  private void Section(string text){var label=UiFactory.Label(_content,"Section",text,13,Vector2.zero,Vector2.one,TextAnchor.MiddleLeft,Leaf,FontStyle.Bold);label.gameObject.AddComponent<LayoutElement>().preferredHeight=25;}
  private void TilePair(string leftName,string leftTitle,string leftDetail,Action leftAction,string rightName,string rightTitle,string rightDetail,Action rightAction){
   var group=UiFactory.Panel(_content,"AdventureTiles",Color.clear,Vector2.zero,Vector2.one);group.gameObject.AddComponent<LayoutElement>().preferredHeight=UiMetrics.TargetSize(_safe,72);
   void Tile(string name,string title,string detail,Action action,float min,float max){var button=UiFactory.Button(group,name,"",new Color(.84f,.91f,.87f),Ink,new Vector2(min,0),new Vector2(max,1),()=>{Tap();action();});UiFactory.Label(button.transform,"TileTitle",title,18,new Vector2(.075f,.42f),new Vector2(.925f,.90f),TextAnchor.MiddleLeft,Ink,FontStyle.Bold);UiFactory.Label(button.transform,"TileDetail",detail,12,new Vector2(.075f,.10f),new Vector2(.925f,.45f),TextAnchor.MiddleLeft,Leaf);}
   Tile(leftName,leftTitle,leftDetail,leftAction,0,.485f);Tile(rightName,rightTitle,rightDetail,rightAction,.515f,1);
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
   Note(store.Status,72);if(!store.Ready)Row("RetryStore",T("STORE ERNEUT VERBINDEN","RECONNECT STORE"),Blue,store.RetryConnection,store.CanRetry);Row("Restore",T("KÄUFE WIEDERHERSTELLEN","RESTORE PURCHASES"),Navy,store.Restore,store.Ready&&!store.Busy);
   Note(videos.Status,76);int remaining=Kamilunavo.RisingSteps.Monetization.RewardRules.Remaining(P,DateTime.UtcNow);Row("RewardVideo",T("FREIWILLIGES VIDEO · +50 KRISTALLE","OPTIONAL VIDEO · +50 CRYSTALS")+" · "+remaining+"/5",Blue,videos.Watch,videos.CanWatch);
   Row("PrepareVideo",T("VIDEO VORBEREITEN","PREPARE VIDEO"),Navy,videos.Prepare,!videos.IsPresenting);if(videos.PrivacyRequired)Row("PrivacyOptions",T("DATENSCHUTZEINSTELLUNGEN","PRIVACY OPTIONS"),Navy,videos.ShowPrivacy,!videos.IsPresenting);
   if(Kamilunavo.RisingSteps.Monetization.MonetizationConfig.Load().InternalTestAds)Note(T("Interner Test: Videos verwenden Testanzeigen.","Internal test: videos use test ads."),62);
  }
  public void ShowDaily(){Begin("daily",T("TAGESGESCHENK","DAILY GIFT"));bool ready=string.CompareOrdinal(RisingRules.Day(DateTime.UtcNow),P.DailyDay)>0;Note(T("Einmal je UTC-Tag. Deine Serie erhöht die Belohnung bis auf 160 Kristalle.","Once each UTC day. Your streak increases the reward up to 160 crystals."),100);Note(T("Serie: ","Streak: ")+P.DailyStreak+" / 7");Row("ClaimDaily",ready?T("GESCHENK ABHOLEN","CLAIM GIFT"):T("HEUTE BEREITS ABGEHOLT","ALREADY CLAIMED TODAY"),new Color(1,.4f,.06f),()=>{Course.ClaimDaily();ShowDaily();},ready);}
  public void ShowSettings(){Begin("settings",T("EINSTELLUNGEN","SETTINGS"));void Toggle(Action act){act();RisingSave.Save(P);Refresh();ShowSettings();}
   Row("Tutorial",T("SPIELSCHULE WIEDERHOLEN","REPLAY TUTORIAL"),Leaf,ReplayTutorial);
   Row("ResetView",T("KAMERA ZUM PFAD AUSRICHTEN","FACE CAMERA TOWARD THE PATH"),Navy,()=>Look.Camera?.ResetView());
   Row("Language",T("SPRACHE: DEUTSCH","LANGUAGE: ENGLISH"),Blue,()=>Toggle(()=>P.Language=P.Language=="de"?"en":"de"));
   Row("Sound",T("TON: ","SOUND: ")+On(P.Sound),Navy,()=>Toggle(()=>P.Sound=!P.Sound));Row("Haptics",T("HAPTIK: ","HAPTICS: ")+On(P.Haptics),Navy,()=>Toggle(()=>P.Haptics=!P.Haptics));Row("Motion",T("WENIGER BEWEGUNG: ","REDUCED MOTION: ")+On(P.ReducedMotion),Navy,()=>Toggle(()=>P.ReducedMotion=!P.ReducedMotion));Row("Contrast",T("HOHER KONTRAST: ","HIGH CONTRAST: ")+On(P.HighContrast),Navy,()=>Toggle(()=>P.HighContrast=!P.HighContrast));}
  private string On(bool b)=>b?T("AN","ON"):T("AUS","OFF");
  public void ShowAchievements(){Begin("achievements",T("ERFOLGE","ACHIEVEMENTS"));Note(T("Dein Fortschritt wird auf diesem Gerät gespeichert.","Your progress is saved on this device."));string Mark(bool done)=>done?T("GESCHAFFT · ","DONE · "):T("ZIEL · ","GOAL · ");Note(Mark(P.Stars[0]>0)+T("Erstes Reich","First realm"));Note(Mark(P.Stars[0]>0&&P.Stars[1]>0&&P.Stars[2]>0)+T("Alle drei Reiche","All three realms"));Note(Mark(P.BestPerfects==12)+T("12 perfekte Landungen in einer Route","12 perfect landings in one route"));Note(Mark(P.BestDailyStreak==7)+T("Sieben Tage in Folge","Seven days in a row"));}
  public void ShowCompletion(){Begin("complete",T("PORTAL ERREICHT!","PORTAL REACHED!"));int stars=P.Falls>0?1:P.Elapsed<=180?3:2;Note(stars+" / 3 "+T("Sterne","stars")+"\n"+P.Perfects+" "+T("perfekte Landungen","perfect landings")+" · "+P.Falls+" "+T("Stürze","falls"),110);if(!P.Challenge&&P.Realm<2)Row("NextRealm",T("NÄCHSTES REICH","NEXT REALM"),Blue,()=>{Course.StartRun(P.Realm+1,false);Close();});Row("Replay",T("NOCH EINMAL","PLAY AGAIN"),Purple,()=>{Course.StartRun(P.Realm,P.Challenge);Close();});Row("Home",T("ALLE REICHE","ALL REALMS"),Navy,ShowHome);}
  private void Reopen(){switch(_page){case "settings":ShowSettings();break;case "daily":ShowDaily();break;case "style":ShowStyle();break;case "shop":ShowShop();break;case "complete":ShowCompletion();break;case "achievements":ShowAchievements();break;default:ShowHome();break;}}
 }
}
