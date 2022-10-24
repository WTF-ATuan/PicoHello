Shader "GGDog/FinalBoss_ScrollUV"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _SpeedX("Speed X",float) = -0.25
        _SpeedY("Speed Y",float) = 0

        _WaveColor ("Wave Color", Color) = (1,0,0,1)
        _WaveTiling ("Wave Tiling", Float) = 10
        _WaveSpeed ("Wave Speed", Float) = 2.5
        _WaveSmoothstepMin ("Wave Smoothstep Min", Range(0,1)) = 0
        _WaveSmoothstepMax ("Wave Smoothstep Max", Range(0,1)) = 1
        
        _DistortionTiling ("Distortion Tiling", Float) = 20
        _DistortionSpeed ("Distortion Speed", Float) = 2.5
        _DistortionIntense ("Distortion Intense", Float) = 0.05

        _AlphaClip ("AlphaClip", Range(0,1)) = 0.03
        
        _NoiseTiling ("Noise Density", Float) = 0.06
        _NoiseStrength ("Noise Strength", Range(2,2.5)) = 2
    }
    SubShader
    {
		Tags
		{
			"Queue"="Transparent"
		}

        LOD 1
		
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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
            };
            
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


            sampler2D _MainTex;
            half4 _MainTex_ST;
            
            half _NoiseTiling;
            half _NoiseStrength;
            v2f vert (appdata v)
            {
                v2f o;
                
				half3 worldPos = v.vertex*_NoiseTiling;

                half Noise =WaterTex(worldPos.xy,50,0.5) + WaterTex(worldPos.xy,30,-1); 

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*(Noise*2-_NoiseStrength-1)*_NoiseStrength*0.1);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }
            
            half4 _Color;
            
            half _SpeedX;
            half _SpeedY;

            half4 _WaveColor;

            half _WaveTiling;
            half _WaveSpeed;
            half _WaveSmoothstepMin;
            half _WaveSmoothstepMax;

            half _DistortionTiling;
            half _DistortionSpeed;
            half _DistortionIntense;
            
            half _AlphaClip;
            
            half4 frag (v2f i) : SV_Target
            {
                half Noise = WaterTex(i.uv,_DistortionTiling,_DistortionSpeed)*_DistortionIntense;

                half4 col = tex2D(_MainTex, i.uv + half2(_SpeedX,_SpeedY)*_Time.y + Noise)*_Color;
                
                half T =smoothstep(_WaveSmoothstepMin,_WaveSmoothstepMax,WaterTex(i.uv + -Noise*0.5,_WaveTiling,_WaveSpeed));

                col.rgb+=T*_WaveColor*_WaveColor.a;
                
                clip(col.a-_AlphaClip);

                return col*i.color ;
            }
            ENDCG
        }
    }
}
