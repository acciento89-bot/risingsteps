using System;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Core
{
 [Serializable] public sealed class RisingProfile
 {
  public int Schema=1,Crystals,Realm,Step,UnlockedRealm,Style,Falls,Perfects,DailyStreak,BestDailyStreak,BestPerfects;
  public bool TutorialDone;
  public float Elapsed; public bool Completed,Challenge,Sound=true,Haptics=true,ReducedMotion,HighContrast;
  public string Language="de",DailyDay="",ChallengeDay="",RunDay="";
  public Kamilunavo.RisingSteps.Monetization.CommerceProfile Commerce=new();public Kamilunavo.RisingSteps.Monetization.RewardProfile Rewards=new();
  public bool[] Styles={true,false,false,false,false,false,false,false};public int[] Stars=new int[3];
  public void Normalize(){Crystals=Mathf.Clamp(Crystals,0,100000000);UnlockedRealm=Mathf.Clamp(UnlockedRealm,0,2);Realm=Mathf.Clamp(Realm,0,UnlockedRealm);Step=Mathf.Clamp(Step,0,12);Falls=Mathf.Max(0,Falls);Perfects=Mathf.Clamp(Perfects,0,12);Elapsed=float.IsNaN(Elapsed)||float.IsInfinity(Elapsed)?0:Mathf.Clamp(Elapsed,0,86400);BestDailyStreak=Mathf.Clamp(BestDailyStreak,0,7);BestPerfects=Mathf.Clamp(BestPerfects,0,12);DailyStreak=Mathf.Clamp(DailyStreak,0,7);
   if(Styles==null||Styles.Length!=8){var old=Styles;Styles=new bool[8];if(old!=null)Array.Copy(old,Styles,Math.Min(old.Length,8));}Styles[0]=true;
   if(Stars==null||Stars.Length!=3){var old=Stars;Stars=new int[3];if(old!=null)Array.Copy(old,Stars,Math.Min(old.Length,3));}for(int i=0;i<3;i++)Stars[i]=Mathf.Clamp(Stars[i],0,3);
   Kamilunavo.RisingSteps.Monetization.CommerceRules.Normalize(this);Style=Mathf.Clamp(Style,0,7);if(!Styles[Style])Style=0;Language=Language=="en"?"en":"de";DailyDay??="";ChallengeDay??="";RunDay??="";Completed=Completed&&Step==12;
  }
 }
}
