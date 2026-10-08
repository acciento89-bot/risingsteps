using UnityEngine;
using UnityEngine.EventSystems;

namespace Kamilunavo.RisingSteps.CameraSystem
{
    public sealed class OrbitCamera:MonoBehaviour
    {
        public Transform Target;public float Distance=7.2f;public float Height=2.7f;public float Sensitivity=.15f;private float _yaw;private float _pitch=12;private int _finger=-1;private Vector2 _last;
        private void LateUpdate(){if(Target==null)return;var motor=Target.GetComponent<Kamilunavo.RisingSteps.Gameplay.PlayerMotor>();if(motor!=null&&motor.Paused)_finger=-1;else Read();var rot=Quaternion.Euler(_pitch,_yaw,0);var focus=Target.position+Vector3.up*Height;var desired=focus-rot*Vector3.forward*Distance;transform.position=Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-12*Time.deltaTime));transform.rotation=Quaternion.LookRotation(focus-transform.position);}
        private void Read(){if(UnityEngine.Input.GetMouseButton(1)){_yaw+=UnityEngine.Input.GetAxis("Mouse X")*3;_pitch-=UnityEngine.Input.GetAxis("Mouse Y")*2;}for(var i=0;i<UnityEngine.Input.touchCount;i++){var t=UnityEngine.Input.GetTouch(i);if(_finger==-1&&t.phase==TouchPhase.Began&&!EventSystem.current.IsPointerOverGameObject(t.fingerId)){_finger=t.fingerId;_last=t.position;}if(t.fingerId!=_finger)continue;if(t.phase==TouchPhase.Moved){var d=t.position-_last;_yaw+=d.x*Sensitivity;_pitch-=d.y*Sensitivity;_last=t.position;}if(t.phase is TouchPhase.Ended or TouchPhase.Canceled)_finger=-1;}if(UnityEngine.Input.touchCount==0)_finger=-1;_pitch=Mathf.Clamp(_pitch,-8,42);}
    }
}
