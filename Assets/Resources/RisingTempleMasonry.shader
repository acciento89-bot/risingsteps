Shader "Kamilunavo/RisingTempleMasonry"
{
 Properties
 {
  _Color ("Mineral color", Color) = (1,1,1,1)
  _MainTex ("Original sandstone", 2D) = "white" {}
  _BumpMap ("Authored linear RGB normal", 2D) = "bump" {}
  _BumpScale ("Relief strength", Range(0,1)) = .65
  _OcclusionStrength ("Authored mesh occlusion", Range(0,1)) = .85
  _Metallic ("Metallic", Range(0,1)) = 0
  _Glossiness ("Smoothness", Range(0,1)) = .07
 }
 SubShader
 {
  Tags { "RenderType"="Opaque" }
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows
  #pragma target 3.0
  sampler2D _MainTex, _BumpMap;
  fixed4 _Color;
  half _BumpScale, _Metallic, _Glossiness, _OcclusionStrength;
  struct Input { float2 uv_MainTex; float2 uv_BumpMap; float4 color:COLOR; };
  void surf(Input i, inout SurfaceOutputStandard o)
  {
   o.Albedo=tex2D(_MainTex,i.uv_MainTex).rgb*_Color.rgb;
   half3 n=tex2D(_BumpMap,i.uv_BumpMap).xyz*2-1;n.xy*=_BumpScale;
   o.Normal=normalize(half3(n.xy,sqrt(max(.0001,1-saturate(dot(n.xy,n.xy))))));
   o.Metallic=_Metallic;o.Smoothness=_Glossiness;o.Alpha=1;
   o.Occlusion=lerp(1,saturate(i.color.a),_OcclusionStrength);
  }
  ENDCG
 }
 FallBack "Diffuse"
}
