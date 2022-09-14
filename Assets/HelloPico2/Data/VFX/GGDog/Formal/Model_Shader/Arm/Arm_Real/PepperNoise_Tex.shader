Shader "Unlit/PepperNoise_Tex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_ShadowColor("Shadow Color",Color) = (0,0,0,1)
        _NoiseTex ("Texture", 2D) = "white" {}
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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 worldNormal : NORMAL;
                half4 SrcUV : TEXCOORD1;
            };

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
		     half2 unity_voronoi_noise_randomVector (half2 UV, half offset)
			{
			    half2x2 m = half2x2(15.27, 47.63, 99.41, 89.98);
			    UV = frac(sin(mul(UV, m)) * 46839.32);
				return half2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
			}
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
				o.worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);

                //抓取螢幕截圖的位置
				o.SrcUV = ComputeScreenPos(o.vertex);  
                
                //CameraDistance
				o.SrcUV.z = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);

                return o;
            }
            
			half4 _LightDir;
            half4 _ShadowColor;
            half4 frag (v2f i) : SV_Target
            {

                half2 srcUV = i.SrcUV.xy/i.SrcUV.w;

                half noise = tex2D(_NoiseTex,30*i.SrcUV.z* srcUV-10*_Time.y).r;
                half noise2 = tex2D(_NoiseTex,30*i.SrcUV.z* srcUV+10*_Time.y).r;

                noise = smoothstep(0.5,0.5, (noise+noise2)/2.5 );

               // half2 N = unity_voronoi_noise_randomVector(50*i.uv+_Time.y,500);
               
                float3 WorldNormal = normalize(i.worldNormal);
				float NdotL = saturate(dot(WorldNormal,_LightDir));

                //noise = lerp(1,noise,smoothstep(0,0.5,NdotL+Rim));
                
                half4 tex = tex2D(_MainTex,i.uv);

                half4 col = lerp(tex,tex*_ShadowColor+_ShadowColor*noise/1,smoothstep(0,0.5,NdotL));
                      col = lerp(tex,col,smoothstep(0,0.75,1-i.uv.y));
                      

                return col;
            }
            ENDCG
        }
    }
}
