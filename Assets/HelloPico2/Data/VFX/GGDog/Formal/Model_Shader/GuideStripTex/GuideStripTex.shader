// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Unlit/GGDog/Model_Shader/GuideStripTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowTex ("Glow Texture", 2D) = "white" {}
        _RandomrangeColor ("Color Random Seed", Range(0,1)) = 1
        _RandomrangeSpeed ("Speed Random Seed", Range(0,0.1)) = 0.07
        _BasicSpeed ("Basic Fly Speed", Range(0,1)) = 0.5
        _HueSpeed ("Hue Changing Speed", Range(0,1)) = 0.5
        _AlphaClip ("Alpha Clip", Range(-0.1,1)) = 0
    }
    SubShader
    {
        
        Pass
        {
            Tags { "RenderType"="Opaque" }
            Blend SrcAlpha OneMinusSrcAlpha
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            
            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
            {
                 Out = floor(In / (1 / Steps)) * (1 / Steps);
            }
            
            void Unity_ColorspaceConversion_RGB_RGB_float(float3 In, out float3 Out)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                Out = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
            }
            void Unity_Hue_Radians_float(float3 In, float Offset, out float3 Out)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
                float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            
                float hue = hsv.x + Offset;
                hsv.x = (hue < 0)
                        ? hue + 1
                        : (hue > 1)
                            ? hue - 1
                            : hue;
            
                // HSV to RGB
                float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
                Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
            }

            float _RandomrangeColor;
            float _RandomrangeSpeed;
            float _BasicSpeed;
            float _HueSpeed;
            

            float _AlphaClip;
            float _Hue;
            
            float4 frag (v2f i) : SV_Target
            {
                
                UNITY_SETUP_INSTANCE_ID (i);

                float4 k;
                
                Unity_Posterize_float4( float4( i.uv.x,1,1,1),8,k);

                float RandomSpeed = (random(k.xy*_RandomrangeSpeed)/2+_BasicSpeed)*_Time.y/2;

                i.uv.y -= RandomSpeed;

                fixed4 col = tex2D(_MainTex, i.uv);

                float4 FadeColor_Shadow;
                    Unity_Posterize_float4( float4( i.uv.y*1.5,i.uv.x,i.uv.x,1),8,FadeColor_Shadow);

                    float r = random(FadeColor_Shadow.xy*_RandomrangeColor);

                    Unity_ColorspaceConversion_RGB_RGB_float(float3(r*r,1,1),FadeColor_Shadow.xyz);
                    Unity_Hue_Radians_float(FadeColor_Shadow.xyz,-_Time.y*_HueSpeed/5,FadeColor_Shadow.xyz);

                FadeColor_Shadow.rgb = clamp(FadeColor_Shadow.rgb,0.25,1);
                FadeColor_Shadow = lerp(FadeColor_Shadow*FadeColor_Shadow,0.5,0.75);


                float4 FadeColor_Light;
                    Unity_ColorspaceConversion_RGB_RGB_float(float3(r,1,1),FadeColor_Light.xyz);
                    Unity_Hue_Radians_float(FadeColor_Light.xyz,_Time.y*_HueSpeed,FadeColor_Light.xyz);

                FadeColor_Light.rgb = clamp(FadeColor_Light.rgb,0,1);
                FadeColor_Light = lerp(FadeColor_Light,0.75,0.6)*2.75;

                float colr = col.r;
                
                col.rgb = lerp(FadeColor_Light,col.rgb,saturate(smoothstep(0,0.25,colr)-0.5));

                col.rgb = lerp(FadeColor_Shadow,col.rgb,saturate(smoothstep(0,1.25,colr)));
                
                clip(col.a-_AlphaClip);
                

                return col;
            }
            ENDCG
        }
        
        Pass
        {
            Tags { "Queue"="Transparent" }
            Blend SrcAlpha One
            ZWrite Off
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _GlowTex;
            float4 _GlowTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _GlowTex);
                return o;
            }
            
            
            float random (float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            void Unity_Posterize_float4(float4 In, float4 Steps, out float4 Out)
            {
                 Out = floor(In / (1 / Steps)) * (1 / Steps);
            }
            
            void Unity_ColorspaceConversion_RGB_RGB_float(float3 In, out float3 Out)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                Out = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
            }
            void Unity_Hue_Radians_float(float3 In, float Offset, out float3 Out)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 P = lerp(float4(In.bg, K.wz), float4(In.gb, K.xy), step(In.b, In.g));
                float4 Q = lerp(float4(P.xyw, In.r), float4(In.r, P.yzx), step(P.x, In.r));
                float D = Q.x - min(Q.w, Q.y);
                float E = 1e-10;
                float3 hsv = float3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            
                float hue = hsv.x + Offset;
                hsv.x = (hue < 0)
                        ? hue + 1
                        : (hue > 1)
                            ? hue - 1
                            : hue;
            
                // HSV to RGB
                float4 K2 = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
                Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
            }

            float _RandomrangeColor;
            float _RandomrangeSpeed;
            float _BasicSpeed;
            float _HueSpeed;
            

            float _AlphaClip;
            float _Hue;
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                float4 k;

                Unity_Posterize_float4( float4( i.uv.x,1,1,1),8,k);

                float RandomSpeed = (random(k.xy*_RandomrangeSpeed)/2+_BasicSpeed)*_Time.y/2;

                i.uv.y -= RandomSpeed;

                fixed4 col = tex2D(_GlowTex, i.uv);

                float4 FadeColor_Shadow;
                    Unity_Posterize_float4( float4( i.uv.y*1.5,i.uv.x,i.uv.x,1),8,FadeColor_Shadow);

                    float r = random(FadeColor_Shadow.xy*_RandomrangeColor);

                    Unity_ColorspaceConversion_RGB_RGB_float(float3(r*r,1,1),FadeColor_Shadow.xyz);
                    Unity_Hue_Radians_float(FadeColor_Shadow.xyz,0.1-_Time.y*_HueSpeed/5,FadeColor_Shadow.xyz);

                FadeColor_Shadow.rgb = clamp(FadeColor_Shadow.rgb,0.25,1);
                FadeColor_Shadow = lerp(FadeColor_Shadow*FadeColor_Shadow,0.5,0.75);

                float colr = col.r;
                
                col.rgb = lerp(FadeColor_Shadow,col.rgb,saturate(smoothstep(0,1.25,colr)));
                
                col.rgb *= saturate((colr));
                
                clip(colr-0.015);
                
                return col/1.5;
            }
            ENDCG
        }

    }
}
