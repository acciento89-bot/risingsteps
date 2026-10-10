using System;
using System.Collections.Generic;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Visuals;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.Gameplay
{
    public sealed class RisingCourse:MonoBehaviour
    {
        public Transform Player;public Text HeightText;public Text CrystalsText;public Image Progress;public Text Toast;public Renderer PlayerRenderer;
        private GameObject _scenery;private readonly List<StepMarker> _steps=new();private Vector3 _safe;private bool _paused,_focused=true,_applicationPaused;private float _saveTimer;
        public RisingProfile Profile{get;private set;}public int Height=>Profile?.Step??0;public IReadOnlyList<StepMarker> Steps=>_steps;
        public event Action Changed,RunStarted;public event Action<bool> Landed;public event Action Fell,PortalCompleted;
        public bool Paused{get=>_paused||!_focused||_applicationPaused;set{_paused=value;var m=Player!=null?Player.GetComponent<PlayerMotor>():null;if(m!=null){m.Paused=Paused;m.ResetInput();}}}
        public Vector3 SafePosition=>_safe;public Kamilunavo.RisingSteps.UI.RisingHud Hud;public Kamilunavo.RisingSteps.Monetization.StorePurchases Store;public Kamilunavo.RisingSteps.Monetization.RewardedVideos Videos;
        public void RefreshProfile()=>Refresh();
        private static readonly Color Rock=new(.32f,.30f,.39f);private static readonly Color Grass=new(.28f,.58f,.25f);private static readonly Color Gold=new(1f,.80f,.20f);

        public void Build(){Profile=RisingSave.Load();BuildRoute();}
        private void BuildRoute(){BuildScenery();foreach(var step in _steps)if(step!=null){step.transform.parent.gameObject.SetActive(false);Destroy(step.transform.parent.gameObject);}_steps.Clear();
            var points=CoursePatterns.Points(Profile.Realm,Profile.Challenge?DaySeed(Profile.RunDay):260906+Profile.Realm);
            for(int i=0;i<points.Length;i++){var root=new GameObject($"Island_{i:00}");root.transform.SetParent(transform,false);root.transform.position=points[i];
                var marker=IslandArt.Build(root.transform,i,Profile.Realm);_steps.Add(marker);if(i==12)PortalArt.Build(root.transform,this);
                MeshArt.Batch(root);
            }Physics.SyncTransforms();SetSafe();MoveToSafe();Refresh();RunStarted?.Invoke();}
        private void BuildScenery(){if(_scenery!=null){_scenery.SetActive(false);Destroy(_scenery);}_scenery=new GameObject("RealmScenery"+Profile.Realm);_scenery.transform.SetParent(transform,false);SkyArt.Build(_scenery.transform,Profile.Realm);
            for(int i=0;i<14;i++){var root=new GameObject(Profile.Realm==1?"DistantCascade":Profile.Realm==2?"DistantTemple":"DistantMeadow");root.transform.SetParent(_scenery.transform,false);root.transform.position=Profile.Realm==1?new Vector3((i%2==0?-1:1)*(12+i%4*4),-4+i%4*4,7+i*5):Profile.Realm==2?new Vector3((i%2==0?-1:1)*(11+i%4*4),-3+i%4*3,8+i*5):new Vector3((i%2==0?-1:1)*(10+i%4*4),-3+i%4*3,4+i*5);root.transform.localScale=Vector3.one*(Profile.Realm==1?2.0f+i%3*.45f:Profile.Realm==2?1.65f+i%3*.45f:1.5f+i%3*.5f);IslandArt.Build(root.transform,i,Profile.Realm,true);MeshArt.Batch(root);}}
        private static int DaySeed(string day){unchecked{int hash=17;foreach(char c in day)hash=hash*31+c;return hash;}}
        public void StartRun(int realm,bool challenge){if(realm<0||realm>Profile.UnlockedRealm)return;Profile.Realm=realm;Profile.Step=0;Profile.Elapsed=0;Profile.Falls=0;Profile.Perfects=0;Profile.Completed=false;Profile.Challenge=challenge;Profile.RunDay=RisingRules.Day(DateTime.UtcNow);Save();BuildRoute();}
        private void SetSafe()=>_safe=_steps[Height].transform.position+Vector3.up*.65f;
        private void MoveToSafe(){var cc=Player.GetComponent<CharacterController>();if(cc!=null)cc.enabled=false;Player.position=_safe;Physics.SyncTransforms();if(cc!=null)cc.enabled=true;Player.GetComponent<PlayerMotor>()?.ResetMotion();}
        private void Update(){if(_steps.Count==0||Player==null||Paused)return;
            if(!Profile.Completed){Profile.Elapsed+=Time.deltaTime;_saveTimer+=Time.deltaTime;if(_saveTimer>=5){_saveTimer=0;Save();}}
            if(Player.position.y<_steps[Height].transform.position.y-8)Respawn();}
        public void Land(StepMarker step){if(Paused||Profile==null)return;var delta=Player.position-step.transform.position;bool perfect=new Vector2(delta.x,delta.z).magnitude<=.8f;
            if(!RisingRules.Land(Profile,step.Index,perfect))return;SetSafe();Save();if(Toast!=null)Toast.text=Height==12?"PORTAL READY":perfect?"PERFECT!":"STEP CLEARED";Refresh();Landed?.Invoke(perfect);}
        public void Respawn(){if(Profile==null)return;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
 Debug.Log("RISING_RECOVERY frame="+Time.frameCount+" player="+Player.position+" safe="+_safe+" fallsBefore="+Profile.Falls);
#endif
 Profile.Falls++;MoveToSafe();Save();Refresh();Fell?.Invoke();}
        public bool ClaimDaily(){bool claimed=RisingRules.ClaimDaily(Profile,DateTime.UtcNow);if(claimed)Save();Refresh();return claimed;}
        public bool SelectStyle(int style){bool selected=RisingRules.SelectStyle(Profile,style);if(selected)Save();Refresh();return selected;}
        public void CycleStyle()=>SelectStyle((Profile.Style+1)%4);
        public int CompletePortal(){int stars=RisingRules.Complete(Profile,DateTime.UtcNow);if(stars>0){Save();Refresh();PortalCompleted?.Invoke();}return stars;}
        public void Save(){try{RisingSave.Save(Profile);}catch(Exception e){Debug.LogWarning("Profile save unavailable: "+e.Message);if(Toast!=null)Toast.text="SAVE UNAVAILABLE";}}
        private void OnApplicationPause(bool pause){_applicationPaused=pause;SyncFocus();}
        private void OnApplicationFocus(bool focus){Debug.Log("RISING_FOCUS "+focus);_focused=focus;SyncFocus();}
        private void SyncFocus(){if(Profile!=null)Save();if(Player!=null){var motor=Player.GetComponent<PlayerMotor>();if(motor!=null){motor.Paused=Paused;motor.ResetInput();}}}
        private void OnApplicationQuit(){if(Profile!=null)Save();}
        private void Refresh(){if(HeightText!=null)HeightText.text=$"HEIGHT {Height} / 12";if(CrystalsText!=null)CrystalsText.text=$"◇  {Profile.Crystals}";if(Progress!=null)Progress.fillAmount=Height/12f;Changed?.Invoke();}
    }
}
