Shader "GGDog/Space_Test/PointGlow"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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

            float4 frag (v2f i) : SV_Target
            {
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//漸層度
				float D2 = smoothstep(0.75,1.5,D)*8;

				D = D2+smoothstep(0.5,1.5,D)*1.5;

				i.color = saturate(lerp(i.color*i.color,i.color,D));

				float4 finalColor = saturate(i.color*D*i.color.a);
				
				finalColor.a *= smoothstep(0,50,i.CameraDistance);

                return finalColor;
            }
            ENDCG
        }
    }
}
