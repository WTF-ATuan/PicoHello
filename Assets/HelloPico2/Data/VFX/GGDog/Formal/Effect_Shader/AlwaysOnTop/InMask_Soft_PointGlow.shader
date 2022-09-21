// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/Soft_PointGlow"
{
    Properties
    {
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
            Ref 0
            Comp Less
            Pass Keep
        }
        ZTest Always
		
        Pass
        {
		    Blend One One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

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
                UNITY_SETUP_INSTANCE_ID (i);
				//中心距離場
				//float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				float D = smoothstep(-15.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

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
