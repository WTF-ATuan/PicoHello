Shader "GGDog/Grass_use"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color) = (1,1,1,1)
        _AlphaClip("Alpha Clip", Range(0,1)) = 0
		[HDR]_FarFogColor ("Far Fog Color", Color) = (.5,.5,.5,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off //JK Edit

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half3 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				o.uv.z = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				o.uv.z =  saturate(smoothstep(35,70,o.uv.z));

                return o;
            }
            half4 _Color;
            
            half _AlphaClip;
            
			half4 _FarFogColor;

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv.xy)*_Color;

                col.rgb*=smoothstep(-1,1.5,i.uv.y);
                clip(col.a-_AlphaClip*1.01);
                
				col.rgb =lerp(col.rgb,_FarFogColor.rgb,i.uv.z*_FarFogColor.a);

                return col;
            }
            ENDCG
        }
    }
}
