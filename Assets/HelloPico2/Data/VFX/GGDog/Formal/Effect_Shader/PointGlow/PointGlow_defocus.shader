// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/PointGlow_defocus"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
        _Alpha("Alpha",Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		
		Zwrite Off
		Blend One One
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
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
				float CameraDistance : TEXCOORD1;
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
				
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

                return o;
            }
            
            float4 _Color;
            float _Alpha;

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//漸層度
				float D2 = smoothstep(0.75,1.5,D)*8;

				D = D2+smoothstep(0.5,1.5,D)*1.5;

				D = lerp(smoothstep(0,1.25,smoothstep(0.05*smoothstep(10,90,i.CameraDistance),0.125,D/3)/1.4) *(sin(_Time.y*1)*i.color.a+2)/2.2, D/3 ,smoothstep(90,120,i.CameraDistance) )*3;

				i.color = saturate(lerp(i.color*i.color,i.color,D));

				float4 finalColor = saturate(i.color*D*i.color.a);
				
				finalColor.a *= smoothstep(10,90,i.CameraDistance);

               // clip(finalColor.a - 0.0015);

                return finalColor*_Color*_Alpha;
            }
            ENDCG
        }
    }
}
