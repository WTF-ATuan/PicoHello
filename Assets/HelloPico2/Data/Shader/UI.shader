Shader "GGDog/UI"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _AlphaClip("AlphaClip", Range(0,1)) = 0
    }
        SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        ZTest Always
        ZTest Off
        
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            
            float _AlphaClip;
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                clip(col.a-_AlphaClip);
                return col * i.color;
            }
            ENDCG
        }
    }
}
