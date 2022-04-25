Shader "Unlit/NewUnlitShader 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_r("r", range(0,2)) = 1
		_r2("r2", range(0,1)) = 0
		_SpeedX("SpeedX", range(-2,2)) = 0
		_SpeedY("SpeedY", range(-2,2)) = 0
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }
			
			float _r;
			float _r2;

			fixed _SpeedX;
			fixed _SpeedY;
			
            fixed4 frag (v2f i) : SV_Target
            {
			
				//中心距離場
				fixed D = 1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//漸層度
				D = smoothstep(_r2*_r,_r,D);

                fixed4 col = tex2D(_MainTex, i.uv+  float2(_SpeedX,_SpeedY)*_Time.y);
				

				col = lerp(col,fixed4(1,1,1,1),smoothstep(0,1,1-D));

                return col;
            }
            ENDCG
        }
    }
}
