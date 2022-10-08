Shader "GGDog/Badge"
{
    Properties
    {
		[HDR]_Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
				half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				half3 normal : NORMAL;
				half3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }
            fixed4 _Color;
            fixed4 frag (v2f i) : SV_Target
            {
                half3 worldNormal  = normalize(i.normal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half Rim = 1-dot(worldNormal, viewDir);
				Rim = smoothstep(-1,0.5,Rim);

                return _Color*Rim;
            }
            ENDCG
        }
    }
}
