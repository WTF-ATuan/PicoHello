Shader "Custom/Hole" {
    Properties{
    }
    SubShader
	{
        Tags { "RenderType"="Transparent" }

        ColorMask RGBA
		
        ZWrite Off
        Stencil {
            Ref 0
            Comp Less
            Pass Keep
        }
		
        Pass
        {
			Blend One One
			ZWrite Off
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

            fixed4 frag (v2f i) : SV_Target
            {
				//���߶Z����
				fixed D =1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//���h��
				fixed D2 = smoothstep(0.75,1.5,D)*8;

				D = D2+smoothstep(0.5,1.5,D)*1.5;

                return i.color*D*i.color.a;
            }
            ENDCG
        }
    }
}