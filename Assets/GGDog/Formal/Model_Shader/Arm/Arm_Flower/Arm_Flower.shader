Shader "Unlit/Arm_Flower"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RimColor("RimColor",Color) = (1,1,1,1)
        [HDR]_Color("Color",Color) = (1,1,1,1)
        _ShadowColor("Shadow Color",Color) = (1,1,1,1)
        _h("_h",Range(0,1)) = 0
		[HDR]_DissolveColor("DissolveColor",Color) = (1,1,1,1)
		[HDR]_DissolveDirColor("_DissolveDirColor",Color) = (1,1,1,1)
		_DissolveBackDirColor("_DissolveBackDirColor",Color) = (1,1,1,1)

        _injured("_injured",Range(0,1)) = 0

        _LightDir ("LightDir", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
        //ZWrite off
        Stencil {
            Ref 1
            Comp Always
            Pass replace
        }
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
				half4 scrPos : TEXCOORD3;
            };
            
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
			float4 _RimColor;

            fixed4 _Color;
            fixed4 _ShadowColor;
            v2f vert (appdata v)
            {
                v2f o;
				o.scrPos = ComputeScreenPos(v.vertex);  //抓取螢幕截圖的位置
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                

                return o;
            }
            float _h;
            fixed4 _DissolveColor;
            fixed4 _DissolveDirColor;
            fixed4 _DissolveBackDirColor;
            float _injured;
            fixed3 _LightDir;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

                fixed3 LightDir = _LightDir;

				float Rim = 1-saturate(smoothstep(0,2,dot(worldNormal,worldViewDir) ));

                col = lerp(_ShadowColor, col,  (col.r+col.g+col.b)/3  );

                //_ShadowColor = lerp(_ShadowColor*col/5, _ShadowColor,  (col.r+col.g+col.b)/3  );

                fixed4 FinalColor =  lerp( _ShadowColor, col*_Color , smoothstep(-0.5,0.25,dot(worldNormal,LightDir))*(1-i.uv.y)  );
                
                FinalColor += smoothstep(0,3,1-dot(worldNormal,LightDir))*Rim*_RimColor;

                
				float3 H = normalize( -normalize(_WorldSpaceCameraPos.xyz) + worldNormal );
				float I =pow(saturate(dot(worldViewDir,-H)) , 2) * 2;

                fixed4 DColor =  lerp( _DissolveBackDirColor, _DissolveDirColor , smoothstep(-0.5,0.25,dot(worldNormal,LightDir))*(1-i.uv.y)  )+ smoothstep(0,1,1-dot(worldNormal,worldViewDir))*_DissolveColor+I*_DissolveColor;

                float _hh = _h;

                _h=(1-_h)*1.75;
                float h =1- smoothstep(0.45,0.7, _h-i.uv.y );
                float h2 =1- smoothstep(0.5,0.5, _h-i.uv.y );
                
                
                FinalColor =  FinalColor +h*_DissolveColor;

                FinalColor = lerp( DColor, FinalColor , (1- h2));
                

                //injured
                
				half2 scruv = i.scrPos.xy/i.scrPos.w;

				float Out_noise;
                Unity_GradientNoise_float(scruv+_Time.y*0.075,150,Out_noise);
				float Out_noise2;
                Unity_GradientNoise_float(scruv-_Time.y*0.15,100,Out_noise2);
                scruv+= (Out_noise+Out_noise2)*float2(0.003,0.003);

				float Out;
                Unity_GradientNoise_float(scruv,30,Out);
                
				float Out2;
                Unity_GradientNoise_float(scruv,50,Out2);

                Out*=Out2;

                Out = smoothstep(0.15,0.5,Out-_injured+0.75);

                FinalColor = lerp(FinalColor,float4(0,0,0,1),saturate((1-Out)));
                
                FinalColor += (1-Out2)*smoothstep(0.8,1.1,(1-Out))*_RimColor*5*saturate(1-_injured*_injured*_injured);

                clip(Out-0.01);

                return  FinalColor;
            }
            ENDCG
        }


        
        Pass
        {
            Blend One One
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
            
			//Noise圖計算
			float3 mod289(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
			float2 mod289(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
			float3 permute(float3 x) { return mod289(((x*34.0) + 1.0)*x); }
			float snoise(float2 v)
			{
				// Precompute values for skewed triangular grid
				const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
					0.366025403784439,	// 0.5*(sqrt(3.0)-1.0)
					-0.577350269189626, // -1.0 + 2.0 * C.x
					0.024390243902439	// 1.0 / 41.0
					);


				// First corner (x0)
				float2 i = floor(v + dot(v, C.yy));
				float2 x0 = v - i + dot(i, C.xx);

				// Other two corners (x1, x2)
				float2 i1;
				i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
				float2 x1 = x0.xy + C.xx - i1;
				float2 x2 = x0.xy + C.zz;

				// Do some permutations to avoid
				// truncation effects in permutation
				i = mod289(i);
				float3 p = permute(
					permute(i.y + float3(0.0, i1.y, 1.0))
					+ i.x + float3(0.0, i1.x, 1.0));

				float3 m = max(0.5 - float3(
					dot(x0, x0),
					dot(x1, x1),
					dot(x2, x2)
					), 0.0);

				m = m*m;
				m = m*m;

				// Gradients: 
				//  41 pts uniformly over a line, mapped onto a diamond
				//  The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

				float3 x = 2.0 * frac(p * C.www) - 1.0;
				float3 h = abs(x) - 0.5;
				float3 ox = floor(x + 0.5);
				float3 a0 = x - ox;
				
				// Normalise gradients implicitly by scaling m
				// Approximation of: m *= inversesqrt(a0*a0 + h*h);
				m *= 1.79284291400159 - 0.85373472095314 *(a0*a0 + h*h);

				// Compute final noise value at P
				float3 g;
				g.x = a0.x  * x0.x + h.x  * x0.y;
				g.yz = a0.yz * float2(x1.x, x2.x) + h.yz * float2(x1.y, x2.y);

				return 130.0 * dot(m, g);
			}
            
            float _h;
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex + saturate(_h*2)*0.02*v.normal*snoise(float2(7,3)*o.uv-_Time.y));

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            fixed4 _DissolveColor;
            fixed4 _DissolveDirColor;
            fixed4 _DissolveBackDirColor;
            
            fixed3 _LightDir;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);
                
                fixed3 LightDir = _LightDir;

                fixed4 DColor =  lerp( _DissolveBackDirColor, _DissolveDirColor , smoothstep(-0.5,0.25,dot(worldNormal,LightDir))*(1-i.uv.y)  )+ smoothstep(0,1,1-dot(worldNormal,worldViewDir))*_DissolveColor;

                return  saturate(_h*2)*smoothstep(0,1,1-dot(worldNormal,worldViewDir))*_DissolveBackDirColor*i.uv.y/4;
            }
            ENDCG
        }
    }
}
