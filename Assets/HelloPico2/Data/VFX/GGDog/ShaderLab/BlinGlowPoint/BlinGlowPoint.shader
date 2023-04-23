// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Demo/BlinPointGlow"
{
    Properties
    {
        _DNoise_Tiling("Dynamic Noise_Tiling",Float) = 10
        _DNoise_Speed("Dynamic Noise_Speed",Float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            half _DNoise_Tiling;
            half _DNoise_Speed;

            half2 Rotate_UV(half2 uv, half sin, half cos)
            {
                return half2(uv.x * cos - uv.y * sin, uv.x * sin + uv.y * cos);
            }
            half WaterTex(half2 uv, half Tilling, half FlowSpeed)
            {
                uv.xy *= Tilling;
                half Time = _Time.y * FlowSpeed;

                uv.xy = Rotate_UV(uv, 0.34, 0.14);
                half2 UV = frac(uv.xy * 0.75 + Time * half2(-1.0, -0.25));
                half UV_Center = (UV.x - 0.5) * (UV.x - 0.5) + (UV.y - 0.5) * (UV.y - 0.5);
                half D = smoothstep(-10.4, 4.2, 1.0 - 38.7 * UV_Center - 1.0);

                uv.xy = Rotate_UV(uv, 0.94, 0.44);
                UV = frac(uv.xy * 1.2 + Time * 0.33 * half2(-1.74, 0.33));
                UV_Center = (UV.x - 0.5) * (UV.x - 0.5) + (UV.y - 0.5) * (UV.y - 0.5);
                half D2 = smoothstep(-18.4, 4.2, 1.0 - 38.7 * UV_Center - 1.0);

                D = max(D, D2);

                return D;
            }

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                half3 WorldUV = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.color.a *= 1-WaterTex(WorldUV.xz, _DNoise_Tiling, _DNoise_Speed);

                o.color.a *= o.color.a;

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                //¤¤¤ß¶ZÂ÷³õ

                float2 CenterUV = (i.uv.x - 0.5) * (i.uv.x - 0.5) + (i.uv.y - 0.5) * (i.uv.y - 0.5);

                float D = smoothstep(-16,9.6,1 - 49 * CenterUV - 1) * 0.85;
                float D2 = smoothstep(-31.5,47,1 - 460.9 * CenterUV - 1);

                D = i.color.a * (D * D + D2 * D2 * 20.5);

                clip(D - 0.0015);

                return i.color * D;
            }
            ENDCG
        }
    }
}
