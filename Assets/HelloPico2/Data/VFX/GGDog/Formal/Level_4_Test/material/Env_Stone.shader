Shader "GGDog/Env_StoneParticle"
{
	Properties
	{
		_RimColor("RimColor",Color) = (1,1,1,1)
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half4 color : COLOR;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
                half3 worldNormal : TEXCOORD3;
				half3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
			half4 _RimColor;
            half4 frag (v2f i) : SV_Target
            {
                half3 normalDir = normalize(i.worldNormal);

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

				float Rim = 1-saturate(smoothstep(0,0.05,dot(worldNormal,worldViewDir)*(1-dot(normalDir,lightDir)) ));
				
                return lerp(_RimColor,i.color/1.75,smoothstep(-20,0,i.worldPos.y))+Rim*_RimColor;
            }
            ENDCG
        }
    }
}
