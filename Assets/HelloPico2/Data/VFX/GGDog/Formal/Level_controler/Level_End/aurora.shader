Shader "Unlit/aurora"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Tilling ("Tilling", Vector) = (1,1,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" }
        LOD 1

        ZWrite Off

        Blend SrcAlpha One

        ZTest Always

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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _Tilling;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                
				half3 worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);


                half col = tex2Dlod (_MainTex, float4(v.uv.xy*0.5 + _Time.y*float2(0.02,0),0,0)).r;
                half col2 = tex2Dlod (_MainTex, float4(v.uv.xy*0.5 - _Time.y*float2(0.05,0),0,0)).r;

                o.vertex = UnityObjectToClipPos(v.vertex + 0.5*(col+col2)*v.normal*(0.75-dot(worldNormal,worldViewDir)));
                o.uv.xy = v.uv;

                
                
				 o.uv.z = saturate(smoothstep(0,0.0005,dot(worldNormal,worldViewDir)));

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                
                UNITY_SETUP_INSTANCE_ID (i);

                float Mask;

                Mask = smoothstep(0,1+0.35*saturate(1+sin(_Time.y)),i.uv.y)*smoothstep(0,0.5,1-i.uv.y)*smoothstep(0,0.25,1-i.uv.x)*smoothstep(0,0.25,i.uv.x);

                float4 col = tex2D(_MainTex,_Tilling.x* i.uv.xy + _Time.y*float2(0.175,0))*Mask;
                float4 col2 = tex2D(_MainTex,_Tilling.y* i.uv.xy - _Time.y*float2(0.35,0))*Mask;

                col = saturate(col+col2)*i.uv.z;

                return col*_Color;
            }
            ENDCG
        }
    }
}
