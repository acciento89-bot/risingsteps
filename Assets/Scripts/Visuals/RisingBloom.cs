using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 [RequireComponent(typeof(Camera))]public sealed class RisingBloom:MonoBehaviour
 {
  public Kamilunavo.RisingSteps.Gameplay.RisingCourse Course;
  private Material _m;private Camera _camera;private bool _depthSupported;
  private void Awake(){_camera=GetComponent<Camera>();var shader=Resources.Load<Shader>("RisingBloom");if(shader!=null&&shader.isSupported){_m=new Material(shader){name="RisingOwnedCameraGrade"};ArtLifetime.Own(gameObject,_m);}_depthSupported=SystemInfo.graphicsShaderLevel>=30&&SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth);if(_depthSupported)_camera.depthTextureMode|=DepthTextureMode.DepthNormals;}
  private void OnRenderImage(RenderTexture source,RenderTexture destination)
  {
   if(_m==null){Graphics.Blit(source,destination);return;}
   _m.SetFloat("_GammaSpace",QualitySettings.activeColorSpace==ColorSpace.Gamma?1:0);_m.SetFloat("_Exposure",1f);_m.SetFloat("_Warmth",Course?.Profile?.Realm==2?.025f:0);
   _m.SetFloat("_AOStrength",_depthSupported?.30f:0);_m.SetFloat("_FarClip",_camera.farClipPlane);_m.SetFloat("_ProjectionScale",Mathf.Abs(_camera.projectionMatrix.m11)*source.height*.5f);
   _m.SetVector("_SourceSize",new Vector4(source.width,source.height,1f/source.width,1f/source.height));_m.SetMatrix("_InverseProjection",GL.GetGPUProjectionMatrix(_camera.projectionMatrix,true).inverse);
   int w=Mathf.Max(8,source.width/4),h=Mathf.Max(8,source.height/4);RenderTexture a=null,b=null;
   try{var format=SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR)?RenderTextureFormat.DefaultHDR:RenderTextureFormat.Default;a=RenderTexture.GetTemporary(w,h,0,format,RenderTextureReadWrite.Linear);b=RenderTexture.GetTemporary(w,h,0,format,RenderTextureReadWrite.Linear);a.filterMode=b.filterMode=FilterMode.Bilinear;
    Graphics.Blit(source,a,_m,0);_m.SetVector("_Axis",new Vector4(1,0,0,0));Graphics.Blit(a,b,_m,1);_m.SetVector("_Axis",new Vector4(0,1,0,0));Graphics.Blit(b,a,_m,1);_m.SetTexture("_GlowTex",a);Graphics.Blit(source,destination,_m,2);
   }finally{_m.SetTexture("_GlowTex",Texture2D.blackTexture);if(a!=null)RenderTexture.ReleaseTemporary(a);if(b!=null)RenderTexture.ReleaseTemporary(b);}
  }
 }
 public sealed class CloudBillboard:MonoBehaviour{private void LateUpdate(){if(Camera.main!=null)transform.rotation=Camera.main.transform.rotation;}}
}
