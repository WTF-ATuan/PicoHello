// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Additive_OneOne_CullOn"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HDR("HDR",Range(1,7)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		
		Zwrite Off
		Blend One One
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
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _HDR;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                col*= i.color* i.color.a * _HDR;
                
               // col.rgb*=col.a;

                clip(col.a - 0.0015);

                return col;
            }
            ENDCG
        }
    }
}
