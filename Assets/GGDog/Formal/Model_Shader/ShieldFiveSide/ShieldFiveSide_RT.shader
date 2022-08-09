Shader "Unlit/ShieldFiveSide_RT"
{
    Properties
    {
        _RenderTex("Render Tex", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
		[HDR]_Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
				half4 scrPos : TEXCOORD3;
            };
            
            sampler2D _NoiseTex;
            half4 _NoiseTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置
                o.uv = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }
            
			sampler2D _RenderTex;
            
            half4 _Color;
            fixed4 frag (v2f i) : SV_Target
            {
				//內容底背景
				half2 scruv = i.scrPos.xy/i.scrPos.w;
                
				half Noise = tex2D(_NoiseTex, i.uv + _Time.y*half2(1,0)).r;

				half4 refrCol = tex2D(_RenderTex, scruv + Noise*0.005 *smoothstep(0,0.15,i.uv.x));

                refrCol.a *= smoothstep(0,0.85,i.uv.x);

                return refrCol + _Color*smoothstep(0,1.5,i.uv.x);
            }
            ENDCG
        }
    }
}
