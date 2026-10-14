Shader "Kamilunavo/RisingCharacter"
{
 Properties {
  _MainTex("Original surface detail",2D)="white"{}
  _Color("Garment or skin",Color)=(1,1,1,1)
  _Glossiness("Smoothness",Range(0,1))=.16
  _Metallic("Metallic",Range(0,1))=0
  _OcclusionStrength("Authored mesh occlusion",Range(0,1))=.80
 }
 SubShader {
  Tags {"RenderType"="Opaque"}
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows
  #pragma target 3.0
  sampler2D _MainTex;half4 _Color;half _Glossiness,_Metallic,_OcclusionStrength;
  struct Input {float2 uv_MainTex;float4 color:COLOR;};
  void surf(Input i,inout SurfaceOutputStandard o){
   o.Albedo=tex2D(_MainTex,i.uv_MainTex).rgb*_Color.rgb;
   o.Metallic=_Metallic;o.Smoothness=_Glossiness;
   o.Occlusion=lerp(1,saturate(i.color.a),_OcclusionStrength);o.Alpha=1;
  }
  ENDCG
 }
 FallBack "Diffuse"
}
