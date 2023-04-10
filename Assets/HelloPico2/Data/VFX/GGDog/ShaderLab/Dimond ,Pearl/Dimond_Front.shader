Shader "Unlit/Electroplated"
{
    Properties
    {
        _LightDir("Light Dir",Vector) = (-1.5,1.5,-2,0)
        _DiffuseCol("Diffuse",COLOR) = (1,1,1,1)
        _ShadowCol("Shadow",COLOR) = (0.15,0.15,0.15,1)
	    _SpecularNoise("Specular Noise" , Range(0,5)) =1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Blend SrcAlpha One
        ZWrite Off
        Pass
        {
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
		        half4 NormalColor : TEXCOORD3;
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

		    half4 _DiffuseCol;
		    half4 _ShadowCol;
		    half _SpecularNoise;
            
			half3 _LightDir;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
		        o.NormalColor = saturate(half4(normalize(mul(UNITY_MATRIX_MV,v.normal)).xyz,normalize(mul(UNITY_MATRIX_MV,v.normal)).x));
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {

                half4 n = saturate(1.25-WaterTex(i.uv,1000000,0)*_SpecularNoise);

		        half3 worldNormal = normalize(i.worldNormal);
		        half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

		        half3 halfDir = normalize( _LightDir + worldViewDir ); 

		        half NdotH = max(0 , dot(halfDir , worldNormal));	
		        half NdotL = saturate(dot(worldNormal , _LightDir)); 
		        half NdotV = dot(worldNormal , worldViewDir); 
				

		        half4 rim = i.NormalColor*saturate(smoothstep(0.45,0.5,1-NdotV))*0.2;

		        half4 specular = i.NormalColor*NdotH*(1.3-n)  ;
                
		        half4 rim2 = _ShadowCol*saturate(smoothstep(0.9,1,NdotV));

		        half4 FinalColor = lerp(_ShadowCol,_DiffuseCol,NdotL) + rim +rim*NdotL*5 + specular*0.05;

		        return  FinalColor+rim2;
            }
         ENDCG
        }
    }
}