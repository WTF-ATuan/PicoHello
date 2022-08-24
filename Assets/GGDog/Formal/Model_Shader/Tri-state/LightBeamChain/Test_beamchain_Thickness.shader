Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Scale ("Scale", Range(0,1)) = 1
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
                half4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half3 normal : NORMAL;
            };

            half _Scale;
            v2f vert (appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.normal = v.normal;
                
               // half scale = smoothstep(-1,1,0.75-v.uv.y) * smoothstep(-0.65,0.75,v.uv.y*2)*_Scale;
                
              //  half scale = (1-smoothstep(-1,1,(0.5+v.uv.y)) * smoothstep(-1,1,v.uv.y/2))*_Scale;

                half scale = smoothstep(0,1.75,1-v.uv.y)*_Scale;


                o.vertex = UnityObjectToClipPos(v.vertex - 0.01*v.normal*scale);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return 1;
            }
            ENDCG
        }
    }
}
