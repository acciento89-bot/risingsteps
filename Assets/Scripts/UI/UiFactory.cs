using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.UI
{
    public static class UiFactory
    {
        public static bool HighContrast;private static Sprite _white;private static Font _font;
        public static Font Font=>_font!=null?_font:(_font=Resources.Load<Font>("Fonts/Barlow-Regular")??Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
        public static Canvas Canvas(){var go=new GameObject("MobileCanvas",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));var c=go.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;var s=go.GetComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=Screen.width>Screen.height?new Vector2(844,390):new Vector2(390,844);go.AddComponent<CanvasOrientation>();c.additionalShaderChannels|=AdditionalCanvasShaderChannels.TexCoord1;s.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;s.matchWidthOrHeight=.5f;return c;}
        public static RectTransform Panel(Transform p,string n,Color c,Vector2 min,Vector2 max){var go=new GameObject(n,typeof(RectTransform),typeof(RoundedPanel));go.transform.SetParent(p,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;if(HighContrast&&c.a>.1f)c.a=1;go.GetComponent<Image>().color=c;go.GetComponent<Image>().raycastTarget=c.a>.01f;return r;}
        public static Text Label(Transform p,string n,string text,int size,Vector2 min,Vector2 max,TextAnchor align,Color color,FontStyle style=FontStyle.Normal){var go=new GameObject(n,typeof(RectTransform),typeof(Text));go.transform.SetParent(p,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;var t=go.GetComponent<Text>();t.font=style==FontStyle.Bold?(Resources.Load<Font>("Fonts/Barlow-Bold")??Font):Font;t.raycastTarget=false;t.text=text;t.fontSize=size;t.alignment=align;t.color=color;t.fontStyle=style;t.resizeTextForBestFit=true;t.resizeTextMinSize=12;t.resizeTextMaxSize=size;return t;}
        public static Button Button(Transform p,string n,string label,Color bg,Color fg,Vector2 min,Vector2 max,UnityAction action){if(HighContrast&&bg.a>.1f)bg.a=1;var r=Panel(p,n,bg,min,max);if(bg.r*.2126f+bg.g*.7152f+bg.b*.0722f>.48f)fg=new Color(.025f,.07f,.12f);var b=r.gameObject.AddComponent<Button>();if(action!=null)b.onClick.AddListener(action);Label(r,"Label",label,22,Vector2.zero,Vector2.one,TextAnchor.MiddleCenter,fg,FontStyle.Bold);return b;}
        public static Image Progress(Transform p,Vector2 min,Vector2 max,Color track,Color fill){var tr=Panel(p,"Track",track,min,max);var fr=Panel(tr,"Fill",fill,Vector2.zero,Vector2.one);var i=fr.GetComponent<Image>();if(_white==null){var tex=new Texture2D(1,1);tex.SetPixel(0,0,Color.white);tex.Apply();_white=Sprite.Create(tex,new Rect(0,0,1,1),Vector2.one*.5f);}i.sprite=_white;i.type=Image.Type.Filled;i.fillMethod=Image.FillMethod.Horizontal;i.fillAmount=0;return i;}
    }
}
