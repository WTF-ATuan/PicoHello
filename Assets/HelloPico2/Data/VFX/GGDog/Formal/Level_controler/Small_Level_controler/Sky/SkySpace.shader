Shader "Unlit/CameraDistance_Test"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _Far ("Vanishing Position", Float) = 70
		[KeywordEnum(Vertex, WorldPosition)] _ToCamera("CameraDistance Mode", Float) = 0.0

        
        _SkyFarPos ("_SkyFarPos", Float) = 70

        _ViewFarPos ("_ViewFarPos", Float) = 70

    }
    SubShader
    {
        Tags { "Queue"="Geometry-1000" }

        
        Pass
        {
            Cull Front
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

		#pragma shader_feature _TOCAMERA_VERTEX  _TOCAMERA_WORLDPOSITION

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
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
            half3 _SkyColor;
            half3 _ShadowColor;
            half _SkyFarPos;
            half _ViewFarPos;
            
            half4 frag (v2f i) : SV_Target
            {

               half3 FinalColor = _ShadowColor;
               /*
                //頂光色
               FinalColor = lerp(FinalColor,_SkyColor*2,smoothstep(0,30,i.WorldPos_CD.y-0));

               //地霧
                FinalColor = lerp(FinalColor,_SkyColor*2,1-smoothstep(-10,10,abs(i.WorldPos_CD.y)-10));

                //後方暗漸層
                FinalColor = lerp(_ShadowColor,FinalColor, smoothstep(-_SkyFarPos,_SkyFarPos,i.WorldPos_CD.z));
                

                half Z =smoothstep(-20,_SkyFarPos,i.WorldPos_CD.z);

               //SSS發光地霧
                FinalColor = lerp(FinalColor,_SkyColor*2, Z*(1-smoothstep(-3.5,7,abs(i.WorldPos_CD.y+1.5))) );
                */
                

                FinalColor = lerp(FinalColor,_SkyColor*2, smoothstep(_SkyFarPos/5,_SkyFarPos,i.WorldPos_CD.z));

				return half4( FinalColor,1);            
			}
            ENDCG
        }

        /*
        Pass
        {
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

		#pragma shader_feature _TOCAMERA_VERTEX  _TOCAMERA_WORLDPOSITION

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
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
            half3 _SkyColor;
            half3 _ShadowColor;
            half _SkyFarPos;
            
            half4 frag (v2f i) : SV_Target
            {

               half3 FinalColor = _ShadowColor;
               
                //頂光色
               FinalColor = lerp(FinalColor,_SkyColor*2,smoothstep(0,30,i.WorldPos_CD.y-0));

               //地霧
                FinalColor = lerp(FinalColor,_SkyColor*2,1-smoothstep(-10,10,abs(i.WorldPos_CD.y)-10));

                //後方暗漸層
                FinalColor = lerp(_ShadowColor,FinalColor, smoothstep(-_SkyFarPos,_SkyFarPos,i.WorldPos_CD.z));
                
                half Z =smoothstep(-20,_SkyFarPos,i.WorldPos_CD.z);

               //SSS發光地霧
                FinalColor = lerp(FinalColor,_SkyColor*2, Z*(1-smoothstep(-3.5,7,abs(i.WorldPos_CD.y+1.5))) );

				return half4( FinalColor,1);            
			}
            ENDCG
        }*/
    }
}
