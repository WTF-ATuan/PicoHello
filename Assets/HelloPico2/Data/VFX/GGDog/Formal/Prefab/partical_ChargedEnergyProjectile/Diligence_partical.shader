Shader "GGDog/partical_ChargedEnergyProjectile/Diligence_partical"
{
    Properties
    {
        _Tilling ("Tilling", Range(0,30)) = 5
        _Speed ("Speed", Range(-30,30)) = 10
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off
        Blend SrcAlpha One

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            half2 Rotate_UV(half2 uv , half sin , half cos)
            {
                return float2(uv.x*cos - uv.y*sin ,uv.x*sin + uv.y*cos);
            }
            half WaterTex(v2f i,half Tilling,half FlowSpeed)
            {
                i.uv.xy*=Tilling;
                half Time = _Time.y*FlowSpeed;

                i.uv.xy = Rotate_UV(i.uv,0.34,0.14);
                half2 UV = frac(i.uv.xy*0.75+Time* float2(-1,0.25));
				half D = smoothstep(-10.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                i.uv.xy = Rotate_UV(i.uv,0.94,0.44);
                UV = frac(i.uv.xy*1.2+Time*0.33* float2(-0.24,-0.33));
				half D2 = smoothstep(-18.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);
                
                i.uv.xy = Rotate_UV(i.uv,0.64,0.74);
                UV = frac(i.uv.xy*1+Time*1.34* float2(0.54,0.33));
				half D3 = smoothstep(-15.4,4.2,1-38.7*((UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5))-1);

                D = 1-max(max(D,D2),D3);
                
                return D;
            }
            half _Tilling;
            half _Speed;

            fixed4 frag (v2f i) : SV_Target
            {

                
				half D = smoothstep(-10.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                half D2 =WaterTex(i,_Tilling,_Speed)*D*3;

                return D2*i.color;
            }
            ENDCG
        }
    }
}
