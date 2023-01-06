Shader "GGDog/Guide_Toon"
{
    Properties
    {
        _DespairColor("Despair Color",Range(0,1)) = 0

        _FadeColor1("FadeUV Color1 (Script Random)",Color) = (0.9,0.82,0.48,1)
        _FadeColor2("FadeUV Color2 (Script Random)",Color) = (1,0.94,0.7,1)

        _MainColor("Main Color",Color) = (1,0.82,0.64,1)
        _ShadowColor("Shadow Color",Color) = (0.49,0.5,0.8,1)


        _LightSmooth("Light Edge Smooth",Range(0,20)) = 0.3
        _LightRange("Light Edge Range",Range(-1,1)) = 1
        _BloomFade("Bloom Fade",Range(0,1)) = 0.5

        
        _ShadowSmooth("Shadow Edge Smooth",Range(0,20)) = 0.25
        _ShadowRange("Shadow Edge Range",Range(-1,1)) = 1
        _ShadowFadeUV("Shadow Fade Out With UV",Range(0,1)) = 0.45
        
        _LightDir("Light Dir",Vector) = (-1.5,1.5,-2,0)
        _ShadowDir("Shadow Dir",Vector) = (-2,2,-1,0)

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
			half4 _MainColor;
			half4 _ShadowColor;
			

			half4 _FadeColor1;
			half4 _FadeColor2;

			half _DespairColor;
            
			half _BloomFade;
			half _LightSmooth;
			half _LightRange;

			half3 _LightDir;
			half3 _ShadowDir;
            
			half _ShadowSmooth;
			half _ShadowRange;
			half _ShadowFadeUV;
            
		    uniform half4 _Guide_FarColor;
		    uniform half _Guide_Far;

            half4 frag (v2f i) : SV_Target
            {
			
				half Time_y = abs(fmod(_DespairColor * _Time.y*0.5,1.0f)*2.0f-1.0f);

                _FadeColor1.rgb = clamp(_FadeColor1.rgb,0.75,1)*1.5;

                _FadeColor2.rgb = clamp(_FadeColor2.rgb,0.25,0.5);

                half FadeUV = saturate(frac(2*i.uv.y)+0.1);

                FadeUV = lerp(FadeUV,1.5,floor(frac(8*i.uv.x)*2)/2);

                


				half4 FinalColor = lerp((_FadeColor2*FadeUV *Time_y ),float4(1,0.96,0.84,1)* _FadeColor1*FadeUV ,FadeUV ) ;

                
                //Toon光影

                i.normal_VS = float4(i.normal_VS.xyz,1);

                //half3 _LightDir = half3(2,3,1);

                half N_VS_Dot_L = smoothstep(0,_LightSmooth,dot(i.normal_VS.xyz,_LightDir)-_LightRange + _LightSmooth/2);

				N_VS_Dot_L += smoothstep(-3.0,1.0,dot(i.normal_VS.xyz,_LightDir))*_BloomFade*0.75;
                
                //_ShadowColor = lerp(_FadeColor1,_ShadowColor,0.5);

				FinalColor = lerp(FinalColor*_MainColor*1.25,FinalColor*1.5+i.uv.z*_FadeColor2,N_VS_Dot_L/2)  ;

                
                half N_VS_Dot_L_shadow = smoothstep(0,_ShadowSmooth*0.5,dot(i.normal_VS.xyz,_ShadowDir)-_ShadowRange+3.5 +_ShadowSmooth/4);
                
				//N_VS_Dot_L_shadow += 1-smoothstep(0,3,dot(i.normal_VS.xyz,_LightDir)+4)*_BloomFade;

                half4 shadowcol =_ShadowColor;

                shadowcol = lerp(shadowcol,FinalColor,smoothstep(_ShadowFadeUV,1,FadeUV));

				FinalColor = lerp( shadowcol +smoothstep(-0.5,1.5,i.uv.z)/3.5,FinalColor,N_VS_Dot_L_shadow)  ;

                FinalColor = lerp(FinalColor,_Guide_FarColor,i.CameraDistance*_Guide_FarColor.a/_Guide_Far);
                

                return saturate(FinalColor);
                

                
            }
            ENDCG
        }
    }
}
