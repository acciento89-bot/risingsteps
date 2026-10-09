using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Gameplay;

namespace Kamilunavo.RisingSteps.UI
{
    public sealed class RisingTutorial : MonoBehaviour
    {
        private RisingCourse _course;private RisingHud _hud;private PlayerMotor _motor;private RectTransform _safe,_panel;private Text _instruction;
        private Vector3 _origin;private int _startStep,_stage;private Vector2 _size;private bool _active;
        public bool Active=>_active;
        public int Stage=>_stage;
        public void Initialize(RisingCourse course,RisingHud hud,RectTransform safe)
        {
            _course=course;_hud=hud;_safe=safe;_motor=course.Player.GetComponent<PlayerMotor>();
            if(_motor!=null)_motor.Jumped+=OnJump;
            _course.Landed+=OnLanding;_course.RunStarted+=OnRunStarted;
        }
        public void Begin()
        {
            // The tutorial continues the current checkpoint. Replaying never resets
            // a run, crystal balance, unlocked realm, style or completion rights.
            if(_course.Profile.Completed){_hud.ShowHome();return;}
            _origin=_course.Player.position;_startStep=_course.Height;_stage=0;_active=true;
            if(_panel==null){_panel=UiFactory.Panel(_safe,"TutorialTray",new Color(.97f,.96f,.86f,.98f),Vector2.zero,Vector2.one);
                _instruction=UiFactory.Label(_panel,"Instruction","",16,new Vector2(.06f,.52f),new Vector2(.94f,.98f),TextAnchor.MiddleCenter,new Color(.10f,.23f,.26f),FontStyle.Bold);
                var skip=UiFactory.Button(_panel,"SkipTutorial",_hud.T("ÜBERSPRINGEN","SKIP"),new Color(.76f,.85f,.76f),Color.white,Vector2.zero,Vector2.one,Finish);var rect=(RectTransform)skip.transform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,0);rect.pivot=new Vector2(.5f,0);rect.anchoredPosition=new Vector2(0,5);rect.sizeDelta=new Vector2(108,UiMetrics.TargetSize(_safe,49));}
            _panel.gameObject.SetActive(true);Layout();Refresh();
        }
        private void Layout()
        {
            if(_panel==null)return;float w=_safe.rect.width,h=_safe.rect.height;
            var joystick=(RectTransform)_hud.Joystick.transform;var jump=(RectTransform)_hud.Jump.transform;
            float left=joystick.anchoredPosition.x+joystick.rect.width+8,right=jump.anchoredPosition.x-8;
            // Keep normal-phone guidance in the unused area between the two thumbs.
            // A narrow split-screen pane instead gets a compact edge tray above them.
            float width=right-left;bool narrow=width<122;
            if(narrow){left=12;width=w-24;}
            _panel.anchorMin=_panel.anchorMax=Vector2.zero;_panel.pivot=Vector2.zero;_panel.anchoredPosition=new Vector2(left,narrow?Mathf.Max(joystick.rect.height,jump.rect.height)+30:18);_panel.sizeDelta=new Vector2(width,112);
            _size=_safe.rect.size;
        }
        private void Update()
        {
            if(!_active||_panel==null)return;
            bool visible=!_hud.ModalOpen&&!_course.Paused;
            if(_panel.gameObject.activeSelf!=visible)_panel.gameObject.SetActive(visible);
            if(!visible)return;
            if(_size!=_safe.rect.size)Layout();
            if(_stage==0&&_hud.Joystick.Value.sqrMagnitude>.08f){var delta=_course.Player.position-_origin;delta.y=0;if(delta.sqrMagnitude>.0625f){_stage=1;Refresh();}}
        }
        private void OnJump(){if(!_active||_stage!=1)return;_stage=2;Refresh();}
        private void OnLanding(bool perfect){if(_active&&_stage==2&&_course.Height>_startStep){Finish();}}
        private void OnRunStarted(){if(_active){_origin=_course.Player.position;_startStep=_course.Height;_stage=0;Refresh();}}
        private void Refresh(){if(_instruction==null)return;_instruction.text=_stage==0?_hud.T("1 / 3 · JOYSTICK\nBewege dich nach vorn","1 / 3 · JOYSTICK\nMove forward"):_stage==1?_hud.T("2 / 3 · SPRUNG\nHalte vorwärts + springe","2 / 3 · JUMP\nHold forward + jump"):_hud.T("3 / 3 · LANDEN\nErreiche die nächste Insel","3 / 3 · LAND\nReach the next island");}
        public void Finish(){_active=false;if(_panel!=null)_panel.gameObject.SetActive(false);_course.Profile.TutorialDone=true;_course.Save();}
        private void OnDestroy(){if(_motor!=null)_motor.Jumped-=OnJump;if(_course!=null){_course.Landed-=OnLanding;_course.RunStarted-=OnRunStarted;}if(_panel!=null)Destroy(_panel.gameObject);}
    }
}
