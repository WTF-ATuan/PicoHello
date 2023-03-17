Shader "GGDog/Guide_Toon_simple"
{
    Properties
    {
        _GradientUVAdd("GradientUV Add",Range(0,1)) = 0.25

        _DespairColor("Despair Color",Range(0,1)) = 0

        _FadeColor1("FadeUV Color1 (Script Random)",Color) = (1,0.85,0.63,1)
        _FadeColor2("FadeUV Color2 (Script Random)",Color) = (0.35,0.15,1,1)
        
        _MainColor("Main Color",Color) = (1,0.82,0.64,1)

        _LightSmooth("Light Edge Smooth",Range(0,3)) = 0.3
        _LightRange("Light Edge Range",Range(-1,1)) = 0.5
        _BloomFade("Bloom Fade",Range(0,1)) = 1

        
        _LightDir("Light Dir",Vector) = (-2,2,-1,0)
    }
    SubShader
    {
        Tags {"RenderType"= "Geometry" "Queue"="Geometry" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 normal_VS : TEXCOORD1;
				half CameraDistance : TEXCOORD2;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
				
				half3 WorldNormal = normalize(mul(v.normal,(half3x3)unity_WorldToObject));
                half3 worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos );

                half3 LightDir = half3(-0.19,0,-0.82);

				o.uv.z = 1-smoothstep(0,0.5,(dot(WorldNormal,ViewDir)));
				o.uv.w = saturate(dot(WorldNormal,LightDir));
                
                //Toon光影用的normal
                half4 normal_OS = half4(v.normal.xyz,0);
                o.normal_VS = mul(UNITY_MATRIX_MV,normal_OS);
                
                o.CameraDistance = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);

                return o;
            }
            CBUFFER_START(UnityPerMaterial) 
			half4 _MainColor;
			half4 _FadeColor1;
			half4 _FadeColor2;

			half _DespairColor;
            
			half _BloomFade;
			half _LightSmooth;
			half _LightRange;

			half3 _LightDir;

			half _GradientUVAdd;
            CBUFFER_END

		    uniform half4 _Guide_FarColor;
		    uniform half _Guide_Far;

            
            half4 frag (v2f i) : SV_Target
            {
			
				half Time_y = abs(fmod(_DespairColor * _Time.y*0.5,1.0f)*2.0f-1.0f);

                _FadeColor1.rgb = clamp(_FadeColor1.rgb,0.75,1)*1.5;

                
                _FadeColor2.rgb = clamp(_FadeColor2.rgb,0.25,0.5);



                half FadeUV = saturate(frac(2*i.uv.y)+0.1);

                FadeUV = lerp(FadeUV,1.5,floor(frac(8*i.uv.x)*2)/2);
                
                FadeUV = saturate( FadeUV + _GradientUVAdd *smoothstep(0.1,1,1-FadeUV));
                


				half4 FinalColor = lerp((_FadeColor2*FadeUV *Time_y ),float4(1,0.96,0.84,1)* _FadeColor1*FadeUV ,FadeUV ) ;

                
                //Toon光影

                i.normal_VS = float4(normalize(i.normal_VS.xyz),1);

                //half3 _LightDir = half3(2,3,1);

                half N_VS_Dot_L = smoothstep(0,_LightSmooth,dot(i.normal_VS.xyz,_LightDir)-_LightRange);

				N_VS_Dot_L += smoothstep(0,1.5,dot(i.normal_VS.xyz,_LightDir)-0)*_BloomFade;
                
                //_ShadowColor = lerp(_FadeColor1,_ShadowColor,0.5);

				FinalColor = lerp(FinalColor*_MainColor,FinalColor*1.5+i.uv.z*_FadeColor2,N_VS_Dot_L/2)  ;
                
                FinalColor = lerp(FinalColor,_Guide_FarColor,smoothstep(0,1,i.CameraDistance*_Guide_FarColor.a/_Guide_Far));
                
                
                return saturate(FinalColor);
                

                
            }
            ENDCG
        }
    }
}
