Shader "Rising/Sky" {
 Properties { _Horizon("Horizon",Color)=(0.78,0.92,1,1) _Zenith("Zenith",Color)=(0.24,0.58,0.89,1) _Below("Cloud mist",Color)=(0.91,0.96,1,1) }
 SubShader { Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"} Cull Off ZWrite Off
 Pass { CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "UnityCG.cginc"
 fixed4 _Horizon,_Zenith,_Below;
 struct v2f {float4 pos:SV_POSITION;float3 dir:TEXCOORD0;};
 v2f vert(float4 vertex:POSITION){v2f o;o.pos=UnityObjectToClipPos(vertex);o.dir=vertex.xyz;return o;}
 fixed4 frag(v2f i):SV_Target{float3 direction=normalize(i.dir);float elevation=direction.y;
  fixed3 sky=lerp(_Horizon.rgb,_Zenith.rgb,smoothstep(-0.16,0.48,elevation));
  sky=lerp(sky,_Below.rgb,smoothstep(0.22,0.72,-elevation));
  // Sparse atmospheric wisps contain no painted landforms or playable route.
  float band=sin(direction.x*23+direction.z*7+sin(direction.z*5))*sin(direction.z*13-direction.x*6);
  float wisps=pow(saturate(band),6)*smoothstep(0.04,0.14,elevation)*(1-smoothstep(0.28,0.54,elevation));
  sky=lerp(sky,_Below.rgb,wisps*0.18);return fixed4(sky,1);
 }
 ENDCG }
 }
}
