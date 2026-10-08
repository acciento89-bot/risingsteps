using System;
using System.Globalization;
namespace Kamilunavo.RisingSteps.Core
{
 public static class RisingRules
 {
  public static string Day(DateTime date)=>date.ToUniversalTime().ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
  public static bool ClaimDaily(RisingProfile p,DateTime now){string day=Day(now);if(string.CompareOrdinal(day,p.DailyDay)<=0)return false;
   bool successive=DateTime.TryParseExact(p.DailyDay,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.AssumeUniversal,out var old)&&old.Date.AddDays(1)==now.ToUniversalTime().Date;
   p.DailyStreak=successive?Math.Min(7,p.DailyStreak+1):1;p.BestDailyStreak=Math.Max(p.BestDailyStreak,p.DailyStreak);p.DailyDay=day;p.Crystals+=100+10*(p.DailyStreak-1);return true;}
  public static bool SelectStyle(RisingProfile p,int style){if(style<0||style>=4)return false;int[] price={0,75,150,250};if(!p.Styles[style]){if(p.Crystals<price[style])return false;p.Crystals-=price[style];p.Styles[style]=true;}p.Style=style;return true;}
  public static bool Land(RisingProfile p,int step,bool perfect){if(p.Completed||step!=p.Step+1||step>12)return false;p.Step=step;p.Crystals+=10+2*step+(perfect?5:0);if(perfect)p.Perfects++;p.BestPerfects=Math.Max(p.BestPerfects,p.Perfects);return true;}
  public static int Complete(RisingProfile p,DateTime now){if(p.Step!=12)return 0;int stars=p.Falls>0?1:p.Elapsed<=180?3:2;if(p.Completed)return stars;p.Completed=true;
   if(p.Challenge){string day=Day(now);if(p.RunDay==day&&p.ChallengeDay!=day&&p.Perfects>=8){p.ChallengeDay=day;p.Crystals+=75;}}
   else{p.Stars[p.Realm]=Math.Max(p.Stars[p.Realm],stars);p.UnlockedRealm=Math.Max(p.UnlockedRealm,Math.Min(2,p.Realm+1));}return stars;}
 }
}
