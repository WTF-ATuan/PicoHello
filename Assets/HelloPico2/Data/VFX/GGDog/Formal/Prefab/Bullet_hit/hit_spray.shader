// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 1
        _Size ("Size", Range(0,10)) = 1

        _HDR ("HDR", Range(1,10)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off

        Cull Off

        Blend SrcAlpha One

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
            
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* float2(-1,0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* float2(-0.24,-0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* float2(0.54,0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                D = 1-max(max(D,D2),D3);
                
                return D;
            }


            v2f vert (appdata v)
            {
                v2f o;
                half n = 1-WaterTex(v.uv,7,3);
                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*n*0.01*_Size);

                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            half _Alpha;
            half _HDR;
            
            half4 frag (v2f i) : SV_Target
            {
                
                i.color.a = smoothstep(0,0.75,i.color.a);


                half n = WaterTex(i.uv,6,1.5);
                half k = WaterTex(i.uv+0.01,6,1.5);
                //half k = n;

                n = smoothstep(0.5,0.5,n-saturate(0.5-i.color.a));


                half4 col = half4(1,1,1,n);

                col.a *= (1-smoothstep(0.35,0.75,k-saturate(0.5-i.color.a))) *i.color.a;

                col.rgb *=i.color.rgb*_HDR;

                return col;
            }
            ENDCG
        }
    }
}
