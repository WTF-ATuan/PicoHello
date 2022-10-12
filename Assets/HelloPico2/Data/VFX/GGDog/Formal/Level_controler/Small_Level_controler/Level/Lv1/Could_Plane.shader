Shader "GGDog/Tunnel"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
        _Color ("Color", COLOR) = (1,1,1,1)
        _ShadowColor ("ShadowColor", COLOR) = (0.5,0.5,0.5,1)
        _ViewFarPos ("_ViewFarPos", Float) = 70
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                half4 vertex : SV_POSITION;

                half2 NdotV_FarFog : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			half2 unity_gradientNoise_dir(half2 p)
			{
				p = p % 289;
				half x = (34 * p.x + 1) * p.x % 289 + p.y;
				x = (34 * x + 1) * x % 289;
				x = frac(x / 41) * 2 - 1;
				return normalize(half2(x - floor(x + 0.5), abs(x) - 0.5));
			}
			
			half unity_gradientNoise(half2 p)
			{
				half2 ip = floor(p);
				half2 fp = frac(p);
				half d00 = dot(unity_gradientNoise_dir(ip), fp);
				half d01 = dot(unity_gradientNoise_dir(ip + half2(0, 1)), fp - half2(0, 1));
				half d10 = dot(unity_gradientNoise_dir(ip + half2(1, 0)), fp - half2(1, 0));
				half d11 = dot(unity_gradientNoise_dir(ip + half2(1, 1)), fp - half2(1, 1));
				fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
				return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
			}

			void Unity_GradientNoise_float(half2 UV, half Scale, out half Out)
			{
				Out = unity_gradientNoise(UV * Scale) + 0.5;
			}


            v2f vert (appdata v)
            {
                v2f o;
				
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

				//float noise = saturate( frac(sin(dot(v.uv, float2(12.9898, 78.233)+sin(_Time.y*0.00015) ))*43758.5453));

				half T =_Time.y*0.5;

				half Out;
				Unity_GradientNoise_float(v.uv.xy+half2(0.5,1)*T*0.1+half2(1.0,0.0)*T*0.1,30,Out);

				half Out2;
				Unity_GradientNoise_float(v.uv.xy-half2(0.75,0.5)*T*0.1+half2(1.0,0.0)*T*0.1,70,Out2);

				Out*=1;
				Out2*=0.5;
				Out*=Out2;
				
				half Out3;
				Unity_GradientNoise_float(v.uv.xy-half2(-1.0,0.0)*T*0.125  +half2(1.0,0.0)*T*0.1,10,Out3);
				
				half Out4;
				Unity_GradientNoise_float(v.uv.xy-half2(-3.0,0.0)*T*0.125  +half2(1.0,0.0)*T*0.1,7,Out4);
				Out3*=Out4;

				half Noise =  Out*40 + (0.5-Out3)*80;

				
				half Out5;
				Unity_GradientNoise_float(1.0*v.uv.xy-half2(-1.25,0.0)*T*0.4  +half2(1.0,0.0)*T*0.1,7,Out5);
				Noise+=Out5*100;


				half uv_dis = smoothstep(0.0,0.1,v.uv.xy.y*(1-v.uv.xy.y));

                o.vertex = UnityObjectToClipPos(v.vertex + 0.003*Noise*v.normal*uv_dis);

				
				half3 worldNormal = normalize(mul(v.normal,(half3x3)unity_WorldToObject));
				
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex+ 0.003*Noise*v.normal ).xyz;
				
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				
				//漸層外圍天空色
				//o.NdotV_FarFog.y = ((worldPos.z-0.5)*(worldPos.z-0.5)+(worldPos.x-0.5)*(worldPos.x-0.5))*0.03;
				half worldPos2 = mul(unity_ObjectToWorld, v.vertex).z;
                o.NdotV_FarFog.y = worldPos2;

				//Rim的NDotV
				o.NdotV_FarFog.x = dot(worldNormal,worldViewDir);

                return o;
            }
			
            half4 _SkyColor;
			half4 _Color;
			half4 _ShadowColor;
            half _ViewFarPos;
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
				
                half4 col = 1;
				
				col.rgb = lerp(_ShadowColor,_Color,col.r);
				
				col.rgb = lerp(col,_Color,smoothstep(0.5,1.0,col.r));
				
				half4 Rim = 1-saturate(smoothstep(-0.5,1.0,i.NdotV_FarFog.x));
				
				col.rgb = lerp(col*_ShadowColor,_Color,Rim);
				
				
                half Z =smoothstep(100,_ViewFarPos,i.NdotV_FarFog.y);
                //後方暗漸層
                col.rgb = lerp(col.rgb,_SkyColor, Z);


                return col;
            }
            ENDCG
        }
    }
}
