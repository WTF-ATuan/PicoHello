Shader "GGDog/Real_PepperNoise_Tex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_ShadowColor("Shadow Color",Color) = (0,0,0,1)
        _NoiseTex ("Texture", 2D) = "white" {}
        _NoiseTexTilling ("NoiseTex Tilling", Float) = 1
        _LightDir ("LightDir", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float3 worldNormal : NORMAL;
                float4 SrcUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _NoiseTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
				o.worldNormal = normalize(mul(v.normal,(half3x3)unity_WorldToObject));

                //抓取螢幕截圖的位置
				o.SrcUV = ComputeScreenPos(o.vertex);  
                
                //CameraDistance
				o.SrcUV.z = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);

                return o;
            }
            
			float4 _LightDir;
            float4 _ShadowColor;
            
            float _NoiseTexTilling;
            
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

            float4 frag (v2f i) : SV_Target
            {

                float2 srcUV = i.SrcUV.xy/i.SrcUV.w;
                /*
                float noise = tex2D(_NoiseTex,_NoiseTexTilling*30*i.SrcUV.z* srcUV-10*_Time.y).r;
                float noise2 = tex2D(_NoiseTex,_NoiseTexTilling*30*i.SrcUV.z* srcUV+10*_Time.y).r;

                noise = smoothstep(0.5,0.5, (noise+noise2)/2.5 );
                */
                float noise = WaterTex(srcUV*i.SrcUV.z,50,5)*2;

				float NdotL = saturate(dot(i.worldNormal,_LightDir));

                float4 tex = tex2D(_MainTex,i.uv);

                float4 col = lerp(tex,tex*_ShadowColor+_ShadowColor*noise/1,smoothstep(0,0.5,NdotL));
                      col = lerp(tex,col,smoothstep(0,0.75,1-i.uv.y));
                      

                return col;
            }
            ENDCG
        }
    }
}
