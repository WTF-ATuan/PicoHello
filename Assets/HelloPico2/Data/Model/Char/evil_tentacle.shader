Shader "GGDog/evil_tentacle" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _AlphaClip ("AlphaClip", Float) = 0.5

        _ReflectTilling ("Reflect Tilling", Range(0, 5000)) = 100
        _Reflect ("Reflect", Range(0, 1)) = 1
        [HDR]_ReflectColor ("Reflect Color", Color) = (0, 0, 0, 0)
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }


        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };
            
            float2 Rotate_UV(float2 uv, float sin, float cos) {
                return float2(uv.x * cos - uv.y * sin, uv.x * sin + uv.y * cos);
            }
            float WaterTex(float2 uv, float Tilling, float FlowSpeed) {
                uv.xy *= Tilling / 50;
                float Time = _Time.y * FlowSpeed;

                uv.xy = Rotate_UV(uv, 0.34, 0.14);
                float2 UV = frac(uv.xy * 0.75 + Time * float2(-1, -0.25));
                float D = smoothstep(-10.4, 4.2, 1 - 38.7 * ((UV.x - 0.5) * (UV.x - 0.5) + (UV.y - 0.5) * (UV.y - 0.5)) - 1);
                
                uv.xy = Rotate_UV(uv, 0.94, 0.44);
                UV = frac(uv.xy * 1.2 + Time * 0.33 * float2(-0.24, 0.33));
                float D2 = smoothstep(-18.4, 4.2, 1 - 38.7 * ((UV.x - 0.5) * (UV.x - 0.5) + (UV.y - 0.5) * (UV.y - 0.5)) - 1);
                
                uv.xy = Rotate_UV(uv, 0.64, 0.74);
                UV = frac(uv.xy * 1 + Time * 1.34 * float2(0.54, -0.33));
                float D3 = smoothstep(-15.4, 4.2, 1 - 38.7 * ((UV.x - 0.5) * (UV.x - 0.5) + (UV.y - 0.5) * (UV.y - 0.5)) - 1);

                D = 1 - max(max(D, D2), D3);
                //D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _AlphaClip;
            float _Reflect;
            float _ReflectTilling;
            float3 _ReflectColor;
            CBUFFER_END
            
            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            

            
            float4 frag(v2f i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv);

                clip(col.a - _AlphaClip);
                
                col.rgb += WaterTex(i.worldPos.xy, _ReflectTilling, 1.25) * _Reflect * _ReflectColor;

                return col;
            }
            ENDCG

        }
    }
}
