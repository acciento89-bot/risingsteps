Shader "Kamilunavo/RisingWater"
{
 Properties {
  _MainTex("Original caustics",2D)="white"{} _Color("Water tint",Color)=(1,1,1,1)
  _Glossiness("Smoothness",Range(0,1))=.52 _Metallic("Metallic",Range(0,1))=0
  _RippleTime("Owned ripple clock",Float)=0
 }
 SubShader {
  Tags {"RenderType"="Opaque"} Cull Off
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows
  #pragma target 3.0
  #pragma shader_feature_local _NORMALMAP
  sampler2D _MainTex;half4 _Color;half _Glossiness,_Metallic;float _RippleTime;
  struct Input {float2 uv_MainTex;float3 worldPos;float3 viewDir;};
  void surf(Input i,inout SurfaceOutputStandard o){
   float phase=_RippleTime*.65;float2 at=i.uv_MainTex+float2(sin(phase*.6),cos(phase*.5))*.012;
   half3 color=tex2D(_MainTex,at).rgb*_Color.rgb;
   half2 ripple=half2(sin(i.worldPos.x*1.7+i.worldPos.z*.6+phase),cos(i.worldPos.z*2.1-i.worldPos.x*.4+phase*.8))*.065;
   o.Normal=normalize(half3(ripple,1));half fresnel=pow(1-saturate(dot(normalize(i.viewDir),o.Normal)),4);
   o.Albedo=color;o.Metallic=_Metallic;o.Smoothness=_Glossiness;o.Emission=half3(.24,.50,.59)*fresnel*.18;o.Alpha=1;
  }
  ENDCG
 }
 FallBack "Diffuse"
}
