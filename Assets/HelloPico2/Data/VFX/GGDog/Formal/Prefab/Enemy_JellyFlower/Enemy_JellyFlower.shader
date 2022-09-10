Shader "GGDog/Enemy_JellyFlower"
{
    Properties
    {
        _RenderTex("Render Tex", 2D) = "white" {}
        [HDR]_Color("_Color", Color) = (1,1,1,1)
        [HDR]_GlowColor("_GlowColor", Color) = (1,1,1,1)
        _noise("_noise", Range(0,0.01)) = 0
        _noiseTilling("_noiseTilling", Float) = 1

        [IntRange]_Step ("_Step", Range(2,500)) = 150
        _Tilling ("_Tilling", Vector) = (1,1,0,0)
        _num ("_num", Range(0,1)) = 0.5
        _overallSpeed ("_overallSpeed", Range(0,1)) = 0.5
        _waveSpeed ("_waveSpeed", Range(0,1)) = 0.5
        _waveAmp ("_waveAmp", Range(0,1)) = 0.5
        _twinklingSpeed ("_twinklingSpeed", Range(0.1,10)) = 1
        _twinklingAmp ("_twinklingAmp", Range(0.1,10)) = 1
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        LOD 1

        ZWrite On

        Pass
        {
            Cull Back

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
				half4 scrPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
            };
            float _noise;
            float4 _Color;

            float _noiseTilling;
            
            v2f vert (appdata v)
            {
                v2f o;
                
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                float n =  smoothstep(0.01,1,distance(frac(_noiseTilling*float2(5,5)*v.uv+_Time.y*float2(1,0)*0.5),0.5));
                float n2 =  smoothstep(0.05,1,distance(frac(_noiseTilling*float2(7,3)*v.uv+_Time.y*float2(-1,0)*0.25),0.5));
                n+=n2*_Color.a;

                o.vertex = UnityObjectToClipPos(v.vertex + _noise*n*v.normal);
                o.uv = v.uv;
                
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
            
			sampler2D _RenderTex;	
            float4 _GlowColor;
			
            float4 frag (v2f i) : SV_Target
            {
                i.uv.z = smoothstep(0,500,i.uv.z/2);

                _num = 5*smoothstep(0,0.25,i.uv.y)*smoothstep(0,1.5,1-i.uv.y)*_Color.a*_Color.a*_num/(3+i.uv.z);

                float2 uv1 =float2( i.uv.xy.x*0.25 -i.uv.xy.y*0.95 , i.uv.xy.x*0.95 + i.uv.xy.y*0.25);

                float2 uv2 =float2( i.uv.xy.x*0.55 -i.uv.xy.y*0.15 , i.uv.xy.x*0.15 + i.uv.xy.y*0.55);
                
                float2 uv3 =float2( i.uv.xy.x*0.45 -i.uv.xy.y*0.25 , i.uv.xy.x*0.25 + i.uv.xy.y*0.45);

                float v1 = VFX_MillionPartical(i.uv.z,uv1 ,float2(0,-0.5) ,_Step,_num,_overallSpeed ,_waveSpeed ,_waveAmp ,_twinklingSpeed ,_twinklingAmp);
                
                float v2 = VFX_MillionPartical(i.uv.z,uv2 ,float2(-0.15,-0.75) ,_Step*1.5,_num,_overallSpeed/2 ,_waveSpeed/2 ,_waveAmp/2 ,_twinklingSpeed/2 ,_twinklingAmp/2);
                
                float v3 = VFX_MillionPartical(i.uv.z,uv3,float2(-0.2,-0.35) ,_Step*1.25,_num,_overallSpeed /3,_waveSpeed /3.5,_waveAmp/3.5 ,_twinklingSpeed/3.5 ,_twinklingAmp/3.5);
                
                float Final = v1+v2+v3;

                

                fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                fixed3 worldNormal = normalize(i.worldNormal);

                float Rim = 1-saturate(smoothstep(0,0.5,dot(worldNormal,worldViewDir) ));
                float Rim2 = 1-saturate(smoothstep(0,0.75,dot(worldNormal,worldViewDir) ));
                float Rim3 = 1-saturate(smoothstep(0.25,1,dot(worldNormal,worldViewDir) ));

				//內容底背景
				float2 scruv = i.scrPos.xy/i.scrPos.w;
				float4 col = tex2D(_RenderTex, scruv + Rim2/10);

             return col + Final*_Color*(Rim3) + _GlowColor*smoothstep(0.5,1,1-i.uv.x-0.25)*smoothstep(0,0.25,i.uv.x+0.75) +Rim*_Color/20;
             
            }

            ENDCG
        }
    }
}
