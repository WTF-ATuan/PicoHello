Shader "Unlit/VertexColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.BlendMode)] _SourceBlend ("Source Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DestBlend ("Dest Blend Mode", Float) = 10
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
		
		Zwrite Off
		Blend [_SourceBlend] [_DestBlend]
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

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv)*i.color;
                return col;
            }
            ENDCG
        }
    }
}
