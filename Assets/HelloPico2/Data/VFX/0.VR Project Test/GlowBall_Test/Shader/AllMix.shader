Shader "Unlit/Rim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color)=(1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		ZWrite Off
		Blend One One
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				o.worldNormal = UnityObjectToWorldNormal(v.normal); //o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed Noise = tex2D(_MainTex, i.uv - _Time.y *0.5).r;

				fixed4 col = tex2D(_MainTex, i.uv+_Time.y*float2(0,0.5) + Noise*0.05);
				
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

				float Rim = 1-saturate(pow(dot(worldNormal,worldViewDir)+0.05,0.5));

				
				float Rim2 = saturate(pow(dot(worldNormal,worldViewDir)+0.25,20));

				
				return Rim2*Rim*col*_Color;
			}
			ENDCG
		}
	}
}
