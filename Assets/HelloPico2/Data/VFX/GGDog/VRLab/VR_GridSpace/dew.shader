Shader "GGDog/dew"
{
	Properties
	{
        _RenderTex("Render Tex", 2D) = "white" {}

		_SpecularColor("Specular Color",Color) = (1,1,1,1)

		[HDR]_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		
		_RimColor("Rim Color",Color) = (1,1,1,1)
		
        _Gloss("Gloss",Range(1,200)) = 10

		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
		_RandomOffset("RandomOffset", Vector) = (0, 0, 0,0)
	}
	SubShader
	{
		Pass
		{	
			Tags {  "RenderType" = "Opaque" "Queue" = "Geometry+1"}

			CGPROGRAM
            #include "Lighting.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				half4 vertex : POSITION;
				half3 normal : NORMAL;
			};

			struct v2f
			{
				half4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
				half4 CameraDistance : TEXCOORD4;
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

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _RandomOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			sampler2D _RenderTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.worldNormal = UnityObjectToWorldNormal(v.normal); 
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.scrPos = ComputeGrabScreenPos(o.vertex);  //抓取螢幕截圖的位置
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
				half2 scruv = o.scrPos.xy/o.scrPos.w;

				float Noise1 = snoise(v.vertex.xy*0.55+_Time.y*(0.35,-0.25)*2.5 + UNITY_ACCESS_INSTANCED_PROP(Props, _RandomOffset).xy);
				float Noise2 = snoise(v.vertex.yz*0.75+_Time.y*(-0.15,0.45)*1.5 + UNITY_ACCESS_INSTANCED_PROP(Props, _RandomOffset).xy);

				float noise = (Noise1*0.3+Noise2*0.2);

				o.vertex = UnityObjectToClipPos(v.vertex /1.15 + v.normal * noise*noise);
				
				return o;
			}

            half _Gloss;
			
			half3 _Vector;

			half4 frag (v2f i) : SV_Target
			{

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);

				
				half NdotV = dot(worldNormal,worldViewDir);
				//Rim
				half Rim = saturate(1-smoothstep(-0.25,1,NdotV));

				//Dark
                half DarkPart =  smoothstep( 0,0.35, (1-(2*dot(worldNormal,half3(0.1,0.5,-1) )-1)) * (2*dot(worldNormal,worldViewDir)-0.5) -0.35);
				
                half DarkPart2 = smoothstep( 0.5,1, 1-dot(worldNormal,worldViewDir+half3(-0.2,-0.5,0)) +0.1)/1.5;

				DarkPart = lerp(DarkPart+DarkPart2,DarkPart2,DarkPart2)/2;
				DarkPart*=Rim;


				//高光
				
                worldLightDir = normalize(mul(UNITY_MATRIX_MV,float4(worldLightDir,0)));

                half3 reflectDir = reflect(-worldLightDir,worldNormal);
				

                half Specular =  pow(max(0,dot(worldViewDir,reflectDir)),_Gloss);

                half Specular_s =  smoothstep(0,55,dot(worldViewDir,reflectDir))*0.55;
			
                half Specular2 = pow(max(0,dot(worldNormal,worldLightDir) ),_Gloss*20);

                half3 ViewNormal = normalize(mul(UNITY_MATRIX_MV,float4(-worldNormal,0)));
				
                half Specular3 = pow(max(0,dot(ViewNormal,worldLightDir) ),_Gloss*10);

				Specular += Specular2+Specular3 + Specular_s;


				//底光邊緣
                half Specular_Bottom = pow(saturate(dot(worldViewDir,-reflectDir)),_Gloss/20);
				
				//大塊的
                half Specular_Bottom2 =  smoothstep( 0,0.5, (1-(2*dot(worldNormal,worldLightDir )+0.15)) * (2*dot(worldNormal,worldViewDir )-0.5) +0.1)/3;

				Specular_Bottom = Specular_Bottom+Specular_Bottom2;

				Specular_Bottom*=Rim;

				Specular = smoothstep(0,0.001,Specular) + smoothstep(0.1,0.35,Specular_Bottom)*0.55;


				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb/2,DarkPart) ,1-saturate(Specular));

				
				//內容底背景
				half2 scruv = i.scrPos.xy/i.scrPos.w;

                half Rimscruv = 1-saturate(smoothstep(0,0.5,NdotV));
                half Rimscruv2 = saturate(smoothstep(0.25,1,NdotV));

				Rimscruv+=Rimscruv2;
				half4 refrCol = tex2D(_RenderTex, scruv + Rimscruv/50) ;
				
				FinalColor = lerp(FinalColor,refrCol*_ShadowColor,1-FinalColor.a);

				return FinalColor + Rim*_RimColor ;
			}
			ENDCG
		}
	}
}