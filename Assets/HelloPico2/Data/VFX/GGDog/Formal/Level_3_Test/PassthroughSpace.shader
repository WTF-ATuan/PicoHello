Shader "Unlit/PassthroughSpace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Clip ("Clip", Range(0,1)) = 0
        _Gradient ("Gradient", Range(0.5,0.666)) = 0.6

        _SkyColor ("Sky Color", Color) = (1, 1, 1, 1)
        [HDR]_HorizonColor ("Horizon Color", Color) = (1, 1, 1, 1)
		
        _SmoothStepMin ("漸層度(最低)", Range(0, 1)) = 0
        _SmoothStepMax ("漸層度(最高)", Range(0, 1)) = 1

        _WaterColor ("Water Color", Color) = (1, 1, 1, 1)
		
        _OriScale ("Original Scale", float) = 3000
    }
    SubShader
    {

		Tags{ "RenderType"="transparent" "Queue"="transparent"}

		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
			Cull Front
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

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
			
            float _Clip;
            float _Gradient;
			
            float4 _WaterColor;
            fixed4 frag (v2f i) : SV_Target
            {

				_Clip = _Clip - _Gradient + (0.5-(0.5-_Gradient))*_Clip;


				float value = smoothstep(0+_Gradient +_Clip-(1-_Gradient) ,1-_Gradient +_Clip ,i.uv.y);
				float value2 = smoothstep(0+0.6 +_Clip-(1-0.6) ,1-0.6 +_Clip ,i.uv.y);

                fixed4 col2 = tex2D(_MainTex, i.uv + pow(value2,1)*float2(0,1) +_Time.y*float2(0,0.15) );
				


				float scale = i.worldPos.x;
				
				float4 col = lerp(_HorizonColor,_SkyColor,smoothstep(_SmoothStepMin,_SmoothStepMax, i.uv.y) );
				

				col = lerp(col,col2,value);


				col = lerp(_WaterColor,col,smoothstep(-100*scale,300*scale,i.worldPos.y));

				col = lerp(_HorizonColor,col,smoothstep(-1500*scale,250*scale,i.worldPos.z));



                return col;
            }
            ENDCG
        }
    }
}
