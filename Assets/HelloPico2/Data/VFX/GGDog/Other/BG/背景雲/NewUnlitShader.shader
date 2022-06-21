Shader "Unlit/NewUnlitShader"
{
	Properties
	{
        [HDR] _Color("Color", Color) = (1,1,1,1)
        [HDR] _LightColor("LightColor", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col  = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.01));
				fixed4 col2 = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.02) + float2(0.2,0));
				fixed4 col3 = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.03) + float2(-0.7,0));
				fixed4 col4 = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.04) + float2(-0.2,0));
				fixed4 col5 = tex2D(_MainTex, i.uv*float2(1,1)+_Time.y*float2(0,0.05) + float2(0.5,0));

				fixed4 color = fixed4(1,1,1,1);

				col = lerp(col,col2,col2.r);
				col = lerp(col,col3,col3.r);
				col = lerp(col,col4,col4.r);
				col = lerp(col,col5,col5.r);

				col.rgb *= _Color;
				col.rgb += _LightColor*smoothstep(0.25,0.75,i.uv.y/5);

				col.rgb *=smoothstep(0.25,0.75,i.uv.y);

				return col;
			}
			ENDCG
		}
	}
}
