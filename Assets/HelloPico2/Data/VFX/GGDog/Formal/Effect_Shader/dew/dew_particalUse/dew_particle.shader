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
			#pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half3 normal : NORMAL;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half _SrcUVTilling;
            half _NoiseStrength;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                
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
                UNITY_SETUP_INSTANCE_ID (i);
                half n =  smoothstep(0.5,1,1-distance(frac(2*i.uv+_Time.y*half2(1,0.5)*0.25),0.5));
                half n2 =  smoothstep(0.3,1,1-distance(frac(2*i.uv-_Time.y*half2(0.7,1)*0.75),0.5));

                n+=n2;
                
                n *= smoothstep(0,0.5,i.uv.y);

                half4 col = tex2D(_MainTex, i.uv +n/15)*i.color*_Color;

                clip(col.a-0.03);

                return col;
            }
            ENDCG
        }
    }
}
