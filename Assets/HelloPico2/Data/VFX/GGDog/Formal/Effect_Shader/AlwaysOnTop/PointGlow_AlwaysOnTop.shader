// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/Glow_AlwaysOnTop"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
        _Alpha("Alpha",Range(0,1)) = 1
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1

		Blend One One
		Zwrite Off
        ZTest[_ZTest]
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
            float4 _Color;
            
            float _Alpha;

            /*
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				//float D = smoothstep(-15.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

				//漸層度
				float D2 = smoothstep(0.75,1.5,D)*8;

				D = D2+smoothstep(0.5,1.5,D)*1.5;

				i.color = lerp(i.color*i.color,i.color,D);

                float4 col = i.color * D ;

                clip(col.a - 0.0015);

                return col*_Color* i.color.a*_Alpha;
            }*/

            //拿掉distance的省能版
            float4 frag (v2f i) : SV_Target
            {
				//中心距離場
				float D = smoothstep(-16,9.6,1-49*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
				float D2 = smoothstep(-32.5,47,1-460.9*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                D =D*D+D2*D2*13.5;

				i.color = lerp(i.color*i.color,i.color,D);

                float4 col = i.color * D ;

                clip(col.a - 0.0015);

                return col*_Color* i.color.a*_Alpha;
            }

            ENDCG
        }
    }
}
