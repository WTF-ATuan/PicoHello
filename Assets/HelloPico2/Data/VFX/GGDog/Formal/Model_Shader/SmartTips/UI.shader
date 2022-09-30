Shader "Unlit/UI"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }

        ZTest Always

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

				float2 scale = float2(length(unity_ObjectToWorld._m00_m10_m20),length(unity_ObjectToWorld._m01_m11_m21));
				float4 positionWS = unity_ObjectToWorld._m03_m13_m23_m33;
				float2 bias = v.vertex.xy;
				
				float3 forward = normalize(_WorldSpaceCameraPos - positionWS.xyz);

				float3 up = normalize(UNITY_MATRIX_V[1].xyz);
				float3 right = cross(forward, up);
				positionWS.xyz += right * bias.r * scale.x + up * bias.g  * scale.y;

				o.vertex = mul(UNITY_MATRIX_VP, positionWS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col*i.color;
            }
            ENDCG
        }
    }
}
