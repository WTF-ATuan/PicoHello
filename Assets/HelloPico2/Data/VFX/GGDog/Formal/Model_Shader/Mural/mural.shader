Shader "Unlit/mural"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowTex ("Glow Texture", 2D) = "white" {}
        _Color ("Color", COLOR) = (0.5,0.5,0.5,1)
        
        [HDR]_DissolveColor ("DissolveColor", COLOR) = (0.5,0.5,0.5,1)
        [HDR]_GlowColor ("GlowColor", COLOR) = (0.5,0.5,0.5,1)
        _OffSet ("OffSet", Range(-1,1)) = 0
        _Gradient ("Gradient", Range(1,2)) =1.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            sampler2D _MainTex;
            half4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            sampler2D _GlowTex;
            
            half4 _Color;
            half _OffSet;
            half4 _DissolveColor;
            half4 _GlowColor;

            half _Gradient;
            
            half4 frag (v2f i) : SV_Target
            {

                half col = tex2D(_MainTex, i.uv).r ;

                half col_glow = tex2D(_GlowTex, i.uv).r ;

                half Forward = smoothstep(0,1,(i.uv.x+_OffSet));
                half DoubleFade = smoothstep(_Gradient/2,_Gradient,4*(i.uv.x+_OffSet)*(1-i.uv.x-_OffSet));

                //點亮前，浮雕樣
                half4 Finalcol = lerp(_Color,1, -col_glow/10 + col * (Forward+0.25) );

                //漸層點亮過渡
                Finalcol = lerp(Finalcol,_GlowColor, col_glow * (Forward /3) );
                
                //點亮後持續著發光
                Finalcol += col_glow*_DissolveColor* DoubleFade;

                return Finalcol;
            }
            ENDCG
        }
    }
}
