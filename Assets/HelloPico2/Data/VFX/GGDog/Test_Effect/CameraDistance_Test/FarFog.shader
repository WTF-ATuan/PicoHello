Shader "Unlit/CameraDistance_Test"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_Color("Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _Far ("Vanishing Position", Float) = 70
		[KeywordEnum(Vertex, WorldPosition)] _ToCamera("CameraDistance Mode", Float) = 0.0

        _FarSmoothStep ("Far Contrast", Range(0,0.5)) = 0
        _FarPosition ("Far Position", Range(-0.5,0.5)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

		#pragma shader_feature _TOCAMERA_VERTEX  _TOCAMERA_WORLDPOSITION

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				half4 WorldPos_CD : TEXCOORD0;
				half3 ViewDir : TEXCOORD1;
                half3 WorldNormal : TEXCOORD2;
            };

            half _Far;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                o.WorldPos_CD.xyz = unity_ObjectToWorld._m03_m13_m23;

            #if _TOCAMERA_VERTEX

				o.WorldPos_CD.w = length(mul(UNITY_MATRIX_MV,v.vertex).xyz)/_Far;

            #elif _TOCAMERA_WORLDPOSITION

				o.WorldPos_CD.w = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23)/_Far ;

            #endif
            
                o.ViewDir.xyz = normalize(_WorldSpaceCameraPos.xyz - o.WorldPos_CD.xyz );

				o.WorldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));


                return o;
            }
            half4 _SkyColor;
            half4 _Color;
            half4 _ShadowColor;

            half _FarSmoothStep;
            half _FarPosition;

            
            half4 frag (v2f i) : SV_Target
            {

                half CD = smoothstep(0+_FarSmoothStep+_FarPosition,1-_FarSmoothStep+_FarPosition,i.WorldPos_CD.w);
                
                half NdotL =  saturate(dot(i.WorldNormal,_WorldSpaceLightPos0));

				half Rim = 1-smoothstep(-1,0.75,dot(i.WorldNormal,i.ViewDir.xyz));
				half Rim_Shadow = 1-smoothstep(0,1,dot(i.WorldNormal,i.ViewDir.xyz));

                half4 FinalColor = 1;

                half4 RimCol = _SkyColor*Rim*(NdotL)*(1-CD);
                half4 RimCol_Shadow = _SkyColor*Rim_Shadow*(1-NdotL)*(1-CD);

                FinalColor = lerp(_ShadowColor,_SkyColor,CD*smoothstep(0,100,i.WorldPos_CD.z-20));

                FinalColor = lerp(FinalColor,_Color,NdotL*(1-CD));

                FinalColor = FinalColor+RimCol+RimCol_Shadow/10;
                
                

				return FinalColor;            
			}
            ENDCG
        }
    }
}
