Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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
                float4 vertex : SV_POSITION;
                float2 World_uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				//�@�ɤ���uv
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.World_uv = TRANSFORM_TEX(worldPos.xz, _MainTex);
				
                return o;
            }
			
			float _r;
			float _r2;

			fixed _SpeedX;
			fixed _SpeedY;
			
			//�����i���
			fixed2 SawToothWave(fixed speedX , fixed speedY)
			{
				speedX = saturate(fmod(_Time.y*abs(speedX),1))*sign(speedX);
				speedY = saturate(fmod(_Time.y*abs(speedY),1))*sign(speedY);

				return fixed2(speedX,speedY);
			}
			fixed4 _Color;
			
            fixed4 frag (v2f i) : SV_Target
            {
				i.uv /= float2(_MainTex_ST.x,_MainTex_ST.y);

				//���߶Z����
				fixed D = 1- distance(float2(i.uv.x,i.uv.y),float2(0.5,0.5));

				//���h��
				D = smoothstep(_r2*_r,_r,D);

				//�Huv�����ߩ�j�Y�p��i: �Huv�����ߥb�|�Z���h�ܧΡA�[�WWorld_uv�H�����q���覡���@�ɤ��߰��
				i.uv = i.World_uv + (i.uv+0.5*(D-1))/D;

                fixed4 col = tex2D(_MainTex, i.uv/10 +  SawToothWave(_SpeedX,_SpeedY) );
				
				//�z����t���Ѫť�
				D = smoothstep(0,0.5,D);
				col = lerp(col,_Color,smoothstep(0.5,1,1-D));

                return col;
            }
            ENDCG
        }
    }
}
