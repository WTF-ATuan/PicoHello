Shader "GGDog/CameraEffect_MR"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Front
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float2 Rotate_UV(float2 uv , float sin , float cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.xy*=4;
                i.uv.xy = Rotate_UV(i.uv,0.34,0.14);
                float2 UV = frac(i.uv.xy*0.75+_Time.y * float2(-1,0.25));
				float D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                i.uv.xy = Rotate_UV(i.uv,0.94,0.44);
                UV = frac(i.uv.xy*1.2+_Time.y*0.33* float2(-0.24,-0.33));
				float D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                i.uv.xy = Rotate_UV(i.uv,0.64,0.74);
                UV = frac(i.uv.xy*1+_Time.y*1.34* float2(0.14,-0.73));
				float D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                D = 1-max(max(D,D2),D3);

                return D;
            }
            ENDCG
        }
    }
}
