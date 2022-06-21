Shader "GGDog/3DWorldUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
				half4 worldPos : TEXCOORD1;
				half4 scrPos : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
               // o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.uv = TRANSFORM_TEX(o.worldPos.xy, _MainTex);

				o.scrPos = ComputeGrabScreenPos(o.vertex);  //抓取螢幕截圖的位置

                return o;
            }

			void Unity_PolarCoordinates_float(float2 UV,float2 Center,float RadialScale,float Length,out float2 Out)
			{
				float2 delta = UV - Center;
				float radius = length(delta)*2*RadialScale;
				float angle = atan2(delta.x,delta.y)*1/6.28*Length;
				Out = float2(radius,angle);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				/*
				float2 PUV = i.uv/i.worldPos.w;

				Unity_PolarCoordinates_float(i.uv/i.worldPos.w,float2(0.5,0.5),1,1,PUV);


                fixed4 col = tex2D(_MainTex, (i.uv/i.worldPos.w) * float2(0.005,0.0075) + fixed2(0,1) * _Time.y);
				*/

				
				float2 uv_front = TRANSFORM_TEX(i.worldPos.xy, _MainTex);
				float2 uv_side = TRANSFORM_TEX(i.worldPos.zy, _MainTex);
				float2 uv_top = TRANSFORM_TEX(i.worldPos.xz, _MainTex);
				
				fixed4 col_front = tex2D(_MainTex, uv_front);
				fixed4 col_side = tex2D(_MainTex, uv_side);
				fixed4 col_top = tex2D(_MainTex, uv_top);
				
				fixed4 col = col_front + col_side + col_top;

                return smoothstep(0,2.5,col);
            }
            ENDCG
        }
    }
}
