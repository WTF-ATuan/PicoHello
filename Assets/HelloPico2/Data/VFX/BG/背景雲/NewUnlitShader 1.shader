Shader "Unlit/NewUnlitShader"
{
	Properties
	{
        [HDR] _Color("Color", Color) = (1,1,1,1)
        [HDR] _LightColor("LightColor", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_MainTex2 ("Texture2", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Cull Front
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
			float4 _Color;
			float4 _LightColor;
			sampler2D _MainTex2;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col  = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.01)+ float2(0,0.75));
				fixed4 col2 = tex2D(_MainTex2, i.uv*float2(1,1)+_Time.y*float2(0,0.015) + float2(0,0));
				fixed4 col3 = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.03) + float2(0,-0.5));
				fixed4 col4 = tex2D(_MainTex2, i.uv*float2(1,1)+_Time.y*float2(0,0.05) + float2(0,0));
				fixed4 col5 = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.07) + float2(0,-0.25));
				fixed4 col6 = tex2D(_MainTex2, i.uv*float2(1,1)+_Time.y*float2(0,0.1) + float2(0,0));

				fixed4 color = fixed4(1,1,1,1);
				
				col = lerp(color,col,smoothstep(0.9,1,col.a));
				col = lerp(col,col2*_Color,col2.a*0.1);
				col = lerp(col,col3*float4(1,1,1,1),smoothstep(0.75,1,col3.a)*smoothstep(0,0.3,1-i.uv.y));
				col = lerp(col,col4*_Color,col4.a*0.25);
				col = lerp(col,col5*float4(1,1,1,1),smoothstep(0.5,1,col5.a)*smoothstep(0,0.3,1-i.uv.y));
				col = lerp(col,col6*_Color,col6.a);

				//col.rgb *= _Color;
				col.rgb += _LightColor*smoothstep(0.15,0.75,i.uv.y/2.25);

				col.rgb *=smoothstep(0.0,0.5,i.uv.y*1);

				return col;
			}
			ENDCG
		}
	}
}
