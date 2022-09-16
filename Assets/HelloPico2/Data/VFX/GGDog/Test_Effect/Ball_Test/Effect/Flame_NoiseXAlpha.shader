Shader "MyShader/Flame_Noise*Alpha"
{
    Properties
    {
        [Enum(Additive,1,Sprite,10)]
		_Blend("Blend", Int) =1

		[HDR]_TintColor("Tint Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _Intensity("intensity", float) = 0.1
        _XSpeed("Flow Speed", float) = -0.2
		
        _FadeOut("GradientAlphaOut", Range(0,1)) = 0.5

    }
    SubShader
    {
        Tags {"RenderType"="Transparent" "Queue"="Transparent" }
		LOD 1
		Cull Off
        Pass
        {
			Blend SrcAlpha [_Blend]
			ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };
			
			float4 _TintColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float _Intensity;
            float _XSpeed;

            float _FadeOut;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 noise_col = tex2D(_NoiseTex, i.uv2 + fixed2(-_Time.x*_XSpeed/2, _Time.x*_XSpeed));
                fixed Offset = noise_col.r;
				
                fixed4 noise_col2 = tex2D(_NoiseTex, i.uv2 + fixed2(_Time.x*_XSpeed/2,_Time.x*_XSpeed));
                fixed Offset2 = noise_col2.r;

				_Intensity *= i.color.a;

                fixed4 col = tex2D(_MainTex, i.uv + fixed2(-0.2,-0.2)*(_Intensity+0.2) + _Intensity * fixed2(Offset, Offset)* fixed2(Offset2, Offset2));

                return fixed4(col.rgb*i.color.rgb*_TintColor.rgb ,col.a*i.color.a * smoothstep(0,_FadeOut,col.r-(1-i.color.a))  );
            }
            ENDCG
        }
    }
}
