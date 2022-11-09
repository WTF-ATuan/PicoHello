Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_ST;


            float3 SDF_Pos;

            v2f vert (appdata v)
            {
                v2f o;

                half3 WorldPos = unity_ObjectToWorld._m03_m13_m23;
                
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal  = UnityObjectToWorldNormal(v.normal);


                half d = 1-smoothstep(0,1,distance(o.worldPos,SDF_Pos)*3-1);

                half3 n = (normalize(SDF_Pos-WorldPos)/length(SDF_Pos-WorldPos))*smoothstep(0.15,0.5,dot((v.normal),(SDF_Pos-WorldPos))*d) *0.5;

                half Noise =WaterTex(v.vertex.xy,25,2.5) + WaterTex(v.vertex.xy,15,-5); 

                half Noise2 =WaterTex(v.vertex.xy,5,2.5) + WaterTex(v.vertex.xy,5,-5); 
                
                half Noise_always =WaterTex(v.vertex.xy,3,0.75) + WaterTex(v.vertex.xy,1,-1.5); 

                o.vertex = UnityObjectToClipPos(v.vertex + d*n*(Noise-1) +(Noise2-1)*smoothstep(0,3,1/distance(o.worldPos,SDF_Pos)-3.5)*v.normal*0.25 + (Noise_always-1.5)*v.normal*0.25*(1-d/2));

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                return col;
            }
            ENDCG
        }
    }
}
