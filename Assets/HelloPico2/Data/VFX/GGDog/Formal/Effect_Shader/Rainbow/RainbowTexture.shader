Shader "GGDog/RainbowTexture"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
		_Alpha("_Alpha",Range(0,1)) = 1
		_Gray("_Gray",Range(0,1)) = 1
		_Width("_Width",Range(1,10)) = 5
		_OffSet("OffSet",Range(-10,10)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
		ZTest Always
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
				
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }
			
            float _Width;
            float _OffSet;
            float _Alpha;
            float _Gray;
			
            float4 frag (v2f i) : SV_Target
            {

                float D = tex2D(_MainTex, i.uv).r;

                float4 col = 1;

				col =lerp( float4(1,0,0,1) , float4(0,1,0,1) , smoothstep(0,2,(D)*_Width +_OffSet));
				col =lerp(             col , float4(0,0,1,1) , smoothstep(0,2,(D)*_Width-1+_OffSet));

				col+=0.5;

				col = float4(lerp( 0 , 1-2*(1-col.rgb), step(0.5,col.rgb) ),D);
				

                return saturate(col*2.5);

            }

            ENDCG
        }
    }
}
