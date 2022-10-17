Shader "GGDog/Space_Test/Soft_PointGlow"
{
    Properties
    {
		_Layer("Layer",Range(0,30)) = 1
		_Color("Color",Color) = (1,1,1,1)
        _Alpha("Alpha",Range(0,1)) = 1
        _intense("Intense",Range(1,5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		
        ColorMask RGBA
		
        ZWrite Off
        Stencil {
            Ref [_Layer]
            Comp Equal
            Pass Keep
        }
        ZTest Always
		
        Pass
        {
		    Blend One One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
				
                return o;
            }
            float _intense;
            float4 _Color;
            float _Alpha;
            float4 frag (v2f i) : SV_Target
            {
				//中心距離場
				//float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				float D = smoothstep(-15.5,14.0,1-35.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1)*1.75;

				//漸層度
				D = smoothstep(0.5,2,D)*_intense;

				float4 finalColor = saturate(i.color*D*i.color.a);
				
                clip(saturate(finalColor.a) - 0.00015);

                return finalColor*_Color*_Alpha;
            }
            ENDCG
        }
    }
}
