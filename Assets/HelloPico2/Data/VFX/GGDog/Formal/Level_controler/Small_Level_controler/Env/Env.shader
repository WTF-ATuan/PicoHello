Shader "Unlit/CameraDistance_Test"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		[HDR]_Color("Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _Far ("Vanishing Position", Float) = 70
        _FarSmoothStep ("Far Contrast", Range(0,0.5)) = 0
        _FarPosition ("Far Position", Range(-0.5,0.5)) = 0

        _SkyFarPos ("_SkyFarPos", Float) = 70
    }
    SubShader
    {
        Tags { "Queue"="Geometry-950" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
				half Rim : TEXCOORD1;
            };

            half _Far;
            half _FarSmoothStep;
            half _FarPosition;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                o.WorldPos_CD.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;


				o.WorldPos_CD.w = length(mul(UNITY_MATRIX_MV,v.vertex).xyz)/_Far;
                o.WorldPos_CD.w = smoothstep(0+_FarSmoothStep+_FarPosition,1-_FarSmoothStep+_FarPosition,o.WorldPos_CD.w);
            
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - unity_ObjectToWorld._m03_m13_m23 );

				half3 WorldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
                
                half NdotV =  saturate(dot(WorldNormal,ViewDir.xyz));

				o.Rim = 1-smoothstep(-1,0.75,NdotV);


                return o;
            }
            half3 _SkyColor;
            half3 _Color;
            half3 _ShadowColor;

            half _SkyFarPos;
            
            half4 frag (v2f i) : SV_Target
            {

                half CD = i.WorldPos_CD.w;
                
                half3 FinalColor = _ShadowColor;
                
                //頂光色
                FinalColor = lerp(FinalColor,_SkyColor*2,CD*(smoothstep(0,30,i.WorldPos_CD.y-0)));

               //地霧
                FinalColor = lerp(FinalColor,_SkyColor*2,1*(1-smoothstep(-10,10,abs(i.WorldPos_CD.y)-10)));
                
                //後方暗漸層
                FinalColor = lerp(_ShadowColor,FinalColor,CD*smoothstep(-_SkyFarPos,_SkyFarPos,i.WorldPos_CD.z));
                
                FinalColor = FinalColor + _Color*i.Rim*(1-CD);
                
               //SSS發光地霧
                FinalColor = lerp(FinalColor,FinalColor+_SkyColor,1-smoothstep(-5,10,abs(i.WorldPos_CD.y+1.5*(1-CD))));
                
				return half4( FinalColor,1);            
			}
            ENDCG
        }
    }
}
