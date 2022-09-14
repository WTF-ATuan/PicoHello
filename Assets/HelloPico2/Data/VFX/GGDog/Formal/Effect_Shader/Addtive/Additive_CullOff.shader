Shader "GGDog/Additive_OneOne_CullOn"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HDR("HDR",Range(1,7)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 1
		
		ZWrite Off
		Blend One One

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

            float _HDR;
            
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
                
                col*= i.color* i.color.a * _HDR;
                
               // col.rgb*=col.a;


                return col;
            }
            ENDCG
        }
    }
}
