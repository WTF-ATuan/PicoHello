Shader "GGDog/Only_Mask"
{
	Properties
	{
		_Layer("Mask Layer",Range(0,30)) = 3
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[HDR]_Color ("Main Color", Color) = (1.35,1.08,0.97,1)
		
        _LightSmooth("Light Edge Smooth",Range(0,1)) = 0.1
        _LightRange("Light Edge Range",Range(-1,1)) = 0.15
        _BloomFade("Bloom Fade",Range(0,1)) = 1
		
		[HDR]_ShadowColor ("Shadow Color", Color) = (.53,.53,.6,1)
		
		[HDR]_EdgeRimColor ("Edge Rim Color", Color) = (.67,.36,.2,1)
        _AmbientFade("Ambient Rim Fade",Range(0,1)) = 0.3
		
        _AlphaClip("Alpha Clip",Range(0,1)) = 0.35
		_LightDir ("Light Direction", Vector) = (0.5, 1, 1 ,0)
		
        _ReflectTilling ("Reflect Tilling", Range(0,5000)) = 100
        _Reflect ("Reflect", Range(0,1)) = 1
		[HDR]_ReflectColor("Reflect Color",Color) = (0,0,0,0)

		[Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend Mode", Float) = 0
		[Enum(Off,0,On,2)] _Cull ("Cull Mode", Float) = 0

        _BackLightDir("BackLight Dir",Vector) = (2,2,2,0)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		
		Blend [_SourceBlend] [_DestBlend]
		
		Cull [_Cull]

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
                float3 normal_VS : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
			};
			
            sampler2D _MainTex;
            half4 _MainTex_ST;
			half3 _LightDir;
			half3 _BackLightDir;
			
		    uniform half _BackLightLerp;

            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling/50;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,-0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-0.24,0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                D = 1-max(max(D,D2),D3);
                //D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }


			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float4 normal_OS = float4(v.normal.xyz,0);
                o.normal_VS = mul(UNITY_MATRIX_MV,normal_OS);
				

                half3 worldNormal  = normalize(UnityObjectToWorldNormal(v.normal));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
				o.uv.z = 1-dot(worldNormal, viewDir);
				

				return o;
			}
			
			half4 _Color;
			half4 _ShadowColor;
			half4 _EdgeRimColor;
			half _AlphaClip;
			half _BloomFade;
			half _AmbientFade;
			half _LightSmooth;
			half _LightRange;
			
            half _Reflect;
            half _ReflectTilling;
            half3 _ReflectColor;

			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex,i.uv.xy);
				clip(col.a-_AlphaClip);
				
				half Rim = smoothstep(0.7,0.9,i.uv.z);
				half Rim_Ambient = smoothstep(0,1,i.uv.z);

                i.normal_VS = float4(normalize(i.normal_VS.xyz),1);

                half N_VS_Dot_L = smoothstep(0,_LightSmooth,dot(i.normal_VS.xyz,_LightDir)-_LightRange);

				N_VS_Dot_L += smoothstep(0,1,dot(i.normal_VS.xyz,_LightDir)-0)*_BloomFade;

				col = lerp(col*_ShadowColor,col*_Color,N_VS_Dot_L/2);

				col += Rim*N_VS_Dot_L*_EdgeRimColor*col.a  +  _EdgeRimColor*saturate(0.75-N_VS_Dot_L)*Rim_Ambient *_AmbientFade*col.a;

				
                col.rgb += smoothstep(-0.05,0.75,WaterTex(i.worldPos.xy,_ReflectTilling,1.25) )*_Reflect*_ReflectColor ;
				

                fixed Dir2 = saturate(dot(i.normal_VS.xyz,_BackLightDir));

                fixed4 BackLightcol = lerp( 0 , 1 , step(Dir2,0.5) );

                col = lerp(saturate(col),BackLightcol,_BackLightLerp);

				return col;
				
			}
			ENDCG
		}
	}
}
