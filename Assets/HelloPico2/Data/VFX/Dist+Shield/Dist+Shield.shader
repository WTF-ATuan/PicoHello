Shader "Unlit/Dist+Shield"
{
	Properties
	{
		[HDR]_OutLineColor("OutLineColor", Color) = (0,0,0,0)
		_IntersectWidth("IntersectWidth", Range(0, 3.5)) = 1.1
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
	    ZWrite Off
	    Cull Off
	    Tags
	    {
	        "RenderType" = "Transparent"
	        "Queue" = "Transparent"
	    }
        GrabPass
        {
            "_BackTex"           //每個Shader所使用的名稱不能重複，否則會造成兩個相同名稱的無法正常顯示雙方的背景畫面
        }

		Pass
		{
		    CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		    struct appdata
			{
			    float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};
			struct v2f
			{
			    float4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
				half4 scrPos : TEXCOORD1;
				float3 worldNormal:NORMAL;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
            sampler2D _BackTex;
			
			fixed4 _OutLineColor;
			v2f vert(appdata v)
			{
			    v2f o;
			    o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			    o.scrPos = ComputeScreenPos(o.vertex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			    return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				fixed3 worldViewDir = normalize(viewDir);

				fixed rim = smoothstep(0,1,1-dot(worldNormal,worldViewDir));


				fixed noise = tex2D(_MainTex, 3*i.uv + float2(-0.3,-1.5)*_Time.y).r;
				fixed noise2 = tex2D(_MainTex, 3*i.uv + float2(1.4,0.5)*_Time.y).r;

				fixed4 refrCol = tex2D(_BackTex, i.scrPos.xy/i.scrPos.w +(1-rim)*(noise+noise2)/100) ;
				
			    return refrCol;
			}
			ENDCG
		}

		Pass
		{	
			Cull Back
			Blend SrcAlpha One
		    CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
		    struct appdata
			{
			    float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};
			struct v2f
			{
			    float4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
			    float4 screenPos : TEXCOORD1;
				float3 worldNormal:NORMAL;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _CameraDepthTexture;
	
			float _IntersectWidth;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
            sampler2D _BackTex;

			v2f vert(appdata v)
			{
			    v2f o;
			    o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			    o.screenPos = ComputeScreenPos(o.vertex);
			    COMPUTE_EYEDEPTH(o.screenPos.z);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			    return o;
			}
			fixed4 _OutLineColor;
			fixed4 frag(v2f i) : SV_Target
			{
			   //判斷相交
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
				//float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.screenPos );
				//float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, i.screenPos )));
			    float partZ = i.screenPos.z;
			    float diff = sceneZ - partZ;
			    float intersect =  saturate( (1-diff)*0.5+_IntersectWidth);

			    float intersect2 =  saturate( smoothstep(0.0,0.25,diff)  ); //柔化相交處
				intersect*=intersect2;

				//輸出顏色: 相交+背面顏色、Rim邊緣光雙色混合
			    float4 col = saturate(intersect)*_OutLineColor ;
				
				fixed noise = tex2D(_MainTex, i.uv + float2(0.3,1)*_Time.y).r;
				fixed noise2 = tex2D(_MainTex, i.uv  +noise/10).r;
				
				col *= noise2;
				
				fixed3 worldNormal = normalize(i.worldNormal);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				fixed3 worldViewDir = normalize(viewDir);

				fixed rim = smoothstep(0,1,dot(worldNormal,worldViewDir));

				col = lerp( col,_OutLineColor,intersect2*(1-rim+0.5)/2);
				col = lerp( col,_OutLineColor,saturate((1-rim)-0.75)*noise2);
			    return col;
			}
			ENDCG
		}
	}
}
