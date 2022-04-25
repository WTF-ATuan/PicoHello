Shader "Unlit/PointGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

		Blend One One
		Zwrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

				//中心距離場
				fixed D =1- distance(fixed2(i.uv.x,i.uv.y),fixed2(0.5,0.5));

				//漸層度
				fixed D2 = smoothstep(0.75,1.5,D)*8;

				D = D2+smoothstep(0.5,1.5,D)*1.5;

                return i.color*D*i.color.a;
            }
            ENDCG
        }
    }
}
