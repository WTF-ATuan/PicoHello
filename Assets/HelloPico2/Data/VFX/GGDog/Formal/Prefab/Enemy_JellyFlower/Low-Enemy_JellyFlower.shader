// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Unlit/Low-Enemy_JellyFlower"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
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
            
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half4 scrPos : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TexColor;
            sampler2D _MainTex2;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                
                half3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

                o.uv.z = dot(worldNormal,worldViewDir);

				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
                
				float2 scruv = i.scrPos.xy/i.scrPos.w;

                float col = tex2D(_MainTex, 8*scruv+2*_Time.y*float2(-0.025,0.2)).b;

                float col2 = tex2D(_MainTex,6*scruv+2*_Time.y*float2(0.015,0.3)).b;

                col = min(col,col2);

                float4 FinalColor = col*_Color;

                float Rim = 1-saturate(smoothstep(-5,0,i.uv.z ));
                float Rim2 = saturate(smoothstep(-1,0,i.uv.z ));
                
                float col3 = tex2D(_MainTex2, i.uv.xy*2+_Time.y*float2(0.75,0.15)).b;

                return FinalColor*Rim*2 +Rim2*col3*_TexColor/2;
            }
            ENDCG
        }
    }
}
