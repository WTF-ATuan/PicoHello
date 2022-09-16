Shader "GGDog/SSS"
{
    Properties
    {
        _DespairColor("Despair Color",Range(0,1)) = 0

        _FadeColor1("Fade Color1",Color) = (0.75,0.75,0.75,1)
        _FadeColor2("Fade Color2",Color) = (0.75,0.75,0.75,1)

        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        _Distortion ("Distortion", Range(0,1)) = 0.75
        [HDR]_SSSColor("SSS Color",Color) = (1,0.5,0.5,1)
		
        _LightDir ("LightDir", Vector) = (0,0,0,0)
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
                o.worldPos = mul(v.vertex,unity_WorldToObject).xyz;

                return o;
            }
			float _Distortion;

			float4 _Color;
			float4 _ShadowColor;
			float4 _SSSColor;
			
			float4 _LightDir;

			float4 _FadeColor1;
			float4 _FadeColor2;

			float _DespairColor;
            
            float4 frag (v2f i) : SV_Target
            {
			
                UNITY_SETUP_INSTANCE_ID(i);

				float Time_y = abs(fmod(_DespairColor * _Time.y*0.5,1.0f)*2.0f-1.0f);

                _FadeColor1.rgb = clamp(_FadeColor1.rgb,0.5,1);
                _FadeColor1 = lerp(_FadeColor1,0.75,0.5)*1.75*_Color;

                
                _FadeColor2.rgb = clamp(_FadeColor2.rgb,0.25,1);
                _FadeColor2 = lerp(_FadeColor2,0.75,0.5)*1.75*_ShadowColor;

                float3 WorldNormal = normalize(i.worldNormal);
                float3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

				float NdotL = saturate(dot(WorldNormal,_LightDir));
				float Rim = 1-saturate(dot(WorldNormal,ViewDir));

				float3 H = normalize( ViewDir + WorldNormal);
				float I = saturate(dot(ViewDir,-H));


                float FadeUV = saturate(frac(2*i.uv.y)+0.1);

                FadeUV = lerp(FadeUV,1.5,floor(frac(8*i.uv.x)*2)/2);

                _SSSColor = FadeUV*FadeUV/2;



				float4 FinalColor = lerp((_FadeColor2*FadeUV *Time_y + Rim* _SSSColor ),float4(1,0.96,0.84,1)* _FadeColor1*FadeUV ,NdotL ) ;

                return saturate(FinalColor);
                
            }
            ENDCG
        }
    }
}
