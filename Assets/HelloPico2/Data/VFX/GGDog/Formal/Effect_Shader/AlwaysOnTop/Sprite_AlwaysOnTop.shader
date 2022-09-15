// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/Sprite_AlwaysOnTop"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1

		ZWrite Off
		ZTest Always
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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;

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
            half4 _Color;
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
                half4 col = tex2D(_MainTex, i.uv);
                
                col = half4(1, 1, 1, col.a) * i.color*_Color;

                clip(col.a - 0.0015);
                
                return col;
            }
            ENDCG
        }
    }
}
