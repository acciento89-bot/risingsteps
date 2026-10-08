Shader "Rising/Cloud" { Properties {_MainTex("Cloud",2D)="white"{} _Color("Color",Color)=(1,1,1,1)} SubShader {Tags {"Queue"="Transparent-10" "RenderType"="Transparent"} Blend SrcAlpha OneMinusSrcAlpha ZWrite Off Cull Off Pass { CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"
sampler2D _MainTex;fixed4 _Color;struct app{float4 vertex:POSITION;float2 uv:TEXCOORD0;};struct vf{float4 pos:SV_POSITION;float2 uv:TEXCOORD0;UNITY_FOG_COORDS(1)};vf vert(app v){vf o;o.pos=UnityObjectToClipPos(v.vertex);o.uv=v.uv;UNITY_TRANSFER_FOG(o,o.pos);return o;}fixed4 frag(vf i):SV_Target{fixed4 c=tex2D(_MainTex,i.uv)*_Color;UNITY_APPLY_FOG(i.fogCoord,c);return c;}
ENDCG } } }
