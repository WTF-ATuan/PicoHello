Shader "GGDog/dew_GPUInstance"
{
	Properties
	{
        _RenderTex("Render Tex", 2D) = "white" {}

		_SpecularColor("Specular Color",Color) = (1,1,1,1)
		_Color("Color",Color) = (0,0,0,1)

		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		
		_RimColor("Rim Color",Color) = (1,1,1,1)
		
        _Gloss("Gloss",Range(1,200)) = 10

		_Vector ("Light Direction", Vector) = (0, 0, 0)
		
		_Distance_Intensive("Intensive",Range(-3,0))=-3
		_Distance_Radius("Radius",Range(0,30))=12
	}
	SubShader
	{
       // GrabPass { "_BackTex123" }
	   
		LOD 200
		//ZTest Always
		Pass
		{	
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
				half CameraDistance : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			half4 _Color;
			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			uniform	half3 Follow_Pos;
			half _Distance_Intensive;
			half _Distance_Radius;
			
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



			v2f vert (appdata v)
			{
				v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                half3 WorldPos = unity_ObjectToWorld._m03_m13_m23;
				
				half d;
				half3 n;

				d = saturate(distance( Follow_Pos,o.worldPos/length(o.worldNormal)) / (_Distance_Radius/(1+distance( Follow_Pos,o.worldPos)) ));

                n =  ((o.worldPos-(o.worldNormal)/(1+distance( Follow_Pos,WorldPos)))-Follow_Pos)  *_Distance_Intensive *smoothstep(0,2.7,1-d);

                v.vertex.xyz += saturate(smoothstep(0,0.25,saturate(distance( Follow_Pos,WorldPos))-0.2)) *n/(2+d);

				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

				float Noise1 = snoise(v.vertex*0.55+_Time.y*(0.35,-0.25)*2.5);
				float Noise2 = snoise(v.vertex*0.75+_Time.y*(-0.15,0.45)*1.5);

				float noise = (Noise1*0.35+Noise2*0.25);

				o.vertex = UnityObjectToClipPos(v.vertex /1.15 + v.normal * noise*noise);
				
				return o;
			}
			
            half _Gloss;
			
			half3 _Vector;

            sampler2D _BackTex123;

			sampler2D _RenderTex;

			half4 frag (v2f i) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(i);


				half CameraDistance950_1500 =  saturate(smoothstep(950,1500,i.CameraDistance));

				_ShadowColor = lerp(_ShadowColor,1,CameraDistance950_1500);
				_Gloss = lerp(_Gloss,1000,saturate(smoothstep(700,900,i.CameraDistance)));

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);

				
				half NdotV = dot(worldNormal,worldViewDir);
				//Rim
				half Rim = saturate(1-smoothstep(0,1,NdotV));

				half DarkPart = lerp((1-i.uv.y),0,CameraDistance950_1500)/2;

				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

                half Specular =  pow(max(0,dot(viewDir,reflectDir)),_Gloss);
                half Specular2 = pow(max(0,NdotV),_Gloss*100);
                half Specular3 = pow(max(0,dot(worldNormal,worldLightDir-0.35)/1.157 ),_Gloss*10);

				Specular = Specular+Specular2+Specular3;

				Specular = smoothstep(0,0.001,Specular) ;
				
				Specular = lerp(Specular,0,CameraDistance950_1500);

				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb,DarkPart) ,1-saturate(Specular));
				
				FinalColor =lerp(FinalColor,FinalColor/1.75,smoothstep(0,1.5,1-dot(viewDir,reflectDir)));
				
				

				//內容底背景

				half2 scruv = i.scrPos.xy/i.scrPos.w;

				half f = (snoise(i.worldPos.xy*0.2*float2(0.5,1)+sin(_Time.y*3)*float2(0.5,-0.2))/2 + snoise(i.worldPos.xy*0.1*float2(0.5,0.25)-sin(_Time.y*2.5)*float2(0.8,0.52))/2)*(1-Rim)*(1-Rim)*(1-Rim)*(1-Rim)*(1-Rim)*(1-Rim)*smoothstep(0,0.45,Rim);

				f = smoothstep(0.0045,0.0075,saturate(smoothstep(50,700,i.CameraDistance)*f/2)*smoothstep(0.3,1,1-i.uv.y));

				f = lerp(f,0,CameraDistance950_1500);
				Rim = lerp(Rim,0,CameraDistance950_1500);
				
				
                half Rimscruv = 1-saturate(smoothstep(0,0.5,NdotV));
                half Rimscruv2 = saturate(smoothstep(0.25,1,NdotV));

				Rimscruv+=Rimscruv2;
				half4 refrCol = tex2D(_RenderTex, scruv + Rimscruv/50) ;

				//refrCol = lerp(refrCol,refrCol*_ShadowColor,smoothstep(-0.25,0.35,Rim*i.uv.y));

				refrCol = lerp(refrCol,refrCol*_ShadowColor,1-smoothstep(-0.25,1,Rim*i.uv.y));

				FinalColor = lerp(FinalColor,refrCol,1-FinalColor.a);
				
				FinalColor += Rim*smoothstep(0.25,1.15,1-i.uv.y)*_RimColor*1.5 + saturate(f)*_SpecularColor/5;
				

				return FinalColor+_Color*smoothstep(-0.5,1.15,i.uv.y);
			}
			ENDCG
		}
	}

	
	SubShader
	{

		LOD 100

		//ZTest Always
		ZWrite off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{	
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1"}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

			struct appdata
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
				half4 scrPos : TEXCOORD3;
				half4 CameraDistance : TEXCOORD4;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

			half4 _RimColor;
			half4 _ShadowColor;
			half4 _SpecularColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
				
				o.worldNormal = UnityObjectToWorldNormal(v.normal); 
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.scrPos = ComputeGrabScreenPos(o.vertex);  //抓取螢幕截圖的位置
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
				half2 scruv = o.scrPos.xy/o.scrPos.w;

				half4 Noise1 = tex2Dlod(_NoiseTex, half4( scruv*_NoiseTex_ST.xy +_Time.y*(0.25,-0.25)*1.5 ,0,0));
				half4 Noise2 = tex2Dlod(_NoiseTex, half4( scruv*_NoiseTex_ST.xy +_Time.y*(-0.25,0.25)*1.5 ,0,0));

				half4 Noise3 = tex2Dlod(_NoiseTex, half4( 0.35*v.vertex*_NoiseTex_ST.xy +_Time.y*(-0.3,-0.3)*1.5 ,0,0));
				half uvdis = (smoothstep(0,1,v.uv.y)+smoothstep(0,1,1-v.uv.y))/2;

				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * pow((Noise1+Noise2+Noise3)*uvdis/1.25,2) );
				
				return o;
			}
			
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

            half _Gloss;
			
			half3 _Vector;

			half4 frag (v2f i) : SV_Target
			{
                UNITY_SETUP_INSTANCE_ID(i);

				half CameraDistance950_1500 =  saturate(smoothstep(950,1500,i.CameraDistance));

				_ShadowColor = lerp(_ShadowColor,1,saturate(smoothstep(850,1200,i.CameraDistance)));
				_Gloss = lerp(_Gloss,1000,saturate(smoothstep(700,900,i.CameraDistance)));

				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				//閃動高光
				_Gloss = _Gloss+30*Time_y;

                half3 worldLightDir = half3(_Vector.x +0.005*Time_y,_Vector.y+0.002*Time_y,_Vector.z+0.0025*Time_y);

				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);


				//Rim
				half Rim = saturate(1-smoothstep(0,1,dot(worldNormal,worldViewDir)));

				half DarkPart = smoothstep(-1,0.3,(1-i.uv.y));
				DarkPart = lerp(DarkPart,0,CameraDistance950_1500)/2.5;

				//高光
                half3 reflectDir = reflect(-worldLightDir,worldNormal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

                half Specular =  pow(max(0,dot(viewDir,reflectDir)),_Gloss);
                half Specular2 = pow(max(0,dot(worldNormal,worldLightDir) ),_Gloss*100);
                half Specular3 = pow(max(0,dot(worldNormal,worldLightDir-0.35)/1.157 ),_Gloss*10);

				Specular = Specular+Specular2+Specular3;

				Specular = smoothstep(0,0.001,Specular) ;
				
				Specular = lerp(Specular,0,CameraDistance950_1500);

				half4 FinalColor =lerp( _SpecularColor, half4(_ShadowColor.rgb*7.5,DarkPart)/2 ,1-saturate(Specular));
				
				
				
				half f = (snoise(i.worldPos.xy*0.2*float2(0.5,1)+sin(_Time.y*3)*float2(0.5,-0.2))/2 + snoise(i.worldPos.xy*0.1*float2(0.5,0.25)-sin(_Time.y*2.5)*float2(0.8,0.52))/2)*(1-Rim)*(1-Rim)*(1-Rim)*(1-Rim)*(1-Rim)*(1-Rim)*smoothstep(0,0.45,Rim);

				f = smoothstep(0.0045,0.0075,saturate(smoothstep(50,700,i.CameraDistance)*f/2)*smoothstep(0.3,1,1-i.uv.y));

				
				f = lerp(f,0,CameraDistance950_1500);
				Rim = lerp(Rim,0,CameraDistance950_1500);
				
				FinalColor = lerp(FinalColor,0,1-FinalColor.a);

				//FinalColor += Rim*smoothstep(0,1,1-i.uv.y)*_RimColor + f*_RimColor/2;

				FinalColor += Rim*smoothstep(0.25,1.15,1-i.uv.y)*_RimColor*_RimColor*1.75 + saturate(f)*_SpecularColor/2;

				return FinalColor;
			}
			ENDCG
		}
	}
}
