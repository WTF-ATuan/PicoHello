Shader "Unlit/Cloud"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float _Alpha;
            
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv + float2(0,-0.01)*_Time.y);

                return fixed4(col.rgb,col.a*_Alpha);
            }
            ENDCG
        }
    }
}
