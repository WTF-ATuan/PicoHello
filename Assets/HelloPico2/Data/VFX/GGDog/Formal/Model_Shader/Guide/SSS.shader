Shader "GGDog/SSS"
{
    Properties
    {
        _DespairColor("Despair Color",Range(0,1)) = 0

        _FadeColor1("Fade Color1",Color) = (0.75,0.75,0.75,1)
        _FadeColor2("Fade Color2",Color) = (0.75,0.75,0.75,1)

        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        [HDR]_SSSColor("SSS Color",Color) = (1,0.5,0.5,1)


        _LightSmooth("Light Edge Smooth",Range(0,1)) = 0.1
        _LightRange("Light Edge Range",Range(-1,1)) = 0.15
        _BloomFade("Bloom Fade",Range(0,1)) = 1
    }
    SubShader
    {
        Tags {"RenderType"= "Geometry" "Queue"="Geometry" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half4 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 normal_VS : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
				
				half3 WorldNormal = normalize(mul(v.normal,(half3x3)unity_WorldToObject));
                half3 worldPos = mul(v.vertex,unity_WorldToObject).xyz;
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos );

                half3 LightDir = half3(-0.19,0,-0.82);

				o.uv.z = smoothstep(0,0.5,1-saturate(dot(WorldNormal,ViewDir)));
				o.uv.w = saturate(dot(WorldNormal,LightDir));
                
                //Toon光影用的normal
                float4 normal_OS = float4(v.normal.xyz,0);
                o.normal_VS = mul(UNITY_MATRIX_MV,normal_OS);

                return o;
            }
			half4 _Color;
			half4 _ShadowColor;
			half4 _SSSColor;
			

			half4 _FadeColor1;
			half4 _FadeColor2;

			half _DespairColor;
            
			half _BloomFade;
			half _LightSmooth;
			half _LightRange;

            half4 frag (v2f i) : SV_Target
            {
			
                UNITY_SETUP_INSTANCE_ID(i);

				half Time_y = abs(fmod(_DespairColor * _Time.y*0.5,1.0f)*2.0f-1.0f);

                _FadeColor1.rgb = clamp(_FadeColor1.rgb,0.5,1);
                _FadeColor1 = lerp(_FadeColor1,0.75,0.5)*1.75*_Color;

                
                _FadeColor2.rgb = clamp(_FadeColor2.rgb,0.25,1);
                _FadeColor2 = lerp(_FadeColor2,0.75,0.5)*1.75*_ShadowColor;

                half FadeUV = saturate(frac(2*i.uv.y)+0.1);

                FadeUV = lerp(FadeUV,1.5,floor(frac(8*i.uv.x)*2)/2);

                _SSSColor = FadeUV*FadeUV/2;

                


				half4 FinalColor = lerp((_FadeColor2*FadeUV *Time_y  + i.uv.z* _SSSColor),float4(1,0.96,0.84,1)* _FadeColor1*FadeUV ,i.uv.w ) ;

                
                //Toon光影

                i.normal_VS = float4(normalize(i.normal_VS.xyz),1);

                half3 _LightDir = half3(2,3,1);

                half N_VS_Dot_L = smoothstep(0,_LightSmooth,dot(i.normal_VS.xyz,_LightDir)-_LightRange);

				N_VS_Dot_L += smoothstep(0,1,dot(i.normal_VS.xyz,_LightDir)-0)*_BloomFade;
                
				FinalColor = lerp(FinalColor*_ShadowColor,FinalColor,N_VS_Dot_L/2 + i.uv.z)  ;



                return saturate(FinalColor);
                
            }
            ENDCG
        }
    }
}
