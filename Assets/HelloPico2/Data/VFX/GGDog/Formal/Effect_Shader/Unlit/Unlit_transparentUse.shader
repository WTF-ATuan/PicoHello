Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
        _AlphaClip("Alpha Clip", Range(0,1)) = 0

        _X_Speed("X_Speed",Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

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
            float4 _Color;
            
            float _AlphaClip;
            
            float _X_Speed;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv + float2(_X_Speed,0)*_Time.y);

                col = lerp(col,_Color,col.a*_Color.a);
                
                clip(col.a-_AlphaClip*1.01);

                return col;
            }
            ENDCG
        }
    }
}
