Shader "GGDog/dew_beamchain"
{
	Properties
	{
        [IntRange]_joint ("joint", Range(0,50)) = 50
        _Scale ("Scale", Range(0,1)) = 1

        _RenderTex("Render Tex", 2D) = "white" {}
        _MainTex("MainTex", 2D) = "white" {}

		_SpecularColor("Specular Color",Color) = (1,1,1,1)

		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		
		_RimColor("Rim Color",Color) = (1,1,1,1)
		
        _Gloss("Gloss",Range(1,200)) = 10

		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
		_Distance_Intensive("Intensive",Range(-3,0))=-3
		_Distance_Radius("Radius",Range(0,30))=12

		
		_NoisePower("Noise Power",Range(0,1.5))=0
		_NoiseUV("Noise UV",Vector)=(1.5, 3, 0.1,0.1)

		_RainbowOffSet("Rainbow OffSet",Range(-1,1))=-0.31
		_Film("Film Intensive",Range(0,1))=0.3
		_Width("Film Intensive",Range(0,5))=2
	}
	SubShader
	{

		Pass
		{	
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
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
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
				half CameraDistance : TEXCOORD4;
			};
			
			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			uniform	half3 All_Pos[10];
			half _Distance_Intensive;
			half _Distance_Radius;
			half _NoisePower;
			half4 _NoiseUV;
			
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

			half _Scale;
            half _joint;

			v2f vert (appdata v)
			{
				v2f o;

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				/*
                half3 WorldPos = unity_ObjectToWorld._m03_m13_m23;

				half d;
				half3 n;
				
                for(int i=0; i<10; i++)
                {
				    d = saturate(distance( All_Pos[i],o.worldPos/length(o.worldNormal)) / (_Distance_Radius/(1+distance( All_Pos[i],o.worldPos)) ));

                    n =  ((o.worldPos-(o.worldNormal)/(1+distance( All_Pos[i],WorldPos)))-All_Pos[i])  *_Distance_Intensive *smoothstep(0,2.7,1-d);

                    v.vertex.xyz += saturate(smoothstep(0,0.5,saturate(distance( All_Pos[i],WorldPos))-0.2)) *n/(2+d);
                }
				
				*/
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
                v.uv.y = smoothstep(_joint/50,1,v.uv.y);

                half scale = smoothstep(0,1.75,1-v.uv.y)*_Scale;

                o.vertex = UnityObjectToClipPos(v.vertex - 0.01*v.normal*scale);


				return o;
			}
			
            half _Gloss;
			
			half3 _Vector;

            sampler2D _MainTex;

			sampler2D _RenderTex;	
			
            half _RainbowOffSet;
            half _Film;
            half _Width;
			

			//虹彩薄膜干涉函數
			half3 Rainbow(half gray , half offSet)
			{
                fixed3 col = 1;

				col =lerp( float4(1,0,0,1) , float3(0,1,0) , (gray)*_Width + offSet);
				col =lerp(             col , float3(0,0,1) , (gray)*_Width-0.5 + offSet);

				col+=0.5;

				col = lerp( 0 , 1-2*(1-col), step(0.5,col) );

				return col;
			}

			half4 frag (v2f i) : SV_Target
			{
				half CameraDistance950_1500 =  saturate(smoothstep(950,1500,i.CameraDistance));

				_ShadowColor = lerp(_ShadowColor,1,CameraDistance950_1500);
				_Gloss = lerp(_Gloss,1000,saturate(smoothstep(700,900,i.CameraDistance)));

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);


				//Rim
				half Rim = saturate(1-smoothstep(0,1,dot(worldNormal,worldViewDir)));

				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

                half Specular =  pow(max(0,dot(viewDir,reflectDir)),_Gloss);
                half Specular2 = pow(max(0,dot(worldNormal,worldLightDir) ),_Gloss*100);
                half Specular3 = pow(max(0,dot(worldNormal,worldLightDir-0.35)/1.157 ),_Gloss*10);

				Specular = Specular+Specular2+Specular3;

				Specular = smoothstep(0,0.001,saturate(Specular)*(1-Rim)*(Rim)) ;
				
				Specular = lerp(Specular,0,CameraDistance950_1500);

				half4 FinalColor = half4(_SpecularColor.rgb ,saturate(Specular));
				
				FinalColor =lerp(FinalColor,FinalColor/1.75,smoothstep(0,1.5,1-dot(viewDir,reflectDir)));
				
				

				//內容底背景

				half2 scruv = i.scrPos.xy/i.scrPos.w;

				Rim = lerp(Rim,0,CameraDistance950_1500);
				
				half Noise_Wave = smoothstep(0.35,1,i.uv.x)*frac(5*i.uv.x -_Time.y ) * frac(1-5*i.uv.x+_Time.y);
				half Noise_Wave2 = smoothstep(0.25,1,i.uv.x)*frac(3*i.uv.x -_Time.y ) * frac(1-3*i.uv.x+_Time.y);
				half Noise_Wave3 = smoothstep(0.15,1,i.uv.x)*frac(5*i.uv.x -_Time.y ) * frac(5-5*i.uv.x+_Time.y);

				half4 refrCol = tex2D(_RenderTex, scruv + Noise_Wave/2 + Noise_Wave2) ;

				refrCol = lerp(refrCol,refrCol*_ShadowColor, (1-smoothstep( 0,0.5,i.uv.y*(1-i.uv.y) ))*(i.uv.x) );

				FinalColor = lerp(FinalColor,refrCol,1-FinalColor.a);
				
				FinalColor += Rim*Rim*_RimColor;

				half4 R_col = lerp(_RimColor , half4(Rainbow(smoothstep(0,0.25,Noise_Wave2)*smoothstep(0.5,1,dot(worldNormal,worldLightDir) *(1-Rim)),_RainbowOffSet),1) , smoothstep(0,_Film,1-i.uv.x));

				FinalColor += smoothstep(0,0.75,Noise_Wave3) * R_col/1.5;
				
				half j = smoothstep(0.5,0.75,frac(2*i.uv.x-_Time.y)*(frac(i.uv.x-0.5*_Time.y)+0.5));
				FinalColor += smoothstep(-0.5,1,i.uv.x)*tex2D(_MainTex, 2*i.uv + 2*_Time.y*half2(-0.75,0))*_RimColor*j/2;
				

				return FinalColor;
			}
			ENDCG
		}
	}

	
}
