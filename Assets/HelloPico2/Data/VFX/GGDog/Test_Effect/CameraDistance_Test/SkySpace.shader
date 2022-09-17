Shader "Unlit/CameraDistance_Test"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _Far ("Vanishing Position", Float) = 70
		[KeywordEnum(Vertex, WorldPosition)] _ToCamera("CameraDistance Mode", Float) = 0.0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Front

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
            };

            half _Far;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                o.WorldPos_CD.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;

            #if _TOCAMERA_VERTEX

				o.WorldPos_CD.w = length(mul(UNITY_MATRIX_MV,v.vertex).xyz)/_Far;

            #elif _TOCAMERA_WORLDPOSITION

				o.WorldPos_CD.w = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23)/_Far ;

            #endif
            
                return o;
            }
            half4 _SkyColor;
            half4 _ShadowColor;

            half4 frag (v2f i) : SV_Target
            {
               half4 FinalColor = lerp(_ShadowColor,_SkyColor, smoothstep(0,70,i.WorldPos_CD.z-20));

				return FinalColor;            
			}
            ENDCG
        }
    }
}
