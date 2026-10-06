using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.CameraSystem;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.Input;
using Kamilunavo.RisingSteps.UI;

namespace Kamilunavo.RisingSteps
{
    public sealed class GameBootstrap:MonoBehaviour
    {
        private static readonly Color Navy=new(.035f,.12f,.24f,.94f);private static readonly Color Purple=new(.45f,.12f,.94f,.96f);private static readonly Color Orange=new(1f,.40f,.06f,.96f);private static readonly Color Blue=new(.05f,.52f,1f,.96f);private static readonly Color White=new(.98f,.99f,1f);
        private void Start(){Screen.orientation=ScreenOrientation.Portrait;Application.targetFrameRate=60;QualitySettings.vSyncCount=0;RenderSettings.fog=true;RenderSettings.fogColor=new Color(.58f,.80f,1f);RenderSettings.fogDensity=.004f;EnsureEventSystem();Lighting();
            var canvas=UiFactory.Canvas();var safe=UiFactory.Panel(canvas.transform,"SafeArea",Color.clear,Vector2.zero,Vector2.one);safe.gameObject.AddComponent<SafeAreaFitter>();
            UiFactory.Button(safe,"Settings","⚙",Navy,White,new Vector2(.03f,.915f),new Vector2(.115f,.98f),()=>{});
            UiFactory.Button(safe,"Inbox","✉",Navy,White,new Vector2(.125f,.915f),new Vector2(.21f,.98f),()=>{});
            UiFactory.Button(safe,"Audio","♫",Navy,White,new Vector2(.22f,.915f),new Vector2(.305f,.98f),()=>{});
            var heightPanel=UiFactory.Panel(safe,"HeightPanel",Navy,new Vector2(.34f,.915f),new Vector2(.67f,.98f));var height=UiFactory.Label(heightPanel,"Height","HEIGHT 0 / 12",30,new Vector2(.08f,.38f),new Vector2(.92f,.94f),TextAnchor.MiddleCenter,White,FontStyle.Bold);var progress=UiFactory.Progress(heightPanel,new Vector2(.08f,.12f),new Vector2(.92f,.28f),new Color(.18f,.24f,.32f),new Color(1f,.75f,.12f));
            var crystalPanel=UiFactory.Panel(safe,"Crystals",Navy,new Vector2(.70f,.915f),new Vector2(.97f,.98f));var crystals=UiFactory.Label(crystalPanel,"Value","◇  0",32,new Vector2(.08f,.08f),new Vector2(.92f,.92f),TextAnchor.MiddleCenter,White,FontStyle.Bold);
            var toast=UiFactory.Label(safe,"Toast","READY",28,new Vector2(.35f,.74f),new Vector2(.65f,.79f),TextAnchor.MiddleCenter,new Color(1f,.80f,.20f),FontStyle.Bold);
            var joystick=VirtualJoystick.Create(safe,new Vector2(.03f,.035f),new Vector2(.29f,.18f));var jump=PressButton.Create(safe,"↑",new Vector2(.78f,.035f),new Vector2(.97f,.17f));
            var player=Player();var camera=Camera(player.transform);
            var courseObject=new GameObject("RisingCourse");var course=courseObject.AddComponent<RisingCourse>();course.Player=player.transform;course.PlayerRenderer=player.GetComponent<Renderer>();course.HeightText=height;course.CrystalsText=crystals;course.Progress=progress;course.Toast=toast;
            var motor=player.GetComponent<PlayerMotor>();motor.Joystick=joystick;motor.Jump=jump;motor.CameraTransform=camera.transform;motor.Course=course;
            var overlay=UiFactory.Panel(safe,"ModalBackdrop",new Color(0,0,0,.56f),Vector2.zero,Vector2.one);overlay.gameObject.SetActive(false);
            RectTransform modal=null;
            void Close(){overlay.gameObject.SetActive(false);if(modal!=null)Destroy(modal.gameObject);}
            void Open(string title,string body,Color accent,System.Action primary,string actionLabel){overlay.gameObject.SetActive(true);if(modal!=null)Destroy(modal.gameObject);modal=UiFactory.Panel(overlay,title+"Modal",Navy,new Vector2(.10f,.28f),new Vector2(.90f,.70f));UiFactory.Label(modal,"Title",title,46,new Vector2(.07f,.75f),new Vector2(.93f,.94f),TextAnchor.MiddleCenter,accent,FontStyle.Bold);UiFactory.Label(modal,"Body",body,28,new Vector2(.09f,.33f),new Vector2(.91f,.74f),TextAnchor.MiddleCenter,White);UiFactory.Button(modal,"Primary",actionLabel,accent,Color.white,new Vector2(.10f,.12f),new Vector2(.90f,.28f),()=>{primary?.Invoke();Close();});UiFactory.Button(modal,"Close","CLOSE",new Color(.08f,.14f,.24f),White,new Vector2(.32f,.02f),new Vector2(.68f,.09f),Close);}
            UiFactory.Button(safe,"Shop","SHOP   ›",Purple,White,new Vector2(.04f,.59f),new Vector2(.41f,.665f),()=>Open("SHOP","Sky Trail\nGolden Landing\nAurora Steps",Purple,()=>toast.text="SHOP PREVIEW","PREVIEW"));
            UiFactory.Button(safe,"Daily","DAILY   ›",Orange,White,new Vector2(.04f,.50f),new Vector2(.41f,.575f),()=>Open("DAILY","Daily reward\n100 crystals",Orange,course.ClaimDaily,"CLAIM"));
            UiFactory.Button(safe,"Style","STYLE   ›",Blue,White,new Vector2(.04f,.41f),new Vector2(.41f,.485f),()=>Open("STYLE","Switch runner accent\nGreen • Blue • Gold",Blue,course.CycleStyle,"NEXT STYLE"));
            course.Build();
        }
        private static GameObject Player(){var go=GameObject.CreatePrimitive(PrimitiveType.Capsule);go.name="Runner";Destroy(go.GetComponent<Collider>());var cc=go.AddComponent<CharacterController>();cc.height=2;cc.radius=.42f;cc.center=new Vector3(0,1,0);go.AddComponent<PlayerMotor>();go.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=new Color(.12f,.46f,.18f)};return go;}
        private static Camera Camera(Transform t){var go=new GameObject("Main Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitCamera));go.tag="MainCamera";var c=go.GetComponent<Camera>();c.fieldOfView=58;var o=go.GetComponent<OrbitCamera>();o.Target=t;go.transform.position=t.position+new Vector3(0,3,-7);return c;}
        private static void Lighting(){var go=new GameObject("Sun",typeof(Light));var l=go.GetComponent<Light>();l.type=LightType.Directional;l.intensity=1.25f;l.color=new Color(1f,.82f,.58f);go.transform.rotation=Quaternion.Euler(35,-30,0);}
        private static void EnsureEventSystem(){if(FindFirstObjectByType<EventSystem>()==null)new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));}
    }
}
