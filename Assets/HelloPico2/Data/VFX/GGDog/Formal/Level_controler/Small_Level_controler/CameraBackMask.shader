Shader "Unlit/CameraBackMask"
{
    Properties
    {
		[HDR]_Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        ZTest Always

        Blend SrcAlpha OneMinusSrcAlpha

        Cull Front
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
            float4 _Color;
            float4 frag (v2f i) : SV_Target
            {
				//¤¤¤ß¶ZÂ÷³õ
                float2 CenterUV = (i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5);
                
				float D = smoothstep(-16,9.6,1-49*CenterUV-1)*0.85;
				float D2 = smoothstep(-41.5,60,1-460.9*CenterUV-1);

                D = (D*D+D2*D2*5.5);

                clip(D - 0.0015);
                
                return float4(_Color.rgb,D);
            }
            ENDCG
        }
    }
}
