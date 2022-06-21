//2D遮罩_發光
Shader "MyShader/Mask_In_Add"
{
	Properties
	{
		[Header(GlowSprite No Alpha Color)]
		_MainTex ("Base layer (RGB)", 2D) = "white" {}
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

			#pragma vertex vert    //定義vertex Shader 方法
			#pragma fragment frag  //定義 fragment Shader 方法
			
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			//輸入端
			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed4 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};


			//輸出端，把頂點傳給片段
			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD1;
				fixed4 color : TEXCOORD2;
			};

			//方法
			v2f vert (appdata v)
			{
			    v2f o;
				
				o.color =  v.color;

				o.vertex = UnityObjectToClipPos( v.vertex);
				
				o.uv = TRANSFORM_TEX(v.texcoord.xy,_MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{  
				//中心距離場
				fixed D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//漸層度
				fixed D2 = smoothstep(0.75,1.5,D)*8;

				fixed4 col =D2+ smoothstep(0.5,1.5,D)*1.5 ;

			    return col* i.color*i.color.a*2;
			}
			ENDCG
		}
	}

}