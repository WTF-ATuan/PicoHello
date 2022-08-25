Shader "Unlit/Black_Fade"
{
    Properties
    {
        _Alpha("Alpha",Range(0,1)) = 1
        _r("r",Range(0,10000)) = 1
        _p("p",Range(0,10000)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent+9999" }

       // Offset [_r],[_p]

        ZTest Always

		Blend SrcAlpha OneMinusSrcAlpha

        Cull Front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half _Alpha;
            half4 frag (v2f i) : SV_Target
            {
                return half4(0,0,0,_Alpha);
            }
            ENDCG
        }
    }
}
