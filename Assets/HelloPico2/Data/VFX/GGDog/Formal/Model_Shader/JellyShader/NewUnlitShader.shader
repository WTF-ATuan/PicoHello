Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionTiling ("Distortion Tiling", Float) = 20
        _DistortionSpeed ("Distortion Speed", Float) = 2.5
        _DistortionIntense ("Distortion Intense", Float) = 0.05
        _AlphaClip ("AlphaClip", Range(0,1)) = 0.001
        _NoiseTiling ("Noise Density", Float) = 0.06
        _NoiseStrength ("Noise Strength", Range(0,2.5)) = 2
    }
    SubShader
    {
		Tags{ "RenderType"="transparent" "Queue"="transparent"}

		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha

        Cull Off
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            half _NoiseTiling;
            half _NoiseStrength;
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;



                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,-0.25));
                half UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D = smoothstep(-10.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D2 = smoothstep(-18.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.13));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D3 = smoothstep(-15.4,4.2,1-38.7*UV_Center-1);

                D = 1-max(max(D,D2),D3);
                
                return D;
            }

            v2f vert (appdata v)
            {
                v2f o;

				half3 worldPos = v.vertex*_NoiseTiling;

                half Noise =WaterTex(worldPos.xy,50,2.5) + WaterTex(worldPos.xy,30,-1); 

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*(Noise*2-_NoiseStrength-1)*_NoiseStrength*0.1);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            half _DistortionTiling;
            half _DistortionSpeed;
            half _DistortionIntense;
            
            half _AlphaClip;
            
            fixed4 frag (v2f i) : SV_Target
            {
                
				float D = smoothstep(-16,9.6,1-49*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
				float D2 = smoothstep(-32.5,47,1-460.9*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                D =D*D+D2*D2*50.5;

                half Noise = 1-WaterTex(i.uv,_DistortionTiling,_DistortionSpeed)*_DistortionIntense -_DistortionIntense/2;

                fixed4 col = tex2D(_MainTex, i.uv+ Noise*D);
                clip(col.a-_AlphaClip);
                return col;
            }
            ENDCG
        }
    }
}
