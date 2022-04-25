Shader "GGDog/Env_Model/Shade+DepthFog"
{
	Properties
	{
        _MainTex ("Main Tex", 2D) = "white" {}
		
		_ColorHDR("ColorHDR",Range(0,5))=1
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _ShadowColor ("ShadowColor", Color) = (1, 1, 1, 1)
		

		[Header(Rim Color)]
		_RimLight_Direct(" RimLight Direct",Range(0,1.5)) = 1
		_Rim_Direct_Pow("Rim Direct Pow",Range(0,30)) = 30
        [HDR]_RimColor_Direct ("RimColor Direct", Color) = (1, 1, 1, 1)

		[Header(Depth Fog)]
		_Far("Far",Range(0,500))=100
        _FogColor("Fog Color", Color) = (1,1,1,1)

		
	}
	SubShader
	{
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		
		Pass
		{
            Tags { "LightMode"="ForwardBase" }
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_fwdbase
 
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
 
			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
			    half4 screenPos : TEXCOORD1;
			    half2 LightDirDOTNormal_ViewDirDOTNormal : TEXCOORD2;
			};

			sampler2D _MainTex;
			half4 _MainTex_ST;
			
			sampler2D _CameraDepthTexture;
	
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
			    o.screenPos = ComputeScreenPos(o.vertex);
			    COMPUTE_EYEDEPTH(o.screenPos.z);
				
			    float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                float3 worldLightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
 
				o.LightDirDOTNormal_ViewDirDOTNormal.x = dot(worldLightDir,worldNormal);
				o.LightDirDOTNormal_ViewDirDOTNormal.y = dot(worldViewDir,worldNormal);

				o.LightDirDOTNormal_ViewDirDOTNormal.y = saturate(( 0.5 + o.LightDirDOTNormal_ViewDirDOTNormal.x) *( 1.15-o.LightDirDOTNormal_ViewDirDOTNormal.y ));

				return o;
			}
			
	        half _ColorHDR;

            half4 _Color;
            half4 _ShadowColor;
	        half _LightGradient;

	        half _RimLight_Direct;

            half4 _RimColor_Direct;
			
	        half _Rim_Direct_Pow;
			
			half _Far;
			half4 _FogColor;

			half4 frag (v2f i) : SV_Target
			{
			/*   主體色   */

                half4 col = tex2D (_MainTex, i.uv);
				
                half3 Light_Part = _LightColor0.rgb * col * _Color.rgb;//亮面
                half3 Shadow_Part = _LightColor0.rgb * col *_ShadowColor;//暗面
				
				col = half4( lerp(Shadow_Part,Light_Part*1, smoothstep(0,1,i.LightDirDOTNormal_ViewDirDOTNormal.x+0.25))  , 1);
				

			/*   邊緣光處理   */
			
				half Rim_Direct = i.LightDirDOTNormal_ViewDirDOTNormal.y*_RimLight_Direct; //邊緣光

				Rim_Direct = Rim_Direct*Rim_Direct*Rim_Direct*Rim_Direct;

				col = lerp(col,_RimColor_Direct,Rim_Direct*_RimColor_Direct.a);
				
			/*   深度霧   */

				half partZ = saturate(smoothstep(_Far/10,_Far, i.screenPos.z));
				col = lerp( col , _FogColor , partZ  );



				return col;
			}
			ENDCG
		}
	}
}
