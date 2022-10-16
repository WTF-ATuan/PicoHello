Shader "GGDog/Effect/ShockWave"
{
    Properties
    {
        _RenderTex("Render Tex", 2D) = "white" {}
        _Intense("Intense", Range(0,1)) = 0.25
        [Enum(Order,4,AlwaysOnTop,8)] _ZTest("ZTest", Float) = 8
    }
    SubShader
    {
        Tags { "Queue"="Transparent+1" }

        Blend SrcAlpha OneMinusSrcAlpha

        ZWrite Off
        ZTest[_ZTest]

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
                half4 color : COLOR;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
				half4 scrPos : TEXCOORD3;
                half4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置
                
                o.color = v.color;
                return o;
            }
            
			sampler2D _RenderTex;
			half _Intense;
            fixed4 frag (v2f i) : SV_Target
            {
                
				float D = smoothstep(-13,4.2,1-38.7*((i.uv.x-0.5)*(i.uv.x-0.5)+(i.uv.y-0.5)*(i.uv.y-0.5))-1);
                D  = D*D;
                D -= D*D*1.5*(1+i.color.a*2);
                D *=15;
                
				//內容底背景
				half2 scruv = i.scrPos.xy/i.scrPos.w;

				half4 refrCol = tex2D(_RenderTex, scruv + D*_Intense*0.1*i.color.a) ;

                refrCol.a = saturate(D)*i.color.a;

                return refrCol;
            }
            ENDCG
        }
    }
}
