
Shader "MyShader/Water_Plane" 
{
    Properties 
    {
        [HDR]_Color ("Color Tint", Color) = (1, 1, 1, 1)
        _ShadowColor ("ShadowColor", Color) = (1, 1, 1, 1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _NormalMap ("NormalMap", 2D) = "white" {}

        _NormalMapUVTiling ("NormalMapUVTiling", Range(1, 10)) = 1
        //高光反射顏色
        _Specular ("高光反射顏色", Color) = (1, 1, 1, 1)

        //高光反射強度
        _SpecularScale ("高光反射強度", Range(0, 0.1)) = 0.01
        //高光漸層度
        _SpecularGradient ("高光漸層度", Range(0, 100)) = 0.01

    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		
        Pass 
        {
            Tags { "LightMode"="ForwardBase" }
 
            Cull Back
 
            CGPROGRAM
 
            #pragma vertex vert
            #pragma fragment frag
 
            #pragma multi_compile_fwdbase
 
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityShaderVariables.cginc"
 
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			
            sampler2D _NormalMap;
            float4 _NormalMap_ST;

            sampler2D _Ramp;
            float4 _Specular;
            float _SpecularScale;
            float _SpecularGradient;
			
            struct a2v 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 tangent : TANGENT;
            }; 
 
            struct v2f 
            {
                float4 pos : POSITION;
                float4 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 lightDir : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
            };
 
            v2f vert (a2v v) 
            {
                v2f o;
 
                o.pos = UnityObjectToClipPos( v.vertex);

                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                o.uv.zw = v.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
				
				TANGENT_SPACE_ROTATION;
				o.lightDir = mul(rotation,ObjSpaceLightDir(v.vertex)).xyz;
				o.viewDir = mul(rotation,ObjSpaceViewDir(v.vertex)).xyz;
				
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
 
                return o;
            }
            fixed  _NormalMapUVTiling;
            fixed4 _ShadowColor;
 
			uniform	half4 GLOBAL_Pos;
			uniform	half4 GLOBAL_SpotColor;
			uniform	half GLOBAL_SpotRadius;
            float4 frag(v2f i) : SV_Target 
            { 
				
                float3 worldNormal = normalize(i.worldNormal);
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));

				float3 tangent_LightDir = normalize(i.lightDir);
				float3 tangent_ViewDir = normalize(i.viewDir);
                float3 tangent_HalfDir = normalize(tangent_LightDir*1.1 + tangent_ViewDir)/1.01;
                //fixed3 tangent_HalfDir = normalize(tangent_LightDir*1.1 + (tangent_ViewDir+0.15)/1.25);
				
				_NormalMap_ST.xy*=_NormalMapUVTiling;

				float4 packedNormal = tex2D(_NormalMap,_NormalMap_ST.xy*i.worldPos.xz/50 +float2(0.05,0.1)*_Time.y );
				float4 packedNormal2 = tex2D(_NormalMap,_NormalMap_ST.xy*i.worldPos.xz/50 +float2(0,0.1)*_Time.y );

				packedNormal = packedNormal/2+packedNormal2/2;

				float3 tangent_Normal = UnpackNormal(packedNormal);
				tangent_Normal.xy *= 0.5;
				tangent_Normal.z = sqrt(1-saturate(dot(tangent_Normal.xy,tangent_Normal.xy)));

				
				//漫反射光照
                fixed4 col = tex2D (_MainTex, i.uv.xy + packedNormal/5 - 0.05);
				//亮色
                fixed3 diffuse = _LightColor0.rgb * col * _Color.rgb;
				//陰影色
                fixed3 ambient = _LightColor0.rgb * col *_ShadowColor;
				//亮暗Lerp值
				 fixed Color_lerp = smoothstep(0.25,1,1-dot(worldNormal,worldViewDir))*(smoothstep(0,0.5,dot(tangent_Normal,tangent_LightDir)));
				 


				//高光
                float spec = dot(tangent_Normal,tangent_HalfDir);

                //抗鋸齒處理
                float w = fwidth(spec) * _SpecularGradient;
                float3 specular = lerp(0,_LightColor0.rgb* _Specular.rgb, smoothstep(-w, w, spec +_SpecularScale - 1)) ;
				


				//玩家底光
				half d =distance( GLOBAL_Pos,i.worldPos);
				half D = abs(d+GLOBAL_SpotRadius)/GLOBAL_SpotRadius;

				 half SpotLight = saturate(dot( worldNormal, normalize( GLOBAL_Pos - i.worldPos ) )/D + 1/D );
				 half SpotLight2 = dot( tangent_Normal, normalize( tangent_ViewDir) )*(SpotLight+0.45)*3.5;
				 
				 fixed3 spot = SpotLight*GLOBAL_SpotColor + pow(SpotLight2,2.5)*GLOBAL_SpotColor.rgb;
				 



				return float4( lerp(ambient,diffuse,Color_lerp) + specular + spot, 1);
            }
 
            ENDCG
        }
    }
    FallBack "Diffuse"
}
 

