Shader "Unlit/Env"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color) = (1,1,1,1)
		_ShadowColor("Shadow Color",Color) = (1,1,1,1)
		_FarColor("Far Color",Color) = (0.5,0.5,0.5,1)
		_BackFogColor("Back Fog Color",Color) = (0,0,0,1)
		_FogColor("Fog Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 CameraDistance : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
				
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);

                return o;
            }
            float4 _Color;
            float4 _ShadowColor;
			
            float4 _FarColor;
            float4 _BackFogColor;
            float4 _FogColor;
			
            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 normalDir = normalize(i.worldNormal);

                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				
                fixed3 ShadowColor = _ShadowColor;
                fixed3 LightColor  = _LightColor0* _Color ;

                fixed3 FinalColor =  lerp( ShadowColor, LightColor , max(dot(normalDir,lightDir),0)+0.25  );

				
				half2 NoiseUV = i.uv*sin(100) * 143758.5453;
				half  NoiseTex = (saturate( sin(NoiseUV.x*100)*cos(NoiseUV.y*100)-0.75)*2);

                fixed3 reflectDir = reflect(-lightDir,normalDir);
                fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );
				
                //°ª¥ú¤Ï®g
                fixed3 specular = _LightColor0.rgb * pow(max(dot(normalDir,lightDir),0),2)  * NoiseTex *LightColor*1.5;
                       specular += _LightColor0.rgb * (1-max(dot(normalDir,lightDir),0))  * NoiseTex *LightColor*0.15;






                fixed4 col = tex2D(_MainTex, i.uv)* fixed4(FinalColor,1)+fixed4(specular*1.5,0);

				col = lerp(col,_FarColor,saturate(smoothstep(100,700,i.CameraDistance)));
				
				col = lerp(col,_FogColor,1-saturate(smoothstep(-100,50,i.worldPos.y)));

				
				col = lerp(col,_BackFogColor,1-saturate(smoothstep(-1000,1000,i.worldPos.z)));
				

                return col;
            }
            ENDCG
        }
    }
}
