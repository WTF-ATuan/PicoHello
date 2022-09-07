Shader "GGDog/M_Partical_Rim"
{
    Properties
    {
        _noise("擾動幅度", Range(0,100)) = 15

        [IntRange]_Step ("密度/大小", Range(2,500)) = 150
        _Tilling ("密度/大小", Vector) = (1,1,0,0)
        _num ("數量", Range(0,1)) = 0.5
        _overallSpeed ("整體流動速度", Range(-1,1)) = 0.5
        _waveSpeed ("擾動速度", Range(0,1)) = 0.5
        _waveAmp ("擾動幅度", Range(0,1)) = 0.5
        _twinklingSpeed ("閃爍率", Range(0.1,10)) = 1
        _twinklingAmp ("隨機數量減少", Range(0.1,10)) = 1

        _RimPow("RimPow",Range(0,1.5)) = 1
        _RimPart("RimPart",Range(0,1)) = 0.15
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        LOD 1

        ZWrite Off

       // Cull Off

        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };
            float _noise;
            v2f vert (appdata v)
            {
                v2f o;
                
                //CameraDistance
				o.uv.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

                float n =  smoothstep(0.01,1,distance(frac(float2(5,5)*v.uv+_Time.y*float2(1,0)*0.5),0.5));
                float n2 =  smoothstep(0.05,1,distance(frac(float2(7,3)*v.uv+_Time.y*float2(-1,0)*0.25),0.5));
                n+=n2*v.color.a;

                o.vertex = UnityObjectToClipPos(v.vertex + _noise*n*v.normal*(1.75-v.color.a));
                o.uv = v.uv;
                o.color = v.color;

                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            
            //隨機-胡椒鹽雜訊圖
            float random (float2 uv) { return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123); }

            float _Step, _num,_overallSpeed ,_waveSpeed ,_waveAmp ,_twinklingSpeed ,_twinklingAmp;
            
            float2 _Tilling;
            float VFX_MillionPartical(float CameraDistance ,float2 uv ,float2 speedDir ,float Step,float num,float overallSpeed ,float waveSpeed ,float waveAmp ,float twinklingSpeed ,float twinklingAmp)
            {
                //整體移動
                uv = uv+_Time.y*overallSpeed*speedDir;
                
                //色調分層
                float ky = floor( uv.y*(Step*_Tilling.y))/(Step*_Tilling.y);

                //橫軸隨機擾動
                uv.x = uv.x + abs(fmod( random(ky)*_Time.y*waveSpeed /2,1)*2-1) *waveAmp/2;

                //色調分層
                float kx = floor( uv.x*(Step*_Tilling.x))/(Step*_Tilling.x);
                
                //隨機
                float r =random( float2(kx,ky) );

                //數量
                float r01 = smoothstep(1-num,1,r);

                uv.xy = frac(uv.xy*Step*_Tilling.xy);

                //隨機大小閃爍
                r *= abs(fmod(random(kx*ky)* _Time.y*twinklingSpeed/2,1)*2-1)*twinklingAmp;
                
				//中心距離場
				float D =1- distance(float2(uv.x,uv.y),float2(0.5,0.5))-r*r/(3+CameraDistance);
				//漸層度
				float D2 = smoothstep(0.8,0.85,D)*1.5;
				D = D2+smoothstep(0.5,1.25,D);

                return r01*D;
            }

            float _RimPow;
            float _RimPart;
            float4 frag (v2f i) : SV_Target
            {/*
                //整體移動
                i.uv.y = i.uv.y+_Time.y*_overallSpeed;
                
                //色調分層
                float ky = floor( i.uv.y*_Step)/_Step;

                //橫軸隨機擾動
                i.uv.x = i.uv.x + abs(fmod( random(ky)*_Time.y*_waveSpeed /2,1)*2-1) *_waveAmp/2;

                //色調分層
                float kx = floor( i.uv.x*_Step)/_Step;
                
                //隨機
                float r =random( float2(kx,ky) );

                //數量
                float r01 = step(1-_num,r);

                i.uv.xy = frac(i.uv.xy*_Step);

                //隨機大小閃爍
                r *= abs(fmod(random(kx*ky)* _Time.y*_twinklingSpeed/2,1)*2-1)*_twinklingAmp;
                
				//中心距離場
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5))-r*r/(3+i.uv.z);
				//漸層度
				float D2 = smoothstep(0.8,0.85,D)*2;
				D = D2+smoothstep(0.5,1.5,D);

                return r01*D;
                */


                fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                fixed3 worldNormal = normalize(i.worldNormal);

                float Rim = saturate(smoothstep(0,_RimPow,dot(worldNormal,worldViewDir) + (1 - _RimPart)));


                i.uv.z = smoothstep(0,500,i.uv.z/2);

                _num = Rim*5*smoothstep(0,0.35,i.uv.y)*smoothstep(0,0.5,1-i.uv.y)*i.color.a*i.color.a*_num/(3+i.uv.z);

                float v1 = VFX_MillionPartical(i.uv.z,i.uv.xy ,float2(0,1) ,_Step,_num,_overallSpeed ,_waveSpeed ,_waveAmp ,_twinklingSpeed ,_twinklingAmp);
                
                float v2 = VFX_MillionPartical(i.uv.z,i.uv.xy ,float2(0.15,0.75) ,_Step*1.5,_num,_overallSpeed/2 ,_waveSpeed/2 ,_waveAmp/2 ,_twinklingSpeed/2 ,_twinklingAmp/2);
                
                float v3 = VFX_MillionPartical(i.uv.z,i.uv.xy,float2(-0.2,0.35) ,_Step*1.25,_num,_overallSpeed /3,_waveSpeed /3.5,_waveAmp/3.5 ,_twinklingSpeed/3.5 ,_twinklingAmp/3.5);
                
                float Final = v1+v2+v3;

                clip(Final-0.01);

             return Final*i.color;
            }

            ENDCG
        }
    }
}
