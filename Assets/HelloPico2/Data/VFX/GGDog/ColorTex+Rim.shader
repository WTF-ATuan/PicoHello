//模組的縮放
Shader "MyShader/ColorTex+Rim"
{

	Properties
	{
		_Texture ("Texture", 2D) = "white" {}
        [HDR]_RimColor("RimColor",Color) = (0,1,1,1)

		_RimGradient ("Rim Gradient", Range(0,1)) = 0.5
		_RimOffSet ("Rim OffSet", Range(-1,0)) = -0.75
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert    //定義vertex Shader 方法
			#pragma fragment frag  //定義 fragment Shader 方法

			//把輸入的參數匯入Pass
			sampler2D _Texture;    
			
			#include "UnityCG.cginc"
			float4 _RimColor;

			float _RimGradient;
			float _RimOffSet;

			//輸入端
			struct appdata
			{
				fixed4 vertex : POSITION; //後面的語義用來讓Unity知道這個變數的涵義
				fixed3 normal : NORMAL;
				fixed2 uv : TEXCOORD0;
				
				float4 color:COLOR;
			};

			//輸出端，把頂點傳給片段
			struct v2f
			{
				fixed4 vertex : SV_POSITION; //給Unity作特定處理，作透視除法、光柵化等等
				fixed2 uv : TEXCOORD0;

				float4	color:COLOR;
			};

			//方法
			v2f vert (appdata v)
			{
			    v2f o;

				o.vertex = UnityObjectToClipPos( v.vertex);   //與輸入的頂點做矩陣乘積
				
				o.uv = v.uv;

				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));   //ObjSpaceViewDir包含在"UnityCG.cginc"裡，正交鏡頭則是用UnityWorldSpaceViewDir
                float rim = 1 - (dot(viewDir,v.normal ));

                o.color = _RimColor*smoothstep(-_RimGradient,_RimGradient,rim+_RimOffSet);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target  //SV_Target: 渲染顯示在畫面上
			{
			   return tex2D(_Texture, i.uv) + i.color; //貼圖根據uv位置打在模型上
			}
			ENDCG
		}
		
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            ZWrite On
            ColorMask R
        }
	}
}
