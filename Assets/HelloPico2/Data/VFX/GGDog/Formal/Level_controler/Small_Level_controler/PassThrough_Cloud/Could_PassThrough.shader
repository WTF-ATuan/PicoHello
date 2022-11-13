Shader "GGDog/Tunnel"
{
    Properties
    {
        _Color ("Color", COLOR) = (1,1,1,1)
        _DarkColor ("DarkColor", COLOR) = (0.5,0.5,0.5,1)
		_Alpha ("Alpha", Range(0,1)) = 1
		_FlowSpeed ("FlowSpeed", Range(2.5,10)) = 2.5
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
    }
    SubShader
    {
        Tags { "Queue"="Geometry-50" }

		//ZWrite Off
        //ZTest[_ZTest]
		
		Blend SrcAlpha OneMinusSrcAlpha
		
		//Cull Back
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
                half4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
                half4 NdotV_FarFog : TEXCOORD1;

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

			half _FlowSpeed;
            v2f vert (appdata v)
            {
                v2f o;
				//float noise = saturate( frac(sin(dot(v.uv, float2(12.9898, 78.233)+sin(_Time.y*0.00015) ))*43758.5453));

				half T =_Time.y*_FlowSpeed;

				half Out;
				Unity_GradientNoise_float(v.uv.xy+half2(1,0.5)*T/10+half2(0,1)*T/10,30,Out);

				half Out2;
				Unity_GradientNoise_float(v.uv.xy-half2(0.5,0.75)*T/10+half2(0,1)*T/10,70,Out2);

				Out*=1;
				Out2*=0.5;
				Out*=Out2;
				
				half Out3;
				Unity_GradientNoise_float(v.uv.xy-half2(0,-0.5)*T/8  +half2(0,1)*T/10,10,Out3);
				
				half Out4;
				Unity_GradientNoise_float(v.uv.xy-half2(0,-1.5)*T/8  +half2(0,1)*T/10,7,Out4);
				Out3*=Out4;

				half Noise =  Out*25 + (0.5-Out3)*50;

				
				half Out5;
				Unity_GradientNoise_float(0.5*v.uv.xy-half2(0,-1.5)*T/8  +half2(0,1)*T/10,7,Out5);
				Noise*=Out5*3.5;


				half uv_dis = smoothstep(0.1,0.2,v.uv.xy.x*(1-v.uv.xy.x));

                o.vertex = UnityObjectToClipPos(v.vertex + 0.0015*Noise*v.normal*uv_dis);

				
				half3 worldNormal = normalize(mul(v.normal,(half3x3)unity_WorldToObject));
				
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex ).xyz;
				
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				
				//RimªºNDotV
				o.NdotV_FarFog.x = dot(worldNormal,worldViewDir);

				o.NdotV_FarFog.zw = v.uv.xy;

				o.uv = v.uv;
                return o;
            }
			
			half4 _Color;
			half4 _DarkColor;
			
			half _Alpha;

            half4 frag (v2f i) : SV_Target
            {
                half4 col = 1;
				
				col.rgb = lerp(_DarkColor,_Color,col.r);
				
				col.rgb = lerp(col,_Color,smoothstep(0.5,1,col.r));
				
				half4 Rim = 1-saturate(smoothstep(-0.5,1,i.NdotV_FarFog.x));
				
				col.rgb = lerp(_DarkColor,_Color,Rim);
				
				half dis = smoothstep(0.1,0.25,i.uv.y)*(smoothstep(0.1,0.25,1-i.uv.y));

                return half4(col.rgb,dis);
            }
            ENDCG
        }
    }
}
