// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/GloryCircle"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
		_Alpha("_Alpha",Range(0,1)) = 1
		_Gray("_Gray",Range(0,1)) = 1
		_Width("_Width",Range(1,10)) = 5
		_OffSet("OffSet",Range(-10,10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		ZTest Always
		Blend SrcAlpha One
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
                return o;
            }
			
            float _Width;
            float _OffSet;
            float _Alpha;
            float _Gray;
            float4 _Color;
			
            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);
				//¤¤¤ß¶ZÂ÷³õ
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));
				//float D = smoothstep(-15.4,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                float4 col = 1;

				col =lerp( float4(1,0,0,1) , float4(0,1,0,1) , (D)*_Width +_OffSet);
				col =lerp(             col , float4(0,0,1,1) , (D)*_Width-0.3 +_OffSet);

				col+=0.5;

				col = float4(lerp( 0 , 1-2*(1-col.rgb), step(0.5,col.rgb) ),1);
				
				
				float D2 = saturate(smoothstep(0.65,1,D)*2);

				float D3 = saturate(smoothstep(0.75,1,D)*2);

				float DD = saturate(D2-D3)*2.5;

				col = lerp(col,float4(0.5,0.5,0.5,1),1-_Gray);

                clip(DD-0.15);

                return _Color*i.color*DD*saturate(col/10)*3*_Alpha;

            }

            ENDCG
        }
    }
}
