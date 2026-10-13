Shader "Kamilunavo/RisingMineral"
{
 Properties
 {
  _Color ("Mineral color", Color) = (1,1,1,1)
  _MainTex ("Mineral deposits", 2D) = "white" {}
  _BumpMap ("Authored linear RGB normal", 2D) = "bump" {}
  _BumpScale ("Relief strength", Range(0,1)) = .65
  _Metallic ("Metallic", Range(0,1)) = 0
  _Glossiness ("Smoothness", Range(0,1)) = .1
  _EmissionColor ("Emission", Color) = (0,0,0,1)
 }
 SubShader
 {
  Tags { "RenderType"="Opaque" }
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows
  #pragma target 3.0
  sampler2D _MainTex, _BumpMap;
  fixed4 _Color, _EmissionColor;
  half _BumpScale, _Metallic, _Glossiness;
  struct Input { float2 uv_MainTex; float2 uv_BumpMap; };
  void surf(Input i, inout SurfaceOutputStandard o)
  {
   fixed4 color=tex2D(_MainTex,i.uv_MainTex)*_Color;
   o.Albedo=color.rgb; o.Alpha=color.a;
   // Runtime RGBA32 is deliberately uncompressed linear XYZ, not an imported
   // platform-packed normal. Decode it explicitly on every graphics backend.
   half3 n=tex2D(_BumpMap,i.uv_BumpMap).xyz*2-1;
   n.xy*=_BumpScale;
   o.Normal=normalize(half3(n.xy,sqrt(max(.0001,1-saturate(dot(n.xy,n.xy))))));
   o.Metallic=_Metallic; o.Smoothness=_Glossiness; o.Emission=_EmissionColor.rgb;
  }
  ENDCG
 }
 FallBack "Diffuse"
}
