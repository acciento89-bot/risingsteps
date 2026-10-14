Shader "Rising/Bloom" { Properties {_MainTex("Source",2D)="white"{} _GlowTex("Glow",2D)="black"{} _Exposure("Exposure",Float)=1 _GammaSpace("Gamma source",Float)=1 _Warmth("Realm warmth",Float)=0 _AOStrength("Contact depth",Range(0,1))=0 } SubShader { Cull Off ZWrite Off ZTest Always
CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex,_GlowTex,_CameraDepthNormalsTexture;float4 _MainTex_TexelSize,_SourceSize;float2 _Axis;float _Exposure,_GammaSpace,_Warmth,_AOStrength,_FarClip,_ProjectionScale;float4x4 _InverseProjection;
float3 LinearScene(float3 color){return _GammaSpace>.5?GammaToLinearSpace(color):color;}
// A bounded filmic shoulder keeps warm HDR channels distinct while restoring
// brighter daylight midtones than the earlier grey Reinhard compression.
float3 DaylightCurve(float3 c){return saturate((c*(2.51*c+.03))/(c*(2.43*c+.59)+.14));}
float3 ViewPoint(float2 uv,float depth){float4 ray=mul(_InverseProjection,float4(uv*2-1,1,1));return ray.xyz*(depth/max(abs(ray.z),.0001));}
float ContactShade(float2 uv){
 if(_AOStrength<=0)return 1;
 float2 depthUv=uv;
 #if UNITY_UV_STARTS_AT_TOP
 if(_MainTex_TexelSize.y<0)depthUv.y=1-depthUv.y;
 #endif
 float depth;float3 normal;DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture,depthUv),depth,normal);float eye=depth*_FarClip;
 if(eye<=.1||depth>.995)return 1;
 float3 origin=ViewPoint(depthUv,eye);float pixelRadius=clamp(.14*_ProjectionScale/eye,1.5,12);float occlusion=0;
 [unroll]for(int sampleIndex=0;sampleIndex<8;sampleIndex++){
  float angle=sampleIndex*.785398+ .23;float2 at=saturate(depthUv+float2(cos(angle),sin(angle))*_SourceSize.zw*pixelRadius);float otherDepth;float3 otherNormal;DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture,at),otherDepth,otherNormal);
  float3 delta=ViewPoint(at,otherDepth*_FarClip)-origin;float distance=length(delta);float angular=max(0,dot(normal,delta/max(distance,.0001))-.08);occlusion+=angular*(1-saturate(distance/.65));
 }
 return 1-_AOStrength*occlusion*.25;
}
ENDCG
Pass {CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
half4 frag(v2f_img i):SV_Target{half3 c=LinearScene(tex2D(_MainTex,i.uv).rgb);half bright=max(c.r,max(c.g,c.b));return half4(c*saturate((bright-1.25)/max(bright,.01)),1);}
ENDCG}
Pass {CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
half4 frag(v2f_img i):SV_Target{float2 d=_MainTex_TexelSize.xy*_Axis;return tex2D(_MainTex,i.uv)*.40+(tex2D(_MainTex,i.uv+d*1.4)+tex2D(_MainTex,i.uv-d*1.4))*.24+(tex2D(_MainTex,i.uv+d*3.2)+tex2D(_MainTex,i.uv-d*3.2))*.06;}
ENDCG}
Pass {CGPROGRAM
#pragma vertex vert_img
#pragma fragment frag
#pragma target 3.0
half4 frag(v2f_img i):SV_Target{float3 c=LinearScene(tex2D(_MainTex,i.uv).rgb)*ContactShade(i.uv)+tex2D(_GlowTex,i.uv).rgb*.14;c=max(0,c*_Exposure*float3(1+_Warmth,1,1-_Warmth));float luma=dot(c,float3(.2126,.7152,.0722));c=max(0,lerp(luma.xxx,c,1.07));c=DaylightCurve(c);if(_GammaSpace>.5)c=LinearToGammaSpace(c);return half4(c,1);}
ENDCG} } }
