Shader "GGDog/Env_StoneParticle"
{
	Properties
	{
		_RimColor("RimColor",Color) = (1,1,1,1)
        
        _ColorLerp("ColorLerp", Range(0,1)) = 0
        
		_Rim2Color("Rim2Color",Color) = (1,1,1,1)
		_Color2("Color2",Color) = (1,1,1,1)
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
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

			fixed4 _RimColor;
			fixed4 _Rim2Color;
			fixed4 _Color2;

			fixed _ColorLerp;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 normalDir = normalize(i.worldNormal);

                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
				fixed3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);
				
				fixed3 worldNormal = normalize(i.worldNormal);

				fixed Rim = 1-saturate(smoothstep(0,0.05,dot(worldNormal,worldViewDir)*(1-dot(normalDir,lightDir)) ));

                _RimColor = lerp(_RimColor,_Rim2Color,_ColorLerp);
                i.color = lerp(i.color/1.75,_Color2,_ColorLerp);

                fixed4 FinalColor = lerp(_RimColor,i.color,smoothstep(-20,0,i.worldPos.y))+Rim*_RimColor;
				
                return FinalColor;
            }
            ENDCG
        }
    }
}
