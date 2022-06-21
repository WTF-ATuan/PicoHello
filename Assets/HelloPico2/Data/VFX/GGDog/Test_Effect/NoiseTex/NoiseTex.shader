Shader "Unlit/NoiseTex"
{
    Properties
    {
        _Noise ("Noise", Range(-1.1,1)) = 0.75
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			half _Noise;

            fixed4 frag (v2f i) : SV_Target
            {
				half2 NoiseUV = i.uv*sin(100) * 143758.5453;
				half  NoiseTex = (saturate( sin(NoiseUV.x*100)*cos(NoiseUV.y*100)-_Noise)*2);

                return NoiseTex;
            }
            ENDCG
        }
    }
}
