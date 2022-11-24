Shader "Unlit/Unlit Specular Shader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _DiffuseCol("Diffuse",COLOR) = (1,1,1,1)
        _ShadowCol("Shadow",COLOR) = (0.15,0.15,0.15,1)
        _RRim("RRim",Range(0,20)) = 8
	    _SpecularCol("Specular" , COLOR) = (1,1,1,1)
	    _SpecularSharpness("Specular Sharpness",Range(0,20)) = 20
	    _SpecularScale("Specular Scale",Range(280,300)) =300
	    _SpecularNoise("Specular Noise" , Range(0,5)) =1
	    _Rim("Rim",Range(1,3)) = 2
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
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
		        float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
		        float3 worldNormal : TEXCOORD1;
		        float3 worldPos : TEXCOORD2;
		        float3 Normal : TEXCOORD3;
            };
            
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

            
            sampler2D _MainTex;
            float4 _MainTex_ST;

		    fixed4 _DiffuseCol;
		    fixed4 _ShadowCol;
		    fixed4 _SpecularCol;
		    fixed _SpecularSharpness;
		    fixed _SpecularScale;
		    fixed _SpecularNoise;

		    fixed _Rim;
		    fixed _RRim;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
		        o.Normal = v.normal;
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv) ;
                fixed4 n = saturate(1.25-WaterTex(i.uv,1000000,0)*_SpecularNoise);

		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

		        float3 halfDir = normalize( worldLightDir + worldViewDir ); 

		        float NdotH = max(0 , dot(halfDir , worldNormal));	
		        float NdotL = dot(worldNormal , worldLightDir); 
		        float NdotV = dot(worldNormal , worldViewDir); 
				
		        fixed3 diffuse = saturate(NdotL) ;
		        //fixed3 rim = saturate(pow(1-NdotV,_max)) * _DiffuseCol.rgb/3;
		        //fixed3 specular = lerp(0,i.Normal+0.25,pow(NdotH,_Gloss*_min*(1.3-tex2D(_MainTex, i.uv))) ) ;
                
		        fixed3 rim = saturate(i.Normal+0.35)*saturate(pow(1-NdotV,_Rim)) /5;

		        fixed3 specular = saturate(i.Normal+0.35)*pow(NdotH,_SpecularSharpness*(301-_SpecularScale)*(1.3-col.r*n))  ;
                

		        fixed3 rim2 = _ShadowCol*saturate(pow(NdotV,_RRim));

		        fixed3 FinalColor = lerp(_ShadowCol,col*_DiffuseCol.rgb,diffuse) + rim +rim*saturate(NdotL)*5 + specular*min(2,_SpecularSharpness/10);

		        return fixed4(FinalColor+rim2,1);
            }
         ENDCG
        }
    }
}