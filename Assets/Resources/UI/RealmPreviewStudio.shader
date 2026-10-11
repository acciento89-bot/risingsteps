Shader "Kamilunavo/UI/RealmPreviewStudio"
{
 Properties
 {
  _MainTex ("Shipping surface", 2D) = "white" {}
  _Color ("Surface colour", Color) = (1,1,1,1)
  _EmissionColor ("Landmark glow", Color) = (0,0,0,0)
  _Glossiness ("Surface polish", Range(0,1)) = .15
  _StudioKeyDirection ("Local key direction", Vector) = (-.3,.7,-.6,0)
  _StudioFillDirection ("Local fill direction", Vector) = (.65,.2,.6,0)
  _StudioKeyColor ("Local key colour", Color) = (1,.94,.82,1)
  _StudioFillColor ("Local fill colour", Color) = (.76,.87,1,1)
 }
 SubShader
 {
  Tags { "RenderType"="Opaque" "Queue"="Geometry" }
  Pass
  {
   CGPROGRAM
   #pragma vertex vert
   #pragma fragment frag
   #pragma target 3.0
   #include "UnityCG.cginc"
   sampler2D _MainTex;float4 _MainTex_ST,_Color,_EmissionColor,_StudioKeyDirection,_StudioFillDirection,_StudioKeyColor,_StudioFillColor;float _Glossiness;
   struct Input {float4 vertex:POSITION;float3 normal:NORMAL;float2 uv:TEXCOORD0;};
   struct Output {float4 position:SV_POSITION;float2 uv:TEXCOORD0;float3 normal:TEXCOORD1;float3 world:TEXCOORD2;};
   Output vert(Input v){Output o;o.position=UnityObjectToClipPos(v.vertex);o.uv=TRANSFORM_TEX(v.uv,_MainTex);o.normal=UnityObjectToWorldNormal(v.normal);o.world=mul(unity_ObjectToWorld,v.vertex).xyz;return o;}
   float4 frag(Output i):SV_Target
   {
    float3 n=normalize(i.normal),key=normalize(_StudioKeyDirection.xyz),fill=normalize(_StudioFillDirection.xyz);
    float3 surface=tex2D(_MainTex,i.uv).rgb*_Color.rgb;
    // Only these owned studio lamps contribute. Gameplay ambient, fog and sun cannot alter a card.
    float3 illumination=.30+max(0,dot(n,key))*.63*_StudioKeyColor.rgb+max(0,dot(n,fill))*.22*_StudioFillColor.rgb;
    float3 halfway=normalize(key+normalize(_WorldSpaceCameraPos-i.world));
    float sheen=pow(max(0,dot(n,halfway)),lerp(9,40,_Glossiness))*_Glossiness*.22;
    return float4(surface*illumination+sheen*_StudioKeyColor.rgb+_EmissionColor.rgb*.35,1);
   }
   ENDCG
  }
 }
 Fallback Off
}
