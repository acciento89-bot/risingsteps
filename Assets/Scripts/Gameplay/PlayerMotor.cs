using UnityEngine;
using Kamilunavo.RisingSteps.Input;

namespace Kamilunavo.RisingSteps.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor:MonoBehaviour
    {
        public VirtualJoystick Joystick;public PressButton Jump;public Transform CameraTransform;public RisingCourse Course;public float Speed=5.8f;public float JumpSpeed=8.4f;public float Gravity=22f;private CharacterController _cc;private float _vertical;
        private void Awake()=>_cc=GetComponent<CharacterController>();
        private void Update(){var touch=Joystick!=null?Joystick.Value:Vector2.zero;var keys=new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"),UnityEngine.Input.GetAxisRaw("Vertical"));var input=touch.sqrMagnitude>.01f?touch:Vector2.ClampMagnitude(keys,1);var f=CameraTransform!=null?CameraTransform.forward:Vector3.forward;var r=CameraTransform!=null?CameraTransform.right:Vector3.right;f.y=0;r.y=0;f.Normalize();r.Normalize();var planar=f*input.y+r*input.x;if(planar.sqrMagnitude>.001f)transform.forward=Vector3.Slerp(transform.forward,planar.normalized,1-Mathf.Exp(-14*Time.deltaTime));if(_cc.isGrounded&&_vertical<0)_vertical=-2;var jump=UnityEngine.Input.GetKeyDown(KeyCode.Space)||(Jump!=null&&Jump.Consume());if(_cc.isGrounded&&jump)_vertical=JumpSpeed;_vertical-=Gravity*Time.deltaTime;var v=planar*Speed;v.y=_vertical;_cc.Move(v*Time.deltaTime);}
        private void OnControllerColliderHit(ControllerColliderHit hit){if(hit.normal.y<.45f)return;var step=hit.collider.GetComponent<StepMarker>();if(step!=null)Course?.Land(step);}
    }
}
