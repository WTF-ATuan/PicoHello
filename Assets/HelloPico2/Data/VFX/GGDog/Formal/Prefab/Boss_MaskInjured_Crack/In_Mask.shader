Shader "GGDog/In_Mask_BossInjured"
{
    Properties
    {
		_Layer("Layer",Range(0,30)) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 1
		
        ColorMask RGBA
		
        ZWrite Off
        Stencil {
            Ref [_Layer]
            Comp Equal
            Pass Keep
        }
        ZTest Always
        Pass
        {
		    Blend SrcAlpha One
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
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
				
                return o;
            }
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID (i);

                half3 TexRGB = tex2D(_MainTex, i.uv).rgb;

                half col = TexRGB.g;
                
                half GlowTex = TexRGB.b;

                half MaskTex = TexRGB.r;
				
                col *= step(1,MaskTex+i.color.a) *i.color.a;
                GlowTex *= (MaskTex+i.color.a);

                col += GlowTex*smoothstep(0,0.5,i.color.a);

                return half4(col*i.color.rgb,col);
            }
            ENDCG
        }
    }
}
