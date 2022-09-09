Shader "GGDog/Space_Test/Soft_PointGlow"
{
    Properties
    {
        _intense("Intense",Range(1,5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		
		Zwrite Off
		Blend One One

        ZTest Always
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
				float CameraDistance : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
				
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

                return o;
            }
            float _intense;
            float4 frag (v2f i) : SV_Target
            {
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//漸層度
				D = smoothstep(0.5,1.75,D)*_intense;

				float4 finalColor = saturate(i.color*D*i.color.a);
				
                clip(saturate(finalColor.a) - 0.000005);

                return finalColor;
            }
            ENDCG
        }
    }
}
