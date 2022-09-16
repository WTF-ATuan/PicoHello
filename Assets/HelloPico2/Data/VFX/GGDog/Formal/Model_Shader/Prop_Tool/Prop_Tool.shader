Shader "Unlit/Prop_Tool"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RimColor("RimColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                return o;
            }
            
			float4 _RimColor;
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
                fixed4 col = tex2D(_MainTex, i.uv);
                
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

                fixed3 LightDir = normalize(_WorldSpaceLightPos0.xyz);

				float Rim = 1-saturate(smoothstep(0,2,dot(worldNormal,worldViewDir) ));


                return float4(col.rgb+Rim*_RimColor.rgb,col.r+Rim*_RimColor.r);
            }
            ENDCG
        }
    }
}
