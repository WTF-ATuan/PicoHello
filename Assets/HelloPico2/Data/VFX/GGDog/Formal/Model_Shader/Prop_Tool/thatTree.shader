Shader "GGDog/ModelShader/thatTree"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FlakeTex ("Flake Texture", 2D) = "white" {}
		[HDR]_BaseColor("BaseColor",Color) = (1,1,1,1)
		_RimColor("RimColor",Color) = (1,1,1,1)
		_WorldPos("WorldPos",Vector) = (1,1,1,1)
		_Flake("Flake Range",Range(-0.5,2)) = 0
		_FlakeSmooth("Flake Smooth",Range(-0.5,1)) = 0
		_FlakeColor("FlakeColor",Color) = (1,1,1,1)
		_FlakeRimColor("Flake RimColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                return o;
            }
            
            
			float4 _BaseColor;
			float4 _RimColor;
			float3 _WorldPos;
			float _Flake;
			float _FlakeSmooth;
			float4 _FlakeColor;
			float4 _FlakeRimColor;

            sampler2D _FlakeTex;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv)*_BaseColor;
                
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

                fixed3 LightDir = normalize(_WorldSpaceLightPos0.xyz);

				float Rim = 1-saturate(smoothstep(0,1.5,dot(worldNormal,worldViewDir) ));

                
                fixed Flake = tex2D(_FlakeTex, i.uv - _MainTex_ST.zw).r;
                
                float d = (i.worldPos.x-_WorldPos.x)*(i.worldPos.x-_WorldPos.x)
                +(i.worldPos.y-_WorldPos.y)*(i.worldPos.y-_WorldPos.y)
                +(i.worldPos.z-_WorldPos.z)*(i.worldPos.z-_WorldPos.z);
                
                d*=Flake;

                d = smoothstep((_Flake+0.25)*2-1 - _FlakeSmooth,(_Flake+0.25)*2 +_FlakeSmooth,d /700);
                

                float3 colrgb = lerp(_FlakeColor.rgb*col.rgb+Rim*_FlakeRimColor*_FlakeRimColor.a,col.rgb+Rim*_RimColor.rgb,d);

                float a = smoothstep(-30,0,i.worldPos.y);

                return float4(colrgb, (col.a+Rim*_RimColor.r )* a ) ;
            }
            ENDCG
        }
    }
}
