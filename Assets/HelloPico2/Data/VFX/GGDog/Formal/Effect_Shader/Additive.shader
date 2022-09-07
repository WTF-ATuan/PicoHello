Shader "GGDog/Additive_OneOne_CullOff"
{
    Properties
    {
		_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

		Blend One One
		Zwrite Off

        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                fixed2 uv : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed4 color : COLOR;
				float CameraDistance : TEXCOORD1;
            };
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				o.CameraDistance = length(mul(UNITY_MATRIX_MV,v.vertex).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
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
