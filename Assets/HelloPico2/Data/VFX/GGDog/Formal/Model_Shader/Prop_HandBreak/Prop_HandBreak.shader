Shader "GGDog/Prop_HandBreak"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color) = (1,1,1,1)
		[HDR]_RimColor("Rim Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }
            
            float4 _Color;
            float4 _RimColor;
            float4 frag (v2f i) : SV_Target
            {

                float4 col = tex2D(_MainTex, i.uv)*_Color;

		        float3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        float3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		        float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
		        float NdotV = dot(worldNormal , worldViewDir); 
                
		        float rim = saturate(pow(1-NdotV,1.5)) *2;

                return col + rim*_RimColor;
            }
            ENDCG
        }
    }
}
