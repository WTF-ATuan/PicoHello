
Shader "GGDog/Space_Test/PointGlow_Sprite"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
		
		Zwrite Off
		Blend SrcAlpha OneMinusSrcAlpha
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

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
				
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ

                float2 CenterUV = (i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5);

				float D = smoothstep(-16,9.6,1-49*CenterUV-1);
				float D2 = smoothstep(-32.5,47,1-460.9*CenterUV-1);

                D = i.color.a * (D*D+D2*D2*13.5);

                clip(D - 0.0015);

                return float4(i.color.rgb, D);
            }
            ENDCG
        }
    }
}