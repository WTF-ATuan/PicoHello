Shader "GGDog/Uber_ToonShader"
{
	//不透明體Blend: One Zero
	//半透明體Blend: SrcAlpha OneMinusSrcAlpha

	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[HDR]_Color ("Main Color", Color) = (.5,.5,.5,1)
		
        _LightSmooth("Light Edge Smooth",Range(0,1)) = 0.05
        _LightRange("Light Edge Range",Range(-1,1)) = 0.25
        _BloomFade("Bloom Fade",Range(0,1)) = 0.5
		
		[HDR]_ShadowColor ("Shadow Color", Color) = (.5,.5,.5,1)
		
		[HDR]_EdgeRimColor ("Edge Rim Color", Color) = (.5,.5,.5,1)
        _AmbientFade("Ambient Rim Fade",Range(0,1)) = 0.2
		
        _AlphaClip("Alpha Clip",Range(0,1)) = 0.5
		_LightDir ("Light Direction", Vector) = (0.25, 0.75, 0 ,0)
		
		[Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend Mode", Float) = 0
		[Enum(Off,0,On,2)] _Cull ("Cull Mode", Float) = 0
	}

	SubShader
	{
		Tags { "Queue"="Transparent" }
		
		Blend [_SourceBlend] [_DestBlend]
		
		Cull [_Cull]
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
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

                half3 worldNormal  = normalize(UnityObjectToWorldNormal(v.normal));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                
				o.uv.z = 1-dot(worldNormal, viewDir);
				
				half3 halfVector = normalize(_LightDir + viewDir );
				o.uv.w = dot(worldNormal, halfVector);

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

				half Rim = smoothstep(0.7,0.9,i.uv.z);

				half Rim_Ambient = smoothstep(0,1,i.uv.z);

				half specular = smoothstep(0.5-_LightSmooth*0.5-_LightRange,0.5+_LightSmooth*0.5-_LightRange,i.uv.w)/2  + smoothstep(0,1,i.uv.w) *_BloomFade;

				half4 col = tex2D(_MainTex,i.uv.xy);
				clip(col.a-_AlphaClip);

				col = lerp(col*_ShadowColor,col*_Color,specular);

				//正邊緣光、環境邊緣光
				col += _EdgeRimColor*specular*Rim +  _EdgeRimColor*(0.75-specular)*Rim_Ambient *_AmbientFade;


				return saturate(col);
				
			}
			ENDCG
		}
		
	}

}
