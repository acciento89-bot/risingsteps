using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 [RequireComponent(typeof(Camera))]public sealed class RisingBloom:MonoBehaviour
 {
  private Material _m;private void Awake(){var shader=Shader.Find("Rising/Bloom");if(shader!=null&&shader.isSupported)_m=new Material(shader);}
  private void OnRenderImage(RenderTexture source,RenderTexture destination){if(_m==null){Graphics.Blit(source,destination);return;}int w=Mathf.Max(8,source.width/4),h=Mathf.Max(8,source.height/4);var a=RenderTexture.GetTemporary(w,h,0,RenderTextureFormat.DefaultHDR);var b=RenderTexture.GetTemporary(w,h,0,RenderTextureFormat.DefaultHDR);a.filterMode=b.filterMode=FilterMode.Bilinear;Graphics.Blit(source,a,_m,0);_m.SetVector("_Axis",new Vector4(1,0,0,0));Graphics.Blit(a,b,_m,1);_m.SetVector("_Axis",new Vector4(0,1,0,0));Graphics.Blit(b,a,_m,1);_m.SetTexture("_GlowTex",a);Graphics.Blit(source,destination,_m,2);RenderTexture.ReleaseTemporary(a);RenderTexture.ReleaseTemporary(b);}
  private void OnDestroy(){if(_m!=null)Destroy(_m);}
 }
 public sealed class CloudBillboard:MonoBehaviour{private void LateUpdate(){if(Camera.main!=null)transform.rotation=Camera.main.transform.rotation;}}
}
