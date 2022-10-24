// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/Soft_PointGlow"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
        _Alpha("Alpha",Range(0,1)) = 1

        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
        _intense("Intense",Range(1,5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		
		Zwrite Off
		Blend One One
        
        ZTest[_ZTest]
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
            float _intense;
            float4 _Color;
            float _Alpha;

            float4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ
				float D = smoothstep(-25.8,33.6,1-80.1*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
                D =D*D;

				i.color = lerp(i.color*i.color,i.color,D);

                float4 col = i.color * D ;

                clip(col.a - 0.0015);

                return col*_Color* i.color.a*_Alpha * _intense;
            }
            ENDCG
        }
    }
}
