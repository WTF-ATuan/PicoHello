// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/PointGlow"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100
		
		Zwrite Off
		Blend One One
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
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
				o.color = v.color;
				
				o.uv.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				//���߶Z����
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				//float D = smoothstep(-15.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

				//���h��
				float D2 = smoothstep(0.87,0.9,D);

				D = D2+smoothstep(0.5,1.5,D)*1.5;

				i.color = saturate(lerp(i.color*i.color,i.color,D));

				float4 finalColor = saturate(i.color*D*i.color.a);
				
				finalColor.a *= smoothstep(0,50,i.uv.z);

                clip(saturate(finalColor.a) - 0.000015);

                return finalColor;
            }
            ENDCG
        }
    }
}
