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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float4 _Color1;
            float4 _Color2;
            float _Alpha;
            
            float4 frag (v2f i) : SV_Target
            {
                return float4(lerp(_Color2.rgb,_Color1.rgb,i.uv.y),_Alpha);
            }
            ENDCG
        }
    }
}
