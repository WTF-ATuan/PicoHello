Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 1
        _Size ("Size", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off

        Cull Off

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
                half4 color : COLOR;
                half4 normal : NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
            };
            half _Size;
            
            v2f vert (appdata v)
            {
                v2f o;
				half4 srcUV = ComputeScreenPos(v.vertex);  //抓取螢幕截圖的位置
                half n =  smoothstep(0.5,1,1-distance(frac(30*srcUV.xy/srcUV.w-_Time.y*half2(1,0.5)*0.25),0.5));
                half n2 =  smoothstep(0.5,1,1-distance(frac(20*srcUV.xy/srcUV.w+_Time.y*half2(0.7,1)*0.75),0.5));
                n+=n2;
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*n*0.01*_Size);

                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            half _Alpha;
            
            half4 frag (v2f i) : SV_Target
            {
                
                _Alpha*=i.color.a;

                half n =  smoothstep(0,0.75,distance(frac(2.3*i.uv+_Time.y*half2(0,0.5)*2.25),0.5));
                half n2 =  smoothstep(0,0.25,distance(frac(2.7*i.uv+half2(0.2,0.4)-_Time.y*half2(0.7,1)*2.75),0.5));
                
                half n3 =  smoothstep(0.3,1,1-distance(frac(2.5*i.uv+_Time.y*half2(0.27,0.3)*1.75),0.5))*(1-_Alpha*2);

                n= saturate(n * n2 + smoothstep(0.75,1,i.uv.y)) -n3 - (1-_Alpha*2);
                
                half k = n;

                n = smoothstep(0.5,0.5,n);


                half4 col = half4(1,1,1,n/1.5);

                col.a *= smoothstep(0.25,1.25,2*saturate(1.15-smoothstep(0,0.75,k)+_Alpha*_Alpha*_Alpha));

                col.rgb *=i.color.rgb*0.85;

                return col;
            }
            ENDCG
        }
    }
}
