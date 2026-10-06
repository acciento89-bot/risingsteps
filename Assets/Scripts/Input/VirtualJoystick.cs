using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.Input
{
    public sealed class VirtualJoystick:MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler
    {
        public Vector2 Value{get;private set;}public RectTransform Knob{get;set;}private RectTransform _r;
        private void Awake()=>_r=(RectTransform)transform;
        public void OnPointerDown(PointerEventData e)=>Set(e);public void OnDrag(PointerEventData e)=>Set(e);public void OnPointerUp(PointerEventData e){Value=Vector2.zero;if(Knob!=null)Knob.anchoredPosition=Vector2.zero;}
        private void Set(PointerEventData e){if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(_r,e.position,e.pressEventCamera,out var p))return;var radius=Mathf.Min(_r.rect.width,_r.rect.height)*.5f;Value=Vector2.ClampMagnitude(p/Mathf.Max(1,radius),1);if(Knob!=null)Knob.anchoredPosition=Value*radius*.42f;}
        public static VirtualJoystick Create(Transform p,Vector2 min,Vector2 max){var go=new GameObject("Joystick",typeof(RectTransform),typeof(Image),typeof(VirtualJoystick));go.transform.SetParent(p,false);var r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;go.GetComponent<Image>().color=new Color(.03f,.06f,.10f,.62f);var k=new GameObject("Knob",typeof(RectTransform),typeof(Image));k.transform.SetParent(go.transform,false);var kr=k.GetComponent<RectTransform>();kr.anchorMin=new Vector2(.28f,.28f);kr.anchorMax=new Vector2(.72f,.72f);kr.offsetMin=Vector2.zero;kr.offsetMax=Vector2.zero;k.GetComponent<Image>().color=new Color(.82f,.85f,.88f,.86f);var j=go.GetComponent<VirtualJoystick>();j.Knob=kr;return j;}
    }
}
