Shader "GGDog/FlowerHand_M_Partical"
{
    Properties
    {
        [IntRange]_Step ("密度/大小", Range(2,500)) = 150
        _Tilling ("密度/大小", Vector) = (1,1,0,0)
        _num ("數量", Range(0,1)) = 0.5
        _overallSpeed ("整體流動速度", Range(0,1)) = 0.5
        _waveSpeed ("擾動速度", Range(0,1)) = 0.5
        _waveAmp ("擾動幅度", Range(0,1)) = 0.5
        _twinklingSpeed ("閃爍率", Range(0.1,10)) = 1
        _twinklingAmp ("隨機數量減少", Range(0.1,10)) = 1
        
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        ZWrite Off

        Cull Off

        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half3 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half3 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                
                //CameraDistance
				o.uv.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

                half n =  smoothstep(0.01,1,distance(frac(half2(50,50)*v.uv+_Time.y*half2(1,1)*0.5),0.5));
                half n2 =  smoothstep(0.05,1,distance(frac(half2(70,30)*v.uv+_Time.y*half2(-1,1)*0.25),0.5));
                n+=n2;

                o.vertex = UnityObjectToClipPos(v.vertex + 0.05*n*v.normal);
                o.uv = v.uv;
                o.color = v.color;
                
                return o;
            }
            
            //隨機-胡椒鹽雜訊圖
            half random (half2 uv) { return frac(sin(dot(uv,half2(12.9898,78.233)))*43758.5453123); }

            half _Step, _num,_overallSpeed ,_waveSpeed ,_waveAmp ,_twinklingSpeed ,_twinklingAmp;
            
            half2 _Tilling;
            half VFX_MillionPartical(half CameraDistance ,half2 uv ,half2 speedDir ,half Step,half num,half overallSpeed ,half waveSpeed ,half waveAmp ,half twinklingSpeed ,half twinklingAmp)
            {
                //整體移動
                uv = uv+_Time.y*overallSpeed*speedDir;
                
                //色調分層
                half ky = floor( uv.y*(Step*_Tilling.y))/(Step*_Tilling.y);

                //橫軸隨機擾動
                uv.x = uv.x + abs(fmod( random(ky)*_Time.y*waveSpeed /2,1)*2-1) *waveAmp/2;

                //色調分層
                half kx = floor( uv.x*(Step*_Tilling.x))/(Step*_Tilling.x);
                
                //隨機
                half r =random( half2(kx,ky) );

                //數量
                half r01 = smoothstep(1-num,1,r);

                uv.xy = frac(uv.xy*Step*_Tilling.xy);

                //隨機大小閃爍
                r *= abs(fmod(random(kx*ky)* _Time.y*twinklingSpeed/2,1)*2-1)*twinklingAmp;
                
				//中心距離場
				half D =1- distance(half2(uv.x,uv.y),half2(0.5,0.5))-r*r/(3+CameraDistance);
				//漸層度
				half D2 = smoothstep(0.8,0.85,D)*1.5;
				D = D2+smoothstep(0.5,1.25,D);

                return r01*D;
            }

            half4 frag (v2f i) : SV_Target
            {
                /*
                //整體移動
                i.uv.y = i.uv.y+_Time.y*_overallSpeed;
                
                //色調分層
                half ky = floor( i.uv.y*_Step)/_Step;

                //橫軸隨機擾動
                i.uv.x = i.uv.x + abs(fmod( random(ky)*_Time.y*_waveSpeed /2,1)*2-1) *_waveAmp/2;

                //色調分層
                half kx = floor( i.uv.x*_Step)/_Step;
                
                //隨機
                half r =random( half2(kx,ky) );

                //數量
                half r01 = step(1-_num,r);

                i.uv.xy = frac(i.uv.xy*_Step);

                //隨機大小閃爍
                r *= abs(fmod(random(kx*ky)* _Time.y*_twinklingSpeed/2,1)*2-1)*_twinklingAmp;
                
				//中心距離場
				half D =1- distance(half2(i.uv.x,i.uv.y),half2(0.5,0.5))-r*r/(3+i.uv.z);
				//漸層度
				half D2 = smoothstep(0.8,0.85,D)*2;
				D = D2+smoothstep(0.5,1.5,D);

                return r01*D;
                */
                
                i.uv.z = smoothstep(0,500,i.uv.z/2);

                _num = 5*smoothstep(0,0.25,i.uv.y)*smoothstep(0,1.5,1-i.uv.y)*i.color.a*i.color.a*_num/(3+i.uv.z);

                half v1 = VFX_MillionPartical(i.uv.z,i.uv.xy ,half2(0,1) ,_Step,_num,_overallSpeed ,_waveSpeed ,_waveAmp ,_twinklingSpeed ,_twinklingAmp);
                
                half v2 = VFX_MillionPartical(i.uv.z,i.uv.xy ,half2(0.5,0.75) ,_Step*1.5,_num,_overallSpeed/2 ,_waveSpeed/2 ,_waveAmp/2 ,_twinklingSpeed/2 ,_twinklingAmp/2);
                
                half v3 = VFX_MillionPartical(i.uv.z,i.uv.xy,half2(-0.2,0.35) ,_Step*1.25,_num,_overallSpeed /3,_waveSpeed /3.5,_waveAmp/3.5 ,_twinklingSpeed/3.5 ,_twinklingAmp/3.5);
                
                half Final = v1+v2+v3;

                clip(Final-0.01);

             return Final*i.color;
            }

            ENDCG
        }
    }
}
