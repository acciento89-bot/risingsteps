#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Editor
{
    public static class CommerceConfiguration
    {
        public static void Configure()
        {
#if UNITY_ANDROID
            // Batch builds do not reliably execute the package's delayed default-store setup.
            // Persist the actual store selector before resources are collected for the player.
            UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.GooglePlay);
            var billing=Resources.Load<TextAsset>("BillingMode");
            if(billing==null||!billing.text.Contains("\"androidStore\":\"GooglePlay\""))
                throw new InvalidOperationException("Android release requires a serialized Google Play BillingMode resource");
            Debug.Log("ANDROID_REAL_STORE_PASS GooglePlay selected; no fake-store fallback");
#endif
            var type=Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor",true);
            var asset=(ScriptableObject)type.GetMethod("LoadInstance",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,null);
            var settings=new SerializedObject(asset);
            settings.FindProperty("adMobIOSAppId").stringValue="ca-app-pub-8944085355624754~8613336153";
            settings.FindProperty("adMobAndroidAppId").stringValue="ca-app-pub-8944085355624754~6808290736";
            settings.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(asset);AssetDatabase.SaveAssets();
            CommerceValidation.Validate();
            Debug.Log("AdMob app IDs configured; internal test ads="+Monetization.MonetizationConfig.Load().InternalTestAds);
        }
        public static void ValidateRelease(BuildTarget target)
        {
            var config=Monetization.MonetizationConfig.Load();
            if(config.InternalTestAds)throw new InvalidOperationException("Release cannot ship internal test advertising.");
            string[] units=target==BuildTarget.iOS?new[]{config.IosRewarded,config.IosInterstitial}:new[]{config.AndroidRewarded,config.AndroidInterstitial};
            foreach(string unit in units)if(string.IsNullOrWhiteSpace(unit)||!System.Text.RegularExpressions.Regex.IsMatch(unit,@"^ca-app-pub-8944085355624754/\d{10}$"))throw new InvalidOperationException("Missing or unowned production ad unit for "+target);
            if(Monetization.MonetizationConfig.NonPersonalizedRequest().Extras["npa"]!="1")throw new InvalidOperationException("Release ad requests must remain non-personalized.");
            Debug.Log("RISING_RELEASE_COMMERCE_PASS owned production ad units / test mode off / non-personalized requests");
        }
    }
}
#endif
