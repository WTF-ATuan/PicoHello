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
			#pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            half3 _LightPos;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);

				half3 worldNormal = normalize(mul(v.normal,(float3x3)unity_WorldToObject));
				half3 WP = mul(unity_ObjectToWorld, v.vertex).xyz;
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - WP);
                
				o.worldPos.x = smoothstep(0,1,dot(worldNormal,_LightPos)); //NDotL
				o.worldPos.y = dot(worldNormal,worldViewDir); //NDotV

				o.worldPos.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

                return o;
            }

		    half4 _Color;

		    half4 _LightColor;
		    half4 _ShadowColor;
		    half4 _RimColor;
		    half4 _RimColor2;
		    half4 _WarningRimColor;
            
		    half4 _FarColor;
		    half _FarDistance;
            
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

				half Rim = (1-saturate(smoothstep(0,1,i.worldPos.y)))*smoothstep(0.75,1.5,1-i.worldPos.x)*3.5;

				half Rim2 = (1-saturate(smoothstep(0,1.5,i.worldPos.y)))*smoothstep(0.5,1,i.worldPos.x)*1.5;
                
				half Rim3 = (1-saturate(smoothstep(0.25,1,i.worldPos.y)));
                
                half4 col = lerp(_ShadowColor,_LightColor,i.worldPos.x);

                col+=Rim*_RimColor+Rim2*_RimColor2;

                col+=Rim3*_WarningRimColor*_WarningRimColor.a;
                
                col = lerp(col,(_Color+col)*1.5,_Color.a);


                //¶ZÂ÷Ãú
				col = lerp(col,_FarColor, smoothstep(0,_FarDistance,i.worldPos.z));


                return col ;
            }
            ENDCG
        }
    }
}
