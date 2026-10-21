using System;
using Kamilunavo.RisingSteps.Core.Mastery;

public static class MasteryTests
{
 static int checks;
 static void Check(bool ok,string message){checks++;if(!ok)throw new Exception("FAIL "+message);}
 static void Reject(Action action,string message){checks++;try{action();}catch(Exception){return;}throw new Exception("FAIL expected rejection: "+message);}
 static readonly string Geometry=new string('a',64);
 static MasteryKey Key(int realm=0,int level=0)=>new MasteryKey(realm,level,Geometry);
 static string ID(int number)=>number.ToString("x32");
 static string When(int minute=0)=>new DateTime(2026,10,21,8,minute,0,DateTimeKind.Utc).ToString("O");
 static MasteryAttempt Finish(MasteryKey key,int id,double delta=1,bool fell=false,int minute=0)
 {
  var run=new MasteryTrial(key,ID(id),0,false);run.Advance(0,true);run.Capture(0,0,0);
  for(int step=1;step<=12;step++){run.Advance(delta,true);run.Capture(step,step,0);Check(run.Land(step,true),"ordered actual landing accepted");}
  if(fell)run.Fall();return run.Complete(When(minute),12,12,0);
 }
 static void ClockAndEligibility()
 {
  var run=new MasteryTrial(Key(),ID(1),0,false);run.Advance(0,true);run.Advance(2,true);
  run.Advance(999,false);run.Advance(999,true);Check(run.Elapsed==2,"pause and first resume frame never charge background gap");
  run.Advance(3,true);Check(run.Elapsed==5,"active interval resumes after reanchor");
  Reject(()=>run.Advance(double.NaN,true),"NaN delta");Reject(()=>run.Advance(-1,true),"negative delta");Check(run.Elapsed==5,"invalid deltas cannot poison elapsed time");
  Reject(()=>new MasteryTrial(Key(),ID(2),4,false),"resumed checkpoint is not full trial");Reject(()=>new MasteryTrial(Key(),ID(2),0,true),"daily route cannot share campaign PB");
  Reject(()=>new MasteryKey(10,0,Geometry),"unknown realm");Reject(()=>new MasteryKey(0,10,Geometry),"unknown course");
  Check(run.Complete(When(),0,0,0)==null,"incomplete course cannot record");run.Cancel();Check(run.Complete(When(),0,0,0)==null,"cancelled course cannot record");
 }
 static void OrderedSplitsAndOneTerminalResult()
 {
  var run=new MasteryTrial(Key(),ID(3),0,false);run.Advance(0,true);run.Capture(0,0,0);
  run.Advance(1,true);Check(run.Land(1,true),"first landing");Check(!run.Land(1,true)&&!run.Land(3,false),"duplicates/skips do not invent splits");
  for(int step=2;step<=12;step++){run.Advance(1,true);run.Capture(step,step,0);Check(run.Land(step,step%2==0),"successive landing");}
  var result=run.Complete(When(),12,12,0);Check(result!=null&&result.Splits.Length==12&&result.Seconds==12,"full run records exact12splits/active duration");
  for(int i=0;i<12;i++)Check(result.Splits[i]==i+1,"splits are measured active seconds");
  Check(run.Complete(When(),12,12,0)==null,"repeat portal cannot create another terminal result");
  Check(result.Perfects==7&&result.Falls==0,"actual event counters retained");
 }
 static void GuideInterpolationBoundsAndRouteIdentity()
 {
  var result=Finish(Key(),4);var guide=new MasteryGuide(result.Key,result.Samples);
  var middle=guide.At(.5,Key());Check(Math.Abs(middle.X-.5)<1e-9&&Math.Abs(middle.Y-.5)<1e-9,"interpolated actual recorded path");
  Check(guide.At(-3,Key()).X==0&&guide.At(900,Key()).X==12,"guide clamps endpoints");
  Reject(()=>guide.At(1,new MasteryKey(0,0,new string('b',64))),"changed route never replays incompatible guide");
  Reject(()=>new MasteryGuide(Key(),new[]{new MasterySample(0,0,0,0),new MasterySample(0,1,1,1)}),"duplicate sample times");
  Reject(()=>new MasteryGuide(Key(),new[]{new MasterySample(0,0,0,0),new MasterySample(1,double.NaN,0,0)}),"nonfinite pose");
  var run=new MasteryTrial(Key(),ID(5),0,false);run.Advance(0,true);Reject(()=>run.Capture(10001,0,0),"out-of-bounds pose");
 }
 static void LongAttemptKeepsTimeWithoutPartialGuide()
 {
  var run=new MasteryTrial(Key(),ID(6),0,false);run.Advance(0,true);run.Capture(0,0,0);
  for(int frame=0;frame<2000;frame++){run.Advance(.25,true);run.Capture(frame*.01,0,0);}
  for(int step=1;step<=12;step++){run.Advance(.1,true);run.Land(step,false);}
  var result=run.Complete(When(),12,12,0);
  Check(result!=null&&result.Seconds>500&&result.Samples.Length==0,"over-budget trace is absent, not misleadingly truncated");
  Check(result.Splits.Length==12,"time and split facts survive sample budget");
 }
 static void IndependentBookCleanPBHistoryAndRetries()
 {
  var original=MasteryBook.Empty;var book=original;
  for(int i=0;i<6;i++)book=MasteryBookRules.Record(book,Finish(Key(),10+i,6-i,false,i));
  var entry=MasteryBookRules.Find(book,Key());Check(original.Entries.Length==0,"recording never mutates prior book");
  Check(entry.History.Length==5&&entry.TotalAttempts==6&&entry.Best.Seconds==12&&entry.ImprovementCount==5,"bounded recent history and strict improvements independent of stars");
  var tied=Finish(Key(),20,1,false,6);book=MasteryBookRules.Record(book,tied);Check(MasteryBookRules.Find(book,Key()).Best.ID==ID(15),"tie keeps original best and guide");
  var revision=book.Revision;book=MasteryBookRules.Record(book,tied);Check(book.Revision==revision&&MasteryBookRules.Find(book,Key()).TotalAttempts==7,"same retained attempt retry is idempotent");
  Reject(()=>MasteryBookRules.Record(book,Finish(Key(),20,2,false,6)),"changed payload cannot reuse recorded ID");
  book=MasteryBookRules.Record(book,Finish(Key(),21,.1,true,7));entry=MasteryBookRules.Find(book,Key());
  Check(entry.Best.Seconds==12&&entry.History[entry.History.Length-1].Falls==1,"faster fallen attempt is factual history, not clean PB");
  var other=new MasteryKey(0,0,new string('b',64));book=MasteryBookRules.Record(book,Finish(other,22,1,false,8));Check(book.Entries.Length==2,"different actual route signature keeps independent records");
  var result=Finish(Key(1,0),23,1,false,9);book=MasteryBookRules.Record(book,result);result.Splits[0]=999;
  Check(MasteryBookRules.Find(book,Key(1,0)).Best.Splits[0]==1,"archive owns copied immutable attempt arrays");
 }
 static void GlobalGuideBudgetPreservesAllBestStatistics()
 {
  var book=MasteryBook.Empty;
  for(int i=0;i<21;i++)book=MasteryBookRules.Record(book,Finish(Key(i/10,i%10),100+i,1,false,i));
  int guides=0,samples=0;foreach(var entry in book.Entries){Check(entry.Best!=null&&entry.History.Length==1,"pruning guide keeps PB/history");if(entry.Best.Samples.Length>0)guides++;samples+=entry.Best.Samples.Length;}
  Check(book.Entries.Length==21&&guides<=20&&samples<=30000,"bounded global guide memory, all course facts retained");
  Check(MasteryBookRules.Find(book,Key(0,0)).Best.Samples.Length==0,"oldest guide expires independently of PB");
  var enabled=MasteryBookRules.SetGuideEnabled(book,true);Check(enabled.GuideEnabled&&!book.GuideEnabled,"guide preference separate immutable save");
  enabled.Schema=99;Reject(()=>MasteryBookRules.Validate(enabled),"future archive schema cannot be normalized/reset");
 }
 public static int Main()
 {
  ClockAndEligibility();OrderedSplitsAndOneTerminalResult();GuideInterpolationBoundsAndRouteIdentity();LongAttemptKeepsTimeWithoutPartialGuide();IndependentBookCleanPBHistoryAndRetries();GlobalGuideBudgetPreservesAllBestStatistics();
  Console.WriteLine("RISING MASTERY CORE PASS checks="+checks);return 0;
 }
}
