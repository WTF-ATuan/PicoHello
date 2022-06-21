Shader "Ground_WorldUV"{

	Properties{
		_Color ("Tint", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader{

		Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag


			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : NORMAL;
			};

			v2f vert(appdata v){
				v2f o;

				o.position = UnityObjectToClipPos(v.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

				o.uv = TRANSFORM_TEX(worldPos.xz, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{

				fixed4 col = tex2D(_MainTex, i.uv);


				return col;
			}

			ENDCG
		}
	}
}