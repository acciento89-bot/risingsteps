using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.UI
{
    public static class UiFactory
    {
        private static Font _font;
        public static Font Font=>_font!=null?_font:(_font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
        public static Canvas Canvas(){var go=new GameObject("MobileCanvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var c=go.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;var s=go.GetComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1080,1920);s.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;s.matchWidthOrHeight=.5f;return c;}
        public static RectTransform Panel(Transform p,string n,Color c,Vector2 min,Vector2 max){var go=new GameObject(n,typeof(RectTransform),typeof(Image));go.transform.SetParent(p,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;go.GetComponent<Image>().color=c;return r;}
        public static Text Label(Transform p,string n,string text,int size,Vector2 min,Vector2 max,TextAnchor align,Color color,FontStyle style=FontStyle.Normal){var go=new GameObject(n,typeof(RectTransform),typeof(Text));go.transform.SetParent(p,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;var t=go.GetComponent<Text>();t.font=Font;t.text=text;t.fontSize=size;t.alignment=align;t.color=color;t.fontStyle=style;t.resizeTextForBestFit=true;t.resizeTextMinSize=12;t.resizeTextMaxSize=size;return t;}
        public static Button Button(Transform p,string n,string label,Color bg,Color fg,Vector2 min,Vector2 max,UnityAction action){var r=Panel(p,n,bg,min,max);var b=r.gameObject.AddComponent<Button>();if(action!=null)b.onClick.AddListener(action);Label(r,"Label",label,34,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,fg,FontStyle.Bold);return b;}
        public static Image Progress(Transform p,Vector2 min,Vector2 max,Color track,Color fill){var tr=Panel(p,"Track",track,min,max);var fr=Panel(tr,"Fill",fill,Vector2.zero,Vector2.one);var i=fr.GetComponent<Image>();i.type=Image.Type.Filled;i.fillMethod=Image.FillMethod.Horizontal;i.fillAmount=0;return i;}
    }
}
