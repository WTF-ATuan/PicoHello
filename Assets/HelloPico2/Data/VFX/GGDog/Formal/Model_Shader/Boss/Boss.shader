Shader "GGDog/Boss"
{
    Properties
    {
		_Layer("Mask Layer",Range(0,30)) = 3
        _NoiseTiling ("Noise Density", Float) = 1
        _NoiseStrength ("Noise Strength", Range(0,5)) = 1.5
        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
    }
    SubShader
    {
        Stencil {
            Ref [_Layer]
            Comp always
            Pass replace
        }

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
                half3 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(half2 uv,half Tilling,half FlowSpeed)
            {
                uv.xy*=Tilling;
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

                //D = 1-max(max(D,D2),D3);
                D = smoothstep(-3.5,3.5,D+D2+D3);
                
                return D;
            }
            
            half _NoiseTiling;
            half _NoiseStrength;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);
                
                half Noise =WaterTex(v.vertex.xy*_NoiseTiling+_Time.y*float2(0,0.125),50,0.5) + WaterTex(v.vertex.xy*_NoiseTiling+_Time.y*float2(0,0.125),30,-1); 

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*(Noise*0.75-1)*0.001*_NoiseStrength);

                o.uv.xy = v.uv;
				
				half3 worldNormal = mul(v.normal,(half3x3)unity_WorldToObject);
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                half3 WorldNormal = normalize(worldNormal);
                half3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos );

				o.uv.z = smoothstep(0.65,0.75,1-saturate(dot(WorldNormal,ViewDir)));


                return o;
            }
			half4 _Color;
			half4 _ShadowColor;

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

				half4 FinalColor = lerp(_ShadowColor +(1-WaterTex(i.vertex.xy/50,1,1))*_Color*0.75,_Color ,i.uv.z );

                return FinalColor;
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
			
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }
            float4 frag (v2f i) : SV_Target
            {
                return 0.25 ;
            }
            ENDCG
        }
    }
}
