//Author：AboutVFX
//Email:54315031@qq.com
//QQ Group：156875373
Shader "AboutVFX/GhostTrail" {
    Properties {
        _TintColor ("TintColor", Color) = (0.5,0.5,0.5,0.5)
        _MainTex ("MainTex", 2D) = "white" {}
        [HideInInspector]_Opacity ("Opacity", Range(0, 1)) = 1
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	    Blend SrcAlpha One
	    ColorMask RGB
	    Cull Off Lighting Off ZWrite Off
        Pass {            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            uniform sampler2D _MainTex; 
            uniform fixed4 _TintColor;
            uniform fixed _Opacity=1.0;

            struct appdata {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };
            struct v2f {
                fixed4 pos : SV_POSITION;
                fixed2 uv : TEXCOORD0;
            };
            v2f vert (appdata v) {
                v2f o;
                o.uv = v.uv;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            fixed4 frag(v2f i) : SV_Target {
                return (tex2D(_MainTex,i.uv)*_TintColor*_TintColor.a*_Opacity);
            }
            ENDCG
        }
    }
}
