Shader "GGDog/Grass_grow"
{
    Properties
    {
		_ColorLerp("ColorLerp",Range(0,1))=0
        _MainTex ("Texture", 2D) = "white" {}
		_AlphaClip("AlphaClip",Range(0,1))=0
		[HDR]_Color("Color",Color) = (1,1,1,1)
		[HDR]_BurnEdgeColor("BurnEdgeColor",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
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

                D = max(max(D,D2),D3);
                
                return D;
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex + 0.05*v.uv.y * (WaterTex(v.vertex.xy,50,0.5)-0.5));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float _AlphaClip;
            float4 _Color;
            float _ColorLerp;
            float4 _BurnEdgeColor;
            
            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                fixed4 col = tex2D(_MainTex, i.uv);

                clip(col.a-_AlphaClip);

                float B = 1-smoothstep(0,0.25,(1-i.uv.y*saturate(0.5+WaterTex(i.uv,5,-1)))-1.1+_ColorLerp*1.2);

                col.rgb *= saturate(i.uv.y+0.65);

                fixed4 col2 = lerp(col*_BurnEdgeColor*_ColorLerp,col,saturate(B+0.45+WaterTex(i.uv,20,1)*_ColorLerp) );

                col = lerp(col*_Color,col2,B);
                
                return col;
            }
            ENDCG
        }
    }
}
