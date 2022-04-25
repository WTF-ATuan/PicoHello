Shader "Unlit/Reflection_Camera"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float4 scrPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.scrPos = ComputeGrabScreenPos(o.vertex);  //抓取螢幕截圖的位置

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 ScrUV = i.scrPos.xy/i.scrPos.w;

				ScrUV.y =1-ScrUV.y;

				i.uv = float2(1-i.uv.x,i.uv.y);
				fixed4 col = tex2D(_MainTex, ScrUV)*float4(0.5,1,0.75,1);

                return col;
            }
            ENDCG
        }
    }
}
