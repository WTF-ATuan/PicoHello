Shader "GGDog/Evil_partical"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

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
            
            float2 Rotate_UV(float2 uv , float sin , float cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            float WaterTex(float2 uv,float Tilling,float FlowSpeed)
            {
                uv.xy*=Tilling;
                float Time = _Time.y*FlowSpeed;



                uv.xy = Rotate_UV(uv,0.34,0.14);
                float2 UV = frac(uv.xy*0.75+Time* float2(-1,-0.25));
                float UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				float D = smoothstep(-10.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* float2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				float D2 = smoothstep(-18.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* float2(0.54,-0.13));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				float D3 = smoothstep(-15.4,4.2,1-38.7*UV_Center-1);

                D = 1-max(max(D,D2),D3);
                
                return D;
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ
                float2 uv = i.uv-0.5;
				float D = smoothstep(-25.8,33.6,1.0-80.1*( uv.x*uv.x + uv.y*uv.y )-1.0);
                D =D*D;
                D = smoothstep(0.0,0.25,saturate(D))*2;
                
                i.uv += 0.1*(1.0-WaterTex(i.uv-i.color.r*float2(1.0,0.0),5-i.color.r,2.5));
                
                float a = WaterTex(i.uv+i.color.r*float2(1.0,0.0),5-i.color.r,2.5*(1.0-2*i.color.r))*i.color.a *D;

                float4 col = float4( 0.025,0.0,0.05,a*0.5);


                return col;
            }
            ENDCG
        }
    }
}
