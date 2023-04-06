Shader "Unlit/SDF_partical"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_Radius_max("Radius_max",Float)=9
		_Radius_min("Radius_min",Float)=10

		_Amplitude("Amplitude",Float)=2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Blend SrcAlpha One
        ZWrite Off
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
            
			fixed4 GLOBAL_Pos;
            
			half _Radius_max;
			half _Radius_min;
			half _Amplitude;

            v2f vert (appdata v)
            {
                v2f o;
                
                half3 WorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                half SDF_Radius =  1-smoothstep(_Radius_min,_Radius_max,length(WorldPos-GLOBAL_Pos.xyz));

                half3 SDF_Direct =  normalize(WorldPos-GLOBAL_Pos.xyz);

                o.vertex = UnityObjectToClipPos(v.vertex + normalize(SDF_Direct)*_Amplitude*SDF_Radius);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
        }
    }
}
