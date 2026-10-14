#if UNITY_EDITOR
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Gameplay;
public static class RisingCampaignValidation
{
 static void Check(bool value,string name){if(!value)throw new Exception("RISING_CAMPAIGN: "+name);}
 public static void Validate()
 {
  var type=typeof(RisingRules).Assembly.GetType("Kamilunavo.RisingSteps.Core.RisingCampaign");Check(type!=null,"campaign has100level definitions rather than three single routes");
  Check((int)type.GetField("LevelsPerRealm").GetRawConstantValue()==10,"ten levels per realm");
  Check((int)type.GetField("RealmCount").GetRawConstantValue()==10&&(int)type.GetField("LevelCount").GetRawConstantValue()==100,"ten realms and100independent levels");
  var pointsMethod=typeof(CoursePatterns).GetMethod("LevelPoints");Check(pointsMethod!=null,"distinct campaign routes exist");
  var levelField=typeof(RisingProfile).GetField("Level");var starsField=typeof(RisingProfile).GetField("LevelStars");Check(levelField!=null&&starsField!=null,"save records current level and independent stars");
  var signatures=new HashSet<string>();
  for(int realm=0;realm<10;realm++)for(int level=0;level<10;level++){
   var points=(Vector3[])pointsMethod.Invoke(null,new object[]{realm,level});Check(points.Length==13,"each level retains twelve physical jumps");string signature="";
   for(int i=1;i<points.Length;i++){var delta=points[i]-points[i-1];float flight=(9.2f+Mathf.Sqrt(9.2f*9.2f-44*delta.y))/22;float distance=new Vector2(delta.x,delta.z).magnitude;
    // A genuinely separated square roof can be approached from its safe edge.
    // Account for the actual direction-dependent square support at both ends,
    // reserving0.50m per roof for the controller radius/skin and landing margin.
    float support=realm==2?(CoursePatterns.TempleRoofWidth(i-1)+CoursePatterns.TempleRoofWidth(i))*.5f*distance/Mathf.Max(Mathf.Abs(delta.x),Mathf.Abs(delta.z))-1:2.2f;
    Check(delta.y>0&&delta.y<1.3f&&distance-support<6.2f*flight-.35f,"jump reachability with real motor limits");signature+=points[i].ToString("F4");}
   Check(signatures.Add(signature),"all hundred routes differ");
   if(level==0&&realm<3){var seeded=CoursePatterns.Points(realm,260906+realm);for(int i=0;i<13;i++)Check(points[i]==seeded[i],"first course retains its seeded checkpoint indexing; Temple forward spacing intentionally reflows");}
  }
  var oldProfile=RisingSave.Parse("{\"Schema\":1,\"Crystals\":777,\"Realm\":2,\"UnlockedRealm\":2,\"Step\":4,\"Stars\":[3,2,1],\"Styles\":[true,true,false,false,false,false,false,false],\"Style\":1}");
  Check(oldProfile.Schema==3&&oldProfile.Crystals==777&&oldProfile.Realm==2&&oldProfile.Step==4&&oldProfile.Style==1,"old save migrates without resetting current run or wallet/style");
  var scores=(int[])starsField.GetValue(oldProfile);Check(scores.Length==100&&scores[0]==3&&scores[10]==2&&scores[20]==1&&scores[1]==0,"migration awards only historical first routes");
  var fresh=new RisingProfile();fresh.Normalize();var unlocked=type.GetMethod("IsUnlocked");Check((bool)unlocked.Invoke(null,new object[]{fresh,0,0})&&!(bool)unlocked.Invoke(null,new object[]{fresh,0,1}),"new profile starts with level1 only");
  fresh.Step=12;fresh.Elapsed=30;fresh.Perfects=12;RisingRules.Complete(fresh,DateTime.UtcNow);int wallet=fresh.Crystals;RisingRules.Complete(fresh,DateTime.UtcNow);Check(fresh.Crystals==wallet&&((int[])starsField.GetValue(fresh))[0]==3&&(bool)unlocked.Invoke(null,new object[]{fresh,0,1}),"completion persists independent stars and next level without duplicate reward");
  Check(fresh.UnlockedRealm==0,"first level does not bypass nine routes in the realm");
  for(int realm=0;realm<10;realm++)for(int level=0;level<10;level++){Check(RisingCampaign.IsUnlocked(fresh,realm,level),"ordered100level unlock");fresh.Realm=realm;fresh.Level=level;fresh.Step=12;fresh.Completed=false;fresh.Perfects=12;fresh.Elapsed=30;RisingRules.Complete(fresh,DateTime.UtcNow);Check(fresh.UnlockedRealm==Math.Min(9,level==9?realm+1:realm),"next realm unlocks only at its tenth completed route");}
  Check(!RisingCampaign.IsUnlocked(fresh,10,0)&&!RisingCampaign.IsUnlocked(fresh,-1,0)&&!RisingCampaign.IsUnlocked(fresh,0,10),"invalid realm and level rejected");
  var schema2=new RisingProfile{Schema=2,Realm=2,UnlockedRealm=2,Level=5,Step=6,Crystals=321,LevelStars=new int[30],Stars=new[]{3,2,1}};for(int i=0;i<30;i++)schema2.LevelStars[i]=(i%3)+1;
  var expanded=RisingSave.Parse(JsonUtility.ToJson(schema2));Check(expanded.Schema==3&&expanded.Realm==2&&expanded.Level==5&&expanded.Step==6&&expanded.Crystals==321&&expanded.LevelStars.Length==100&&expanded.Stars.Length==10,"schema2 expands100without resetting an existing run");for(int i=0;i<100;i++)Check(expanded.LevelStars[i]==(i<30?(i%3)+1:0),"schema2 retains exact first30stars without gifting new scores");
  ValidateCompletedSchema2Migration();
  var restored=RisingSave.Parse(JsonUtility.ToJson(oldProfile));Check(restored.Crystals==777&&((int[])starsField.GetValue(restored))[20]==1,"new save round trip retains progress");
  string priorKey=RisingSave.QaKey;const string key="kamilunavo.risingsteps.qa.campaign.future";string raw=PlayerPrefs.GetString(key,"");try{RisingSave.QaKey=key;string future="{\"Schema\":99,\"Crystals\":999}";PlayerPrefs.SetString(key,future);RisingSave.Load();Check(!RisingSave.Writable,"future schema remains read only");bool blocked=false;try{RisingSave.Save(new RisingProfile());}catch(InvalidOperationException){blocked=true;}Check(blocked&&PlayerPrefs.GetString(key)==future,"future raw payload cannot be overwritten");}finally{if(string.IsNullOrEmpty(raw))PlayerPrefs.DeleteKey(key);else PlayerPrefs.SetString(key,raw);RisingSave.QaKey=priorKey;RisingSave.Load();}
  Debug.Log("RISING_CAMPAIGN_PASS 100 unique reachable levels, migration, independent stars, unlocks, idempotency");
 }
 public static void ValidateCompletedSchema2Migration()
 {
  var legacy=new RisingProfile{Schema=2,Realm=2,Level=9,Step=12,Completed=true,UnlockedRealm=2,Crystals=1234,Perfects=12,Elapsed=30,Style=1,Styles=new[]{true,true,false,false,false,false,false,false},Stars=new[]{3,3,3},LevelStars=new int[30]};
  for(int i=0;i<30;i++)legacy.LevelStars[i]=(i%3)+1;
  var migrated=RisingSave.Parse(JsonUtility.ToJson(legacy));
  Check(migrated.Schema==3&&migrated.UnlockedRealm==3&&RisingCampaign.IsUnlocked(migrated,3,0),"earned Temple level10 unlocks Jungle during schema2 migration without replay");
  Check(migrated.Realm==2&&migrated.Level==9&&migrated.Step==12&&migrated.Completed&&migrated.Perfects==12&&migrated.Elapsed==30&&migrated.Crystals==1234&&migrated.Style==1&&migrated.Styles[1],"completed schema2 migration preserves current run, wallet and selected style");
  for(int i=0;i<100;i++)Check(migrated.LevelStars[i]==(i<30?(i%3)+1:0),"completed schema2 migration preserves every old score and awards no new score");
  Check(!RisingCampaign.IsUnlocked(migrated,3,1)&&!RisingCampaign.IsUnlocked(migrated,4,0),"migration unlocks only earned Jungle entry");
  RisingRules.Complete(migrated,DateTime.UtcNow);
  var restored=RisingSave.Parse(JsonUtility.ToJson(migrated));
  Check(restored.UnlockedRealm==3&&restored.Completed&&restored.Crystals==1234&&restored.LevelStars[29]==legacy.LevelStars[29],"completed migration and idempotent completion survive save round trip without reward");
  legacy.LevelStars[29]=0;legacy.Step=7;legacy.Completed=false;
  var unfinished=RisingSave.Parse(JsonUtility.ToJson(legacy));
  Check(unfinished.Schema==3&&unfinished.UnlockedRealm==2&&!RisingCampaign.IsUnlocked(unfinished,3,0)&&unfinished.Realm==2&&unfinished.Level==9&&unfinished.Step==7&&unfinished.Crystals==1234,"unfinished schema2 retains realm2 cap and its current checkpoint");
  // A completion flag alone is not proof that Temple's tenth campaign route was earned.
  legacy.Step=12;legacy.Completed=true;legacy.Challenge=true;
  var challenge=RisingSave.Parse(JsonUtility.ToJson(legacy));
  Check(challenge.UnlockedRealm==2&&!RisingCampaign.IsUnlocked(challenge,3,0),"completed daily challenge cannot grant the campaign migration unlock");
  Debug.Log("RISING_COMPLETED_SCHEMA2_MIGRATION_PASS earned Jungle unlock, original30 scores and unfinished cap");
 }
}
#endif
