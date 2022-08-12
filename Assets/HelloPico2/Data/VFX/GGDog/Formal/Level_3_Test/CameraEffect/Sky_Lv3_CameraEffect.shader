Shader "GGDog/Space_Test/Lv3_CameraEffect"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (1, 1, 1, 1)
        [HDR]_HorizonColor ("Horizon Color", Color) = (1, 1, 1, 1)
		
        _SmoothStepMin ("漸層度(最低)", Range(0, 1)) = 0
        _SmoothStepMax ("漸層度(最高)", Range(0, 1)) = 1

        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogPos ("Fog Pos", Range(-500,500)) = 0
		
        _OriScale ("Original Scale", Float) = 3000

        _Dis ("Dis", Range(0,1)) = 0
        [HDR]_SeethroughColor ("Seethrough Fade Color", Color) = (0.5, 0.5, 0.5, 1)
        [HDR]_EdgeColor ("Dissolve Edge Color", Color) = (2, 2, 2, 1)
    }
    SubShader
    {
		Tags { "Queue"="Transparent" }

        LOD 0
		
		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha

        Cull Front

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
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half4 worldPos : TEXCOORD1;
            };

            half4 _SkyColor;
            half4 _HorizonColor;

            half _SmoothStepMin;
            half _SmoothStepMax;
			
            half _OriScale;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				//Scale
				o.worldPos.w = length(unity_ObjectToWorld._m00_m10_m20)/_OriScale;

                return o;
            }
			
			//snoise
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
				g.yz = a0.yz * half2(x1.x, x2.x) + h.yz * half2(x1.y, x2.y);

				return 130.0 * dot(m, g);
			}
			

            half4 _FogColor;
            half _FogPos;

            half _Dis;
            half4 _SeethroughColor;
            half4 _EdgeColor;
			
            half4 frag (v2f i) : SV_Target
            {


				half scale = i.worldPos.w;
				
				half4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax, i.uv.y) );

				col = lerp(_FogColor,col,smoothstep(-100*scale,300*scale,i.worldPos.y+_FogPos));

				col = lerp(_HorizonColor,col,smoothstep(-1500*scale,250*scale,i.worldPos.z));
				
				half n =7* _Dis* saturate(snoise(2000*half2(2,2)*i.worldPos.xy/1000+_Time.y*half2(0,1.25)) + snoise(2000*half2(5,5)*i.worldPos.xy/1000-_Time.y*half2(0,1)));

				half fade =(i.uv.y+_Dis*(_OriScale+100))/_OriScale-0.05 ;

				col.a = 1 - smoothstep(0.45,0.5,fade-n);

				col = lerp(_SeethroughColor,col,col.a);

				col.a *= (1-_Dis);
				
				col = lerp(col,_EdgeColor,smoothstep(0.5,0.7,col.a)*smoothstep(0.425,0.5,fade-n));




                return col;

            }
            ENDCG
        }
    }
}
