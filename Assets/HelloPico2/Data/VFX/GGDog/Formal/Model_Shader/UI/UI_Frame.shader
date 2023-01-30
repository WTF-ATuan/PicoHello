Shader "Unlit/UI_Frame"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _width("width",Range(0,1)) = 0.97
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            float _width;
            fixed4 frag (v2f i) : SV_Target
            {

                float alpha = 0.05*(1-i.color.a);

                i.uv.x+=0.5;

                i.uv = frac(i.uv);

                float f = step(_width+alpha,i.uv.x+i.uv.y)*step(_width+alpha,2-(i.uv.x+i.uv.y));
                
                f += step(_width+alpha,(1-i.uv.x)+i.uv.y)*step(_width+alpha,2-((1-i.uv.x)+i.uv.y));

                return float4(i.color.rgb,saturate(f)*i.color.a);
            }
            ENDCG
        }
    }
}
