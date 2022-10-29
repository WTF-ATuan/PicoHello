// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Unlit/Low-Enemy_JellyFlower"
{
    Properties
    {
        _NoiseTiling ("Noise Density", Float) = 1
        _NoiseStrength ("Noise Strength", Range(0,5)) = 1.5
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        [HDR]_TexColor ("Tex Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off

        Blend One One

        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half4 scrPos : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _TexColor;
            sampler2D _MainTex2;
            
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
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-0.24,0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                //D = 1-max(max(D,D2),D3);
                D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }
            
            half _NoiseTiling;
            half _NoiseStrength;
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);
                
				half2 worldpos = mul(unity_ObjectToWorld, v.vertex).xy;

                half Noise =WaterTex(v.vertex.xy*_NoiseTiling,50,0.5) + WaterTex(v.vertex.xy*_NoiseTiling,30,-1); 

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*(Noise*0.75-1)*0.001*_NoiseStrength);

               // o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                
                half3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

                o.uv.z = dot(worldNormal,worldViewDir);

				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
                
				float2 scruv = i.scrPos.xy/i.scrPos.w;

                float col = tex2D(_MainTex, 8*scruv+2*_Time.y*float2(-0.025,0.2)).b;

                float col2 = tex2D(_MainTex,6*scruv+2*_Time.y*float2(0.015,0.3)).b;

                col = min(col,col2);

                float4 FinalColor = col*_Color;

                float Rim = 1-saturate(smoothstep(-5,0,i.uv.z ));
                float Rim2 = saturate(smoothstep(-1,0,i.uv.z ));
                
                float col3 = tex2D(_MainTex2, i.uv.xy*2+_Time.y*float2(0.75,0.15)).b;

                return FinalColor*Rim*2 +Rim2*col3*_TexColor/2;
            }
            ENDCG
        }
    }
}
