Shader "Rising/Bloom" { Properties {_MainTex("Source",2D)="white"{} _GlowTex("Glow",2D)="black"{} } SubShader { Cull Off ZWrite Off ZTest Always
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex,_GlowTex;float4 _MainTex_TexelSize;float2 _Axis;
ENDCG
Pass {CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
half4 frag(v2f_img i):SV_Target{half3 c=tex2D(_MainTex,i.uv).rgb;half bright=max(c.r,max(c.g,c.b));return half4(c*saturate((bright-1.8)/max(bright,.01)),1);}
ENDCG}
Pass {CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
half4 frag(v2f_img i):SV_Target{float2 d=_MainTex_TexelSize.xy*_Axis;return tex2D(_MainTex,i.uv)*.40+(tex2D(_MainTex,i.uv+d*1.4)+tex2D(_MainTex,i.uv-d*1.4))*.24+(tex2D(_MainTex,i.uv+d*3.2)+tex2D(_MainTex,i.uv-d*3.2))*.06;}
ENDCG}
Pass {CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
half4 frag(v2f_img i):SV_Target{half3 c=tex2D(_MainTex,i.uv).rgb+tex2D(_GlowTex,i.uv).rgb*.28;c=c/(1+max(c-1,0)*.55);return half4(c,1);}
ENDCG} } }
