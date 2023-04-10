Shader "Unlit/Matte"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseCol("Noise Color",COLOR) = (1,1,1,1)
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                return o;
            }
            fixed4 _NoiseCol;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 n =_NoiseCol*saturate(1.25-WaterTex(i.uv,1000000,0));

		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);
		        fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);
		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
		        float NdotV = dot(worldNormal , worldViewDir); 
		        fixed rim = saturate(pow(1-NdotV,1.5)) *2;
                
		        float NdotL = dot(worldNormal , worldLightDir); 
		        fixed diffuse = saturate(NdotL) ;

                return col + rim*n*diffuse + n*diffuse/7;
            }
            ENDCG
        }
    }
}
