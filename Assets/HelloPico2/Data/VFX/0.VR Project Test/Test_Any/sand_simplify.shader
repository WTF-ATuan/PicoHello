Shader "Unlit/NewUnlitShader"
{

    Properties
	{
        _Color("Light Color",Color) = (1,1,1,1)
        _ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _Specular("_Specular Color",Color) = (1,1,1,1)
        _Noise ("Noise", Range(-1.1,1)) = 0.75
    }
    SubShader {
        Pass{
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag


            struct appdata {

                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f{
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD2;
                float3 worldNormal : TEXCOORD0;
                float3 worldVertex : TEXCOORD1;
            };
			
            fixed4 _Color;
            fixed4 _ShadowColor;
            fixed4 _Specular;

			v2f vert (appdata v)
			{
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

                o.worldVertex = mul(v.vertex,unity_WorldToObject).xyz;

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                o.uv = v.uv;

                return o;
            };

			half _Noise;
			fixed4 frag (v2f i) : SV_Target
			{
				half2 NoiseUV = i.uv*sin(100) * 143758.5453;
				half  NoiseTex = (saturate( sin(NoiseUV.x*100)*cos(NoiseUV.y*100)-_Noise)*2);

                fixed3 normalDir = normalize(i.worldNormal);

                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
                fixed3 ShadowColor = _ShadowColor;
                fixed3 LightColor  = _LightColor0* _Color ;

                fixed3 FinalColor =  lerp( ShadowColor, LightColor , max(dot(normalDir,lightDir),0)+0.25  );

				

                fixed3 reflectDir = reflect(-lightDir,normalDir);
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldVertex );
				
                //高光反射
                fixed3 specular = _LightColor0.rgb * pow(max(dot(normalDir,lightDir),0),2)  * NoiseTex *_Specular*1.5;
                       specular += _LightColor0.rgb * (1-max(dot(normalDir,lightDir),0))  * NoiseTex *_Specular*0.15;

                FinalColor = FinalColor + specular;

                return fixed4( FinalColor , 1 ) ;
            };

            ENDCG
        }
    }
}