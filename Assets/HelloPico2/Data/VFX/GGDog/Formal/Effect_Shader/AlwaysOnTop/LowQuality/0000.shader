// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "GGDog/Space_Test/Glow_AlwaysOnTop"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
        _Alpha("Alpha",Range(0,1)) = 1
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
        
        _i("i",Range(1,5)) = 1
        
        _a("a",Float) = 1
        _b("b",Float) = 1
        _c("c",Float) = 1

        _a2("a2",Float) = 1
        _b2("b2",Float) = 1
        _c2("c2",Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
		Blend One One
		Zwrite Off
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
            float4 _Color;
            
            float _Alpha;
            
            float _a;
            float _b;
            float _c;
            float _a2;
            float _b2;
            float _c2;
            float _i;
            
            float4 frag (v2f i) : SV_Target
            {
				//���߶Z����
				float D = smoothstep(_b,_a,1-_c*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
				float D2 = smoothstep(_b2,_a2,1-_c2*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);

                D =D*D+D2*D2*13.5;

				i.color = lerp(i.color*i.color,i.color,D);

                float4 col = i.color * D ;

                clip(col.a - 0.0015);

                return col*_Color* i.color.a*_Alpha * _i;
            }
            ENDCG
        }
    }
}
