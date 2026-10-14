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
   float4 vert(float4 vertex:POSITION):SV_POSITION {return UnityObjectToClipPos(vertex);}
   fixed4 frag():SV_Target {return _Color;}
   ENDCG
  }
 }
}
