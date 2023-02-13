Shader "GGDog/Effect/BeamLight"
{
    Properties
    {
		[Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend Mode", Float) = 1
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        ZWrite Off
		Blend [_SourceBlend] [_DestBlend]

		ZTest [_ZTest]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ
				half D = smoothstep(-25.8,33.6,1-80.1*((0.4*i.uv.x-0.5+0.5)*(0.4*i.uv.x-0.5+0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
                D =D*D;
                D = smoothstep(0,0.025,saturate(D*1))*2;


                half a = smoothstep(0.25,1,i.uv.x) * smoothstep(0,0.25,1-i.uv.x);
                half3 col = lerp( i.color.rgb , 1 , a * 0.15 );

                return half4( col , a* i.color.a *D );
            }
            ENDCG
        }
    }
}
