Shader "GGDog/SkyBox/Sky_Color"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (1, 1, 1, 1)
        [HDR]_HorizonColor ("Horizon Color", Color) = (1, 1, 1, 1)
		
        _SmoothStepMin ("漸層度(最低)", Range(0, 1)) = 0
        _SmoothStepMax ("漸層度(最高)", Range(0, 1)) = 1

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
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				float4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax,i.uv.y));

                return col;
            }
            ENDCG
        }
    }
}
