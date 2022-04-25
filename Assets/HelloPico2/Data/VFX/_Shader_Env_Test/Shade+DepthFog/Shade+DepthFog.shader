Shader "GGDog/Env_Model/Shade+DepthFog"
{
	Properties
	{
        _MainTex ("Main Tex", 2D) = "white" {}
		
		_LightGradient("LightGradient",Range(0,1)) = 1
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
        _ShadowColor ("ShadowColor", Color) = (1, 1, 1, 1)
		

		[Header(Rim Color)]
		_RimLight_Direct(" RimLight Direct",Range(0,1.5)) = 1
		_Rim_Direct_Pow("Rim Direct Pow",Range(0,30)) = 30
        [HDR]_RimColor_Direct ("RimColor Direct", Color) = (1, 1, 1, 1)

		_RimLight_Reverse("RimLight Reverse",Range(0,3)) = 2
		_Rim_Reverse_Pow("Rim Reverse Pow",Range(0,15)) = 15
        [HDR]_RimColor_Reverse ("RimColor Reverse", Color) = (1, 1, 1, 1)
		
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
				fixed4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
                fixed3 normal : NORMAL;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				fixed4 vertex : SV_POSITION;
			    fixed4 screenPos : TEXCOORD1;
			    fixed3 worldPos : TEXCOORD3;
                fixed3 worldNormal : TEXCOORD2;
			};

			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			
			sampler2D _CameraDepthTexture;
	
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
			    o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			    o.screenPos = ComputeScreenPos(o.vertex);
			    COMPUTE_EYEDEPTH(o.screenPos.z);

				return o;
			}

            fixed4 _Color;
            fixed4 _ShadowColor;
	        fixed _LightGradient;

	        fixed _RimLight_Direct;
	        fixed _RimLight_Reverse;

            fixed4 _RimColor_Direct;
            fixed4 _RimColor_Reverse;
			
	        fixed _Rim_Direct_Pow;
	        fixed _Rim_Reverse_Pow;
			
			fixed _Far;
			fixed4 _FogColor;

			fixed4 frag (v2f i) : SV_Target
			{
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
 
			/*   主體色   */

                fixed4 col = tex2D (_MainTex, i.uv);

                fixed3 Light_Part = _LightColor0.rgb * col * _Color.rgb;//亮面
                fixed3 Shadow_Part = _LightColor0.rgb * col *_ShadowColor;//暗面

				fixed Shading =  smoothstep( 0 , _LightGradient, dot(worldNormal,worldLightDir) + _LightGradient/5    ) ;
				
				col = fixed4( (lerp(Shadow_Part,Light_Part, Shading))  , 1);



			/*   正反邊緣光處理   */

				fixed LightDirDOTNormal = dot(worldLightDir,worldNormal);
				fixed ViewDirDOTNormal = dot(worldViewDir,worldNormal);

				fixed Rim_Direct = pow( saturate( 0.5 + LightDirDOTNormal) * ( 1.15-ViewDirDOTNormal )*_RimLight_Direct ,_Rim_Direct_Pow); //正邊緣光
				fixed Rim_Reverse = pow( saturate( 0.5 - LightDirDOTNormal) * ( 1-ViewDirDOTNormal ) *_RimLight_Reverse ,_Rim_Reverse_Pow); //反邊緣光

				col = lerp(col,_RimColor_Direct,Rim_Direct*_RimColor_Direct.a);
				col = lerp(col,_RimColor_Reverse,Rim_Reverse*_RimColor_Reverse.a);
				

			/*   深度霧   */

				fixed partZ = saturate(smoothstep(_Far/10,_Far, i.screenPos.z));
				col = lerp( col , float4(0,0,0,1) , (smoothstep(0,0.75,partZ))  );
				col = lerp( col , _FogColor , partZ  );



				return col;
			}
			ENDCG
		}
	}
}
