Shader "Matrix64/IceShader" {
   Properties {
    _MainColor ("Main Color", Color) = (1,1,1,1)
    _Specular ("Specular", Color) = (1,1,1,1)
    _Gloss ("Gloss", Range(8, 256)) = 20
    _MainTex ("Main Texture", 2D) = "white" {}
    _CubeMap ("CubeMap", Cube) = "_Skybox" {}
    _BumpMap ("Normal Map", 2D) = "white" {}
    _DistertionIntensity ("Distertion Intensity" ,Range(-1, 1)) = 0.73
    _Fresnel ("Fresnel", Range(0, 1)) = 0.25
    _FresnelBoost ("Fresnel Boost", Float ) = 4
    _EmissiveBoost ("Emissive Boost" ,Range(1, 5)) = 2.58
    _Opacity ("Opacity" ,Range(0, 1)) = 0.306
}

 SubShader {
  Tags {
   "IgnoreProjector"="True"
   "RenderType" = "Transparent"
   "Queue" = "Transparent"
  }
  CGINCLUDE
   #include "UnityCG.cginc"
   #include "AutoLight.cginc"
   #include "Lighting.cginc"

   float4 _MainColor;
   float4 _Specular;
   float _Gloss;
   sampler2D _MainTex;float4 _MainTex_ST;
   samplerCUBE _CubeMap;
   sampler2D _BumpMap;float4 _BumpMap_ST;
   float _DistertionIntensity;
   float _Fresnel;
   float _FresnelBoost;
   float _EmissiveBoost;
   float _Opacity;
   sampler2D _ReflectionTex;

   struct a2v {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float4 tangent : TANGENT;
    float2 texcoord : TEXCOORD0;
   };

   struct v2f {
    float4 pos : SV_POSITION;
    float4 scrPos : TEXCOORD0;
    float2 uv : TEXCOORD1;
    float4 TtoW0 : TEXCOORD2;
    float4 TtoW1 : TEXCOORD3;
    float4 TtoW2 : TEXCOORD4;
   };
   v2f vert(a2v v){
    v2f o = (v2f)0;
    o.pos = UnityObjectToClipPos(v.vertex);
    float3 worldPos = mul(unity_ObjectToWorld, v.vertex);

    float3 worldNormal = UnityObjectToWorldNormal(v.normal);
    float3 worldTangent = UnityObjectToWorldDir(v.tangent);
    float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

    o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
    o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
    o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
    o.scrPos = ComputeGrabScreenPos(o.pos);
    o.uv = v.texcoord;
    return o;
   }

   float4 frag(v2f i) : SV_TARGET {
    float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
    float3 worldNormal = float3(i.TtoW0.z, i.TtoW1.z, i.TtoW2.z);
    float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

    #ifdef _IS_PASS_FWDBASE
    float3 worldLightDir = _WorldSpaceLightPos0.xyz;
    #elif _IS_PASS_FWDDELTA
    float3 worldLightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - worldPos.xyz, _WorldSpaceLightPos0.w));
    #endif

    float3 bumpMap = UnpackNormal(tex2D(_BumpMap, TRANSFORM_TEX(i.uv, _BumpMap)));

    bumpMap.xy *= _DistertionIntensity;
    bumpMap.z = sqrt(1.0 - saturate(dot(bumpMap.xy, bumpMap.xy)));
    float2 sceneUV = (i.scrPos.xy / i.scrPos.w) + bumpMap.rg;
    bumpMap = normalize(half3(dot(i.TtoW0.xyz, bumpMap), dot(i.TtoW1.xyz, bumpMap), dot(i.TtoW2.xyz, bumpMap)));
    float3 worldRefl = reflect(-worldViewDir, bumpMap);
    float3 sceneColor = tex2D(_ReflectionTex, sceneUV);

    ///Specular
    float3 halfDir = normalize(worldLightDir + worldViewDir);
    float3 _specular = _LightColor0.rgb * _Specular.rgb * pow(saturate(dot(bumpMap,halfDir)), _Gloss);

    float EmissiveScale = _EmissiveBoost * lerp(0.8,1,(_SinTime.z + 1) * 0.5);
    float fresnel = _Fresnel + (1 - _Fresnel) * pow(1 - (dot(worldViewDir, bumpMap)),5);
    float EmissiveFresnel = (1-_Fresnel) + ( _Fresnel) * pow(1 - (dot(worldViewDir * EmissiveScale, bumpMap)),5);

    float3 _emissive =   (fresnel + 1-EmissiveFresnel) * _FresnelBoost * texCUBE(_CubeMap, worldRefl).rgb * tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)).rgb * _MainColor.rgb;

    float4 finalRGBA = float4(lerp(sceneColor, (_specular + _emissive) , _Opacity) ,1);
    return finalRGBA;
   }
  ENDCG


  GrabPass {"_ReflectionTex"}
  Pass{
   Tags{
    "LightMode" = "ForwardBase"
   }
   CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    #pragma multi_compile_fwdbase
    #pragma target 3.0

    #pragma multi_compile _IS_PASS_FWDDELTA
   ENDCG
  }
  Pass{
   Tags{
    "LightMode" = "ForwardAdd"
   }
   Blend One One
   CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag

    #pragma multi_compile_fwdadd_fullshadows
    #pragma target 3.0

    #pragma multi_compile _IS_PASS_FWDDELTA
   ENDCG
  }
 }
 Fallback "Legacy Shaders/VertexLit"
}