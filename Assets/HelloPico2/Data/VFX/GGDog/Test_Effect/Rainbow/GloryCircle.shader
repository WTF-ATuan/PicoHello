Shader "Unlit/GloryCircle"
{
    Properties
    {
		_Alpha("_Alpha",Range(0,1)) = 1
		_Gray("_Gray",Range(0,1)) = 1
		_Width("_Width",Range(1,10)) = 5
		_OffSet("OffSet",Range(-10,10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

		Blend SrcAlpha One
		ZWrite Off
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
			
            float _Width;
            float _OffSet;
            float _Alpha;
            float _Gray;
			
            float4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ
				float D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

                float4 col = 1;

				col =lerp( float4(1,0,0,1) , float4(0,1,0,1) , (D)*_Width +_OffSet);
				col =lerp(             col , float4(0,0,1,1) , (D)*_Width-0.3 +_OffSet);

				col+=0.5;

				col = float4(lerp( 0 , 1-2*(1-col.rgb), step(0.5,col.rgb) ),1);
				
				
				float D2 = saturate(smoothstep(0.65,1,D)*2);

				float D3 = saturate(smoothstep(0.75,1,D)*2);

				float DD = saturate(D2-D3)*2.5;

				col = lerp(col,float4(0.5,0.5,0.5,1),1-_Gray);


                return DD*saturate(col/10)*3*_Alpha;

            }

            ENDCG
        }
    }
}
