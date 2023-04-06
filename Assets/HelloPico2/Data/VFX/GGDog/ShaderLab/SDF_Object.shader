Shader "Unlit/SDF_Object"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.25, 0.25, 0.25, 1)
        
		_Sharpness ("Color Sharpness", Range(0,1)) = 0

		_WorldLightDir ("Light Direction", Vector) = (0.5, 1, 0.5,0)
        
        [KeywordEnum(Object, Vertex)] _SDF ("SDF OffSet Mode", Float) = 0.0
        [KeywordEnum(Object, Vertex)] _DN_UV ("Dynamic Noise UV Mode", Float) = 0.0

		_Radius_max("Radius_max",Float)=9
		_Radius_min("Radius_min",Float)=10

		_Amplitude("Amplitude",Float)=2

        
		_DNoise_Tiling("Dynamic Noise_Tiling",Float)=10
		_DNoise_Speed("Dynamic Noise_Speed",Float)=1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #pragma shader_feature _SDF_OBJECT _SDF_VERTEX
            #pragma shader_feature _DN_UV_OBJECT _DN_UV_VERTEX

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
            };
            
			half4 GLOBAL_Pos; //接收Script的中心位置
            
            CBUFFER_START(UnityPerMaterial)

			half _Radius_max;
			half _Radius_min;
			half _Amplitude;
            
			half _DNoise_Tiling;
			half _DNoise_Speed;

			half3 _WorldLightDir;
            half4 _MainColor;
            half4 _ShadowColor;
            half _Sharpness;

            CBUFFER_END
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return half2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1.0,-0.25));
                half UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D = smoothstep(-10.4,4.2,1.0-38.7*UV_Center-1.0);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D2 = smoothstep(-18.4,4.2,1.0-38.7*UV_Center-1.0);
                
                D = max(D,D2);
                
                return D;
            }

            v2f vert (appdata v)
            {
                v2f o;
                
                #if _DN_UV_OBJECT
                       //物件的世界中心
				    half3 DN_WorldUV = unity_ObjectToWorld._m03_m13_m23;

                #elif _DN_UV_VERTEX

                    half3 DN_WorldUV = mul(unity_ObjectToWorld, v.vertex).xyz;
                #endif
				

                
                #if _SDF_OBJECT
                       //物件的世界中心
				     half3 WorldPos = unity_ObjectToWorld._m03_m13_m23;

                #elif _SDF_VERTEX

                     half3 WorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                #endif



                half SDF_Radius =  1-smoothstep(_Radius_min,_Radius_max,length(WorldPos-GLOBAL_Pos.xyz));

                half3 SDF_Direct =  normalize(WorldPos-GLOBAL_Pos.xyz);

                half3 SDF = normalize(SDF_Direct)*_Amplitude*SDF_Radius * WaterTex(DN_WorldUV.xz,_DNoise_Tiling,_DNoise_Speed);

                o.vertex = UnityObjectToClipPos(v.vertex + SDF );

                o.uv = v.uv;
                
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {

				half3 worldNormal = normalize(i.worldNormal);
                half Shadow = smoothstep(_Sharpness/2,1-_Sharpness/2,dot(_WorldLightDir,worldNormal));
                
                half4 col = lerp(_ShadowColor,_MainColor,Shadow);

                return col;
            }
            ENDCG
        }
    }
}
