Shader "Unlit/Back_Fade"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 1
        _Alphadecrease ("Alpha decrease", Range(0,1)) = 1
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
    }
    SubShader
    {
        Tags { "Queue"="Transparent+2000" }

        Cull Front
        ZWrite Off
        ZTest[_ZTest]

        Blend SrcAlpha OneMinusSrcAlpha

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
            half _Alphadecrease;
            
            half4 frag (v2f i) : SV_Target
            {
                return float4(0,0,0,_Alpha*_Alphadecrease);
            }
            ENDCG
        }
    }
}
