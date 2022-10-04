Shader "Unlit/Level_End_Sky"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 1
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        Cull Front
        
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
            
            half4 _Color1;
            half4 _Color2;
            half _Alpha;
            
            half4 frag (v2f i) : SV_Target
            {
                return float4(lerp(_Color2.rgb,_Color1.rgb,i.uv.y),_Alpha);
            }
            ENDCG
        }
    }
}
