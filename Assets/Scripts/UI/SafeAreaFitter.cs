using UnityEngine;
namespace Kamilunavo.RisingSteps.UI
{
 [RequireComponent(typeof(RectTransform))] public sealed class SafeAreaFitter:MonoBehaviour
 {
  private RectTransform _r;private Rect _last=new(-1,-1,-1,-1);private void Awake(){_r=GetComponent<RectTransform>();Apply();}private void Update()=>Apply();
  private void Apply(){if(Screen.width<=0||Screen.height<=0)return;var safe=Screen.safeArea;safe=new Rect(safe.x/Screen.width,safe.y/Screen.height,safe.width/Screen.width,safe.height/Screen.height);if(ReservedRegionProvider.TryGetDivisionRegion(out var region))safe=UsableRegion.Choose(safe,region);if(safe==_last)return;_last=safe;_r.anchorMin=safe.min;_r.anchorMax=safe.max;_r.offsetMin=_r.offsetMax=Vector2.zero;if(UnityEngine.Camera.main!=null)UnityEngine.Camera.main.rect=safe;}
 }
 public static class UsableRegion
 {
  public static Rect Choose(Rect safe,Rect excluded){var blocked=Rect.MinMaxRect(Mathf.Max(safe.xMin,excluded.xMin),Mathf.Max(safe.yMin,excluded.yMin),Mathf.Min(safe.xMax,excluded.xMax),Mathf.Min(safe.yMax,excluded.yMax));if(blocked.width<=0||blocked.height<=0)return safe;
   var candidates=new[]{Rect.MinMaxRect(safe.xMin,safe.yMin,blocked.xMin,safe.yMax),Rect.MinMaxRect(blocked.xMax,safe.yMin,safe.xMax,safe.yMax),Rect.MinMaxRect(safe.xMin,safe.yMin,safe.xMax,blocked.yMin),Rect.MinMaxRect(safe.xMin,blocked.yMax,safe.xMax,safe.yMax)};var best=new Rect();foreach(var c in candidates)if(c.width*c.height>best.width*best.height)best=c;return best.width>0&&best.height>0?best:safe;
  }
 }
}
