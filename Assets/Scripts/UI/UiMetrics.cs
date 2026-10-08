using UnityEngine;
namespace Kamilunavo.RisingSteps.UI
{
 public static class UiMetrics
 {
#if UNITY_IOS && !UNITY_EDITOR
 [System.Runtime.InteropServices.DllImport("__Internal")]private static extern float RSScreenScale();
#endif
 public static float PointScale{get{
#if UNITY_IOS && !UNITY_EDITOR
 return Mathf.Max(1,RSScreenScale());
#elif UNITY_ANDROID && !UNITY_EDITOR
 using var unity=new AndroidJavaClass("com.unity3d.player.UnityPlayer");using var activity=unity.GetStatic<AndroidJavaObject>("currentActivity");using var resources=activity.Call<AndroidJavaObject>("getResources");using var metrics=resources.Call<AndroidJavaObject>("getDisplayMetrics");return Mathf.Max(1,metrics.Get<float>("density"));
#else
 return 1;
#endif
 }}
 public static Rect ScreenRect(RectTransform rect){var c=new Vector3[4];rect.GetWorldCorners(c);Vector2 a=RectTransformUtility.WorldToScreenPoint(null,c[0]),b=RectTransformUtility.WorldToScreenPoint(null,c[2]);return Rect.MinMaxRect(a.x,a.y,b.x,b.y);}
 }
}
