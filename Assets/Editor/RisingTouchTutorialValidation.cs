#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.CameraSystem;
using Kamilunavo.RisingSteps.Input;
public static class RisingTouchTutorialValidation
{
    static void Check(bool value,string name){if(!value)throw new Exception(name);}
    public static void Validate()
    {
        var cameraObject=new GameObject("CameraOwnershipProbe",typeof(Camera),typeof(OrbitCamera));
        var target=new GameObject("Target");var padObject=new GameObject("ViewOwnershipProbe",typeof(RectTransform),typeof(CameraLookControl));
        try{
            var orbit=cameraObject.GetComponent<OrbitCamera>();orbit.Target=target.transform;var pad=padObject.GetComponent<CameraLookControl>();pad.Camera=orbit;
            var move=new PointerEventData(EventSystem.current){pointerId=11,position=new Vector2(10,30)};pad.OnDrag(move);Check(orbit.Yaw==0,"unclaimed movement finger cannot orbit");
            var look=new PointerEventData(EventSystem.current){pointerId=33,position=new Vector2(100,200)};pad.OnPointerDown(look);
            var jump=new PointerEventData(EventSystem.current){pointerId=22,position=new Vector2(400,200)};pad.OnPointerDown(jump);pad.OnDrag(jump);pad.OnPointerUp(jump);Check(orbit.Yaw==0,"jump finger cannot steal or release View owner");
            look.position+=Vector2.right*40;pad.OnDrag(look);float yaw=orbit.Yaw;Check(yaw>0,"deliberate owner still orbits after foreign finger up");pad.OnPointerUp(look);look.position+=Vector2.right*40;pad.OnDrag(look);Check(orbit.Yaw==yaw,"released View finger no longer orbits");
            pad.OnPointerDown(look);pad.ResetInput();look.position+=Vector2.right*40;pad.OnDrag(look);Check(orbit.Yaw==yaw,"pause/reset invalidates captured View finger");orbit.ResetView();Check(orbit.Yaw==0,"new route returns camera toward forward path");
            var old=RisingSave.Parse("{\"Schema\":1,\"Step\":4,\"Crystals\":321,\"Styles\":[true,true,false,false]}");Check(old.TutorialDone&&old.Crystals==321&&old.Step==4&&old.Styles[1],"old progressed save gets compatible tutorial migration without economy change");
            var fresh=RisingSave.Parse("{\"Schema\":1,\"Step\":0}");Check(!fresh.TutorialDone,"fresh save needs playable tutorial");
            var finished=RisingSave.Parse("{\"Schema\":1,\"TutorialDone\":true,\"Step\":0}");Check(finished.TutorialDone,"skip/tutorial completion persists independently of checkpoint");
            Debug.Log("RISING_TOUCH_TUTORIAL_PASS 9 ownership/migration checks");
        }finally{UnityEngine.Object.DestroyImmediate(padObject);UnityEngine.Object.DestroyImmediate(target);UnityEngine.Object.DestroyImmediate(cameraObject);}
    }
}
#endif
