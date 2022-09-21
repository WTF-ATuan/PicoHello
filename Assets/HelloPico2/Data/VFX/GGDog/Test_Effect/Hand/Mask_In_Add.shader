//2D遮罩_發光
Shader "MyShader/Mask_In_Add"
{
	Properties
	{
	}

	SubShader
	{
        Tags { "RenderType"="Transparent" }

        ColorMask RGBA
		
        ZWrite Off
        Stencil {
            Ref 0
            Comp Less
            Pass Keep
        }
		
		Pass
		{
			Blend One One 

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD1;
				float4 color : TEXCOORD2;
			};

			v2f vert (appdata v)
			{
			    v2f o;
				
				o.color =  v.color;

				o.vertex = UnityObjectToClipPos( v.vertex);
				
				o.uv = v.uv;

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{  
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				//float D = smoothstep(-15.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

				//漸層度
				float D2 = smoothstep(0.75,1.5,D)*8;

				float4 col =D2+ smoothstep(0.5,1.5,D)*1.5 ;

			    return col* i.color*i.color.a*2;
			}
			ENDCG
		}
	}

}