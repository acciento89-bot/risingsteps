#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Monetization;
using UnityEngine;
public static class CommerceValidation
{
 static int count;static void Check(bool ok,string label){count++;if(!ok)throw new Exception(label);}
 public static void Validate(){
 var p=new RisingProfile();p.Normalize();Check(p.Styles.Length==8&&p.Styles[0],"free style migration");
 Check(CommerceRules.FulfillPending(p,CommerceRules.Starter,"transaction1",_=>{}),"pending fulfillment");Check(p.Crystals==500&&p.Styles[4],"starter once");
 CommerceRules.FulfillPending(p,CommerceRules.Starter,"transaction1",_=>{});Check(p.Crystals==500,"transaction replay");
 CommerceRules.FulfillPending(p,CommerceRules.Starter,"transaction2",_=>{});Check(p.Crystals==500,"starter bonus never twice");
 p=new RisingProfile();p.Normalize();CommerceRules.RestoreEntitlement(p,CommerceRules.Starter);Check(p.Crystals==0&&p.Styles[4],"confirmed restoration never mints currency");
 CommerceRules.FulfillPending(p,CommerceRules.Starter,"afterrestore",_=>{});Check(p.Crystals==0,"restore plus delayed pending no mint");
 p=new RisingProfile();p.Normalize();try{CommerceRules.FulfillPending(p,CommerceRules.Starter,"failedsave",_=>throw new Exception("disk"));}catch(Exception){}Check(p.Crystals==0&&!p.Styles[4]&&p.Commerce.FulfilledTransactions.Count==0,"failed persistence rolls back atomic fulfillment");
 Check(!CommerceRules.FulfillPending(p,"unknown","x",_=>{})&&!CommerceRules.FulfillPending(p,CommerceRules.Starter,"",_=>{}),"unknown/missing ID rejected");
 CommerceRules.RestoreEntitlement(p,CommerceRules.Collection);p.Style=7;CommerceRules.ReconcileEntitlements(p,new HashSet<string>());Check(!p.Styles[7]&&p.Style==0,"revocation resets premium selection");
 Check(!RisingRules.SelectStyle(p,4),"premium cannot be bought with free currency");p.Crystals=100;Check(RisingRules.SelectStyle(p,1)&&p.Crystals==25,"earned style preserved");
 var now=new DateTime(2026,10,8,0,0,0,DateTimeKind.Utc);p=new RisingProfile();p.Normalize();
 Check(RewardRules.Fulfill(p,"view1",now,_=>{})&&p.Crystals==50,"completed video reward");Check(!RewardRules.Fulfill(p,"view1",now.AddMinutes(2),_=>{}),"video replay no reward");Check(!RewardRules.Fulfill(p,"cooldown",now.AddSeconds(10),_=>{}),"reward cooldown");
 for(int i=1;i<5;i++)Check(RewardRules.Fulfill(p,"view"+(i+1),now.AddMinutes(i*2),_=>{}),"daily reward slot");Check(!RewardRules.Fulfill(p,"overlimit",now.AddHours(1),_=>{}),"five videos per day");Check(RewardRules.Fulfill(p,"nextday",now.AddDays(1),_=>{}),"UTC day resets limit");
 int wallet=p.Crystals;try{RewardRules.Fulfill(p,"bad-save",now.AddDays(1).AddMinutes(2),_=>throw new Exception("disk"));}catch(Exception){}Check(p.Crystals==wallet&&!p.Rewards.Sessions.Contains("bad-save"),"reward persistence rollback");
 var generation=new AdLoadGeneration();var before=generation.Begin();generation.Invalidate();Check(!generation.IsCurrent(before),"privacy invalidates old loads");
 Debug.Log("RISING_COMMERCE_PASS checks="+count);
 }
}
#endif
