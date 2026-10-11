using System;
using System.Collections.Generic;
using Kamilunavo.RisingSteps.Core;
namespace Kamilunavo.RisingSteps.Monetization
{
 // Session-only pacing. No saved scores, currency or rewards belong to an ad opportunity.
 public sealed class InterstitialPolicy
 {
  readonly HashSet<string> completedRuns=new HashSet<string>();
  readonly double sessionStart;double lastShown;bool hasShown;int sinceShown;
  public InterstitialPolicy(double sessionStart){this.sessionStart=sessionStart;}
  public bool RecordCompletion(RisingProfile profile,string runToken,bool genuineTransition,bool tutorialActive)
  {
   if(profile==null||!genuineTransition||tutorialActive||!profile.TutorialDone||profile.Challenge||!profile.Completed||profile.Step!=12||string.IsNullOrEmpty(runToken)||!completedRuns.Add(runToken))return false;
   sinceShown++;return true;
  }
  public bool Eligible(RisingProfile profile,double now,bool tutorialActive)
  {
   return profile!=null&&!tutorialActive&&profile.TutorialDone&&!profile.Challenge&&profile.Completed&&profile.Step==12&&(profile.Commerce==null||(profile.Commerce.Entitlements&4)==0)&&sinceShown>=3&&!double.IsNaN(now)&&!double.IsInfinity(now)&&now-(hasShown?lastShown:sessionStart)>=180;
  }
  public void DelayAfterVoluntaryAd(double now)
  {
   if(double.IsNaN(now)||double.IsInfinity(now)||now<sessionStart||(hasShown&&now<lastShown))return;
   hasShown=true;lastShown=now;
  }
  public void MarkShown(double now){if(double.IsNaN(now)||double.IsInfinity(now))return;hasShown=true;lastShown=now;sinceShown=0;}
 }
}
