Shader "Kamilunavo/RisingCharacter"
{
 Properties {
  _MainTex("Original surface detail",2D)="white"{}
  _Color("Garment or skin",Color)=(1,1,1,1)
  _Glossiness("Smoothness",Range(0,1))=.16
  _Metallic("Metallic",Range(0,1))=0
  _OcclusionStrength("Authored mesh occlusion",Range(0,1))=.80
  _DetailRelief("Original textile or hair relief",Range(0,1))=0
 }
 SubShader {
  Tags {"RenderType"="Opaque"}
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows
  #pragma target 3.0
  sampler2D _MainTex;float4 _MainTex_TexelSize;half4 _Color;half _Glossiness,_Metallic,_OcclusionStrength,_DetailRelief;
  struct Input {float2 uv_MainTex;float4 color:COLOR;};
  void surf(Input i,inout SurfaceOutputStandard o){
   o.Albedo=tex2D(_MainTex,i.uv_MainTex).rgb*_Color.rgb;
   // Original albedo is also the shallow height field. A centered one-texel
   // gradient is bounded and fades at distant mip levels to avoid sparkling.
   float2 texel=_MainTex_TexelSize.xy;
   half hx=dot(tex2D(_MainTex,i.uv_MainTex+float2(texel.x,0)).rgb-tex2D(_MainTex,i.uv_MainTex-float2(texel.x,0)).rgb,half3(.2126,.7152,.0722));
   half hy=dot(tex2D(_MainTex,i.uv_MainTex+float2(0,texel.y)).rgb-tex2D(_MainTex,i.uv_MainTex-float2(0,texel.y)).rgb,half3(.2126,.7152,.0722));
   float2 footprint=max(abs(ddx(i.uv_MainTex)),abs(ddy(i.uv_MainTex)))*_MainTex_TexelSize.zw;
   half relief=_DetailRelief/(1+max(footprint.x,footprint.y));
   o.Normal=normalize(half3(clamp(-half2(hx,hy)*relief*2.5,-.24,.24),1));
   o.Metallic=_Metallic;o.Smoothness=_Glossiness;
   o.Occlusion=lerp(1,saturate(i.color.a),_OcclusionStrength);o.Alpha=1;
  }
  ENDCG
 }
 FallBack "Diffuse"
}
