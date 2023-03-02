Shader "GGDog/Uber_ToonShader_GPU_Instance"
{
	//不透明體Blend: One Zero
	//半透明體Blend: SrcAlpha OneMinusSrcAlpha

	Properties
	{
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

		[Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend Mode", Float) = 0
		[Enum(Off,0,On,2)] _Cull ("Cull Mode", Float) = 0
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 4
		

        _BackLightDir("BackLight Dir",Vector) = (2,2,2,0)
        //_BackLightLerp("BackLightLerp",Range(0,1)) = 0
	}

	SubShader
	{
		Tags { "Queue"="Transparent" }
		
		Blend [_SourceBlend] [_DestBlend]
		
		Cull [_Cull]
		
		ZTest [_ZTest]

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
                float3 normal_VS : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
            sampler2D _MainTex;
            half4 _MainTex_ST;
			half3 _LightDir;
			half3 _BackLightDir;
			
		    uniform half _BackLightLerp;
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
			

			half4 frag (v2f i) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(i);
				
				half4 col = tex2D(_MainTex,i.uv.xy);
				clip(col.a-_AlphaClip);
				
				half Rim = smoothstep(0.7,0.9,i.uv.z);
				half Rim_Ambient = smoothstep(0,1,i.uv.z);

                i.normal_VS = float4(normalize(i.normal_VS.xyz),1);
				
                half N_VS_Dot_L = smoothstep(0,_LightSmooth,dot(i.normal_VS.xyz,_LightDir)-_LightRange);

				N_VS_Dot_L += smoothstep(0,1,dot(i.normal_VS.xyz,_LightDir)-0)*_BloomFade;

				col = lerp(col*_ShadowColor,col*_Color,N_VS_Dot_L/2);

				col += Rim*N_VS_Dot_L*_EdgeRimColor*col.a  +  _EdgeRimColor*saturate(0.75-N_VS_Dot_L)*Rim_Ambient *_AmbientFade*col.a;

				
                fixed Dir2 = saturate(dot(i.normal_VS.xyz,_BackLightDir));

                fixed4 BackLightcol = lerp( 0 , 1 , step(Dir2,0.5) );

                col = lerp(saturate(col),BackLightcol,_BackLightLerp);


				return col;
				
			}
			ENDCG
		}
		
	}

}
