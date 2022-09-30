Shader "Unlit/Sky_Glow_Fade"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Geometry-100" }

        Cull Front
        ZWrite Off
        Blend One One
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half _Alpha;
            half4 frag (v2f i) : SV_Target
            {
                return _Alpha;
            }
            ENDCG
        }
    }
}
