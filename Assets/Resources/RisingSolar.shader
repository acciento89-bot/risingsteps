Shader "Kamilunavo/RisingSolar"
{
 Properties { _Color("Sky sun",Color)=(1,.90,.50,1) }
 SubShader {
  Tags {"Queue"="Geometry" "RenderType"="Opaque"}
  Pass {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #include "UnityCG.cginc"
   float4 _Color;
   struct v2f {float4 vertex:SV_POSITION;float3 world:TEXCOORD0;float3 normal:TEXCOORD1;};
   v2f vert(float4 vertex:POSITION,float3 normal:NORMAL){v2f o;o.vertex=UnityObjectToClipPos(vertex);o.world=mul(unity_ObjectToWorld,vertex).xyz;o.normal=UnityObjectToWorldNormal(normal);return o;}
   // Bright smooth core supplies real HDR bloom; the quieter gold limb avoids
   // the former flat yellow coin silhouette. Scene fog does not wash out the sun.
   half4 frag(v2f i):SV_Target {half facing=saturate(dot(normalize(i.normal),normalize(_WorldSpaceCameraPos-i.world)));half core=smoothstep(.15,.88,facing);return half4(lerp(_Color.rgb*.85,half3(2.4,2.2,1.5),core),1);}
   ENDCG
  }
 }
}
