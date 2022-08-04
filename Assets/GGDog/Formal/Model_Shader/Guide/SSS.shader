Shader "GGDog/SSS"
{
    Properties
    {
        _Color("Color",Color) = (0.75,0.75,0.75,1)
        _ShadowColor("ShadowColor",Color) = (0.25,0.25,0.25,1)
        _Distortion ("Distortion", Range(0,1)) = 0.75
        [HDR]_SSSColor("SSS Color",Color) = (1,0.5,0.5,1)
        _ThicknessTex ("Thickness Tex", 2D) = "black" {}
		
        _LightDir ("LightDir", Vector) = (0,0,0,0)
    }
    SubShader
    {
            Tags { "LightMode"="ForwardBase" }
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				
				o.worldNormal = mul(v.normal,(float3x3)unity_WorldToObject);
                o.worldPos = mul(v.vertex,unity_WorldToObject).xyz;

                return o;
            }
			float _Distortion;

			float4 _Color;
			float4 _ShadowColor;
			float4 _SSSColor;
			
            sampler2D _ThicknessTex;
			float4 _LightDir;
            
            float4 frag (v2f i) : SV_Target
            {
			
				half Time_y = abs(fmod(_Time.y*3,1.0f)*2.0f-1.0f);

				_Distortion = _Distortion+0.05*Time_y*cos(_Time.y);


                float4 thickness = tex2D(_ThicknessTex, i.uv);

                float3 WorldNormal = normalize(i.worldNormal);
                //float3 LightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed3 ViewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos );

				float NdotL = saturate(dot(WorldNormal,_LightDir));
				float Rim = 1-saturate(dot(WorldNormal,ViewDir));

				float3 H = normalize( ViewDir + WorldNormal * _Distortion);
				float I = saturate(dot(ViewDir,-H));

				float4 FinalColor = lerp(_ShadowColor + ( (Rim+I) * _SSSColor * thickness) ,_LightColor0 * _Color ,NdotL ) ;

                return FinalColor;
            }
            ENDCG
        }
    }
}
