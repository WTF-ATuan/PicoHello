Shader "Unlit/CenterFade_Trail"
{
    Properties
    {
		_Alpha("Alpha",Range(0,1))=1
        _MainTex ("Texture", 2D) = "white" {}
		_Fade_min("Fade_min",Range(0,10))=2
		_Fade_max("Fade_max",Range(0,10))=5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 1
		
		Zwrite Off
		Blend One One
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
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
			uniform	fixed4 CenterFadeTrail_GlobalPos;
            half _Fade_min;
            half _Fade_max;
            half _Alpha;
            
            v2f vert (appdata v)
            {
                v2f o;
                half3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz ;
                
				o.uv.z = smoothstep(_Fade_min,_Fade_max,distance( CenterFadeTrail_GlobalPos,worldPos));

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv.xy)*i.uv.z;
                return col*i.color * half4(1,1,1,_Alpha);
            }
            ENDCG
        }
    }
}
