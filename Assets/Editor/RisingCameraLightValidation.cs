#if UNITY_EDITOR
using System;
using UnityEngine;
public static class RisingCameraLightValidation
{
 public static void Validate()
 {
  var shader=Resources.Load<Shader>("RisingBloom");if(shader==null||!shader.isSupported)throw new InvalidOperationException("Rising camera light shader is unavailable");
  var source=new Texture2D(8,8,TextureFormat.RGBAFloat,false,true);var pixels=new Color[64];for(int i=0;i<pixels.Length;i++)pixels[i]=new Color(3,2,.8f,1);source.SetPixels(pixels);source.Apply();
  var material=new Material(shader);var target=RenderTexture.GetTemporary(8,8,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear);var readback=new Texture2D(8,8,TextureFormat.RGBAFloat,false,true);var prior=RenderTexture.active;
  try{
   material.SetTexture("_GlowTex",Texture2D.blackTexture);material.SetFloat("_Exposure",1);material.SetFloat("_AOStrength",0);material.SetFloat("_Warmth",0);
   Color Sample(Color input,bool gamma){for(int i=0;i<pixels.Length;i++)pixels[i]=input;source.SetPixels(pixels);source.Apply();material.SetFloat("_GammaSpace",gamma?1:0);Graphics.Blit(source,target,material,2);RenderTexture.active=target;readback.ReadPixels(new Rect(0,0,8,8),0,0);readback.Apply();return readback.GetPixel(4,4);}
   var result=Sample(new Color(3,2,.8f,1),false);if(!float.IsFinite(result.r)||!float.IsFinite(result.g)||!float.IsFinite(result.b)||result.r>=1||result.g>=1||result.b>=1||result.r-result.g<=.015f||!(result.g>result.b))throw new InvalidOperationException("Rising HDR camera output must remain bounded and preserve warm channel separation: "+result);
   var dark=Sample(new Color(.02f,.02f,.02f,1),false);var mid=Sample(new Color(.18f,.18f,.18f,1),false);var lit=Sample(new Color(.6f,.6f,.6f,1),false);
   if(dark.r>=.15f||mid.r<=.20f||mid.r>=.65f||lit.r<=.55f||lit.r>=.95f||lit.r-mid.r<=.20f)throw new InvalidOperationException("Daylight grade must preserve shadows, separated midtones and bright nonclipped limestone: "+dark+" / "+mid+" / "+lit);
   var encoded=Sample(new Color(.45f,.45f,.45f,1),true);if(encoded.r<=.50f||encoded.r>=.85f||Mathf.Abs(encoded.r-encoded.g)>.01f||Mathf.Abs(encoded.g-encoded.b)>.01f)throw new InvalidOperationException("Current Gamma project must preserve a bright neutral midtone without a double conversion: "+encoded);
   Debug.Log("RISING_CAMERA_LIGHT_GPU_PASS bounded HDR="+result+" linear shadows/mid/limestone="+dark+" / "+mid+" / "+lit+" Gamma mid="+encoded);
  }
  finally{RenderTexture.active=prior;RenderTexture.ReleaseTemporary(target);UnityEngine.Object.DestroyImmediate(source);UnityEngine.Object.DestroyImmediate(readback);UnityEngine.Object.DestroyImmediate(material);}
 }
}
#endif
