Shader "FunS/XR/Billboard"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Alpha("Alpha",Range(0,1)) = 1
		_Color("Color", Color) = (1,1,1,1)

		[Header(Option)]
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend", Float) = 6
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTestMode("ZTestMode", Float) = 4

		[KeywordEnum(None, Y)] _AXISFIXED("Axis fixed", Float) = 0.0

		_Axis("Axis", Vector) = (0,1,0,10)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		ZWrite Off
		Blend[_SrcBlend][_DstBlend]
		ZTest[_ZTestMode]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature _AXISFIXED_NONE _AXISFIXED_Y

			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma target 3.0

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				
				// VR SinglePassInstancing
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 positionCS : SV_POSITION;
				
				// VR SinglePassInstancing
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float3 _Axis;
			// GPU Instancing
			UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
				UNITY_DEFINE_INSTANCED_PROP(float, _Alpha)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert (appdata v)
			{
				v2f o;
				
				// VR SinglePassInstancing
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o); 
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float2 scale = float2(length(unity_ObjectToWorld._m00_m10_m20),length(unity_ObjectToWorld._m01_m11_m21));
				float4 positionWS = unity_ObjectToWorld._m03_m13_m23_m33;
				float2 bias = v.positionOS.xy;
				
				float3 forward = normalize(_WorldSpaceCameraPos - positionWS.xyz);

#if _AXISFIXED_NONE
				float3 up = normalize(UNITY_MATRIX_V[1].xyz);
				float3 right = cross(forward, up);
#elif _AXISFIXED_Y
				float3 up = _Axis;
				float3 right = normalize(cross(forward, up));
#endif

				positionWS.xyz += right * bias.x * scale.x + up * bias.y  * scale.y;

				o.positionCS = mul(UNITY_MATRIX_VP, positionWS);

				// GPU Instancing (UNITY_ACCESS_INSTANCED_PROP) Use "MaterialPropertyBlock" to set _MainTex_ST.
				float4 ST = UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ST);
				o.uv = v.uv.xy * ST.xy + ST.zw;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// GPU Instancing (UNITY_ACCESS_INSTANCED_PROP)
				fixed4 col = tex2D(_MainTex, i.uv) * UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
				col.a *= UNITY_ACCESS_INSTANCED_PROP(Props, _Alpha);

				return col;
			}
			ENDCG
		}
	}
}