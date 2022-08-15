Shader "Unlit/GGDog/Model_Shader/GuideStripTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            
            half random (half2 uv)
            {
                return frac(sin(dot(uv,half2(12.9898,78.233)))*43758.5453123);
            }

            void Unity_Posterize_half4(half4 In, half4 Steps, out half4 Out)
            {
                 Out = floor(In / (1 / Steps)) * (1 / Steps);
            }
            
            void Unity_ColorspaceConversion_RGB_RGB_half(half3 In, out half3 Out)
            {
                half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                half3 P = abs(frac(In.xxx + K.xyz) * 6.0 - K.www);
                Out = In.z * lerp(K.xxx, saturate(P - K.xxx), In.y);
            }
            void Unity_Hue_Radians_half(half3 In, half Offset, out half3 Out)
            {
                half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                half4 P = lerp(half4(In.bg, K.wz), half4(In.gb, K.xy), step(In.b, In.g));
                half4 Q = lerp(half4(P.xyw, In.r), half4(In.r, P.yzx), step(P.x, In.r));
                half D = Q.x - min(Q.w, Q.y);
                half E = 1e-10;
                half3 hsv = half3(abs(Q.z + (Q.w - Q.y)/(6.0 * D + E)), D / (Q.x + E), Q.x);
            
                half hue = hsv.x + Offset;
                hsv.x = (hue < 0)
                        ? hue + 1
                        : (hue > 1)
                            ? hue - 1
                            : hue;
            
                // HSV to RGB
                half4 K2 = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                half3 P2 = abs(frac(hsv.xxx + K2.xyz) * 6.0 - K2.www);
                Out = hsv.z * lerp(K2.xxx, saturate(P2 - K2.xxx), hsv.y);
            }

            half _RandomrangeColor;
            half _RandomrangeSpeed;
            half _BasicSpeed;
            half _HueSpeed;
            

            half _AlphaClip;
            half _Hue;
            
            half4 frag (v2f i) : SV_Target
            {
                

                half4 k;
                
                Unity_Posterize_half4( half4( i.uv.x,1,1,1),8,k);

                half RandomSpeed = (random(k.xy*_RandomrangeSpeed)/2+_BasicSpeed)*_Time.y/2;

                i.uv.y -= RandomSpeed;

                fixed4 col = tex2D(_MainTex, i.uv);

                half4 FadeColor_Shadow;
                    Unity_Posterize_half4( half4( i.uv.y*1.5,i.uv.x,i.uv.x,1),8,FadeColor_Shadow);

                    half r = random(FadeColor_Shadow.xy*_RandomrangeColor);

                    Unity_ColorspaceConversion_RGB_RGB_half(half3(r*r,1,1),FadeColor_Shadow.xyz);
                    Unity_Hue_Radians_half(FadeColor_Shadow.xyz,-_Time.y*_HueSpeed/5,FadeColor_Shadow.xyz);

                FadeColor_Shadow.rgb = clamp(FadeColor_Shadow.rgb,0.25,1);
                FadeColor_Shadow = lerp(FadeColor_Shadow*FadeColor_Shadow,0.5,0.75);


                half4 FadeColor_Light;
                    Unity_ColorspaceConversion_RGB_RGB_half(half3(r,1,1),FadeColor_Light.xyz);
                    Unity_Hue_Radians_half(FadeColor_Light.xyz,_Time.y*_HueSpeed,FadeColor_Light.xyz);

                FadeColor_Light.rgb = clamp(FadeColor_Light.rgb,0,1);
                FadeColor_Light = lerp(FadeColor_Light,0.75,0.6)*2.75;

                half colr = col.r;
                
                col.rgb = lerp(FadeColor_Light,col.rgb,saturate(smoothstep(0,0.25,colr)-0.5));

                col.rgb = lerp(FadeColor_Shadow,col.rgb,saturate(smoothstep(0,1.25,colr)));
                
                clip(col.a-_AlphaClip);
                

                return col;
            }
            ENDCG
        }

    }
}
