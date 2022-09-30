Shader "Unlit/LoadingCircle"
{
    Properties
    {
        _Value("Value",Range(0,1))=1
		_Color("Circle Color",Color) = (1,1,1,1)
        _Scale("scale",Range(0,1))=0.4
        _Width("width",Range(0.3,0.35))=0.35
        
		_GlowColor("Glow Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }

        ZTest Always

        Pass
        {
            ZWrite Off
            Blend SrcAlpha One

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

            v2f vert (appdata v)
            {
                v2f o;

                

				float2 scale = float2(length(unity_ObjectToWorld._m00_m10_m20),length(unity_ObjectToWorld._m01_m11_m21));
				float4 positionWS = unity_ObjectToWorld._m03_m13_m23_m33;
				float2 bias = v.vertex.xy;
				
				float3 forward = normalize(_WorldSpaceCameraPos - positionWS.xyz);

				float3 up = normalize(UNITY_MATRIX_V[1].xyz);
				float3 right = cross(forward, up);
				positionWS.xyz += right * bias.r * scale.x + up * bias.g  * scale.y;

				o.vertex = mul(UNITY_MATRIX_VP, positionWS);

                o.uv = v.uv;
                return o;
            }

            float2 Polar(float2 UV )
            {
                float2 d = UV-0.5;
                float r = length(d)*2*1;
                float a = atan2(d.x,d.y)*1.0/6.28*1;
                return float2(r,a);
            }

            float _Value;

            float4 _Color;
            
            
            float _Scale;
            float _Width;
            fixed4 frag (v2f i) : SV_Target
            {
                
                i.uv = Polar(float2(i.uv.x ,1-i.uv.y));

                float circle = step(_Width,(i.uv.x-_Scale)*(1-i.uv.x+_Scale)+0.11);

                circle = circle*step(1.1*((1-_Value)-0.5),i.uv.y);

                clip(circle-0.5);

                return circle*_Color;
            }
            ENDCG
        }

        Pass
        {
        
            ZWrite Off
            Blend SrcAlpha One

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

            v2f vert (appdata v)
            {
                v2f o;
                

				float2 scale = float2(length(unity_ObjectToWorld._m00_m10_m20),length(unity_ObjectToWorld._m01_m11_m21));
				float4 positionWS = unity_ObjectToWorld._m03_m13_m23_m33/1.5;
				float2 bias = v.vertex.xy;
				
				float3 forward = normalize(_WorldSpaceCameraPos - positionWS.xyz);

				float3 up = normalize(UNITY_MATRIX_V[1].xyz);
				float3 right = cross(forward, up);
				positionWS.xyz += right * bias.r * scale.x + up * bias.g  * scale.y;

				o.vertex = mul(UNITY_MATRIX_VP, positionWS);

                o.uv = v.uv;
                return o;
            }
            float _Value;
            float4 _GlowColor;
            fixed4 frag (v2f i) : SV_Target
            {
                
                float d = smoothstep(-10.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                
                float d2 = 1-smoothstep(-4.4,1.7,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                d = d*d2*2.5*_Value;

                return d*_GlowColor;
            }
            ENDCG
        }
    }
}
