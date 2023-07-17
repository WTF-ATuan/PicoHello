Shader "GGDog/Uber_ToonShader" {
    //不透明體Blend: One Zero
    //半透明體Blend: SrcAlpha OneMinusSrcAlpha

    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        [HDR]_Color ("Main Color", Color) = (1.35, 1.08, 0.97, 1)
        
        [KeywordEnum(ViewNormal, ObjectNormal)] _PROJECTION ("Light ProjectMode", Float) = 0.0

        _LightSmooth ("Light Edge Smooth", Range(0, 1)) = 0.1
        _LightRange ("Light Edge Range", Range(-1, 1)) = 0.15
        _BloomFade ("Bloom Fade", Range(0, 1)) = 1
        
        [HDR]_ShadowColor ("Shadow Color", Color) = (.53, .53, .6, 1)
        
        _ShadowDir ("Shadow Direction", Vector) = (0, 1, 0.25, 0)
        [HDR]_O_ShadowColor ("Occlusion Shadow Color", Color) = (.53, .53, .6, 1)
        _O_ShadowColorRange ("Occlusion Shadow Range", Range(0, 1)) = 0.65
        _O_ShadowColorCover ("Occlusion Shadow Cover", Range(0, 1)) = 0.5
         
        [HDR]_EdgeRimColor ("Edge Rim Color", Color) = (.67, .36, .2, 1)
        _AmbientFade ("Ambient Rim Fade", Range(0, 1)) = 0.3
        [HDR]_FarFogColor ("Far Fog Color", Color) = (.5, .5, .5, 0)
        
        _AlphaClip ("Alpha Clip", Range(0, 1)) = 0.35
        _LightDir ("Light Direction", Vector) = (0.5, 1, 1, 0)
        
        [HDR]_SelfShadowColor ("Self Shadow Color", Color) = (0.5, 0.5, 0.5, 1)
        _SelfShadowSmooth ("Self Shadow Gradient Smooth", Range(0, 10)) = 0.3
        _SelfShadowOffSet ("Self Shadow Gradient OffSet", Range(-1, 10)) = 0

        [Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend Mode", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend Mode", Float) = 0
        [Enum(Off, 0, On, 2)] _Cull ("Cull Mode", Float) = 0
        [Enum(Order, 4, AlwaysOnTop, 8)] _ZTest ("ZTest", Float) = 4

        
        _BackLightDir ("BackLight Dir", Vector) = (2, 2, 2, 0)
        //_BackLightLerp ("BackLightLerp", Range(0,1)) = 0

    }

    SubShader {
        Tags { "Queue" = "Transparent" }
        
        Blend [_SourceBlend] [_DestBlend]
        
        Cull [_Cull]

        ZTest [_ZTest]

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            #pragma shader_feature _PROJECTION_VIEWNORMAL _PROJECTION_OBJECTNORMAL

            #include "UnityCG.cginc"

            struct appdata {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f {
                half4 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 normal_VS : TEXCOORD1;
                half4 normal : TEXCOORD2;
            };
            
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            half4 _MainTex_ST;
            half3 _LightDir;
            half3 _BackLightDir;
            half4 _Color;
            half4 _ShadowColor;
            half4 _O_ShadowColor;
            half3 _ShadowDir;
            
            half _O_ShadowColorRange;
            half _O_ShadowColorCover;
            
            
            half4 _EdgeRimColor;
            half _AlphaClip;
            half _BloomFade;
            half _AmbientFade;
            half _LightSmooth;
            half _LightRange;

            half4 _SelfShadowColor;
            half _SelfShadowSmooth;
            half _SelfShadowOffSet;
            half4 _FarFogColor;
            CBUFFER_END

            uniform half _BackLightLerp;

            v2f vert(appdata v) {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                
                //ViewNormal
                #if _PROJECTION_VIEWNORMAL

                    float4 normal_OS = float4(v.normal.xyz, 0);
                    o.normal_VS.xyz = mul(UNITY_MATRIX_MV, normal_OS);
                    
                    //Normal
                #elif _PROJECTION_OBJECTNORMAL

                    o.normal_VS.xyz = v.normal;
                    
                #endif

                half3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.uv.z = 1 - dot(worldNormal, viewDir);
                
                o.uv.w = v.vertex.y;
                
                o.normal_VS.w = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
                
                o.normal_VS.w = saturate(smoothstep(35, 70, o.normal_VS.w));
                
                o.normal.xyz = (UnityObjectToWorldNormal(v.normal));

                return o;
            }
            
            half4 frag(v2f i) : SV_Target {
                half4 col = tex2D(_MainTex, i.uv.xy);
                clip(col.a - _AlphaClip);
                
                half Rim = smoothstep(0.7, 0.9, i.uv.z);
                half Rim_Ambient = i.uv.z;
                

                
                //NDotL_SmoothShadow
                half n_vs_dot_2 = dot(i.normal_VS.xyz, _ShadowDir);
                half N_VS_Dot_L2 = smoothstep(-0.5, 1, n_vs_dot_2 +_O_ShadowColorRange);
                col = lerp(lerp(_O_ShadowColor, col , N_VS_Dot_L2),col,_O_ShadowColorCover);
                


                half n_vs_dot_l = dot(i.normal_VS.xyz, _LightDir);

                //NDotL
                half N_VS_Dot_L = smoothstep(0.0, _LightSmooth, n_vs_dot_l - _LightRange);
                

                //BloomFade
                N_VS_Dot_L += smoothstep(-3.0, 1.0, n_vs_dot_l) * _BloomFade * 0.75;

                col = lerp(col * _ShadowColor, col * _Color, N_VS_Dot_L / 2);
                

                col += Rim * N_VS_Dot_L * _EdgeRimColor * col.a + _EdgeRimColor * saturate(0.75 - N_VS_Dot_L) * Rim_Ambient * _AmbientFade * col.a;
                
                
                //NDotL_O_ShadowColor
                half N_VS_Dot_L3 = smoothstep(0.0, 0.025, n_vs_dot_2 +_O_ShadowColorRange);
                col = lerp(lerp(_O_ShadowColor, col , N_VS_Dot_L3),col,_O_ShadowColorCover);


                //自陰影
                i.uv.w = 1 - smoothstep(-_SelfShadowSmooth, _SelfShadowSmooth, i.uv.w + _SelfShadowOffSet);

                col = lerp(col, _SelfShadowColor, i.uv.w * _SelfShadowColor.a);


                col.rgb = lerp(col.rgb, _FarFogColor.rgb, i.normal_VS.w * _FarFogColor.a);


                fixed Dir2 = saturate(dot(i.normal_VS.xyz, _BackLightDir));

                fixed4 BackLightcol = lerp(0, 1, step(Dir2, 0.5));

                col = lerp(saturate(col), BackLightcol, _BackLightLerp);
                

                return col;
            }
            ENDCG

        }
    }
}
