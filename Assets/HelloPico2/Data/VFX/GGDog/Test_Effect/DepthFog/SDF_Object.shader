Shader "Unlit/SDF_Object"
{
    Properties
    {
        [HDR]_MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.25, 0.25, 0.25, 1)
		_Sharpness ("Color Sharpness", Range(0,1)) = 0

		_WorldLightDir ("Light Direction", Vector) = (0.5, 1, 0.5,0)
        
        /*
                //Defrost near camera with SDF Range
    [Header(_ _ _ _ _ Near Camera Defrost _ _ _ _ _ )]

		_DefrostRange("Defrost Range",Float)= 5
		_DefrostRange_Intense("DefrostRange Intense",Range(0,1))= 0
        */
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
			uniform half3 GLOBAL_Pos; //接收Script的中心位置
            
			uniform half GLOBAL_HeightFog_pos;
			uniform half GLOBAL_HeightFog_Range;

			uniform half GLOBAL_DefrostRange;
			uniform half GLOBAL_DefrostRange_Intense;
            

			uniform half GLOBAL_FarFog_Range;
			uniform half GLOBAL_FarFog_Intense;
            
			uniform fixed4 GLOBAL_Fog_Color;
            
            
       CBUFFER_START(UnityPerMaterial)
            
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
                
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
                
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                
                UNITY_SETUP_INSTANCE_ID(i);
				
				half3 worldNormal = normalize(i.worldNormal);
                half Shadow = smoothstep(_Sharpness/2,1-_Sharpness/2,dot(_WorldLightDir,worldNormal));
                
                half4 col = lerp(_ShadowColor,_MainColor,Shadow);


                // Defrost near camera with SDF Range
                half SDF_Defrost =  smoothstep(GLOBAL_DefrostRange_Intense*GLOBAL_DefrostRange,GLOBAL_DefrostRange, length(i.worldPos-GLOBAL_Pos.xyz) );

                // Height_Fog
                half Height_Fog = 1-smoothstep(0,GLOBAL_HeightFog_Range,i.worldPos.y-GLOBAL_HeightFog_pos);

                
                // Far Fog
                half Far_Fog =  smoothstep(GLOBAL_FarFog_Intense*GLOBAL_FarFog_Range,GLOBAL_FarFog_Range, length(i.worldPos-GLOBAL_Pos.xyz) );
                
                Height_Fog*= saturate(GLOBAL_Fog_Color.a+Far_Fog);

                Height_Fog = Far_Fog*(1-Height_Fog) + Height_Fog;
                
                half fog = Height_Fog * SDF_Defrost;

                col = lerp(col,GLOBAL_Fog_Color,fog);

                return col;
            }
            ENDCG
        }
    }
}
