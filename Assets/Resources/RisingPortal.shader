Shader "Kamilunavo/RisingPortal"
{
 Properties { _Color ("Warm radiance",Color) = (1,.68,.20,1) }
 SubShader {
  Tags {"Queue"="Transparent" "RenderType"="Transparent"}
  Cull Off ZWrite Off Blend One One
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   struct V{float4 vertex:POSITION;float2 uv:TEXCOORD0;};
   struct F{float4 position:SV_POSITION;float2 uv:TEXCOORD0;};
   float4 _Color;
   F vert(V v){F o;o.position=UnityObjectToClipPos(v.vertex);o.uv=v.uv;return o;}
   fixed4 frag(F i):SV_Target{
    float center=pow(saturate(1-abs(i.uv.x-.5)*1.8),2);
    float light=.8+center*1.9;
    return float4(lerp(_Color.rgb,float3(1,.94,.68),center*.65)*light,1);
   }
   ENDCG
  }
 }
}
