Shader "Unlit/Mask"
{
	Properties
	{
		_Layer("Layer",Range(0,30)) = 0
		_RimColor("RimColor",Color) = (1,1,1,1)
		_RimPow("RimPow",Range(0,1.5)) = 1
		_RimPart("RimPart",Range(0,1)) = 0.15

		_LightDir ("Light Direction", Vector) = (0.5, 1, 1 ,0)
	}
	SubShader
	{
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
        ZWrite off
        Stencil {
            Ref [_Layer]
            Comp always
            Pass replace
        }

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

			float4 _RimColor;
			float _RimPow;
			float _RimPart;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

				//o.worldNormal = UnityObjectToWorldNormal(v.normal); 
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				return o;
			}
			
			fixed3 _LightDir;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

				float Rim = 1-saturate(smoothstep(0,_RimPow,dot(worldNormal,worldViewDir)+(1-_RimPart) ));

				float NDotL =smoothstep(0.5,0.75, dot(worldNormal,_LightDir))/2;

				return (Rim+NDotL)*_RimColor*1.5*i.color.a;
			}
			ENDCG
		}
	}
}
