Shader "GGDog/Smoke"
{
	Properties
	{
		_Color ("Tint", Color) = (1, 1, 1, 1)
		_ShadowColor ("Shadow", Color) = (0, 0, 0, 1)
		_MainTex ("Texture", 2D) = "white" {}
		_FlowSpeed ("FlowSpeed", Range(0,2)) = 0.3

		_IntersectWidth("IntersectWidth", Range(0, 3.5)) = 1.1

		[ShowAsVector2]_SmoothStepMinMax("_SmoothStep_Min/Max", Vector) = (0.25,2,0,0)
	}

	SubShader{
		Tags{ "RenderType"="transparent" "Queue"="transparent"}

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

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
				float4 color : COLOR;
				float3 normal : NORMAL;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float3 worldPos : TEXCOORD0;
				float3 normal : NORMAL;
			    float4 screenPos : TEXCOORD1;
				float3 WorldScale : TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				
			    o.screenPos = ComputeScreenPos(o.vertex);
			    COMPUTE_EYEDEPTH(o.screenPos.z);

				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldPos = worldPos.xyz;

				float3 worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.normal = normalize(worldNormal);


				o.WorldScale = float3(
					length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
					length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
					length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
					);
				o.color = v.color;
				return o;
			}
			float4 _ShadowColor;
			
			float _FlowSpeed;
			
			sampler2D _CameraDepthTexture;
	
			float _IntersectWidth;
			float2 _SmoothStepMinMax;
			
			float4 frag(v2f i) : SV_TARGET
			{
				float2 uv_front = TRANSFORM_TEX(i.worldPos.xy, _MainTex)+_Time.y*float2(1,1)*_FlowSpeed*i.WorldScale.z;
				float2 uv_side = TRANSFORM_TEX(i.worldPos.zy, _MainTex)+_Time.y*float2(1,1)*_FlowSpeed*i.WorldScale.x;
				float2 uv_top = TRANSFORM_TEX(i.worldPos.xz, _MainTex)+_Time.y*float2(1,1)*_FlowSpeed*i.WorldScale.y;
				
				float4 col_front = tex2D(_MainTex, uv_front/i.WorldScale.z);
				float4 col_side = tex2D(_MainTex, uv_side/i.WorldScale.x);
				float4 col_top = tex2D(_MainTex, uv_top/i.WorldScale.y);
				
				float3 weights = i.normal;
				weights = abs(weights);
				weights = smoothstep(0,0.8,weights);
				weights = weights / (weights.x + weights.y + weights.z);

				col_front *= weights.z;
				col_side *= weights.x;
				col_top *= weights.y;

				float4 col = col_front + col_side + col_top;
				col = smoothstep(0,0.75,col);


				_SmoothStepMinMax.y *= (1-i.color.a)*4+1;

				
				float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				float3 worldNormal = normalize(i.normal);

				float Rim = saturate(smoothstep(_SmoothStepMinMax.x-0.5,_SmoothStepMinMax.y,dot(worldNormal,worldViewDir)-0.5));



				
			   //判斷相交
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
			    float partZ = i.screenPos.z;
			    float diff = sceneZ - partZ;
			    float intersect =  saturate( (1-diff)*0.5+_IntersectWidth);
				
			    float intersect2 =  saturate( smoothstep(_SmoothStepMinMax.x,_SmoothStepMinMax.y*1,diff)  ); //柔化相交處
				intersect*=intersect2;


				col.a*=saturate(Rim*Rim*pow(0.25-col.r,2) *intersect2*intersect2*5);

				col.a = smoothstep(0,1,col.a*(col.r+col.g+col.b)/3);

				col.rgb = lerp(_ShadowColor,col.rgb*_Color,smoothstep(0.25,1,col.r));
				

				return col;
			}

			ENDCG
		}
	}
    FallBack "Diffuse"
}
