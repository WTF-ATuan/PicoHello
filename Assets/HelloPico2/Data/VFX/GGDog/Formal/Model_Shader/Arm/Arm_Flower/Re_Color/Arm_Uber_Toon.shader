Shader "GGDog/Arm_Uber_Toon"
{
	//不透明體Blend: One Zero
	//半透明體Blend: SrcAlpha OneMinusSrcAlpha

	Properties
	{
        _h("Uplevel",Range(0,1)) = 0
        _injured("Injured",Range(0,1)) = 0
		_Layer("Layer",Range(0,30)) = 1
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[HDR]_Color ("Main Color", Color) = (1.35,1.08,0.97,1)
		
        _LightSmooth("Light Edge Smooth",Range(0,1)) = 0.1
        _LightRange("Light Edge Range",Range(-1,1)) = 0.15
        _BloomFade("Bloom Fade",Range(0,1)) = 1
		
		[HDR]_ShadowColor ("Shadow Color", Color) = (.53,.53,.6,1)
		
		[HDR]_EdgeRimColor ("Edge Rim Color", Color) = (.67,.36,.2,1)
        _AmbientFade("Ambient Rim Fade",Range(0,1)) = 0.3
		
		_LightDir ("Light Direction", Vector) = (0.5, 1, 1 ,0)

	}

	SubShader
	{
		Tags { "Queue"="Transparent" }

        Stencil {
            Ref [_Layer]
            Comp always
            Pass replace
        }
		
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
				half4 vertexUV : TEXCOORD2;
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

            sampler2D _MainTex;
            half4 _MainTex_ST;
			half3 _LightDir;

			v2f vert (appdata v)
			{
				v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

                float4 normal_OS = float4(v.normal.xyz,0);
                o.normal_VS = mul(UNITY_MATRIX_MV,normal_OS);
				

                half3 worldNormal  = normalize(UnityObjectToWorldNormal(v.normal));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				o.uv.z = 1-dot(worldNormal, viewDir);
				
				o.vertexUV.xyz = v.vertex.xyz;

				return o;
			}
			
			half4 _Color;
			half4 _ShadowColor;
			half4 _EdgeRimColor;
			half _BloomFade;
			half _AmbientFade;
			half _LightSmooth;
			half _LightRange;
			half _injured;
			
			half4 frag (v2f i) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(i);
				
				half4 col = tex2D(_MainTex,i.uv.xy);
				half Rim = smoothstep(0.7,0.9,i.uv.z);
				half Rim_Ambient = smoothstep(0,1,i.uv.z);

                i.normal_VS = float4(normalize(i.normal_VS.xyz),1);

                half N_VS_Dot_L = smoothstep(0,_LightSmooth,dot(i.normal_VS.xyz,_LightDir)-_LightRange);

				N_VS_Dot_L += smoothstep(0,1,dot(i.normal_VS.xyz,_LightDir)-0)*_BloomFade;

				col = lerp(col*_ShadowColor,col*_Color,N_VS_Dot_L/2);

				col += Rim*N_VS_Dot_L*_EdgeRimColor*col.a  +  _EdgeRimColor*saturate(0.75-N_VS_Dot_L)*Rim_Ambient *_AmbientFade*col.a;
				
				
				half D_noise;
				Unity_GradientNoise_float(i.vertexUV.xy+_Time.y*0.1,50,D_noise);

				half D;
				Unity_GradientNoise_float((i.vertexUV.xy+i.vertexUV.yz)*0.5 + D_noise*0.01,40,D);
				D-=_injured;
				
				return saturate(col *(smoothstep(0,0.5,saturate(D+smoothstep(-10,0.5,1-_injured)))+0.25) );
				
			}
			ENDCG
		}
		
	}

}
