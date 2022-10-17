Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowTex ("GlowTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _UVSpeed ("Speed", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1000" }

        ZWrite Off
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _GlowTex;
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }
            float4 _Color;
            float _UVSpeed;
            
            float4 frag (v2f i) : SV_Target
            {
                
                UNITY_SETUP_INSTANCE_ID(i);

                float4 col = tex2D(_MainTex, i.uv+_Time.y*_UVSpeed*float2(0,-1));
                
                float glow = tex2D(_GlowTex, i.uv+_Time.y*_UVSpeed*float2(0,-1)).r;

                col = lerp(_Color.a*_Color*float4(1,1,1,glow)+col,col,col.a);

                return col;
            }
            ENDCG
        }
    }
}
