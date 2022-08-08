Shader "Unlit/ShieldFiveSide"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
			#pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
				
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            half4 _Color;
            
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 col = tex2D(_MainTex, i.uv + _Time.y*half2(1,0.05));

                col*=col.a ;
                
                return col*_Color;
            }
            ENDCG
        }
    }
}
