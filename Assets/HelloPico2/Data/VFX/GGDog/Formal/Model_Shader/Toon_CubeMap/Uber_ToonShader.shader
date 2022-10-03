Shader "GGDog/Uber_ToonShader"
{
	//不透明體Blend: One Zero
	//半透明體Blend: SrcAlpha OneMinusSrcAlpha

	Properties
	{
		[HDR]_Color ("Main Color", Color) = (.5,.5,.5,1)
		[HDR]_ShadowColor ("Shadow Color", Color) = (.5,.5,.5,1)
		[HDR]_SpecularColor ("Specular Color", Color) = (.5,.5,.5,1)
		
        _AlphaClip("Alpha Clip",Range(0,1)) = 0.5

		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
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
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
            sampler2D _MainTex;
            half4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
				return o;
			}
			
			half3 _Vector;
			half4 _Color;
			half4 _ShadowColor;
			half4 _SpecularColor;
			half _AlphaClip;
			
			half4 frag (v2f i) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(i);

                half3 worldLightDir = _Vector;

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);

				half NdotV = dot(worldNormal,worldViewDir);
				
				//Rim
				half Rim = saturate(1-smoothstep(-0.15,0.35 ,NdotV-dot(worldViewDir,worldLightDir)/15))/1.5;
				Rim += saturate(1-smoothstep(0,1.5,NdotV))/1.75;
				
				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );
				half specular = dot(viewDir,reflectDir);

                half Specular =  max(0,specular);
                half Ambient_Specular =  max(0,specular+0.25);

				//Ambient_Specular = smoothstep(0.35,1,Ambient_Specular*5);
				
				Ambient_Specular = (smoothstep(0.35,1,Ambient_Specular*5)-smoothstep(-3,5,Ambient_Specular*1.15)) * saturate(smoothstep(0,1.5,NdotV));

				


				Specular = smoothstep(0.95,1,Specular/1.25);
				

				half4 col = tex2D(_MainTex, i.uv);
				
				clip(col.a-_AlphaClip);

				col = lerp(col*_ShadowColor,col*_Color,saturate(Ambient_Specular+Rim));
				
				return half4(col.rgb + Specular*_SpecularColor.rgb ,col.a);
				
			}
			ENDCG
		}
		
	}

}
