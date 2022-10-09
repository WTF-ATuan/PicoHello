Shader "GGDog/Badge"
{
    Properties
    {
		[HDR]_Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
				half3 normal : NORMAL;
				half4 color : COLOR;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half3 normal : NORMAL;
				half3 worldPos : TEXCOORD1;
				half4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                o.color = v.color;

                return o;
            }
            fixed4 _Color;
            fixed4 frag (v2f i) : SV_Target
            {
                half3 worldNormal  = normalize(i.normal);
                half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				half Rim = 1-dot(worldNormal, viewDir);
				Rim = smoothstep(-1,1.5,Rim);
				Rim += smoothstep(0.8,0.8,Rim);
                
                return _Color*Rim*i.color.a;
            }
            ENDCG
        }
        
        
    }
}
