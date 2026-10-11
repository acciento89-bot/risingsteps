using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Core;
namespace Kamilunavo.RisingSteps.Monetization
{
    [Serializable]
    public sealed class CommerceProfile
    {
        public int Entitlements;
        public bool StarterCreditClaimed;
        public List<string> FulfilledTransactions=new();
    }
    public static class CommerceRules
    {
        public const string Starter="com.kamilunavo.risingsteps.starter";
        public const string Collection="com.kamilunavo.risingsteps.skycollection";
        public const string RemoveAds="com.kamilunavo.risingsteps.removeads";
        public static int EntitlementBit(string id)=>id==Starter?1:id==Collection?2:id==RemoveAds?4:0;
        public static bool KnownProduct(string id)=>EntitlementBit(id)!=0;
        public static int PremiumStyles(int entitlements)=>((entitlements&1)!=0?16:0)|((entitlements&2)!=0?224:0);
        public static void Normalize(RisingProfile profile)
        {
            profile.Commerce??=new CommerceProfile();
            profile.Commerce.Entitlements&=7;
            profile.Commerce.FulfilledTransactions??=new List<string>();
            if(profile.Styles==null||profile.Styles.Length!=8){var previous=profile.Styles;profile.Styles=new bool[8];if(previous!=null)Array.Copy(previous,profile.Styles,Math.Min(8,previous.Length));}profile.Styles[0]=true;var mask=PremiumStyles(profile.Commerce.Entitlements);for(int i=4;i<8;i++)profile.Styles[i]=(mask&(1<<i))!=0;
            profile.Style=Mathf.Clamp(profile.Style,0,7);
            if(!profile.Styles[profile.Style])profile.Style=0;
        }
        public static bool ApplyPending(RisingProfile profile,string productId,string transactionId)
        {
            if(!KnownProduct(productId) || string.IsNullOrWhiteSpace(transactionId))return false;
            Normalize(profile);
            var marker=productId+":"+transactionId;
            if(profile.Commerce.FulfilledTransactions.Contains(marker))return false;
            if(productId==Starter)
            {
                profile.Commerce.Entitlements|=1;
                if(!profile.Commerce.StarterCreditClaimed)
                {
                    profile.Crystals=(int)Math.Min(100000000,(long)profile.Crystals+500);
                    profile.Commerce.StarterCreditClaimed=true;
                }
            }
            else profile.Commerce.Entitlements|=EntitlementBit(productId);
            profile.Commerce.FulfilledTransactions.Add(marker);
            Normalize(profile);return true;
        }
        public static bool RestoreEntitlement(RisingProfile profile,string productId)
        {
            if(!KnownProduct(productId))return false;
            Normalize(profile);var before=profile.Commerce.Entitlements;
            profile.Commerce.Entitlements|=EntitlementBit(productId);
            // Confirmed restoration restores cosmetics, never starter currency.
            if(productId==Starter)profile.Commerce.StarterCreditClaimed=true;
            Normalize(profile);return before!=profile.Commerce.Entitlements;
        }
        public static void ReconcileEntitlements(RisingProfile profile,IEnumerable<string> activeProductIds)
        {
            var mask=0;
            foreach(var id in activeProductIds)mask|=EntitlementBit(id);
            Normalize(profile);profile.Commerce.Entitlements=mask;
            if((mask&1)!=0)profile.Commerce.StarterCreditClaimed=true;
            Normalize(profile);
        }
        public static void ReconcileCatalog(RisingProfile profile,IEnumerable<string> activeProductIds,IEnumerable<string> fetchedProductIds)
        {
            Normalize(profile);int known=0,active=0;
            foreach(var id in fetchedProductIds)known|=EntitlementBit(id);
            foreach(var id in activeProductIds)active|=EntitlementBit(id);
            profile.Commerce.Entitlements=(profile.Commerce.Entitlements&~known)|active;
            if((profile.Commerce.Entitlements&1)!=0)profile.Commerce.StarterCreditClaimed=true;
            Normalize(profile);
        }
        public static bool FulfillPending(RisingProfile profile,string productId,string transactionId,Action<RisingProfile> persist)
        {
            if(!KnownProduct(productId) || string.IsNullOrWhiteSpace(transactionId))return false;
            Normalize(profile);
            var previous=JsonUtility.ToJson(profile.Commerce);
            var coins=profile.Crystals;var styles=(bool[])profile.Styles.Clone();var selection=profile.Style;
            ApplyPending(profile,productId,transactionId);
            try { persist(profile);return true; }
            catch
            {
                profile.Commerce=JsonUtility.FromJson<CommerceProfile>(previous);
                profile.Crystals=coins;profile.Styles=styles;profile.Style=selection;
                throw;
            }
        }
    }
}
