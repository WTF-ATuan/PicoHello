Shader "GGDog/CameraEffect_MR"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 1
        _smoothstepmin ("smoothstepmin", Range(0,1)) = 0.1
        _smoothstepmax ("smoothstepmax", Range(0,1)) = 0.5
        _NoiseSpeed ("Noise Speed", Float) = 0.5
        _NoiseTiling ("Noise Tiling", Float) = 17
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Front

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
                return o;
            }
            float _Alpha;
            float _smoothstepmin;
            float _smoothstepmax;
            float _NoiseSpeed;
            float _NoiseTiling;

            float4 frag (v2f i) : SV_Target
            {

                float D = WaterTex(i.uv+float2(0,0.5)+WaterTex(i.uv+float2(0,0.5),8,-1)*0.05,17,0.5);

                D =smoothstep(_smoothstepmin,_smoothstepmax,D);

                return float4(0,0,0,D*_Alpha + smoothstep(0.5,0.85,1-i.uv.y));
            }
            ENDCG
        }
    }
}
