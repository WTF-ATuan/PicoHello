Shader "Unlit/Tunnel"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", COLOR) = (1,1,1,1)
        _ShadowColor ("ShadowColor", COLOR) = (0.5,0.5,0.5,1)
        [HDR]_HDRColor ("HDRColor", COLOR) = (1,1,1,1)
		
        _ColorLerp("ColorLerp", Range(0,1)) = 0

        _Color2 ("Color2", COLOR) = (1,1,1,1)
        _ShadowColor2 ("ShadowColor2", COLOR) = (0.5,0.5,0.5,1)
        [HDR]_HDRColor2 ("HDRColor2", COLOR) = (1,1,1,1)

        _HDRColorIntense("HDRColorIntense", Range(1,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			

			float2 unity_gradientNoise_dir(float2 p)
			{
				p = p % 289;
				float x = (34 * p.x + 1) * p.x % 289 + p.y;
				x = (34 * x + 1) * x % 289;
				x = frac(x / 41) * 2 - 1;
				return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
			}
			
			float unity_gradientNoise(float2 p)
			{
				float2 ip = floor(p);
				float2 fp = frac(p);
				float d00 = dot(unity_gradientNoise_dir(ip), fp);
				float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
				float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
				float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
				fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
				return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
			}

			void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
			{
				Out = unity_gradientNoise(UV * Scale) + 0.5;
			}


            v2f vert (appdata v)
            {
                v2f o;

				//float noise = saturate( frac(sin(dot(v.uv, float2(12.9898, 78.233)+sin(_Time.y*0.00015) ))*43758.5453));

				float Out;
				Unity_GradientNoise_float(v.uv+float2(1,0.5)*_Time.y/10+float2(0,1)*_Time.y/10,30,Out);

				float Out2;
				Unity_GradientNoise_float(v.uv-float2(0.5,0.75)*_Time.y/10+float2(0,1)*_Time.y/10,70,Out2);

				Out*=1;
				Out2*=0.5;
				Out*=Out2;
				
				float Out3;
				Unity_GradientNoise_float(v.uv-float2(0,-0.5)*_Time.y/8  +float2(0,1)*_Time.y/10,10,Out3);
				
				float Out4;
				Unity_GradientNoise_float(v.uv-float2(0,-1.5)*_Time.y/8  +float2(0,1)*_Time.y/10,7,Out4);
				Out3*=Out4;

				float Noise =  Out*50 + (0.5-Out3)*100;

				float uv_dis = smoothstep(0,0.1,v.uv.x*(1-v.uv.x));

                o.vertex = UnityObjectToClipPos(v.vertex + Noise*v.normal*uv_dis);


                o.uv = TRANSFORM_TEX(v.uv, _MainTex);


				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex + Noise*v.normal*uv_dis).xyz;

                return o;
            }
			float4 _Color;
			float4 _ShadowColor;
			float4 _HDRColor;
			float _HDRColorIntense;
			float _ColorLerp;

			float4 _Color2;
			float4 _ShadowColor2;
			float4 _HDRColor2;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv+float2(-0.5,1)*_Time.y/10);
                fixed4 col2 = tex2D(_MainTex, i.uv+float2(0.25,1)*_Time.y/10);
				col*=col2;
				
				float uv_dis = smoothstep(0,0.175,i.uv.x*(1-i.uv.x));
				col*=uv_dis;
				col.rgb = lerp(float3(0.5,0.5,0.5),float3(1,1,1),col.r);
				
				

				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

				float Rim = 1-saturate(smoothstep(0,0.5,dot(worldNormal,worldViewDir)));
				
				float4 col3 = col*Rim;

				col.rgb = lerp(col/1.25,1,smoothstep(0.15,1,Rim));
				
				_Color = lerp(_Color,_Color2,_ColorLerp);
				_ShadowColor = lerp(_ShadowColor,_ShadowColor2,_ColorLerp);
				_HDRColor = lerp(_HDRColor,_HDRColor2,_ColorLerp);


				col.rgb = lerp(_ShadowColor,_Color,smoothstep(0.5,1,col.r));

				col.rgb = lerp(col,_HDRColor*_HDRColorIntense,smoothstep(0.1,1,(col3.r+col3.g+col3.b)/3));

                return col;
            }
            ENDCG
        }
    }
}
