Shader "GGDog/Effect/SpiritLink"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _NoiseTilingX("NoiseTiling_X",float) = 5
        _NoiseTilingY("NoiseTiling_Y",float) = 2

        _YFSmin("Y_FadeSmoothstep_min",Range(0,1)) = 0.25
        _YFSmax("Y_FadeSmoothstep_max",Range(0,1)) = 1

        _XFSmin("X_FadeSmoothstep_min",Range(0,1)) = 0
        _XFSmax("X_FadeSmoothstep_max",Range(0,1)) = 0.6
    }
    SubShader
    {

        Blend SrcAlpha One
        ZWrite Off

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

            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half2 Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling.xy;
                half Time = _Time.y*FlowSpeed;



                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,0));
                half UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D = smoothstep(-10.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-0.75,0.1));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D2 = smoothstep(-18.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(1,-0.1));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D3 = smoothstep(-15.4,4.2,1-38.7*UV_Center-1);

                D = 1-max(max(D,D2),D3);
                
                return D;
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            half _NoiseTilingX;
            half _NoiseTilingY;
            half _YFSmin;
            half _YFSmax;
            half4 _Color;
            
            half _XFSmin;
            half _XFSmax;
            half4 frag (v2f i) : SV_Target
            {

                half w = WaterTex(i.uv +_Time.y*half2(-0.5,0),half2(_NoiseTilingX,_NoiseTilingY),1) + WaterTex(i.uv,half2(_NoiseTilingX,_NoiseTilingY),-1);
                
                half w2 = WaterTex(i.uv +_Time.y*half2(0.5,0),half2(_NoiseTilingX,_NoiseTilingY),1) + WaterTex(i.uv,half2(_NoiseTilingX,_NoiseTilingY),-1);

                w = lerp(w,w2,smoothstep(0,0.15,saturate(i.uv.x-0.4)));

                w*= smoothstep(_YFSmin,_YFSmax ,1-abs(i.uv.y-(1-i.uv.y)));
                w*= smoothstep(_XFSmin,_XFSmax ,1-abs(i.uv.x-(1-i.uv.x)));

                return _Color * w/2;
            }
            ENDCG
        }
    }
}
