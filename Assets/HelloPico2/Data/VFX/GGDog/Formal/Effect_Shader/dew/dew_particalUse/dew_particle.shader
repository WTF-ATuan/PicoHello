// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Unlit/SpriteDefault"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color ("Color", Color) = (1,1,1,1)

        _SrcUVTilling ("SrcUV Tilling", Float) = 1
        _NoiseStrength ("Noise Strength", Float) = 1
    }
    SubShader
    {
		Tags
		{
			"Queue"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half _SrcUVTilling;
            half _NoiseStrength;
            
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
				half4 srcUV = ComputeScreenPos(v.vertex);  //抓取螢幕截圖的位置
                
                half n =  smoothstep(0.5,1,1-distance(frac(_SrcUVTilling*30*srcUV.xy/srcUV.w-_Time.y*half2(1,0.5)*0.25),0.5));
                half n2 =  smoothstep(0.5,1,1-distance(frac(_SrcUVTilling*20*srcUV.xy/srcUV.w+_Time.y*half2(0.7,1)*0.75),0.5));
                
                n+=n2;

                o.vertex = UnityObjectToClipPos(v.vertex + v.normal*n*0.0175*_NoiseStrength);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }
            half4 _Color;
            
            half4 frag (v2f i) : SV_Target
            {
                /*
                half n =  smoothstep(0.5,1,1-distance(frac(2*i.uv+_Time.y*half2(1,0.5)*0.25),0.5));
                half n2 =  smoothstep(0.3,1,1-distance(frac(2*i.uv-_Time.y*half2(0.7,1)*0.75),0.5));
                n+=n2;
                */

                half n = WaterTex(i.uv,10,2);
                
                n *= smoothstep(0,0.5,i.uv.y);

                half4 col = tex2D(_MainTex, i.uv +n/15)*i.color*_Color;

                clip(col.a-0.03);

                return col;
            }
            ENDCG
        }
    }
}
