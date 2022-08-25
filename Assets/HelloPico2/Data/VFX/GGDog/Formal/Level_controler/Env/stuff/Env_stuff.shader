Shader "GGDog/Space_Test/Env_stuff"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
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
        _Reflect("Reflection",Range(0,1.5)) = 1.5
    }
    SubShader
    {
		LOD 100 
        Pass
        {
			Tags { "LightMode"="ForwardBase"  "Queue" = "Transparent"}

            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #include "Lighting.cginc"
            #pragma vertex vert
            #pragma fragment frag
			
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 CameraDistance : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
			inline half2 unity_voronoi_noise_randomVector (half2 UV, half offset)
			{
			    half2x2 m = half2x2(15.27, 47.63, 99.41, 89.98);
			    UV = frac(sin(mul(UV, m)) * 46839.32);
				return half2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
			}
			void Unity_Voronoi_half(half2 UV, half AngleOffset, half CellDensity, out half Out, out half Cells)
			{
				half2 g = floor(UV * CellDensity);
				half2 f = frac(UV * CellDensity);
				half t = 8.0;
				half3 res = half3(8.0, 0.0, 0.0);

				for(int y=-1; y<=1; y++)
				{
					for(int x=-1; x<=1; x++)
					{
						half2 lattice = half2(x,y);
						half2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
						half d = distance(lattice + offset, f);
						if(d < res.x)
						{
							res = half3(d, offset.x, offset.y);
							Out = res.x;
							Cells = res.y;
						}
					}
				}
			}
            float _ModelReflection;
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

			    half Out;
			    half Cells;
                Unity_Voronoi_half(o.worldPos.xy+o.worldPos.xz+o.worldPos.yz,_Time.y*0.01,0.007,Out,Cells);
                
                half worldPosY = step(o.worldPos.y,-30);

                o.vertex = UnityObjectToClipPos(v.vertex + _ModelReflection * 0.1*half3(-1,0,-1)*Out*worldPosY);

				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                o.normal = v.normal;

                return o;
            }
            sampler2D _MainTex;
            float4 _Color;
            float4 _ShadowColor;

            float4 _SkyColor;
			
            float4 _FarColor;
            float4 _BackFogColor;
            float4 _FogColor;

            float _Rim;
			
            float _FogPos;
            
            half _UV_Tilling;
            half _UV_Offset;
            half _Reflect;
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 normalDir = normalize(i.worldNormal);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );
				
                float3 ShadowColor = _ShadowColor;
                float3 LightColor  = _LightColor0* _Color ;
                
                float4 MainTex = tex2D(_MainTex,i.uv +_Time.y*float2(0.1,0));
                float3 FinalColor =  lerp( ShadowColor*MainTex.rgb, MainTex.rgb , (max(dot(normalDir,lightDir),0)+0.25) *smoothstep(-300,500,i.worldPos.z)   );

                //FinalColor = MainTex.rgb;

				//Ãä½t¥ú
				float Rim = 1-saturate(smoothstep(0,0.75,dot(normalDir,viewDir)));
				Rim =  ( Rim + smoothstep(0.5,1,Rim) )/_Rim;

				Rim*=smoothstep(300,700,i.CameraDistance);
				

                float4 col = float4(FinalColor,1);

				col += Rim*_SkyColor;

				col = lerp(col,_FarColor,saturate(smoothstep(0,1000,i.CameraDistance)));
				

				col = lerp(col,_SkyColor,saturate(smoothstep(850,1000,i.worldPos.z)));
				

				col = lerp(col,_FogColor,(1-saturate(smoothstep(-100,300,i.worldPos.y+_FogPos))) *smoothstep(0,1000,i.CameraDistance)  );
				

				col = lerp(col,_BackFogColor,1-saturate(smoothstep(-700,1000,i.worldPos.z)));
				
			 half Out0;
			 half Cells0;

			 Unity_Voronoi_half(i.uv,0,10,Out0,Cells0);

             i.uv = i.normal.xy * i.normal.yz * i.normal.xz;

			 half Out;
			 half Cells;

			 Unity_Voronoi_half(i.uv,_UV_Offset,_UV_Tilling,Out,Cells);

             Out = smoothstep(0.5,0.75,1-Out);

             Out *= Out0;
             

             
			 half Out2;
			 half Cells2;
			 Unity_Voronoi_half(i.worldPos.xy+i.worldPos.yz-_Time.y*0.25,-_Time.y*1,0.005,Out2,Cells2);
             
			 half Out3;
			 half Cells3;
			 Unity_Voronoi_half(i.worldPos.xy+i.worldPos.yz+_Time.y*0.15,-_Time.y*1,0.0075,Out3,Cells3);

             half Reflect = smoothstep(1,1.75,(Out2+Out3) )*(smoothstep(0,2,(0.5-i.normal.y)));

             Reflect *= (1-saturate(smoothstep(0,1500,i.worldPos.z))) * (saturate(smoothstep(-150,1000,i.worldPos.z))) ;
             

             clip(MainTex.a-0.03);

                return col +Reflect*_FogColor*_Reflect;
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float CameraDistance : TEXCOORD1;
				float3 worldPos : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
                return o;
            }
            float4 _LOD_LowColor;
            float4 _SkyColor;
			
            float4 _BackFogColor;
            float4 _FogColor;

            float _FogPos;

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float4 col = _LOD_LowColor;

				col = lerp(col,_SkyColor,saturate(smoothstep(850,1000,i.worldPos.z)));
				

				col = lerp(col,_FogColor,(1-saturate(smoothstep(-100,300,i.worldPos.y+_FogPos))) *smoothstep(0,1000,i.CameraDistance)  );
				

				col = lerp(col,_BackFogColor,1-saturate(smoothstep(-700,1000,i.worldPos.z)));
				
                return col ;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
