// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Evil_color"
{
    Properties
    {
        _Alpha ("Alpha", Range(0,1)) = 1

        _MainTex ("Texture", 2D) = "white" {}
		_FarColor("Far Color",Color) = (1,1,1,1)
		_FarDistance("Far Distance",Float) = 3

		[HDR]_Color("Color",Color) = (1,1,1,1)
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
            
            float3 _Color;
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

                col.rgb =  lerp( _ShadowColor * col, _Color * col, (max(dot(normalDir,lightDir),0+0.25))) ;

                col.rgb += frac_Noise(i.worldPos.xy,_ReflectTilling)*_Reflect*_ReflectColor;

                clip(col.a-_AlphaClip);
                
                //¶ZÂ÷Ãú
				col = lerp(col,float4(_FarColor.rgb,col.a), smoothstep(-_FarDistance/3,_FarDistance,i.worldPos.z));

                return float4(col.rgb,col.a*_Alpha);
            }
            ENDCG
        }
        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


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
            
            float3 _Color;
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

                col.rgb =  lerp( _ShadowColor * col, _Color * col, (max(dot(normalDir,lightDir),0+0.25))) ;
                
                col.rgb += frac_Noise(i.worldPos.xy,_ReflectTilling)*_Reflect*_ReflectColor;

                clip(col.a-_AlphaClip);
                
                //¶ZÂ÷Ãú
				col = lerp(col,float4(_FarColor.rgb,col.a), smoothstep(0,_FarDistance,i.worldPos.z));
                
                return float4(col.rgb,col.a*_Alpha);
            }
            ENDCG
        }
    }
}
