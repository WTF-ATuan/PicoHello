Shader "GGDog/Space_Test/Env"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_FarColor("Far Color",Color) = (0.5,0.5,0.5,1)
		_BackFogColor("Back Fog Color",Color) = (0,0,0,1)
		[HDR]_FogColor("Fog Color",Color) = (1,1,1,1)
        _FogPos ("Fog Pos", Range(-200,200)) = 77
		
		_Rim("Rim",Range(0.01,10)) = 5
    }
    SubShader
    {

        Pass
        {
            Tags { "LightMode"="ForwardBase" }
 
            Cull Back
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 CameraDistance : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                return o;
            }
            float4 _Color;
            float4 _ShadowColor;

            float4 _SkyColor;
			
            float4 _FarColor;
            float4 _BackFogColor;
            float4 _FogColor;

            float _Rim;
			
            float _FogPos;

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 normalDir = normalize(i.worldNormal);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );
				
                float3 ShadowColor = _ShadowColor;
                float3 LightColor  = _LightColor0* _Color ;

                float3 FinalColor =  lerp( ShadowColor, LightColor , (max(dot(normalDir,lightDir),0)+0.25) *smoothstep(-300,500,i.worldPos.z)   );


				//Ãä½t¥ú
				float Rim = 1-saturate(smoothstep(0,0.75,dot(normalDir,viewDir)));
				Rim =  ( Rim + smoothstep(0.5,1,Rim) )/_Rim;

				Rim*=smoothstep(300,700,i.CameraDistance);
				

                float4 col = tex2D(_MainTex, i.uv)* float4(FinalColor,1);

				col += Rim*_SkyColor;

				col = lerp(col,_FarColor,saturate(smoothstep(0,1000,i.CameraDistance)));
				

				col = lerp(col,_SkyColor,saturate(smoothstep(850,1000,i.worldPos.z)));
				

				col = lerp(col,_FogColor,(1-saturate(smoothstep(-100,300,i.worldPos.y+_FogPos))) *smoothstep(0,1000,i.CameraDistance)  );
				

				col = lerp(col,_BackFogColor,1-saturate(smoothstep(-700,1000,i.worldPos.z)));
				

                return col ;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
