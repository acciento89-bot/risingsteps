using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Kamilunavo.RisingSteps.Core;
using Kamilunavo.RisingSteps.Visuals;
namespace Kamilunavo.RisingSteps.UI
{
 // Shipping island and horizon geometry, framed as a small isolated studio scene.
 public sealed class RealmScenePreview:RawImage
 {
  public const int PreviewLayer=31;
  const int SlotCount=32;const float Spacing=256;
  static readonly bool[] Slots=new bool[SlotCount];
  public static int ActiveLeaseCount {get{int count=0;foreach(bool occupied in Slots)if(occupied)count++;return count;}}
  public int Realm{get;private set;}public bool Hero{get;private set;}public GameObject SceneRoot=>_scene;
  int _slot=-1;GameObject _scene;Camera _camera;RenderTexture _target;bool _rendered;Vector2 _size;
  static int Lease(){for(int i=0;i<Slots.Length;i++)if(!Slots[i]){Slots[i]=true;return i;}throw new InvalidOperationException("Realm preview scene capacity exhausted");}
  static void Dispose(UnityEngine.Object value){if(Application.isPlaying)Destroy(value);else DestroyImmediate(value);}
  public static Graphic Add(Transform parent,int realm,Vector2 min,Vector2 max,bool hero=false)
  {
   var go=new GameObject("RealmArt",typeof(RectTransform),typeof(CanvasRenderer),typeof(RealmScenePreview));go.transform.SetParent(parent,false);var image=go.GetComponent<RealmScenePreview>();image.rectTransform.anchorMin=min;image.rectTransform.anchorMax=max;image.rectTransform.offsetMin=image.rectTransform.offsetMax=Vector2.zero;
   image.Realm=(int)RisingRealmCatalog.Theme(realm);image.Hero=hero;image.raycastTarget=false;image.material=RoundedPanel.GalleryMaterial;
   try{image.BuildScene();}catch{Dispose(go);throw;}return image;
  }
  void BuildScene()
  {
   _slot=Lease();_scene=new GameObject("RealmPreviewScene"+Realm);_scene.transform.position=new Vector3(2000+(_slot%8)*Spacing,0,2000+(_slot/8)*Spacing);_scene.AddComponent<LandscapePalette>();
   var island=new GameObject("ActualIsland");island.transform.SetParent(_scene.transform,false);IslandArt.Build(island.transform,12,Realm,true);
   var backdrop=new GameObject("ActualBackdrop");backdrop.transform.SetParent(_scene.transform,false);RealmArt.BuildBackdrop(backdrop.transform,Realm);backdrop.transform.localScale=Vector3.one*.115f;backdrop.transform.localPosition=new Vector3(0,.3f,5.5f);
   var key=Lamp("RealmStudioKey",new Vector3(-4,7,-6),new Color(1,.94f,.82f),1.4f);var fill=Lamp("RealmStudioFill",new Vector3(6,3,-1),new Color(.76f,.87f,1),.65f);
   OwnStudioMaterials(-key.transform.forward,-fill.transform.forward);
   var host=new GameObject("RealmPreviewCamera",typeof(Camera));host.transform.SetParent(_scene.transform,false);_camera=host.GetComponent<Camera>();_camera.enabled=false;_camera.cullingMask=1<<PreviewLayer;_camera.clearFlags=CameraClearFlags.SolidColor;_camera.backgroundColor=Sky(Realm);_camera.orthographic=true;_camera.nearClipPlane=.1f;_camera.farClipPlane=65;_camera.allowHDR=false;_camera.allowMSAA=true;
   host.transform.localPosition=new Vector3(7.5f,4.8f,-11);host.transform.LookAt(_scene.transform.position+new Vector3(0,.30f,.7f));_camera.orthographicSize=4.3f;
   Layer(_scene.transform);_target=new RenderTexture(128,128,16,RenderTextureFormat.ARGB32){name="RealmPreviewTarget"+Realm,antiAliasing=2,filterMode=FilterMode.Bilinear};_camera.targetTexture=_target;texture=_target;
  }
  Light Lamp(string name,Vector3 position,Color tint,float intensity)
  {
   var go=new GameObject(name,typeof(Light));go.transform.SetParent(_scene.transform,false);go.transform.localPosition=position;go.transform.LookAt(_scene.transform.position+Vector3.up*.3f);var light=go.GetComponent<Light>();light.type=LightType.Spot;light.range=32;light.spotAngle=110;light.color=tint;light.intensity=intensity;light.shadows=LightShadows.None;light.cullingMask=1<<PreviewLayer;return light;
  }
  void OwnStudioMaterials(Vector3 key,Vector3 fill)
  {
   var shader=Resources.Load<Shader>("UI/RealmPreviewStudio");if(shader==null)throw new InvalidOperationException("Realm preview studio shader is missing");var clones=new Dictionary<Material,Material>();
   foreach(var renderer in _scene.GetComponentsInChildren<Renderer>()){
    var materials=renderer.sharedMaterials;for(int i=0;i<materials.Length;i++){var source=materials[i];if(source==null)continue;if(!clones.TryGetValue(source,out var owned)){owned=new Material(shader);owned.CopyPropertiesFromMaterial(source);owned.name="RealmPreview_"+Realm+"_"+source.name;owned.SetVector("_StudioKeyDirection",key);owned.SetVector("_StudioFillDirection",fill);owned.SetColor("_StudioKeyColor",new Color(1,.94f,.82f));owned.SetColor("_StudioFillColor",new Color(.76f,.87f,1));ArtLifetime.Own(_scene,owned);clones.Add(source,owned);}materials[i]=owned;}
    renderer.sharedMaterials=materials;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
   }
  }
  static Color Sky(int realm)=>RisingRealmCatalog.Theme(realm) switch {RisingRealmTheme.Jungle=>new(.56f,.78f,.72f),RisingRealmTheme.Crystal=>new(.15f,.20f,.33f),RisingRealmTheme.Frost=>new(.72f,.85f,.93f),RisingRealmTheme.Desert=>new(.97f,.76f,.51f),RisingRealmTheme.Volcano=>new(.32f,.28f,.39f),RisingRealmTheme.CloudCoast=>new(.57f,.83f,.91f),_=>new(.10f,.15f,.30f)};
  static void Layer(Transform node){node.gameObject.layer=PreviewLayer;foreach(Transform child in node)Layer(child);}
  protected override void OnPopulateMesh(VertexHelper mesh){base.OnPopulateMesh(mesh);RoundedPanel.SetGalleryCoordinates(mesh,rectTransform.rect);}
  // Manual rendering occurs once per image/size, rather than seven cameras every gameplay frame.
  public void RenderPreview()
  {
   if(_camera==null||!isActiveAndEnabled)return;var size=rectTransform.rect.size;if(size.x<1||size.y<1||(_rendered&&size==_size))return;_size=size;_camera.aspect=size.x/size.y;_camera.orthographicSize=Mathf.Max(4.3f,3.3f/_camera.aspect);
   int width=Mathf.Clamp(Mathf.CeilToInt(size.x*2),96,512),height=Mathf.Clamp(Mathf.CeilToInt(size.y*2),96,512);if(_target.width!=width||_target.height!=height){_target.Release();_target.width=width;_target.height=height;}_target.Create();_camera.Render();_rendered=true;
  }
  void LateUpdate(){if(Application.isPlaying)RenderPreview();}
  protected override void OnDisable(){base.OnDisable();if(_scene!=null)_scene.SetActive(false);}
  protected override void OnEnable(){base.OnEnable();if(_scene!=null)_scene.SetActive(true);_rendered=false;}
  protected override void OnDestroy(){base.OnDestroy();texture=null;if(_camera!=null)_camera.targetTexture=null;if(_target!=null){_target.Release();Dispose(_target);}_target=null;if(_scene!=null){_scene.SetActive(false);Dispose(_scene);}_scene=null;if(_slot>=0){Slots[_slot]=false;_slot=-1;}}
 }
}
