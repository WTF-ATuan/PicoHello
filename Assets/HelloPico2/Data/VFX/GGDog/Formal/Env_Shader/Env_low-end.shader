Shader "GGDog/Space_Test/Env_low-end"
{
    Properties
    {
		[HDR]_Color("Color",Color) = (1,1,1,1)
		_SkyColor("Sky Color",Color) = (1,1,1,1)
		_BackFogColor("Back Fog Color",Color) = (0,0,0,1)
		[HDR]_FogColor("Fog Color",Color) = (1,1,1,1)
        _FogPos ("Fog Pos", Range(-200,200)) = 77
    }
    SubShader
    {

        Pass
        {
			Tags { "RenderType"="Opaque" }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float CameraDistance : TEXCOORD1;
				float3 worldPos : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
                return o;
            }
            float4 _Color;
            float4 _SkyColor;
			
            float4 _BackFogColor;
            float4 _FogColor;

            float _FogPos;

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float4 col = _Color;

				col = lerp(col,_SkyColor,saturate(smoothstep(850,1000,i.worldPos.z)));
				

				col = lerp(col,_FogColor,(1-saturate(smoothstep(-100,300,i.worldPos.y+_FogPos))) *smoothstep(0,1000,i.CameraDistance)  );
				

				col = lerp(col,_BackFogColor,1-saturate(smoothstep(-700,1000,i.worldPos.z)));
				

                return col ;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
