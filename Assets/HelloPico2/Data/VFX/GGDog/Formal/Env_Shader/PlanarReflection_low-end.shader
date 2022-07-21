Shader "GGDog/Space_Test/PlanarReflection_low-end"
{	
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
		_FadeColor("Fade Color",Color) = (0.5,0.5,0.5,1)
		_FogColor("Fog Color",Color) = (1,1,1,1)
		_BackFogColor("Back Fog Color",Color) = (0,0,0,1)
    }

	SubShader
	{
		Tags { "RenderType"="Opaque" }
 
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
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
			};
 
			sampler2D _ReflectionTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				return o;
			}
			float4 _FogColor;
			float4 _FadeColor;
			float4 _Color;
			float4 _BackFogColor;
			
			float4 frag (v2f i) : SV_Target
			{
				float4 col = 0.75*_Color;
				//中心距離場
				float D = distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				
				col = lerp(col ,_FadeColor*_Color,1-smoothstep(0.005,0.2,D));

				col = lerp(col , _FogColor, smoothstep(0,0.5,D)*smoothstep(0,0.5,1-i.uv.y));
				
				col = lerp(col ,_BackFogColor,smoothstep(0,1,i.uv.y));

				return col;
			}
			ENDCG
		}
	}
}
