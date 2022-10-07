Shader "Unlit/Boss_crack"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
		_Fade_min("Fade_min",Range(0,1))=0.5
		_Fade_max("Fade_max",Range(0,0.5))=0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _MainTex2;
            
			uniform	fixed4 BossCrack_GlobalPos;
            half _Fade_min;
            half _Fade_max;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = v.uv;
                

                half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz ;
				o.uv.z = smoothstep(_Fade_min,_Fade_min+_Fade_max,distance( BossCrack_GlobalPos,worldPos));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 col2 = tex2D(_MainTex2, i.uv);

                col = lerp( lerp(0,col,1-col2.r), col , i.uv.z );

                return col;
            }
            ENDCG
        }
    }
}
