Shader "Unlit/aurora"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Tilling ("Tilling", Vector) = (1,1,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" }
        LOD 1

        ZWrite Off

        Blend SrcAlpha One

        Cull Off
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
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half3 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half4 _Color;
            half4 _Tilling;
            
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

            

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                
				half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                half Noise = WaterTex(worldPos.xy,0.075,0.15);
                half Noise2 = WaterTex(worldPos.xy,0.05,-0.05);

                 o.vertex = UnityObjectToClipPos(v.vertex  + 0.5* (Noise+Noise2)*v.normal);
                 o.uv.xy = v.uv;
                
				 o.uv.z = smoothstep(0.35,0.5,length(mul(UNITY_MATRIX_MV,v.vertex).xyz)/70);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                
                UNITY_SETUP_INSTANCE_ID (i);

                half Mask;

                Mask = smoothstep(0,1+0.35*saturate(1+_Time.y),i.uv.y)*smoothstep(0,0.5,1-i.uv.y)*smoothstep(0,0.25,1-i.uv.x)*smoothstep(0,0.25,i.uv.x);

                half4 col = tex2D(_MainTex,_Tilling.x* i.uv.xy + _Time.y*half2(0.175,0))*Mask;
                half4 col2 = tex2D(_MainTex,_Tilling.y* i.uv.xy - _Time.y*half2(0.05,0))*Mask;

               // col = saturate(col+col2);

                col = max(col,col2);

                return col*_Color*i.uv.z;
            }
            ENDCG
        }
    }
}
