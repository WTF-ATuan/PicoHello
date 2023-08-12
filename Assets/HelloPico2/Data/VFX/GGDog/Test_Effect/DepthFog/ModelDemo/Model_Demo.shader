Shader "Unlit/SophonShader/3DModel_Demo"
{
    Properties
    {
        _Scale ("Scale", Range(0, 1)) = 1

        _MainTex ("Texture", 2D) = "white" {}

        [HDR]_Color ("Main Color", Color) = (1.35, 1.08, 0.97, 1)
        
        [HDR]_ShadowColor ("Shadow Color", Color) = (.53, .53, .6, 1)
        
        [Header(___________ Projection __________________ )]
        [KeywordEnum(ViewNormal, ObjectNormal)] _PROJECTION ("Light ProjectMode", Float) = 0.0

        _LightDir ("Light Direction", Vector) = (0.5, 1, 0.5, 0)
        
        _LightSmooth ("Light Edge Smooth", Range(0, 1)) = 0.5
        _LightRange ("Light Edge Range", Range(-1, 1)) = 0.15
        
        [Header(_____________ Boundary __________________ )]
        _Boundary ("Boundary", Range(0, 2)) = 1
        _BoundaryOffSet ("Boundary OffSet", Range(0, 0.2)) = 0.05
        [HDR]_EdgeRimColor ("Edge Rim Color", Color) = (.67, .36, .2, 1)
        
        
        [Header(__________ Under Gradient _______________ )]
        _UnderGradRange ("UnderGradRange", Range(0, 1)) = 0.3
        _UnderGradOffset ("UnderGradOffset", Range(-1, 1)) = 0
        [HDR]_UnderGradColor ("UnderGradColor", Color) = (.67, .36, .2, 1)
            
        [Header(__________ Dynamic WaterTex ____________ )]
        [HDR]_WaterTexColor ("WaterTexColor", Color) = (.53, .53, .6, 0.5)

        [Header(_____________ Self Shadow _______________ )]
		_SelfShadowSmooth ("_SelfShadowSmooth", Range(0,1)) = 0.25
		_SelfShadowOffSet ("_SelfShadowOffSet", Range(-1,1)) = 0
        
        [Header(_____________ Depth Fog __________________ )]
		_FarFogRange ("_FarFogRange", Range(0,300)) = 10
		_NearDefrost ("NearDefrost", Range(0,1)) = 0.25

        _FogColor ("Fog Color", Color) = (.1, .1, .1, 0)
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
            
            #pragma shader_feature _PROJECTION_VIEWNORMAL _PROJECTION_OBJECTNORMAL

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 normal_VS : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _Color;
            float4 _ShadowColor;
            float3 _LightDir;
            half _LightSmooth;
            half _LightRange;

            float4 _EdgeRimColor;
        
            float _Boundary;
            float _BoundaryOffSet;

            float4 _FogColor;
            
            float _SelfShadowSmooth;
            float _SelfShadowOffSet;

            float _FarFogRange;
            float _NearDefrost;
            
            float _UnderGradRange;
            float _UnderGradOffset;
            float4 _UnderGradColor;
            
            float _Scale;

            float4 _WaterTexColor;
            
            
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
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                
                
                //ViewNormal
                #if _PROJECTION_VIEWNORMAL

                    float4 normal_OS = float4(v.normal.xyz, 0);
                    o.normal_VS.xyz = mul(UNITY_MATRIX_MV, normal_OS);
                    
                    //Normal
                #elif _PROJECTION_OBJECTNORMAL

                    o.normal_VS.xyz = v.normal;
                #endif

                
                o.uv.z = mul(UNITY_MATRIX_MV, float4(v.vertex.xyz, 0)).z;
                
                o.uv.w = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
                
                o.uv.w = 1-saturate(smoothstep(_NearDefrost*_FarFogRange, _FarFogRange, o.uv.w));
                
                half3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.normal_VS.w = 1 - dot(worldNormal, viewDir);
                
                o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                o.worldPos.w = v.vertex.y;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv.xy);
                
                half Rim = smoothstep(0.7, 0.9, i.normal_VS.w);
                
                half n_vs_dot_l = dot(normalize(i.normal_VS.xyz), normalize(_LightDir));
                
                //NDotL
                half N_VS_Dot_L = smoothstep(0.0, _LightSmooth, n_vs_dot_l - _LightRange);
                
                col = lerp(col * _ShadowColor, col * _Color , N_VS_Dot_L) + Rim*N_VS_Dot_L*_EdgeRimColor;
                
                N_VS_Dot_L= saturate(N_VS_Dot_L-_BoundaryOffSet);

                col += ( N_VS_Dot_L - smoothstep(0.05,1.05,saturate(N_VS_Dot_L-0.05)) )*_EdgeRimColor*_Boundary;
                
                col += saturate((1-WaterTex(i.worldPos.yz+i.worldPos.xy,5,-0.5))*n_vs_dot_l*_WaterTexColor*_WaterTexColor.a);
                
                i.worldPos.w = 1 - smoothstep(-_UnderGradRange/_Scale, _UnderGradRange/_Scale, i.worldPos.w + _UnderGradOffset/_Scale);
                col = lerp(col,_UnderGradColor,i.worldPos.w*_UnderGradColor.a);
                
                //col = lerp(col,1,i.worldPos.w);
                
                
                //¦Û³±¼v
                float selfShadow = smoothstep(-_SelfShadowSmooth, _SelfShadowSmooth, i.uv.z + _SelfShadowOffSet);

                float DepthShadow = i.uv.w;

                col = lerp(_FogColor,col,DepthShadow *selfShadow);

                return col;
            }
            ENDCG
        }
    }
}
