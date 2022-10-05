Shader "Unlit/Loading"
{
    Properties
    {
		_Value ("Value", Range(0,1)) = 0.75
		_Width ("Width", Range(0.2,0.249)) = 0.24
		_Color("Circle Color",Color) = (1,1,1,1)
		_GlowColor("Glow Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        ZTest Always
        ZWrite Off
        Blend SrcAlpha One

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
            
            half _Value;
            half _Width;

            half4 _Color;
            half4 _GlowColor;
            fixed4 frag (v2f i) : SV_Target
            {

                half Value = saturate(step(_Width,(i.uv.y-0.15)*(1-i.uv.y+0.15) ) - step(_Value, i.uv.x));

                
                half Glow = saturate(smoothstep(0.05,0.55,(i.uv.y-0.15)*(1-i.uv.y+0.15) ) - smoothstep(_Value,_Value+0.35, i.uv.x+0.075*(1-_Value)));

                return Value*_Color + Glow*3*smoothstep(0,0.35,_Value)*_GlowColor;
            }
            ENDCG
        }
    }
}
