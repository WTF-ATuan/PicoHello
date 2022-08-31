Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed("Speed", Range(0,10)) = 1.5
        [HDR]_Color("Color", COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off

        Blend SrcAlpha One

        Cull Off
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
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2 * RadialScale;
                float angle = atan2(delta.x, delta.y) * 1.0 / 6.28 * LengthScale;
                Out = float2(radius, angle);
            }

            float _Speed;
            float4 _Color;
            
            fixed4 frag(v2f i) : SV_Target
            {

                //¤¤¤ß¶ZÂ÷³õ
                float D = 1 - distance(i.uv, 0.5);
                D = smoothstep(0.5, 0.75, D);
                D *= 1 - smoothstep(0.75, 1.2, D);
                float2 PUV = i.uv ;
                Unity_PolarCoordinates_float(i.uv, float2(0.5, 0.5), _MainTex_ST.x, _MainTex_ST.y, PUV);
                PUV -= float2(_Time.y* _Speed, 0);


                fixed4 col = tex2D(_MainTex, PUV)* D * i.color * _Color;

                
                return col;
            }
            ENDCG
        }
    }
}
