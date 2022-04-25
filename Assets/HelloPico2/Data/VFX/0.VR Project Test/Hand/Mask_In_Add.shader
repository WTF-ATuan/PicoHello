//2D�B�n_�o��
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

			#pragma vertex vert    //�w�qvertex Shader ��k
			#pragma fragment frag  //�w�q fragment Shader ��k
			
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			//��J��
			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed4 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};


			//��X�ݡA�⳻�I�ǵ����q
			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD1;
				fixed4 color : TEXCOORD2;
			};

			//��k
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
				//���߶Z����
				fixed D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//���h��
				fixed D2 = smoothstep(0.75,1.5,D)*8;

				fixed4 col =D2+ smoothstep(0.5,1.5,D)*1.5 ;

			    return col* i.color*i.color.a*2;
			}
			ENDCG
		}
	}

}