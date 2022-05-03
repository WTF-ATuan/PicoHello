Shader "GGDog/Trail_FixUV"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_HDR("HDR",Range(1,10))=1
		_Speed("UVSpeed",Range(0,10))=2

		[HideInInspector]_OffSetUV_x ("OffSetUV_x",Float) = 0
	}
	SubShader
	{
		Tags { "Queue" = "transparent" }
		LOD 100

		ZWrite Off

		//Blend SrcAlpha OneMinusSrcAlpha
		
		Blend One One
		
		Pass
		{
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform	float _OffSetUV_x;
			
			float _HDR;
			float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.color = v.color;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv + _Time.y*float2(-_Speed,0) + float2(_OffSetUV_x,0) )*_HDR;

				return col*i.color*i.color.a;
			}
			ENDCG
		}
	}
}
