Shader "Unlit/RGBOffset"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RGBOffset ("RGB_Offset", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
		
		Zwrite Off
		Blend SrcAlpha One
		
        Pass
        {
		    Cull Front
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float _RGBOffset;
			
            fixed4 frag (v2f i) : SV_Target
            {
				
				float R_r = 1 + _RGBOffset/10;
				float B_r = 1 - _RGBOffset/10;

				float2 R_UV = (i.uv+0.5*(R_r-1))/R_r;
				float2 B_UV = (i.uv+0.5*(B_r-1))/B_r;
				
                float4 R = tex2D(_MainTex, R_UV);
                float4 G = tex2D(_MainTex, i.uv);
                float4 B = tex2D(_MainTex, B_UV);

				float4 col = float4(R.r*R.a,G.g*G.a,B.b*B.a,1);

                return col;
            }
            ENDCG
        }
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float _RGBOffset;
			
            fixed4 frag (v2f i) : SV_Target
            {
				
				float R_r = 1 + _RGBOffset/10;
				float B_r = 1 - _RGBOffset/10;

				float2 R_UV = (i.uv+0.5*(R_r-1))/R_r;
				float2 B_UV = (i.uv+0.5*(B_r-1))/B_r;
				
                float4 R = tex2D(_MainTex, R_UV);
                float4 G = tex2D(_MainTex, i.uv);
                float4 B = tex2D(_MainTex, B_UV);

				float4 col = float4(R.r*R.a,G.g*G.a,B.b*B.a,1);

                return col;
            }
            ENDCG
        }
    }
}
