Shader "Unlit/Toon_Light"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_LightDir ("Light Direction", Vector) = (0.25, 0.75, 0 ,0)
		_RimWidth ("Rim Width", Float) = -25
    }
    SubShader
    {

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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal_VS : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float4 normal_OS = float4(v.normal.xyz,0);
                o.normal_VS = mul(UNITY_MATRIX_MV,normal_OS);

                return o;
            }
            float3 _LightDir;
            float _RimWidth;
            
            fixed4 frag (v2f i) : SV_Target
            {
                
                i.normal_VS = float4(normalize(i.normal_VS.xyz),1);

                half N_VS_Dot_L = smoothstep(0,0.25,dot(i.normal_VS.xyz,_LightDir)-0.15);

                fixed4 col = tex2D(_MainTex, i.uv.xy)*saturate(N_VS_Dot_L+0.5) ;

                return col;
            }
            ENDCG
        }
    }
}
