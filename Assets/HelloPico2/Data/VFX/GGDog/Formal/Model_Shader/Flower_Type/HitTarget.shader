// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/HitTarget"
{
    Properties
    {
		_Color("Color",Color) = (1,0,0,1)
		_WarningRimColor("Warning Rim Color ",Color) = (0,0,0,1)
		_FarColor("Far Color",Color) = (1,1,1,1)
		_FarDistance("Far Distance",Float) = 3

		_LightColor("Light Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (0,0,0,1)
		_RimColor("Rim Color in Shadow",Color) = (0,0,0,1)
		_RimColor2("Rim Color in Light",Color) = (0,0,0,1)
		_LightPos("LightPos",Vector) = (0.25,0,-1,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldPos.w = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
                return o;
            }

		    float4 _Color;

		    float4 _LightColor;
		    float4 _ShadowColor;
		    float4 _RimColor;
		    float4 _RimColor2;
		    float4 _WarningRimColor;
            
            float3 _LightPos;

		    float4 _FarColor;
		    float _FarDistance;
            
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                float3 normalDir = normalize(i.worldNormal);

                float3 lightDir = _LightPos;
				
				float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				
				float3 worldNormal = normalize(i.worldNormal);
                
				float NdotL = smoothstep(0,1,dot(normalDir,lightDir));

				float Rim = (1-saturate(smoothstep(0,1,dot(worldNormal,worldViewDir))))*smoothstep(0.75,1.5,1-NdotL)*3.5;

				float Rim2 = (1-saturate(smoothstep(0,1.5,dot(worldNormal,worldViewDir))))*smoothstep(0.5,1,NdotL)*1.5;
                
				float Rim3 = (1-saturate(smoothstep(0.25,1,dot(worldNormal,worldViewDir))));
                
                float4 col = lerp(_ShadowColor,_LightColor,NdotL);

                col+=Rim*_RimColor+Rim2*_RimColor2;

                col+=Rim3*_WarningRimColor*_WarningRimColor.a;
                
                col = lerp(col,(_Color+col)*1.5,_Color.a);


                //¶ZÂ÷Ãú
				col = lerp(col,_FarColor, smoothstep(0,_FarDistance,i.worldPos.w));


                return col ;
            }
            ENDCG
        }
    }
}
