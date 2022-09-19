Shader "FunS/XR/Billboard"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Header(Option)]
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend", Float) = 6

	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		ZWrite Off

		Blend[_SrcBlend][_DstBlend]
        Pass
        {
			Cull Off
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
				
                clip(col.a - 0.25);

                return col;
            }
            ENDCG
        }

		Pass
		{
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend One One
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 positionCS : SV_POSITION;
				float4 color : COLOR;
				
			};
			
            sampler2D _MainTex;
			v2f vert (appdata v)
			{
				v2f o;
				
				float2 scale = 2.5 * float2(length(unity_ObjectToWorld._m00_m10_m20),length(unity_ObjectToWorld._m01_m11_m21));
				float4 positionWS = unity_ObjectToWorld._m03_m13_m23_m33;
				float2 bias = v.positionOS.xy;
				
				float3 forward = normalize(_WorldSpaceCameraPos - positionWS.xyz);

				float3 up = normalize(UNITY_MATRIX_V[1].xyz);
				float3 right = cross(forward, up);
				positionWS.xyz += right * bias.r * scale.x + up * bias.g *scale.y;

				o.positionCS = mul(UNITY_MATRIX_VP, positionWS);

				o.uv = v.uv;
				o.color = v.color;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//漸層度
				D = smoothstep(0.5,2,D)*1.5;

				float4 finalColor = saturate(i.color*D*i.color.a);
				
                clip(saturate(finalColor.a) - 0.00015);

				return finalColor;
			}
			ENDCG
		}
	}
}