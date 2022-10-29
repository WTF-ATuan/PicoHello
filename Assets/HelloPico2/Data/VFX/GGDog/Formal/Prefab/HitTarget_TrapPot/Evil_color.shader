// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Evil_color"
{
    Properties
    {
		[HDR]_Color("Color",Color) = (1,1,1,1)
        _Alpha ("Alpha", Range(0,1)) = 1

        _MainTex ("Texture     ( Offset Sets UVScroll Speed )", 2D) = "white" {} //JK Edited
		_FarColor("Far Color",Color) = (1,1,1,1)
		_FarDistance("Far Distance",Float) = 3

		[HDR]_LightColor("Light Color",Color) = (1,1,1,1)
		[HDR]_ShadowColor("Shadow Color",Color) = (0.5,0.5,0.5,1)
        
        _ReflectTilling ("Reflect Tilling", Range(0,5000)) = 100
        _Reflect ("Reflect", Range(0,1)) = 1
		[HDR]_ReflectColor("Reflect Color",Color) = (1,1,1,1)

        _AlphaClip ("AlphaClip", Range(0,1)) = 0.001
    }
    SubShader
    {
		Tags{ "RenderType"="transparent" "Queue"="transparent"}

		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Back
            CGPROGRAM
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
				float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling/50;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,-0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-0.24,0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                D = 1-max(max(D,D2),D3);
                //D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }


            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                o.normal = v.normal;

				o.worldPos.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz); //CameraDistance

                return o;
            }
            float _AlphaClip;
            
            float4 _Color;
            float3 _LightColor;
            float3 _ShadowColor;
            float _Reflect;
            float _ReflectTilling;
            float3 _ReflectColor;
            
		    float4 _FarColor;
		    float _FarDistance;
            
		    float _Alpha;

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                fixed4 col = tex2D(_MainTex, i.uv + _Time.y*_MainTex_ST.zw);

                float3 normalDir = normalize(i.worldNormal);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                col.rgb =  lerp( _ShadowColor * col, _LightColor * col, (max(dot(normalDir,lightDir),0+0.25))) ;

                col.rgb += WaterTex(i.worldPos.xy,_ReflectTilling,1.25)*_Reflect*_ReflectColor;

                clip(col.a-_AlphaClip);
                
                //¶ZÂ÷Ãú
				col = lerp(col,float4(_FarColor.rgb,col.a), smoothstep(-_FarDistance/3,_FarDistance,i.worldPos.z));

                return float4(col.rgb,col.a*_Alpha)*_Color;
            }
            ENDCG
        }
        Pass
        {
            Cull Front
            CGPROGRAM
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
				float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling/50;
                half Time = _Time.y*FlowSpeed;

                uv.xy = Rotate_UV(uv,0.34,0.14);
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,-0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-0.24,0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                D = 1-max(max(D,D2),D3);
                //D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }


            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                o.normal = v.normal;
                
				o.worldPos.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz); //CameraDistance

                return o;
            }
            
            float _AlphaClip;
            
            float4 _Color;
            float3 _LightColor;
            float3 _ShadowColor;
            float _Reflect;
            float _ReflectTilling;
            float3 _ReflectColor;
            
		    float4 _FarColor;
		    float _FarDistance;

		    float _Alpha;

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                float4 col = tex2D(_MainTex, i.uv + _Time.y*_MainTex_ST.zw);

                float3 normalDir = normalize(i.worldNormal);

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                col.rgb =  lerp( _ShadowColor * col, _LightColor * col, (max(dot(normalDir,lightDir),0+0.25))) ;
                
                col.rgb += WaterTex(i.worldPos.xy,_ReflectTilling,1.25)*_Reflect*_ReflectColor;

                clip(col.a-_AlphaClip);
                
                //¶ZÂ÷Ãú
				col = lerp(col,float4(_FarColor.rgb,col.a), smoothstep(0,_FarDistance,i.worldPos.z));
                
                return float4(col.rgb,col.a*_Alpha)*_Color;
            }
            ENDCG
        }
    }
}
