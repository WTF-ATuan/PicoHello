Shader "Unlit/Ground"
{
    Properties
    {
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Geometry-900" }
        ZWrite Off
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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }
            
            half4 _SkyColor;
            half4 _ShadowColor;
            half _SkyFarPos;

            half4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ
				float D =distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				D = smoothstep(0,0.5,D);

               half4 FinalColor = lerp(_ShadowColor,_SkyColor*2, D );
               

                return half4(FinalColor.rgb,0.75);
            }
            ENDCG
        }
    }
}
