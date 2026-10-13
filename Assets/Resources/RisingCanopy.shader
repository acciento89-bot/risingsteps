Shader "Kamilunavo/RisingCanopy"
{
 Properties {
  _MainTex ("Original leaf cutout", 2D) = "white" {}
  _Color ("Leaf tint", Color) = (1,1,1,1)
  _Cutoff ("Leaf edge", Range(0,1)) = .42
 }
 SubShader {
  Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
  CGPROGRAM
  #pragma surface surf Standard alphatest:_Cutoff addshadow fullforwardshadows
  #pragma target 3.0
  sampler2D _MainTex; fixed4 _Color;
  struct Input { float2 uv_MainTex; };
  void surf(Input i, inout SurfaceOutputStandard o) {
   fixed4 c=tex2D(_MainTex,i.uv_MainTex)*_Color;
   o.Albedo=c.rgb; o.Alpha=c.a; o.Metallic=0; o.Smoothness=.08;
  }
  ENDCG
 }
 FallBack "Transparent/Cutout/Diffuse"
}
