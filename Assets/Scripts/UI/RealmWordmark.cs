using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.RisingSteps.UI
{
 public static class RealmWordmark
 {
  // A display crop selects the authored alpha artwork without changing the
  // original PNG. The two-line Text label remains its semantic/fallback title.
  public static RawImage Add(Transform parent,Vector2 min,Vector2 max){
   var frame=new GameObject("Wordmark",typeof(RectTransform));frame.transform.SetParent(parent,false);var rect=(RectTransform)frame.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;
   var image=new GameObject("OriginalLogo",typeof(RectTransform),typeof(CanvasRenderer),typeof(RawImage),typeof(AspectRatioFitter));image.transform.SetParent(frame.transform,false);var at=(RectTransform)image.transform;at.anchorMin=Vector2.zero;at.anchorMax=Vector2.one;at.offsetMin=at.offsetMax=Vector2.zero;
   var art=image.GetComponent<RawImage>();art.texture=Resources.Load<Texture2D>("Art/RisingWordmark-v1");art.uvRect=new Rect(219f/1535f,51f/1025f,1095f/1535f,940f/1025f);art.raycastTarget=false;var aspect=image.GetComponent<AspectRatioFitter>();aspect.aspectMode=AspectRatioFitter.AspectMode.FitInParent;aspect.aspectRatio=1095f/940f;return art;
  }
 }
}
