// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/HitTarget_outline"
{
    Properties
    {
		_OutLine("OutLine",Range(0,0.1)) = 0.03
		_OutLineColor("OutLineColor",Color) = (1,1,0.5,0.5)
		_OutLineNoiseTiling("OutLine Noise Tiling",Float) = 0.01

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
        
        Pass
        {
            Tags { "Queue"="Geometry-1000"}
            ZWrite Off
            Cull Front
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
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

                D = 1-max(max(D,D2),D3);
                
                return D;
            }


            half _OutLine;
            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*_OutLine);

                return o;
            }
            half4 _OutLineColor;
            half _OutLineNoiseTiling;
            
            half4 frag (v2f i) : SV_Target
            {
                half d =smoothstep(0.15,0.5,WaterTex(i.vertex.xy +half2(0,-100)*_Time.y,_OutLineNoiseTiling,0));

                return _OutLineColor*_OutLineColor.a*d;
            }
            ENDCG
        }

        Pass
        {
            Tags { "Queue"="Geometry"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
				half3 worldPos : TEXCOORD0;
            };
            
            half3 _LightPos;

            v2f vert (appdata v)
            {
                v2f o;

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
