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
                return o;
            }
            
            /*
			inline half2 unity_voronoi_noise_randomVector (half2 UV, half offset)
			{
			    half2x2 m = half2x2(15.27, 47.63, 99.41, 89.98);
			    UV = frac(sin(mul(UV, m)) * 46839.32);
				return half2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
			}

			void Unity_Voronoi_half(half2 UV, half AngleOffset, half CellDensity, out half Out, out half Cells)
			{
				half2 g = floor(UV * CellDensity);
				half2 f = frac(UV * CellDensity);
				half t = 8.0;
				half3 res = half3(8.0, 0.0, 0.0);

				for(int y=-1; y<=1; y++)
				{
					for(int x=-1; x<=1; x++)
					{
						half2 lattice = half2(x,y);
						half2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
						half d = distance(lattice + offset, f);
					    if(d < res.x)
					    {
						    res = half3(d, offset.x, offset.y);
						    Out = res.x;
						    Cells = res.y;
					    }
				    }
			    }
            }*/
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
                
                /*
				half2 scruv = i.scrPos.xy/i.scrPos.w;
			    half Out;
			    half Cells;
			    Unity_Voronoi_half(scruv+_Time.y*half2(0.25,0.15),_Time.y*5,7,Out,Cells);
			    half Out2;
			    half Cells2;
			    Unity_Voronoi_half(scruv-_Time.y*half2(0.25,0.25),_Time.y*7,5,Out2,Cells2);

                col.a -= saturate(1-Out*Out2*2-col.a*2);
                col.rgb = col.rgb*saturate(Out*Out2*Out*Out2)*1/3 + col.rgb*2/3;
                */
                clip(col.a - 0.005);
                
                _Color = lerp(_Color,_Color2,_h);

                return col*_Color;
            }
            ENDCG
        }
    }
}
