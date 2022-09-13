Shader "GGDog/Arm_GodFlower"
{
    Properties
    {
        _LightDir ("LightDir", Vector) = (0,0,0,0)
        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        _Distortion ("Distortion", Range(0,1)) = 0.75
        [HDR]_SSSColor("SSS Color",Color) = (1,0.5,0.5,1)
        _ThicknessTex ("Thickness Tex", 2D) = "black" {}
		
		[HDR]_DissolveColor("DissolveColor",Color) = (1,1,1,1)
		[HDR]_DissolveDirColor("_DissolveDirColor",Color) = (1,1,1,1)
		_DissolveBackDirColor("_DissolveBackDirColor",Color) = (1,1,1,1)
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
            #include "Lighting.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
                o.worldPos = mul(v.vertex,unity_WorldToObject).xyz;

                return o;
            }
			float _Distortion;

			float4 _Color;
			float4 _ShadowColor;
			float4 _SSSColor;
			
            sampler2D _ThicknessTex;
			float4 _LightDir;
            
            float4 frag (v2f i) : SV_Target
            {
			
				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				_Distortion = _Distortion+0.05*Time_y*cos(_Time.y);


                float4 thickness = tex2D(_ThicknessTex, i.uv);

                float3 WorldNormal = normalize(i.worldNormal);
                //float3 LightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

				float NdotL = saturate(dot(WorldNormal,_LightDir));
				float Rim = 1-saturate(dot(WorldNormal,ViewDir));

				float3 H = normalize( ViewDir + WorldNormal * _Distortion);
				float I = saturate(dot(ViewDir,-H));

				float4 FinalColor = lerp(_ShadowColor + ( (Rim+I) * _SSSColor * thickness) ,_Color ,NdotL ) ;

                return FinalColor;
            }
            ENDCG
        }

        
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
            
			//Noise¹Ï­pºâ
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
            
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex + 0.01*v.normal*snoise(float2(7,3)*o.uv-_Time.y));

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            fixed4 _DissolveColor;
            fixed4 _DissolveDirColor;
            fixed4 _DissolveBackDirColor;
            
            fixed4 frag (v2f i) : SV_Target
            {
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

                fixed3 LightDir = normalize(_WorldSpaceLightPos0.xyz);

                fixed4 DColor =  lerp( _DissolveBackDirColor, _DissolveDirColor , smoothstep(-0.5,0.25,dot(worldNormal,LightDir))*(1-i.uv.y)  )+ smoothstep(0,1,1-dot(worldNormal,worldViewDir))*_DissolveColor;

                return  smoothstep(0,1,1-dot(worldNormal,worldViewDir))*_DissolveBackDirColor*i.uv.y/4;
            }
            ENDCG
        }
    }
}
