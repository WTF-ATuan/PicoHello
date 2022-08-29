Shader "GGDog/Boss"
{
    Properties
    {
        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        [HDR]_BackRimColor("BackRim Color",Color) = (1,0.5,0.5,1)
        [HDR]_DirRimColor("DirRim Color",Color) = (1,0.5,0.5,1)
        _Gloss("Gloss",Range(1,200)) = 10
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
			float4 _Color;
			float4 _ShadowColor;
			float4 _BackRimColor;
			float4 _DirRimColor;
			
            half _Gloss;

            float4 frag (v2f i) : SV_Target
            {
                float n =  smoothstep(0.5,1,distance(frac(20*i.uv+_Time.y*half2(0.7,1)*0.25),0.5));
                float n2 =  smoothstep(0.3,1,distance(frac(10*i.uv+_Time.y*half2(0.9,0.75)*0.75),0.5));
                n+=n2;
                n = saturate(n+0.25);


                float3 WorldNormal = normalize(i.worldNormal);
                float3 LightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

				float NdotL = (dot(WorldNormal,LightDir));
				float Rim = 1-saturate(smoothstep(0,1,dot(WorldNormal,ViewDir)));

                float4 BackRimColor =  ( smoothstep(0,1,Rim*smoothstep(0.99,1.02,saturate(1-NdotL-0.0015)))  * _BackRimColor )*n;
                float4 DirRimColor =  (smoothstep(-0.5,1,Rim*smoothstep(0,0.25,saturate(NdotL+0.0015)))  * _DirRimColor )*n;


				float4 FinalColor = lerp(_ShadowColor + BackRimColor ,_Color + DirRimColor  ,NdotL );

                return FinalColor;
            }
            ENDCG
        }
    }
}
