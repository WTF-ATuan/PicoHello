Shader "Unlit/mural"
{
    Properties
    {
        _OffSet ("OffSet", Range(-1,1)) = 0
        _Gradient ("Gradient", Range(1,2)) =1.5
		[KeywordEnum(X, X_inverse,Y,Y_inverse,Dot,Dot_inverse,MaskTex)] _MASKSHAPE("Mask Shape", Float) = 0
        [HDR]_DissolveColor ("DissolveColor", COLOR) = (0.5,0.5,0.5,1)

        _MainTex ("Texture", 2D) = "white" {}
        _GlowTex ("Glow Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Color ("Color", COLOR) = (0.5,0.5,0.5,1)
        [HDR]_GlowColor ("GlowColor", COLOR) = (0.5,0.5,0.5,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#pragma shader_feature _MASKSHAPE_X _MASKSHAPE_X_INVERSE _MASKSHAPE_Y _MASKSHAPE_Y_INVERSE  _MASKSHAPE_DOT _MASKSHAPE_DOT_INVERSE _MASKSHAPE_MASKTEX

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
            
            sampler2D _MaskTex;
            sampler2D _GlowTex;
            
            half4 _Color;
            half _OffSet;
            half4 _DissolveColor;
            half4 _GlowColor;

            half _Gradient;
            
            //消融形式
            v2f MaskShape(v2f i)
            {
                #if _MASKSHAPE_X
                            i.uv.x = i.uv.x;
                #elif _MASKSHAPE_X_INVERSE
                            i.uv.x = 1-i.uv.x;
                #elif _MASKSHAPE_Y
                            i.uv.x = i.uv.y;
                #elif _MASKSHAPE_Y_INVERSE
                            i.uv.x = 1-i.uv.y;
                #elif _MASKSHAPE_DOT
                            i.uv.x = smoothstep(-20.4,7.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
                #elif _MASKSHAPE_DOT_INVERSE
                            i.uv.x = 1-smoothstep(-20.4,7.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
                #elif _MASKSHAPE_MASKTEX
                            i.uv.x = tex2D(_MaskTex, i.uv).r ;
                #endif

                return i;
            }

            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(v2f i,half Tilling,half FlowSpeed)
            {
                i.uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                i.uv.xy = Rotate_UV(i.uv,0.34,0.14);
                half2 UV = frac(i.uv.xy*0.75+Time* float2(-1,0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                i.uv.xy = Rotate_UV(i.uv,0.94,0.44);
                UV = frac(i.uv.xy*1.2+Time*0.33* float2(-0.24,-0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                i.uv.xy = Rotate_UV(i.uv,0.64,0.74);
                UV = frac(i.uv.xy*1+Time*1.34* float2(0.54,0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                //D = 1-max(max(D,D2),D3);
                
                D = (1-D)+max(D2,1-D3);

                return D;
            }

            half4 frag (v2f i) : SV_Target
            {

                half col = tex2D(_MainTex, i.uv).r ;

                half col_glow = tex2D(_GlowTex, i.uv).r ;

                half D =WaterTex(i,20,-1.5)+0.75;
                
                half D2 =WaterTex(i,3,-1.5);

                i = MaskShape(i);

                half Forward =D2*saturate(D-0.35)*smoothstep(0,1,(i.uv.x+_OffSet));
                half DoubleFade = D2*D*D*D*smoothstep(_Gradient/2,_Gradient,4*(i.uv.x+_OffSet)*(1-i.uv.x-_OffSet));

                //點亮前，浮雕樣
                half4 Finalcol = lerp(_Color,1, -col_glow/10 + col * (Forward+0.25) );

                //漸層點亮過渡
                Finalcol = lerp(Finalcol,_GlowColor, col_glow * (Forward /3) );
                
                //點亮後持續著發光
                Finalcol += col_glow*_DissolveColor*saturate(_OffSet*2)*_DissolveColor.a* DoubleFade * (_OffSet*3);

                return Finalcol;
            }
            ENDCG
        }
    }
}
