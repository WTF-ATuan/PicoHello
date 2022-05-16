Shader "Reflection/PlanarReflection"
{	
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
		_FadeColor("Fade Color",Color) = (0.5,0.5,0.5,1)
		_FarColor("Far Color",Color) = (1,1,1,1)
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
				float4 screenPos : TEXCOORD1;
			};
 
			sampler2D _ReflectionTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}
			float4 _FarColor;
			float4 _FadeColor;
			float4 _Color;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_ReflectionTex, i.screenPos.xy / i.screenPos.w)*_Color;
				
				col = lerp(col ,_FadeColor,smoothstep(0,1,i.uv.y));
				
				//中心距離場
				float D = distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				
				col = lerp(col , _FarColor, smoothstep(0,0.5,D)*smoothstep(0.25,0.5,1-i.uv.y));
				
				return col;
			}
			ENDCG
		}
	}
}
