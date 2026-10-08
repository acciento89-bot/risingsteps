Shader "Rising/Sparkles" { Properties { _Color("Color",Color)=(1,.87,.3,1) } SubShader { Tags {"Queue"="Transparent" "RenderType"="Transparent"} Blend SrcAlpha One ZWrite Off Cull Off Pass { CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
fixed4 _Color;struct app{float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};struct vf{float4 pos:SV_POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};vf vert(app v){vf o;o.pos=UnityObjectToClipPos(v.vertex);o.color=v.color*_Color;o.uv=v.uv;return o;}fixed4 frag(vf i):SV_Target{float r=length(i.uv-.5)*2;float a=saturate(1-r);return fixed4(i.color.rgb*(.5+a),i.color.a*a*a);}
ENDCG } } }
