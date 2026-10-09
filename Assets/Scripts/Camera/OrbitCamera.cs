using UnityEngine;

namespace Kamilunavo.RisingSteps.CameraSystem
{
    public sealed class OrbitCamera : MonoBehaviour
    {
        public Transform Target;
        public float Distance=7.2f,Height=2.7f,Sensitivity=.15f;
        public float Yaw=>_yaw;
        private float _yaw,_pitch=12;
        private Kamilunavo.RisingSteps.Gameplay.PlayerMotor _motor;

        // Only the UI-owned View pad may orbit on touch. Ambient fingers never enter
        // this class, so joystick/jump ownership cannot depend on cached UI raycasts.
        public void Orbit(Vector2 delta){if(Target==null||(_motor!=null&&_motor.Paused))return;_yaw+=delta.x*Sensitivity;_pitch=Mathf.Clamp(_pitch-delta.y*Sensitivity,-8,42);}
        public void ResetView(){_yaw=0;_pitch=12;}
        private void LateUpdate()
        {
            if(Target==null)return;
            if(_motor==null)_motor=Target.GetComponent<Kamilunavo.RisingSteps.Gameplay.PlayerMotor>();
            // Unity synthesizes a right mouse button from two touches. A movement
            // finger followed by Jump must never enter the desktop orbit path.
            if(!Application.isMobilePlatform&&UnityEngine.Input.touchCount==0&&
                (_motor==null||!_motor.Paused)&&UnityEngine.Input.GetMouseButton(1))
                Orbit(new Vector2(UnityEngine.Input.GetAxis("Mouse X")*20,UnityEngine.Input.GetAxis("Mouse Y")*(2/Sensitivity)));
            var rotation=Quaternion.Euler(_pitch,_yaw,0);
            var focus=Target.position+Vector3.up*Height;
            var desired=focus-rotation*Vector3.forward*Distance;
            transform.position=Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-12*Time.deltaTime));
            transform.rotation=rotation;
        }
    }
}
