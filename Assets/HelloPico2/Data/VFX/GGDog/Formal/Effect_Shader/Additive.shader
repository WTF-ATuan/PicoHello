Shader "GGDog/Additive_OneOne_CullOff"
{
    Properties
    {
		_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 1

		Blend One One
		Zwrite Off

        Cull Off
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
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR;
				float CameraDistance : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID (v);
                UNITY_TRANSFER_INSTANCE_ID (v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

				fixed4 col = tex2D(_MainTex, i.uv)*i.color*i.color.a;

                col = lerp(col,col*fixed4(i.color.rgb*i.color.rgb,1),1-i.color.a);
                
				col.a *= smoothstep(10,90,i.CameraDistance);
                
               // col.rgb*=col.a;

                clip(i.color.a-0.0015);
                
                return col;
            }
            ENDCG
        }
    }
}
