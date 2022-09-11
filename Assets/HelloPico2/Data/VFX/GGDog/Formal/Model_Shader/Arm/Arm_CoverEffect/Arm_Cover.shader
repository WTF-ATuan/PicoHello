Shader "GGDog/Arm_cover"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
		[HDR]_Color2("Color2",Color) = (1,1,1,1)
		_h("_h",Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        
		Zwrite Off
		ZTest Always
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
                half4 color : COLOR;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
				half4 scrPos : TEXCOORD1;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                //CameraDistance
				o.scrPos.z = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);

                return o;
            }
            
            half4 _Color;
            half4 _Color2;
            
            half _h;
            
            half4 frag (v2f i) : SV_Target
            {
				//中心距離場
				half D =1- distance(half2(i.uv.x,i.uv.y),half2(0.5,0.5));

				//漸層度
				half D2 = smoothstep(0.75,1.5,D)*3;

				D = smoothstep(0.5,1.5,D)*1.5;

                half4 col = i.color ;
                col.a *= saturate(D) ;
                
                
				half2 scruv = i.scrPos.xy/i.scrPos.w;
                half n =  smoothstep(0.15,0.75,distance(frac(half2(2,2)*scruv+_Time.y*half2(0.25,0.5)*1.5),0.5));
                half n2 =  smoothstep(0.15,0.5,distance(frac(half2(1.5,1.5)*scruv+_Time.y*half2(-0.5,0.75)*0.5),0.5));
                n+=n2/2;

                col.a -= saturate(1-n-col.a*2);
                col.rgb = col.rgb*saturate(n*n)*1/3 + col.rgb*2/3;
                
                clip(col.a - 0.005);
                
                _Color = lerp(_Color,_Color2,_h);

                return col*_Color;
            }
            ENDCG
        }
    }
}
