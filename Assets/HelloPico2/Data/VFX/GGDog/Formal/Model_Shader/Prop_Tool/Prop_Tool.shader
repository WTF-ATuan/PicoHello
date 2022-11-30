Shader "Unlit/Prop_Tool"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RimColor("RimColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
      //  Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Cull Front
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex*1.01 + v.normal*0.002);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return 1;
            }
            ENDCG
        }

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
            
            float2 Rotate_UV(float2 uv , float sin , float cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            float WaterTex(float2 uv,float Tilling,float FlowSpeed)
            {
                uv.xy*=Tilling;
                float Time = _Time.y*FlowSpeed;



                uv.xy = Rotate_UV(uv,0.34,0.14);
                float2 UV = frac(uv.xy*0.75+Time* float2(-1,-0.25));
                float UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				float D = smoothstep(-10.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* float2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				float D2 = smoothstep(-18.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* float2(0.54,-0.13));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				float D3 = smoothstep(-15.4,4.2,1-38.7*UV_Center-1);

                D = 1-max(max(D,D2),D3);
                
                return D;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                return o;
            }
            
			float4 _RimColor;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

                fixed3 LightDir = normalize(_WorldSpaceLightPos0.xyz);

				float Rim = 1-saturate(smoothstep(0,2,dot(worldNormal,worldViewDir) ));

                Rim += WaterTex( i.worldPos,30,-1);

                return float4(col.rgb+Rim*_RimColor.rgb,col.r+Rim*_RimColor.r);
            }
            ENDCG
        }
    }
}
