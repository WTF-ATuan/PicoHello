Shader "GGDog/UI"
{
    Properties
    {
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
        _MainTex ("Texture", 2D) = "white" {}
        _HDR ("HDR", Range(1,10)) = 1
        _AlphaClip("AlphaClip", Range(0,1)) = 0.5

    }
    SubShader
    {
        Tags { "Queue"="Geometry" }

        ZTest [_ZTest]
        Blend SrcAlpha OneMinusSrcAlpha
        
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
            
            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            float _HDR;
            float _AlphaClip;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a-_AlphaClip);

                return i.color*_HDR;
            }
            ENDCG
        }
    }
}
