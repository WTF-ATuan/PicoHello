Shader "GGDog/SkyBox/Sky_Color"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (1, 1, 1, 1)
        [HDR]_HorizonColor ("Horizon Color", Color) = (1, 1, 1, 1)
		
        _SmoothStepMin ("���h��(�̧C)", Range(0, 1)) = 0
        _SmoothStepMax ("���h��(�̰�)", Range(0, 1)) = 1

        _WaterColor ("Water Color", Color) = (1, 1, 1, 1)

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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD1;
            };

            float4 _SkyColor;
            float4 _HorizonColor;

            float _SmoothStepMin;
            float _SmoothStepMax;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            float4 _WaterColor;
			
            float4 frag (v2f i) : SV_Target
            {
				float4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax,i.uv.y));

				
				col = lerp(_WaterColor,col,smoothstep(-100,250,i.worldPos.y));

				col = lerp(_HorizonColor,col,smoothstep(-1500,250,i.worldPos.z));

                return col;
            }
            ENDCG
        }
    }
}
