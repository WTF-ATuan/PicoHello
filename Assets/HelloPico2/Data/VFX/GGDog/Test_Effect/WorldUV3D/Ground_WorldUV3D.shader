Shader "Ronja_Tutorial/Triplanar_Mapping"
// code refer: https://www.ronja-tutorials.com/post/010-triplanar-mapping/

{
	Properties{
		[HDR]_CoverColor ("_CoverColor", Color) = (1, 1, 1, 1)
		_CoverTex_Density ("_CoverTex_Density", Range(0, 1)) = 0
		_CoverTex ("_CoverTex", 2D) = "white" {}
		[HDR]_MainColor ("_MainColor", Color) = (0.5, 0.5, 0.5, 1)
		_MainTex ("_MainTex", 2D) = "white" {}
		
		_MainTex_Fade ("_MainTex_Fade", Range(0, 1)) = 0
		
		[Header(______ Cover Setting ______)]
		_Cover_Range ("_Cover_Range", Range(-1, 0)) = -0.25
		_Cover_Max ("_Cover_Max", Range(0.5, 1)) = 0.75
		_Cover_Sharpness ("_Cover_Sharpness", Range(0, 0.999)) = 0.5

	}

	SubShader{

		Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _CoverTex;
			float4 _CoverTex_ST;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _CoverColor;
			fixed4 _MainColor;

			float _CoverTex_Density;
			
			float _MainTex_Fade;

			float _Cover_Range;

			float _Cover_Max;
			float _Cover_Sharpness;
			
			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float3 worldPos : TEXCOORD0;
				float3 normal : NORMAL;
			};
			
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return half2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1.0,-0.25));
                half UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D = smoothstep(-10.4,4.2,1.0-38.7*UV_Center-1.0);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D2 = smoothstep(-18.4,4.2,1.0-38.7*UV_Center-1.0);
                
                D = max(D,D2);
                
                return D;
            }

			v2f vert(appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldPos = worldPos.xyz;

				float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.normal = normalize(worldNormal);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET
			{
				float2 uv_front = TRANSFORM_TEX(i.worldPos.xy, _MainTex);
				float2 uv_side = TRANSFORM_TEX(i.worldPos.zy, _MainTex);
				float2 uv_top = TRANSFORM_TEX(i.worldPos.xz, _CoverTex)  *_CoverTex_Density;
				
				fixed4 col_front = tex2D(_MainTex, uv_front);
				fixed4 col_side = tex2D(_MainTex, uv_side);
				fixed4 col_top = 1-tex2D(_CoverTex, uv_top - _Time.y*0.002);

				col_front = lerp(col_front*_MainColor,_MainColor,_MainTex_Fade);
				col_side = lerp(col_side*_MainColor,_MainColor,_MainTex_Fade);

				float3 weights = i.normal;
				weights.y = saturate(weights.y+_Cover_Range);
				weights.xz = abs(weights.xz)+(1-weights.y);
				
				weights.xz = max(0.5,weights.xz);

				weights = smoothstep((_Cover_Sharpness*_Cover_Max)*col_top,_Cover_Max*col_top,weights);
				

				weights = weights / (weights.x + weights.y + weights.z);
				
				
				col_front *= weights.z;
				col_side *= weights.x;

				col_top=smoothstep(-0.5,_CoverColor.a,col_top);
				col_top *= weights.y;

				//fixed4 col = (col_front + col_side)*_MainColor + col_top*_CoverColor;

				fixed4 col = lerp( (col_front + col_side) ,_CoverColor ,  col_top*3);
				

				return col;
			}

			ENDCG
		}
	}
}