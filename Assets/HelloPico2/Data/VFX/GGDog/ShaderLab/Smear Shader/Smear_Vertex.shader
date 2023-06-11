Shader "Unlit/Smear_Vertex"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Shadow Color", Color) = (0.25, 0.25, 0.25, 1)
        _RimColor ("Rim Color", Color) = (0.65, 0.85, 1, 1)
        
		_Vector ("Light Direction", Vector) = (0, 0, 0)
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
		        float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;

				half3 worldNormal : TEXCOORD1;
				half3 worldPos : TEXCOORD2;
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

		    half3 _Smear_Dir;
			half3 _Vector;

            v2f vert (appdata v)
            {
                v2f o;

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;


                //half Smear_dot = smoothstep(0,1.5,dot(_Smear_Dir,o.worldNormal));
                
                half3 WorldUV = mul(unity_ObjectToWorld, v.vertex).xyz;

                //o.vertex = UnityObjectToClipPos(v.vertex + _Smear_Dir*saturate(Smear_dot)*1.5*WaterTex(WorldUV.xz,500000,500));
               

                half Smear_dot = clamp(dot(_Smear_Dir,o.worldNormal)+1,-1,1);

                half3 smear = _Smear_Dir*Smear_dot*200*saturate(WaterTex(WorldUV.xz,500000,100)-0.65)
                                                        *saturate(WaterTex(WorldUV.xz,500000,1000)-0.85) 
                                                           *saturate(WaterTex(WorldUV.xz,1,0.000001)+0.1) ;
                                                           
                half3 smear2 = _Smear_Dir*Smear_dot*0.5*saturate(WaterTex(WorldUV.xz,50000,1000));

                o.vertex = UnityObjectToClipPos(v.vertex +smear+smear2);

                return o;
            }
            

            half4 _MainColor;
            half4 _ShadowColor;
            half4 _RimColor;
        
            fixed4 frag (v2f i) : SV_Target
            {
                
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				half3 worldNormal = normalize(i.worldNormal);
                
                half LdotN = smoothstep(0,1.5,dot(_Vector,i.worldNormal));

				half NdotV = dot(worldNormal,worldViewDir);

				half Rim = saturate(1-smoothstep(-0.25,1.5,NdotV))*(1-LdotN)*0.75;

                half4 col = lerp(_ShadowColor,_MainColor,LdotN);

                return col + Rim*_RimColor;
            }
            ENDCG
        }
    }
}
