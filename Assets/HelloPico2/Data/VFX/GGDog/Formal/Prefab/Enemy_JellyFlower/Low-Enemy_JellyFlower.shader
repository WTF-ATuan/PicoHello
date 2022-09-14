Shader "Unlit/Low-Enemy_JellyFlower"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        [HDR]_TexColor ("Tex Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off

        Blend One One

        Cull Off

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
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TexColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv+_Time.y*_MainTex_ST.zw);

                col *= col.a*_TexColor + col.a*_Color*_Color.a;

                
                half3 worldNormal = normalize(i.worldNormal);
                half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                float Rim = 1-saturate(smoothstep(-0.25,0.25,dot(worldNormal,worldViewDir) ));


                return col*Rim;
            }
            ENDCG
        }
    }
}
