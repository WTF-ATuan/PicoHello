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
		LOD 100 
        Pass
        {
			Tags { "LightMode"="ForwardBase" "RenderType" = "Opaque" "Queue" = "Geometry"}
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
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
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
                o.uv = v.uv;

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

#if _REFAXIS_WORLD
                float3 YPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float n = frac_Noise(YPos.xy,1);
                float worldPosY = step(YPos.y,-30);
                float worldPosY_smoothstep = saturate(smoothstep(0,50,1-YPos.y));
#elif _REFAXIS_LOCAL
				float3 YPos = v.vertex.xyz;
                float n = frac_Noise(1000*YPos.xy,1);
                float worldPosY = step(YPos.y,0);
                float worldPosY_smoothstep = 1;
#endif
                o.vertex = UnityObjectToClipPos(v.vertex + _ModelReflection * 0.1*float3(-sign(YPos.x),0,0.5)*n*worldPosY*worldPosY_smoothstep);

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                o.normal = v.normal;

                return o;
            }
            float4 _Color;
            float4 _ShadowColor;

            float4 _SkyColor;
			
            float4 _FarColor;
            float4 _BackFogColor;
            float4 _FogColor;

            float _Rim;
			
            float _FogPos;
            
            float _UV_Tilling;
            float _UV_Offset;
            float _Reflect;
            float _ReflectRGBOffSet;
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float3 normalDir = (i.worldNormal);

                float3 lightDir = (_WorldSpaceLightPos0.xyz);

                float3 viewDir = (_WorldSpaceCameraPos.xyz - i.worldPos );
				
                float3 ShadowColor = _ShadowColor;
                float3 LightColor  = _LightColor0* _Color ;

                float3 FinalColor =  lerp( ShadowColor, LightColor , (max(dot(normalDir,lightDir),0)+0.25) *smoothstep(-300,500,i.worldPos.z)   );


				//Ãä½t¥ú
				float Rim = 1-smoothstep(0,0.75,dot(normalDir,viewDir));
				Rim =  ( Rim + smoothstep(0.5,1,Rim) )/_Rim;

				Rim*=smoothstep(300,700,i.CameraDistance);
				
                float4 col = float4(FinalColor,1);

				col += Rim*_SkyColor;

				col = lerp(col,_FarColor,smoothstep(0,1000,i.CameraDistance));
				
				col = lerp(col,_SkyColor,smoothstep(850,1000,i.worldPos.z));
				
				col = lerp(col,_FogColor,(1-smoothstep(-100,300,i.worldPos.y+_FogPos)) *smoothstep(0,1000,i.CameraDistance)  );
				
				col = lerp(col,_BackFogColor,1-smoothstep(-700,1000,i.worldPos.z));


        #if _REF_ON

             float n = frac_Noise(i.worldPos.xy+_ReflectRGBOffSet,0.5);
             float n2 = frac_Noise(i.worldPos.xy,0.5);
             float n3 = frac_Noise(i.worldPos.xy-_ReflectRGBOffSet,0.5);

             float4 Reflect = float4(n,n2,n3,1)*(smoothstep(0,2,(0.5-i.normal.y)));

             Reflect *= (1-smoothstep(0,1500,i.worldPos.z)) * (smoothstep(-150,1000,i.worldPos.z)) ;

                return col +Reflect*_FogColor*_Reflect;


        #elif _REF_OFF

                return col;

        #endif

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
