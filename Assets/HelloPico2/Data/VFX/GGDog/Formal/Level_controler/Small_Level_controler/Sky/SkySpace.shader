Shader "Unlit/CameraDistance_Test"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _SkyFarPos ("_SkyFarPos", Float) = 70
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

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
				half WorldPos_CD : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                o.WorldPos_CD = mul(unity_ObjectToWorld, v.vertex).z;

                return o;
            }
            half3 _SkyColor;
            half3 _ShadowColor;
            half _SkyFarPos;
            
            half4 frag (v2f i) : SV_Target
            {

                half3 FinalColor = lerp(_ShadowColor,_SkyColor*2, smoothstep(_SkyFarPos/5,_SkyFarPos,i.WorldPos_CD));

				return half4(FinalColor,1);            
			}
            ENDCG
        }

    }
}
