Shader "Rising/Sky" { Properties { _MainTex("Panorama",2D)="white"{} } SubShader { Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox"} Cull Off ZWrite Off Pass { CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
sampler2D _MainTex;struct v2f{float4 pos:SV_POSITION;float3 dir:TEXCOORD0;};v2f vert(float4 vertex:POSITION){v2f o;o.pos=UnityObjectToClipPos(vertex);o.dir=vertex.xyz;return o;}fixed4 frag(v2f i):SV_Target{float3 d=normalize(i.dir);float u=atan2(d.z,d.x)/6.283185+.5;float v=saturate(.30+d.y*.73);return tex2D(_MainTex,float2(u,v));}
ENDCG } } }
