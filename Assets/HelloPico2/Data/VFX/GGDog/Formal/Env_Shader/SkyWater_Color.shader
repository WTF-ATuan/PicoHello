Shader "GGDog/Space_Test/Sky_Color"
{
    Properties
    {
        _SkyColor ("Sky Color", Color) = (1, 1, 1, 1)
        [HDR]_HorizonColor ("Horizon Color", Color) = (1, 1, 1, 1)
		
        _SmoothStepMin ("漸層度(最低)", Range(0, 1)) = 0
        _SmoothStepMax ("漸層度(最高)", Range(0, 1)) = 1

        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogPos ("Fog Pos", Range(-500,500)) = 0
		
        _OriScale ("Original Scale", float) = 3000
    }
    SubShader
    {
		LOD 100 
        Tags { "RenderType"="Opaque" }
        Cull Front
		Offset 10000, 10000
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
				float3 worldPos : TEXCOORD1;
            };

            float4 _SkyColor;
            float4 _HorizonColor;

            float _SmoothStepMin;
            float _SmoothStepMax;
			
            float _OriScale;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				//Scale
				o.worldPos.x = length(unity_ObjectToWorld._m00_m10_m20)/_OriScale;

                return o;
            }
            float4 _FogColor;
            float _FogPos;
			
            float4 frag (v2f i) : SV_Target
            {
				float scale = i.worldPos.x;
				
				float4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax, i.uv.y) );

				col = lerp(_FogColor,col,smoothstep(-100*scale,300*scale,i.worldPos.y+_FogPos));

				col = lerp(_HorizonColor,col,smoothstep(-1500*scale,250*scale,i.worldPos.z));

                return col;
            }
            ENDCG
        }
    }
    
    SubShader
    {
		LOD 1 
        Tags { "RenderType"="Opaque" }
        Cull Front
		Offset 10000, 10000
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
				float3 worldPos : TEXCOORD1;
            };

            float4 _SkyColor;
            float4 _HorizonColor;

            float _SmoothStepMin;
            float _SmoothStepMax;
			
            float _OriScale;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				//Scale
				o.worldPos.x = length(unity_ObjectToWorld._m00_m10_m20)/_OriScale;

                return o;
            }
            float4 _FogColor;
            float _FogPos;
			
            float4 frag (v2f i) : SV_Target
            {
				float scale = i.worldPos.x;
				
				float4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax, i.uv.y) );

				col = lerp(_FogColor,col,smoothstep(-100*scale,300*scale,i.worldPos.y+_FogPos));

				col = lerp(_HorizonColor,col,smoothstep(-1500*scale,250*scale,i.worldPos.z));

                return col;
            }
            ENDCG
        }
    }
    
    SubShader
    {
		LOD 0 
        Tags { "RenderType"="Opaque" }
        Cull Front
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            float4 frag (v2f i) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
