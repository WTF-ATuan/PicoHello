Shader "GGDog/Grass_grow"
{
    Properties
    {
		_ColorLerp("ColorLerp",Range(0,1))=0
        _MainTex ("Texture", 2D) = "white" {}
		_AlphaClip("AlphaClip",Range(0,1))=0
		[HDR]_Color("Color",Color) = (1,1,1,1)
		[HDR]_BurnEdgeColor("BurnEdgeColor",Color) = (1,1,1,1)
		[HDR]_FarFogColor ("Far Fog Color", Color) = (.5,.5,.5,0)
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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half3 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
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
                half2 UV = frac(uv.xy*0.75+Time* half2(-1,-0.25));
                half UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D = smoothstep(-10.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.94,0.44);
                UV = frac(uv.xy*1.2+Time*0.33* half2(-1.74,0.33));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D2 = smoothstep(-18.4,4.2,1-38.7*UV_Center-1);
                
                uv.xy = Rotate_UV(uv,0.64,0.74);
                UV = frac(uv.xy*1+Time*1.34* half2(0.54,-0.13));
                UV_Center = (UV.x-0.5)*(UV.x-0.5)+(UV.y-0.5)*(UV.y-0.5);
				half D3 = smoothstep(-15.4,4.2,1-38.7*UV_Center-1);

                D = max(max(D,D2),D3);
                
                return D;
            }

            sampler2D _MainTex;
            half4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex + 0.05*v.uv.y * (WaterTex(v.vertex.xy,50,0.5)-0.5));
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                
				o.uv.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				o.uv.z =  saturate(smoothstep(35,100,o.uv.z));
                return o;
            }
            half _AlphaClip;
            half4 _Color;
            half _ColorLerp;
            half4 _BurnEdgeColor;
            
			half4 _FarFogColor;

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                half4 col = tex2D(_MainTex, i.uv.xy);

                clip(col.a-_AlphaClip);

                half B = 1-smoothstep(0,0.25,(1-i.uv.y*saturate(0.5+WaterTex(i.uv.xy,5,-1)))-1.1+_ColorLerp*1.2);

                col.rgb *= saturate(i.uv.y+0.65);

                half4 col2 = lerp(col*_BurnEdgeColor*_ColorLerp,col,saturate(B+0.45+WaterTex(i.uv.xy,20,1)*_ColorLerp) );

                col = lerp(col*_Color,col2,B);
                
                col.rgb = lerp(col.rgb,_FarFogColor.rgb,smoothstep(0,1,i.uv.z*_FarFogColor.a));
                
                return col;
            }
            ENDCG
        }
    }
}
