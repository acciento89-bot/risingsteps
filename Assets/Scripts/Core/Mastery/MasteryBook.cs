using System;
using System.Collections.Generic;

namespace Kamilunavo.RisingSteps.Core.Mastery
{
 [Serializable] public sealed class MasteryEntry
 {
  public MasteryKey Key;public int TotalAttempts,ImprovementCount;public MasteryAttempt[] History=new MasteryAttempt[0];public MasteryAttempt Best;
  public MasteryEntry Copy(){var value=new MasteryEntry{Key=Key,TotalAttempts=TotalAttempts,ImprovementCount=ImprovementCount,Best=Best==null?null:Best.Copy()};value.History=new MasteryAttempt[History.Length];for(int i=0;i<History.Length;i++)value.History[i]=History[i].Copy(false);return value;}
 }
 [Serializable] public sealed class MasteryBook
 {
  public int Schema=1,Revision;public bool GuideEnabled;public MasteryEntry[] Entries=new MasteryEntry[0];
  public static MasteryBook Empty=>new MasteryBook();
  public MasteryBook Copy(){var value=new MasteryBook{Schema=Schema,Revision=Revision,GuideEnabled=GuideEnabled,Entries=new MasteryEntry[Entries.Length]};for(int i=0;i<Entries.Length;i++)value.Entries[i]=Entries[i].Copy();return value;}
 }
 public static class MasteryBookRules
 {
  public const int MaximumBytes=8*1024*1024;
  public static MasteryEntry Find(MasteryBook book,MasteryKey key){foreach(var entry in book.Entries)if(entry.Key.Equals(key))return entry;return null;}
  public static MasteryBook SetGuideEnabled(MasteryBook book,bool enabled){Validate(book);if(book.GuideEnabled==enabled)return book;var next=book.Copy();Increment(next);next.GuideEnabled=enabled;return next;}
  static void Increment(MasteryBook book){if(book.Revision==int.MaxValue)throw new InvalidOperationException("Journal revision limit");book.Revision++;}
  public static MasteryBook Record(MasteryBook book,MasteryAttempt attempt)
  {
   Validate(book);if(attempt==null)throw new ArgumentNullException("attempt");attempt.Validate();
   foreach(var entry in book.Entries){if(entry.Best!=null&&entry.Best.ID==attempt.ID){if(!SameFacts(entry.Best,attempt))throw new InvalidOperationException("Recorded ID conflict");return book;}foreach(var prior in entry.History)if(prior.ID==attempt.ID){if(!SameFacts(prior,attempt))throw new InvalidOperationException("Recorded ID conflict");return book;}}
   var next=book.Copy();var target=Find(next,attempt.Key);
   if(target==null){if(next.Entries.Length>=200)throw new InvalidOperationException("Journal course limit");var entries=new List<MasteryEntry>(next.Entries);target=new MasteryEntry{Key=attempt.Key};entries.Add(target);next.Entries=entries.ToArray();}
   if(target.TotalAttempts>=1000000)throw new InvalidOperationException("Attempt count limit");
   target.TotalAttempts++;var history=new List<MasteryAttempt>(target.History);history.Add(attempt.Copy(false));if(history.Count>5)history.RemoveAt(0);target.History=history.ToArray();
   if(attempt.Falls==0&&(target.Best==null||attempt.Seconds<target.Best.Seconds)){if(target.Best!=null)target.ImprovementCount++;target.Best=attempt.Copy();}
   PruneGuides(next);Increment(next);Validate(next);return next;
  }
  static void PruneGuides(MasteryBook book)
  {
   var guides=new List<MasteryEntry>();int samples=0;foreach(var entry in book.Entries)if(entry.Best!=null&&entry.Best.Samples.Length>0){guides.Add(entry);samples+=entry.Best.Samples.Length;}
   guides.Sort((a,b)=>{int date=string.CompareOrdinal(a.Best.RecordedUtc,b.Best.RecordedUtc);return date!=0?date:string.CompareOrdinal(a.Best.ID,b.Best.ID);});
   int index=0;while(guides.Count-index>20||samples>30000){samples-=guides[index].Best.Samples.Length;guides[index].Best.Samples=new MasterySample[0];index++;}
  }
  public static void Validate(MasteryBook book)
  {
   if(book==null||book.Schema!=1)throw new InvalidOperationException("Unsupported journal schema");
   if(book.Revision<0||book.Entries==null||book.Entries.Length>200)throw new ArgumentException("Invalid journal");
   var keys=new HashSet<MasteryKey>();var ids=new Dictionary<string,MasteryAttempt>();int guides=0,samples=0;
   foreach(var entry in book.Entries)
   {
    if(entry==null)throw new ArgumentException("Invalid journal entry");entry.Key.Validate();
    if(!keys.Add(entry.Key)||entry.TotalAttempts<1||entry.TotalAttempts>1000000||entry.ImprovementCount<0||entry.ImprovementCount>=entry.TotalAttempts||entry.History==null||entry.History.Length<1||entry.History.Length>5||entry.TotalAttempts<entry.History.Length)throw new ArgumentException("Invalid journal history");
    var localIDs=new HashSet<string>();
    foreach(var value in entry.History){if(value==null)throw new ArgumentException("Missing trial");value.Validate();if(!value.Key.Equals(entry.Key)||!localIDs.Add(value.ID)||value.Samples.Length!=0)throw new ArgumentException("Invalid history association");CheckID(ids,value);if(value.Falls==0&&(entry.Best==null||entry.Best.Seconds>value.Seconds))throw new ArgumentException("Clean best missing");}
    if(entry.Best==null){if(entry.ImprovementCount!=0)throw new ArgumentException("Improvement without best");}
    else{entry.Best.Validate();if(!entry.Best.Key.Equals(entry.Key)||entry.Best.Falls!=0)throw new ArgumentException("Invalid clean best");CheckID(ids,entry.Best);if(entry.Best.Samples.Length>0){guides++;samples+=entry.Best.Samples.Length;}}
   }
   if(guides>20||samples>30000)throw new ArgumentException("Guide memory limit");
  }
  static void CheckID(Dictionary<string,MasteryAttempt> ids,MasteryAttempt value){MasteryAttempt prior;if(ids.TryGetValue(value.ID,out prior)){if(!SameFacts(prior,value))throw new ArgumentException("Repeated ID changed facts");}else ids.Add(value.ID,value);}
  public static bool SameFacts(MasteryAttempt a,MasteryAttempt b)
  {
   if(a==null||b==null||a.ID!=b.ID||a.RecordedUtc!=b.RecordedUtc||!a.Key.Equals(b.Key)||a.Seconds!=b.Seconds||a.Falls!=b.Falls||a.Perfects!=b.Perfects||a.Splits.Length!=b.Splits.Length)return false;
   for(int i=0;i<a.Splits.Length;i++)if(a.Splits[i]!=b.Splits[i])return false;return true;
  }
 }
}
