Shader "GGDog/Env_Model/DepthFog"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _FogColor("Fog Color", Color) = (1,1,1,1)
		_Far("Far",Range(0,500))=100
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				fixed4 vertex : SV_POSITION;
			    fixed4 screenPos : TEXCOORD1;
			    fixed3 worldPos : TEXCOORD3;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			fixed4 _FogColor;
			
			sampler2D _CameraDepthTexture;
	
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
			    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			    o.screenPos = ComputeScreenPos(o.vertex);
			    COMPUTE_EYEDEPTH(o.screenPos.z);

				return o;
			}
			fixed _Far;
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				fixed partZ = saturate(smoothstep(_Far/10,_Far, i.screenPos.z));
				
				col = lerp( col , fixed4(0,0,0,1) , (smoothstep(0,0.75,partZ))  );

				col = lerp( col , _FogColor , partZ  );

				return col;
			}
			ENDCG
		}
	}
}
