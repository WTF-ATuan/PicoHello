Shader "GGDog/Space_Test/Env"
{
    Properties
    {
		[HDR]_Color("Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_FarColor("Far Color",Color) = (0.5,0.5,0.5,1)
		_BackFogColor("Back Fog Color",Color) = (0,0,0,1)
		[HDR]_FogColor("Fog Color",Color) = (1,1,1,1)
        _FogPos ("Fog Pos", Range(-200,200)) = 77
		
		_UV_Tilling ("UV_Tilling", Float) = 1
		_UV_Offset ("UV_Offset", Float) = 1

		_Rim("Rim",Range(0.01,10)) = 5
		_LOD_LowColor("LOD LowColor",Color) = (1,1,1,1)
		
        [Toggle(_False)]_ModelReflection("Model Reflection",Float) = 0
        _Reflect("Reflection",Range(0,3)) = 1.5

        _ReflectRGBOffSet("Reflection RGBOffSet",Range(0,20)) = 10

        
		[KeywordEnum(World, Local)] _REFAXIS("Reflection Axis", Float) = 0.0

		[KeywordEnum(On, Off)] _REF("Reflection", Float) = 0
    }
    SubShader
    {
        Pass
        {
			Tags {"Queue" = "Geometry"}
            CGPROGRAM
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
			
			#pragma shader_feature _REFAXIS_WORLD  _REFAXIS_LOCAL
			#pragma shader_feature _REF_ON  _REF_OFF

			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
			//snoise
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

            float frac_Noise(float2 UV, float Tilling)
            {

                UV = UV*Tilling/100;

                float2 n_UV =float2( UV.x *0.75 - UV.y*0.15 ,UV.x*0.15 + UV.y*0.75);

                float2 n2_UV =float2( UV.x *0.25 - UV.y*0.5 ,UV.x*0.5 + UV.y*0.25);

                float timeY =_Time.y;
                float n0 =  smoothstep(0.15,1,1-distance(frac(1*n_UV+timeY*float2(-0.3,-0.75)*0.55),0.5));
                float n01 =  smoothstep(0.3,1,1-distance(frac(0.75*n2_UV+timeY*float2(0.75,0.5)*0.25),0.5));

                float n02 =  smoothstep(0.5,1,1-distance(frac(0.25*UV+timeY*float2(0.5,-0.25)*0.75),0.5));

                float n03 =  smoothstep(0.5,1,1-distance(frac(0.15*UV+timeY*float2(0.25,-0.5)*0.75),0.5));


                float n =  smoothstep(0.15,1,distance(frac(1*n_UV+n0/3-n02/1+timeY*float2(0.7,1)*0.25),0.5)) ;

                float n2 =  smoothstep(0.3,1,distance(frac(1.25*n2_UV-n01/3+n02/1.5+timeY*float2(-0.2,-0.75)*0.75),0.5)) ;

                n+= n2;

               return saturate(n+0.25);
            }
            float _ModelReflection;
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

#if _REFAXIS_WORLD
                float3 YPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                //float n = frac_Noise(YPos.xy,1);
                float n = snoise(YPos.xy*0.005+_Time.y/1.5);
                float worldPosY = step(YPos.y,-30);
                float worldPosY_smoothstep = saturate(smoothstep(0,50,1-YPos.y));
#elif _REFAXIS_LOCAL
				float3 YPos = v.vertex.xyz;
                //float n = frac_Noise(1000*YPos.xy,1);
                float n = snoise(1000*YPos.xy*0.005+_Time.y/1.5);
                float worldPosY = step(YPos.y,0);
                float worldPosY_smoothstep = 1;
#endif
                o.vertex = UnityObjectToClipPos(v.vertex + _ModelReflection * 0.1*float3(-sign(YPos.x),0,0.5)*n*worldPosY*worldPosY_smoothstep);

                return o;
            }
            float4 _Color;
            float4 _ShadowColor;

            float4 _SkyColor;
			
            float4 _FarColor;
            float4 _BackFogColor;
            float4 _FogColor;
            float _FogPos;

            float _Reflect;
            float _ReflectRGBOffSet;
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float Z_smooth = smoothstep(600,1200,i.worldPos.z);

                float4 col =  lerp( _BackFogColor/2, _SkyColor , Z_smooth );

				col = lerp(col,_FogColor,Z_smooth*(1-smoothstep(-100,300,i.worldPos.y+_FogPos)));


                return col;
          }
            ENDCG
        }
    }
	
    SubShader
    {
		LOD 0 
        Pass
        {
			Tags { "RenderType"="Opaque" }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                return 0.75 ;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
