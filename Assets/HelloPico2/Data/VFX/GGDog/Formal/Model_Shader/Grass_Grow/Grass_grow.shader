Shader "GGDog/Grass_grow"
{
    Properties
    {
		_ColorLerp("ColorLerp",Range(0,1))=0
        _MainTex ("Texture", 2D) = "white" {}
		_AlphaClip("AlphaClip",Range(0,1))=0
		[HDR]_Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float _AlphaClip;
            float4 _Color;
            float _ColorLerp;
            
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                fixed4 col = tex2D(_MainTex, i.uv);

                clip(col.a-_AlphaClip);

                col = lerp(col*1.1,col*_Color*1.1,_ColorLerp);

                return col*i.uv.y;
            }
            ENDCG
        }
    }
}
