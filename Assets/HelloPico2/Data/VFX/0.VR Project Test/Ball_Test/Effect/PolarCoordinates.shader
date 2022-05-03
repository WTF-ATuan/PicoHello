Shader "MyShader/PolarCoordinates"
{
    Properties
    {
		[HDR]_TintColor("Tint Color", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _Intensity("intensity", float) = 0.1
        _XSpeed("Flow Speed", float) = -0.2
		
        _FadeOut("GradientAlphaOut", Range(0,1)) = 0.5

		_NoiseCenterOffset("NoiseCenterOffset", Vector) = (0.5, 0.5,0)
		_RadialScale("RadialScale", Range(0,1)) = 0.5
		_LengthScale("LengthScale", Range(0,5)) = 1
    }
    SubShader
    {
        Tags {"RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
		Cull Off
        Pass
        {
			Blend SrcAlpha One
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
			Vector _NoiseCenterOffset;
			
            float _RadialScale;
            float _LengthScale;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _NoiseTex);
				o.color = v.color;
                return o;
            }

			void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
            {
                 float2 delta = UV - Center;
                 float radius = length(delta) * 2 * RadialScale;
                 float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                 Out = float2(radius, angle);
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 tc = i.uv2-float2(0.5,0.5);
				float d= length(tc);

				float2 PUV = i.uv;
				
				Unity_PolarCoordinates_float(i.uv,_NoiseCenterOffset,_RadialScale,_LengthScale,PUV);

                fixed4 noise_col = tex2D(_NoiseTex, _Time.x*_XSpeed*fixed2(1,0) + PUV );

                fixed Offset = noise_col.r;
				
                fixed4 col = tex2D(_MainTex,i.uv-_Intensity * fixed2(0.5,0.5) +  _Intensity * fixed2(Offset, Offset));


                return fixed4(col.rgb*i.color.rgb*_TintColor.rgb ,col.a*i.color.a * smoothstep(  0,_FadeOut , col.r-(1-i.color.a)  ) *saturate(1-d*2)  );
            }
            ENDCG
        }
    }
}
