Shader "Unlit/Rim"
{
	Properties
	{
		[HDR]_RimColor("RimColor",Color) = (1,1,1,1)
		_Gray("_Gray",Range(0,1)) = 1
		_Width("_Width",Range(1,10)) = 5
		_OffSet("OffSet",Range(-10,10)) = 0
	}
	SubShader
	{
        Tags { "Queue" = "Transparent+1"}
		ZTest Always
		ZWrite Off
		Pass
		{	
			Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 color : COLOR;
			};

            CBUFFER_START(UnityPerMaterial) 
			float4 _RimColor;
            float _Width;
            float _OffSet;
            float _Gray;
            CBUFFER_END

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				//o.worldNormal = UnityObjectToWorldNormal(v.normal); 
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
                o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

				float Rim = 1-saturate(smoothstep(0,1.1,dot(worldNormal,worldViewDir)));

				float Rim2 = saturate(smoothstep(0,1.5,dot(worldNormal,worldViewDir)));

                float4 col = 1;

				col =lerp( float4(1,0,0,1) , float4(0,1,0,1) , (1-Rim)*_Width +_OffSet);
				col =lerp(             col , float4(0,0,1,1) , (1-Rim)*_Width-0.2 +_OffSet);

				col+=0.5;

				col = float4(lerp( 0 , 1-2*(1-col.rgb), step(0.5,col.rgb) ),Rim*Rim2);

				return col*_RimColor*i.color;
			}
			ENDCG
		}
	}
}
