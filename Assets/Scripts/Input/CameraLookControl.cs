using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.RisingSteps.CameraSystem;

namespace Kamilunavo.RisingSteps.Input
{
    public sealed class CameraLookControl : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler
    {
        public OrbitCamera Camera { get; set; }
        private int _owner=int.MinValue;
        private Vector2 _last;
        public void OnPointerDown(PointerEventData pointer)
        {
            if(_owner!=int.MinValue||Camera==null)return;
            _owner=pointer.pointerId;_last=pointer.position;
        }
        public void OnDrag(PointerEventData pointer)
        {
            if(pointer.pointerId!=_owner)return;
            // Screen pixels would make the same thumb movement rotate 3x faster on
            // Retina displays. Convert the deliberate gesture to logical points.
            Camera?.Orbit((pointer.position-_last)/UI.UiMetrics.PointScale);
            _last=pointer.position;
        }
        public void OnPointerUp(PointerEventData pointer){if(pointer.pointerId==_owner)ResetInput();}
        public void ResetInput()=>_owner=int.MinValue;
        private void OnDisable()=>ResetInput();
    }
}
