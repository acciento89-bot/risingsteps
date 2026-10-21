using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Core.Mastery;
using Kamilunavo.RisingSteps.Gameplay;

namespace Kamilunavo.RisingSteps.UI
{
 public sealed partial class RisingHud
 {
  RisingMastery _mastery;int _trialLevel;string _trialHint;
  public void AttachMastery(RisingMastery mastery){_mastery=mastery;_mastery.Split+=TrialSplit;}
  void OnDestroy(){if(_mastery!=null)_mastery.Split-=TrialSplit;}
  string TrialTime(double seconds)=>seconds.ToString("0.00",CultureInfo.GetCultureInfo(P.Language=="en"?"en-US":"de-DE"))+" s";
  void TrialSplit(int step,double seconds)
  {
   // Reuse the existing reserved hint, without adding controls to the route.
   _trialHint=T("Etappe ","Step ")+step+" · "+TrialTime(seconds);
  }
  void FlushTrialHint(){if(_trialHint==null)return;var message=_trialHint;_trialHint=null;if(_mastery!=null&&_mastery.Recording&&!ModalOpen)Hint(message);}
  void TrialWarning()
  {
   if(_mastery.Error!=null)Note(T("Das Aufstiegsbuch ist nicht verfügbar. Vorhandene Daten bleiben erhalten. Das normale Spiel bleibt nutzbar.","The climb journal is unavailable. Existing data is preserved. Normal play remains available."),128);
   if(_mastery.HasPending)Note(T("Dein letzter Aufstieg ist noch nicht gespeichert. Bitte erneut versuchen, bevor du einen weiteren aufzeichnest.","Your last climb has not been saved. Retry before recording another climb."),116);
   if(_mastery.Error!=null||_mastery.HasPending)Row("RetryTrialSave",T("SPEICHERN ERNEUT VERSUCHEN","RETRY JOURNAL SAVE"),Leaf,()=>{_mastery.RetryPending();Reopen();});
  }
  public void ShowTrialAlbum(int realm)
  {
   if(PresenterActive||_mastery==null||realm<0||realm>P.UnlockedRealm)return;_levelRealm=realm;
   Begin("trials",T("AUFSTIEGSBUCH","CLIMB JOURNAL"));
   Note(T("Eigene Zeiten und 12 Etappen pro aufgezeichnetem Aufstieg. Sterne und Belohnungen folgen den normalen Spielregeln.","Your own times and 12 splits per recorded climb. Stars and rewards follow the normal game rules."),128);
   TrialWarning();
   if(!P.TutorialDone)Note(T("Schließe zuerst das Tutorial ab. Danach kannst du freigeschaltete Level aufzeichnen.","Complete the tutorial first. Then record unlocked levels."),100);
   Row("PersonalPacePreference",T("TEMPO-LICHT: ","PACE LIGHT: ")+On(_mastery.Book.GuideEnabled),Blue,()=>{_mastery.SetGuide(!_mastery.Book.GuideEnabled);ShowTrialAlbum(realm);},!_mastery.HasPending&&_mastery.Error==null);
   Note(T("Das optionale Licht folgt deinem passenden sturzfreien Rekord. Bei weniger Bewegung bleibt es verborgen. Es verändert weder Kamera noch Landungen.","The optional light follows your compatible clean best. Reduced Motion hides it. It does not change the camera or landings."),144);
   for(int i=0;i<10;i++)
   {
    int level=i;bool unlocked=RisingCampaign.IsUnlocked(P,realm,level);int attempts=0;foreach(var entry in _mastery.Book.Entries)if(entry.Key.Realm==realm&&entry.Key.Level==level)attempts+=entry.TotalAttempts;
    Row("TrialCourse"+i,(i+1)+" · "+RisingCampaign.Title(realm,i,P.Language=="en")+"\n"+(unlocked?attempts+T(" Aufzeichnungen"," recordings"):T("Gesperrt","Locked")),unlocked?Leaf:Navy,()=>ShowTrialCourse(realm,level),unlocked);
   }
  }
  public void ShowTrialCourse(int realm,int level)
  {
   if(PresenterActive||_mastery==null||!RisingCampaign.IsUnlocked(P,realm,level))return;
   _levelRealm=realm;_trialLevel=level;Begin("trialcourse",(level+1)+" · "+RisingCampaign.Title(realm,level,P.Language=="en"));
   TrialWarning();
   Note(T("Vom Start bis zum Portal. Nur aktive Spielzeit zählt. Unterbrochene oder fortgesetzte Läufe werden nicht aufgezeichnet. Tagesrouten bleiben getrennt.","From the start to the portal. Only active play time counts. Interrupted or resumed runs are not recorded. Daily routes remain separate."),144);
   Row("StartRecordedClimb",T("NEUEN AUFSTIEG AUFZEICHNEN","RECORD A FRESH CLIMB"),Leaf,()=>{if(_mastery.StartRecorded(realm,level)){Close();Hint(T("Aufzeichnung gestartet","Recording started"));}else ShowTrialCourse(realm,level);},P.TutorialDone&&!_mastery.HasPending&&_mastery.Error==null);
   bool any=false;
   foreach(var entry in _mastery.Book.Entries)
   {
    if(entry.Key.Realm!=realm||entry.Key.Level!=level)continue;any=true;
    Section(T("ROUTE ","ROUTE ")+entry.Key.Geometry.Substring(0,8));
    Note(entry.TotalAttempts+T(" Aufzeichnungen · "," recordings · ")+entry.ImprovementCount+T(" Verbesserungen"," improvements"),64);
    Note(entry.Best==null?T("Noch kein sturzfreier Rekord.","No clean best yet."):T("Sturzfreier Rekord: ","Clean best: ")+TrialTime(entry.Best.Seconds)+(entry.Best.Samples.Length==0?T("\nOhne Tempo-Licht","\nNo pace-light recording"):""),80);
    Note(T("Routenstände haben getrennte Rekorde. Ein Licht erscheint nur bei exakt passender Geometrie.","Route versions keep separate records. A light appears only for matching geometry."),96);
    for(int i=entry.History.Length-1;i>=0;i--){var attempt=entry.History[i];TrialFacts(attempt);}
   }
   if(!any)Note(T("Noch keine Aufzeichnung für dieses Level.","No recording for this level yet."),76);
  }
  void TrialFacts(MasteryAttempt attempt)
  {
   DateTime at=DateTime.ParseExact(attempt.RecordedUtc,"O",CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind);
   Section(at.ToString("yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture)+" UTC · "+TrialTime(attempt.Seconds));
   Note(attempt.Falls+T(" Stürze · "," falls · ")+attempt.Perfects+T(" perfekte Landungen"," perfect landings"),64);
   for(int i=0;i<12;i+=3)Note((i+1)+": "+TrialTime(attempt.Splits[i])+"\n"+(i+2)+": "+TrialTime(attempt.Splits[i+1])+"\n"+(i+3)+": "+TrialTime(attempt.Splits[i+2]),98);
  }
  void AppendTrialCompletion()
  {
   if(_mastery==null||_mastery.LastCompleted==null)return;
   Section(T("DEINE AUFZEICHNUNG","YOUR RECORDING"));TrialWarning();TrialFacts(_mastery.LastCompleted);
   Row("OpenRecordedJournal",T("AUFSTIEGSBUCH ÖFFNEN","OPEN CLIMB JOURNAL"),Leaf,()=>ShowTrialCourse(_mastery.LastCompleted.Key.Realm,_mastery.LastCompleted.Key.Level));
  }
 }
}
