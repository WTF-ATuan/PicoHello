Shader "Unlit/cell_liquid"
{
    Properties
    {
		_Vector ("Light Direction", Vector) = (0, 0, 0)

        _Color("Color",COLOR) = (1,1,1,1)
        _Color2("Color2",COLOR) = (1,1,1,1)
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
            
			half3 _Vector;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		        o.worldNormal = mul(v.normal, unity_WorldToObject);
		        o.worldPos = mul(unity_ObjectToWorld , v.vertex);
                
                _Vector = normalize(_Vector);
                

                    float4 normal_OS = float4(_Vector.xyz, 0);
                    _Vector.xyz = mul(UNITY_MATRIX_MV, normal_OS);
                

                return o;
            }
            fixed4 _Color;
            fixed4 _Color2;
            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
                
		        fixed3 worldPos = (i.worldPos);
		        float3 worldNormal = normalize(i.worldNormal);

		        fixed3 worldLightDir = _Vector;

		        fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                
		        float3 halfDir1 = normalize( worldLightDir + worldViewDir-half3(1.5,0.5,1)); 
		        float NdotL = max(0 , dot(halfDir1 , worldNormal));	

		        float NdotL2 = dot(worldNormal, worldLightDir - half3(0.25,0,-0.05)); 

		        fixed Back = 1-step(NdotL,0.9)*1.5 + (1-worldPos.y)*0.15;
                
		        float3 halfDir = normalize( worldLightDir + worldViewDir+0.5); 
		        float NdotH = max(0 , dot(halfDir , worldNormal));	
		        fixed s = step(NdotH,0)*0.5 ;
                
		        fixed diffuse2 = 1-step(NdotL2,0.75);

		        fixed diffuse3 = (1-smoothstep(0.65,0.9,NdotL2));
		        
		        fixed specular = 1-step(NdotH,0.995)  ;
                
              //  diffuse2 -= 1-smoothstep(0.5,1,NdotL2);
              
              fixed f = lerp(-Back*0.25-s+0.25,0.55 + (worldPos.y)*0.025,diffuse2)+diffuse3*(1.05-diffuse3)*0.35;

                return f*_Color +specular*_Color2;

               // return saturate((diffuse2*0.35)-Back*0.25-s+diffuse3*0.1+0.25 )*_Color +specular*_Color2;
            }
            ENDCG
        }
    }
}
