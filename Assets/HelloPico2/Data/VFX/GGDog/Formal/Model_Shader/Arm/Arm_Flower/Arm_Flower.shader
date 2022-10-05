Shader "Unlit/Arm_Flower"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_RimColor("RimColor",Color) = (1,1,1,1)
        
		_RimSmooth("RimSmooth",Range(0,1)) = 1

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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half4 scrPos : TEXCOORD1;
            };
            
			half2 unity_gradientNoise_dir(half2 p)
			{
				p = p % 289;
				half x = (34 * p.x + 1) * p.x % 289 + p.y;
				x = (34 * x + 1) * x % 289;
				x = frac(x / 41) * 2 - 1;
				return normalize(half2(x - floor(x + 0.5), abs(x) - 0.5));
			}
			
			half unity_gradientNoise(half2 p)
			{
				half2 ip = floor(p);
				half2 fp = frac(p);
				half d00 = dot(unity_gradientNoise_dir(ip), fp);
				half d01 = dot(unity_gradientNoise_dir(ip + half2(0, 1)), fp - half2(0, 1));
				half d10 = dot(unity_gradientNoise_dir(ip + half2(1, 0)), fp - half2(1, 0));
				half d11 = dot(unity_gradientNoise_dir(ip + half2(1, 1)), fp - half2(1, 1));
				fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
				return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
			}

			void Unity_GradientNoise_float(half2 UV, half Scale, out half Out)
			{
				Out = unity_gradientNoise(UV * Scale) + 0.5;
			}

            sampler2D _MainTex;
            half4 _MainTex_ST;
            
			half4 _RimColor;

            half4 _Color;
            half4 _ShadowColor;
            half3 _LightDir;
            
            v2f vert (appdata v)
            {
                v2f o;
				o.scrPos = ComputeScreenPos(v.vertex);  //抓取螢幕截圖的位置
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				half3 worldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));

                //NdotV
                o.uv.z = dot(worldNormal,worldViewDir);
                //NdotL
                o.uv.w = dot(worldNormal,_LightDir);

                return o;
            }
            
            half _RimSmooth;
            half _h;
            half4 _DissolveColor;
            half4 _DissolveDirColor;
            half4 _DissolveBackDirColor;
            half _injured;
            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv.xy);

				half Rim = 1-saturate(smoothstep(0,2*_RimSmooth,i.uv.z ));

                col = lerp(_ShadowColor, col,  (col.r+col.g+col.b)/3  );

                //_ShadowColor = lerp(_ShadowColor*col/5, _ShadowColor,  (col.r+col.g+col.b)/3  );

                half4 FinalColor =  lerp( _ShadowColor, col*_Color , smoothstep(-0.5,0.25,i.uv.w)*(1-i.uv.y)  );
                
                FinalColor += smoothstep(0,3,1-i.uv.w)*Rim*_RimColor;

                half4 DColor =  lerp( _DissolveBackDirColor, _DissolveDirColor , smoothstep(-0.5,0.25,i.uv.w)*(1-i.uv.y)  )+ smoothstep(0,1*_RimSmooth,1-i.uv.z)*_DissolveColor;

                half _hh = _h;

                _h=(1-_h)*1.75;
                half h =1- smoothstep(0.45,0.7, _h-i.uv.y );
                half h2 =1- smoothstep(0.5,0.5, _h-i.uv.y );
                
                
                FinalColor =  FinalColor +h*_DissolveColor;

                FinalColor = lerp( DColor, FinalColor , (1- h2));
                

                //injured
                
				half2 scruv = i.scrPos.xy/i.scrPos.w;

				half Out_noise;
                Unity_GradientNoise_float(2*scruv+_Time.y*0.075,150,Out_noise);
				half Out_noise2;
                Unity_GradientNoise_float(2*scruv-_Time.y*0.15,100,Out_noise2);
                scruv+= (Out_noise+Out_noise2)*half2(0.003,0.003);

				half Out;
                Unity_GradientNoise_float(2*scruv,30,Out);
                
				half Out2;
                Unity_GradientNoise_float(2*scruv,50,Out2);

                Out*=Out2;

                Out = smoothstep(0.15,0.5,Out-_injured+0.75);

                FinalColor = lerp(FinalColor,half4(0,0,0,1),saturate((1-Out)));
                
               // FinalColor += (1-Out2)*smoothstep(0.8,1.1,(1-Out))*_RimColor*5*saturate(1-_injured*_injured*_injured);

                clip(Out-0.01);

                return  FinalColor;
            }
            ENDCG
        }


        /*
        Pass
        {
            ZWrite off
            Blend One One
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
                half4 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
            };
            
			//Noise圖計算
			half3 mod289(half3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
			half2 mod289(half2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
			half3 permute(half3 x) { return mod289(((x*34.0) + 1.0)*x); }
			half snoise(half2 v)
			{
				// Precompute values for skewed triangular grid
				const half4 C = half4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
					0.366025403784439,	// 0.5*(sqrt(3.0)-1.0)
					-0.577350269189626, // -1.0 + 2.0 * C.x
					0.024390243902439	// 1.0 / 41.0
					);


				// First corner (x0)
				half2 i = floor(v + dot(v, C.yy));
				half2 x0 = v - i + dot(i, C.xx);

				// Other two corners (x1, x2)
				half2 i1;
				i1 = (x0.x > x0.y) ? half2(1.0, 0.0) : half2(0.0, 1.0);
				half2 x1 = x0.xy + C.xx - i1;
				half2 x2 = x0.xy + C.zz;

				// Do some permutations to avoid
				// truncation effects in permutation
				i = mod289(i);
				half3 p = permute(
					permute(i.y + half3(0.0, i1.y, 1.0))
					+ i.x + half3(0.0, i1.x, 1.0));

				half3 m = max(0.5 - half3(
					dot(x0, x0),
					dot(x1, x1),
					dot(x2, x2)
					), 0.0);

				m = m*m;
				m = m*m;

				// Gradients: 
				//  41 pts uniformly over a line, mapped onto a diamond
				//  The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

				half3 x = 2.0 * frac(p * C.www) - 1.0;
				half3 h = abs(x) - 0.5;
				half3 ox = floor(x + 0.5);
				half3 a0 = x - ox;
				
				// Normalise gradients implicitly by scaling m
				// Approximation of: m *= inversesqrt(a0*a0 + h*h);
				m *= 1.79284291400159 - 0.85373472095314 *(a0*a0 + h*h);

				// Compute final noise value at P
				half3 g;
				g.x = a0.x  * x0.x + h.x  * x0.y;
				g.yz = a0.yz * float2(x1.x, x2.x) + h.yz * float2(x1.y, x2.y);

				return 130.0 * dot(m, g);
			}
            
            half _h;
            half3 _LightDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv.xy = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex + saturate(_h*2)*0.02*v.normal*snoise(half2(7,3)*o.uv-_Time.y));
                
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
				half3 worldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));

                //NdotV
                o.uv.z = smoothstep(0,1,1-dot(worldNormal,worldViewDir));
                //NdotL
                o.uv.w = dot(worldNormal,_LightDir);

                return o;
            }
            half4 _DissolveColor;
            half4 _DissolveDirColor;
            half4 _DissolveBackDirColor;
            
            half4 frag (v2f i) : SV_Target
            {
                half4 DColor =  lerp( _DissolveBackDirColor, _DissolveDirColor , smoothstep(-0.5,0.25,i.uv.w)*(1-i.uv.y)  )+ i.uv.z*_DissolveColor;

                half4 FinalColor = saturate(_h*2)*i.uv.z*_DissolveBackDirColor*i.uv.y*0.25;

                clip(FinalColor.a-0.00015);

                return  FinalColor;
            }
            ENDCG
        }*/
    }
}
