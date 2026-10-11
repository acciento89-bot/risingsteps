using System;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Core
{
 [Serializable] public sealed class RisingProfile
 {
  public int Schema=3,Crystals,Realm,Level,Step,UnlockedRealm,Style,Falls,Perfects,DailyStreak,BestDailyStreak,BestPerfects;
  public bool TutorialDone;
  public float Elapsed; public bool Completed,Challenge,Sound=true,Music=true,Haptics=true,ReducedMotion,HighContrast;
  public string Language="de",DailyDay="",ChallengeDay="",RunDay="";
  public Kamilunavo.RisingSteps.Monetization.CommerceProfile Commerce=new();public Kamilunavo.RisingSteps.Monetization.RewardProfile Rewards=new();
  public bool[] Styles={true,false,false,false,false,false,false,false};public int[] Stars=new int[RisingCampaign.RealmCount];public int[] LevelStars=new int[RisingCampaign.LevelCount];
  public void Normalize(){Crystals=Mathf.Clamp(Crystals,0,100000000);UnlockedRealm=Mathf.Clamp(UnlockedRealm,0,RisingCampaign.RealmCount-1);Realm=Mathf.Clamp(Realm,0,UnlockedRealm);Step=Mathf.Clamp(Step,0,12);Falls=Mathf.Max(0,Falls);Perfects=Mathf.Clamp(Perfects,0,12);Elapsed=float.IsNaN(Elapsed)||float.IsInfinity(Elapsed)?0:Mathf.Clamp(Elapsed,0,86400);BestDailyStreak=Mathf.Clamp(BestDailyStreak,0,7);BestPerfects=Mathf.Clamp(BestPerfects,0,12);DailyStreak=Mathf.Clamp(DailyStreak,0,7);
   if(Styles==null||Styles.Length!=8){var old=Styles;Styles=new bool[8];if(old!=null)Array.Copy(old,Styles,Math.Min(old.Length,8));}Styles[0]=true;
   if(Stars==null||Stars.Length!=RisingCampaign.RealmCount){var old=Stars;Stars=new int[RisingCampaign.RealmCount];if(old!=null)Array.Copy(old,Stars,Math.Min(old.Length,RisingCampaign.RealmCount));}for(int i=0;i<RisingCampaign.RealmCount;i++)Stars[i]=Mathf.Clamp(Stars[i],0,3);
   if(LevelStars==null||LevelStars.Length!=RisingCampaign.LevelCount){var old=LevelStars;LevelStars=new int[RisingCampaign.LevelCount];if(old!=null)Array.Copy(old,LevelStars,Math.Min(old.Length,RisingCampaign.LevelCount));}
   // Schema2 ended at Temple level10 (stable score slot29); its earned exit now opens Jungle.
   if(Schema==2&&LevelStars[29]>0)UnlockedRealm=Math.Max(UnlockedRealm,3);
   if(Schema==1){for(int i=0;i<3;i++)LevelStars[i*RisingCampaign.LevelsPerRealm]=Math.Max(LevelStars[i*RisingCampaign.LevelsPerRealm],Stars[i]);Level=0;}if(Schema==1||Schema==2)Schema=3;
   for(int i=0;i<RisingCampaign.LevelCount;i++)LevelStars[i]=Mathf.Clamp(LevelStars[i],0,3);Level=Mathf.Clamp(Level,0,9);if(!RisingCampaign.IsUnlocked(this,Realm,Level))Level=0;
   Kamilunavo.RisingSteps.Monetization.CommerceRules.Normalize(this);Style=Mathf.Clamp(Style,0,7);if(!Styles[Style])Style=0;Language=Language=="en"?"en":"de";DailyDay??="";ChallengeDay??="";RunDay??="";Completed=Completed&&Step==12;
  }
 }
}
