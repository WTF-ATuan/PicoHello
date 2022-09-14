Shader "GGDog/Arm_cover"
{
    Properties
    {
		_Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

		ZWrite Off
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
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
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
				half4 scrPos : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
				o.scrPos = ComputeScreenPos(o.vertex);  //抓取螢幕截圖的位置

                //CameraDistance
				o.scrPos.z = distance(_WorldSpaceCameraPos, unity_ObjectToWorld._m03_m13_m23);

                return o;
            }
            
            half4 _Color;
            
            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
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
                
                return col*_Color;
            }
            ENDCG
        }
    }
}
